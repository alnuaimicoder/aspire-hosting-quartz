using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Library.Quartz.Abstractions;
using Microsoft.Extensions.Logging;

namespace Aspire.Library.Quartz.Client;

internal sealed class BackgroundJobClient : IBackgroundJobClient
{
    private readonly IDbConnection _connection;
    private readonly ILogger<BackgroundJobClient> _logger;
    private readonly ActivitySource _activitySource;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly JobSerializer _serializer;

    public BackgroundJobClient(
        IDbConnection connection,
        ILogger<BackgroundJobClient> logger,
        ActivitySource activitySource,
        IIdempotencyStore idempotencyStore,
        JobSerializer serializer)
    {
        _connection = connection;
        _logger = logger;
        _activitySource = activitySource;
        _idempotencyStore = idempotencyStore;
        _serializer = serializer;
    }

    public async Task<string> EnqueueAsync<TJob>(
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        using var activity = _activitySource.StartActivity("job.enqueue");

        var jobId = Guid.NewGuid().ToString();
        var jobType = typeof(TJob).AssemblyQualifiedName
            ?? throw new InvalidOperationException($"Cannot get assembly qualified name for {typeof(TJob).Name}");

        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.type", typeof(TJob).Name);

        // Check idempotency
        if (options?.IdempotencyKey != null)
        {
            if (!await _idempotencyStore.TryAcquireAsync(options.IdempotencyKey, jobId, cancellationToken))
            {
                _logger.LogWarning("Duplicate job rejected: {IdempotencyKey}", options.IdempotencyKey);
                throw new DuplicateJobException(options.IdempotencyKey);
            }
        }

        // Serialize and store job
        var jobData = _serializer.Serialize(parameters, options);
        await StoreJobAsync(jobId, jobType, jobData, DateTimeOffset.UtcNow, cancellationToken);

        _logger.LogInformation("Job enqueued: {JobId} ({JobType})", jobId, typeof(TJob).Name);

        return jobId;
    }

    public async Task<string> ScheduleAsync<TJob>(
        TimeSpan delay,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        using var activity = _activitySource.StartActivity("job.schedule");

        var jobId = Guid.NewGuid().ToString();
        var jobType = typeof(TJob).AssemblyQualifiedName
            ?? throw new InvalidOperationException($"Cannot get assembly qualified name for {typeof(TJob).Name}");

        var scheduledTime = DateTimeOffset.UtcNow.Add(delay);

        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.type", typeof(TJob).Name);
        activity?.SetTag("job.scheduled_time", scheduledTime);

        // Check idempotency
        if (options?.IdempotencyKey != null)
        {
            if (!await _idempotencyStore.TryAcquireAsync(options.IdempotencyKey, jobId, cancellationToken))
            {
                _logger.LogWarning("Duplicate job rejected: {IdempotencyKey}", options.IdempotencyKey);
                throw new DuplicateJobException(options.IdempotencyKey);
            }
        }

        // Serialize and store job
        var jobData = _serializer.Serialize(parameters, options);
        await StoreJobAsync(jobId, jobType, jobData, scheduledTime, cancellationToken);

        _logger.LogInformation("Job scheduled: {JobId} ({JobType}) at {ScheduledTime}",
            jobId, typeof(TJob).Name, scheduledTime);

        return jobId;
    }

    public async Task<string> ScheduleAsync<TJob>(
        string cronExpression,
        object? parameters = null,
        JobOptions? options = null,
        CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        using var activity = _activitySource.StartActivity("job.schedule.cron");

        // Validate cron expression
        CronExpressionValidator.Validate(cronExpression);

        var jobId = Guid.NewGuid().ToString();
        var jobType = typeof(TJob).AssemblyQualifiedName
            ?? throw new InvalidOperationException($"Cannot get assembly qualified name for {typeof(TJob).Name}");

        activity?.SetTag("job.id", jobId);
        activity?.SetTag("job.type", typeof(TJob).Name);
        activity?.SetTag("job.cron_expression", cronExpression);

        // Check idempotency
        if (options?.IdempotencyKey != null)
        {
            if (!await _idempotencyStore.TryAcquireAsync(options.IdempotencyKey, jobId, cancellationToken))
            {
                _logger.LogWarning("Duplicate job rejected: {IdempotencyKey}", options.IdempotencyKey);
                throw new DuplicateJobException(options.IdempotencyKey);
            }
        }

        // Serialize and store job with cron trigger
        var jobData = _serializer.Serialize(parameters, options);
        await StoreCronJobAsync(jobId, jobType, jobData, cronExpression, cancellationToken);

        _logger.LogInformation("Job scheduled with cron: {JobId} ({JobType}) - {CronExpression}",
            jobId, typeof(TJob).Name, cronExpression);

        return jobId;
    }

    private async Task StoreJobAsync(
        string jobId,
        string jobType,
        byte[] jobData,
        DateTimeOffset scheduledTime,
        CancellationToken cancellationToken)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        using var transaction = _connection.BeginTransaction();

        try
        {
            // Insert job details
            const string jobSql = @"
                INSERT INTO QRTZ_JOB_DETAILS (SCHED_NAME, JOB_NAME, JOB_GROUP, JOB_CLASS_NAME, IS_DURABLE, IS_NONCONCURRENT, IS_UPDATE_DATA, REQUESTS_RECOVERY, JOB_DATA)
                VALUES (@SchedulerName, @JobId, @JobGroup, @JobType, 1, 0, 0, 0, @JobData)";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = jobSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@JobId", jobId);
                AddParameter(command, "@JobGroup", "DEFAULT");
                AddParameter(command, "@JobType", jobType);
                AddParameter(command, "@JobData", jobData);
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            // Insert simple trigger
            const string triggerSql = @"
                INSERT INTO QRTZ_TRIGGERS (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, NEXT_FIRE_TIME, PREV_FIRE_TIME, PRIORITY, TRIGGER_STATE, TRIGGER_TYPE, START_TIME, END_TIME, CALENDAR_NAME, MISFIRE_INSTR, JOB_DATA)
                VALUES (@SchedulerName, @TriggerId, @TriggerGroup, @JobId, @JobGroup, @NextFireTime, -1, 5, 'WAITING', 'SIMPLE', @StartTime, NULL, NULL, 0, NULL)";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = triggerSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@TriggerId", $"{jobId}_trigger");
                AddParameter(command, "@TriggerGroup", "DEFAULT");
                AddParameter(command, "@JobId", jobId);
                AddParameter(command, "@JobGroup", "DEFAULT");
                AddParameter(command, "@NextFireTime", scheduledTime.ToUnixTimeMilliseconds());
                AddParameter(command, "@StartTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            // Insert simple trigger details
            const string simpleTriggerSql = @"
                INSERT INTO QRTZ_SIMPLE_TRIGGERS (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, REPEAT_COUNT, REPEAT_INTERVAL, TIMES_TRIGGERED)
                VALUES (@SchedulerName, @TriggerId, @TriggerGroup, 0, 0, 0)";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = simpleTriggerSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@TriggerId", $"{jobId}_trigger");
                AddParameter(command, "@TriggerGroup", "DEFAULT");
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task StoreCronJobAsync(
        string jobId,
        string jobType,
        byte[] jobData,
        string cronExpression,
        CancellationToken cancellationToken)
    {
        if (_connection.State != ConnectionState.Open)
        {
            await Task.Run(() => _connection.Open(), cancellationToken);
        }

        using var transaction = _connection.BeginTransaction();

        try
        {
            // Insert job details
            const string jobSql = @"
                INSERT INTO QRTZ_JOB_DETAILS (SCHED_NAME, JOB_NAME, JOB_GROUP, JOB_CLASS_NAME, IS_DURABLE, IS_NONCONCURRENT, IS_UPDATE_DATA, REQUESTS_RECOVERY, JOB_DATA)
                VALUES (@SchedulerName, @JobId, @JobGroup, @JobType, 1, 0, 0, 0, @JobData)";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = jobSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@JobId", jobId);
                AddParameter(command, "@JobGroup", "DEFAULT");
                AddParameter(command, "@JobType", jobType);
                AddParameter(command, "@JobData", jobData);
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            // Insert cron trigger
            const string triggerSql = @"
                INSERT INTO QRTZ_TRIGGERS (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, NEXT_FIRE_TIME, PREV_FIRE_TIME, PRIORITY, TRIGGER_STATE, TRIGGER_TYPE, START_TIME, END_TIME, CALENDAR_NAME, MISFIRE_INSTR, JOB_DATA)
                VALUES (@SchedulerName, @TriggerId, @TriggerGroup, @JobId, @JobGroup, @NextFireTime, -1, 5, 'WAITING', 'CRON', @StartTime, NULL, NULL, 0, NULL)";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = triggerSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@TriggerId", $"{jobId}_trigger");
                AddParameter(command, "@TriggerGroup", "DEFAULT");
                AddParameter(command, "@JobId", jobId);
                AddParameter(command, "@JobGroup", "DEFAULT");
                AddParameter(command, "@NextFireTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                AddParameter(command, "@StartTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            // Insert cron trigger details
            const string cronTriggerSql = @"
                INSERT INTO QRTZ_CRON_TRIGGERS (SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, CRON_EXPRESSION, TIME_ZONE_ID)
                VALUES (@SchedulerName, @TriggerId, @TriggerGroup, @CronExpression, 'UTC')";

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = cronTriggerSql;
                AddParameter(command, "@SchedulerName", "AspireQuartzScheduler");
                AddParameter(command, "@TriggerId", $"{jobId}_trigger");
                AddParameter(command, "@TriggerGroup", "DEFAULT");
                AddParameter(command, "@CronExpression", cronExpression);
                await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
