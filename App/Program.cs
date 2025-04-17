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

app.UseMiddleware<TraceIdMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();

app.UseStaticFiles(StaticFileOptionsFactory.CreateFileOptions());

app.RegisterEndpoints();

app.LogStartup();

app.Run();