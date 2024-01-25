using FluentAssertions;
using NSubstitute;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.UnitTests;

public class CreateUserAccountTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccountReadService _userAccountReadService;
    private readonly ICryptographyService _cryptographyService;
    private readonly ICommandHandler<CreateUserAccountCommand> _commandHandler;

    public CreateUserAccountTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _userAccountReadService = Substitute.For<IUserAccountReadService>();
        _cryptographyService = Substitute.For<ICryptographyService>();

        _commandHandler = new CreateUserAccountCommandHandler(
            _userAccountReadService,
            _cryptographyService,
            _userAccountWriteRepository,
            _unitOfWork);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_LoginAlreadyTakenException_When_Login_Already_Exists()
    {
        // Arrange
        var command = new CreateUserAccountCommand("login", "password");
        _userAccountReadService.ExistsByLoginAsync(new Login(command.Login)).Returns(true);
        
        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _commandHandler.Handle(command, CancellationToken.None);
        });

        // Assert
        exception.Should().NotBeNull().And.BeOfType<LoginAlreadyTakenException>();
        exception!.Message.Should().Contain(command.Login);
    }
    
    [Fact]
    public async Task Handle_Calls_Once_UnitOfWork_On_Success()
    {
        // Arrange
        var command = new CreateUserAccountCommand("login", "password");
        _userAccountReadService.ExistsByLoginAsync(new Login(command.Login)).Returns(false);

        
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