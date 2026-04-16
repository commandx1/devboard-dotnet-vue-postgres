namespace DevBoard.Api.Services;

public interface ICurrentUserAccessor
{
    Guid GetRequiredUserId();
}
