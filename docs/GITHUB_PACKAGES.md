# Publishing to GitHub Packages

This guide explains how to publish packages to GitHub Packages.

## Prerequisites

### 1. Create GitHub Personal Access Token (PAT)

You need a token with package permissions:

**Option A: Classic Token (Recommended)**
1. Go to: https://github.com/settings/tokens/new
2. Token name: `NuGet Package Publishing`
3. Expiration: Choose duration
4. Select scopes:
   - ✅ `write:packages` - Upload packages
   - ✅ `read:packages` - Download packages
   - ✅ `delete:packages` - Delete packages (optional)
5. Click "Generate token"
6. **Copy the token immediately** (you won't see it again!)

**Option B: Fine-grained Token**
1. Go to: https://github.com/settings/personal-access-tokens/new
2. Token name: `NuGet Package Publishing`
3. Repository access: Select `aspire-hosting-quartz`
4. Permissions:
   - Repository permissions → Contents: `Read and write`
5. Click "Generate token"
6. **Copy the token immediately**

### 2. Store Token Securely

**Option 1: Environment Variable (Recommended)**
```powershell
# Windows PowerShell (current session)
$env:GITHUB_TOKEN = "ghp_your_token_here"

# Windows PowerShell (permanent)
[System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', 'ghp_your_token_here', 'User')

# Bash/Linux
export GITHUB_TOKEN="ghp_your_token_here"
```

**Option 2: Pass as Parameter**
```powershell
.\scripts\publish-to-github.ps1 -Token "ghp_your_token_here"
```

## Publishing Packages

### Step 1: Build Packages

```powershell
# Build all packages
.\scripts\local-ci.ps1 -Version "1.0.0"
```

### Step 2: Publish to GitHub Packages

```powershell
# Using environment variable
$env:GITHUB_TOKEN = "ghp_your_token_here"
.\scripts\publish-to-github.ps1 -Version "1.0.0"

# Or pass token directly
.\scripts\publish-to-github.ps1 -Version "1.0.0" -Token "ghp_your_token_here"
```

### Step 3: Verify Publication

Go to: https://github.com/alnuaimicoder?tab=packages

You should see:
- CommunityToolkit.Aspire.Quartz.Abstractions
- CommunityToolkit.Aspire.Quartz
- CommunityToolkit.Aspire.Hosting.Quartz

## Installing from GitHub Packages

### For Users

**Step 1: Configure Source**
```bash
dotnet nuget add source https://nuget.pkg.github.com/alnuaimicoder/index.json \
  --name github \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text
```

**Step 2: Install Package**
```bash
dotnet add package CommunityToolkit.Aspire.Hosting.Quartz --version 1.0.0 --source github
```

### Using nuget.config

Create `nuget.config` in your project:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/alnuaimicoder/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_TOKEN" />
    </github>
  </packageSourceCredentials>
</configuration>
```

**⚠️ Important**: Add `nuget.config` to `.gitignore` to avoid committing tokens!

## Troubleshooting

### Error: 401 Unauthorized

**Cause**: Invalid or expired token, or insufficient permissions.

**Solution**:
1. Verify token has `write:packages` scope
2. Check token hasn't expired
3. Regenerate token if needed
4. Ensure token is for the correct account

### Error: 409 Conflict

**Cause**: Package version already exists.

**Solution**:
- GitHub Packages doesn't allow overwriting versions
- Increment version number
- Or delete the existing package version first

### Error: 403 Forbidden

**Cause**: Token doesn't have permission for this repository.

**Solution**:
1. Ensure you're the repository owner or have admin access
2. For fine-grained tokens, ensure repository is selected
3. Check token permissions include package write access

### Packages Not Showing Up

**Wait Time**: It may take a few minutes for packages to appear.

**Check**:
1. Go to: https://github.com/alnuaimicoder?tab=packages
2. Refresh the page
3. Check package visibility settings

## Package Visibility

By default, packages inherit repository visibility:
- Private repo → Private packages
- Public repo → Public packages

### Making Packages Public

1. Go to package page
2. Click "Package settings"
3. Scroll to "Danger Zone"
4. Click "Change visibility"
5. Select "Public"

## Best Practices

### 1. Use Environment Variables
Never hardcode tokens in scripts or commit them to git.

### 2. Token Rotation
Rotate tokens regularly (every 90 days recommended).

### 3. Minimal Permissions
Use tokens with only required permissions.

### 4. Separate Tokens
Use different tokens for different purposes:
- One for CI/CD
- One for local development
- One for package management

### 5. Monitor Usage
Check package downloads and usage regularly.

## Comparison: NuGet.org vs GitHub Packages

| Feature | NuGet.org | GitHub Packages |
|---------|-----------|-----------------|
| Public packages | ✅ Free | ✅ Free |
| Private packages | ❌ No | ✅ Free |
| Storage limit | Unlimited | 500 MB free |
| Bandwidth | Unlimited | 1 GB/month free |
| Authentication | API key | GitHub token |
| Discoverability | High | Lower |
| Best for | Public libraries | Private/internal packages |

## Recommendations

### For Open Source Projects
- **Primary**: Publish to NuGet.org (better discoverability)
- **Secondary**: Publish to GitHub Packages (backup/preview builds)

### For Private Projects
- **Primary**: GitHub Packages (free for private repos)
- **Alternative**: Azure Artifacts, MyGet

## Additional Resources

- [GitHub Packages Documentation](https://docs.github.com/en/packages)
- [NuGet Package Publishing](https://learn.microsoft.com/nuget/nuget-org/publish-a-package)
- [Managing PATs](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens)

---

**Last Updated**: March 31, 2026
