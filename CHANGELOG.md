# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project structure with three NuGet packages
- Core abstractions library (Library.Quartz.Abstractions)
- Client library for job enqueuing (CommunityToolkit.Aspire.Quartz)
- Hosting library for AppHost integration (CommunityToolkit.Aspire.Hosting.Quartz)
- Background job client with OpenTelemetry tracing
- Idempotency key support for duplicate prevention
- Cron expression validation
- Retry policy with exponential/linear backoff
- SQL Server and PostgreSQL support
- Automatic database migration service
- Quartz.NET schema creation scripts
- Aspire resource pattern implementation
- Fluent configuration API (WithDatabase, WithMaxConcurrency, etc.)
- Automatic service discovery integration
- Comprehensive XML documentation
- Open source project files (LICENSE, CONTRIBUTING, CODE_OF_CONDUCT)
- Community Toolkit naming standards compliance
- Aspire CLI support (dotnet aspire add)

### Changed
- BREAKING: Renamed packages to follow Aspire Community Toolkit standards
  - Aspire.Library.Quartz.Client → CommunityToolkit.Aspire.Quartz
  - Aspire.Library.Quartz.Hosting → CommunityToolkit.Aspire.Hosting.Quartz
- Updated all namespaces to CommunityToolkit.Aspire.*
- Added IsAspireClientIntegration and IsAspireHostingIntegration properties

### Deprecated
- N/A (initial release)

### Removed
- N/A (initial release)

### Fixed
- N/A (initial release)

### Security
- N/A (initial release)

---

## [0.1.0-alpha] - 2026-03-31

### Added
- Initial alpha release
- Core abstractions and client library
- Basic job enqueuing and scheduling functionality

[Unreleased]: https://github.com/alnuaimicoder/aspire-hosting-quartz/compare/v0.1.0-alpha...HEAD
[0.1.0-alpha]: https://github.com/alnuaimicoder/aspire-hosting-quartz/releases/tag/v0.1.0-alpha
