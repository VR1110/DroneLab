using System;
using DroneLab.Domain.Models;

namespace DroneLab.Domain.Entities;

/// <summary>
/// Основная сущность дрона, управляющая его состоянием.
/// </summary>
public class Drone
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public bool IsConnected { get; private set; }
    public DroneTelemetry CurrentTelemetry { get; private set; }

    // События для реактивного обновления UI и логгеров
    public event Action<DroneTelemetry>? TelemetryUpdated;
    public event Action<bool>? ConnectionStatusChanged;

    public Drone(Guid id, string name)
    {
        Id = id;
        Name = name;
        IsConnected = false;
    }

    /// <summary>
    /// Обновляет текущую телеметрию дрона и уведомляет подписчиков.
    /// </summary>
    public void UpdateTelemetry(DroneTelemetry telemetry)
    {
        CurrentTelemetry = telemetry;
        TelemetryUpdated?.Invoke(telemetry);
    }

    /// <summary>
    /// Изменяет статус подключения и генерирует событие при его смене.
    /// </summary>
    public void SetConnectionStatus(bool isConnected)
    {
        if (IsConnected != isConnected)
        {
            IsConnected = isConnected;
            ConnectionStatusChanged?.Invoke(isConnected);
        }
    }
}