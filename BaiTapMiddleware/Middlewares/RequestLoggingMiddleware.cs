using Microsoft.Extensions.Logging;

namespace QuanLySinhVien.Middlewares;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        _logger.LogInformation("[{Timestamp}] Method: {Method} - Path: {Path}", time, method, path);

        try
        {
            var invalidMessage = GetInvalidIdMessage(path);
            if (invalidMessage is not null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(invalidMessage);
                return;
            }

            await _next(context);
        }
        finally
        {
            var completedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _logger.LogInformation("[{Timestamp}] Status Code: {StatusCode}", completedAt, context.Response.StatusCode);
        }
    }

    private static string? GetInvalidIdMessage(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 3 || !int.TryParse(segments[2], out var id) || id > 0)
        {
            return null;
        }

        if (segments[0].Equals("Book", StringComparison.OrdinalIgnoreCase) &&
            segments[1].Equals("Detail", StringComparison.OrdinalIgnoreCase))
        {
            return "Book id không hợp lệ";
        }

        if (segments[0].Equals("SinhVien", StringComparison.OrdinalIgnoreCase) &&
            segments[1].Equals("Details", StringComparison.OrdinalIgnoreCase))
        {
            return "Mã sinh viên không hợp lệ";
        }

        return null;
    }
}
