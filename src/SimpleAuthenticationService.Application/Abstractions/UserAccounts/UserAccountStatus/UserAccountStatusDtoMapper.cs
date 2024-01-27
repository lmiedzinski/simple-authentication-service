namespace SimpleAuthenticationService.Application.Abstractions.UserAccounts.UserAccountStatus;

public static class UserAccountStatusDtoMapper
{
    public static UserAccountStatusDto ToApplication(this Domain.UserAccounts.UserAccountStatus status)
    {
        return status switch
        {
            Domain.UserAccounts.UserAccountStatus.Active => UserAccountStatusDto.Active,
            Domain.UserAccounts.UserAccountStatus.Locked => UserAccountStatusDto.Locked,
            Domain.UserAccounts.UserAccountStatus.Deleted => UserAccountStatusDto.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
    
    public static Domain.UserAccounts.UserAccountStatus ToDomain(this UserAccountStatusDto status)
    {
        return status switch
        {
            UserAccountStatusDto.Active => Domain.UserAccounts.UserAccountStatus.Active,
            UserAccountStatusDto.Locked => Domain.UserAccounts.UserAccountStatus.Locked,
            UserAccountStatusDto.Deleted => Domain.UserAccounts.UserAccountStatus.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}