using Serilog.Core;
using Serilog.Events;

namespace App.Logging;

public class TraceIdEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        try
        {
            var traceId = httpContextAccessor.HttpContext?.TraceIdentifier ?? "no-trace-id";
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
        }
        catch (Exception)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", "error-getting-trace-id"));
        }
    }
}