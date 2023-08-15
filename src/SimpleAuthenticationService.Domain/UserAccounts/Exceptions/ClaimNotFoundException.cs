using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class ClaimNotFoundException : DomainException
{
    public Claim Claim { get; private set; }
    
    public ClaimNotFoundException(Claim claim)
        : base($"Claim with the type {claim.Type} and value {claim.Value} was not found")
    {
        Claim = claim;
    }
}