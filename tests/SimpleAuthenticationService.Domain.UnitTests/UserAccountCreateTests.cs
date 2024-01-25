using FluentAssertions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts.Events;
using Xunit;

namespace SimpleAuthenticationService.Domain.UnitTests;

public class UserAccountCreateTests
{
    [Fact]
    public void Create_Returns_UserAccount_Object_With_Correct_Properties_On_Success()
    {
        // Arrange
        const string login = "userlogin@domain.com";
        const string passwordHash = "SuperSecretPasswordHash";
        UserAccount? userAccount = null;

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount = UserAccount.Create(new Login(login), new PasswordHash(passwordHash));
        });

        // Assert
        exception.Should().BeNull();
        userAccount.Should().NotBeNull();
        userAccount!.Login.Should().NotBeNull();
        userAccount!.Login.Value.Should().Be(login);
        userAccount!.PasswordHash.Should().NotBeNull();
        userAccount!.PasswordHash.Value.Should().Be(passwordHash);
        userAccount!.Id.Should().NotBeNull().And.NotBe(Guid.Empty);
        userAccount!.Status.Should().Be(UserAccountStatus.Active);
        userAccount!.RefreshToken.Should().BeNull();
        userAccount!.Claims.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Create_Adds_UserAccountCreatedDomainEvent_On_Success()
    {
        // Arrange
        UserAccount? userAccount = null;

        // Act
        var exception = Record.Exception(() =>
        {
            userAccount = UserAccount.Create(new Login(string.Empty), new PasswordHash(string.Empty));
        });

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
}