namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class DeletedUserAccountUpdatesNotAllowedException : Exception
{
    public UserAccountId UserAccountId { get; private set; }
    public DeletedUserAccountUpdatesNotAllowedException(UserAccountId id)
        : base($"User account with the id {id.value} is deleted and cannot be updated")
    {
        UserAccountId = id;
    }
}