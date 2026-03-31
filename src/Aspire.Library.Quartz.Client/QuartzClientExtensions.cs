using System.Data;
using System.Diagnostics;
using Library.Quartz.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Aspire.Library.Quartz.Client;

/// <summary>
/// Extension methods for adding Quartz background job client to the service collection.
/// </summary>
public static class QuartzClientExtensions
{
    /// <summary>
    /// Adds Quartz background job client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionName">The name of the connection string (default: "quartz").</param>
    /// <param name="idempotencyExpiration">The expiration time for idempotency keys (default: 7 days).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQuartzClient(
        this IServiceCollection services,
        string connectionName = "quartz",
        TimeSpan? idempotencyExpiration = null)
    {
        var expiration = idempotencyExpiration ?? TimeSpan.FromDays(7);

        // Register ActivitySource for OpenTelemetry
        services.AddSingleton(new ActivitySource("Aspire.Quartz.Client"));

        // Register connection
        services.AddSingleton<IDbConnection>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(connectionName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{connectionName}' not found. " +
                    "Ensure the Quartz resource is referenced in the AppHost using WithReference().");

            // Detect provider from connection string
            if (connectionString.Contains("Server=") && connectionString.Contains("Database="))
            {
                return new SqlConnection(connectionString);
            }
            else if (connectionString.Contains("Host=") || connectionString.Contains("Server=") && connectionString.Contains("Port="))
            {
                return new NpgsqlConnection(connectionString);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unable to detect database provider from connection string '{connectionName}'. " +
                    "Supported providers: SQL Server, PostgreSQL.");
            }
        });

        // Register services
        services.AddSingleton<JobSerializer>();
        services.AddSingleton<IIdempotencyStore>(sp =>
        {
            var connection = sp.GetRequiredService<IDbConnection>();
            return new IdempotencyStore(connection, expiration);
        });
        services.AddSingleton<IBackgroundJobClient, BackgroundJobClient>();

        return services;
    }
}
