using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.LockUserAccount;

internal sealed class LockUserAccountCommandValidator : AbstractValidator<LockUserAccountCommand>
{
    public LockUserAccountCommandValidator()
    {
        RuleFor(x => x.UserAccountId)
            .NotEmpty();
    }
}