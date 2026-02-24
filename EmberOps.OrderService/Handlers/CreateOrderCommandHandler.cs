using EmberOps.Contracts.Order;
using EmberOps.Contracts.Order.Events;
using EmberOps.OrderService.Domain.Common;
using EmberOps.OrderService.Domain.Order;
using EmberOps.OrderService.Infrastructure.DB;
using MassTransit;

namespace EmberOps.OrderService.Handlers
{
    public sealed class CreateOrderCommandHandler
    {
        private readonly OrderDbContext _db;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IPublishEndpoint _publish;

        public CreateOrderCommandHandler(OrderDbContext db, ILogger<CreateOrderCommandHandler> logger, IPublishEndpoint publish)
        {
            _db = db;
            _logger = logger;
            _publish = publish;
        }

        public async Task HandleAsync(CreateOrderRequest command, CancellationToken ct)
        {
            _logger.LogInformation("Handling CreateOrderCommand with CorrelationId: {CorrelationId}", command.CorrelationId);

            if (command.ProductsInOrder is null || command.ProductsInOrder.Count == 0)
                throw new DomainException("Order must contain at least one item.");


            var orderId = Guid.NewGuid();
            var order = new Order(orderId, DateTime.UtcNow);

            foreach (var i in command.ProductsInOrder)
            {
                order.AddItem(
                    sku: i.Sku,
                    nameSnapshot: i.Name,
                    unitPriceSnapshot: i.UnitPrice,
                    quantity: i.Quantity
                );
            }

            _db.Orders.Add(order);

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Created order with Id: {OrderId} and TotalAmount: {TotalAmount}", order.Id, order.TotalAmount);

            var evt = new OrderCreatedIntegrationEvent
            {
                Order = new OrderCreated() { Id = order.Id, Status = (int)order.Status, CreatedAtUtc = order.CreatedAtUtc, TotalAmount = order.TotalAmount, SubmittedAtUtc = order.SubmittedAtUtc},
                TotalAmount = order.TotalAmount,
                CorrelationId = command.CorrelationId,
                CausationId = command.MessageId
            };

            await _publish.Publish(evt, context =>
            {
                context.CorrelationId = evt.CorrelationId;
                context.Headers.Set("CausationId", evt.CausationId);
                context.Headers.Set("Source", "EmberOps.OrderService");
                context.Headers.Set("OccurredAtUtc", evt.OccurredAtUtc.ToString("O"));
            }, ct);


        }
    }
}
