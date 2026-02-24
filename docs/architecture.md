# Architecture Diagram

This file contains a Mermaid diagram representing the high-level architecture of the `EmberOps` solution. You can view the rendered diagram on GitHub (or any tool that supports Mermaid).

```mermaid
flowchart LR
  subgraph Clients
    A[Web / Mobile Client]
  end

  A -->|HTTP / WebSocket| API[Api Gateway (BFF)\n- REST endpoints\n- SignalR Hubs]

  subgraph API_Read
    API -->|Read queries| BFFDB[BffDbContext (Read DB)]
    API -->|SignalR notifications| Clients[Web / Mobile Client]
  end

  API -->|Commands / Requests| OrderService[Order Service\n- Domain (Order, OrderItem)\n- Command handlers]
  OrderService -->|Writes| OrderDB[(Order SQL DB)]
  OrderService -->|Publish events| Bus[(RabbitMQ / MassTransit)]

  subgraph Consumers
    API -->|Consume: OrderCreated| OrderCreatedProjectionConsumer
    API -->|Consume: OrderCreated| OrderCreatedNotificationConsumer
  end

  Bus --> Inventory[Inventory Service]
  Bus --> Payments[Payments Service]
  Bus --> Auth[Auth Service]

  Contracts[Contracts / DTOs] -->|Shared schema| Bus
  BuildingBlocks[BuildingBlocks\nLogging, Persistence Helpers] --> OrderService
  BuildingBlocks --> API

  subgraph Orchestration
    DockerCompose[(docker-compose)]
    DockerCompose --> Bus
    DockerCompose --> OrderDB
    DockerCompose --> API
    DockerCompose --> OrderService
    DockerCompose --> Inventory
    DockerCompose --> Payments
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
