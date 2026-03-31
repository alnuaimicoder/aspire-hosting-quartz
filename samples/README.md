# Quartz Sample - Complete Background Job Scheduling Demo

This sample demonstrates a complete background job scheduling system using CommunityToolkit.Aspire.Quartz with .NET Aspire.

## Architecture

```
┌─────────────────┐
│   Web Frontend  │  (Blazor UI)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   API Service   │  (Enqueues jobs)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   SQL Server    │  (Persistent storage)
│   + Quartz DB   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Worker Service │  (Processes jobs)
└─────────────────┘
```

## Projects

### 1. QuartzSample.AppHost
The Aspire orchestration project that configures all resources and services.

**Key Features:**
- SQL Server with persistent lifetime
- Quartz resource with automatic database migration
- Service references and dependencies

### 2. QuartzSample.ApiService
REST API that enqueues background jobs.

**Endpoints:**
- `POST /send-email` - Enqueue an email job for immediate execution
- `POST /schedule-email` - Schedule an email job with delay

**Features:**
- Uses `IBackgroundJobClient` to enqueue jobs
- Idempotency keys to prevent duplicates
- Retry policies with exponential backoff
- OpenTelemetry tracing

### 3. QuartzSample.Worker
Background worker service that processes jobs from the queue.

**Features:**
- Quartz.NET scheduler with SQL Server persistence
- Automatic job execution
- Concurrent job processing (max 10)
- Logging and observability

### 4. QuartzSample.Web
Blazor web frontend for testing the system.

### 5. QuartzSample.ServiceDefaults
Shared service defaults for all projects (health checks, OpenTelemetry, etc.).

## Running the Sample

### Prerequisites

- .NET 8.0 SDK or higher
- Docker Desktop (for SQL Server container)
- Aspire workload: `dotnet workload install aspire`

### Steps

1. **Navigate to the samples directory:**
   ```bash
   cd samples
   ```

2. **Run the AppHost:**
   ```bash
   dotnet run --project QuartzSample.AppHost
   ```

3. **Access the Aspire Dashboard:**
   - Open the URL shown in the console (typically `http://localhost:15888`)
   - View all services, logs, traces, and metrics

4. **Test the API:**

   **Enqueue an immediate job:**
   ```bash
   curl -X POST http://localhost:5000/send-email \
     -H "Content-Type: application/json" \
     -d '{"email":"test@example.com","subject":"Hello World"}'
   ```

   **Schedule a delayed job:**
   ```bash
   curl -X POST http://localhost:5000/schedule-email \
     -H "Content-Type: application/json" \
     -d '{"email":"test@example.com","subject":"Delayed Email","delayMinutes":2}'
   ```

5. **Monitor job execution:**
   - Check the Worker logs in the Aspire Dashboard
   - View distributed traces showing job flow
   - Monitor metrics (job count, duration, failures)

## Code Walkthrough

### Enqueuing a Job (ApiService)

```csharp
app.MapPost("/send-email", async (IBackgroundJobClient jobClient, EmailRequest request) =>
{
    var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
        parameters: new { email = request.Email, subject = request.Subject },
        options: new JobOptions
        {
            IdempotencyKey = $"email-{request.Email}-{DateTime.UtcNow:yyyyMMdd}",
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                Strategy = BackoffStrategy.Exponential
            }
        });

    return Results.Ok(new { jobId, message = "Email job enqueued successfully" });
});
```

### Processing a Job (Worker)

```csharp
public class SendEmailJob : IJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var email = dataMap.GetString("email") ?? "unknown@example.com";
        var subject = dataMap.GetString("subject") ?? "No Subject";

        _logger.LogInformation("Sending email to {Email} with subject '{Subject}'",
            email, subject);

        // Simulate email sending
        await Task.Delay(1000, context.CancellationToken);

        _logger.LogInformation("Email sent successfully to {Email}", email);
    }
}
```

### Configuring Quartz (AppHost)

```csharp
var sqlserver = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

var worker = builder.AddProject<Projects.QuartzSample_Worker>("worker")
    .WithReference(sqlserver);

var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice")
    .WithReference(quartz);
```

## Features Demonstrated

### 1. Job Enqueuing
- Immediate execution
- Delayed execution
- Cron-based scheduling (can be added)

### 2. Idempotency
- Prevents duplicate job execution
- Uses unique keys per job
- Configurable expiration

### 3. Retry Policies
- Exponential backoff
- Linear backoff
- Configurable max attempts

### 4. Observability
- Distributed tracing with OpenTelemetry
- Metrics (job count, duration, failures)
- Structured logging
- Aspire Dashboard integration

### 5. Persistence
- SQL Server job storage
- Automatic database migration
- Survives application restarts

## Troubleshooting

### SQL Server container not starting
```bash
# Check Docker is running
docker ps

# View container logs
docker logs <container-id>
```

### Jobs not executing
1. Check Worker service is running in Aspire Dashboard
2. Verify SQL Server connection string
3. Check Worker logs for errors
4. Ensure Quartz tables were created (check database)

### Database migration issues
- The migration runs automatically on first start
- Check Worker logs for migration errors
- Manually verify tables exist: `SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'QRTZ_%'`

## Next Steps

1. **Add more job types**: Create additional IJob implementations
2. **Add cron scheduling**: Use `ScheduleAsync` with cron expressions
3. **Add job monitoring UI**: Build a dashboard to view job status
4. **Add job cancellation**: Implement job cancellation support
5. **Add job priorities**: Use job priorities for execution order
6. **Add PostgreSQL support**: Switch to PostgreSQL instead of SQL Server

## Learn More

- [CommunityToolkit.Aspire.Quartz Documentation](../../README.md)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Quartz.NET Documentation](https://www.quartz-scheduler.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
