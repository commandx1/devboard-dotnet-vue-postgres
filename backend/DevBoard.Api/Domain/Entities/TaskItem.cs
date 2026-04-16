using DevBoard.Api.Domain.Enums;
using TaskStatusEnum = DevBoard.Api.Domain.Enums.TaskStatus;

namespace DevBoard.Api.Domain.Entities;

public class TaskItem
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.TODO;
    public TaskPriority Priority { get; set; } = TaskPriority.MEDIUM;
    public DateOnly? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
}
