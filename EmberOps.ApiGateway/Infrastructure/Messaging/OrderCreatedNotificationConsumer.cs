using EmberOps.ApiGateway.Hubs;
using EmberOps.Contracts.Order.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace EmberOps.ApiGateway.Infrastructure.Messaging
{
    public sealed class OrderCreatedNotificationConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedNotificationConsumer> _logger;
        private readonly IHubContext<NotificationsHub> _notificationHub;
        private readonly IHubContext<DashboardHub> _dashboardHub;


        public OrderCreatedNotificationConsumer(
            ILogger<OrderCreatedNotificationConsumer> logger, 
            IHubContext<NotificationsHub> notificationHub, 
            IHubContext<DashboardHub> dashboardHub
            ) 
        {
            _logger = logger;
            _notificationHub = notificationHub;
            _dashboardHub = dashboardHub;
        }

        public Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            _logger.LogInformation("Apigateway received OrderCreated for OrderId {OrderId}, CorrelationId {CorrelationId}",
                context.Message.Order.Id, context.Message.CorrelationId);

            _notificationHub.Clients.All.SendAsync("orderCreated", new
            {
                context.Message.Order.Id,
                context.Message.CorrelationId
            });

            _dashboardHub.Clients.All.SendAsync("orderCreated", new
            {
                context.Message.Order.Id,
                context.Message.CorrelationId
            });

            return Task.CompletedTask;
        }
    }
}
