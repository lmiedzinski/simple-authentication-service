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
    public bool IsLocked { get; private set; }
    public bool IsDeleted { get; private set; }
    public string Login { get; private set; }
    public string PasswordHash { get; private set; }
    public RefreshToken? RefreshToken { get; private set; }
    public IReadOnlyList<Claim> Claims => _claims.ToList();

    public static UserAccount Create(
        string login,
        string passwordHash)
    {
        var userAccount = new UserAccount
        {
            Id = new UserAccountId(Guid.NewGuid()),
            IsLocked = false,
            IsDeleted = false,
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
        IsLocked = true;

        Raise(new UserAccountLockedDomainEvent(Guid.NewGuid(), Id));
    }

    public void Unlock()
    {
        if (IsDeleted) throw new DeletedUserAccountUpdatesNotAllowedException(Id);

        if (!IsLocked) return;

        IsLocked = false;

        Raise(new UserAccountUnlockedDomainEvent(Guid.NewGuid(), Id));
    }

    // TODO: Continue tests from here
    public void UpdatePasswordHash(string passwordHash)
    {
        CheckIfUserAccountUpdatesAllowed();

        PasswordHash = passwordHash;

        Raise(new PasswordHashUpdatedDomainEvent(Guid.NewGuid(), Id));
    }

    public void Delete()
    {
        if (IsDeleted) return;

        IsDeleted = true;

        Raise(new UserAccountDeletedDomainEvent(Guid.NewGuid(), Id));
    }

    private void CheckIfUserAccountUpdatesAllowed()
    {
        if (IsLocked) throw new LockedUserAccountUpdatesNotAllowedException(Id);
        if (IsDeleted) throw new DeletedUserAccountUpdatesNotAllowedException(Id);
    }
}