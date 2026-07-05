namespace SimpleCommerce.Contract.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action.")
        : base(message, StatusCodes.Unauthorized)
    {
    }
}
