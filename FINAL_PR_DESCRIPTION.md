# Pull Request Description (Copy This)

**Closes #1259**

## Overview

This PR adds a production-ready integration for background job scheduling using Quartz.NET in .NET Aspire applications.

.NET Aspire currently lacks native support for background job scheduling. Developers must manually integrate Quartz.NET or Hangfire, configure persistence, set up observability, and implement idempotency themselves. This integration solves that problem by providing an Aspire-native solution.

## What's Included

### Three Packages

1. **CommunityToolkit.Aspire.Hosting.Quartz** (Hosting Integration)
   - Resource pattern for Quartz.NET
   - Automatic database configuration (PostgreSQL, SQL Server, MySQL, SQLite)
   - Health checks integration
   - OpenTelemetry metrics
   - Automatic schema migrations using EF Core

2. **CommunityToolkit.Aspire.Quartz** (Client Integration)
   - `IBackgroundJobClient` for dynamic job scheduling
   - Idempotency support (prevent duplicate execution)
   - Retry policies with exponential/linear backoff
   - OpenTelemetry distributed tracing
   - Full Quartz.NET power (cron expressions, triggers, listeners)

3. **CommunityToolkit.Aspire.Quartz.Abstractions** (Core Abstractions)
   - Core interfaces: `IJob`, `IBackgroundJobClient`
   - Shared types: `JobOptions`, `RetryPolicy`, `JobContext`

## Key Features

- ✅ **Aspire-native patterns** - Built for Aspire from the ground up
- ✅ **Production features** - Idempotency, OpenTelemetry, health checks out of the box
- ✅ **Multi-database support** - PostgreSQL, SQL Server, MySQL, SQLite with auto-migrations
- ✅ **Full Quartz.NET power** - No feature hiding, complete access to Quartz.NET APIs
- ✅ **All-in-one architecture** - No separate worker needed, jobs run in your API service
- ✅ **Type-safe API** - Strong typing with generics for job parameters

## Usage Example

### AppHost
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .AddDatabase("quartzdb");

builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres);

builder.Build().Run();
```

### API Service
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(store =>
        store.UsePostgres(builder.Configuration.GetConnectionString("quartzdb")!));
});

builder.Services.AddQuartzHostedService();
builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb"));

var app = builder.Build();

app.MapPost("/jobs/enqueue", async (IBackgroundJobClient jobClient) =>
{
    var jobId = await jobClient.EnqueueAsync<SendEmailJob>(
        new { email = "user@example.com" },
        new JobOptions { IdempotencyKey = "email-123" });

    return Results.Ok(new { jobId });
});

app.Run();
```

### Job Definition
```csharp
public class SendEmailJob : IJob
{
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(ILogger<SendEmailJob> logger) => _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        var email = context.JobDetail.JobDataMap.GetString("email");
        _logger.LogInformation("Sending email to {Email}", email);
        await Task.Delay(1000);
        _logger.LogInformation("Email sent successfully!");
    }
}
```

## Proof of Concept

This integration has been published and tested:
- **NuGet**: https://www.nuget.org/packages/AspireQuartz (v1.0.1)
- **GitHub**: https://github.com/alnuaimicoder/aspire-hosting-quartz
- **Status**: Production-ready, published, and actively maintained

## PR Checklist

- [x] Created a feature/dev branch in your fork (vs. submitting directly from a commit on main)
- [x] Based off latest main branch of toolkit
- [x] PR doesn't include merge commits (always rebase on top of our main, if needed)
- [x] New integration
- [x] Docs are written (README.md in each package)
- [x] Added description of major feature to project description for NuGet package
- [ ] Tests for the changes have been added (IN PROGRESS - will add in follow-up commits)
- [x] Contains **NO** breaking changes
- [x] Every new API (including internal ones) has full XML docs
- [x] Code follows all style conventions

## Current Status & Next Steps

This is an initial PR with the core integration code. I'm actively working on:

### In Progress
- [ ] Unit tests for all public APIs
- [ ] Integration tests with `[RequiresDocker]` attribute
- [ ] Example application in `examples/Quartz/`
- [ ] PublicAPI.txt files for API tracking
- [ ] Update main README.md (need maintainer guidance on format)
- [ ] Update CI workflow (need maintainer guidance)

### Completed
- [x] Core integration code
- [x] XML documentation on all public APIs
- [x] Package metadata with `hosting` and `client` tags
- [x] Namespaces follow guidelines (`Aspire.Hosting`, `Aspire.Hosting.ApplicationModel`)
- [x] README.md files in each package
- [x] Follows CommunityToolkit conventions

## Testing Plan

Will add comprehensive tests covering:

### Unit Tests
- Resource creation and configuration
- Job client API (enqueue, schedule, cancel)
- Idempotency store behavior
- Retry policy logic
- Job serialization

### Integration Tests
- End-to-end job execution with PostgreSQL
- Database migration verification
- Health check validation
- OpenTelemetry trace verification
- Multi-database support (PostgreSQL, SQL Server)

All integration tests will be marked with `[RequiresDocker]` as they require database containers.

## Other Information

### Why This Integration Matters

Background job scheduling is a fundamental requirement for most production applications. Currently, .NET Aspire developers must:
1. Manually integrate Quartz.NET or Hangfire
2. Configure database persistence themselves
3. Set up observability from scratch
4. Implement idempotency and retry logic
5. Handle health checks separately

This integration makes background jobs a first-class citizen in Aspire, just like databases, caches, and messaging.

### Design Decisions

1. **Three-package structure**: Separates abstractions, client, and hosting for clean dependency management
2. **Aspire.Hosting namespace**: Extension methods in `Aspire.Hosting` for discoverability
3. **No wrapper abstraction**: Full access to Quartz.NET APIs - we enhance, not hide
4. **All-in-one architecture**: Jobs run in API service, no separate worker needed (simpler deployment)
5. **Multi-database support**: Works with PostgreSQL, SQL Server, MySQL, SQLite

### Maintenance Commitment

I'm committed to:
- Long-term maintenance of this integration
- Responding promptly to issues and feedback
- Keeping up with Aspire and Quartz.NET updates
- Adding features based on community needs

### Questions for Maintainers

1. **Tests**: Should I add tests in this PR or create a follow-up PR? (I can add them quickly)
2. **Example**: Should the example be minimal or comprehensive? (I have a full sample ready)
3. **Documentation**: Any specific format for the main README.md update?
4. **CI**: How should I update the tests.yml workflow?

I'm happy to make any changes based on your feedback!

---

**Branch**: `feature/add-quartz-integration`
**Original Repository**: https://github.com/alnuaimicoder/aspire-hosting-quartz
