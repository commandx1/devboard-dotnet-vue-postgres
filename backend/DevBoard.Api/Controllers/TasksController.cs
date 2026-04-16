using DevBoard.Api.DTOs.Tasks;
using DevBoard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBoard.Api.Controllers;

[Authorize]
[ApiController]
public class TasksController(ITaskService taskService, ICurrentUserAccessor currentUserAccessor) : ControllerBase
{
    [HttpGet("api/v1/projects/{projectId:long}/tasks")]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetByProject(long projectId, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        return Ok(await taskService.GetByProjectAsync(userId, projectId, ct));
    }

    [HttpPost("api/v1/projects/{projectId:long}/tasks")]
    public async Task<ActionResult<TaskResponse>> Create(long projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        var created = await taskService.CreateAsync(userId, projectId, request, ct);
        return Created($"/api/v1/tasks/{created.Id}", created);
    }

    [HttpPatch("api/v1/tasks/{taskId:long}")]
    public async Task<ActionResult<TaskResponse>> Update(long taskId, [FromBody] UpdateTaskRequest request, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        return Ok(await taskService.UpdateAsync(userId, taskId, request, ct));
    }

    [HttpDelete("api/v1/tasks/{taskId:long}")]
    public async Task<IActionResult> Delete(long taskId, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredUserId();
        await taskService.DeleteAsync(userId, taskId, ct);
        return NoContent();
    }
}
