using System.ComponentModel.DataAnnotations;

namespace DevBoard.Api.DTOs.Auth;

public sealed record RegisterRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(3), MaxLength(50)] string Username,
    [param: Required, MinLength(8)] string Password
);
