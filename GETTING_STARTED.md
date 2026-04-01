# Getting Started with AspireQuartz

This guide will help you get started with background job scheduling in your .NET Aspire application using AspireQuartz.

## Prerequisites

Before you begin, ensure you have:

- **.NET SDK 8.0 or higher** - [Download](https://dotnet.microsoft.com/download)
- **.NET Aspire workload** - Install with: `dotnet workload install aspire`
- **Docker Desktop** - For running SQL Server/PostgreSQL containers
- **IDE** - Visual Studio 2022, VS Code, or JetBrains Rider

## Installation

### Option 1: Using .NET CLI (Recommended)

```bash
# In your AppHost project
cd YourApp.AppHost
dotnet add package AspireQuartz.Hosting

# In your API/Service projects
cd YourApp.ApiService
dotnet add package AspireQuartz
```

### Option 2: Using Aspire CLI

> **Note**: The `aspire add` command currently only works with packages published in the official Aspire Community Toolkit. Since AspireQuartz is not yet part of the toolkit, this option is not available yet.
>
> **Why?** The Aspire CLI only recognizes packages that are registered in the [Aspire Community Toolkit](https://github.com/CommunityToolkit/Aspire). We've submitted a proposal to add AspireQuartz to the toolkit, and once approved, you'll be able to use `aspire add AspireQuartz`.
>
> **For now**, please use Option 1 (`dotnet add package`) to install AspireQuartz.

```bash
# This will work once AspireQuartz is added to Aspire Community Toolkit
aspire add AspireQuartz.Hosting  # Not available yet
aspire add AspireQuartz           # Not available yet
```

### Option 3: Using Package Manager Console (Visual Studio)

```powershell
Install-Package AspireQuartz.Hosting
Install-Package AspireQuartz
```

## Quick Start (5 Minutes)

### Step 1: Configure AppHost

Open your `Program.cs` in the AppHost project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("quartzdb");

// Add your API service (jobs run here - no separate worker needed!)
builder.AddProject<Projects.YourApp_ApiService>("apiservice")
    .WithReference(postgres);

builder.Build().Run();
```

### Step 2: Create a Job

Create a new file `SendEmailJob.cs` in your API project:

```csharp
using Quartz;

namespace YourApp.ApiService;

public class SendEmailJob : IJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var email = context.JobDetail.JobDataMap.GetString("email");
        var subject = context.JobDetail.JobDataMap.GetString("subject");

        _logger.LogInformation("Sending email to {Email}: {Subject}", email, subject);

        // Your email sending logic here
        await Task.Delay(1000);

        _logger.LogInformation("Email sent successfully!");
    }
}
```

### Step 3: Configure API Service

In your API's `Program.cs`:

```csharp
using Aspire.Quartz;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add Quartz.NET with full scheduling power
builder.Services.AddQuartz(q =>
{
    // Configure PostgreSQL persistence
    q.UsePersistentStore(store =>
    {
        store.UsePostgres(builder.Configuration.GetConnectionString("quartzdb")!);
        store.UseNewtonsoftJsonSerializer();
    });
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Add AspireQuartz client for dynamic scheduling (MUST be called after AddQuartz)
builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb"));

var app = builder.Build();

// Enqueue a job
app.MapPost("/send-email", async (IBackgroundJobClient jobClient) =>
{
    var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
        new { email = "user@example.com", subject = "Hello!" },
        new JobOptions
        {
            IdempotencyKey = "email-123",
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                Strategy = BackoffStrategy.Exponential
            }
        });

    return Results.Ok(new { jobId });
});

app.Run();
```

```bash
dotnet run --project YourApp.AppHost
```

Open the Aspire Dashboard (usually at `http://localhost:15888`) and watch your jobs execute!

## Common Scenarios

### Scheduling with Delay

```csharp
var jobId = await jobClient.ScheduleAsync<SendEmailJob>(
    delay: TimeSpan.FromMinutes(5),
    parameters: new { email = "user@example.com" });
```

### Scheduling with Cron

```csharp
var jobId = await jobClient.ScheduleAsync<SendEmailJob>(
    cronExpression: "0 0 9 * * ?",  // Every day at 9 AM
    parameters: new { email = "user@example.com" });
```

### Idempotency

```csharp
var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
    parameters: new { email = "user@example.com" },
    options: new JobOptions
    {
        IdempotencyKey = $"daily-email-{DateTime.UtcNow:yyyyMMdd}"
    });
```

### Custom Retry Policy

```csharp
var retryPolicy = new RetryPolicyBuilder()
    .WithMaxAttempts(5)
    .WithExponentialBackoff(
        initialDelay: TimeSpan.FromSeconds(1),
        multiplier: 2.0,
        maxDelay: TimeSpan.FromMinutes(5))
    .Build();

var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
    parameters: new { email = "user@example.com" },
    options: new JobOptions { RetryPolicy = retryPolicy });
```

## Configuration Options

### AppHost Configuration

```csharp
var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver)
    .WithMaxConcurrency(20)
    .WithIdempotencyExpiration(TimeSpan.FromDays(7))
    .WithoutMigration();  // If you manage schema manually
```

### PostgreSQL Support

```csharp
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithDatabase(postgres);
```

## Observability

### View Traces

1. Open Aspire Dashboard
2. Navigate to "Traces"
3. Filter by "Aspire.Quartz.Client" or job name
4. View distributed traces across services

### View Metrics

1. Open Aspire Dashboard
2. Navigate to "Metrics"
3. View:
   - Job execution count
   - Job duration
   - Job failures
   - Queue depth

### View Logs

1. Open Aspire Dashboard
2. Navigate to "Logs"
3. Filter by service name
4. View structured logs with job context

## Troubleshooting

### Jobs Not Executing

**Problem**: Jobs are enqueued but never execute.

**Solution**:
1. Ensure Worker service is running
2. Check database connection string
3. Verify Quartz tables exist in database
4. Check Worker logs for errors

### Database Migration Fails

**Problem**: Migration service fails to create tables.

**Solution**:
1. Check database permissions
2. Verify connection string
3. Check if tables already exist
4. Review migration logs

### Duplicate Jobs

**Problem**: Same job executes multiple times.

**Solution**:
1. Use idempotency keys
2. Check for multiple worker instances
3. Verify job configuration

## Next Steps

- [View the complete sample](./samples/README.md)
- [Read the API documentation](./README.md)
- [Check the versioning policy](./VERSIONING.md)
- [Report issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- [Join discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Quartz.NET Documentation](https://www.quartz-scheduler.net/)
- [Cron Expression Guide](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)

## Getting Help

- **Questions**: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- **Bugs**: [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- **Security**: See [SECURITY.md](./SECURITY.md)

---

Happy coding! 🚀
