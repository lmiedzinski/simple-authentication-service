using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Infrastructure.Authorization;
using SimpleAuthenticationService.Infrastructure.Cryptography;
using SimpleAuthenticationService.Infrastructure.Database;
using SimpleAuthenticationService.Infrastructure.DateAndTime;
using SimpleAuthenticationService.Infrastructure.Middlewares;
using SimpleAuthenticationService.Infrastructure.OutboxPattern;
using SimpleAuthenticationService.Infrastructure.Swagger;
using SimpleAuthenticationService.Infrastructure.Token;

namespace SimpleAuthenticationService.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        
        services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));
        services.Configure<OutboxPatternOptions>(configuration.GetSection("OutboxPatternOptions"));

        services.AddSingleton<RsaKeysProvider>();

        services.AddScoped<ICryptographyService, CryptographyService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ITokenService, TokenService>();
        
        services.AddQuartz();
        services.ConfigureOptions<QuartzOptionsSetup>();
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.InternalAdministrator,
                policy => policy.RequireClaim(AuthorizationPolicies.InternalAdministrator, "true"));
        });
        
        services.AddEndpointsApiExplorer();
        services.AddConfiguredSwagger();
        
        services.AddHttpContextAccessor();
        services.AddHealthChecks();
        
        services.AddControllers();
    }

    public static void UseInfrastructure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseConfiguredSwagger();
        
        app.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
    }
}