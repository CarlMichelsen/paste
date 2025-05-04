using Database;
using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Presentation.Dto.User;
using Presentation.Repository;

namespace Application.Repository;

public class UserRepository(
    ILogger<UserRepository> logger,
    ApplicationDatabaseContext databaseContext,
    TimeProvider timeProvider) : IUserRepository
{
    public async Task TryUpsertUser(AuthenticatedUser user)
    {
        var found = await databaseContext.User.FindAsync(user.Id);
        if (found is not null)
        {
            logger.LogInformation(
                "Presence of user <{Id}> '{Name}' was verified.",
                user.Id,
                user.Username);
            return;
        }
        
        try
        {
            var userEntity = new UserEntity
            {
                Id = user.Id,
                AuthenticationMethod = user.AuthenticationMethod,
                AuthenticationId = user.AuthenticationId,
                FirstLoginUtc = timeProvider.GetUtcNow(),
            };
            
            databaseContext.Add(userEntity);
            await databaseContext.SaveChangesAsync();
            
            logger.LogInformation(
                "User <{Id}> '{Name}' logged in for the first time.",
                user.Id,
                user.Username);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            // PostgreSQL unique violation code
            logger.LogInformation(
                "Presence of user <{Id}> '{Name}' was verified.",
                user.Id,
                user.Username);
        }
    }
}