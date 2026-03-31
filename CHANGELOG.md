# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Unit tests with 80%+ coverage
- Integration tests with Testcontainers
- PostgreSQL sample
- Cron scheduling sample
- API documentation with DocFX
- Performance benchmarks

## [1.0.0] - TBD

### Added

#### Core Features
- **Background Job Scheduling**: Complete job scheduling system for .NET Aspire
- **Three NuGet Packages**:
  - `CommunityToolkit.Aspire.Quartz.Abstractions` - Core interfaces and contracts
  - `CommunityToolkit.Aspire.Quartz` - Client library for job enqueuing
  - `CommunityToolkit.Aspire.Hosting.Quartz` - Hosting integration for Aspire
- **Multi-Targeting**: Support for .NET 8.0 and .NET 9.0
- **Job Enqueuing**: Enqueue jobs for immediate execution
- **Delayed Scheduling**: Schedule jobs with TimeSpan delay
- **Cron Scheduling**: Schedule jobs with cron expressions
- **Retry Policies**: Configurable retry with exponential and linear backoff
- **Idempotency**: Prevent duplicate job execution with idempotency keys
- **Database Support**: SQL Server and PostgreSQL persistence
- **Automatic Migration**: Database schema created automatically on startup

#### Observability
- **OpenTelemetry Integration**: Distributed tracing for all operations
- **Metrics**: Job execution count, duration, and failure metrics
- **Structured Logging**: Comprehensive logging with job context
- **Aspire Dashboard**: Full integration with Aspire Dashboard

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
- .NET Aspire 9.0+
- SQL Server 2019+
- PostgreSQL 12+

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
