namespace EmberOps.Contracts.Order.Events
{
    public sealed record OrderCreatedIntegrationEvent
    {        
        public Guid EventId { get; init; } = Guid.NewGuid();
        public string EventType { get; init; } = "OrderCreated";
        public int EventVersion { get; init; } = 1;

        public DateTimeOffset OccurredAtUtc { get; init; } = DateTimeOffset.UtcNow;
        public Guid? CorrelationId { get; init; }
        public Guid? CausationId { get; init; } 
        public string Source { get; init; } = "EmberOps.OrderService";
        public OrderCreated Order { get; init; } = default;      
        public decimal TotalAmount
        {
            get; init;
        }
    }

    public sealed record OrderCreated
    {
        public Guid Id { get; init; }
        public int Status { get; init; } = 0;

        public decimal TotalAmount { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? SubmittedAtUtc { get; init; }
        public DateTime? PaidAtUtc { get; init; }

    }
}
