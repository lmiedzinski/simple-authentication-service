using SimpleAuthenticationService.Application.Abstractions.DateAndTime;

namespace SimpleAuthenticationService.Infrastructure.DateAndTime;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}