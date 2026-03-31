# CommunityToolkit.Aspire.Quartz.Abstractions

Core abstractions for background job scheduling in .NET Aspire using Quartz.NET.

## Installation

```bash
dotnet add package CommunityToolkit.Aspire.Quartz.Abstractions
```

## What's Included

- `IBackgroundJobClient` - Interface for enqueuing and scheduling jobs
- `IJob` - Base interface for job implementations
- `JobContext` - Execution context for jobs
- `JobOptions` - Configuration options for jobs
- `RetryPolicy` - Retry configuration with exponential/linear backoff

## Usage

```csharp
public class SendEmailJob : IJob
{
    public async Task ExecuteAsync(JobContext context, CancellationToken cancellationToken)
    {
        var email = context.Parameters["email"]?.ToString();
        // Send email logic
    }
}
```

## Documentation

Visit [GitHub Repository](https://github.com/alnuaimicoder/aspire-hosting-quartz) for full documentation.

## License

MIT License - See [LICENSE](https://github.com/alnuaimicoder/aspire-hosting-quartz/blob/main/LICENSE)
