using FluentValidation;

namespace SimpleAuthenticationService.Application.UserAccounts.AddUserAccountClaim;

internal sealed class AddUserAccountClaimCommandValidator : AbstractValidator<AddUserAccountClaimCommand>
{
    public AddUserAccountClaimCommandValidator()
    {
        RuleFor(x => x.UserAccountId)
            .NotEmpty();

        RuleFor(x => x.ClaimType)
            .NotEmpty();
    }
}