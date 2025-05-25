using App.Extensions;
using App.Logging;
using App.Middleware;
using Application.Accessor;
using Application.Configuration;
using Application.Handler;
using Application.Repository;
using Database;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation.Accessor;
using Presentation.Handler;
using Presentation.Repository;

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
        builder.Services.AddHttpLogging();
        
        // Handler
        builder.Services
            .AddScoped<IFileDeletionHandler, FileDeletionHandler>()
            .AddScoped<IFileDownloadHandler, FileDownloadHandler>()
            .AddScoped<IFileSearchHandler, FileSearchHandler>()
            .AddScoped<IFileUploadHandler, FileUploadHandler>();
        
        // Services
        builder.Services
            .AddAntiforgery(options =>
            {
                options.HeaderName = ApplicationConstants.AntiforgeryHeaderName;
                options.Cookie.Name = ApplicationConstants.AntiforgeryCookieName;
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = builder.Environment.IsDevelopment()
                    ? SameSiteMode.Lax
                    : SameSiteMode.Strict;
            })
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
        
        // Repository
        builder.Services
            .AddScoped<IFileReadRepository, FileReadRepository>()
            .AddScoped<IFileWriteRepository, FileWriteRepository>()
            .AddScoped<IUserRepository, UserRepository>();
        
        builder.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        
        // Database
        builder.Services.AddDbContext<ApplicationDatabaseContext>(options =>
        {
            options.UseNpgsql(
                    builder.Configuration.GetConnectionStringOrThrow("DefaultConnection"),
                    b =>
                    {
                        b.MigrationsAssembly("App");
                        b.MigrationsHistoryTable(
                            "__EFMigrationsHistory",
                            ApplicationDatabaseContext.SchemaName);
                    })
                .UseSnakeCaseNamingConvention();
            
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
        
        // Auth
        builder.AddAuthDependencies();

        return builder;
    }

    private static string GetConnectionStringOrThrow(
        this IConfiguration configuration,
        string name)
    {
        return configuration.GetConnectionString(name)
            ?? throw new InvalidOperationException($"Missing connection string '{name}'.");
    }
}