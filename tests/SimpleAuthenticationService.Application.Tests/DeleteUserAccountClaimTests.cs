using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using Xunit;

namespace SimpleAuthenticationService.Application.Tests;

public class DeleteUserAccountClaimTests
{
    #region TestsSetup

    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandHandler<RemoveUserAccountClaimCommand> _commandHandler;

    public DeleteUserAccountClaimTests()
    {
        _userAccountWriteRepository = Substitute.For<IUserAccountWriteRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _commandHandler = new RemoveUserAccountClaimCommandHandler(
            _userAccountWriteRepository,
            _unitOfWork);
    }

    #endregion
    
    [Fact]
    public async Task Handle_Throws_NotFoundException_When_UserAccount_Not_Exists()
    {
        // Arrange
        var command = new RemoveUserAccountClaimCommand(Guid.NewGuid(), string.Empty, string.Empty);
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
        var claim = new Claim("type", "value");
        userAccount.AddClaim(claim);
        var command = new RemoveUserAccountClaimCommand(userAccount.Id.Value, claim.Type, claim.Value);
        
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