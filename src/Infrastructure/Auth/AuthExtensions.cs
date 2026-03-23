using FixNet.Application.Users.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace FixNet.Infrastructure.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakSettings>(configuration.GetSection(KeycloakSettings.SectionName));

        services.AddSingleton(TimeProvider.System);
        services.AddHttpClient(nameof(CachedKeycloakTokenProvider), client => {
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddStandardResilienceHandler();
        services.AddSingleton<CachedKeycloakTokenProvider>(); 
        services.AddTransient<KeycloakAuthHandler>();

        services.AddHttpClient<IExternalIdentityProvider, KeycloakIdentityProvider>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<KeycloakAuthHandler>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
            });

        return services;
    }
}