using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevBoard.Api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DevBoard.Api.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing");
        var expirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var value) ? value : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
