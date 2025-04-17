using Application.Configuration;
using Serilog;

namespace App.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ApplicationUseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, sp, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(sp)
                .Enrich.With(sp.GetRequiredService<TraceIdEnricher>())
                .Enrich.WithProperty("Application", "JobRunner")
                .Enrich.WithProperty("Environment", GetEnvironmentName(builder));
        });
        builder.Services.AddSingleton<TraceIdEnricher>();

        return builder;
    }
    
    public static WebApplication LogStartup(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var addresses = app.Urls;  // For minimal API, we can access urls directly
            foreach (var address in addresses)
            {
                logger.LogInformation(
                    "{ApplicationName} service has started at {Address}",
                    ApplicationConstants.Name,
                    address);
            }
        });
        
        return app;
    }
    
    private static string GetEnvironmentName(WebApplicationBuilder builder) =>
        builder.Environment.IsProduction() ? "Production" : "Development";
}