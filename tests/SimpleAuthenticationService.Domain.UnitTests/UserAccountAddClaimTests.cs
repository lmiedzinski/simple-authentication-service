using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.UnitTests;

public class UserAccountAddClaimTests
{
    [Fact]
    public void AddClaim_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.AddClaim(new Claim(string.Empty, default));
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void AddClaim_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Delete();
        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.AddClaim(new Claim(string.Empty, default));
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<DeletedUserAccountUpdatesNotAllowedException>();
        ((DeletedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }
    
    [Fact]
    public void AddClaim_Throws_ClaimAlreadyExistsException_When_Duplicated_Claim_Is_Added()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        var claim = new Claim(string.Empty, default);
        userAccount.AddClaim(claim);
        
        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.AddClaim(claim);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ClaimAlreadyExistsException>();
        ((ClaimAlreadyExistsException)exception!).Claim.Should().Be(claim);
    }

    [Fact]
    public void AddClaim_Adds_Claim_On_Success()
    {
        // Arrange
        const string claimType = "claimType";
        const string claimValue = "claimValue";
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.AddClaim(new Claim(claimType, claimValue));
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
    public void AddClaim_Adds_ClaimAddedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.AddClaim(new Claim(string.Empty, default));
        });

        // Assert
        exception.Should().BeNull();
        var domainEvents = userAccount.GetDomainEvents();
        domainEvents.Should().NotBeNull().And.HaveCount(1);
        var domainEvent = domainEvents.First();
        domainEvent.Should().BeOfType<ClaimAddedDomainEvent>();
        domainEvent.Id.Should().NotBe(Guid.Empty);
        ((ClaimAddedDomainEvent)domainEvent).UserAccountId.Should().Be(userAccount.Id);
        ((ClaimAddedDomainEvent)domainEvent).Claim.Should().Be(userAccount.Claims[0]);
    }
}