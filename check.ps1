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

# ============================================================
# Verification pipeline
# ============================================================

Run-Step "Formatting verification" {
    dotnet format CosmosCasino.sln
}

Run-Step "Build & Style Analysis (Debug)" {
    dotnet build CosmosCasino.sln -c Debug -warnaserror:SA
}

Run-Step "Build & Style Analysis (Release)" {
    dotnet build CosmosCasino.sln -c Release -warnaserror:SA
}

Run-Step "Run tests (Debug)" {
    dotnet test CosmosCasino.sln -c Debug --no-build
}

Run-Step "Run tests (Release)" {
    dotnet test CosmosCasino.sln -c Release --no-build
}

Write-Host "All checks passed." -ForegroundColor Black -BackgroundColor Green
Write-Host ""

# ============================================================
# Line count metrics (excluding bin/obj)
# ============================================================

$files = Get-ChildItem -Recurse -Filter *.cs |
    Where-Object {
        $_.FullName -notmatch '\\bin\\' -and
        $_.FullName -notmatch '\\obj\\'
    }

$all = $files |
    Get-Content |
    Measure-Object -Line

$code = $files |
    Get-Content |
    Where-Object {
        $_ -notmatch '^\s*$' -and      # empty lines
        $_ -notmatch '^\s*//' -and     # single-line comments
        $_ -notmatch '^\s*/\*' -and    # block comment start
        $_ -notmatch '^\s*\*' -and     # block comment body
        $_ -notmatch '^\s*\*/'         # block comment end
    } |
    Measure-Object -Line

$comment = $all.Lines - $code.Lines
$percent = if ($all.Lines -gt 0) {
    [math]::Round(($comment / $all.Lines) * 100, 2)
} else {
    0
}

Write-Host "Code statistics" -ForegroundColor Black -BackgroundColor Cyan
Write-Host "Total lines      : $($all.Lines)"
Write-Host "Code lines       : $($code.Lines)"
Write-Host "Comments/Other   : $comment"
Write-Host "Comment %        : $percent%"
