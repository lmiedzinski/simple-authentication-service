namespace SimpleAuthenticationService.Application.Exceptions;

public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException(string userAccountId)
        : base($"Given password is incorrect for user account with identifier {userAccountId}")
    {
    }
}