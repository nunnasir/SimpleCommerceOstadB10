namespace SimpleCommerce.Web.Models;

public class ErrorPageViewModel
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RequestId { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public bool HasValidationErrors => Errors is { Count: > 0 };
}
