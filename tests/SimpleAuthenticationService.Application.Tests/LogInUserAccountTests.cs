using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.Tests;

public class LogInUserAccountTests
{
    #region TestsSetup

    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandHandler<LogInUserAccountCommand, LogInUserAccountResponse> _commandHandler;

    public LogInUserAccountTests()
    {
        _userAccountRepository = Substitute.For<IUserAccountRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _cryptographyService = Substitute.For<ICryptographyService>();
        _tokenService = Substitute.For<ITokenService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _commandHandler = new LogInUserAccountCommandHandler(
            _userAccountRepository,
            _unitOfWork,
            _cryptographyService,
            _tokenService,
            _dateTimeProvider);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_UserAccountNotFoundOrGivenPasswordIsIncorrectException_When_UserAccount_Is_Null()
    {
        // Arrange
        var command = new LogInUserAccountCommand("login", "password");
        _userAccountRepository.GetByLoginAsync(new Login(command.Login)).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<UserAccountNotFoundOrGivenPasswordIsIncorrectException>();
        exception!.Message.Should().Contain(command.Login);
    }
    
    [Fact]
    public async Task Handle_Throws_UserAccountNotFoundOrGivenPasswordIsIncorrectException_When_Password_Is_Incorrect()
    {
        // Arrange
        const string passwordHash = "passwordHash";
        var command = new LogInUserAccountCommand("login", "password");
        _userAccountRepository.GetByLoginAsync(new Login(command.Login))
            .Returns(UserAccount.Create(new Login(command.Login), new PasswordHash(passwordHash)));
        _cryptographyService.HashPassword(command.Password).Returns("wrongPasswordHash");
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<UserAccountNotFoundOrGivenPasswordIsIncorrectException>();
        exception!.Message.Should().Contain(command.Login);
    }
    
    [Fact]
    public async Task Handle_Returns_LogInUserAccountResponse_On_Success()
    {
        // Arrange
        const string passwordHash = "passwordHash";
        const string refreshToken = "refreshToken";
        const string accessToken = "accessToken";
        const int refreshTokenLifespanInMinutes = 128;
        var dateTimeNow = DateTime.UtcNow;
        var command = new LogInUserAccountCommand("login", "password");
        var userAccount = UserAccount.Create(new Login(command.Login), new PasswordHash(passwordHash));
        _userAccountRepository.GetByLoginAsync(new Login(command.Login))
            .Returns(userAccount);
        _cryptographyService.HashPassword(command.Password).Returns(passwordHash);
        _tokenService.GenerateRefreshToken().Returns(refreshToken);
        _tokenService.GetRefreshTokenLifetimeInMinutes().Returns(refreshTokenLifespanInMinutes);
        _tokenService.GenerateAccessToken(userAccount.Id, Arg.Any<IReadOnlyCollection<Claim>>()).Returns(accessToken);
        _dateTimeProvider.UtcNow.Returns(dateTimeNow);
        
        // Act
        LogInUserAccountResponse? result = null;
        var exception = await Record.ExceptionAsync(async () =>
        {
            result = await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().BeNull();
        result.Should().NotBeNull();
        result!.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshToken);
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        const string passwordHash = "passwordHash";
        const string refreshToken = "refreshToken";
        const string accessToken = "accessToken";
        const int refreshTokenLifespanInMinutes = 128;
        var dateTimeNow = DateTime.UtcNow;
        var command = new LogInUserAccountCommand("login", "password");
        var userAccount = UserAccount.Create(new Login(command.Login), new PasswordHash(passwordHash));
        _userAccountRepository.GetByLoginAsync(new Login(command.Login))
            .Returns(userAccount);
        _cryptographyService.HashPassword(command.Password).Returns(passwordHash);
        _tokenService.GenerateRefreshToken().Returns(refreshToken);
        _tokenService.GetRefreshTokenLifetimeInMinutes().Returns(refreshTokenLifespanInMinutes);
        _tokenService.GenerateAccessToken(userAccount.Id, Arg.Any<IReadOnlyCollection<Claim>>()).Returns(accessToken);
        _dateTimeProvider.UtcNow.Returns(dateTimeNow);
        
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