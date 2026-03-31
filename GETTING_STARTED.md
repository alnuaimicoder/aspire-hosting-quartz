# Getting Started with CommunityToolkit.Aspire.Quartz

This guide will help you get started with background job scheduling in your .NET Aspire application using CommunityToolkit.Aspire.Quartz.

## Prerequisites

Before you begin, ensure you have:

- **.NET SDK 8.0 or higher** - [Download](https://dotnet.microsoft.com/download)
- **.NET Aspire workload** - Install with: `dotnet workload install aspire`
- **Docker Desktop** - For running SQL Server/PostgreSQL containers
- **IDE** - Visual Studio 2022, VS Code, or JetBrains Rider

## Installation

### Option 1: Using .NET CLI

```bash
# In your AppHost project
cd YourApp.AppHost
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz

# In your API/Service projects
cd YourApp.ApiService
dotnet add package CommunityToolkit.Aspire.Quartz
```

### Option 2: Using Aspire CLI

```bash
# Add hosting integration
dotnet aspire add CommunityToolkit.Aspire.Hosting.Quartz

# Add client integration
dotnet aspire add CommunityToolkit.Aspire.Quartz
```

### Option 3: Using Package Manager Console (Visual Studio)

```powershell
Install-Package CommunityToolkit.Aspire.Hosting.Quartz
Install-Package CommunityToolkit.Aspire.Quartz
```

## Quick Start (5 Minutes)

### Step 1: Configure AppHost

Open your `Program.cs` in the AppHost project:

```csharp
using CommunityToolkit.Aspire.Hosting.Quartz;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server
var sqlserver = builder.AddSqlServer("sql")
    .AddDatabase("quartzdb");

// Add Quartz
var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

// Reference Quartz in your API
var apiService = builder.AddProject<Projects.YourApp_ApiService>("apiservice")
    .WithReference(quartz);

builder.Build().Run();
```

### Step 2: Create a Job

Create a new file `SendEmailJob.cs` in your API project:

```csharp
using CommunityToolkit.Aspire.Quartz;

namespace YourApp.ApiService;

public class SendEmailJob : IJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(JobContext context, CancellationToken cancellationToken)
    {
        var email = context.Parameters?["email"]?.ToString();
        var subject = context.Parameters?["subject"]?.ToString();

        _logger.LogInformation("Sending email to {Email}: {Subject}", email, subject);

        // Your email sending logic here
        await Task.Delay(1000, cancellationToken);

        _logger.LogInformation("Email sent successfully!");
    }
}
```

### Step 3: Enqueue Jobs

In your API's `Program.cs`:

```csharp
using CommunityToolkit.Aspire.Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add Quartz client
builder.Services.AddQuartzClient();

var app = builder.Build();

// Enqueue a job
app.MapPost("/send-email", async (IBackgroundJobClient jobClient) =>
{
    var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
        parameters: new { email = "user@example.com", subject = "Hello!" },
        options: new JobOptions
        {
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

### Step 4: Create a Worker

Create a new Worker Service project:

```bash
dotnet new worker -n YourApp.Worker
cd YourApp.Worker
dotnet add package Quartz
dotnet add package Quartz.Extensions.Hosting
```

Configure the worker in `Program.cs`:

```csharp
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(options =>
    {
        options.UseSqlServer(sqlServer =>
        {
            sqlServer.ConnectionString = builder.Configuration.GetConnectionString("quartzdb");
            sqlServer.TablePrefix = "QRTZ_";
        });
        options.UseNewtonsoftJsonSerializer();
    });
});

builder.Services.AddQuartzHostedService();
builder.Services.AddTransient<SendEmailJob>();

var host = builder.Build();
host.Run();
```

### Step 5: Run Your Application

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
