using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.UpdateCurrentLoggedInUserAccountPassword;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class UpdateCurrentLoggedInUserAccountPasswordTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ICryptographyService _cryptographyService;
    private readonly ICommandHandler<UpdateCurrentLoggedInUserAccountPasswordCommand> _commandHandler;

    public UpdateCurrentLoggedInUserAccountPasswordTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _tokenService = Substitute.For<ITokenService>();
        _cryptographyService = Substitute.For<ICryptographyService>();

        _commandHandler = new UpdateCurrentLoggedInUserAccountPasswordCommandHandler(
            _userAccountWriteRepository,
            _unitOfWork,
            _cryptographyService,
            _tokenService);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Not_Exists()
    {
        // Arrange
        var command = new UpdateCurrentLoggedInUserAccountPasswordCommand(string.Empty, string.Empty);
        var userId = Guid.NewGuid();
        _tokenService.GetUserAccountIdFromContext().Returns(new UserAccountId(userId));
        _userAccountWriteRepository.GetByIdAsync(new UserAccountId(userId)).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<NotFoundException>();
        exception!.Message.Should().Contain(userId.ToString());
    }
    
    [Fact]
    public async Task Handle_Throws_IncorrectPasswordException_When_CurrentPassword_Is_Incorrect()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        var command = new UpdateCurrentLoggedInUserAccountPasswordCommand(string.Empty, string.Empty);
        _tokenService.GetUserAccountIdFromContext().Returns(userAccount.Id);
        _userAccountWriteRepository.GetByIdAsync(userAccount.Id).Returns(userAccount);
        _cryptographyService.IsPasswordMatchingHash(command.CurrentPassword, userAccount.PasswordHash.Value)
            .Returns(false);
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<IncorrectPasswordException>();
        exception!.Message.Should().Contain(userAccount.Id.Value.ToString());
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        var command = new UpdateCurrentLoggedInUserAccountPasswordCommand(string.Empty, string.Empty);
        _tokenService.GetUserAccountIdFromContext().Returns(userAccount.Id);
        _userAccountWriteRepository.GetByIdAsync(userAccount.Id).Returns(userAccount);
        _cryptographyService.IsPasswordMatchingHash(command.CurrentPassword, userAccount.PasswordHash.Value)
            .Returns(true);
        _cryptographyService.HashPassword(command.NewPassword).Returns("newPasswordHash");
        
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