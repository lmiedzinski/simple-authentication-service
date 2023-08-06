namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class LockedUserAccountUpdatesNotAllowedException : Exception
{
    public UserAccountId UserAccountId { get; private set; }
    
    public LockedUserAccountUpdatesNotAllowedException(UserAccountId userAccountId)
        : base($"User account with the id {userAccountId.value} is locked and cannot be updated")
    {
        UserAccountId = userAccountId;
    }
}