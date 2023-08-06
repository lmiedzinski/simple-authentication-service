namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class UserAccountNotFoundException : Exception
{
    public UserAccountNotFoundException(UserAccountId id)
        : base($"User account with the id {id.value} was not found")
    {
    }
}