using EmberOps.ApiGateway.Application.ReadModels.InboxEvent;
using EmberOps.ApiGateway.Application.ReadModels.Orders;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;

namespace EmberOps.ApiGateway.Infrastructure.Persistance
{
    public class BffDbContext : DbContext
    {
        public BffDbContext(DbContextOptions<BffDbContext> options) : base(options) { }

        public DbSet<OrderReadModel> Orders => Set<OrderReadModel>();
        public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BffDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
