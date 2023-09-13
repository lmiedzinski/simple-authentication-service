using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;

public record LogInUserAccountCommand(string Login, string Password)
    :ICommand<LogInUserAccountResponse>;