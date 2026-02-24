using EmberOps.ApiGateway.Application.ReadModels.InboxEvent;
using EmberOps.ApiGateway.Application.ReadModels.Orders;
using EmberOps.ApiGateway.Infrastructure.Persistance;
using EmberOps.Contracts.Order.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace EmberOps.ApiGateway.Infrastructure.Messaging
{
    public sealed class OrderCreatedProjectionConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedProjectionConsumer> _logger;
        private readonly BffDbContext _db;

        public OrderCreatedProjectionConsumer(
            ILogger<OrderCreatedProjectionConsumer> logger,
            BffDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var evt = context.Message;

            _logger.LogInformation(
                "Apigateway received OrderCreated for OrderId {OrderId}, CorrelationId {CorrelationId} storing order in read db",
                evt.Order.Id, evt.CorrelationId);

            // 1) Idempotency (Inbox)
            var alreadyProcessed = await _db.InboxMessages
                .AsNoTracking()
                .AnyAsync(x => x.EventId == evt.EventId, context.CancellationToken);

            if (alreadyProcessed)
            {
                _logger.LogInformation("Skipping duplicate OrderCreated event EventId={EventId}", evt.EventId);
                return;
            }

            // 2) Upsert OrderReadModel
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == evt.Order.Id, context.CancellationToken);

            if (order is null)
            {
                order = new OrderReadModel(evt.Order.Id, evt.Order.Status, DateTime.UtcNow,evt.TotalAmount,evt.Order.SubmittedAtUtc,evt.Order.PaidAtUtc,evt.EventId);
                _db.Orders.Add(order);
            }
            else
            {              
                
                order.TotalAmount = evt.TotalAmount;
                order.LastEventId = evt.EventId;
                order.LastUpdatedUtc = DateTimeOffset.UtcNow;
                
            }
            
            // 3) Mark event as processed (Inbox)
            _db.InboxMessages.Add(new InboxMessage
            {
                EventId = evt.EventId,
                ProcessedAtUtc = DateTimeOffset.UtcNow,
                Consumer = nameof(OrderCreatedProjectionConsumer)
            });

            await _db.SaveChangesAsync(context.CancellationToken);
        }
    }



}
