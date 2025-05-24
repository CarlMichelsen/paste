using App;
using App.Logging;
using App.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();

    app.MapScalarApiReference();
}
else
{
    // "Who throws a shoe? Honestly!"
    await app.Services.EnsureDatabaseUpdated();
}

app.UseRouting();

app.UseMiddleware<TraceIdMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();

app.UseStaticFiles(StaticFileOptionsFactory.CreateFileOptions());

// Recommended for security when there are endpoints with state, such as file-upload endpoints.
app.UseAntiforgery();

app.UseHttpLogging();

app.RegisterEndpoints();

app.LogStartup();

app.Run();