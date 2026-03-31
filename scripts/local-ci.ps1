#!/usr/bin/env pwsh
# Local CI Script - Run before pushing to GitHub
# يشغل نفس خطوات GitHub Actions محليًا

param(
    [string]$Version = "1.0.0",
    [switch]$SkipTests,
    [switch]$SkipPack
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting Local CI for CommunityToolkit.Aspire.Quartz" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Yellow
Write-Host ""

# التأكد من وجود .NET SDK
Write-Host "✓ Checking .NET SDK..." -ForegroundColor Green
try {
    $dotnetVersion = dotnet --version
    Write-Host "  .NET SDK: $dotnetVersion" -ForegroundColor Gray
} catch {
    Write-Host "✗ .NET SDK not found! Install from https://dot.net" -ForegroundColor Red
    exit 1
}

# الانتقال لمجلد المشروع
$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $projectRoot

Write-Host ""
Write-Host "📦 Step 1: Restore Dependencies" -ForegroundColor Cyan
Write-Host "─────────────────────────────────" -ForegroundColor Gray
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Restore failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Restore completed" -ForegroundColor Green

Write-Host ""
Write-Host "🔨 Step 2: Build (Release)" -ForegroundColor Cyan
Write-Host "─────────────────────────────────" -ForegroundColor Gray
dotnet build -c Release --no-restore /p:Version=$Version
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build completed" -ForegroundColor Green

if (-not $SkipTests) {
    Write-Host ""
    Write-Host "🧪 Step 3: Run Tests" -ForegroundColor Cyan
    Write-Host "─────────────────────────────────" -ForegroundColor Gray
    dotnet test -c Release --no-build --verbosity normal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Tests failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Tests passed" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "⏭️  Step 3: Tests skipped" -ForegroundColor Yellow
}

if (-not $SkipPack) {
    Write-Host ""
    Write-Host "📦 Step 4: Pack NuGet Packages" -ForegroundColor Cyan
    Write-Host "─────────────────────────────────" -ForegroundColor Gray

    # حذف artifacts القديمة
    if (Test-Path "./artifacts") {
        Remove-Item -Recurse -Force "./artifacts"
    }

    dotnet pack -c Release --no-build -o ./artifacts /p:Version=$Version
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Pack failed!" -ForegroundColor Red
        exit 1
    }

    Write-Host ""
    Write-Host "✓ Packages created:" -ForegroundColor Green
    Get-ChildItem ./artifacts/*.nupkg | ForEach-Object {
        $size = [math]::Round($_.Length / 1KB, 2)
        Write-Host "  • $($_.Name) ($size KB)" -ForegroundColor Gray
    }
} else {
    Write-Host ""
    Write-Host "⏭️  Step 4: Pack skipped" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✅ Local CI Completed Successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review artifacts in ./artifacts/" -ForegroundColor Gray
Write-Host "  2. Commit changes: git add -A && git commit -m 'message'" -ForegroundColor Gray
Write-Host "  3. Push changes: git push" -ForegroundColor Gray
Write-Host "  4. Create tag: git tag -a v$Version -m 'Release v$Version'" -ForegroundColor Gray
Write-Host "  5. Push tag: git push origin v$Version" -ForegroundColor Gray
Write-Host ""
