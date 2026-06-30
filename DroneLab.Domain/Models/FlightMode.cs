namespace DroneLab.Domain.Models;

/// <summary>
/// Режимы полета полетного контроллера (совместимо с ArduPilot/PX4).
/// </summary>
public enum FlightMode
{
    Unknown = 0,
    Stabilize = 1,  // Ручной режим со стабилизацией горизонта
    AltHold = 2,    // Удержание высоты
    Loiter = 3,     // Удержание точки по GPS и высоты
    Guided = 4,     // Полет по точкам, отправленным с наземной станции
    Rtl = 5,        // Возврат на точку взлета (Return To Launch)
    Land = 6,       // Автоматическая посадка
    Auto = 7        // Выполнение автономной миссии
}