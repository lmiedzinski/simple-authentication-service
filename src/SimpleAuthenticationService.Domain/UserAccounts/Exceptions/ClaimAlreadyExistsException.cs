namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class ClaimAlreadyExistsException : Exception
{
    public Claim Claim { get; private set; }
    
    public ClaimAlreadyExistsException(Claim claim)
        : base($"Claim with the type {claim.Type} and value {claim.Value} already exists")
    {
        Claim = claim;
    }
}