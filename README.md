# CommunityToolkit.Aspire.Quartz

[![CI](https://github.com/alnuaimicoder/aspire-hosting-quartz/actions/workflows/ci.yml/badge.svg)](https://github.com/alnuaimicoder/aspire-hosting-quartz/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0+-512BD4)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-9.0+-512BD4)](https://learn.microsoft.com/dotnet/aspire/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)
[![GitHub issues](https://img.shields.io/github/issues/alnuaimicoder/aspire-hosting-quartz)](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
[![GitHub stars](https://img.shields.io/github/stars/alnuaimicoder/aspire-hosting-quartz)](https://github.com/alnuaimicoder/aspire-hosting-quartz/stargazers)

Enterprise-grade background job scheduling for .NET Aspire using Quartz.NET.

## 🚀 Features

- **Seamless .NET Aspire integration** - First-class resource pattern support
- **Built-in OpenTelemetry observability** - Distributed tracing and metrics
- **Automatic retry policies** - Exponential backoff with configurable strategies
- **Idempotency guarantees** - Prevent duplicate job execution
- **Persistent job storage** - SQL Server and PostgreSQL support
- **Cron-based scheduling** - Complex scheduling with cron expressions
- **Real-time metrics** - Monitor jobs in Aspire Dashboard
- **Multi-targeting support** - Works with .NET 8.0, 9.0, and future versions

## 📋 Requirements

- **.NET SDK**: 8.0 or higher
- **.NET Aspire**: 9.0 or higher
- **Database**: SQL Server or PostgreSQL (for persistent storage)

## 📦 Installation

### Using Aspire CLI (Recommended)

```bash
# In your AppHost project
aspire add CommunityToolkit.Aspire.Hosting.Quartz

# In your API/Service projects
aspire add CommunityToolkit.Aspire.Quartz
```

### Using .NET CLI

```bash
# In your AppHost project
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz

# In your API/Service projects
dotnet add package CommunityToolkit.Aspire.Quartz
```

## 🎯 Quick Start

### 1. Configure AppHost

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sql")
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver);

builder.AddProject<Projects.ApiService>("api")
    .WithReference(quartz);

builder.AddProject<Projects.WorkerService>("worker")
    .WithReference(quartz);
```

### 2. Define a Job

```csharp
public class SendEmailJob : IJob
{
    private readonly IEmailService _emailService;

    public SendEmailJob(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task ExecuteAsync(JobContext context, CancellationToken ct)
    {
        var email = context.Parameters["email"].ToString();
        await _emailService.SendAsync(email, ct);
    }
}
```

### 3. Enqueue Jobs

```csharp
public class EmailController : ControllerBase
{
    private readonly IBackgroundJobClient _jobClient;

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail(EmailRequest request)
    {
        var jobId = await _jobClient.EnqueueAsync<SendEmailJob>(
            parameters: new { email = request.Email },
            options: new JobOptions
            {
                IdempotencyKey = $"email-{request.Email}",
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 3,
                    Strategy = BackoffStrategy.Exponential
                }
            });

        return Ok(new { jobId });
    }
}
```

## 📚 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Complete step-by-step tutorial
- **[Sample Application](samples/README.md)** - Working example with API and Worker
- **[Versioning Policy](docs/VERSIONING.md)** - Multi-targeting and support policy
- **[Changelog](CHANGELOG.md)** - Release history and changes
- **[Roadmap](ROADMAP.md)** - Future plans and features
- **[Contributing](CONTRIBUTING.md)** - How to contribute
- **[Security Policy](SECURITY.md)** - Reporting vulnerabilities

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

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
