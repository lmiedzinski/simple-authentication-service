namespace SimpleAuthenticationService.Application.Abstractions.DateAndTime;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}