using FixNet.API.Endpoints.Clients;

namespace FixNet.API.Endpoints;

public static class Extensions
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateUser();
    }
}