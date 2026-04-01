#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publish NuGet packages to GitHub Packages
.DESCRIPTION
    Publishes all NuGet packages from artifacts folder to GitHub Packages
.PARAMETER Version
    Package version to publish (default: 1.0.0)
.PARAMETER Owner
    GitHub repository owner (default: alnuaimicoder)
.PARAMETER Token
    GitHub Personal Access Token (PAT) with write:packages scope
.EXAMPLE
    .\publish-to-github.ps1 -Version "1.0.0" -Token "ghp_xxxxx"
#>

param(
    [string]$Version = "1.0.0",
    [string]$Owner = "alnuaimicoder",
    [string]$Token = $env:GITHUB_TOKEN
)

$ErrorActionPreference = "Stop"

Write-Host "📦 Publishing to GitHub Packages" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Gray
Write-Host "Owner: $Owner" -ForegroundColor Gray
Write-Host ""

# Check if token is provided
if ([string]::IsNullOrEmpty($Token)) {
    Write-Host "❌ Error: GitHub token not provided" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please provide a token using one of these methods:" -ForegroundColor Yellow
    Write-Host "1. Set environment variable: `$env:GITHUB_TOKEN = 'your-token'" -ForegroundColor Gray
    Write-Host "2. Pass as parameter: -Token 'your-token'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Token requirements:" -ForegroundColor Yellow
    Write-Host "- Classic token with 'write:packages' and 'read:packages' scopes" -ForegroundColor Gray
    Write-Host "- Or Fine-grained token with 'Contents: Read and write' permission" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Create token at: https://github.com/settings/tokens/new" -ForegroundColor Cyan
    exit 1
}

# Verify artifacts exist
$artifactsPath = Join-Path $PSScriptRoot ".." "artifacts"
if (-not (Test-Path $artifactsPath)) {
    Write-Host "❌ Error: artifacts folder not found" -ForegroundColor Red
    Write-Host "Run: .\scripts\local-ci.ps1 -Version $Version" -ForegroundColor Yellow
    exit 1
}

# Find packages
$packages = @(
    "CommunityToolkit.Aspire.Quartz.Abstractions.$Version.nupkg",
    "CommunityToolkit.Aspire.Quartz.$Version.nupkg",
    "CommunityToolkit.Aspire.Hosting.Quartz.$Version.nupkg"
)

Write-Host "Checking packages..." -ForegroundColor Cyan
$missingPackages = @()
foreach ($pkg in $packages) {
    $pkgPath = Join-Path $artifactsPath $pkg
    if (Test-Path $pkgPath) {
        Write-Host "✓ $pkg" -ForegroundColor Green
    } else {
        Write-Host "✗ $pkg (not found)" -ForegroundColor Red
        $missingPackages += $pkg
    }
}

if ($missingPackages.Count -gt 0) {
    Write-Host ""
    Write-Host "❌ Missing packages. Build them first:" -ForegroundColor Red
    Write-Host ".\scripts\local-ci.ps1 -Version $Version" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Configuring GitHub Packages source..." -ForegroundColor Cyan

# Remove existing source if present
$sourceName = "github-$Owner"
dotnet nuget list source | Select-String $sourceName | Out-Null
if ($LASTEXITCODE -eq 0) {
    dotnet nuget remove source $sourceName 2>$null | Out-Null
}

# Add GitHub Packages source
$sourceUrl = "https://nuget.pkg.github.com/$Owner/index.json"
dotnet nuget add source $sourceUrl `
    --name $sourceName `
    --username $Owner `
    --password $Token `
    --store-password-in-clear-text

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to configure source" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Source configured" -ForegroundColor Green
Write-Host ""

# Publish packages
Write-Host "Publishing packages..." -ForegroundColor Cyan
Write-Host ""

$publishedCount = 0
$failedCount = 0

foreach ($pkg in $packages) {
    $pkgPath = Join-Path $artifactsPath $pkg
    $pkgName = [System.IO.Path]::GetFileNameWithoutExtension($pkg)

    Write-Host "Publishing $pkg..." -ForegroundColor Yellow

    dotnet nuget push $pkgPath `
        --source $sourceName `
        --api-key $Token `
        --skip-duplicate 2>&1 | Tee-Object -Variable output

    if ($LASTEXITCODE -eq 0 -or $output -match "already exists") {
        Write-Host "✓ Published (or already exists)" -ForegroundColor Green
        $publishedCount++
    } else {
        Write-Host "✗ Failed" -ForegroundColor Red
        $failedCount++
    }
    Write-Host ""
}

# Cleanup - remove source
Write-Host "Cleaning up..." -ForegroundColor Cyan
dotnet nuget remove source $sourceName 2>$null | Out-Null

# Summary
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host ""
if ($failedCount -eq 0) {
    Write-Host "✅ GitHub Packages Publishing Complete!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Publishing completed with errors" -ForegroundColor Yellow
}
Write-Host ""
Write-Host "Published: $publishedCount" -ForegroundColor Green
Write-Host "Failed: $failedCount" -ForegroundColor $(if ($failedCount -gt 0) { "Red" } else { "Gray" })
Write-Host ""
Write-Host "View packages at:" -ForegroundColor Cyan
Write-Host "https://github.com/$Owner?tab=packages" -ForegroundColor Blue
Write-Host ""
Write-Host "To install from GitHub Packages:" -ForegroundColor Cyan
Write-Host "dotnet nuget add source https://nuget.pkg.github.com/$Owner/index.json --name github --username $Owner --password YOUR_TOKEN --store-password-in-clear-text" -ForegroundColor Gray
Write-Host "dotnet add package CommunityToolkit.Aspire.Hosting.Quartz --version $Version --source github" -ForegroundColor Gray
Write-Host ""

exit $(if ($failedCount -gt 0) { 1 } else { 0 })
