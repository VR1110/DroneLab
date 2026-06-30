# DroneLab Ground Control

**DroneLab** is a cross-platform Ground Control Station (GCS) application designed for real-time UAV telemetry monitoring. The project is built on the **.NET 10** platform using the **Avalonia UI** framework and strictly follows **Clean Architecture** principles.

---

## 📱 Application Interface

<p align="center">
  <img src="images/main_ui.png" alt="DroneLab Telemetry UI" width="450"/>
</p>

---

## 🚀 Key Features

*   **Real-Time Telemetry Monitoring:** Live tracking of UAV spatial orientation (Pitch, Roll, Yaw), GPS coordinates, altitude, speed, and battery status.
*   **High-Speed Connectivity:** Engineered to support stable data links over high-bandwidth wireless channels (including 5 GHz bands).
*   **Persistent Flight Logging:** Automatic, non-volatile logging of all telemetry parameters into a local database (SQLite) for post-mission analysis.
*   **State Management:** Intuitive UI controls for device connection, alongside one-click start/stop commands for flight data recording.

---

## 🏗️ Architecture Overview

The application is decoupled into independent layers adhering to **Clean Architecture** guidelines, ensuring high testability and easily replaceable components:

*   **DroneLab.Domain:** The core of the system. Contains fundamental telemetry entities, enterprise business rules, and basic data models (coordinates, orientation angles, battery metrics). It has zero external dependencies.
*   **DroneLab.Application:** The business logic layer. Defines core use cases such as telemetry stream processing, flight session handling, and abstraction interfaces (Dependency Inversion) for data persistence.
*   **DroneLab.Infrastructure:** Infrastructure concerns. Implements database access (SQLite via Entity Framework Core) and low-level hardware communication interfaces for data ingestion.
*   **DroneLab.UI (Avalonia UI):** The presentation layer built on the **MVVM** pattern. Leveraging Avalonia UI, it delivers a high-performance, cross-platform user interface.

---

## 🛠️ Tech Stack

*   **Platform:** .NET 10 (C#)
*   **UI Framework:** Avalonia UI (MVVM)
*   **Database:** SQLite
*   **Architectural Pattern:** Clean Architecture

---

## 🏁 Quick Start

To run the application locally, ensure you have the .NET 10 SDK installed.

1. Clone the repository:
   ```bash
   git clone [https://github.com/VR1110/DroneLab.git](https://github.com/VR1110/DroneLab.git)
   ```
2. Navigate to the project directory:
   ```bash
   cd DroneLab
   ```
3. Run the UI project:
   ```bash
   dotnet run --project DroneLab.UI
   ```