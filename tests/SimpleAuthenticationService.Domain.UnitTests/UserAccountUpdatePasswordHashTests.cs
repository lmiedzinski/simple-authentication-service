using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.UnitTests;

public class UserAccountUpdatePasswordHashTests
{
    [Fact]
    public void UpdatePasswordHash_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(new PasswordHash(string.Empty));
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void UpdatePasswordHash_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(new PasswordHash(passwordHash));
        });

        // Assert
        exception.Should().BeNull();
        userAccount.PasswordHash.Should().NotBeNull();
        userAccount.PasswordHash.Value.Should().Be(passwordHash);
    }
    
    [Fact]
    public void UpdatePasswordHash_Adds_PasswordHashUpdatedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.ClearDomainEvents();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.UpdatePasswordHash(new PasswordHash(string.Empty));
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
}