using System.ComponentModel.DataAnnotations;

namespace DevBoard.Api.DTOs.Auth;

public sealed record LoginRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password
);
