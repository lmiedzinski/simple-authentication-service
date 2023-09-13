using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.BackgroundJobs;
using SimpleAuthenticationService.Infrastructure.Cryptography;
using SimpleAuthenticationService.Infrastructure.DateAndTime;
using SimpleAuthenticationService.Infrastructure.EntityFramework;
using SimpleAuthenticationService.Infrastructure.EntityFramework.Repositories;
using SimpleAuthenticationService.Infrastructure.OutboxPattern;
using SimpleAuthenticationService.Infrastructure.SqlConnection;
using SimpleAuthenticationService.Infrastructure.Token;

namespace SimpleAuthenticationService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenServiceOptions>(configuration.GetSection("TokenServiceOptions"));
        services.Configure<OutboxPatternOptions>(configuration.GetSection("OutboxPatternOptions"));
        
        var outboxPatternOptions = configuration.GetValue<OutboxPatternOptions>("OutboxPatternOptions") ??
                                   throw new ArgumentNullException(nameof(configuration));
        
        var connectionString =
            configuration.GetConnectionString("SimpleAuthenticationServiceDatabase") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddSingleton(_ => new SqlConnectionFactory(connectionString));
        
        services.AddSingleton(provider =>
        {
            var tokenServiceOptions = provider.GetRequiredService<IOptions<TokenServiceOptions>>().Value;
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(
                Convert.FromBase64String(tokenServiceOptions.AccessTokenOptions.PrivateRsaCertificate),
                out _);
            return new RsaSecurityKey(rsa);
        });

        services.AddScoped<ICryptographyService, CryptographyService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        services.AddQuartz(options =>
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
                                .WithIntervalInSeconds(outboxPatternOptions.IntervalInSeconds)
                                .RepeatForever();
                        });
                });
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }   
}