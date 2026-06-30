using System;

namespace DroneLab.Domain.Models;

/// <summary>
/// Полный снимок текущего состояния (телеметрии) дрона.
/// </summary>
/// <param name="Coordinates">Географические координаты (широта, долгота, высота)</param>
/// <param name="Pitch">Тангаж (наклон вперед/назад) в градусах</param>
/// <param name="Roll">Крен (наклон влево/вправо) в градусах</param>
/// <param name="Yaw">Рыскание (курс/направление носа) в градусах</param>
/// <param name="Speed">Путевая скорость в метрах в секунду</param>
/// <param name="BatteryVoltage">Текущее напряжение батареи в Вольтах</param>
/// <param name="BatteryPercentage">Остаток заряда батареи в процентах (0–100)</param>
/// <param name="Timestamp">Точное время фиксации данных</param>
public readonly record struct DroneTelemetry(
    GeoCoordinates Coordinates,
    double Pitch,
    double Roll,
    double Yaw,
    double Speed,
    double BatteryVoltage,
    int BatteryPercentage,
    FlightMode Mode, // <-- Добавили режим полета
    DateTime Timestamp);