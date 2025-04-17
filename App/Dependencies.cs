using App.Extensions;
using App.Logging;
using App.Middleware;
using Application.Accessor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation.Accessor;

namespace App;

public static class Dependencies
{
    public static WebApplicationBuilder AddApplicationDependencies(
        this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration
            .AddJsonFile(
                "secrets.json",
                optional: builder.Environment.IsDevelopment(),
                reloadOnChange: false);
        
        // Logging
        builder.ApplicationUseSerilog();
        
        // Services
        builder.Services
            .AddOpenApi()
            .AddHttpContextAccessor()
            .AddMemoryCache()
            .AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            })
            .AddScoped<IUserContextAccessor, UserContextAccessor>()
            .AddScoped<TraceIdMiddleware>();
        
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        
        // Auth
        builder.AddAuthDependencies();

        return builder;
    }
}