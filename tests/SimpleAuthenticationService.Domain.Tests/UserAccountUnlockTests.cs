using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountUnlockTests
{
    [Fact]
    public void Unlock_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
    public void Unlock_DoesNothing_When_Status_Is_Locked_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Status.Should().Be(UserAccountStatus.Active);
    }
    
    [Fact]
    public void Unlock_Sets_Status_Active_When_Status_Is_Locked_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Unlock();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Status.Should().Be(UserAccountStatus.Active);
    }
    
    [Fact]
    public void Unlock_Adds_UserAccountUnlockedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
}