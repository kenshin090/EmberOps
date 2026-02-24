# EmberOps

EmberOps is a sample microservices-based e-commerce demo comprised of several .NET services, an API Gateway, and background messaging integration. It demonstrates common patterns including domain-driven design, messaging with RabbitMQ/MassTransit, persistence, EF Core migrations, and SignalR for real-time notifications.

## Services

- `EmberOps.ApiGateway` - BFF / API gateway that aggregates read models and exposes HTTP endpoints and SignalR hubs.
- `EmberOps.OrderService` - Order bounded context implementing domain logic, commands/consumers and persistence.
- `EmberOps.AuthService` - Authentication service (sample).
- `EmberOps.InventoryService` - Inventory demo service.
- `EmberOps.PaymentsService` - Payments demo service.

Other projects include shared contracts (`EmberOps.Contracts`), building blocks, and persistence helpers.

## Tech stack

- .NET 10
- C# 14
- Entity Framework Core (SqlServer persistence helpers present)
- MassTransit + RabbitMQ for messaging
- Serilog for structured logging
- SignalR for real-time notifications
- Docker & Docker Compose for local orchestration

## Getting started

Prerequisites

- .NET 10 SDK
- Docker & Docker Compose (to run services together)
- A local SQL Server instance if not using Docker for the database

Run the full stack with Docker Compose

1. Build and run:

   `docker-compose up --build`

This will build images and start the configured services. Check the `docker-compose.yml` for ports and dependencies.

Run services locally without Docker

1. Build the solution:

   `dotnet build`

2. Run an individual service (example):

   `dotnet run --project EmberOps.ApiGateway/EmberOps.ApiGateway.csproj`

3. Ensure required infrastructure (SQL Server, RabbitMQ) is available and configured via each service's `appsettings.json` or environment variables.

Database migrations

- The `EmberOps.OrderService` project contains EF Core migrations in the `Migrations` folder. To apply migrations locally using the .NET CLI:

  `dotnet ef database update --project EmberOps.OrderService/EmberOps.OrderService.csproj --startup-project EmberOps.OrderService/EmberOps.OrderService.csproj`

Adjust connection strings via environment variables or `appsettings.json` as needed.

Running tests

Run unit tests from solution root:

`dotnet test`

Project structure

- `EmberOps.ApiGateway` - API / Read models / Hubs
- `EmberOps.OrderService` - Domain, handlers, consumers, EF migrations
- `EmberOps.Contracts` - Shared DTOs, commands, events
- `EmberOps.BuildingBlocks` - Logging, persistence helpers

Integration notes

- Messaging uses RabbitMQ and MassTransit. Consumers and endpoints are configured in `Infrastructure/Messaging` and `Messaging` helper classes.
- Read models in the API Gateway are kept in a separate persistence context for querying.

Contributing

1. Fork the repository and open a PR with a clear summary of changes.
2. Keep changes focused and add/update tests for behavior changes.

License

This repository follows the license included with the project (check the repository root for a `LICENSE` file). If none exists, contact the maintainer/owner for guidance.

Questions or issues

Open an issue in the repository with reproduction steps and logs where appropriate.

The architectural diagrams design can be found in the docs folder