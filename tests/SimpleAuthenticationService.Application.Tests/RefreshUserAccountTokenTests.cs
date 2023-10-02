using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.Tests;

public class RefreshUserAccountTokenTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandHandler<RefreshUserAccountTokenCommand, RefreshUserAccountTokenResponse> _commandHandler;

    public RefreshUserAccountTokenTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _tokenService = Substitute.For<ITokenService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _commandHandler = new RefreshUserAccountTokenCommandHandler(
            _userAccountWriteRepository,
            _unitOfWork,
            _tokenService,
            _dateTimeProvider);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Is_Null()
    {
        // Arrange
        var command = new RefreshUserAccountTokenCommand("refreshToken");
        _userAccountWriteRepository.GetByActiveRefreshTokenValueAsync(command.RefreshToken).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<NotFoundException>();
        exception!.Message.Should().Contain(nameof(RefreshToken));
    }
    
    [Fact]
    public async Task Handle_Returns_RefreshUserAccountTokenResponse_On_Success()
    {
        // Arrange
        const string refreshToken = "refreshToken";
        const string accessToken = "accessToken";
        var command = new RefreshUserAccountTokenCommand(refreshToken);
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        userAccount.SetNewRefreshToken(refreshToken, DateTime.UtcNow);
        _userAccountWriteRepository.GetByActiveRefreshTokenValueAsync(refreshToken)
            .Returns(userAccount);
        _tokenService.GenerateAccessToken(userAccount.Id, Arg.Any<IReadOnlyCollection<Claim>>())
            .Returns(accessToken);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        
        // Act
        RefreshUserAccountTokenResponse? result = null;
        var exception = await Record.ExceptionAsync(async () =>
        {
            result = await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().BeNull();
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be(accessToken);
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        const string refreshToken = "refreshToken";
        var command = new RefreshUserAccountTokenCommand(refreshToken);
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        userAccount.SetNewRefreshToken(refreshToken, DateTime.UtcNow);
        _userAccountWriteRepository.GetByActiveRefreshTokenValueAsync(refreshToken)
            .Returns(userAccount);
        _tokenService.GenerateAccessToken(userAccount.Id, Arg.Any<IReadOnlyCollection<Claim>>())
            .Returns("accessToken");
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().BeNull();
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}