using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountTests
{
    [Fact]
    public void Create_Returns_UserAccount_Object_With_Correct_Properties_On_Success()
    {
        // Arrange
        const string login = "userlogin@domain.com";
        const string passwordHash = "SuperSecretPasswordHash";
        UserAccount? userAccount = null;

        // Act
        var exception = Record.Exception(() => { userAccount = UserAccount.Create(login, passwordHash); });

        // Assert
        exception.Should().BeNull();
        userAccount.Should().NotBeNull();
        userAccount!.Login.Should().Be(login);
        userAccount!.PasswordHash.Should().Be(passwordHash);
        userAccount!.Id.Should().NotBeNull().And.NotBe(Guid.Empty);
        userAccount!.IsLocked.Should().BeFalse();
        userAccount!.IsDeleted.Should().BeFalse();
        userAccount!.RefreshToken.Should().BeNull();
        userAccount!.Claims.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Create_Adds_UserAccountCreatedDomainEvent_On_Success()
    {
        // Arrange
        UserAccount? userAccount = null;

        // Act
        var exception = Record.Exception(() => { userAccount = UserAccount.Create(string.Empty, string.Empty); });

        // Assert
        exception.Should().BeNull();
        userAccount.Should().NotBeNull();
        var domainEvents = userAccount!.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<UserAccountCreatedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((UserAccountCreatedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void AddClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() => { userAccount.AddClaim(string.Empty, default); });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void AddClaim_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();
        // Act
        var exception = Record.Exception(() => { userAccount.AddClaim(string.Empty, default); });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void AddClaim_Adds_Claim_On_Success()
    {
        // Arrange
        const string claimType = "claimType";
        const string claimValue = "claimValue";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() => { userAccount.AddClaim(claimType, claimValue); });

        // Assert
        exception.Should().BeNull();
        var claims = userAccount.Claims;
        claims.Should().NotBeNull().And.HaveCount(1);
        var claim = claims.First();
        claim.Id.Should().NotBe(Guid.Empty);
        claim.Type.Should().Be(claimType);
        claim.Value.Should().NotBeNull().And.Be(claimValue);
    }

    [Fact]
    public void AddClaim_Adds_ClaimAddedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() => { userAccount.AddClaim(string.Empty, default); });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<ClaimAddedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((ClaimAddedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
        ((ClaimAddedDomainEvent)domainEvent).ClaimId.Should().Be(userAccount.Claims.First().Id);
    }

    [Fact]
    public void UpdateClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdateClaim(userAccount.Claims.First().Id, string.Empty, default);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void UpdateClaim_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdateClaim(userAccount.Claims.First().Id, string.Empty, default);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void UpdateClaim_Throws_ClaimNotFoundException_When_Given_NotExisting_ClaimId()
    {
        // Arrange
        var notExistingGuid = Guid.NewGuid();
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdateClaim(new ClaimId(notExistingGuid), string.Empty, default);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ClaimNotFoundException>();
        ((ClaimNotFoundException)exception!).ClaimId.value.Should().Be(notExistingGuid);
    }

    [Fact]
    public void UpdateClaim_Updates_Claim_On_Success()
    {
        // Arrange
        const string claimType = "claimType";
        const string claimValue = "claimValue";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdateClaim(userAccount.Claims.First().Id, claimType, claimValue);
        });

        // Assert
        exception.Should().BeNull();
        var claims = userAccount.Claims;
        claims.Should().NotBeNull().And.HaveCount(1);
        var claim = claims.First();
        claim.Type.Should().Be(claimType);
        claim.Value.Should().NotBeNull().And.Be(claimValue);
    }

    [Fact]
    public void UpdateClaim_Adds_ClaimUpdatedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdateClaim(userAccount.Claims.First().Id, string.Empty, default);
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<ClaimUpdatedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((ClaimUpdatedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
        ((ClaimUpdatedDomainEvent)domainEvent).ClaimId.Should().Be(userAccount.Claims.First().Id);
    }
    
    [Fact]
    public void RemoveClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(userAccount.Claims.First().Id);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void RemoveClaim_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(userAccount.Claims.First().Id);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void RemoveClaim_Throws_ClaimNotFoundException_When_Given_NotExisting_ClaimId()
    {
        // Arrange
        var notExistingGuid = Guid.NewGuid();
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(new ClaimId(notExistingGuid));
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ClaimNotFoundException>();
        ((ClaimNotFoundException)exception!).ClaimId.value.Should().Be(notExistingGuid);
    }
    
    [Fact]
    public void RemoveClaim_Removes_Claim_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(userAccount.Claims.First().Id);
        });

        // Assert
        exception.Should().BeNull();
        var claims = userAccount.Claims;
        claims.Should().NotBeNull().And.BeEmpty();
    }
    
    [Fact]
    public void RemoveClaim_Adds_ClaimRemovedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.AddClaim(string.Empty, default);
        var claimId = userAccount.Claims.First().Id;
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(claimId);
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<ClaimRemovedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((ClaimRemovedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
        ((ClaimRemovedDomainEvent)domainEvent).ClaimId.Should().Be(claimId);
    }
    
    [Fact]
    public void SetNewRefreshToken_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(string.Empty);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void SetNewRefreshToken_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(string.Empty);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void SetNewRefreshToken_Sets_RefreshToken_On_Success()
    {
        // Arrange
        const string refreshTokenValue = "refreshTokenValue";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(refreshTokenValue);
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().NotBeNull();
        userAccount.RefreshToken!.IsActive.Should().BeTrue();
        userAccount.RefreshToken!.Value.Should().Be(refreshTokenValue);
    }
    
    [Fact]
    public void RevokeRefreshToken_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RevokeRefreshToken();
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void RevokeRefreshToken_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RevokeRefreshToken();
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void RevokeRefreshToken_DoesNothing_When_RefreshToken_Is_Null_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RevokeRefreshToken();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().BeNull();
    }
    
    [Fact]
    public void RevokeRefreshToken_Sets_IsActive_False_When_RefreshToken_Is_Not_Null_On_Success()
    {
        // Arrange
        const string refreshTokenValue = "refreshTokenValue";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.SetNewRefreshToken(refreshTokenValue);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RevokeRefreshToken();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().NotBeNull();
        userAccount.RefreshToken!.IsActive.Should().BeFalse();
        userAccount.RefreshToken!.Value.Should().Be(refreshTokenValue);
    }
    
    [Fact]
    public void Lock_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void Lock_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void Lock_Sets_RefreshToken_IsActive_False_When_RefreshToken_Is_Not_Null_On_Success()
    {
        // Arrange
        const string refreshTokenValue = "refreshTokenValue";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.SetNewRefreshToken(refreshTokenValue);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().NotBeNull();
        userAccount.RefreshToken!.IsActive.Should().BeFalse();
        userAccount.RefreshToken!.Value.Should().Be(refreshTokenValue);
    }
    
    [Fact]
    public void Lock_Sets_IsLocked_True_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.IsLocked.Should().BeTrue();
    }
    
    [Fact]
    public void Lock_Adds_UserAccountLockedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<UserAccountLockedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((UserAccountLockedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void Unlock_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void Unlock_DoesNothing_When_IsLocked_Is_False_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.IsLocked.Should().BeFalse();
    }
    
    [Fact]
    public void Unlock_Sets_IsLocked_False_When_IsLocked_Is_True_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.IsLocked.Should().BeFalse();
    }
    
    [Fact]
    public void Unlock_Adds_UserAccountUnlockedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<UserAccountUnlockedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((UserAccountUnlockedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void UpdatePasswordHash_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(string.Empty);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void UpdatePasswordHash_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(string.Empty);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void UpdatePasswordHash_Updates_PasswordHash_On_Success()
    {
        // Arrange
        const string passwordHash = "passwordHash";
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(passwordHash);
        });

        // Assert
        exception.Should().BeNull();
        userAccount.PasswordHash.Should().Be(passwordHash);
    }
    
    [Fact]
    public void UpdatePasswordHash_Adds_PasswordHashUpdatedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(string.Empty);
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<PasswordHashUpdatedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((PasswordHashUpdatedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void Delete_DoesNothing_When_IsDeleted_Is_True_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.IsDeleted.Should().BeTrue();
    }
    
    [Fact]
    public void Delete_Sets_IsDeleted_True_When_IsDeleted_Is_False_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.IsDeleted.Should().BeTrue();
    }
    
    [Fact]
    public void Delete_Adds_UserAccountDeletedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(string.Empty, string.Empty);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<UserAccountDeletedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((UserAccountDeletedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
    }
}