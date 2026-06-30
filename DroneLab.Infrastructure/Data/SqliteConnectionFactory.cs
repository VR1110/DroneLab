using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace DroneLab.Infrastructure.Data;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Включаем режим WAL для высокой производительности записи
        await connection.ExecuteAsync("PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;");

        return connection;
    }

    public async Task InitializeDatabaseAsync()
    {
        using var connection = await CreateConnectionAsync();

        const string createTablesSql = @"
            CREATE TABLE IF NOT EXISTS FlightSessions (
                Id TEXT PRIMARY KEY,
                StartTime TEXT NOT NULL,
                EndTime TEXT,
                DroneName TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS TelemetryRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FlightSessionId TEXT NOT NULL,
                Timestamp TEXT NOT NULL,
                Latitude REAL NOT NULL,
                Longitude REAL NOT NULL,
                Altitude REAL NOT NULL,
                Speed REAL NOT NULL,
                Mode INTEGER NOT NULL,
                FOREIGN KEY(FlightSessionId) REFERENCES FlightSessions(Id) ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS IX_Telemetry_SessionId ON TelemetryRecords(FlightSessionId);
        ";

        await connection.ExecuteAsync(createTablesSql);
    }
}