namespace DroneLab.Domain.Entities;

public class FlightSession
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string DroneName { get; set; } = string.Empty;

    // Навигационное свойство для связи один-ко-многим
    public List<TelemetryRecord> TelemetryRecords { get; set; } = new();
}