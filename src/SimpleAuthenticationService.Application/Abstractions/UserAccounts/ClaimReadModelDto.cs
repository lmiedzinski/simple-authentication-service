namespace SimpleAuthenticationService.Application.Abstractions.UserAccounts;

public sealed class ClaimReadModelDto
{
    public string Type { get; init; } = null!;
    public string? Value { get; init; }
}