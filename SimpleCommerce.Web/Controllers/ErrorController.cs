using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SimpleCommerce.Web.Middleware;
using SimpleCommerce.Web.Models;

namespace SimpleCommerce.Web.Controllers;

public class ErrorController : Controller
{
    [HttpGet]
    [ActionName("NotFound")]
    public IActionResult NotFoundError(string? message = null)
    {
        Response.StatusCode = 404;
        return View("NotFound", BuildModel(
            404,
            "Page Not Found",
            "The page or resource you are looking for could not be found.",
            message));
    }

    [HttpGet]
    [ActionName("BadRequest")]
    public IActionResult BadRequestError(string? message = null)
    {
        Response.StatusCode = 400;
        var errors = GetValidationErrorsFromSession();
        return View("BadRequest", BuildModel(
            400,
            "Bad Request",
            "The request could not be processed due to invalid data.",
            message,
            errors));
    }

    [HttpGet]
    [ActionName("Unauthorized")]
    public IActionResult UnauthorizedError(string? message = null)
    {
        Response.StatusCode = 401;
        return View("Unauthorized", BuildModel(
            401,
            "Unauthorized",
            "You must be signed in to access this resource.",
            message));
    }

    [HttpGet]
    [ActionName("Forbidden")]
    public IActionResult ForbiddenError(string? message = null)
    {
        Response.StatusCode = 403;
        return View("Forbidden", BuildModel(
            403,
            "Access Denied",
            "You do not have permission to access this resource.",
            message));
    }

    [HttpGet]
    [ActionName("Conflict")]
    public IActionResult ConflictError(string? message = null)
    {
        Response.StatusCode = 409;
        return View("Conflict", BuildModel(
            409,
            "Conflict",
            "The request could not be completed because of a conflict with the current state.",
            message));
    }

    [HttpGet]
    [ActionName("ServerError")]
    public IActionResult ServerError(string? message = null)
    {
        Response.StatusCode = 500;
        return View("ServerError", BuildModel(
            500,
            "Server Error",
            "Something went wrong on our end. Please try again later.",
            message));
    }

    [HttpGet]
    public IActionResult HandleStatusCode(int code)
    {
        return code switch
        {
            400 => BadRequestError(),
            401 => UnauthorizedError(),
            403 => ForbiddenError(),
            404 => NotFoundError(),
            409 => ConflictError(),
            _ => ServerError()
        };
    }

    private ErrorPageViewModel BuildModel(
        int statusCode,
        string title,
        string defaultMessage,
        string? message = null,
        IDictionary<string, string[]>? errors = null)
    {
        return new ErrorPageViewModel
        {
            StatusCode = statusCode,
            Title = title,
            Message = message ?? defaultMessage,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Errors = errors
        };
    }

    private IDictionary<string, string[]>? GetValidationErrorsFromSession()
    {
        var json = HttpContext.Session.GetString(ExceptionHandlingMiddleware.ValidationErrorsSessionKey);
        if (string.IsNullOrEmpty(json))
            return null;

        HttpContext.Session.Remove(ExceptionHandlingMiddleware.ValidationErrorsSessionKey);
        return JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);
    }
}
