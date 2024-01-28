using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.UnlockUserAccount;

internal sealed class UnlockUserAccountCommandValidator : AbstractValidator<UnlockUserAccountCommand>
{
    public UnlockUserAccountCommandValidator()
    {
        RuleFor(x => x.UserAccountId)
            .NotEmpty();
    }
}