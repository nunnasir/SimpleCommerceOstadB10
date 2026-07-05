namespace SimpleCommerce.Contract.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.NotFound)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} '{key}' was not found.", StatusCodes.NotFound)
    {
    }
}
