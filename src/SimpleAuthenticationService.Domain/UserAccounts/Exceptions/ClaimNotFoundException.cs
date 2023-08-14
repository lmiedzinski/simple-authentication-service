namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class ClaimNotFoundException : Exception
{
    public Claim Claim { get; private set; }
    
    public ClaimNotFoundException(Claim claim)
        : base($"Claim with the type {claim.Type} and value {claim.Value} was not found")
    {
        Claim = claim;
    }
}