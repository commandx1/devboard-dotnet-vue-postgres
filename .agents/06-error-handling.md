# Agent: Error Handling - Tum Katmanlar

## Backend - Global Exception Middleware

```csharp
public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ResourceNotFoundException ex)
        {
            await WriteError(context, 404, "NOT_FOUND", ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteError(context, 400, "VALIDATION_ERROR", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteError(context, 500, "INTERNAL_SERVER_ERROR", "Unexpected error");
        }
    }

    private static async Task WriteError(HttpContext context, int status, string error, string message)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            status,
            error,
            message,
            path = context.Request.Path.Value,
            timestamp = DateTimeOffset.UtcNow
        };

        await context.Response.WriteAsJsonAsync(payload);
    }
}
```

## Backend Validation
- DTO'larda DataAnnotations kullan:
```csharp
[property: Required, MaxLength(255)] string Title
```
- Invalid model state durumunda otomatik 400 donecek sekilde `ApiController` attribute kullan.

## Frontend Error Handling
- API hata parse helper yaz.
- Composable/store icinde kullaniciya anlamli mesaj ver.
- Merkezi notification store (toast) kullan.

```ts
export function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    return error.response?.data?.message ?? 'An unexpected error occured.'
  }
  return 'Unexpected client error.'
}
```

## Validation Strategy
- Backend source of truth.
- Frontend pre-check sadece UX icin (bos baslik, max length vb).
- Iki tarafta da ayni kurallar uyumlu olmali.
