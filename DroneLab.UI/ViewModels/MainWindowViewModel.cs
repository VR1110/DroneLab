using System;
using Avalonia.Threading;
using DroneLab.Application.Interfaces;
using DroneLab.Application.Services;
using DroneLab.Domain.Models;
using DroneLab.Infrastructure.Telemetry;

namespace DroneLab.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly DroneService _droneService;

    // Внутренние поля
    private string _statusText = "Disconnected";
    private string _coordinatesText = "Lat: 0.0, Lon: 0.0";
    private double _altitude;
    private double _pitch;
    private double _roll;
    private double _yaw;
    private double _speed;
    private string _batteryText = "0V (0%)";
    private string _flightModeText = "Unknown";

    // Публичные свойства для связывания с XAML (теперь с корректным get и встроенным SetProperty)
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
    public string CoordinatesText { get => _coordinatesText; set => SetProperty(ref _coordinatesText, value); }
    public double Altitude { get => _altitude; set => SetProperty(ref _altitude, value); }
    public double Pitch { get => _pitch; set => SetProperty(ref _pitch, value); }
    public double Roll { get => _roll; set => SetProperty(ref _roll, value); }
    public double Yaw { get => _yaw; set => SetProperty(ref _yaw, value); }
    public double Speed { get => _speed; set => SetProperty(ref _speed, value); }
    public string BatteryText { get => _batteryText; set => SetProperty(ref _batteryText, value); }
    public string FlightModeText { get => _flightModeText; set => SetProperty(ref _flightModeText, value); }

    public MainWindowViewModel()
    {
        // Инициализируем наш Mock-приемник и сервис управления
        IDroneReceiver receiver = new MockDroneReceiver();
        _droneService = new DroneService(receiver);

        // Подписываемся на события сущности дрона
        _droneService.CurrentDrone.TelemetryUpdated += OnTelemetryUpdated;
        _droneService.CurrentDrone.ConnectionStatusChanged += OnConnectionStatusChanged;

        // Запускаем генерацию телеметрии
        _ = _droneService.ConnectAsync();
    }

    private void OnTelemetryUpdated(DroneTelemetry telemetry)
    {
        // Перенаправляем обновление свойств в UI-поток Авалонии
        Dispatcher.UIThread.Post(() =>
        {
            CoordinatesText = $"Lat: {telemetry.Coordinates.Latitude:F5}, Lon: {telemetry.Coordinates.Longitude:F5}";
            Altitude = telemetry.Coordinates.Altitude;
            Pitch = telemetry.Pitch;
            Roll = telemetry.Roll;
            Yaw = telemetry.Yaw;
            Speed = telemetry.Speed;
            BatteryText = $"{telemetry.BatteryVoltage:F1}V ({telemetry.BatteryPercentage}%)";
            FlightModeText = telemetry.Mode.ToString().ToUpper();
        });
    }

    private void OnConnectionStatusChanged(bool isConnected)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusText = isConnected ? "CONNECTED" : "DISCONNECTED";
        });
    }
}