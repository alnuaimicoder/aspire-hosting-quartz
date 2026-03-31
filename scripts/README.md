# Development Scripts

This directory contains PowerShell scripts for local development and CI workflows.

## Available Scripts

### `local-ci.ps1` - Local CI Pipeline

Runs the complete CI pipeline locally for testing before pushing changes.

**Usage:**
```powershell
# Run full CI (restore, build, test, pack)
.\scripts\local-ci.ps1

# Specify version
.\scripts\local-ci.ps1 -Version "1.0.0"

# Skip tests (faster builds)
.\scripts\local-ci.ps1 -SkipTests

# Skip packaging
.\scripts\local-ci.ps1 -SkipPack
```

**What it does:**
1. Restores NuGet packages
2. Builds all projects
3. Runs tests (optional)
4. Creates NuGet packages (optional)

**Output:**
- Build artifacts in `artifacts/` folder
- Test results in console
- Exit code 0 on success, 1 on failure

---

### `quick-check.ps1` - Fast Pre-Commit Checks

Quick validation before committing code.

**Usage:**
```powershell
.\scripts\quick-check.ps1
```

**What it does:**
1. Restores packages
2. Builds solution
3. Runs basic validation

**Time:** ~30 seconds

---

## Common Workflows

### Before Committing
```powershell
.\scripts\quick-check.ps1
```

### Before Creating PR
```powershell
.\scripts\local-ci.ps1
```

### Testing Package Build
```powershell
.\scripts\local-ci.ps1 -Version "1.0.0"
```

---

## Troubleshooting

### "Cannot find path" errors
Make sure you're running scripts from the repository root:
```powershell
cd path/to/aspire-hosting-quartz
.\scripts\local-ci.ps1
```

### Tests failing
Skip tests during development:
```powershell
.\scripts\local-ci.ps1 -SkipTests
```

---

## CI/CD Integration

These scripts work both locally and in CI/CD pipelines:

### GitHub Actions
```yaml
- name: Run CI
  run: ./scripts/local-ci.ps1 -Version ${{ github.ref_name }}
  shell: pwsh
```

---

## Script Parameters

### local-ci.ps1
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Version | string | "1.0.0" | Package version |
| SkipTests | switch | false | Skip test execution |
| SkipPack | switch | false | Skip package creation |

### quick-check.ps1
No parameters.

---

## Contributing

When adding new scripts:
1. Follow PowerShell best practices
2. Add parameter validation
3. Include error handling
4. Update this README
5. Test on Windows and Linux (PowerShell Core)

---

**Last Updated:** March 31, 2026
