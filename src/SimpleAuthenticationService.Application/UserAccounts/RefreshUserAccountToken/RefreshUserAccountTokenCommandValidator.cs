using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;

public class RefreshUserAccountTokenCommandValidator : AbstractValidator<RefreshUserAccountTokenCommand>
{
    public RefreshUserAccountTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}