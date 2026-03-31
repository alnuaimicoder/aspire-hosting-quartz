#!/usr/bin/env pwsh
# Quick Check - فحص سريع قبل الكوميت

$ErrorActionPreference = "Stop"

Write-Host "⚡ Quick Check" -ForegroundColor Cyan
Write-Host ""

$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $projectRoot

# 1. فحص البناء
Write-Host "1️⃣  Building..." -ForegroundColor Yellow
dotnet build -c Release --no-restore -v quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "   ✓ Build OK" -ForegroundColor Green

# 2. فحص التنسيق
Write-Host "2️⃣  Checking format..." -ForegroundColor Yellow
dotnet format --verify-no-changes --verbosity quiet 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Format OK" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Format issues (run: dotnet format)" -ForegroundColor Yellow
}

# 3. عد الملفات المعدلة
Write-Host "3️⃣  Checking git status..." -ForegroundColor Yellow
$modifiedFiles = (git status --short | Measure-Object).Count
if ($modifiedFiles -gt 0) {
    Write-Host "   ⚠️  $modifiedFiles file(s) modified" -ForegroundColor Yellow
    git status --short
} else {
    Write-Host "   ✓ No changes" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Quick check done!" -ForegroundColor Green
