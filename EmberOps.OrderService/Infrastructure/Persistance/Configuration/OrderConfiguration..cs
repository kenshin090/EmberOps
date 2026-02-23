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

            b.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.TotalAmount)
             .HasPrecision(18, 2);

            // One-to-many con backing field _items
            b.HasMany(typeof(OrderItem), "_items")
             .WithOne()
             .HasForeignKey(nameof(OrderItem.OrderId))
             .OnDelete(DeleteBehavior.Cascade);

            // Para que EF use el campo privado
            b.Navigation(nameof(Order.Items)).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
