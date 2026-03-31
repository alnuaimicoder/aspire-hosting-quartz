# GitHub Actions Workflows

This directory contains automated workflows for CI/CD.

## 🔒 Fork Protection

All workflows are configured to **only run in the original repository** (`alnuaimicoder/aspire-hosting-quartz`).

This prevents:
- Billing issues in forked repositories
- Unnecessary workflow runs in contributor forks
- Failed releases from forks

## 📋 Available Workflows

### CI Workflow (`ci.yml`)
- **Trigger**: Push to `main` or Pull Requests
- **Purpose**: Build, test, and validate code
- **Runs on**: .NET 8.0 and 9.0
- **Fork behavior**: Automatically skipped in forks

### Release Workflow (`release.yml`)
- **Trigger**: Git tags (`v*.*.*`) or manual dispatch
- **Purpose**: Build, pack, and publish NuGet packages
- **Fork behavior**: Automatically skipped in forks

## 🤝 For Contributors

If you've forked this repository:

1. **Workflows are disabled by default** - This is intentional to avoid billing issues
2. **You don't need to enable them** - CI/CD will run when you submit a Pull Request to the original repository
3. **Test locally instead** - Use the scripts in `/scripts` folder:
   ```bash
   # Quick validation
   .\scripts\quick-check.ps1

   # Full CI locally
   .\scripts\local-ci.ps1
   ```

## 🔧 Local Testing

Instead of relying on GitHub Actions in your fork, test locally:

```bash
# Restore packages
dotnet restore

# Build
dotnet build -c Release

# Run tests
dotnet test -c Release

# Pack packages
dotnet pack -c Release -o ./artifacts
```

## ❓ Questions?

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for more information about the contribution workflow.
