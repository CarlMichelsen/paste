using Presentation.Options;

namespace Application.Configuration.Options;

public class JwtOptions : IOptionsImpl
{
    public static string SectionName => "Jwt";
    
    public required string Secret { get; init; }

    public required string Issuer { get; init; }
    
    public required string Audience { get; init; }
}