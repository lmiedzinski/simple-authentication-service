using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;

internal sealed class CreateUserAccountCommandValidator : AbstractValidator<CreateUserAccountCommand>
{
    public CreateUserAccountCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}