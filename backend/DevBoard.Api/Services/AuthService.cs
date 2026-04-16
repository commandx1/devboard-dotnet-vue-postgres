using DevBoard.Api.Data;
using DevBoard.Api.Domain.Entities;
using DevBoard.Api.DTOs.Auth;
using DevBoard.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Api.Services;

public class AuthService(AppDbContext db, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim();

        var exists = await db.Users
            .AsNoTracking()
            .AnyAsync(u => (u.Email == email || u.Username == username) && u.DeletedAt == null, ct);

        if (exists)
        {
            throw new ConflictException("User already exists with this email or username.");
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var token = tokenService.GenerateToken(user);
        return new AuthResponse(token, new UserResponse(user.Id, user.Email, user.Username));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null, ct);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = tokenService.GenerateToken(user);
        return new AuthResponse(token, new UserResponse(user.Id, user.Email, user.Username));
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null, ct)
            ?? throw new ResourceNotFoundException("User", userId);

        return new UserResponse(user.Id, user.Email, user.Username);
    }
}
