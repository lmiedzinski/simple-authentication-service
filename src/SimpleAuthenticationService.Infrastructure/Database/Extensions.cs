using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.Database.EntityFramework;
using SimpleAuthenticationService.Infrastructure.Database.EntityFramework.Repositories;
using SimpleAuthenticationService.Infrastructure.Database.SqlConnection;
using SimpleAuthenticationService.Infrastructure.UserAccounts;

namespace SimpleAuthenticationService.Infrastructure.Database;

internal static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("SimpleAuthenticationServiceDatabase") ??
            throw new ArgumentNullException(nameof(configuration));
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .EnableSensitiveDataLogging();
        });
        
        services.AddSingleton(_ => new SqlConnectionFactory(connectionString));
        
        services.AddScoped<IUserAccountReadService, UserAccountReadService>();
        services.AddScoped<IUserAccountWriteRepository, UserAccountWriteRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        services.AddHostedService<DatabaseInitializer>();

        return services;
    }
}