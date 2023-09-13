using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountSetNewRefreshTokenTests
{
    [Fact]
    public void SetNewRefreshToken_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Lock();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(string.Empty, DateTime.UtcNow);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LockedUserAccountUpdatesNotAllowedException>();
        ((LockedUserAccountUpdatesNotAllowedException)exception!).UserAccountId.Should().Be(userAccount.Id);
    }

    [Fact]
    public void SetNewRefreshToken_Throws_DeletedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Deleted()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.Delete();

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(string.Empty, DateTime.UtcNow);
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
        var refreshTokenExpirationDate = DateTime.UtcNow;
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount.SetNewRefreshToken(refreshTokenValue, refreshTokenExpirationDate);
        });

        // Assert
        exception.Should().BeNull();
        userAccount.RefreshToken.Should().NotBeNull();
        userAccount.RefreshToken!.IsActive.Should().BeTrue();
        userAccount.RefreshToken!.Value.Should().Be(refreshTokenValue);
        userAccount.RefreshToken!.ExpirationDateUtc.Should().Be(refreshTokenExpirationDate);
    }
}