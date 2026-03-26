using FixNet.API.Utilities;
using FixNet.Infrastructure;
using FixNet.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddFixNetCaching(builder.Configuration)
    .AddRateLimiter(options => options.AddFixNetPolicy())
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["Redis:ConnectionString"]
              ?? throw new InvalidOperationException("Redis connection string is missing"), name: "redis")
    .AddDbContextCheck<FixNetDbContext>(name: "database");

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

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

await app.Services.ApplyMigrationsAsync();

app.MapHealthChecks("/healthy", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", () => Results.Content("Hello from FixNet API - 1.0", "text/plain"));

app.Run();