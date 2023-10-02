using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.Abstractions.UserAccounts;

public interface IUserAccountReadService
{
    Task<bool> ExistsByLoginAsync(Login login);
}