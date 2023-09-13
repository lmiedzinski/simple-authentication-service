namespace SimpleAuthenticationService.Infrastructure.OutboxPattern;

public sealed class OutboxPatternOptions
{
    public int IntervalInSeconds { get; init; }

    public int BatchSize { get; init; }
}