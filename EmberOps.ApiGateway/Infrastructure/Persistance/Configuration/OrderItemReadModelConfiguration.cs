using EmberOps.ApiGateway.Application.ReadModels.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmberOps.ApiGateway.Infrastructure.Persistance.Configuration
{
    public sealed class OrderItemReadModelConfiguration : IEntityTypeConfiguration<OrderItemReadModel>
    {
        public void Configure(EntityTypeBuilder<OrderItemReadModel> b)
        {
            b.ToTable("OrderItemsReadModel", "orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.NameSnapshot).HasMaxLength(200).IsRequired();
            b.Property(x => x.UnitPriceSnapshot).HasPrecision(18, 2);

            b.Property(x => x.Quantity).IsRequired();
        }
    }


}
