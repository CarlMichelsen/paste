namespace App;

public static class StaticFileOptionsFactory
{
    public static StaticFileOptions CreateFileOptions()
    {
        return new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            DefaultContentType = "application/octet-stream",
            OnPrepareResponse = context =>
            {
                const int oneMinute = 60;
                const int oneWeek = oneMinute * 60 * 24 * 7;
                
                var path = context.Context.Request.Path.Value!;
                var cacheControlValue = path.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                    ? $"public, max-age={oneMinute}"
                    : $"public, max-age={oneWeek * 53}";
                
                context.Context.Response.Headers
                    .Append("Cache-Control", cacheControlValue);
            },
        };
    }
}