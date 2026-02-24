# Architecture Diagram

This file contains a Mermaid diagram representing the high-level architecture of the `EmberOps` solution. You can view the rendered diagram on GitHub (or any tool that supports Mermaid).

```mermaid
flowchart LR

  %% Clients
  subgraph Clients
    Client[Web / Mobile Client]
  end

  Client -->|HTTP / WebSocket| API["Api Gateway (BFF)<br/>- REST endpoints<br/>- SignalR Hubs"]

  %% Read model hosted by API/BFF
  subgraph API_Read
    API -->|Read queries| BFFDB["BffDbContext (Read DB)"]
  end

  %% Commands to Order Service
  API -->|Commands / Requests| OrderService["Order Service<br/>- Domain (Order, OrderItem)<br/>- Command handlers"]

  OrderService -->|Writes| OrderDB[(Order SQL DB)]
  OrderService -->|Publish events| Bus[(RabbitMQ / MassTransit)]

  %% Consumers (should consume from the Bus, not from API)
  subgraph API_Consumers
    Bus -->|OrderCreated| OrderCreatedProjectionConsumer[OrderCreatedProjectionConsumer]
    Bus -->|OrderCreated| OrderCreatedNotificationConsumer[OrderCreatedNotificationConsumer]
  end

  OrderCreatedProjectionConsumer -->|Updates read model| BFFDB
  OrderCreatedNotificationConsumer -->|Notify via SignalR| API

  %% Other services
  Bus --> Inventory[Inventory Service]
  Bus --> Payments[Payments Service]
  Bus --> Auth[Auth Service]

  %% Shared
  Contracts[Contracts / DTOs] -->|Shared schema| Bus
  BuildingBlocks[BuildingBlocks<br/>Logging, Persistence Helpers] --> OrderService
  BuildingBlocks --> API

  %% Orchestration
  subgraph Orchestration
    DockerCompose[(docker-compose)]
    DockerCompose --> Bus
    DockerCompose --> OrderDB
    DockerCompose --> API
    DockerCompose --> OrderService
    DockerCompose --> Inventory
    DockerCompose --> Payments
    DockerCompose --> Auth
  end

  style API fill:#f9f,stroke:#333,stroke-width:1px
  style OrderService fill:#bbf,stroke:#333,stroke-width:1px
  style Bus fill:#ffd,stroke:#333,stroke-width:1px
  style OrderDB fill:#dfd,stroke:#333,stroke-width:1px
```

Short description

- `Api Gateway` (BFF) exposes REST endpoints and SignalR hubs to clients, hosts read-models (BffDbContext) for queries and consumes integration events.
- `Order Service` contains domain logic (orders, items), persists data to `Order SQL DB`, and publishes domain events via RabbitMQ/MassTransit.
- `RabbitMQ / MassTransit` is the message broker used for integration events (e.g. `OrderCreated`).
- Other services (Inventory, Payments, Auth) subscribe to events from the bus.
- `BuildingBlocks` contains shared helpers (logging, persistence, interceptors) used across services.
- `docker-compose` orchestrates local infrastructure (broker, databases, services).

Notes

- The diagram focuses on data and messaging flows. For deployment diagrams (Kubernetes, containers, scaling) create a separate view.
- To render the diagram, open this file on GitHub or paste the Mermaid block into a Mermaid live editor.
