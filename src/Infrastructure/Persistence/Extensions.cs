using System.Reflection;
using FixNet.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FixNet.Infrastructure.Persistence;

public static class Extensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DbConnection")
                               ?? throw new InvalidOperationException("Connection string 'DbConnection' not found.");

        services.AddDbContext<FixNetDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.LogTo(
                Console.WriteLine,
                [
                    DbLoggerCategory.Database.Command.Name,
                    DbLoggerCategory.Database.Transaction.Name
                ], LogLevel.Information);

            if (!environment.IsDevelopment())
                return;

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        services.Scan(s => s.FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(c => c.AssignableTo<IRepository>(), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());


        return services;
    }
}