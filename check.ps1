$ErrorActionPreference = "Stop"

Write-Host "Verifying formatting..." -ForegroundColor Black -BackgroundColor Yellow
dotnet format CosmosCasino.sln

Write-Host "Building (Debug) (Style analyzer)..." -ForegroundColor Black -BackgroundColor Yellow
dotnet build CosmosCasino.sln -c Debug -warnaserror:SA

Write-Host "Building (Release) (Style analyzer)..." -ForegroundColor Black -BackgroundColor Yellow
dotnet build CosmosCasino.sln -c Release -warnaserror:SA

Write-Host "Running tests (Debug)..." -ForegroundColor Black -BackgroundColor Yellow
dotnet test CosmosCasino.sln -c Debug --no-build

Write-Host "Running tests (Release)..." -ForegroundColor Black -BackgroundColor Yellow
dotnet test CosmosCasino.sln -c Release --no-build

Write-Host "All checks passed." -ForegroundColor Black -BackgroundColor Green