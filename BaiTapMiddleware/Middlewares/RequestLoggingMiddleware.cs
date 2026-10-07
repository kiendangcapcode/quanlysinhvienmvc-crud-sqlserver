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
        var path = context.Request.Path.Value ?? string.Empty;

        // 1. THỰC TẾ: Bỏ qua file tĩnh (ảnh uploads, css, js, favicon) để tránh rác log
        if (IsStaticFile(path))
        {
            await _next(context);
            return;
        }

        var method = context.Request.Method;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var invalidMessage = GetInvalidIdMessage(path);
            if (invalidMessage is not null)
            {
                stopwatch.Stop();
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(invalidMessage);

                var timeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _logger.LogWarning("[{Timestamp}] [CHẶN] {Method} {Path} -> 400 Bad Request ({Elapsed}ms) - Lý do: {Reason}",
                    timeNow, method, path, stopwatch.ElapsedMilliseconds, invalidMessage);
                return;
            }

            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var completedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Xác định chi tiết thao tác nghiệp vụ
            var actionNote = "";
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                if (statusCode >= 300 && statusCode < 400)
                {
                    if (path.Contains("/SinhVien/Create", StringComparison.OrdinalIgnoreCase))
                        actionNote = " [Tạo SV thành công]";
                    else if (path.Contains("/SinhVien/Edit", StringComparison.OrdinalIgnoreCase))
                        actionNote = " [Cập nhật SV thành công]";
                    else if (path.Contains("/SinhVien/Delete", StringComparison.OrdinalIgnoreCase))
                        actionNote = " [Xóa SV thành công]";
                }
                else if (statusCode == 200 && path.Contains("/SinhVien/Create", StringComparison.OrdinalIgnoreCase))
                {
                    actionNote = " [Form lỗi - Nhập lại]";
                }
            }

            // Gộp thành 1 DÒNG LOG DUY NHẤT kèm thời gian xử lý (Execution time)
            _logger.LogInformation(
                "[{Timestamp}] {Method} {Path}{ActionNote} -> {StatusCode} ({Elapsed}ms)",
                completedAt, method, path, actionNote, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }

    private static bool IsStaticFile(string path)
    {
        return path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);
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
