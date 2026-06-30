using System.Data;
using Dapper;
using DroneLab.Application.Interfaces;
using DroneLab.Domain.Entities;
using DroneLab.Infrastructure.Data;

namespace DroneLab.Infrastructure.Repositories;

public class DapperFlightRepository : IFlightRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public DapperFlightRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task StartSessionAsync(FlightSession session)
    {
        using var db = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO FlightSessions (Id, StartTime, EndTime, DroneName)
            VALUES (@Id, @StartTime, @EndTime, @DroneName);";

        await db.ExecuteAsync(sql, new
        {
            Id = session.Id.ToString(),
            StartTime = session.StartTime.ToString("o"), // ISO 8601 формат
            session.EndTime,
            session.DroneName
        });
    }

    public async Task EndSessionAsync(Guid sessionId, DateTime endTime)
    {
        using var db = await _connectionFactory.CreateConnectionAsync();
        const string sql = "UPDATE FlightSessions SET EndTime = @EndTime WHERE Id = @Id;";
        await db.ExecuteAsync(sql, new { Id = sessionId.ToString(), EndTime = endTime.ToString("o") });
    }

    public async Task SaveTelemetryBatchAsync(IEnumerable<TelemetryRecord> records)
    {
        if (!records.Any()) return;

        using var db = await _connectionFactory.CreateConnectionAsync();
        using var transaction = db.BeginTransaction();

        const string sql = @"
            INSERT INTO TelemetryRecords (FlightSessionId, Timestamp, Latitude, Longitude, Altitude, Speed, Mode)
            VALUES (@FlightSessionId, @Timestamp, @Latitude, @Longitude, @Altitude, @Speed, @Mode);";

        try
        {
            // Dapper автоматически выполнит запрос в цикле для каждого объекта в коллекции внутри транзакции
            await db.ExecuteAsync(sql, records.Select(r => new {
                FlightSessionId = r.FlightSessionId.ToString(),
                Timestamp = r.Timestamp.ToString("o"),
                r.Latitude,
                r.Longitude,
                r.Altitude,
                r.Speed,
                Mode = (int)r.Mode
            }), transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<List<FlightSession>> GetAllSessionsAsync()
    {
        using var db = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM FlightSessions ORDER BY StartTime DESC;";
        var result = await db.QueryAsync<dynamic>(sql);

        return result.Select(s => new FlightSession
        {
            Id = Guid.Parse(s.Id),
            StartTime = DateTime.Parse(s.StartTime),
            EndTime = s.EndTime != null ? DateTime.Parse(s.EndTime) : null,
            DroneName = s.DroneName
        }).ToList();
    }

    public async Task<List<TelemetryRecord>> GetTelemetryForSessionAsync(Guid sessionId)
    {
        using var db = await _connectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM TelemetryRecords WHERE FlightSessionId = @SessionId ORDER BY Timestamp ASC;";
        var result = await db.QueryAsync<dynamic>(sql, new { SessionId = sessionId.ToString() });

        return result.Select(t => new TelemetryRecord
        {
            Id = t.Id,
            FlightSessionId = Guid.Parse(t.FlightSessionId),
            Timestamp = DateTime.Parse(t.Timestamp),
            Latitude = t.Latitude,
            Longitude = t.Longitude,
            Altitude = (float)t.Altitude,
            Speed = (float)t.Speed,
            Mode = (DroneLab.Domain.Models.FlightMode)t.Mode
        }).ToList();
    }
}