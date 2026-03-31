# CommunityToolkit.Aspire.Hosting.Quartz

Hosting library for background job scheduling in .NET Aspire using Quartz.NET.

## Installation

```bash
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
```

## Quick Start

```csharp
// In your AppHost
var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sql")
    .AddDatabase("quartzdb");

var quartz = builder.AddQuartz("quartz")
    .WithDatabase(sqlserver)
    .WithMaxConcurrency(20)
    .WithIdempotencyExpiration(TimeSpan.FromDays(7));

builder.AddProject<Projects.ApiService>("api")
    .WithReference(quartz);

builder.AddProject<Projects.Worker>("worker")
    .WithReference(sqlserver);
```

## Features

- Automatic database schema migration
- SQL Server & PostgreSQL support
- Configurable concurrency limits
- Idempotency key management
- Aspire Dashboard integration
- Health checks

## Configuration Options

- `WithDatabase()` - Configure database connection
- `WithMaxConcurrency()` - Set max concurrent jobs
- `WithIdempotencyExpiration()` - Set idempotency key expiration
- `WithoutMigration()` - Disable automatic migrations

## Documentation

Visit [GitHub Repository](https://github.com/alnuaimicoder/aspire-hosting-quartz) for full documentation.

## License

MIT License - See [LICENSE](https://github.com/alnuaimicoder/aspire-hosting-quartz/blob/main/LICENSE)
