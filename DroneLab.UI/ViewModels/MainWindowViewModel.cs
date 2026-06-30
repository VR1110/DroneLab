using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DroneLab.UI.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly DispatcherTimer _simulationTimer;
    private readonly Random _random = new();
    private bool _isConnected;
    private bool _isRecording;

    // Стартовая точка (дрон на земле)
    private double _baseLat = 50.4501;
    private double _baseLng = 30.5234;
    private double _currentAlt = 0.0;
    private double _batteryVoltage = 25.2;

    // --- Свойства для привязки к интерфейсу (UI) ---

    private string _statusText = "Отключен";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    private string _flightModeText = "—";
    public string FlightModeText
    {
        get => _flightModeText;
        set => SetProperty(ref _flightModeText, value);
    }

    private string _coordinatesText = "—";
    public string CoordinatesText
    {
        get => _coordinatesText;
        set => SetProperty(ref _coordinatesText, value);
    }

    private double _altitude;
    public double Altitude
    {
        get => _altitude;
        set => SetProperty(ref _altitude, value);
    }

    private double _speed;
    public double Speed
    {
        get => _speed;
        set => SetProperty(ref _speed, value);
    }

    private double _pitch;
    public double Pitch
    {
        get => _pitch;
        set => SetProperty(ref _pitch, value);
    }

    private double _roll;
    public double Roll
    {
        get => _roll;
        set => SetProperty(ref _roll, value);
    }

    private double _yaw;
    public double Yaw
    {
        get => _yaw;
        set => SetProperty(ref _yaw, value);
    }

    private string _batteryText = "—";
    public string BatteryText
    {
        get => _batteryText;
        set => SetProperty(ref _batteryText, value);
    }

    // --- Команды для кнопок ---
    public ICommand ConnectCommand { get; }
    public ICommand StartFlightCommand { get; }
    public ICommand StopFlightCommand { get; }

    public MainWindowViewModel()
    {
        ConnectCommand = new RelayCommand(ConnectDevice);
        StartFlightCommand = new RelayCommand(StartFlight);
        StopFlightCommand = new RelayCommand(StopFlight);

        // Частота обновления симулятора — 5 Гц (раз в 200 мс)
        _simulationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _simulationTimer.Tick += SimulationTimer_Tick;
    }

    // --- Логика работы команд ---

    private void ConnectDevice()
    {
        if (_isConnected) return;

        _isConnected = true;
        StatusText = "Подключен";
        FlightModeText = "Loiter";

        // Выводим стартовые статические данные (дрон включен, но еще на земле)
        CoordinatesText = $"{_baseLat:F5}, {_baseLng:F5}";
        Altitude = 0.0;
        Speed = 0.0;
        Pitch = 0.0;
        Roll = 0.0;
        Yaw = 0.0;
        BatteryText = $"{_batteryVoltage:F1}V (100%)";
    }

    private void StartFlight()
    {
        if (!_isConnected)
        {
            StatusText = "Ошибка: Сначала подключите устройство!";
            return;
        }

        if (_isRecording) return;

        _isRecording = true;
        StatusText = "ЗАПИСЬ ПОЛЕТА В БД... (5 Гц)";

        // Включаем таймер — цифры побежали, имитируя полет
        if (!_simulationTimer.IsEnabled)
        {
            _simulationTimer.Start();
        }

        // ТУТ В БУДУЩЕМ: _droneService.StartFlightSessionAsync(...)
    }

    private void StopFlight()
    {
        if (!_isRecording) return;

        _isRecording = false;
        StatusText = "Подключен (Полет завершен)";

        // Выключаем таймер — цифры замерли на последней полученной точке
        _simulationTimer.Stop();

        // ТУТ В БУДУЩЕМ: await _droneService.StopFlightSessionAsync()
    }

    // --- Симулятор генерации пакетов данных ---
    private void SimulationTimer_Tick(object? sender, EventArgs e)
    {
        // Имитируем плавное движение по кругу
        _baseLat += (_random.NextDouble() - 0.5) * 0.0001;
        _baseLng += (_random.NextDouble() - 0.5) * 0.0001;
        CoordinatesText = $"{_baseLat:F5}, {_baseLng:F5}";

        // Набираем высоту до 120 метров
        if (_currentAlt < 120.0)
        {
            _currentAlt += 1.5;
        }
        Altitude = Math.Round(_currentAlt, 1);

        // Колебания датчиков в полете
        Speed = Math.Round(12.5 + (_random.NextDouble() * 2.0), 1);
        Pitch = Math.Round((_random.NextDouble() - 0.5) * 5.0, 1);
        Roll = Math.Round((_random.NextDouble() - 0.5) * 4.0, 1);

        double currentYaw = Yaw + 2.0;
        Yaw = Math.Round(currentYaw >= 360 ? 0 : currentYaw, 1);

        // Медленный разряд батареи 6S
        if (_batteryVoltage > 21.0)
        {
            _batteryVoltage -= 0.005;
        }
        int percentage = (int)((_batteryVoltage - 21.0) / (25.2 - 21.0) * 100);
        BatteryText = $"{_batteryVoltage:F1}V ({Math.Clamp(percentage, 0, 100)}%)";
    }
}