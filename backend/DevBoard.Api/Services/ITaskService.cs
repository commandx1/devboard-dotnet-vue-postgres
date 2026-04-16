using DevBoard.Api.DTOs.Tasks;

namespace DevBoard.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetByProjectAsync(Guid userId, long projectId, CancellationToken ct);
    Task<TaskResponse> CreateAsync(Guid userId, long projectId, CreateTaskRequest request, CancellationToken ct);
    Task<TaskResponse> UpdateAsync(Guid userId, long taskId, UpdateTaskRequest request, CancellationToken ct);
    Task DeleteAsync(Guid userId, long taskId, CancellationToken ct);
}
