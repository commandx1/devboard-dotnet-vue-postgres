using System.Security.Claims;

namespace DevBoard.Api.Services;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid GetRequiredUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null || !Guid.TryParse(userId, out var parsed))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return parsed;
    }
}
