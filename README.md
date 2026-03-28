# FixNet Backend 🛠️

A robust service management system built with **.NET 10**, focusing on **Domain-Driven Design (DDD)** and **CQRS** principles.

## 🚀 Tech Stack
- **Framework:** .NET 10 (C#)
- **Architecture:** Clean Architecture, CQRS
- **Persistence:** SQL Server + Entity Framework Core
- **Identity:** External Identity Provider Integration (Keycloak/Auth0)
- **Resilience:** Resilience (Resilience Pipelines for Migrations)
- **Communication:** Minimal APIs
- **Validation:** Domain-level Value Objects & RFC 7231 Problem Details

## 🏗️ Architecture Overview
The project follows **Clean Architecture** layers:
- **Domain:** Business logic, Entities, Aggregates, and Value Objects.
- **Application:** Command/Query Handlers, DTOs, and Interfaces.
- **Infrastructure:** Database persistence (Repositories), Identity integration, and external services.
- **API:** Minimal API endpoints, Middleware, and Idempotency filters.

## 🛠️ Key Features
- **Idempotency Support:** Prevents duplicate processing using the `X-Idempotency-Key` header.
- **Self-Healing Migrations:** Uses **Resilience** to retry database migrations during startup, ensuring high availability.
- **Compensation Pattern:** Automatic cleanup (deletion) of external identity accounts if the local database transaction fails.
- **Strict Domain Validation:** Use of **Value Objects** (Email, Phone, Names) to ensure data integrity at the core level.

## 🚦 Getting Started
1. Configure your `ConnectionStrings` in `appsettings.json`.
2. Ensure your External Identity Provider (e.g., Keycloak) is reachable.
3. Run the project:
   ```bash
   dotnet run --project FixNet.API
