using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.UnlockUserAccount;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class UnlockUserAccountTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandHandler<UnlockUserAccountCommand> _commandHandler;

    public UnlockUserAccountTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _commandHandler = new UnlockUserAccountCommandHandler(
            _userAccountWriteRepository,
            _unitOfWork);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Not_Exists()
    {
        // Arrange
        var command = new UnlockUserAccountCommand(Guid.NewGuid());
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
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        var userAccount = UserAccount.Create(new Login("login"), new PasswordHash("passwordHash"));
        var command = new UnlockUserAccountCommand(userAccount.Id.Value);
        
        _userAccountWriteRepository.GetByIdAsync(new UserAccountId(command.UserAccountId)).Returns(userAccount);
        
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