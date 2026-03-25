using FixNet.API.Utilities;
using FixNet.Application.Users;
using FixNet.Application.Users.Abstractions;
using FixNet.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddFixNetCaching(builder.Configuration)
    .AddRateLimiter(options => options.AddFixNetPolicy())
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseRateLimiter();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/healthy", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", () => Results.Content("Hello from FixNet API - 1.0", "text/plain"));

app.MapGet("/test/register", async (
    IExternalIdentityProvider identityProvider,
    CancellationToken ct) =>
{
    var result = await identityProvider.CreateUserAsync(new CreateIdentityRequest("blachowicz@gmail.com", "Grey", "Jan", "Passwrod123@"),
        ct);

    await identityProvider.AssignRoleAsync(result, "User", ct);


    return Results.Ok(result);
});

app.Run();