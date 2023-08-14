using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountUpdateClaimTests
{
    [Fact]
    public void UpdateClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
}