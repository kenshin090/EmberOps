using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace EmberOps.BuildingBlocks.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();


            var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var cid)
                && !string.IsNullOrWhiteSpace(cid)
                    ? cid.ToString()
                    : Guid.NewGuid().ToString();

            context.Response.Headers["X-Correlation-Id"] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("Path", context.Request.Path.ToString()))
            using (LogContext.PushProperty("Method", context.Request.Method))
            {
                try
                {
                    _logger.LogInformation("Incoming request");

                    await _next(context);

                    sw.Stop();
                    _logger.LogInformation("Request completed with {StatusCode} in {ElapsedMs}ms",
                        context.Response.StatusCode,
                        sw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    _logger.LogError(ex, "Unhandled exception in {ElapsedMs}ms", sw.ElapsedMilliseconds);
                    throw;
                }
            }
        }
    }
}
