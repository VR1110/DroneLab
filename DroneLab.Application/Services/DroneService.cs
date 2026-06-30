using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using DroneLab.Application.Interfaces;
using DroneLab.Domain.Entities;
using DroneLab.Domain.Models;

namespace DroneLab.Application.Services;

public class DroneService
{
    private readonly IDroneReceiver _receiver;
    private readonly IFlightRepository _flightRepository;

    private Guid? _currentSessionId;
    private readonly ConcurrentQueue<TelemetryRecord> _telemetryBuffer = new();
    private readonly int _batchSize = 20;

    public event Action<DroneTelemetry>? TelemetryReceived;

    // Свойство для хранения текущего подключенного устройства
    public Drone? CurrentDrone { get; set; }

    public DroneService(IDroneReceiver receiver, IFlightRepository flightRepository)
    {
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        _flightRepository = flightRepository ?? throw new ArgumentNullException(nameof(flightRepository));

        _receiver.TelemetryReceived += OnTelemetryDataReceived;
    }

    public async Task ConnectAsync()
    {
        await _receiver.StartAsync();
    }

    public async Task StartFlightSessionAsync(string droneName)
    {
        if (_currentSessionId != null) return;

        var session = new FlightSession
        {
            Id = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            DroneName = droneName
        };

        await _flightRepository.StartSessionAsync(session);
        _currentSessionId = session.Id;
    }

    public async Task StopFlightSessionAsync()
    {
        if (_currentSessionId == null) return;

        await FlushBufferAsync();

        await _flightRepository.EndSessionAsync(_currentSessionId.Value, DateTime.UtcNow);
        _currentSessionId = null;
    }

    private async void OnTelemetryDataReceived(DroneTelemetry telemetry)
    {
        // Пробрасываем событие дальше к UI слою
        TelemetryReceived?.Invoke(telemetry);

        if (_currentSessionId != null)
        {
            var record = new TelemetryRecord
            {
                FlightSessionId = _currentSessionId.Value,
                Timestamp = DateTime.UtcNow,
                Latitude = telemetry.Coordinates.Latitude,
                Longitude = telemetry.Coordinates.Longitude,
                Altitude = (float)telemetry.Coordinates.Altitude,
                Speed = (float)telemetry.Speed,
                Mode = telemetry.Mode
            };

            _telemetryBuffer.Enqueue(record);

            if (_telemetryBuffer.Count >= _batchSize)
            {
                await FlushBufferAsync();
            }
        }
    }

    private async Task FlushBufferAsync()
    {
        var recordsToWrite = new List<TelemetryRecord>();

        while (_telemetryBuffer.TryDequeue(out var record))
        {
            recordsToWrite.Add(record);
        }

        if (recordsToWrite.Any())
        {
            try
            {
                await _flightRepository.SaveTelemetryBatchAsync(recordsToWrite);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка записи пакета телеметрии: {ex.Message}");
            }
        }
    }
}