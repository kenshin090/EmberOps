# Database Diagrams

This document contains Mermaid ER diagrams for the main persistence contexts in the solution: the `Order` write database (Order Service) and the `BFF` read database (API Gateway).

Note: these diagrams are inferred from the project's domain and read-model classes (`Order`, `OrderItem`, `OrderReadModel`, `OrderItemReadModel`, `InboxMessage`). Adjust fields to match the exact schema in your `DbContext`/migrations if needed.

## Order Service (write DB)

```mermaid
erDiagram
    ORDERS {
        GUID Id PK "Order.Id (Guid)"
        DATETIME CreatedAt
        DATETIME UpdatedAt
        STRING Status
        DECIMAL TotalAmount
        DATETIME CancelledAt
    }

    ORDER_ITEMS {
        GUID Id PK "OrderItem.Id (Guid)"
        GUID OrderId FK
        STRING ProductId
        STRING ProductName
        DECIMAL UnitPrice
        INT Quantity
        DECIMAL LineTotal
    }

    ORDERS ||--o{ ORDER_ITEMS : has
```

Short notes:
- `ORDERS.TotalAmount` is typically derived as the sum of `ORDER_ITEMS.LineTotal`.
- `ORDER_ITEMS.LineTotal` = `UnitPrice * Quantity` (may be stored or calculated).
- Additional audit fields (CreatedBy/UpdatedBy) may exist depending on `IAuditable` interceptors.

## BFF Read Model (API Gateway)

```mermaid
erDiagram
    ORDER_READS {
        GUID Id PK "OrderReadModel.Id (Guid)"
        DATETIME CreatedAt
        STRING Status
        DECIMAL TotalAmount
        INT ItemCount
    }

    ORDER_ITEM_READS {
        GUID Id PK "OrderItemReadModel.Id (Guid)"
        GUID OrderId FK
        STRING ProductId
        STRING ProductName
        DECIMAL UnitPrice
        INT Quantity
    }

    INBOX_MESSAGES {
        GUID Id PK "InboxMessage.Id (Guid)"
        STRING MessageId "original message id"
        STRING MessageType
        DATETIME ReceivedAt
        DATETIME ProcessedAt
    }

    ORDER_READS ||--o{ ORDER_ITEM_READS : contains
```

Short notes:
- The BFF read model is populated from integration events (e.g., `OrderCreatedIntegrationEvent`) by projection consumers.
- `INBOX_MESSAGES` is used by inbox/outbox processing to ensure idempotent handling of integration events.

## How to update these diagrams

- If you want field-level accuracy, open the entity and configuration classes under `EmberOps.OrderService/Domain` and `EmberOps.ApiGateway/Application/ReadModels`, then update the corresponding table definitions above.
- To visualize locally, paste the Mermaid blocks into a Mermaid live editor or view the files on GitHub (which renders Mermaid in markdown).

