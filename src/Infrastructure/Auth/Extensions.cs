using FixNet.Application.Users.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace FixNet.Infrastructure.Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakSettings>(configuration.GetSection(KeycloakSettings.SectionName));

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<KeycloakTokenCache>();

        services.AddHttpClient<KeycloakTokenProvider>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<KeycloakSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddStandardResilienceHandler();
        services.AddTransient<KeycloakAuthHandler>();

        services.AddHttpClient<IExternalIdentityProvider, KeycloakIdentityProvider>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<KeycloakSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
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