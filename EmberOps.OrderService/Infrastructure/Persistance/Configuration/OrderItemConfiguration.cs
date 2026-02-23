using EmberOps.OrderService.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EmberOps.OrderService.Infrastructure.Persistance.Configuration
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> b)
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.NameSnapshot).HasMaxLength(200).IsRequired();
            b.Property(x => x.UnitPriceSnapshot).HasPrecision(18, 2);

            b.Property(x => x.Quantity).IsRequired();
        }
    }
}
