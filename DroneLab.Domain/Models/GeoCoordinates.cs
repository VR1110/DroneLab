namespace DroneLab.Domain.Models;

/// <summary>
/// Географические координаты дрона.
/// </summary>
/// <param name="Latitude">Широта в градусах</param>
/// <param name="Longitude">Долгота в градусах</param>
/// <param name="Altitude">Высота над уровнем моря (или точкой взлета) в метрах</param>
public readonly record struct GeoCoordinates(
    double Latitude,
    double Longitude,
    double Altitude);