using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.UpdateUserAccountPassword;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class UpdateUserAccountPasswordTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;
    private readonly ICommandHandler<UpdateUserAccountPasswordCommand> _commandHandler;

    public UpdateUserAccountPasswordTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _cryptographyService = Substitute.For<ICryptographyService>();
        _tokenService = Substitute.For<ITokenService>();

        _commandHandler = new UpdateUserAccountPasswordCommandHandler(
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
        var command = new UpdateUserAccountPasswordCommand(Guid.NewGuid(), string.Empty);
        _userAccountWriteRepository.GetByIdAsync(new UserAccountId(command.UserAccountId)).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<NotFoundException>();
        exception!.Message.Should().Contain(command.UserAccountId.ToString());
    }
    
    [Fact]
    public async Task Handle_Throws_SelfOperationNotAllowedException_When_UserAccountId_Is_UserAccountIdFromContext()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        var command = new UpdateUserAccountPasswordCommand(userAccount.Id.Value, "newPassword123");
        
        _userAccountWriteRepository.GetByIdAsync(new UserAccountId(command.UserAccountId)).Returns(userAccount);
        _tokenService.GetUserAccountIdFromContext().Returns(userAccount.Id);
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<SelfOperationNotAllowedException>();
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        var command = new UpdateUserAccountPasswordCommand(userAccount.Id.Value, "newPassword123");
        _userAccountWriteRepository.GetByIdAsync(userAccount.Id).Returns(userAccount);
        _cryptographyService.HashPassword(command.NewPassword).Returns("newPasswordHash");
        _tokenService.GetUserAccountIdFromContext().Returns(new UserAccountId(Guid.Empty));
        
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