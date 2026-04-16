namespace DevBoard.Api.DTOs.Projects;

public sealed record ProjectResponse(
    long Id,
    string Name,
    string? Description,
    string Color,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
