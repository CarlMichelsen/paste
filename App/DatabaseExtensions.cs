using Database;
using Microsoft.EntityFrameworkCore;

namespace App;

public static class DatabaseExtensions
{
    public static async Task EnsureDatabaseUpdated(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
        await dbContext.Database.MigrateAsync();
    }
}