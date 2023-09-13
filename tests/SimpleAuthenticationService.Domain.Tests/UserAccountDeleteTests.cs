using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountDeleteTests
{
    [Fact]
    public void Delete_DoesNothing_When_Status_Is_Deleted_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Status.Should().Be(UserAccountStatus.Deleted);
    }
    
    [Fact]
    public void Delete_Sets_Status_Deleted_When_Status_Is_Active_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Status.Should().Be(UserAccountStatus.Deleted);
    }
    
    [Fact]
    public void Delete_Sets_RefreshToken_IsActive_False_When_RefreshToken_Is_Not_Null_On_Success()
    {
        // Arrange
        const string refreshTokenValue = "refreshTokenValue";
        var refreshTokenExpirationDate = DateTime.UtcNow;
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.SetNewRefreshToken(refreshTokenValue, refreshTokenExpirationDate);

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.Delete();
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().NotBeNull();
        userAccount.RefreshToken!.IsActive.Should().BeFalse();
        userAccount.RefreshToken!.Value.Should().Be(refreshTokenValue);
        userAccount.RefreshToken!.ExpirationDate.Should().Be(refreshTokenExpirationDate);
    }
    
    [Fact]
    public void Delete_Adds_UserAccountDeletedDomainEvent_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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