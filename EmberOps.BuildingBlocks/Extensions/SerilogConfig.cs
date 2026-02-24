using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberOps.BuildingBlocks.Extensions
{
    public static class SerilogConfig
    {
        public static IHostBuilder ConfigureSerilog(
        this IHostBuilder host,
        string logPath)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.File($"{logPath}/log-.txt",
                    rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information()
                .CreateLogger();

            return host.UseSerilog();
        }
    }
}
