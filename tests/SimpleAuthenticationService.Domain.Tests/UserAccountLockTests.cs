using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountLockTests
{
    [Fact]
    public void Lock_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
    public void Lock_Sets_Status_Locked_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Lock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Status.Should().Be(UserAccountStatus.Locked);
    }
    
    [Fact]
    public void Lock_Adds_UserAccountLockedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
}