namespace SimpleAuthenticationService.Application.Abstractions.UserAccounts;

public sealed class UserAccountReadModelDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = null!;
    public UserAccountStatusReadModelDto Status { get; init; }
    public ICollection<ClaimReadModelDto> Claims { get; init; } = new List<ClaimReadModelDto>();
}