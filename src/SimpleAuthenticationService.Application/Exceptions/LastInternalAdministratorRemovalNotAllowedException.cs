namespace SimpleAuthenticationService.Application.Exceptions;

public class LastInternalAdministratorRemovalNotAllowedException : Exception
{
    public LastInternalAdministratorRemovalNotAllowedException()
        : base("Last internal administrator removal not allowed")
    {
    }
}