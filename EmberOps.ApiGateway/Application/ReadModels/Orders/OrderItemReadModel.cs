namespace EmberOps.ApiGateway.Application.ReadModels.Orders
{
    public sealed class OrderItemReadModel
    {
        private OrderItemReadModel() { } // para EF

        public OrderItemReadModel(Guid orderId, string sku, string nameSnapshot, decimal unitPriceSnapshot, int quantity)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            Sku = sku;
            NameSnapshot = nameSnapshot;
            UnitPriceSnapshot = unitPriceSnapshot;
            Quantity = quantity;
        }
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string Sku { get; private set; }
        public string NameSnapshot { get; private set; } = default!;
        public decimal UnitPriceSnapshot { get; private set; }
        public int Quantity { get; private set; }
    }


}
