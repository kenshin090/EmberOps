using EmberOps.BuildingBlocks.Persistance.SqlServer.Intercerptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EmberOps.BuildingBlocks.Persistance.SqlServer.Extensions
{
    public static class ServiceCollectionPersistanseExtensions
    {
        public static IServiceCollection AddSqlServerPersistence<TContext>(
        this IServiceCollection services,
        IConfiguration config,
        SqlServerPersistenceOptions options)
        where TContext : DbContext
        {
            services.AddSingleton<IClock, SystemClock>();
            services.AddScoped<AuditingSaveChangesInterceptor>();            

            services.AddDbContext<TContext>((sp, db) =>
            {
                var cs = config.GetConnectionString(options.ConnectionStringName)
                         ?? throw new InvalidOperationException($"Missing connection string: {options.ConnectionStringName}");

                

                db.UseSqlServer(cs, sql =>
                {
                    sql.CommandTimeout(options.CommandTimeoutSeconds);
                    // IMPORTANT: migrations live in the service assembly
                    sql.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                });

                db.EnableDetailedErrors(options.EnableDetailedErrors);
                db.EnableSensitiveDataLogging(options.EnableSensitiveDataLogging);

                db.AddInterceptors(
                    sp.GetRequiredService<AuditingSaveChangesInterceptor>()                
                );
            });

            return services;
        }
    }
}
