using EmberOps.ApiGateway.Application.ReadModels.Orders;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmberOps.ApiGateway.Infrastructure.Persistance.Configuration
{
   
    public sealed class OrderReadModelConfiguration : IEntityTypeConfiguration<OrderReadModel>
    {
        public void Configure(EntityTypeBuilder<OrderReadModel> b)
        {
            b.ToTable("OrdersReadModel","orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);


            b.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(nameof(OrderItemReadModel.OrderId))
                .OnDelete(DeleteBehavior.Cascade);


            var nav = b.Metadata.FindNavigation(nameof(OrderReadModel.Items));
            nav!.SetPropertyAccessMode(PropertyAccessMode.Field);
            nav.SetField("_items");
        }
    }
}
