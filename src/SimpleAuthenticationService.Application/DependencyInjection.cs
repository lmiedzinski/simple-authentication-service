using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimpleAuthenticationService.Application.Behaviors;

namespace SimpleAuthenticationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<ApplicationAssembly>();

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly);

        return services;
    }
}