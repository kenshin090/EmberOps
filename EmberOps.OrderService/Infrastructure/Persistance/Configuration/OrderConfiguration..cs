using EmberOps.OrderService.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmberOps.OrderService.Infrastructure.Persistance.Configuration
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            
            b.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(nameof(OrderItem.OrderId))
                .OnDelete(DeleteBehavior.Cascade);

            
            var nav = b.Metadata.FindNavigation(nameof(Order.Items));
            nav!.SetPropertyAccessMode(PropertyAccessMode.Field);
            nav.SetField("_items");
        }
    }
}
