# Components Diagram

This file contains a Mermaid component-style diagram describing the main components in the EmberOps solution and how they interact. Render the Mermaid block in a Mermaid-capable viewer (GitHub, Mermaid Live Editor) to see the diagram.

```mermaid
flowchart TB
  %% API Gateway components
  subgraph ApiGateway [EmberOps.ApiGateway]
    direction TB
    A1["OrdersEndPoints\n(HTTP Controllers / Endpoints)"]
    A2["NotificationsHub\n(SignalR Hub)"]
    A3["DashboardHub\n(SignalR Hub)"]
    A4["BffDbContext\n(Read Model / EF Core)"]
    A5["OrderCreatedProjectionConsumer\n(Projection consumer)"]
    A6["OrderCreatedNotificationConsumer\n(Notification consumer)"]
    A7["CreateOrderHttpRequest\n(HTTP DTO)"]
  end

  %% Order Service components
  subgraph OrderService [EmberOps.OrderService]
    direction TB
    S1["Order Domain\n(Aggregate root)"]
    S2["OrderItem Domain\n(Value object / entity)"]
    S3["CreateOrderCommandHandler\n(Command handler)"]
    S4["CreateOrderConsumer\n(MassTransit consumer)"]
    S5["OrderDbContext\n(Persistence / EF Core)"]
    S6["Migrations\n(EF Core migrations)"]
  end

  %% Shared / Infra
  subgraph Shared [Shared / Infrastructure]
    direction TB
    C1["EmberOps.Contracts\n(Commands, Events, DTOs)"]
    C2["BuildingBlocks\n(Logging / Persistence / Interceptors)"]
    C3["RabbitMQ / MassTransit\n(Message Broker & framework)"]
    C4["Inbox/Outbox\n(Outbox / Inbox configured)"]
  end

  %% External services
  subgraph External [Other Services]
    direction TB
    E1[Inventory Service]
    E2[Payments Service]
    E3[Auth Service]
  end

  %% Interactions
  A1 -->|"CreateOrderCommand (via Contracts)"| C1
  A1 -->|Send Request / Publish| C3

  C3 -->|Deliver CreateOrderCommand| S4
  S4 -->|Invoke| S3
  S3 -->|Persist| S5
  S3 -->|Publish OrderCreatedEvent| C3

  C3 -->|OrderCreatedEvent| A5
  A5 -->|Update| A4
  A6 -->|Notify| A2

  C3 --> E1
  C3 --> E2
  C3 --> E3

  A4 -->|Read queries| A1

  C2 --> S3
  C2 --> A5
  C4 --> S3
  C4 --> A5

  style ApiGateway fill:#f2f8ff,stroke:#333
  style OrderService fill:#fff7e6,stroke:#333
  style Shared fill:#f7fff2,stroke:#333
  style External fill:#f8f0ff,stroke:#333
```

Short explanation

- The `ApiGateway` project exposes HTTP endpoints (`OrdersEndPoints`) and SignalR hubs (`NotificationsHub`, `DashboardHub`). It maintains a read model via `BffDbContext` and consumes integration events to keep projections and push notifications.
- The `OrderService` contains the domain model (`Order`, `OrderItem`), the command handler (`CreateOrderCommandHandler`), MassTransit consumer (`CreateOrderConsumer`), and persistence (`OrderDbContext`, migrations).
- `EmberOps.Contracts` holds shared commands, events and DTOs used across services.
- `BuildingBlocks` contains cross-cutting concerns like logging, EF Core interceptors (auditing), and persistence helpers used by services.
- `RabbitMQ / MassTransit` is the message bus used to send commands and publish integration events. Other services (Inventory, Payments, Auth) subscribe to those events.

How to view

- Open this file on GitHub to render the Mermaid diagram, or paste the Mermaid block into a Mermaid Live Editor.
