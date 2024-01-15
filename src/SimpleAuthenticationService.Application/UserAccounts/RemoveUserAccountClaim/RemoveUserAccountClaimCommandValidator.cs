using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;

internal sealed class RemoveUserAccountClaimCommandValidator : AbstractValidator<RemoveUserAccountClaimCommand>
{
    public RemoveUserAccountClaimCommandValidator()
    {
        RuleFor(x => x.UserAccountId)
            .NotEmpty();

        RuleFor(x => x.ClaimType)
            .NotEmpty();
    }
}