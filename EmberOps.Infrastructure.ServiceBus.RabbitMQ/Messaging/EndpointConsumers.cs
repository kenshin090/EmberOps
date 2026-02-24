using System;
using System.Collections.Generic;
using System.Text;

namespace EmberOps.Infrastructure.ServiceBus.RabbitMQ.Messaging
{
    public record EndpointConsumers(string QueueName, params Type[] ConsumerTypes);
}
