using System;
using System.Threading;
using System.Threading.Tasks;
using DroneLab.Application.Interfaces;
using DroneLab.Domain.Entities;

namespace DroneLab.Application.Services;

/// <summary>
/// Сервис управления состоянием дрона и координации потока телеметрии.
/// </summary>
public class DroneService
{
    private readonly IDroneReceiver _receiver;

    /// <summary>
    /// Активная сущность дрона, с которой работает станция.
    /// </summary>
    public Drone CurrentDrone { get; }

    public DroneService(IDroneReceiver receiver)
    {
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));

        // Создаем дефолтный экземпляр дрона (в будущем ID можно запрашивать из базы/конфига)
        CurrentDrone = new Drone(Guid.NewGuid(), "Drone-Lab Alpha");

        // Подписываем сущность дрона на события изменения данных от приемника
        _receiver.TelemetryReceived += CurrentDrone.UpdateTelemetry;
        _receiver.ConnectionStatusChanged += CurrentDrone.SetConnectionStatus;
    }

    /// <summary>
    /// Команда на запуск подключения к дрону (или симулятору).
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _receiver.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Команда на отключение от дрона.
    /// </summary>
    public async Task DisconnectAsync()
    {
        await _receiver.StopAsync();
    }
}