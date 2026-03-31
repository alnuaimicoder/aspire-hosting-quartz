# Aspire.Hosting.Quartz

[![CI](https://github.com/alnuaimicoder/aspire-hosting-quartz/actions/workflows/ci.yml/badge.svg)](https://github.com/alnuaimicoder/aspire-hosting-quartz/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Enterprise-grade background job scheduling for .NET Aspire using Quartz.NET.

## 🚀 Features

- **Seamless .NET Aspire integration** - First-class resource pattern support
- **Built-in OpenTelemetry observability** - Distributed tracing and metrics
- **Automatic retry policies** - Exponential backoff with configurable strategies
- **Idempotency guarantees** - Prevent duplicate job execution
- **Persistent job storage** - SQL Server and PostgreSQL support
- **Cron-based scheduling** - Complex scheduling with cron expressions
- **Real-time metrics** - Monitor jobs in Aspire Dashboard

## 📦 Installation

```bash
# In your AppHost project
dotnet add package Aspire.Library.Quartz.Hosting

# In your API/Service projects
dotnet add package Aspire.Library.Quartz.Client
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

- [Architecture Overview](docs/architecture.md)
- [Getting Started Guide](docs/getting-started.md)
- [Configuration Reference](docs/configuration.md)
- [Observability Guide](docs/observability.md)
- [Troubleshooting](docs/troubleshooting.md)

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

Built with ❤️ for the .NET Aspire community.
