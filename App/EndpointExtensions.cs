using App.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Presentation.Accessor;

namespace App;

public static class EndpointExtensions
{
    public static void RegisterEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapFallbackToFile("index.html");
        
        app.MapHealthChecks("/health");
        
        var apiGroup = app
            .MapGroup("api/v1")
            .RequireAuthorization();

        apiGroup.RegisterFileEndpoints();
        
        apiGroup.MapGet("auth", ([FromServices] IUserContextAccessor accessor) => Results.Ok(accessor.GetUserContext()));
    }
}