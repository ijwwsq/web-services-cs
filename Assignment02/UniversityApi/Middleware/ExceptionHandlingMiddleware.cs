using System.Text.Json;
using UniversityApi.Common;

namespace UniversityApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException exception)
        {
            await WriteResponse(context, exception.StatusCode, exception.ErrorCode, exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteResponse(
                context,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalServerError,
                "An unexpected error occurred");
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string errorCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        var body = ReturnResult<object>.Fail(statusCode, errorCode, message, context.TraceIdentifier);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, options));
    }
}
