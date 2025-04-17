using System.Text.Json;
using Application.Exception;
using Microsoft.AspNetCore.Http;
using Presentation.Dto.User;

namespace Application.Accessor;

public static class HttpUserContextAccessor
{
    public static UserContext GetUserContext(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new UserException("Not authorized when accessing user");
        }

        var userJsonClaim = GetClaim(httpContext, JwtClaims.User);

        var user = JsonSerializer.Deserialize<AuthenticatedUser>(userJsonClaim);
        if (user is null)
        {
            throw new UserException("User json resulted in a null object");
        }
        
        var accessIdClaim = GetClaim(httpContext, JwtClaims.AccessId);

        if (!long.TryParse(accessIdClaim, out var accessId))
        {
            throw new UserException("Failed to map claim to long (accessId)");
        }
        
        var refreshIdClaim = GetClaim(httpContext, JwtClaims.RefreshId);
        if (!long.TryParse(refreshIdClaim, out var refreshId))
        {
            throw new UserException("Failed to map claim to long (refreshId)");
        }
        
        var loginIdClaim = GetClaim(httpContext, JwtClaims.LoginId);
        if (!long.TryParse(loginIdClaim, out var loginId))
        {
            throw new UserException("Failed to map claim to long (loginId)");
        }

        return new UserContext(
            LoginId: loginId,
            RefreshId: refreshId,
            AccessId: accessId,
            User: user);
    }

    private static string GetClaim(HttpContext httpContext, string claimType)
    {
        var claim = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value;
        if (string.IsNullOrWhiteSpace(claim))
        {
            throw new UserException($"Did not find \"{claimType}\" claim type");
        }

        return claim;
    }
}