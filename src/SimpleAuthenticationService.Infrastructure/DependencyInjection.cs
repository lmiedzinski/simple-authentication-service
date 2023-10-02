using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.Cryptography;
using SimpleAuthenticationService.Infrastructure.DateAndTime;
using SimpleAuthenticationService.Infrastructure.EntityFramework;
using SimpleAuthenticationService.Infrastructure.EntityFramework.Repositories;
using SimpleAuthenticationService.Infrastructure.OutboxPattern;
using SimpleAuthenticationService.Infrastructure.SqlConnection;
using SimpleAuthenticationService.Infrastructure.Token;
using SimpleAuthenticationService.Infrastructure.UserAccounts;

namespace SimpleAuthenticationService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));
        services.Configure<OutboxPatternOptions>(configuration.GetSection("OutboxPatternOptions"));

        var connectionString =
            configuration.GetConnectionString("SimpleAuthenticationServiceDatabase") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        
        services.AddSingleton(_ => new SqlConnectionFactory(connectionString));

        services.AddSingleton<RsaKeysProvider>();

        services.AddScoped<ICryptographyService, CryptographyService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAccountReadService, UserAccountReadService>();
        services.AddScoped<IUserAccountWriteRepository, UserAccountWriteRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        services.AddQuartz();
        services.ConfigureOptions<QuartzOptionsSetup>();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
    }   
}