using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static MassTransit.Logging.OperationName;

namespace EmberOps.Infrastructure.ServiceBus.RabbitMQ.Messaging
{
    public static class BusRegistration
    {

        public static IServiceCollection AddMessageBus(
         this IServiceCollection services,
         IConfiguration config,
         IEnumerable<EndpointConsumers>? consumers = null)
        {
            var host = config["Rabbit:Host"] ?? "rabbitmq";
            var user = config["Rabbit:User"] ?? "guest";
            var pass = config["Rabbit:Pass"] ?? "guest";
            var vhost = config["Rabbit:VHost"] ?? "/";

            services.AddMassTransit(x =>
            {                
                if (consumers != null)
                {
                    foreach (var ec in consumers)
                    {
                        foreach (var consumerType in ec.ConsumerTypes)
                            x.AddConsumer(consumerType);
                    }
                }
                                
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: "emberops", includeNamespace: false));

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host, vhost, h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });

                    cfg.UseMessageRetry(r =>
                        r.Interval(MessagingDefaults.RetryCount, MessagingDefaults.RetryInterval));
                   

                    if (consumers != null)
                    {
                        foreach (var ec in consumers)
                        {
                            cfg.ReceiveEndpoint(ec.QueueName, e =>
                            {
                                e.UseMessageRetry(r =>
                                    r.Interval(MessagingDefaults.RetryCount, MessagingDefaults.RetryInterval));

                                foreach (var consumerType in ec.ConsumerTypes)
                                {                                    
                                    e.ConfigureConsumer(context, consumerType);
                                }
                            });
                        }
                    }

                    
                });
            });

            return services;
        }
    }

    /// <summary>
    /// prefix exchanges/queues with "emberops."
    /// </summary>
    public sealed class KebabCaseEntityNameFormatter : IEntityNameFormatter
    {
        private readonly KebabCaseEndpointNameFormatter _formatter;

        // prefix opcional: "emberops" => "emberops-order-created"
        public KebabCaseEntityNameFormatter(string? prefix = null)
        {
            _formatter = new KebabCaseEndpointNameFormatter(prefix, includeNamespace: false);
        }

        public string FormatEntityName<T>()
        {           
            return _formatter.SanitizeName(typeof(T).Name);
        }
    }
}
