using Presentation.Dto.User;

namespace Presentation.Repository;

public interface IUserRepository
{
    Task TryUpsertUser(AuthenticatedUser user);
}