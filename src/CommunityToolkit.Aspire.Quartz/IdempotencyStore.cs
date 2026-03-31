using System.Data;

namespace CommunityToolkit.Aspire.Quartz;

internal sealed class IdempotencyStore : IIdempotencyStore
{
    private readonly IDbConnection _connection;
    private readonly TimeSpan _expiration;

    public IdempotencyStore(IDbConnection connection, TimeSpan expiration)
    {
        _connection = connection;
        _expiration = expiration;
    }

    public async Task<bool> TryAcquireAsync(string key, string jobId, CancellationToken cancellationToken = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        const string checkSql = @"
            SELECT COUNT(*)
            FROM QRTZ_IDEMPOTENCY_KEYS
            WHERE IDEMPOTENCY_KEY = @Key AND EXPIRES_AT > @Now";

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = checkSql;
            AddParameter(command, "@Key", key);
            AddParameter(command, "@Now", DateTime.UtcNow);

            var count = await Task.Run(() => Convert.ToInt32(command.ExecuteScalar()), cancellationToken);

            if (count > 0)
            {
                return false; // Key already exists
            }
        }

        const string insertSql = @"
            INSERT INTO QRTZ_IDEMPOTENCY_KEYS (IDEMPOTENCY_KEY, JOB_ID, CREATED_AT, EXPIRES_AT)
            VALUES (@Key, @JobId, @CreatedAt, @ExpiresAt)";

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = insertSql;
            AddParameter(command, "@Key", key);
            AddParameter(command, "@JobId", jobId);
            AddParameter(command, "@CreatedAt", DateTime.UtcNow);
            AddParameter(command, "@ExpiresAt", DateTime.UtcNow.Add(_expiration));

            try
            {
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
                return true;
            }
            catch
            {
                // Concurrent insert - key already exists
                return false;
            }
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        const string sql = @"
            SELECT COUNT(*)
            FROM QRTZ_IDEMPOTENCY_KEYS
            WHERE IDEMPOTENCY_KEY = @Key AND EXPIRES_AT > @Now";

        using var command = _connection.CreateCommand();
        command.CommandText = sql;
        AddParameter(command, "@Key", key);
        AddParameter(command, "@Now", DateTime.UtcNow);

        var count = await Task.Run(() => Convert.ToInt32(command.ExecuteScalar()), cancellationToken);
        return count > 0;
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}

internal interface IIdempotencyStore
{
    Task<bool> TryAcquireAsync(string key, string jobId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
