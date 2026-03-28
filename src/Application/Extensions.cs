using FixNet.Application.Base;
using FixNet.Application.Base.Abstractions;
using FixNet.Application.Features.Clients.CreateClient;
using Microsoft.Extensions.DependencyInjection;

namespace FixNet.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection service)
    {
        service.AddScoped<ICommandHandler<CreateClientCommand>, CreateClientHandler>();

        return service;
    }
}