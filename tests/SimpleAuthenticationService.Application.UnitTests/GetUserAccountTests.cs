using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.GetUserAccount;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class GetUserAccountTests
{
    #region TestsSetup

    private readonly IUserAccountReadService _userAccountReadService;
    private readonly IQueryHandler<
        GetUserAccountQuery,
        GetUserAccountQueryResponse> _queryHandler;

    public GetUserAccountTests()
    {
        _userAccountReadService = Substitute.For<IUserAccountReadService>();

        _queryHandler = new GetUserAccountQueryHandler(
            _userAccountReadService);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Not_Exists()
    {
        // Arrange
        var query = new GetUserAccountQuery(Guid.NewGuid());
        _userAccountReadService.GetUserAccountByIdAsync(new UserAccountId(query.UserAccountId)).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _queryHandler.Handle(query, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<NotFoundException>();
        exception!.Message.Should().Contain(query.UserAccountId.ToString());
    }
}