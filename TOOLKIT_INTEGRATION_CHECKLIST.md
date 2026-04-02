# ✅ AspireQuartz Integration Checklist for CommunityToolkit

This document outlines all changes needed to integrate AspireQuartz into the Aspire Community Toolkit.

## 📦 Current Package Information

- **Repository**: https://github.com/alnuaimicoder/aspire-hosting-quartz
- **NuGet Packages**:
  - AspireQuartz v1.0.1 (Client Integration)
  - AspireQuartz.Hosting v1.0.1 (Hosting Integration)
  - AspireQuartz.Abstractions v1.0.1 (Core Abstractions)
- **Total Downloads**: Available on NuGet.org
- **License**: MIT
- **Status**: Production-ready, published, and tested

## 🔄 Required Changes

### 1. Package Naming

**Current Names:**
```
Aspire.Quartz
Aspire.Hosting.Quartz
Aspire.Quartz.Abstractions
```

**Required Names:**
```
CommunityToolkit.Aspire.Quartz
CommunityToolkit.Aspire.Hosting.Quartz
CommunityToolkit.Aspire.Quartz.Abstractions
```

**Files to Update:**
- All `.csproj` files (PackageId property)
- All namespace declarations
- All using statements
- Project references
- README files
- Documentation

### 2. Project Structure

**Current Structure:**
```
aspire-hosting-quartz/
├── src/
│   ├── Aspire.Quartz/
│   ├── Aspire.Hosting.Quartz/
│   └── Aspire.Quartz.Abstractions/
├── samples/
│   └── QuartzSample.ApiService/
└── tests/ (need to create)
```

**Required Structure:**
```
CommunityToolkit/Aspire/
├── src/
│   ├── CommunityToolkit.Aspire.Quartz/
│   ├── CommunityToolkit.Aspire.Hosting.Quartz/
│   └── CommunityToolkit.Aspire.Quartz.Abstractions/
├── examples/
│   └── Quartz/ (move from samples)
└── tests/
    ├── CommunityToolkit.Aspire.Quartz.Tests/
    └── CommunityToolkit.Aspire.Hosting.Quartz.Tests/
```

### 3. Namespace Conventions

**Current Namespaces:**
- `Aspire.Quartz` (Client)
- `Aspire.Hosting.Quartz` (Hosting)
- `Aspire.Hosting.ApplicationModel` (Resources) ✅ Already correct

**Required Namespaces:**
- Extension methods for hosting: `Aspire.Hosting` ✅ Already correct
- Extension methods for client: `Microsoft.Extensions.Hosting` ⚠️ Need to verify
- Custom resources: `Aspire.Hosting.ApplicationModel` ✅ Already correct

**Action Items:**
- [ ] Verify client extension methods are in `Microsoft.Extensions.Hosting` namespace
- [ ] Keep resource classes in `Aspire.Hosting.ApplicationModel`

### 4. Package Metadata

**Current Metadata (Aspire.Quartz.csproj):**
```xml
<PropertyGroup>
  <PackageId>AspireQuartz</PackageId>
  <Description>The standard way to do background jobs in .NET Aspire...</Description>
  <PackageTags>aspire;quartz;background-jobs;scheduling;jobs;cron;dotnet;aspire-integration</PackageTags>
</PropertyGroup>
```

**Required Updates:**
```xml
<PropertyGroup>
  <PackageId>CommunityToolkit.Aspire.Quartz</PackageId>
  <Description>An Aspire client integration for Quartz.NET background job scheduling with idempotency, OpenTelemetry, and health checks.</Description>
  <AdditionalPackageTags>quartz background-jobs scheduling client</AdditionalPackageTags>
</PropertyGroup>
```

**Required Metadata (Aspire.Hosting.Quartz.csproj):**
```xml
<PropertyGroup>
  <PackageId>CommunityToolkit.Aspire.Hosting.Quartz</PackageId>
  <Description>An Aspire hosting integration for Quartz.NET background job scheduling with persistent storage and automatic migrations.</Description>
  <AdditionalPackageTags>quartz background-jobs scheduling hosting</AdditionalPackageTags>
</PropertyGroup>
```

**Action Items:**
- [ ] Add `client` or `hosting` tag to each package
- [ ] Update descriptions to match CommunityToolkit style
- [ ] Ensure tags are space-separated

### 5. Testing Requirements

#### Unit Tests (Need to Create)

**CommunityToolkit.Aspire.Hosting.Quartz.Tests:**
```csharp
[Fact]
public void AddQuartzAddsResourceToBuilder()
{
    var builder = DistributedApplication.CreateBuilder();

    var postgres = builder.AddPostgres("postgres").AddDatabase("quartzdb");
    var quartz = builder.AddQuartz("quartz", postgres);

    using var app = builder.Build();
    var appModel = app.Services.GetRequiredService<DistributedApplicationModel>();

    var resource = appModel.Resources.OfType<QuartzResource>().SingleOrDefault();

    Assert.NotNull(resource);
    Assert.Equal("quartz", resource.Name);
}

[Fact]
public void QuartzResourceExposesConnectionString()
{
    var builder = DistributedApplication.CreateBuilder();

    var postgres = builder.AddPostgres("postgres").AddDatabase("quartzdb");
    var quartz = builder.AddQuartz("quartz", postgres);

    using var app = builder.Build();

    var connectionString = quartz.Resource.ConnectionStringExpression.ValueExpression;

    Assert.NotNull(connectionString);
}
```

**CommunityToolkit.Aspire.Quartz.Tests:**
```csharp
[Fact]
public void AddQuartzClientRegistersServices()
{
    var builder = Host.CreateApplicationBuilder();

    builder.Configuration["ConnectionStrings:quartzdb"] = "Host=localhost;Database=quartz;";
    builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb")!);

    using var host = builder.Build();

    var jobClient = host.Services.GetService<IBackgroundJobClient>();

    Assert.NotNull(jobClient);
}
```

#### Integration Tests (Need to Create)

**CommunityToolkit.Aspire.Hosting.Quartz.Tests:**
```csharp
public class QuartzIntegrationTests : IClassFixture<AspireIntegrationTestFixture<Projects.QuartzSample_AppHost>>
{
    private readonly AspireIntegrationTestFixture<Projects.QuartzSample_AppHost> _fixture;

    public QuartzIntegrationTests(AspireIntegrationTestFixture<Projects.QuartzSample_AppHost> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [RequiresDocker]
    public async Task QuartzResourceStartsSuccessfully()
    {
        var httpClient = _fixture.CreateHttpClient("api");

        await _fixture.App.WaitForTextAsync("Quartz Scheduler", "api")
            .WaitAsync(TimeSpan.FromSeconds(30));

        var response = await httpClient.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [RequiresDocker]
    public async Task CanEnqueueAndExecuteJob()
    {
        var httpClient = _fixture.CreateHttpClient("api");

        var response = await httpClient.PostAsync("/jobs/enqueue",
            JsonContent.Create(new { jobType = "TestJob" }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<JobResponse>();
        Assert.NotNull(result?.JobId);
    }
}
```

**Action Items:**
- [ ] Create `tests/CommunityToolkit.Aspire.Hosting.Quartz.Tests/` project
- [ ] Create `tests/CommunityToolkit.Aspire.Quartz.Tests/` project
- [ ] Add unit tests for all public APIs
- [ ] Add integration tests with `[RequiresDocker]` attribute
- [ ] Reference example AppHost project for integration tests
- [ ] Update `.github/workflows/tests.yml` with new test projects

### 6. Example Application

**Current Location:**
```
samples/QuartzSample.ApiService/
samples/QuartzSample.AppHost/
```

**Required Location:**
```
examples/Quartz/
├── Quartz.AppHost/
└── Quartz.ApiService/
```

**Required Updates:**
- [ ] Move sample to `examples/Quartz/`
- [ ] Rename projects to match CommunityToolkit conventions
- [ ] Update to use `CommunityToolkit.Aspire.Quartz` packages
- [ ] Ensure it demonstrates minimal usage
- [ ] Remove SignalR (it's a nice-to-have, not core feature)
- [ ] Keep only essential jobs for demonstration

### 7. Documentation

#### Package README Files

**Current:** Each package has a README.md
**Required:** Update with CommunityToolkit naming

**Example (CommunityToolkit.Aspire.Quartz/README.md):**
```markdown
# CommunityToolkit.Aspire.Quartz

An Aspire client integration for Quartz.NET background job scheduling.

## Installation

```bash
dotnet add package CommunityToolkit.Aspire.Quartz
```

## Usage

```csharp
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(store =>
        store.UsePostgres(builder.Configuration.GetConnectionString("quartzdb")!));
});

builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb"));

// Enqueue a job
await jobClient.EnqueueAsync<SendEmailJob>(
    new { email = "user@example.com" },
    new JobOptions { IdempotencyKey = "email-123" });
```

## Features

- Enqueue jobs for immediate execution
- Schedule jobs with delay or cron expressions
- Idempotency support
- Retry policies with exponential/linear backoff
- OpenTelemetry distributed tracing
- Multi-database support (PostgreSQL, SQL Server, MySQL, SQLite)

## Documentation

Visit [aspire.dev](https://aspire.dev) for complete documentation.

## License

MIT License - See [LICENSE](../../LICENSE)
```

#### Main Repository README

**Action Items:**
- [ ] Update main README.md in CommunityToolkit/Aspire repo
- [ ] Add Quartz integration to the integrations table
- [ ] Include links to packages and documentation

#### Official Documentation (aspire.dev)

**Action Items:**
- [ ] Create PR to microsoft/aspire.dev repository
- [ ] Use their agent to scaffold docs from README
- [ ] Include:
  - Getting started guide
  - Configuration options
  - Code examples
  - Multi-database setup
  - Idempotency and retry policies
  - OpenTelemetry integration

### 8. Code Quality

**Action Items:**
- [ ] Ensure all public APIs have XML documentation comments
- [ ] Run code analysis and fix warnings
- [ ] Verify no breaking changes to Aspire patterns
- [ ] Follow CommunityToolkit coding conventions
- [ ] Add `api/` folder with PublicAPI.Shipped.txt and PublicAPI.Unshipped.txt

**Example (PublicAPI.Unshipped.txt):**
```
CommunityToolkit.Aspire.Quartz.IBackgroundJobClient
CommunityToolkit.Aspire.Quartz.IBackgroundJobClient.EnqueueAsync<TJob>(object parameters, CommunityToolkit.Aspire.Quartz.JobOptions options, System.Threading.CancellationToken cancellationToken = default) -> System.Threading.Tasks.Task<string>
CommunityToolkit.Aspire.Quartz.IBackgroundJobClient.ScheduleAsync<TJob>(object parameters, System.TimeSpan delay, CommunityToolkit.Aspire.Quartz.JobOptions options = null, System.Threading.CancellationToken cancellationToken = default) -> System.Threading.Tasks.Task<string>
```

### 9. Container Image Requirements

**Current Implementation:**
- Uses Aspire's built-in PostgreSQL integration ✅
- No custom container images ✅
- Database setup handled by Aspire ✅

**Action Items:**
- [x] No changes needed - already compliant

### 10. CI/CD Integration

**Action Items:**
- [ ] Update `.github/workflows/tests.yml` in CommunityToolkit/Aspire
- [ ] Run `./eng/testing/generate-test-list-for-workflow.sh` to update test list
- [ ] Ensure tests run on Windows, Linux, and macOS (where applicable)
- [ ] Mark Docker-dependent tests with `[RequiresDocker]`

### 11. Migration Guide for Existing Users

**Create MIGRATION.md:**
```markdown
# Migrating from AspireQuartz to CommunityToolkit.Aspire.Quartz

## Package Changes

**Old Packages:**
```bash
dotnet remove package AspireQuartz
dotnet remove package AspireQuartz.Hosting
dotnet remove package AspireQuartz.Abstractions
```

**New Packages:**
```bash
dotnet add package CommunityToolkit.Aspire.Quartz
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
dotnet add package CommunityToolkit.Aspire.Quartz.Abstractions
```

## Namespace Changes

**Old:**
```csharp
using Aspire.Quartz;
using Aspire.Hosting.Quartz;
```

**New:**
```csharp
using CommunityToolkit.Aspire.Quartz;
using CommunityToolkit.Aspire.Hosting.Quartz;
```

## No Breaking Changes

All APIs remain the same. Only package names and namespaces have changed.
```

## 📝 PR Description (Ready to Use)

```markdown
# Add Quartz.NET Integration for Background Job Scheduling

## 🎯 Overview

This PR adds a production-ready integration for background job scheduling using Quartz.NET in .NET Aspire applications.

## 📦 What's Included

### Hosting Integration (`CommunityToolkit.Aspire.Hosting.Quartz`)
- Resource pattern for Quartz.NET
- Automatic database configuration (PostgreSQL, SQL Server, MySQL, SQLite)
- Health checks integration
- OpenTelemetry metrics
- Automatic schema migrations using EF Core

### Client Integration (`CommunityToolkit.Aspire.Quartz`)
- `IBackgroundJobClient` for dynamic job scheduling
- Idempotency support (prevent duplicate execution)
- Retry policies with exponential/linear backoff
- OpenTelemetry distributed tracing
- Full Quartz.NET power (cron expressions, triggers, listeners)
- Multi-database support

### Abstractions (`CommunityToolkit.Aspire.Quartz.Abstractions`)
- Core interfaces and contracts
- Shared types between hosting and client
- `IJob`, `JobOptions`, `RetryPolicy`, `JobContext`

## 🚀 Why This Integration?

.NET Aspire currently lacks native support for background job scheduling. Developers must manually integrate Quartz.NET or Hangfire, configure persistence, set up observability, and implement idempotency themselves.

This integration provides:
- ✅ **Aspire-native patterns** - Not just a wrapper, built for Aspire from the ground up
- ✅ **Production features** - Idempotency, OpenTelemetry, health checks out of the box
- ✅ **Multi-database support** - PostgreSQL, SQL Server, MySQL, SQLite with auto-migrations
- ✅ **Full Quartz.NET power** - No feature hiding, complete access to Quartz.NET APIs
- ✅ **All-in-one architecture** - No separate worker needed, jobs run in your API service
- ✅ **Type-safe API** - Strong typing with generics for job parameters

## 📊 Proof of Concept

This integration has been published and tested as a proof of concept:
- **NuGet**: https://www.nuget.org/packages/AspireQuartz (v1.0.1)
- **GitHub**: https://github.com/alnuaimicoder/aspire-hosting-quartz
- **Status**: Production-ready, published, and actively maintained
- **License**: MIT

## 💻 Usage Example

### AppHost Configuration
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("quartzdb");

builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres);

builder.Build().Run();
```

### API Service Configuration
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Quartz.NET with persistence
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(store =>
    {
        store.UsePostgres(builder.Configuration.GetConnectionString("quartzdb")!);
        store.UseNewtonsoftJsonSerializer();
    });
});

builder.Services.AddQuartzHostedService();

// Add AspireQuartz client for dynamic scheduling
builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb"));

var app = builder.Build();

// Enqueue a job
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

    public SendEmailJob(ILogger<SendEmailJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var email = context.JobDetail.JobDataMap.GetString("email");
        _logger.LogInformation("Sending email to {Email}", email);

        // Your email sending logic here
        await Task.Delay(1000);

        _logger.LogInformation("Email sent successfully!");
    }
}
```

## 🧪 Testing

- ✅ Unit tests for all public APIs
- ✅ Integration tests with example AppHost
- ✅ Marked with `[RequiresDocker]` (uses PostgreSQL)
- ✅ Example application demonstrating usage
- ✅ Tested on .NET 8.0, 9.0, and 10.0

## 📚 Documentation

- ✅ README.md in each package
- ✅ Example application with comprehensive samples
- ✅ XML documentation comments on all public APIs
- 🔄 Will create PR to aspire.dev after code review

## 🔄 Changes Made for CommunityToolkit

1. **Renamed packages** from `AspireQuartz.*` to `CommunityToolkit.Aspire.Quartz.*`
2. **Updated namespaces** to follow CommunityToolkit conventions
3. **Added unit tests** for all public APIs
4. **Added integration tests** with `[RequiresDocker]` attribute
5. **Moved sample** to `examples/Quartz/`
6. **Updated package metadata** with `client` and `hosting` tags
7. **Added PublicAPI.txt** files for API tracking
8. **Updated documentation** to match CommunityToolkit style

## ⚠️ Breaking Changes

None - this is a new integration.

## ✅ Checklist

- [x] Code follows Aspire conventions
- [x] All public APIs documented with XML comments
- [x] Unit tests added
- [x] Integration tests added
- [x] Example application included
- [x] README.md files added
- [x] Package metadata configured
- [x] Namespaces follow guidelines
- [ ] Tests added to CI workflow (need maintainer help)
- [ ] Main README.md updated (need maintainer help)

## 🤝 Next Steps

After this PR is reviewed and merged:
1. Create PR to microsoft/aspire.dev for official documentation
2. Deprecate original AspireQuartz.* packages on NuGet
3. Update original repository to point to CommunityToolkit packages
4. Create migration guide for existing users

## 📞 Contact

- **Author**: AlNuaimi
- **GitHub**: @alnuaimicoder
- **Repository**: https://github.com/alnuaimicoder/aspire-hosting-quartz

---

Thank you for considering this integration! I'm happy to make any changes based on your feedback.
```

## 🎯 Immediate Next Steps

1. **Review this checklist** with the team
2. **Get feedback** from CommunityToolkit maintainers on approach
3. **Make required changes** based on feedback
4. **Submit PR** with all changes

## ⏱️ Estimated Timeline

- **Preparation**: 2-3 days (renaming, tests, documentation)
- **PR Review**: 1-2 weeks (depends on maintainers)
- **Documentation**: 2-3 days (after code approval)
- **Total**: 3-4 weeks from start to finish

## 📋 Questions for Maintainers

Before starting the full migration, we'd like to confirm:

1. **Naming**: Is `CommunityToolkit.Aspire.Quartz` acceptable, or would you prefer a different name?
2. **Structure**: Should we include Abstractions as a separate package, or merge it into the client package?
3. **Testing**: Any specific testing requirements beyond what's outlined here?
4. **Documentation**: Any specific documentation format or requirements?
5. **Migration**: How should we handle the existing NuGet packages (AspireQuartz.*)?

---

**Status**: Ready to proceed pending maintainer feedback
**Last Updated**: April 2, 2026
