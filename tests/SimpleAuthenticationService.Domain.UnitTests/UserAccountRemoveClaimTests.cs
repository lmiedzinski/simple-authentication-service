using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.UnitTests;

public class UserAccountRemoveClaimTests
{
    [Fact]
    public void RemoveClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var claim = new Claim(string.Empty, default);
        userAccount.AddClaim(claim);
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(claim);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void RemoveClaim_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var claim = new Claim(string.Empty, default);
        userAccount.AddClaim(claim);
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(claim);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void RemoveClaim_Throws_ClaimNotFoundException_When_Given_NotExisting_ClaimId()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var existingClaim = new Claim(string.Empty, default);
        var notExistingClaim = new Claim("NotExistingType", "NotExistingValue");
        userAccount.AddClaim(existingClaim);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(notExistingClaim);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ClaimNotFoundException>();
        ((ClaimNotFoundException)exception!).Claim.Should().Be(notExistingClaim);
    }
    
    [Fact]
    public void RemoveClaim_Removes_Claim_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var claim = new Claim(string.Empty, default);
        userAccount.AddClaim(claim);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(claim);
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var claim = new Claim(string.Empty, default);
        userAccount.AddClaim(claim);
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.RemoveClaim(claim);
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<ClaimRemovedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((ClaimRemovedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
        ((ClaimRemovedDomainEvent)domainEvent).Claim.Should().Be(claim);
    }
}