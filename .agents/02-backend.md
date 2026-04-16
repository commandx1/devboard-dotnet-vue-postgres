# Agent: Backend - .NET 8 + ASP.NET Core Web API

## Katman Mimarisi

```text
Controller -> Service -> DbContext -> PostgreSQL
      ^           ^
    DTO'lar    Entity'ler
```

**Kural:** Controller hicbir zaman EF entity return etmez. Her zaman DTO doner.

## Entity Conventions

```csharp
public class TaskItem
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
```

### Entity Kurallari
- Navigation alanlari `null!` veya constructor ile guvenli init edilir.
- Enum degerleri veritabaninda string tutulur.
- Tum zaman alanlari `DateTimeOffset` / PostgreSQL `timestamptz` map edilir.
- Soft delete desteklenir (`DeletedAt`).

## DbContext ve Configuration

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.Priority).HasConversion<string>();
        });
    }
}
```

## Service Conventions

```csharp
public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IReadOnlyList<TaskResponse>> GetByProjectAsync(long projectId, CancellationToken ct)
    {
        return await db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.DeletedAt == null)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponse(t.Id, t.Title, t.Status.ToString(), t.Priority.ToString(), t.DueDate, t.CreatedAt, t.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<TaskResponse> CreateAsync(long projectId, CreateTaskRequest request, CancellationToken ct)
    {
        var entity = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TaskStatus.Todo,
            DueDate = request.DueDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(ct);

        return new TaskResponse(entity.Id, entity.Title, entity.Status.ToString(), entity.Priority.ToString(), entity.DueDate, entity.CreatedAt, entity.UpdatedAt);
    }
}
```

## Controller Conventions

```csharp
[ApiController]
[Route("api/v1/projects/{projectId:long}/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> Get(long projectId, CancellationToken ct)
    {
        return Ok(await taskService.GetByProjectAsync(projectId, ct));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(long projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var created = await taskService.CreateAsync(projectId, request, ct);
        return Created($"/api/v1/tasks/{created.Id}", created);
    }
}
```

## Naming Conventions
| Alan | Kural | Ornek |
|------|-------|-------|
| Namespace | `DevBoard.Api.{Feature}` | `DevBoard.Api.Services` |
| Controller | `{Resource}Controller` | `TasksController` |
| Service | `I{Resource}Service` + `{Resource}Service` | `ITaskService` |
| Request DTO | `VerbResourceRequest` | `CreateTaskRequest` |
| Response DTO | `{Resource}Response` | `TaskResponse` |

## Dependencies (csproj)
- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `BCrypt.Net-Next`
