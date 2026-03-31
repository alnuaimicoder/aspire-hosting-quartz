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
- **Enqueue jobs** for immediate execution
- **Schedule with delay** using TimeSpan
- **Schedule with cron** expressions for recurring jobs
- **Job priorities** for execution order control

### 💾 Database Support
- **SQL Server** - Full support with automatic migrations
- **PostgreSQL** - Full support with automatic migrations
- **Automatic schema** creation on startup
- **Persistent storage** survives application restarts

### 🔄 Reliability
- **Retry policies** - Exponential and linear backoff strategies
- **Idempotency** - Prevent duplicate job execution
- **Job validation** - Early parameter and cron validation
- **Error handling** - Clear, actionable error messages

### 📊 Observability
- **OpenTelemetry tracing** - Distributed tracing for all operations
- **Aspire Dashboard** - Real-time monitoring and metrics
- **Structured logging** - Comprehensive logging with context
- **Metrics emission** - Job count, duration, failures

### 🎯 Developer Experience
- **Multi-targeting** - .NET 8.0 and 9.0 support
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

var sqlserver = builder.AddSqlServer("sql")
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

builder.AddProject<Projects.ApiService>("api")
    .WithReference(quartz);
```

**2. Define a Job:**
```csharp
public class SendEmailJob : IJob
{
    public async Task ExecuteAsync(JobContext context, CancellationToken ct)
    {
        var email = context.Parameters["email"]?.ToString();
        // Your logic here
    }
}
```

**3. Enqueue Jobs:**
```csharp
await jobClient.EnqueueAsync<SendEmailJob>(
    new { email = "user@example.com" },
    new JobOptions {
        IdempotencyKey = "email-123",
        RetryPolicy = new RetryPolicy { MaxAttempts = 3 }
    }
);
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
- ✅ Complete job scheduling system
- ✅ Three production-ready NuGet packages
- ✅ Multi-targeting (.NET 8.0 and 9.0)
- ✅ SQL Server and PostgreSQL support
- ✅ Automatic database migrations
- ✅ Retry policies with backoff strategies
- ✅ Idempotency support
- ✅ Cron expression scheduling

### Observability
- ✅ OpenTelemetry distributed tracing
- ✅ Aspire Dashboard integration
- ✅ Structured logging
- ✅ Metrics emission

### Developer Tools
- ✅ Complete sample application
- ✅ Comprehensive documentation
- ✅ Local CI scripts
- ✅ GitHub templates
- ✅ Security policy

---

## 🔧 Technical Details

### Supported Platforms
- **.NET**: 8.0 (LTS), 9.0 (STS)
- **.NET Aspire**: 9.0+
- **SQL Server**: 2019+
- **PostgreSQL**: 12+

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

- ✅ Job enqueuing from REST API
- ✅ Job execution in Worker service
- ✅ SQL Server persistence
- ✅ Retry policies
- ✅ Idempotency
- ✅ OpenTelemetry tracing
- ✅ Aspire Dashboard monitoring

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

**Release Date**: March 31, 2026
**Version**: 1.0.0
**Status**: Stable

**Full Changelog**: https://github.com/alnuaimicoder/aspire-hosting-quartz/blob/main/CHANGELOG.md
