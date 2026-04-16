using DevBoard.Api.Domain.Entities;

namespace DevBoard.Api.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
