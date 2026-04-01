# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Unit tests with 80%+ coverage
- Integration tests with Testcontainers
- API documentation with DocFX
- Performance benchmarks

## [1.0.0] - 2026-04-01

### Added

#### Core Features
- **Background Job Scheduling**: Complete job scheduling system for .NET Aspire using Quartz.NET
- **Three NuGet Packages**:
  - `CommunityToolkit.Aspire.Quartz.Abstractions` - Core interfaces and contracts
  - `CommunityToolkit.Aspire.Quartz` - Client library with IScheduler integration
  - `CommunityToolkit.Aspire.Hosting.Quartz` - Hosting integration for Aspire
- **Multi-Targeting**: Support for .NET 8.0 and .NET 9.0
- **Native Quartz.NET Scheduling**: Full access to Quartz.NET features via IScheduler
- **Job Enqueuing**: Enqueue jobs for immediate execution
- **Delayed Scheduling**: Schedule jobs with TimeSpan delay
- **Cron Scheduling**: Schedule jobs with cron expressions
- **Idempotency**: Prevent duplicate job execution with idempotency keys
- **Database Support**: PostgreSQL persistence (SQL Server also supported)
- **Automatic Migration**: Database schema created automatically on startup
- **Real-Time Updates**: SignalR integration for live job status updates

#### Observability
- **OpenTelemetry Integration**: Distributed tracing for all operations
- **Health Checks**: Built-in health check for Quartz scheduler
- **Structured Logging**: Comprehensive logging with job context
- **Aspire Dashboard**: Full integration with Aspire Dashboard
- **SignalR Hub**: Real-time job execution notifications

#### Developer Experience
- **Fluent API**: Clean, intuitive API design
- **Service Discovery**: Automatic connection string configuration
- **Type Safety**: Strong typing with generic job definitions
- **Validation**: Early parameter and cron expression validation
- **Clear Errors**: Actionable error messages

#### Documentation
- Complete README with examples
- Getting Started guide
- Versioning policy
- Security policy
- Sample project with walkthrough
- API documentation (XML comments)
- Contributing guidelines
- Code of Conduct

#### Infrastructure
- GitHub issue templates (Bug, Feature, Question)
- Pull request template
- CI workflow for .NET 8.0 and 9.0
- Release checklist
- Security policy

### Technical Details

#### Package Naming
- Follows Microsoft .NET naming conventions
- Uses `CommunityToolkit.Aspire.*` prefix
- Aligns with Aspire Community Toolkit standards

#### Architecture
- Client-Host separation pattern
- Direct ADO.NET for database access
- No Quartz.NET dependency in client library
- Minimal external dependencies

#### Compatibility
- .NET 8.0 (LTS - supported until November 2026)
- .NET 9.0 (STS - supported until May 2026)
- .NET 10.0 (samples use .NET 10.0)
- .NET Aspire 13.2.0+
- Quartz.NET 3.13.1+
- PostgreSQL 12+ (primary)
- SQL Server 2019+ (supported)

### Breaking Changes
- N/A (initial release)

### Migration Guide
- N/A (initial release)

## Release Notes

### v1.0.0 - Initial Release

This is the first stable release of CommunityToolkit.Aspire.Quartz, providing enterprise-grade background job scheduling for .NET Aspire applications.

**Highlights:**
- 🚀 Production-ready job scheduling
- 📊 Built-in observability with OpenTelemetry
- 🔄 Automatic retry with configurable policies
- 🔒 Idempotency support
- 💾 Persistent storage (SQL Server & PostgreSQL)
- 📅 Cron-based scheduling
- 🎯 Multi-targeting (.NET 8.0 & 9.0)

**Getting Started:**
```bash
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
dotnet add package CommunityToolkit.Aspire.Quartz
```

See [GETTING_STARTED.md](GETTING_STARTED.md) for a complete tutorial.

**Sample Project:**
A complete working sample is available in the [samples](samples/) directory demonstrating:
- Job enqueuing from API
- Job execution in Worker service
- Retry policies
- Idempotency
- Observability

**Community:**
- Report bugs: [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- Ask questions: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- Contribute: See [CONTRIBUTING.md](CONTRIBUTING.md)

---

## Version History

- **1.0.0** - Initial stable release (TBD)
- **0.1.0-alpha** - Development preview (not published)

## Support

For questions and support:
- 📖 [Documentation](README.md)
- 💬 [Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- 🐛 [Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- 🔒 [Security](SECURITY.md)

## Links

- [NuGet Packages](https://www.nuget.org/packages?q=CommunityToolkit.Aspire.Quartz)
- [GitHub Repository](https://github.com/alnuaimicoder/aspire-hosting-quartz)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)
- [Quartz.NET](https://www.quartz-scheduler.net/)
