using FixNet.API.Utilities;
using FixNet.Application.Base.Abstractions;
using FixNet.Application.Features.Clients.CreateClient;

namespace FixNet.API.Endpoints.Clients;

public static class CreateClientEndpoint
{
    public static void MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/clients", async (CreateClientRequest request, ICommandHandler<CreateClientCommand> handler, HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateClientCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Password);
                var result = await handler.HandleAsync(command, cancellationToken);

                return result.IsFailure ? result.ToProblemResult(httpContext.Request.Path) : Results.Ok();
            })
            .WithTags("Clients")
            .AddEndpointFilter<IdempotencyFilter>();
    }
}