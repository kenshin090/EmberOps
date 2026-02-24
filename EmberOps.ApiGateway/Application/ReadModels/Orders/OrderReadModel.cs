namespace EmberOps.ApiGateway.Application.ReadModels.Orders
{
    public sealed class OrderReadModel
    {
        // EF Core friendly
        private readonly List<OrderItemReadModel> _items = new();

        public Guid Id { get;  set; }
        public int Status { get;  set; } 

        public decimal TotalAmount { get;  set; }
        public DateTime CreatedAtUtc { get;  set; }
        public DateTime? SubmittedAtUtc { get;  set; }
        public DateTime? PaidAtUtc { get;  set; }



        public IReadOnlyCollection<OrderItemReadModel> Items => _items.AsReadOnly();

        public Guid LastEventId { get; internal set; }
        public DateTimeOffset LastUpdatedUtc { get; internal set; }

        public OrderReadModel(Guid id,int status, DateTime createdAtUtc, decimal totalAmount, DateTime? submittedAtUtc, DateTime? paidAtUtc, Guid lastEventId)
        {
           
            Id = id;
            CreatedAtUtc = createdAtUtc;
            Status = status;
            TotalAmount = totalAmount;
            SubmittedAtUtc = submittedAtUtc;
            PaidAtUtc = paidAtUtc;
            LastEventId = lastEventId;

        }

    }
}
