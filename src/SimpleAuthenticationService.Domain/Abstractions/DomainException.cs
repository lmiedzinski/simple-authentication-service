namespace SimpleAuthenticationService.Domain.Abstractions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}