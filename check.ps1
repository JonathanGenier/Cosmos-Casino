Clear-Host
$ErrorActionPreference = "Stop"

function Run-Step {
    param (
        [string]$Name,
        [ScriptBlock]$Action
    )

    Write-Host ">> $Name" -ForegroundColor Black -BackgroundColor Yellow

    try {
        & $Action
        Write-Host "$Name succeeded" -ForegroundColor Black -BackgroundColor Green
    }
    catch {
        Write-Host "$Name failed" -ForegroundColor Black -BackgroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Black -BackgroundColor Yellow
        exit 1
    }

    Write-Host ""
}

Run-Step "Formatting verification" {
    dotnet format CosmosCasino.sln
}

Run-Step "Building & Analyzing Style (Debug)" {
    dotnet build CosmosCasino.sln -c Debug -warnaserror:SA
}

Run-Step "Building & Analyzing Style (Release)" {
    dotnet build CosmosCasino.sln -c Release -warnaserror:SA
}

Run-Step "Running tests (Debug)" {
    dotnet test CosmosCasino.sln -c Debug --no-build
}

Run-Step "Running tests (Release)" {
    dotnet test CosmosCasino.sln -c Release --no-build
}

Write-Host "All checks passed." -ForegroundColor Black -BackgroundColor Green
