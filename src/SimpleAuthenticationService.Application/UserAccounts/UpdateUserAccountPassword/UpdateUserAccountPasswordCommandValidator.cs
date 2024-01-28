using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateUserAccountPassword;

internal sealed class UpdateUserAccountPasswordCommandValidator : AbstractValidator<UpdateUserAccountPasswordCommand>
{
    public UpdateUserAccountPasswordCommandValidator()
    {
        RuleFor(x => x.UserAccountId).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}