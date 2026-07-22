using FixNet.API.Extensions;
using FixNet.API.Filters;
using FixNet.Application.Common.Abstractions;
using FixNet.Application.Features.Users;
using FixNet.Application.Features.Users.CreateUser;

namespace FixNet.API.Endpoints.Clients;

public static class CreateClientEndpoint
{
    public static void MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/clients", async (CreateClientRequest request, ICommandHandler<CreateUserCommand> handler, HttpContext httpContext,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateUserCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Password, UserRole.Client);
                var result = await handler.HandleAsync(command, cancellationToken);

                return result.IsFailure ? result.ToProblemResult(httpContext.Request.Path) : Results.Ok();
            })
            .WithTags("Clients")
            .AddEndpointFilter<IdempotencyFilter>();
    }
}