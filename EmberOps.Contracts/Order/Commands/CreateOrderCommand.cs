namespace EmberOps.Contracts.Order.Commands
{

    public record CreateOrderCommand(
    Guid CorrelationId,
    Guid CustomerId,
    string Currency,
    IReadOnlyList<CreateOrderItemCommand> Items);

    public record CreateOrderItemCommand(
    string Sku,
    int Quantity,
    decimal UnitPrice,
    string Name = "" );
}
