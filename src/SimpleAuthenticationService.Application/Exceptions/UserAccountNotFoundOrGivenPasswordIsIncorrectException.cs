namespace SimpleAuthenticationService.Application.Exceptions;

public class UserAccountNotFoundOrGivenPasswordIsIncorrectException : Exception
{
    public UserAccountNotFoundOrGivenPasswordIsIncorrectException(string login)
        : base($"User account with identifier {login} was not found or the given password is incorrect")
    {
    }
}