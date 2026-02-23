using EmberOps.BuildingBlocks.Persistance.SqlServer.Intercerptors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EmberOps.BuildingBlocks.Persistance.SqlServer.Intercerptors
{
    public interface IClock { DateTime UtcNow { get; } }
    public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }
    public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IClock _clock;
        public AuditingSaveChangesInterceptor(IClock clock) => _clock = clock;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var ctx = eventData.Context;
            if (ctx is null) return result;

            var now = _clock.UtcNow;

            foreach (var e in ctx.ChangeTracker.Entries<IAuditable>())
            {
                if (e.State == EntityState.Added)
                    e.Entity.CreatedAtUtc = now;

                if (e.State is EntityState.Added or EntityState.Modified)
                    e.Entity.UpdatedAtUtc = now;
            }

            return result;
        }
    }
}
