using System.Text;
using Application.Configuration;
using Application.Configuration.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Extensions;

public static class AuthExtensions
{
    public static void AddAuthDependencies(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration
            .GetRequiredSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;
        
        builder.Services
            .AddAuthentication()
            .AddJwtBearer(
                "CookieScheme", 
                options =>
        {
            // Configure JWT settings
            options.TokenValidationParameters = AccessValidationParameters(jwtOptions);

            // Get token from cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[ApplicationConstants.AccessCookieName];
                    return Task.CompletedTask;
                },
            };
        });
        
        builder.Services.AddAuthorization();
    }
    
    private static TokenValidationParameters AccessValidationParameters(JwtOptions jwtOptions)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Crucial for lifetime validation
            ClockSkew = TimeSpan.Zero, // Crucial for lifetime validation
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
        };
    }
}