namespace EmberOps.ApiGateway.Infrastructure.DTO
{
    public record CreateOrderHttpRequest(
    Guid CorrelationId,    
    List<CreateOrderItemHttp> ProductsInOrder);

    public record CreateOrderItemHttp(
    string Sku,
    int Quantity,
    decimal UnitPrice,
    string Name);
}
