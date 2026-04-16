using DevBoard.Api.Domain.Enums;
using TaskStatusEnum = DevBoard.Api.Domain.Enums.TaskStatus;

namespace DevBoard.Api.DTOs.Tasks;

public sealed record TaskResponse(
    long Id,
    long ProjectId,
    string Title,
    string? Description,
    TaskStatusEnum Status,
    TaskPriority Priority,
    DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
