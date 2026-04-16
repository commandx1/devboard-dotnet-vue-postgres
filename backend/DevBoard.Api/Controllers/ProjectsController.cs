using DevBoard.Api.DTOs.Projects;
using DevBoard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBoard.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/projects")]
public class ProjectsController(IProjectService projectService, ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> GetMine(CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        return Ok(await projectService.GetMineAsync(userId, ct));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        var created = await projectService.CreateAsync(userId, request, ct);
        return Created($"/api/v1/projects/{created.Id}", created);
    }
}
