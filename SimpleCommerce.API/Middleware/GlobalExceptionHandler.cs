using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using SimpleCommerce.Contract.Exceptions;

namespace SimpleCommerce.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message, errors) = MapException(exception);

        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            statusCode,
            message,
            traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier,
            errors,
            details = _environment.IsDevelopment() ? exception.ToString() : null
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
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
}
