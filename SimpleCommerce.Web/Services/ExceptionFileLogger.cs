using System.Diagnostics;
using System.Text;

namespace SimpleCommerce.Web.Services;

public class ExceptionFileLogger
{
    private static readonly object WriteLock = new();

    private readonly string _logDirectory;

    public ExceptionFileLogger(IWebHostEnvironment environment)
    {
        _logDirectory = Path.Combine(environment.WebRootPath, "logs", "exceptions");
        Directory.CreateDirectory(_logDirectory);
    }

    public void Log(HttpContext context, Exception exception, int statusCode)
    {
        var fileName = $"exceptions-{DateTime.UtcNow:yyyy-MM-dd}.log";
        var filePath = Path.Combine(_logDirectory, fileName);
        var entry = BuildLogEntry(context, exception, statusCode);

        lock (WriteLock)
        {
            File.AppendAllText(filePath, entry, Encoding.UTF8);
        }
    }

    private static string BuildLogEntry(HttpContext context, Exception exception, int statusCode)
    {
        var builder = new StringBuilder();
        builder.AppendLine(new string('=', 80));
        builder.AppendLine($"Time (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        builder.AppendLine($"Status Code: {statusCode}");
        builder.AppendLine($"Request: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
        builder.AppendLine($"TraceId: {Activity.Current?.Id ?? context.TraceIdentifier}");
        builder.AppendLine($"Exception Type: {exception.GetType().FullName}");
        builder.AppendLine($"Message: {exception.Message}");
        builder.AppendLine("Stack Trace:");
        builder.AppendLine(exception.StackTrace);

        var inner = exception.InnerException;
        while (inner is not null)
        {
            builder.AppendLine("--- Inner Exception ---");
            builder.AppendLine($"Type: {inner.GetType().FullName}");
            builder.AppendLine($"Message: {inner.Message}");
            builder.AppendLine(inner.StackTrace);
            inner = inner.InnerException;
        }

        builder.AppendLine(new string('=', 80));
        builder.AppendLine();
        return builder.ToString();
    }
}

// Serilog 