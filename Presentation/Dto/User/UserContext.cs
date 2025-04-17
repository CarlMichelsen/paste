namespace Presentation.Dto.User;

public record UserContext(
    long LoginId,
    long RefreshId,
    long AccessId,
    AuthenticatedUser User);