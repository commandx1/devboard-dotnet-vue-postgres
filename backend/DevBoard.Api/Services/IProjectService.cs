using DevBoard.Api.DTOs.Projects;

namespace DevBoard.Api.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponse>> GetMineAsync(Guid userId, CancellationToken ct);
    Task<ProjectResponse> CreateAsync(Guid userId, CreateProjectRequest request, CancellationToken ct);
}
