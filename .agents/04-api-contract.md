# Agent: API Contract - REST + DTO Kurallari

## URL Yapisi

```text
/api/v1/{resource}
/api/v1/{resource}/{id}
/api/v1/{parent}/{parentId}/{child}
```

Ornek:
- `GET    /api/v1/projects`
- `POST   /api/v1/projects`
- `GET    /api/v1/projects/42/tasks`
- `POST   /api/v1/projects/42/tasks`
- `PATCH  /api/v1/tasks/99`

## HTTP Method Kurallari
- `GET` -> okuma
- `POST` -> olusturma (`201 Created`)
- `PATCH` -> kismi guncelleme (`200 OK`)
- `DELETE` -> soft delete (`204 No Content`)

## Basarili Response Ornegi
```json
{
  "id": 99,
  "title": "API kontratini tamamla",
  "status": "TODO",
  "priority": "HIGH",
  "createdAt": "2026-04-15T12:00:00Z",
  "updatedAt": "2026-04-15T12:00:00Z"
}
```

## Error Response (tek format)
```json
{
  "status": 404,
  "error": "NOT_FOUND",
  "message": "Task with id 99 not found",
  "path": "/api/v1/tasks/99",
  "timestamp": "2026-04-15T12:00:00Z"
}
```

## DTO Kurallari (.NET)

```csharp
public sealed record CreateTaskRequest(
    [property: Required, MaxLength(255)] string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate,
    IReadOnlyList<long>? TagIds
);

public sealed record TaskResponse(
    long Id,
    string Title,
    string Status,
    string Priority,
    DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
```

## Pagination
- Query param:
  - `page` (default: 1)
  - `size` (default: 20)
  - `sortBy` (default: `createdAt`)
  - `sortDir` (`asc|desc`)

Response wrapper:
```json
{
  "items": [],
  "page": 1,
  "size": 20,
  "totalItems": 47,
  "totalPages": 3
}
```
