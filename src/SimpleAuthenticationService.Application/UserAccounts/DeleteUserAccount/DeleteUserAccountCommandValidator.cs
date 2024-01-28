using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.DeleteUserAccount;

internal sealed class DeleteUserAccountCommandValidator : AbstractValidator<DeleteUserAccountCommand>
{
    public DeleteUserAccountCommandValidator()
    {
        RuleFor(x => x.UserAccountId)
            .NotEmpty();
    }
}