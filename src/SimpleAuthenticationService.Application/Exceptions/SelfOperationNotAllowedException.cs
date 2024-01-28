namespace SimpleAuthenticationService.Application.Exceptions;

public class SelfOperationNotAllowedException : Exception
{
    public SelfOperationNotAllowedException()
        : base("Self operation not allowed")
    {
    }
}