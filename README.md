# 🚀 AspireQuartz

[![NuGet](https://img.shields.io/nuget/v/AspireQuartz.svg)](https://www.nuget.org/packages/AspireQuartz/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AspireQuartz.svg)](https://www.nuget.org/packages/AspireQuartz/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0+-512BD4)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-13.2+-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)
[![GitHub stars](https://img.shields.io/github/stars/alnuaimicoder/aspire-hosting-quartz)](https://github.com/alnuaimicoder/aspire-hosting-quartz/stargazers)

**The standard way to do background jobs in .NET Aspire** - Production-ready job scheduling with Quartz.NET.

## 🎯 Why This Library?

.NET Aspire is missing a critical piece: **background job scheduling**.

This isn't "Quartz for Aspire" - it's an **Aspire-native job scheduling platform** that happens to use Quartz.NET as the engine.

### What Makes It Different?

| Feature | Quartz.NET | Hangfire | This Library |
|---------|-----------|----------|--------------|
| Aspire Integration | ❌ Manual | ❌ Manual | ✅ Native |
| Resource Pattern | ❌ | ❌ | ✅ |
| OpenTelemetry | ❌ | ❌ | ✅ |
| Idempotency | ❌ | ❌ | ✅ |
| Real-time Updates | ❌ | ✅ | ✅ |
| Full Quartz Power | ✅ | ❌ | ✅ |
| Cloud-native DX | ❌ | ⚠️ | ✅ |

## 🚀 Features

### Production-Ready
- ✅ **Idempotency** - Prevent duplicate job execution
- ✅ **OpenTelemetry** - Distributed tracing out of the box
- ✅ **Health Checks** - Aspire Dashboard integration
- ✅ **Real-time Updates** - SignalR for live job status
- ✅ **Multi-database** - PostgreSQL, SQL Server, MySQL, SQLite
- ✅ **Auto-migrations** - Automatic table creation using EF Core

### Aspire Integration
- ✅ **Resource Pattern** - First-class citizen in Aspire
- ✅ **Connection Injection** - Automatic configuration
- ✅ **Service Discovery** - Works with Aspire patterns
- ✅ **Dashboard Support** - Full observability

### Developer Experience
- ✅ **Native Quartz.NET** - Full power, no abstractions hiding features
- ✅ **Simplified Client** - For dynamic scheduling
- ✅ **Type Safety** - Strong typing with generics
- ✅ **Clean API** - Intuitive and discoverable

## 📋 Requirements

- **.NET SDK**: 8.0 or higher
- **.NET Aspire**: 13.2.0 or higher
- **Quartz.NET**: 3.13.1 or higher
- **Database**: PostgreSQL 12+ (primary), SQL Server 2019+, MySQL 8.0+, or SQLite 3.0+
- **Auto-migrations**: Uses `AppAny.Quartz.EntityFrameworkCore.Migrations` for automatic table creation

## 📦 Installation

### Using .NET CLI (Recommended)

```bash
# In your AppHost project
dotnet add package AspireQuartz.Hosting

# In your API/Service projects
dotnet add package AspireQuartz
```

### Using Aspire CLI

```bash
# In your AppHost project
aspire add AspireQuartz.Hosting

# In your API/Service projects
aspire add AspireQuartz
```

## 🎯 Quick Start

### 1. Configure AppHost

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("quartzdb");

// Add your API service (jobs run here - no separate worker needed!)
builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres);

builder.Build().Run();
```

**That's it!** No separate worker service needed - jobs run in your API service.

### 2. Configure API Service

```csharp
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

    // Configure recurring jobs
    q.ScheduleJob<HealthCheckJob>(trigger => trigger
        .WithIdentity("health-check-trigger")
        .WithCronSchedule("0 */5 * * * ?") // Every 5 minutes
        .UsingJobData("endpoint", "https://api.example.com/health"));
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Add AspireQuartz client for dynamic scheduling (MUST be called after AddQuartz)
builder.Services.AddQuartzClient(builder.Configuration.GetConnectionString("quartzdb"));

var app = builder.Build();
app.Run();
```

### 3. Define a Job

```csharp
public class HealthCheckJob : IJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckJob> _logger;

    public HealthCheckJob(IHttpClientFactory httpClientFactory, ILogger<HealthCheckJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var endpoint = context.JobDetail.JobDataMap.GetString("endpoint");
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync(endpoint);
        _logger.LogInformation("Health check for {Endpoint}: {Status}",
            endpoint, response.StatusCode);
    }
}
```

### 4. Schedule Jobs Dynamically

```csharp
public class JobsController : ControllerBase
{
    private readonly IBackgroundJobClient _jobClient;

    // Enqueue a job for immediate execution
    [HttpPost("enqueue")]
    public async Task<IActionResult> EnqueueJob()
    {
        var jobId = await _jobClient.EnqueueAsync<SendEmailJob>(
            new { email = "user@example.com" },
            new JobOptions { IdempotencyKey = "email-123" });

        return Ok(new { jobId });
    }

    // Schedule with delay
    [HttpPost("schedule-delay")]
    public async Task<IActionResult> ScheduleWithDelay()
    {
        var jobId = await _jobClient.ScheduleAsync<SendEmailJob>(
            new { email = "user@example.com" },
            TimeSpan.FromMinutes(5));

        return Ok(new { jobId });
    }

    // Schedule with cron
    [HttpPost("schedule-cron")]
    public async Task<IActionResult> ScheduleWithCron()
    {
        var jobId = await _jobClient.ScheduleAsync<SendEmailJob>(
            new { email = "user@example.com" },
            "0 0 9 * * ?"); // Every day at 9 AM

        return Ok(new { jobId });
    }
}
```

## 📚 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Complete step-by-step tutorial
- **[Database Setup Guide](samples/DATABASE_SETUP.md)** - Multi-database configuration
- **[Sample Application](samples/README.md)** - Working example with all features
- **[Versioning Policy](docs/VERSIONING.md)** - Multi-targeting and support policy
- **[Changelog](CHANGELOG.md)** - Release history and changes
- **[Roadmap](ROADMAP.md)** - Future plans and features
- **[Contributing](CONTRIBUTING.md)** - How to contribute
- **[Security Policy](SECURITY.md)** - Reporting vulnerabilities

## 🤝 Contributing

We welcome contributions! Here's how you can help:

### 🌟 Ways to Contribute

- 🐛 **Report bugs** - Found an issue? [Open a bug report](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues/new?template=bug_report.yml)
- 💡 **Suggest features** - Have an idea? [Request a feature](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues/new?template=feature_request.yml)
- 📖 **Improve docs** - Help us make documentation better
- 🧪 **Add tests** - Increase code coverage
- 💻 **Submit PRs** - Fix bugs or add features

### 🎯 Good First Issues

New to the project? Check out issues labeled [`good first issue`](https://github.com/alnuaimicoder/aspire-hosting-quartz/labels/good%20first%20issue):

- [Add more unit tests for RetryPolicy](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues/1)
- [Improve README with more examples](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues/2)

### 📋 Quick Start for Contributors

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git`
3. **Create** a branch: `git checkout -b feature/your-feature`
4. **Make** your changes
5. **Test** locally: `.\scripts\quick-check.ps1`
6. **Commit**: `git commit -m "feat: add your feature"`
7. **Push**: `git push origin feature/your-feature`
8. **Open** a Pull Request

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

### 💬 Join the Community

- 💭 [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions) - Ask questions, share ideas
- 🐛 [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues) - Report bugs, request features
- ⭐ **Star the repo** - Show your support!

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

Built with ❤️ for the .NET Aspire community.

Special thanks to:
- The .NET team for the amazing platform
- The Aspire team for the cloud-native framework
- The Quartz.NET team for the robust scheduling engine
- All contributors and users

## 📞 Support

- **Documentation**: [Getting Started Guide](GETTING_STARTED.md)
- **Issues**: [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- **Discussions**: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- **Security**: See [SECURITY.md](SECURITY.md)

---

**Made with ❤️ by the community**
