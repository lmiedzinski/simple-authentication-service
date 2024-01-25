using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.LogOutUserAccount;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class LogOutUserAccountTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ICommandHandler<LogOutUserAccountCommand> _commandHandler;

    public LogOutUserAccountTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _tokenService = Substitute.For<ITokenService>();

        _commandHandler = new LogOutUserAccountCommandHandler(
            _tokenService,
            _userAccountWriteRepository,
            _unitOfWork);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Is_Null()
    {
        // Arrange
        var command = new LogOutUserAccountCommand();
        var userAccountId = new UserAccountId(Guid.NewGuid());
        _tokenService.GetUserAccountIdFromContext().Returns(userAccountId);
        _userAccountWriteRepository.GetByIdAsync(userAccountId).ReturnsNull();
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<NotFoundException>();
        exception!.Message.Should().Contain(nameof(UserAccount)).And.Contain(userAccountId.Value.ToString());
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        var command = new LogOutUserAccountCommand();
        var userAccountId = new UserAccountId(Guid.NewGuid());
        _tokenService.GetUserAccountIdFromContext().Returns(userAccountId);
        _userAccountWriteRepository.GetByIdAsync(userAccountId)
            .Returns(UserAccount.Create(new Login("login"), new PasswordHash("passwordHash")));
        
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