namespace EmberOps.ApiGateway.Application.ReadModels.InboxEvent
{
    public sealed class InboxMessage
    {
        public Guid EventId { get; set; }
        public DateTimeOffset ProcessedAtUtc { get; set; }
        public string Consumer { get; set; } = default!;
    }
}
