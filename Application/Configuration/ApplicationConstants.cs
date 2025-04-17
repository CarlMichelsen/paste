namespace Application.Configuration;

public static class ApplicationConstants
{
    public const string Name = "Paste";
    
    public const string Version = "0.0.1";
    
    public const string UserAgent = $"{Name}/{Version}";
    
    public const string TraceIdHeaderName = "X-Trace-Id";
    
    public const string AccessCookieName = "identity-access-token";
}