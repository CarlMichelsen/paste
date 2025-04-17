using Microsoft.AspNetCore.Http;
using Presentation.Dto.User;

namespace Presentation.Accessor;

public interface IUserContextAccessor
{
    UserContext GetUserContext(HttpContext? httpContext = default);
}