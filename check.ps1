Clear-Host
$ErrorActionPreference = "Stop"

$Script:HasFailures = $false

# ============================================================
# Utility: Visual Divider
# ============================================================

function Write-Divider {
    param ([int]$FallbackWidth = 80)

    try {
        $width = $Host.UI.RawUI.WindowSize.Width
    }
    catch {
        $width = $FallbackWidth
    }

    Write-Host ""
    Write-Host ("=" * $width) -ForegroundColor DarkGray
    Write-Host ""
}

# ============================================================
# Utility: Run step with status
# ============================================================

function Run-Step {
    param (
        [string]$Name,
        [ScriptBlock]$Action
    )

    Write-Host ">> $Name" -ForegroundColor Black -BackgroundColor Cyan
    Write-Host ""

    try {
        & $Action

        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code $LASTEXITCODE"
        }

        Write-Host ""
        Write-Host "$Name succeeded" -ForegroundColor Black -BackgroundColor Green
    }
    catch {
        $Script:HasFailures = $true

        Write-Host ""
        Write-Host "$Name failed" -ForegroundColor Black -BackgroundColor Red
    }

    Write-Divider
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

# ============================================================
# Global code statistics (code vs tests)
# ============================================================

# Collect all C# files (excluding bin/obj)
$allFiles = Get-ChildItem $PSScriptRoot -Recurse -Filter *.cs |
    Where-Object {
        $_.FullName -notmatch '\\bin\\' -and
        $_.FullName -notmatch '\\obj\\'
    }

# Adjust this if your test folder name changes
$testFiles = $allFiles |
    Where-Object {
        $_.FullName -match '\\code\\Tests\\'
    }

$codeFiles = $allFiles |
    Where-Object {
        $_.FullName -notmatch '\\code\\Tests\\'
    }

function Count-CodeLines {
    param ($Files)

    if ($Files.Count -eq 0) {
        return @{ Lines = 0 }
    }

    $Files |
        Get-Content |
        Where-Object {
            $_ -notmatch '^\s*$' -and      # empty lines
            $_ -notmatch '^\s*//' -and     # single-line comments
            $_ -notmatch '^\s*/\*' -and    # block comment start
            $_ -notmatch '^\s*\*' -and     # block comment body
            $_ -notmatch '^\s*\*/'         # block comment end
        } |
        Measure-Object -Line
}

# Total lines (everything)
$total = $allFiles |
    Get-Content |
    Measure-Object -Line

# Code lines (production)
$code = Count-CodeLines $codeFiles

# Test lines
$tests = Count-CodeLines $testFiles

# Comments / other
$comment = $total.Lines - ($code.Lines + $tests.Lines)

$percent = if ($total.Lines -gt 0) {
    [math]::Round(($comment / $total.Lines) * 100, 2)
} else {
    0
}

Write-Host ">> Code statistics" -ForegroundColor Black -BackgroundColor Cyan
Write-Host ""
Write-Host "Total lines      : $($total.Lines)"
Write-Host "Code lines       : $($code.Lines)"
Write-Host "Tests lines      : $($tests.Lines)"
Write-Host "Comments/Other   : $comment"
Write-Host "Comment %        : $percent%"

Write-Divider

# ============================================================
# Conclusion
# ============================================================

if ($Script:HasFailures) {
    Write-Host "One or more checks failed." -ForegroundColor Black -BackgroundColor Red
    Write-Host ""
    exit 1
}
else {
    Write-Host "All checks passed." -ForegroundColor Black -BackgroundColor Green
    Write-Host ""
    exit 0
}