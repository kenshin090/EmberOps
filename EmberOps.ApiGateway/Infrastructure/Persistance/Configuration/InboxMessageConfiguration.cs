using EmberOps.ApiGateway.Application.ReadModels.InboxEvent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmberOps.ApiGateway.Infrastructure.Persistance.Configuration
{
    public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
    {
        public void Configure(EntityTypeBuilder<InboxMessage> b)
        {
            b.ToTable("InboxMessages", "bff");
            b.HasKey(x => x.EventId);

            b.Property(x => x.ProcessedAtUtc).IsRequired();
            b.Property(x => x.Consumer).IsRequired();

        }
    }
}
