using System;
using System.Collections.Generic;
using System.Text;

namespace EmberOps.Infrastructure.ServiceBus.RabbitMQ.Messaging
{
    public static class MessagingDefaults
    {
        public const int RetryCount = 3;
        public static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(2);
    }
}
