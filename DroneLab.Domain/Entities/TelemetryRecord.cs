using DroneLab.Domain.Models;

namespace DroneLab.Domain.Entities;

public class TelemetryRecord
{
    public long Id { get; set; } // Использование long для автоинкремента в БД
    public Guid FlightSessionId { get; set; }

    public DateTime Timestamp { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float Altitude { get; set; }
    public float Speed { get; set; }
    public FlightMode Mode { get; set; }
}