using System.ComponentModel.DataAnnotations;
using DevBoard.Api.Exceptions;

namespace DevBoard.Api.Middleware;

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
            await WriteError(context, StatusCodes.Status404NotFound, "NOT_FOUND", ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteError(context, StatusCodes.Status409Conflict, "CONFLICT", ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "VALIDATION_ERROR", ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(context, StatusCodes.Status401Unauthorized, "UNAUTHORIZED", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteError(context, StatusCodes.Status500InternalServerError, "INTERNAL_SERVER_ERROR", "Unexpected error");
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
