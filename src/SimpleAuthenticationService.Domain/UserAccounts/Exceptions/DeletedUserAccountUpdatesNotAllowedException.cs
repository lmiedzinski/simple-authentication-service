using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class DeletedUserAccountUpdatesNotAllowedException : DomainException
{
    public UserAccountId UserAccountId { get; private set; }
    public DeletedUserAccountUpdatesNotAllowedException(UserAccountId id)
        : base($"User account with the id {id.Value} is deleted and cannot be updated")
    {
        UserAccountId = id;
    }
}