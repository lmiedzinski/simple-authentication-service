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

    public UserAccountId Id { get; private set; } = null!;
    public UserAccountStatus Status { get; private set; }
    public Login Login { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
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

    public void AddClaim(Claim claim)
    {
        CheckIfUserAccountUpdatesAllowed();

        if (_claims.Contains(claim)) throw new ClaimAlreadyExistsException(claim);

        _claims.Add(claim);

        Raise(new ClaimAddedDomainEvent(Guid.NewGuid(), Id, claim));
    }

    public void RemoveClaim(Claim claim)
    {
        CheckIfUserAccountUpdatesAllowed();
        
        if (!_claims.Contains(claim)) throw new ClaimNotFoundException(claim);

        _claims.Remove(claim);

        Raise(new ClaimRemovedDomainEvent(Guid.NewGuid(), Id, claim));
    }

    public void SetNewRefreshToken(string value, DateTime expiryDate)
    {
        CheckIfUserAccountUpdatesAllowed();

        RefreshToken = new RefreshToken(value, true, expiryDate);
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
        if (RefreshToken is { IsActive: true }) RefreshToken = RefreshToken with { IsActive = false };

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