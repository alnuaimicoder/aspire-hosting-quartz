# Release Notes - v1.0.0

## 🎉 First Stable Release

We're excited to announce the first stable release of **CommunityToolkit.Aspire.Quartz** - Enterprise-grade background job scheduling for .NET Aspire!

---

## 📦 What's Included

### NuGet Packages

Three packages are now available on NuGet.org:

| Package | Version | Description |
|---------|---------|-------------|
| [CommunityToolkit.Aspire.Quartz.Abstractions](https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz.Abstractions/1.0.0) | 1.0.0 | Core interfaces and contracts |
| [CommunityToolkit.Aspire.Quartz](https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz/1.0.0) | 1.0.0 | Client library for job enqueuing |
| [CommunityToolkit.Aspire.Hosting.Quartz](https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Quartz/1.0.0) | 1.0.0 | Hosting integration for Aspire |

---

## ✨ Key Features

### 🚀 Background Job Scheduling
- **Native Quartz.NET integration** - Full access to IScheduler API
- **Enqueue jobs** for immediate execution
- **Schedule with delay** using TimeSpan
- **Schedule with cron** expressions for recurring jobs
- **Dynamic job scheduling** via API endpoints

### 💾 Database Support
- **PostgreSQL** - Primary database with pgAdmin support
- **SQL Server** - Also supported with automatic migrations
- **Automatic schema** creation on startup
- **Persistent storage** survives application restarts

### 🔄 Reliability
- **Idempotency** - Prevent duplicate job execution
- **Job validation** - Early parameter and cron validation
- **Error handling** - Clear, actionable error messages
- **Health checks** - Built-in Quartz scheduler health check

### 📊 Observability
- **OpenTelemetry tracing** - Distributed tracing for all operations
- **Aspire Dashboard** - Real-time monitoring
- **SignalR integration** - Live job status updates
- **Structured logging** - Comprehensive logging with context

### 🎯 Developer Experience
- **Multi-targeting** - .NET 8.0, 9.0, and 10.0+ support
- **Fluent API** - Clean, intuitive configuration
- **Type safety** - Strong typing with generics
- **XML documentation** - Full IntelliSense support
- **Aspire CLI** - Install with `aspire add` command

---

## 🚀 Quick Start

### Installation

Using Aspire CLI (Recommended):
```bash
aspire add CommunityToolkit.Aspire.Hosting.Quartz
aspire add CommunityToolkit.Aspire.Quartz
```

Using .NET CLI:
```bash
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
dotnet add package CommunityToolkit.Aspire.Quartz
```

### Basic Usage

**1. Configure in AppHost:**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithPostgreSql(postgres);

builder.AddProject<Projects.ApiService>("api")
    .WithReference(quartz);
```

**2. Configure API Service:**
```csharp
// Add Quartz.NET with full scheduling power
builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<HealthCheckJob>(trigger => trigger
        .WithCronSchedule("0 */5 * * * ?") // Every 5 minutes
        .UsingJobData("endpoint", "https://api.example.com/health"));
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

// Add Quartz client (MUST be called after AddQuartz)
builder.Services.AddQuartzClient("quartzdb");
```

**3. Define a Job:**
```csharp
public class HealthCheckJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var endpoint = context.JobDetail.JobDataMap.GetString("endpoint");
        // Your logic here
    }
}
```

**4. Schedule Jobs Dynamically:**
```csharp
await scheduler.ScheduleJob(job, trigger);
```

---

## 📖 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Complete step-by-step tutorial
- **[Sample Application](samples/README.md)** - Working example with API and Worker
- **[README](README.md)** - Full documentation and API reference
- **[Versioning Policy](docs/VERSIONING.md)** - Multi-targeting and support policy
- **[Changelog](CHANGELOG.md)** - Detailed change history
- **[Roadmap](ROADMAP.md)** - Future plans and features

---

## 🎯 What's New in v1.0.0

### Core Features
- ✅ Complete job scheduling system using Quartz.NET
- ✅ Three production-ready NuGet packages
- ✅ Multi-targeting (.NET 8.0, 9.0, and 10.0+)
- ✅ PostgreSQL and SQL Server support
- ✅ Automatic database migrations
- ✅ Idempotency support
- ✅ Cron expression scheduling
- ✅ SignalR real-time updates

### Observability
- ✅ OpenTelemetry distributed tracing
- ✅ Aspire Dashboard integration
- ✅ Health checks
- ✅ Structured logging

### Developer Tools
- ✅ Complete sample application
- ✅ Comprehensive documentation
- ✅ Local CI scripts
- ✅ GitHub templates
- ✅ Security policy

---

## 🔧 Technical Details

### Supported Platforms
- **.NET**: 8.0 (LTS), 9.0 (STS), 10.0+ (samples)
- **.NET Aspire**: 13.2.0+
- **Quartz.NET**: 3.13.1+
- **PostgreSQL**: 12+ (primary)
- **SQL Server**: 2019+ (supported)

### Dependencies
- Minimal external dependencies
- OpenTelemetry for observability
- Quartz.NET for scheduling engine
- Database drivers (SQL Server/PostgreSQL)

### Package Sizes
- Abstractions: ~32 KB
- Client: ~49 KB
- Hosting: ~42 KB

---

## 📝 Sample Application

A complete working sample is included demonstrating:

- ✅ Job scheduling from REST API
- ✅ Job execution with Quartz.NET
- ✅ PostgreSQL persistence with pgAdmin
- ✅ SignalR real-time updates
- ✅ Idempotency
- ✅ OpenTelemetry tracing
- ✅ Aspire Dashboard monitoring
- ✅ Blazor dashboard with live updates

See [samples/README.md](samples/README.md) for details.

---

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Ways to Contribute
- 🐛 Report bugs
- 💡 Suggest features
- 📖 Improve documentation
- 🧪 Add tests
- 💻 Submit pull requests

---

## 🔒 Security

Security is a priority. See [SECURITY.md](SECURITY.md) for:
- Reporting vulnerabilities
- Security best practices
- Supported versions
- Response timeline

---

## 🗺️ Roadmap

See [ROADMAP.md](ROADMAP.md) for planned features:

### v1.1.0 (Q2 2026)
- Additional database providers (MySQL, MongoDB)
- Job cancellation support
- Enhanced monitoring dashboard

### v1.2.0 (Q3 2026)
- Job priority queues
- Batch job operations
- Performance optimizations

### v2.0.0 (Q4 2026)
- Management UI
- Advanced scheduling patterns
- Multi-tenant support

---

## 🙏 Acknowledgments

Special thanks to:
- The .NET team for the amazing platform
- The Aspire team for the cloud-native framework
- The Quartz.NET team for the robust scheduling engine
- All contributors and early adopters

---

## 📞 Support & Community

- **Documentation**: [Getting Started Guide](GETTING_STARTED.md)
- **Questions**: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- **Bug Reports**: [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- **Security**: [Security Policy](SECURITY.md)

---

## 🔗 Links

- **GitHub**: https://github.com/alnuaimicoder/aspire-hosting-quartz
- **NuGet**: https://www.nuget.org/packages?q=CommunityToolkit.Aspire.Quartz
- **.NET Aspire**: https://learn.microsoft.com/dotnet/aspire/
- **Quartz.NET**: https://www.quartz-scheduler.net/

---

## 📄 License

MIT License - See [LICENSE](LICENSE) for details.

---

**Release Date**: April 1, 2026
**Version**: 1.0.0
**Status**: Stable

**Full Changelog**: https://github.com/alnuaimicoder/aspire-hosting-quartz/blob/main/CHANGELOG.md
