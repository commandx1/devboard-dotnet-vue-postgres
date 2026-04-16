using System.ComponentModel.DataAnnotations;
using DevBoard.Api.Domain.Enums;

namespace DevBoard.Api.DTOs.Tasks;

public sealed record CreateTaskRequest(
    [param: Required, MaxLength(255)] string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate,
    IReadOnlyList<long>? TagIds
);
