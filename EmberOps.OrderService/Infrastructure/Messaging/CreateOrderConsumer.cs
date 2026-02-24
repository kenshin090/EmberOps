using EmberOps.Contracts.Order;
using EmberOps.OrderService.Handlers;
using MassTransit;

namespace EmberOps.OrderService.Infrastructure.Messaging
{
    public sealed class CreateOrderConsumer : IConsumer<CreateOrderRequest>
    {
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderConsumer(CreateOrderCommandHandler handler)
        {
            _handler = handler;
        }

        public async Task Consume(ConsumeContext<CreateOrderRequest> context)
        {
            var cmd = context.Message;

            await _handler.HandleAsync(cmd, context.CancellationToken);
        }
    }
}
