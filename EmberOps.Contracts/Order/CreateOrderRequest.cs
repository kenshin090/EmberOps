namespace EmberOps.Contracts.Order
{
    public class CreateOrderRequest
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid CorrelationId { get; private set; }
        public List<ProductInOrder> ProductsInOrder { get; set; }


        public CreateOrderRequest(Guid correlationId, List<ProductInOrder> productsInOrder)
        {
            this.CorrelationId = correlationId;
            this.ProductsInOrder = productsInOrder;
        }

    }

    public class ProductInOrder
    {
        public ProductInOrder(string sku, int quantity, decimal unitPrice, string name)
        {
            Sku = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Name = $"Current name {name}";
        }

        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }


}
