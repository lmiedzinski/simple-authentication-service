namespace SimpleAuthenticationService.Application.Exceptions;

public class LoginAlreadyTakenException : Exception
{
    public LoginAlreadyTakenException(string login)
        : base($"User account with login {login} already exists")
    {
    }
}