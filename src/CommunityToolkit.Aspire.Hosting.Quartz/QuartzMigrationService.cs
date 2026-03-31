using System.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommunityToolkit.Aspire.Hosting.Quartz;

/// <summary>
/// Background service that runs database migrations for Quartz.NET tables on startup.
/// </summary>
internal sealed class QuartzMigrationService : IHostedService
{
    private readonly IDbConnection _connection;
    private readonly ILogger<QuartzMigrationService> _logger;
    private readonly DatabaseProvider _provider;
    private readonly bool _enableMigration;

    public QuartzMigrationService(
        IDbConnection connection,
        ILogger<QuartzMigrationService> logger,
        DatabaseProvider provider,
        bool enableMigration)
    {
        _connection = connection;
        _logger = logger;
        _provider = provider;
        _enableMigration = enableMigration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_enableMigration)
        {
            _logger.LogInformation("Database migration is disabled");
            return;
        }

        try
        {
            _logger.LogInformation("Checking if Quartz tables exist...");

            if (await TablesExistAsync(cancellationToken))
            {
                _logger.LogInformation("Quartz tables already exist, skipping migration");
                return;
            }

            _logger.LogInformation("Running Quartz database migration for {Provider}...", _provider);
            await ExecuteScriptAsync(cancellationToken);
            _logger.LogInformation("Quartz database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run Quartz database migration");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task<bool> TablesExistAsync(CancellationToken cancellationToken)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        const string sql = @"
            SELECT COUNT(*)
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_NAME = 'QRTZ_JOB_DETAILS'";

        using var command = _connection.CreateCommand();
        command.CommandText = sql;

        var count = await Task.Run(() => Convert.ToInt32(command.ExecuteScalar()), cancellationToken);
        return count > 0;
    }

    private async Task ExecuteScriptAsync(CancellationToken cancellationToken)
    {
        var script = _provider == DatabaseProvider.SqlServer
            ? SqlServerMigrationScript.Script
            : PostgreSqlMigrationScript.Script;

        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        // Split script by GO statements for SQL Server or semicolons for PostgreSQL
        var separator = _provider == DatabaseProvider.SqlServer ? "GO" : ";";
        var statements = script.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var statement in statements)
        {
            var trimmed = statement.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            using var command = _connection.CreateCommand();
            command.CommandText = trimmed;
            await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
        }
    }
}
