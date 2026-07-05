namespace SimpleCommerce.Contract.Exceptions;

public class ValidationException : AppException
{
    public ValidationException(string message, IDictionary<string, string[]>? errors = null)
        : base(message, StatusCodes.BadRequest)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public IDictionary<string, string[]> Errors { get; }
}
