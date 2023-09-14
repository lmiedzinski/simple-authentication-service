using Microsoft.Extensions.Options;
using Quartz;
using SimpleAuthenticationService.Infrastructure.BackgroundJobs;

namespace SimpleAuthenticationService.Infrastructure.OutboxPattern;

internal sealed class QuartzOptionsSetup : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxPatternOptions _outboxPatternOptions;

    public QuartzOptionsSetup(IOptions<OutboxPatternOptions> outboxPatternOptions)
    {
        _outboxPatternOptions = outboxPatternOptions.Value;
    }

    public void Configure(QuartzOptions options)
    {
        const string outboxPatternBackgroundJobName = nameof(OutboxPatternBackgroundJob);
        options.AddJob<OutboxPatternBackgroundJob>(configure =>
            {
                configure
                    .WithIdentity(outboxPatternBackgroundJobName);
            })
            .AddTrigger(configure =>
            {
                configure
                    .ForJob(outboxPatternBackgroundJobName)
                    .WithSimpleSchedule(schedule =>
                    {
                        schedule
                            .WithIntervalInSeconds(_outboxPatternOptions.IntervalInSeconds)
                            .RepeatForever();
                    });
            });
    }
}