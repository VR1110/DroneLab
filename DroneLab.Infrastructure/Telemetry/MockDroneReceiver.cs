using System;
using System.Threading;
using System.Threading.Tasks;
using DroneLab.Application.Interfaces;
using DroneLab.Domain.Models;

namespace DroneLab.Infrastructure.Telemetry;

/// <summary>
/// Имитатор приемника телеметрии. Генерирует реалистичные полетные данные для отладки UI.
/// </summary>
public class MockDroneReceiver : IDroneReceiver
{
    public event Action<DroneTelemetry>? TelemetryReceived;
    public event Action<bool>? ConnectionStatusChanged;

    public bool IsConnected { get; private set; }

    private CancellationTokenSource? _cts;
    private Task? _generationTask;
    private readonly Random _random = new();

    // Начальные параметры виртуального дрона
    private double _latitude = 50.4501;  // Киев
    private double _longitude = 30.5234;
    private double _altitude = 0.0;
    private double _batteryVoltage = 25.2; // 6S батарея (полный заряд)
    private int _batteryPercentage = 100;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected) return Task.CompletedTask;

        IsConnected = true;
        ConnectionStatusChanged?.Invoke(true);

        _cts = new CancellationTokenSource();
        _generationTask = Task.Run(() => StartTelemetryLoop(_cts.Token));

        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (!IsConnected) return;

        IsConnected = false;
        ConnectionStatusChanged?.Invoke(false);

        if (_cts != null)
        {
            await _cts.CancelAsync();
            try
            {
                if (_generationTask != null) await _generationTask;
            }
            catch (OperationCanceledException) { }
            finally
            {
                _cts.Dispose();
                _cts = null;
                _generationTask = null;
            }
        }
    }

    private async Task StartTelemetryLoop(CancellationToken token)
    {
        double angle = 0;

        while (!token.IsCancellationRequested)
        {
            // Имитируем небольшое движение по кругу и набор высоты
            angle += 0.05;
            _latitude += Math.Sin(angle) * 0.0001;
            _longitude += Math.Cos(angle) * 0.0001;

            if (_altitude < 120.0) _altitude += 1.5; // Набираем 120 метров

            // Имитируем раскачку дрона ветром (Pitch, Roll, Yaw)
            double pitch = Math.Sin(angle * 2) * 5.0;
            double roll = Math.Cos(angle * 1.5) * 4.0;
            double yaw = (angle * 10) % 360;

            // Медленный разряд батареи
            if (_batteryVoltage > 21.0) _batteryVoltage -= 0.005;
            _batteryPercentage = (int)((_batteryVoltage - 21.0) / (25.2 - 21.0) * 100);

            var telemetry = new DroneTelemetry(
                Coordinates: new GeoCoordinates(_latitude, _longitude, _altitude),
                Pitch: Math.Round(pitch, 2),
                Roll: Math.Round(roll, 2),
                Yaw: Math.Round(yaw, 2),
                Speed: 12.5 + _random.NextDouble() * 2, // Скорость около 12-14 м/с
                BatteryVoltage: Math.Round(_batteryVoltage, 2),
                BatteryPercentage: Math.Clamp(_batteryPercentage, 0, 100),
                Mode: FlightMode.Loiter, // Режим удержания точки
                Timestamp: DateTime.Now
            );

            // Отправляем телеметрию вверх по архитектурным слоям
            TelemetryReceived?.Invoke(telemetry);

            // Частота обновления данных — 5 Гц (раз в 200 мс)
            await Task.Delay(200, token);
        }
    }
}