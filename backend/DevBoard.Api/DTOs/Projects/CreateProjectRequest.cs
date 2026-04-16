using System.ComponentModel.DataAnnotations;

namespace DevBoard.Api.DTOs.Projects;

public sealed record CreateProjectRequest(
    [param: Required, MaxLength(100)] string Name,
    [param: MaxLength(2000)] string? Description,
    [param: RegularExpression("^#[0-9a-fA-F]{6}$")] string? Color
);
