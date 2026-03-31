# Versioning Strategy

## Overview

CommunityToolkit.Aspire.Quartz follows semantic versioning (SemVer 2.0) and supports multiple .NET and Aspire versions through multi-targeting.

## .NET Version Support

### Multi-Targeting Strategy

All library packages are multi-targeted to support:

- **.NET 8.0** (LTS - supported until November 2026)
- **.NET 9.0** (STS - supported until May 2026)
- **Future versions** (.NET 10.0+) - automatically supported through multi-targeting

### Configuration

```xml
<PropertyGroup>
  <TargetFrameworks>net8.0;net9.0</TargetFrameworks>
</PropertyGroup>
```

### Why Multi-Targeting?

1. **Maximum Compatibility**: Users can consume the library regardless of their .NET version
2. **Performance Optimization**: Each target can leverage version-specific optimizations
3. **Future-Proof**: New .NET versions are automatically supported
4. **No Breaking Changes**: Users don't need to upgrade their .NET version to use the library

## Aspire Version Support

### Current Support

- **Aspire 9.0+**: Primary target
- **Aspire SDK**: Uses `Aspire.AppHost.Sdk/9.0.0` for AppHost projects
- **Aspire Packages**: All Aspire.Hosting.* packages at version 9.0.0

### Aspire Support Policy

According to [Microsoft's Aspire Support Policy](https://dotnet.microsoft.com/platform/support/policy/aspire):

> At any time, only the latest release of Aspire is supported by Microsoft.

**Our Strategy**:
- We target the latest stable Aspire version (currently 9.0)
- When new Aspire versions are released, we update within 30 days
- We maintain backward compatibility where possible
- Breaking changes are clearly documented in CHANGELOG.md

## Package Versioning

### Semantic Versioning (SemVer 2.0)

Format: `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]`

- **MAJOR**: Breaking changes (e.g., 1.0.0 → 2.0.0)
- **MINOR**: New features, backward compatible (e.g., 1.0.0 → 1.1.0)
- **PATCH**: Bug fixes, backward compatible (e.g., 1.0.0 → 1.0.1)
- **PRERELEASE**: Alpha, beta, rc versions (e.g., 1.0.0-beta.1)

### Version Alignment

All three packages share the same version number:

- `CommunityToolkit.Aspire.Quartz.Abstractions`
- `CommunityToolkit.Aspire.Quartz`
- `CommunityToolkit.Aspire.Hosting.Quartz`

**Example**: If you use `CommunityToolkit.Aspire.Quartz 1.2.0`, you should also use `CommunityToolkit.Aspire.Hosting.Quartz 1.2.0`.

## Release Cadence

### Stable Releases

- **Major**: When breaking changes are necessary (rare)
- **Minor**: Every 2-3 months with new features
- **Patch**: As needed for bug fixes (within 1-2 weeks of discovery)

### Pre-release Versions

- **Alpha** (`-alpha.X`): Early development, unstable API
- **Beta** (`-beta.X`): Feature complete, API stable, testing phase
- **RC** (`-rc.X`): Release candidate, production-ready testing

## Upgrade Path

### From .NET 8 to .NET 9

No code changes required! The library automatically uses the appropriate target:

```bash
# Your project targets net9.0
dotnet add package CommunityToolkit.Aspire.Quartz

# NuGet automatically selects the net9.0 assembly
```

### From .NET 9 to .NET 10

When .NET 10 is released:

1. We add `net10.0` to `TargetFrameworks`
2. Release a new MINOR version (e.g., 1.3.0)
3. No breaking changes for existing users
4. .NET 10 users get optimized assemblies

## Dependency Versioning

### Aspire Dependencies

```xml
<PackageReference Include="Aspire.Hosting" Version="9.0.0" />
<PackageReference Include="Aspire.Hosting.SqlServer" Version="9.0.0" />
<PackageReference Include="Aspire.Hosting.PostgreSQL" Version="9.0.0" />
```

### Quartz.NET Dependencies

```xml
<PackageReference Include="Quartz" Version="3.13.1" />
<PackageReference Include="Quartz.Extensions.Hosting" Version="3.13.1" />
<PackageReference Include="Quartz.Serialization.Json" Version="3.13.1" />
```

### OpenTelemetry Dependencies

```xml
<PackageReference Include="OpenTelemetry" Version="1.10.0" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.10.0" />
```

## Breaking Changes Policy

### What Constitutes a Breaking Change?

- Removing public APIs
- Changing method signatures
- Changing behavior that breaks existing code
- Removing support for a .NET version
- Upgrading to a new major version of Aspire

### How We Handle Breaking Changes

1. **Deprecation Period**: Mark APIs as `[Obsolete]` for at least one MINOR version
2. **Migration Guide**: Provide clear upgrade instructions in CHANGELOG.md
3. **Major Version Bump**: Only introduce breaking changes in MAJOR versions
4. **Communication**: Announce breaking changes in release notes and GitHub discussions

## Version Compatibility Matrix

| Library Version | .NET 8.0 | .NET 9.0 | .NET 10.0 | Aspire Version |
|----------------|----------|----------|-----------|----------------|
| 1.0.0          | ✅       | ✅       | ⏳        | 9.0+           |
| 1.1.0          | ✅       | ✅       | ⏳        | 9.0+           |
| 2.0.0 (future) | ✅       | ✅       | ✅        | 10.0+          |

Legend:
- ✅ Supported
- ⏳ Will be supported when .NET 10 is released
- ❌ Not supported

## Testing Strategy

### Multi-Target Testing

All tests run against all target frameworks:

```bash
dotnet test --framework net8.0
dotnet test --framework net9.0
```

### CI/CD Pipeline

- Build and test on .NET 8.0 and 9.0
- Integration tests with SQL Server and PostgreSQL
- Package validation before publishing

## Questions?

For questions about versioning or compatibility, please:

1. Check the [CHANGELOG.md](CHANGELOG.md) for version history
2. Review [GitHub Releases](https://github.com/alnuaimicoder/aspire-hosting-quartz/releases)
3. Open a [GitHub Discussion](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
4. File an [Issue](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues) for bugs

## References

- [Semantic Versioning 2.0](https://semver.org/)
- [.NET Library Guidance](https://learn.microsoft.com/dotnet/standard/library-guidance/)
- [Cross-Platform Targeting](https://learn.microsoft.com/dotnet/standard/library-guidance/cross-platform-targeting)
- [Aspire Support Policy](https://dotnet.microsoft.com/platform/support/policy/aspire)
- [.NET Support Policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core)
