using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;

internal sealed class RefreshUserAccountTokenCommandValidator : AbstractValidator<RefreshUserAccountTokenCommand>
{
    public RefreshUserAccountTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}