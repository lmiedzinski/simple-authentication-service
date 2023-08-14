using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

namespace SimpleAuthenticationService.Domain.UserAccounts;

public sealed class UserAccount : Entity
{
    private readonly List<Claim> _claims = new();

    private UserAccount()
    {
    }

    public UserAccountId Id { get; private set; }
    public UserAccountStatus Status { get; private set; }
    public Login Login { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public RefreshToken? RefreshToken { get; private set; }
    public IReadOnlyList<Claim> Claims => _claims.ToList();

    public static UserAccount Create(
        Login login,
        PasswordHash passwordHash)
    {
        var userAccount = new UserAccount
        {
            Id = new UserAccountId(Guid.NewGuid()),
            Status = UserAccountStatus.Active,
            Login = login,
            PasswordHash = passwordHash
        };

        userAccount.Raise(new UserAccountCreatedDomainEvent(Guid.NewGuid(), userAccount.Id));

        return userAccount;
    }

    public void AddClaim(string type, string? value)
    {
        CheckIfUserAccountUpdatesAllowed();

        var claim = new Claim(
            new ClaimId(Guid.NewGuid()),
            type,
            value);

        _claims.Add(claim);

        Raise(new ClaimAddedDomainEvent(Guid.NewGuid(), Id, claim.Id));
    }

    public void UpdateClaim(ClaimId claimId, string type, string? value)
    {
        CheckIfUserAccountUpdatesAllowed();

        var claim = _claims.FirstOrDefault(x => x.Id == claimId);
        if (claim is null) throw new ClaimNotFoundException(claimId);

        claim.Update(type, value);

        Raise(new ClaimUpdatedDomainEvent(Guid.NewGuid(), Id, claim.Id));
    }

    public void RemoveClaim(ClaimId claimId)
    {
        CheckIfUserAccountUpdatesAllowed();

        var claim = _claims.FirstOrDefault(x => x.Id == claimId);
        if (claim is null) throw new ClaimNotFoundException(claimId);

        _claims.Remove(claim);

        Raise(new ClaimRemovedDomainEvent(Guid.NewGuid(), Id, claim.Id));
    }

    public void SetNewRefreshToken(string value)
    {
        CheckIfUserAccountUpdatesAllowed();

        RefreshToken = new RefreshToken(value, true);
    }

    public void RevokeRefreshToken()
    {
        CheckIfUserAccountUpdatesAllowed();

        if (RefreshToken is null) return;
        RefreshToken = RefreshToken with { IsActive = false };
    }

    public void Lock()
    {
        CheckIfUserAccountUpdatesAllowed();

        if (RefreshToken is { IsActive: true }) RefreshToken = RefreshToken with { IsActive = false };
        Status = UserAccountStatus.Locked;

        Raise(new UserAccountLockedDomainEvent(Guid.NewGuid(), Id));
    }

    public void Unlock()
    {
        if (Status is UserAccountStatus.Deleted) throw new DeletedUserAccountUpdatesNotAllowedException(Id);

        if (Status is not UserAccountStatus.Locked) return;

        Status = UserAccountStatus.Active;

        Raise(new UserAccountUnlockedDomainEvent(Guid.NewGuid(), Id));
    }

    public void UpdatePasswordHash(PasswordHash passwordHash)
    {
        CheckIfUserAccountUpdatesAllowed();

        PasswordHash = passwordHash;

        Raise(new PasswordHashUpdatedDomainEvent(Guid.NewGuid(), Id));
    }

    public void Delete()
    {
        if (Status is UserAccountStatus.Deleted) return;

        Status = UserAccountStatus.Deleted;

        Raise(new UserAccountDeletedDomainEvent(Guid.NewGuid(), Id));
    }

    private void CheckIfUserAccountUpdatesAllowed()
    {
        switch (Status)
        {
            case UserAccountStatus.Locked:
                throw new LockedUserAccountUpdatesNotAllowedException(Id);
            case UserAccountStatus.Deleted:
                throw new DeletedUserAccountUpdatesNotAllowedException(Id);
            case UserAccountStatus.Active:
            default:
                return;
        }
    }
}