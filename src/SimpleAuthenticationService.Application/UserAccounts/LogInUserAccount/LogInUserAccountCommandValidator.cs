using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;

internal sealed class LogInUserAccountCommandValidator : AbstractValidator<LogInUserAccountCommand>
{
    public LogInUserAccountCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}