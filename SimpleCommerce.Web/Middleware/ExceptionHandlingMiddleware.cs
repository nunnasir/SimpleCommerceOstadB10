using System.Diagnostics;
using System.Text.Json;
using SimpleCommerce.Contract.Exceptions;
using SimpleCommerce.Web.Models;
using SimpleCommerce.Web.Services;

namespace SimpleCommerce.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    public const string ValidationErrorsSessionKey = "ExceptionValidationErrors";

    private readonly RequestDelegate _next;
    
    
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private readonly ExceptionFileLogger _exceptionFileLogger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment,
        ExceptionFileLogger exceptionFileLogger)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _exceptionFileLogger = exceptionFileLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = MapException(exception);

        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        try
        {
            _exceptionFileLogger.Log(context, exception, statusCode);
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Failed to write exception to log file.");
        }

        if (WantsJsonResponse(context))
        {
            await WriteJsonResponseAsync(context, statusCode, message, errors, exception);
            return;
        }

        if (errors is { Count: > 0 })
        {
            context.Session.SetString(
                ValidationErrorsSessionKey,
                JsonSerializer.Serialize(errors));
        }

        var errorPath = GetErrorPagePath(statusCode, message);
        context.Response.Redirect(errorPath);
    }

    private static string GetErrorPagePath(int statusCode, string message)
    {
        var encodedMessage = Uri.EscapeDataString(message);

        var action = statusCode switch
        {
            Contract.Exceptions.StatusCodes.BadRequest => "BadRequest",
            Contract.Exceptions.StatusCodes.Unauthorized => "Unauthorized",
            Contract.Exceptions.StatusCodes.NotFound => "NotFound",
            Contract.Exceptions.StatusCodes.Conflict => "Conflict",
            403 => "Forbidden",
            _ => "ServerError"
        };

        return $"/Error/{action}?message={encodedMessage}";
    }

    private static (int StatusCode, string Message, IDictionary<string, string[]>? Errors) MapException(
        Exception exception)
    {
        return exception switch
        {
            ValidationException validation => (
                validation.StatusCode,
                validation.Message,
                validation.Errors),
            UnauthorizedException unauthorized => (unauthorized.StatusCode, unauthorized.Message, null),
            NotFoundException notFound => (notFound.StatusCode, notFound.Message, null),
            BadRequestException badRequest => (badRequest.StatusCode, badRequest.Message, null),
            ConflictException conflict => (conflict.StatusCode, conflict.Message, null),
            AppException app => (app.StatusCode, app.Message, null),
            _ => (Contract.Exceptions.StatusCodes.InternalServerError, "An unexpected error occurred.", null)
        };
    }

    private async Task WriteJsonResponseAsync(
        HttpContext context,
        int statusCode,
        string message,
        IDictionary<string, string[]>? errors,
        Exception exception)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
            Errors = errors,
            Details = _environment.IsDevelopment() ? exception.ToString() : null
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static bool WantsJsonResponse(HttpContext context)
    {
        if (context.Request.Headers.XRequestedWith == "XMLHttpRequest")
            return true;

        return context.Request.Headers.Accept
            .ToString()
            .Contains("application/json", StringComparison.OrdinalIgnoreCase);
    }
}


// Request -> Middleware Pipeline -> ExceptionHandlingMiddleware -> HandleExceptionAsync -> MapException -> WriteJsonResponseAsync or Redirect to Error Page
// Request -> Middlware1 -> Middleware2 -> Midlware1 -> 