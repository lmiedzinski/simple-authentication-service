using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class LockedUserAccountUpdatesNotAllowedException : DomainException
{
    public UserAccountId UserAccountId { get; private set; }
    
    public LockedUserAccountUpdatesNotAllowedException(UserAccountId userAccountId)
        : base($"User account with the id {userAccountId.Value} is locked")
    {
        UserAccountId = userAccountId;
    }
}