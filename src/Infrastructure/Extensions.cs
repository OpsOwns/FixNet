using FixNet.Infrastructure.Auth;
using FixNet.Infrastructure.EventDispatcher;
using FixNet.Infrastructure.OutBox;
using FixNet.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FixNet.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddAuth(configuration);
        services.AddDatabase(configuration, environment);

        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}