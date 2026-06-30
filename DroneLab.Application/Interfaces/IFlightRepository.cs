using DroneLab.Domain.Entities;

namespace DroneLab.Application.Interfaces;

public interface IFlightRepository
{
    Task StartSessionAsync(FlightSession session);
    Task EndSessionAsync(Guid sessionId, DateTime endTime);
    Task SaveTelemetryBatchAsync(IEnumerable<TelemetryRecord> records);
    Task<List<FlightSession>> GetAllSessionsAsync();
    Task<List<TelemetryRecord>> GetTelemetryForSessionAsync(Guid sessionId);
}