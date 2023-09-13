using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Exceptions;
using Xunit;

namespace SimpleAuthenticationService.Domain.Tests;

public class UserAccountRevokeRefreshTokenTests
{
    [Fact]
    public void RevokeRefreshToken_Throws_LockedUserAccountUpdatesNotAllowedException_When_UserAccount_Is_Locked()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
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
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));

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
        var refreshTokenExpirationDate = DateTime.UtcNow;
        var userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        userAccount.SetNewRefreshToken(refreshTokenValue, refreshTokenExpirationDate);

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
        userAccount.RefreshToken!.ExpirationDate.Should().Be(refreshTokenExpirationDate);
    }
}