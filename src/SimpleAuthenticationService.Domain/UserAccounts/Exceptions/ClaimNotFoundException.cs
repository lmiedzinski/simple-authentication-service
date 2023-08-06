namespace SimpleAuthenticationService.Domain.UserAccounts.Exceptions;

public sealed class ClaimNotFoundException : Exception
{
    public ClaimId ClaimId { get; private set; }
    public ClaimNotFoundException(ClaimId claimId)
        : base($"Claim with the id {claimId.value} was not found")
    {
        ClaimId = claimId;
    }
}