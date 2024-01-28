using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateCurrentLoggedInUserAccountPassword;

internal sealed class UpdateCurrentLoggedInUserAccountPasswordCommandValidator : AbstractValidator<UpdateCurrentLoggedInUserAccountPasswordCommand>
{
    public UpdateCurrentLoggedInUserAccountPasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}