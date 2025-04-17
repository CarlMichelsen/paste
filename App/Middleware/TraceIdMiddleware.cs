using System.Diagnostics;
using Application.Configuration;

namespace App.Middleware;

public class TraceIdMiddleware(ILogger<TraceIdMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var rawTraceId = Activity.Current?.TraceId.ToString()
                         ?? Guid.NewGuid().ToString();
        var traceId = Guid.Parse(rawTraceId);
        context.TraceIdentifier = traceId.ToString().Replace("-", string.Empty);
        
        if (string.IsNullOrWhiteSpace(context.TraceIdentifier) || traceId == Guid.Empty)
        {
            // This should not really happen unless the traceId header exists but is empty.
            logger.LogWarning(
                "If there is a {HeaderName} header it must not be empty.",
                ApplicationConstants.TraceIdHeaderName);
        }
            
        context.Response.Headers
            .Append(ApplicationConstants.TraceIdHeaderName, context.TraceIdentifier);
            
        await next(context);
    }
}