using EmberOps.OrderService.Domain.Common;
using EmberOps.OrderService.Domain.Order.Enums;

namespace EmberOps.OrderService.Domain.Order
{
    public sealed class Order
    {
        // EF Core friendly
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public string TenantId { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;

        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? SubmittedAtUtc { get; private set; }
        public DateTime? PaidAtUtc { get; private set; }

        // Exposición read-only: afuera NO puede mutar
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { } // EF Core

        public Order(Guid id, string tenantId, DateTime createdAtUtc)
        {
            if (id == Guid.Empty) throw new DomainException("Order id is required.");
            if (string.IsNullOrWhiteSpace(tenantId)) throw new DomainException("TenantId is required.");

            Id = id;
            TenantId = tenantId;
            CreatedAtUtc = createdAtUtc;
            Status = OrderStatus.Draft;
        }

        public void AddItem(Guid productId, string nameSnapshot, decimal unitPriceSnapshot, int quantity)
        {
            EnsureEditable();

            if (productId == Guid.Empty) throw new DomainException("ProductId is required.");
            if (string.IsNullOrWhiteSpace(nameSnapshot)) throw new DomainException("Product name is required.");
            if (unitPriceSnapshot <= 0) throw new DomainException("Unit price must be > 0.");
            if (quantity <= 0) throw new DomainException("Quantity must be > 0.");
                        
            var existing = _items.FirstOrDefault(x => x.ProductId == productId);
            if (existing is not null)
            {
                existing.IncreaseQuantity(quantity);
            }
            else
            {
                _items.Add(new OrderItem(Guid.NewGuid(), Id, productId, nameSnapshot, unitPriceSnapshot, quantity));
            }

            RecalculateTotal();
        }

        public void ChangeItemQuantity(Guid orderItemId, int newQuantity)
        {
            EnsureEditable();

            if (newQuantity <= 0) throw new DomainException("Quantity must be > 0.");

            var item = _items.FirstOrDefault(i => i.Id == orderItemId)
                ?? throw new DomainException("Item not found.");

            item.SetQuantity(newQuantity);
            RecalculateTotal();
        }

        public void RemoveItem(Guid orderItemId)
        {
            EnsureEditable();

            var item = _items.FirstOrDefault(i => i.Id == orderItemId)
                ?? throw new DomainException("Item not found.");

            _items.Remove(item);
            RecalculateTotal();
        }

        public void Submit(DateTime utcNow)
        {
            EnsureEditable();

            if (_items.Count == 0) throw new DomainException("Cannot submit an empty order.");

            Status = OrderStatus.Submitted;
            SubmittedAtUtc = utcNow;
        }

        public void MarkAsPaid(DateTime utcNow)
        {
            if (Status != OrderStatus.Submitted)
                throw new DomainException("Only submitted orders can be paid.");

            Status = OrderStatus.Paid;
            PaidAtUtc = utcNow;
        }

        public void Cancel(DateTime utcNow, string reason)
        {
            if (Status == OrderStatus.Paid)
                throw new DomainException("Paid orders cannot be cancelled.");

            Status = OrderStatus.Cancelled;
            // podrías guardar reason en una propiedad si quieres
        }

        private void EnsureEditable()
        {
            if (Status is OrderStatus.Paid or OrderStatus.Cancelled)
                throw new DomainException("Order cannot be modified in its current state.");
        }

        private void RecalculateTotal()
        {
            TotalAmount = _items.Sum(i => i.UnitPriceSnapshot * i.Quantity);
        }
    }

}
