using DevBoard.Api.Data;
using DevBoard.Api.Domain.Entities;
using DevBoard.Api.DTOs.Tasks;
using DevBoard.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Api.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IReadOnlyList<TaskResponse>> GetByProjectAsync(Guid userId, long projectId, CancellationToken ct)
    {
        await EnsureProjectOwnership(userId, projectId, ct);

        return await db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.DeletedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponse(
                t.Id,
                t.ProjectId,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.DueDate,
                t.CreatedAt,
                t.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<TaskResponse> CreateAsync(Guid userId, long projectId, CreateTaskRequest request, CancellationToken ct)
    {
        await EnsureProjectOwnership(userId, projectId, ct);

        var now = DateTimeOffset.UtcNow;
        var entity = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = Domain.Enums.TaskStatus.TODO,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task<TaskResponse> UpdateAsync(Guid userId, long taskId, UpdateTaskRequest request, CancellationToken ct)
    {
        var entity = await db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.DeletedAt == null, ct)
            ?? throw new ResourceNotFoundException("Task", taskId);

        if (entity.Project.UserId != userId || entity.Project.DeletedAt is not null)
        {
            throw new ResourceNotFoundException("Task", taskId);
        }

        if (request.Title is not null)
        {
            entity.Title = request.Title.Trim();
        }

        if (request.Description is not null)
        {
            entity.Description = request.Description.Trim();
        }

        if (request.Status is not null)
        {
            entity.Status = request.Status.Value;
        }

        if (request.Priority is not null)
        {
            entity.Priority = request.Priority.Value;
        }

        if (request.DueDate is not null)
        {
            entity.DueDate = request.DueDate;
        }

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        return ToResponse(entity);
    }

    public async Task DeleteAsync(Guid userId, long taskId, CancellationToken ct)
    {
        var entity = await db.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.DeletedAt == null, ct)
            ?? throw new ResourceNotFoundException("Task", taskId);

        if (entity.Project.UserId != userId || entity.Project.DeletedAt is not null)
        {
            throw new ResourceNotFoundException("Task", taskId);
        }

        entity.DeletedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    private async Task EnsureProjectOwnership(Guid userId, long projectId, CancellationToken ct)
    {
        var ownsProject = await db.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId && p.UserId == userId && p.DeletedAt == null, ct);

        if (!ownsProject)
        {
            throw new ResourceNotFoundException("Project", projectId);
        }
    }

    private static TaskResponse ToResponse(TaskItem task) => new(
        task.Id,
        task.ProjectId,
        task.Title,
        task.Description,
        task.Status,
        task.Priority,
        task.DueDate,
        task.CreatedAt,
        task.UpdatedAt);
}
