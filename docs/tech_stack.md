# Tech Stack

This file shows the primary technologies, libraries, and infrastructure used across the `EmberOps` solution. Render the Mermaid block in a Mermaid-capable viewer (GitHub, Mermaid Live Editor) to see the diagram.

```mermaid
flowchart TB
  subgraph Runtime
    DOTNET[".NET 10\nC# 14"]
  end

  subgraph Frameworks & Libraries
    EF["Entity Framework Core (EF Core)"]
    MASS["MassTransit"]
    SIGNALR["SignalR"]
    SERILOG["Serilog"]
  end

  subgraph Messaging & Integration
    RABBIT["RabbitMQ"]
    CONTRACTS["EmberOps.Contracts\n(Commands, Events, DTOs)"]
    OUTBOX["Inbox/Outbox Pattern\n(Outbox/Inbox)"]
  end

  subgraph Persistence
    SQL["SQL Server\n(Primary relational DB)"]
    EF_MIG["EF Migrations"]
  end

  subgraph Containers & Orchestration
    DOCKER["Docker & Docker Compose"]
    K8S["(Optional) Kubernetes"]
  end

  subgraph Testing & QA
    XUNIT["xUnit"]
    FLUENT["FluentAssertions"]
  end

  subgraph CI_CD
    GHA["GitHub Actions (recommended)"]
  end

  subgraph Observability
    SEQ["Seq / ELK / Log store"]
    METRICS["Prometheus / Grafana (metrics)"]
  end

  subgraph Developer Tools
    VS["Visual Studio / VS Code"]
    DOTNET_CLI["dotnet CLI"]
  end

  %% Connections
  DOTNET --> EF
  DOTNET --> MASS
  DOTNET --> SIGNALR
  DOTNET --> SERILOG

  MASS --> RABBIT
  MASS --> CONTRACTS
  CONTRACTS --> RABBIT

  EF --> SQL
  EF_MIG --> SQL

  DOCKER --> RABBIT
  DOCKER --> SQL
  DOCKER --> DOTNET

  GHA --> DOCKER
  GHA --> DOTNET
  GHA --> XUNIT

  SERILOG --> SEQ
  DOTNET --> METRICS

  VS --> DOTNET
  DOTNET_CLI --> DOTNET
```

Short notes

- Runtime: the solution targets `dotnet 10` and leverages C# 14 language features.
- Core frameworks: `EF Core` for persistence, `MassTransit` + `RabbitMQ` for messaging, `SignalR` for real-time notifications, and `Serilog` for structured logging.
- Persistence: SQL Server is used as the primary relational database; EF Core migrations are included in service projects.
- Messaging: domain commands/events are defined in `EmberOps.Contracts` and exchanged over RabbitMQ using MassTransit. The Inbox/Outbox pattern is used where applicable.
- Containers: Docker and `docker-compose` orchestrate local development. Services can be deployed to Kubernetes in production.
- Testing: unit tests use `xUnit` and `FluentAssertions` (see `EmberOps.OrderService.Tests`).
- CI/CD: recommend `GitHub Actions` (adjust to your CI provider).
- Observability: Serilog sinks to log stores (Seq/ELK) and metrics pipelines (Prometheus/Grafana) are recommended.

To render the diagram, open this file on GitHub or paste the Mermaid block into a Mermaid live editor.
