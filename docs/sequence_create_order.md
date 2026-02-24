# Sequence Diagram: Create Order Command Flow

This file contains a Mermaid sequence diagram that shows the command flow when a client creates an order in the EmberOps solution.

```mermaid
sequenceDiagram
    participant Client as Web/Mobile Client
    participant APIGW as Api Gateway (BFF)
    participant Bus as RabbitMQ / MassTransit
    participant Consumer as OrderService CreateOrderConsumer
    participant Handler as CreateOrderCommandHandler
    participant OrderDb as Order SQL DB
    participant Projection as ApiGateway OrderCreatedProjectionConsumer
    participant Notification as ApiGateway OrderCreatedNotificationConsumer
    participant Hub as SignalR Hub (NotificationsHub)

    Client->>APIGW: POST /orders (CreateOrderHttpRequest)
    APIGW->>Bus: Send/Request CreateOrderCommand
    Bus->>Consumer: Deliver CreateOrderCommand
    Consumer->>Handler: Invoke CreateOrderCommandHandler
    Handler->>OrderDb: Persist Order (EF Core SaveChanges)
    Handler->>Bus: Publish OrderCreatedIntegrationEvent

    note over Bus,Projection: Event distribution
    Bus->>Projection: OrderCreatedIntegrationEvent
    Projection->>APIGW: Update BFF read-model (BffDbContext)

    Bus->>Notification: OrderCreatedIntegrationEvent
    Notification->>Hub: Push notification via SignalR
    Hub-->>Client: Real-time notification (OrderCreated)

    Handler-->>Bus: Respond CreateOrderResponse (if using request/response)
    Bus-->>APIGW: Deliver CreateOrderResponse
    APIGW-->>Client: HTTP 201 Created (CreateOrderResponse)
```

Notes

- The API Gateway may use MassTransit's request/response pattern to wait for a `CreateOrderResponse` before returning an HTTP response. If not, it can return immediately with 202 Accepted while the command is processed asynchronously.
- `OrderCreatedIntegrationEvent` is consumed by API Gateway projection consumer to keep the read-model in sync and by a notification consumer to push real-time updates to connected clients via SignalR.
- Persistence occurs inside the Order Service domain (EF Core) during command handling.

To render the diagram copy the Mermaid block into a Mermaid renderer or view this file on GitHub.
