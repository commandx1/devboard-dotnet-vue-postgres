namespace DevBoard.Api.DTOs.Auth;

public sealed record UserResponse(Guid Id, string Email, string Username);
