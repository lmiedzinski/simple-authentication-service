namespace SimpleAuthenticationService.Domain.UserAccounts;

public class Claim
{
    private Claim()
    {
        
    }

    internal Claim(ClaimId id, string type, string? value)
    {
        Id = id;
        Type = type;
        Value = value;
    }
    
    public ClaimId Id { get; private set; }
    public string Type { get; private set; }
    public string? Value { get; private set; }

    internal void Update(string type, string? value)
    {
        Type = type;
        Value = value;
    }
}