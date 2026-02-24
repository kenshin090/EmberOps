using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberOps.Infrastructure.ServiceBus.RabbitMQ.Extensions
{
    public static class MassTransitEndpointExtensions
    {
        public static void ConfigureCommandEndpoint<TConsumer>(
            this IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            string queueName)
            where TConsumer : class, IConsumer
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.ConfigureConsumer<TConsumer>(context);

                e.UseMessageRetry(r =>
                    r.Interval(3, TimeSpan.FromSeconds(2)));
            });
        }
    }
}
