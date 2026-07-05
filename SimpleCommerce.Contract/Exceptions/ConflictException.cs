namespace SimpleCommerce.Contract.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, StatusCodes.Conflict)
    {
    }
}
