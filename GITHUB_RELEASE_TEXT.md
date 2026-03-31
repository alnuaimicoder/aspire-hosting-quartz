# 🎉 v1.0.0 - First Stable Release

Enterprise-grade background job scheduling for .NET Aspire using Quartz.NET!

## 📦 Published Packages

- [CommunityToolkit.Aspire.Quartz.Abstractions 1.0.0](https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz.Abstractions/1.0.0)
- [CommunityToolkit.Aspire.Quartz 1.0.0](https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz/1.0.0)
- [CommunityToolkit.Aspire.Hosting.Quartz 1.0.0](https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Quartz/1.0.0)

## ✨ Highlights

- 🚀 **Production-ready** job scheduling for .NET Aspire
- 📊 **Built-in observability** with OpenTelemetry and Aspire Dashboard
- 🔄 **Automatic retry** with exponential/linear backoff
- 🔒 **Idempotency support** to prevent duplicate execution
- 💾 **Persistent storage** with SQL Server and PostgreSQL
- 📅 **Cron scheduling** for recurring jobs
- 🎯 **Multi-targeting** .NET 8.0 and 9.0

## 🚀 Quick Start

### Installation

Using Aspire CLI:
```bash
aspire add CommunityToolkit.Aspire.Hosting.Quartz
aspire add CommunityToolkit.Aspire.Quartz
```

Using .NET CLI:
```bash
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
dotnet add package CommunityToolkit.Aspire.Quartz
```

### Basic Example

**Configure in AppHost:**
```csharp
var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

builder.AddProject<Projects.ApiService>("api")
    .WithReference(quartz);
```

**Define a Job:**
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

**Enqueue Jobs:**
```csharp
await jobClient.EnqueueAsync<SendEmailJob>(
    new { email = "user@example.com" },
    new JobOptions {
        IdempotencyKey = "email-123",
        RetryPolicy = new RetryPolicy { MaxAttempts = 3 }
    }
);
```

## 📖 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Complete tutorial
- **[Sample Application](samples/README.md)** - Working example
- **[Full Documentation](README.md)** - API reference
- **[Changelog](CHANGELOG.md)** - Detailed changes
- **[Roadmap](ROADMAP.md)** - Future plans

## 🎯 What's Included

### Core Features
✅ Job enqueuing and scheduling
✅ Retry policies with backoff
✅ Idempotency support
✅ Cron expressions
✅ SQL Server & PostgreSQL
✅ Automatic migrations

### Observability
✅ OpenTelemetry tracing
✅ Aspire Dashboard integration
✅ Structured logging
✅ Metrics emission

### Developer Experience
✅ Multi-targeting (.NET 8.0 & 9.0)
✅ Fluent API
✅ Type safety
✅ XML documentation
✅ Complete samples

## 🔧 Technical Details

- **Platforms**: .NET 8.0 (LTS), 9.0 (STS)
- **Aspire**: 9.0+
- **Databases**: SQL Server 2019+, PostgreSQL 12+
- **License**: MIT

## 📝 Sample Application

Complete working sample included with:
- REST API for job enqueuing
- Worker service for job processing
- SQL Server persistence
- Retry policies and idempotency
- OpenTelemetry tracing

## 🤝 Contributing

Contributions welcome! See [CONTRIBUTING.md](CONTRIBUTING.md)

## 🔒 Security

Report vulnerabilities via [Security Policy](SECURITY.md)

## 🙏 Thanks

Special thanks to the .NET, Aspire, and Quartz.NET teams, and all contributors!

---

**Full Release Notes**: [RELEASE_NOTES.md](RELEASE_NOTES.md)
**Changelog**: [CHANGELOG.md](CHANGELOG.md)
**Support**: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)

---

**Made with ❤️ by the community**
