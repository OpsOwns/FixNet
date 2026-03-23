using FixNet.API.Utilities;
using FixNet.Infrastructure.Auth;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddFixNetCaching(builder.Configuration)
    .AddRateLimiter(options => options.AddFixNetPolicy())
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

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

app.Run();