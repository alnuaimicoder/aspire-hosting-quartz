# 📋 Plan for Contributing AspireQuartz to Aspire Community Toolkit

## ✅ Current Status

We received positive feedback from the Aspire Community Toolkit team:
> "If you're interested in contributing it as an integration do open a PR. Make sure you have a read of the contributing guide to be sure it's structured appropriately and then we can review"

## 📚 Requirements from Contributing Guide

Based on the [CommunityToolkit/Aspire CONTRIBUTING.md](https://github.com/CommunityToolkit/Aspire/blob/main/CONTRIBUTING.md) and [create-integration.md](https://github.com/CommunityToolkit/Aspire/blob/main/docs/create-integration.md), here's what we need to do:

### 1. Repository Structure Requirements

#### ✅ Already Have:
- Source code in `src/` directory
- Sample application in `samples/` directory
- README.md files for each package
- MIT License
- Code of Conduct
- Contributing guidelines

#### ❌ Need to Adapt:

**Naming Convention:**
- Current: `Aspire.Quartz`, `Aspire.Hosting.Quartz`, `Aspire.Quartz.Abstractions`
- Required: `CommunityToolkit.Aspire.Quartz`, `CommunityToolkit.Aspire.Hosting.Quartz`, `CommunityToolkit.Aspire.Quartz.Abstractions`

**Namespace Convention:**
- Extension methods should be in `Aspire.Hosting` namespace (hosting) ✅ Already correct
- Extension methods should be in `Microsoft.Extensions.Hosting` namespace (client) ❌ Need to check
- Custom resources should be in `Aspire.Hosting.ApplicationModel` namespace ✅ Already correct

**Project Structure:**
- Move to `src/CommunityToolkit.Aspire.Hosting.Quartz/`
- Move to `src/CommunityToolkit.Aspire.Quartz/`
- Move to `src/CommunityToolkit.Aspire.Quartz.Abstractions/`

### 2. Testing Requirements

#### ❌ Need to Create:

**Unit Tests:**
- Create `tests/CommunityToolkit.Aspire.Hosting.Quartz.Tests/`
- Create `tests/CommunityToolkit.Aspire.Quartz.Tests/`
- Test public APIs using `DistributedApplication.CreateBuilder()`
- Use xUnit framework

**Integration Tests:**
- Inherit from `IClassFixture<AspireIntegrationTestFixture<TExampleAppHost>>`
- Test end-to-end functionality
- Mark with `[RequiresDocker]` attribute (we use PostgreSQL container)
- Use `WaitForTextAsync` if needed (with `CTASPIRE001` suppression)

**CI Integration:**
- Update `.github/workflows/tests.yml` in their repo
- Run `./eng/testing/generate-test-list-for-workflow.sh` to update test list

### 3. Example Application

#### ✅ Already Have:
- `samples/QuartzSample.ApiService/` - Demonstrates usage
- AppHost project that uses the integration

#### ⚠️ Need to Verify:
- Ensure it demonstrates minimal usage
- Make sure it's suitable for integration tests

### 4. Documentation

#### ✅ Already Have:
- README.md in each package folder
- Main README.md with overview
- GETTING_STARTED.md
- Sample documentation

#### ❌ Need to Create:
- PR to [microsoft/aspire.dev](https://github.com/microsoft/aspire.dev) for official docs
- Update main README.md in CommunityToolkit/Aspire repo to include our integration

### 5. NuGet Package Metadata

#### ✅ Already Have:
- `Description` property in csproj files
- Package tags

#### ⚠️ Need to Update:
- Add `client` or `hosting` tag to `AdditionalPackageTags`
- Ensure tags are space-separated

Example:
```xml
<PropertyGroup>
  <Description>An Aspire hosting integration for Quartz.NET background job scheduling.</Description>
  <AdditionalPackageTags>quartz background-jobs scheduling hosting</AdditionalPackageTags>
</PropertyGroup>
```

### 6. Container Image Requirements

#### ✅ Already Compliant:
- We use PostgreSQL with specific version tags (not `latest`)
- Database setup is handled by Aspire's built-in PostgreSQL integration

### 7. Code Quality

#### ⚠️ Need to Verify:
- All public APIs are documented with XML comments
- Code follows Aspire conventions
- No breaking changes to existing Aspire patterns

## 🎯 Action Plan

### Phase 1: Prepare Code Structure (Before PR)

1. **Fork CommunityToolkit/Aspire repository**
2. **Rename projects and namespaces:**
   - Rename all projects to use `CommunityToolkit.Aspire.*` prefix
   - Update all namespace references
   - Update all using statements
   - Update project references
3. **Move code to correct structure:**
   - Copy to `src/CommunityToolkit.Aspire.Hosting.Quartz/`
   - Copy to `src/CommunityToolkit.Aspire.Quartz/`
   - Copy to `src/CommunityToolkit.Aspire.Quartz.Abstractions/`
4. **Update package metadata:**
   - Add `hosting` or `client` tags
   - Verify descriptions
5. **Create unit tests:**
   - Test all public APIs
   - Test resource creation
   - Test configuration options
6. **Create integration tests:**
   - Test with example AppHost
   - Mark with `[RequiresDocker]`
   - Test end-to-end scenarios
7. **Update example application:**
   - Ensure it uses new naming
   - Verify it demonstrates minimal usage
8. **Update documentation:**
   - Update README files with new naming
   - Ensure NuGet package READMEs are clear

### Phase 2: Submit PR

1. **Create feature branch** in forked repo
2. **Commit all changes** with clear commit messages
3. **Run tests locally** to ensure everything works
4. **Update tests.yml** (run their script to generate test list)
5. **Submit PR** with detailed description:
   - What the integration does
   - Why it's useful for the community
   - Link to our original repository
   - Link to NuGet packages (proof of concept)
   - Screenshots/demos if applicable

### Phase 3: Documentation PR

1. **Wait for code PR approval**
2. **Create PR to microsoft/aspire.dev** for official docs
3. **Use their agent** to scaffold docs from README

### Phase 4: After Merge

1. **Deprecate our NuGet packages** (AspireQuartz.*)
2. **Update our README** to point to CommunityToolkit packages
3. **Archive our repository** or mark as "moved to CommunityToolkit"

## 📝 PR Description Template

```markdown
# Add Quartz.NET Integration for Background Job Scheduling

## Overview

This PR adds a production-ready integration for background job scheduling using Quartz.NET in .NET Aspire applications.

## What's Included

### Hosting Integration (`CommunityToolkit.Aspire.Hosting.Quartz`)
- Resource pattern for Quartz.NET
- Automatic database configuration (PostgreSQL, SQL Server, MySQL, SQLite)
- Health checks integration
- OpenTelemetry metrics

### Client Integration (`CommunityToolkit.Aspire.Quartz`)
- `IBackgroundJobClient` for dynamic job scheduling
- Idempotency support
- Retry policies with exponential/linear backoff
- OpenTelemetry distributed tracing
- Full Quartz.NET power (cron expressions, triggers, listeners)

### Abstractions (`CommunityToolkit.Aspire.Quartz.Abstractions`)
- Core interfaces and contracts
- Shared types between hosting and client

## Why This Integration?

.NET Aspire currently lacks native support for background job scheduling. Developers must manually integrate Quartz.NET or Hangfire, configure persistence, set up observability, and implement idempotency themselves.

This integration provides:
- ✅ Aspire-native patterns (not just a wrapper)
- ✅ Production features out of the box (idempotency, OpenTelemetry, health checks)
- ✅ Multi-database support with auto-migrations
- ✅ Full Quartz.NET power (no feature hiding)
- ✅ All-in-one architecture (no separate worker needed)

## Proof of Concept

This integration has been published as a proof of concept:
- NuGet: https://www.nuget.org/packages/AspireQuartz
- GitHub: https://github.com/alnuaimicoder/aspire-hosting-quartz
- Downloads: [X] total downloads
- Version: 1.0.1

## Testing

- ✅ Unit tests for all public APIs
- ✅ Integration tests with example AppHost
- ✅ Marked with `[RequiresDocker]` (uses PostgreSQL)
- ✅ Example application demonstrating usage

## Documentation

- ✅ README.md in each package
- ✅ Example application with comprehensive samples
- 🔄 Will create PR to aspire.dev after code review

## Breaking Changes

None - this is a new integration.

## Checklist

- [x] Code follows Aspire conventions
- [x] All public APIs documented with XML comments
- [x] Unit tests added
- [x] Integration tests added
- [x] Example application included
- [x] README.md files added
- [x] Package metadata configured
- [x] Namespaces follow guidelines
- [ ] Tests added to CI workflow
- [ ] Main README.md updated

## Related Issues

Closes #[issue_number_if_exists]
```

## 🤔 Questions to Consider

1. **Should we keep our original repository?**
   - Option A: Archive it and point to CommunityToolkit
   - Option B: Keep it as a "development" repo and sync changes

2. **What about existing NuGet packages?**
   - Deprecate AspireQuartz.* packages
   - Add deprecation notice pointing to CommunityToolkit packages

3. **Version numbering?**
   - Start from 1.0.0 in CommunityToolkit?
   - Or continue from 1.0.1?

4. **Maintenance?**
   - We'll be contributors to CommunityToolkit
   - Need to follow their release process

## 📅 Timeline Estimate

- **Phase 1 (Preparation)**: 2-3 days
  - Rename and restructure: 4-6 hours
  - Create tests: 8-12 hours
  - Update documentation: 2-4 hours

- **Phase 2 (PR Submission)**: 1 day
  - Final testing: 2-3 hours
  - PR creation and description: 1-2 hours

- **Phase 3 (Review Process)**: 1-2 weeks (depends on maintainers)

- **Phase 4 (Documentation)**: 2-3 days after code approval

**Total**: ~3-4 weeks from start to finish

## 🎉 Benefits of Contributing to Toolkit

1. **Wider Reach**: More developers will discover and use it
2. **Official Support**: Part of the official Aspire ecosystem
3. **Community Contributions**: Others can help improve it
4. **Aspire CLI Support**: Users can use `aspire add CommunityToolkit.Aspire.Quartz`
5. **Better Maintenance**: Shared responsibility with community
6. **Credibility**: Official integration badge

## ⚠️ Potential Challenges

1. **Code Review**: May need to make changes based on feedback
2. **Testing Requirements**: Need comprehensive test coverage
3. **Documentation**: Need to create official docs on aspire.dev
4. **Naming Changes**: All users will need to migrate package names
5. **Maintenance**: Need to follow their release process

## 🚀 Next Steps

1. **Decision**: Do we want to contribute to CommunityToolkit?
2. **If Yes**: Start Phase 1 (Preparation)
3. **If No**: Continue maintaining independently

---

**Recommendation**: I strongly recommend contributing to CommunityToolkit. The benefits far outweigh the effort, and it will help the entire .NET Aspire community.
