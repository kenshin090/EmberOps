using EmberOps.OrderService.Domain.Common;

namespace EmberOps.OrderService.Domain.Order
{
    public sealed class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string NameSnapshot { get; private set; } = default!;
        public decimal UnitPriceSnapshot { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { } // EF Core

        internal OrderItem(Guid id, Guid orderId, Guid productId, string nameSnapshot, decimal unitPriceSnapshot, int quantity)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            NameSnapshot = nameSnapshot;
            UnitPriceSnapshot = unitPriceSnapshot;
            Quantity = quantity;
        }

        internal void SetQuantity(int qty)
        {
            if (qty <= 0) throw new DomainException("Quantity must be > 0.");
            Quantity = qty;
        }

        internal void IncreaseQuantity(int qty)
        {
            if (qty <= 0) throw new DomainException("Quantity increment must be > 0.");
            Quantity += qty;
        }
    }
}
