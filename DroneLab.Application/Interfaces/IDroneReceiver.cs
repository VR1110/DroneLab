using System;
using System.Threading;
using System.Threading.Tasks;
using DroneLab.Domain.Models;

namespace DroneLab.Application.Interfaces;

/// <summary>
/// Интерфейс приемника данных с дрона (например, через MAVLink, MSP или лог-файлы).
/// Конкретная реализация (работа с сокетами/портами) будет в слое Infrastructure.
/// </summary>
public interface IDroneReceiver
{
    /// <summary>
    /// Событие, срабатывающее при получении свежего снимка телеметрии.
    /// </summary>
    event Action<DroneTelemetry>? TelemetryReceived;

    /// <summary>
    /// Событие изменения статуса связи с дроном.
    /// </summary>
    event Action<bool>? ConnectionStatusChanged;

    /// <summary>
    /// Текущее состояние подключения к источнику данных.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Запуск прослушивания порта/сокета для приема данных.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Остановка приема данных и закрытие соединений.
    /// </summary>
    Task StopAsync();
}