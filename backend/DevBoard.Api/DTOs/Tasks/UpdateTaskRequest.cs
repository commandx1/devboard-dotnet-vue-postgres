using System.ComponentModel.DataAnnotations;
using DevBoard.Api.Domain.Enums;
using TaskStatusEnum = DevBoard.Api.Domain.Enums.TaskStatus;

namespace DevBoard.Api.DTOs.Tasks;

public sealed record UpdateTaskRequest(
    [param: MaxLength(255)] string? Title,
    string? Description,
    TaskStatusEnum? Status,
    TaskPriority? Priority,
    DateOnly? DueDate
);
