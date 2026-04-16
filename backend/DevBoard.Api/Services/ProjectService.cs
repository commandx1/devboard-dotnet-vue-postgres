using DevBoard.Api.Data;
using DevBoard.Api.Domain.Entities;
using DevBoard.Api.DTOs.Projects;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Api.Services;

public class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IReadOnlyList<ProjectResponse>> GetMineAsync(Guid userId, CancellationToken ct)
    {
        return await db.Projects
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectResponse(p.Id, p.Name, p.Description, p.Color, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<ProjectResponse> CreateAsync(Guid userId, CreateProjectRequest request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var entity = new Project
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Color = string.IsNullOrWhiteSpace(request.Color) ? "#2563eb" : request.Color,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Projects.Add(entity);
        await db.SaveChangesAsync(ct);

        return new ProjectResponse(entity.Id, entity.Name, entity.Description, entity.Color, entity.CreatedAt, entity.UpdatedAt);
    }
}
