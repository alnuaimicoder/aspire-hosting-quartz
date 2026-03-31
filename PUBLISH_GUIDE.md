# Publishing Guide

This guide explains how to publish releases of CommunityToolkit.Aspire.Quartz packages to NuGet.org.

## 🤖 Automated Release (Recommended)

The repository now has an automated release workflow that handles everything for you!

### Prerequisites

1. ✅ NuGet API key added to GitHub Secrets (see SETUP_SECRETS.md)
2. ✅ All changes committed and pushed to main branch

### Release Process

**It's as simple as creating a tag!**

```bash
# 1. Ensure you're on main and up to date
git checkout main
git pull

# 2. Create a version tag
git tag -a v1.0.0 -m "Release v1.0.0 - First stable release"

# 3. Push the tag
git push origin v1.0.0
```

**That's it!** The GitHub Actions workflow will automatically:
- ✅ Build all projects (.NET 8.0 and 9.0)
- ✅ Run all tests
- ✅ Pack NuGet packages
- ✅ Publish to NuGet.org
- ✅ Create GitHub release with packages attached
- ✅ Generate release notes

### Monitor the Release

Watch the workflow progress:
https://github.com/alnuaimicoder/aspire-hosting-quartz/actions

### Verify Publication

After 5-10 minutes:
1. Check NuGet.org:
   - https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz
   - https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Quartz
   - https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz.Abstractions

2. Check GitHub Releases:
   - https://github.com/alnuaimicoder/aspire-hosting-quartz/releases

---

## 🔧 Manual Release (Alternative)

If you prefer manual control or the automated workflow fails:

### Prerequisites

1. NuGet.org account - Create one at https://www.nuget.org/
2. API Key from NuGet.org:
   - Go to https://www.nuget.org/account/apikeys
   - Click "Create"
   - Name: "CommunityToolkit.Aspire.Quartz"
   - Glob Pattern: `CommunityToolkit.Aspire.*`
   - Select scopes: Push new packages and package versions
   - Copy the API key (you'll only see it once!)

## Step 1: Verify Packages Locally

The packages are already built in the `artifacts/` folder:

```bash
ls artifacts/
```

You should see:
- CommunityToolkit.Aspire.Quartz.Abstractions.1.0.0.nupkg
- CommunityToolkit.Aspire.Quartz.1.0.0.nupkg
- CommunityToolkit.Aspire.Hosting.Quartz.1.0.0.nupkg
- Symbol packages (.snupkg) for each

## Step 2: Test Packages Locally (Optional but Recommended)

```bash
# Create a local NuGet source
dotnet nuget add source ./artifacts -n local-test

# Create a test project
cd ..
dotnet new aspire-starter -n TestAspireQuartz
cd TestAspireQuartz/TestAspireQuartz.AppHost

# Install from local source
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz --source local-test --version 1.0.0

# Verify it works
dotnet build
```

## Step 3: Create GitHub Release

```bash
cd aspire-hosting-quartz

# Commit all changes
git add -A
git commit -m "Release v1.0.0 - First stable release"
git push

# Create and push tag
git tag -a v1.0.0 -m "Release v1.0.0

First stable release of CommunityToolkit.Aspire.Quartz

Features:
- Background job scheduling with Quartz.NET
- SQL Server and PostgreSQL support
- Automatic database migrations
- Idempotency support
- Retry policies with exponential/linear backoff
- OpenTelemetry distributed tracing
- Aspire Dashboard integration
- Multi-targeting (.NET 8.0 and 9.0)
"

git push origin v1.0.0
```

## Step 4: Create GitHub Release (Web UI)

1. Go to https://github.com/alnuaimicoder/aspire-hosting-quartz/releases
2. Click "Create a new release"
3. Choose tag: v1.0.0
4. Release title: "v1.0.0 - First Stable Release"
5. Copy description from CHANGELOG.md
6. Attach the .nupkg files from artifacts/ folder
7. Click "Publish release"

## Step 5: Publish to NuGet.org

```bash
# Set your API key (replace YOUR_API_KEY with actual key)
$env:NUGET_API_KEY="YOUR_API_KEY"

# Publish all three packages
dotnet nuget push artifacts/CommunityToolkit.Aspire.Quartz.Abstractions.1.0.0.nupkg `
  --api-key $env:NUGET_API_KEY `
  --source https://api.nuget.org/v3/index.json

dotnet nuget push artifacts/CommunityToolkit.Aspire.Quartz.1.0.0.nupkg `
  --api-key $env:NUGET_API_KEY `
  --source https://api.nuget.org/v3/index.json

dotnet nuget push artifacts/CommunityToolkit.Aspire.Hosting.Quartz.1.0.0.nupkg `
  --api-key $env:NUGET_API_KEY `
  --source https://api.nuget.org/v3/index.json
```

## Step 6: Verify Publication

1. Wait 5-10 minutes for indexing
2. Visit https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz
3. Visit https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.Quartz
4. Visit https://www.nuget.org/packages/CommunityToolkit.Aspire.Quartz.Abstractions

## Step 7: Test Installation from NuGet

```bash
# Create a new test project
dotnet new aspire-starter -n TestFromNuGet
cd TestFromNuGet/TestFromNuGet.AppHost

# Install from NuGet.org
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz

# Verify it works
dotnet build
```

---

## 🎉 Post-Release Steps

### 1. Verify Packages

Test installation from NuGet:
```bash
dotnet new aspire-starter -n TestFromNuGet
cd TestFromNuGet/TestFromNuGet.AppHost
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz
dotnet build
```

### 2. Update Documentation

- Update README.md badges (version numbers)
- Update CHANGELOG.md for next version
- Update PROJECT_STATUS.md

### 3. Announce the Release

**Social Media:**
- Twitter/X: "🎉 Just released CommunityToolkit.Aspire.Quartz v1.0.0! Background job scheduling for .NET Aspire with Quartz.NET. Features: SQL Server/PostgreSQL, retry policies, OpenTelemetry, and more! #dotnet #aspire #csharp"
- LinkedIn: Similar announcement with more details
- Reddit r/dotnet: Create a detailed post

**Community:**
- Aspire Discord
- .NET Foundation Slack
- Dev.to blog post

### 4. Monitor and Respond

- Watch GitHub issues
- Respond to questions
- Collect feedback for v1.1.0

---

## 🔄 Release Checklist

Before creating a release tag:

- [ ] All tests pass locally
- [ ] CHANGELOG.md updated
- [ ] Version number decided
- [ ] Breaking changes documented (if any)
- [ ] Sample projects work
- [ ] Documentation up to date

---

## 📊 Version Numbering

Follow Semantic Versioning (SemVer 2.0):

- **Major** (1.0.0 → 2.0.0): Breaking changes
- **Minor** (1.0.0 → 1.1.0): New features, backward compatible
- **Patch** (1.0.0 → 1.0.1): Bug fixes, backward compatible

---

## 🚨 Troubleshooting

### Workflow fails with "401 Unauthorized"
- Check NUGET_API_KEY secret is set correctly
- Verify API key is valid on NuGet.org
- Ensure key has "Push" permissions

### Package already exists error
- You can't republish the same version
- Increment version and create new tag
- Use `--skip-duplicate` flag (already in workflow)

### Tests fail in workflow
- Run tests locally first: `dotnet test`
- Check workflow logs for details
- Fix issues and push changes before tagging

### Tag already exists
```bash
# Delete local tag
git tag -d v1.0.0

# Delete remote tag
git push origin :refs/tags/v1.0.0

# Create new tag
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0
```

---

## 🔒 Security Notes

Never commit your NuGet API key to Git! Use environment variables or secure key storage.

---

**Congratulations on your first release! 🎉**
