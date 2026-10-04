<#
.SYNOPSIS
    Mojibake (broken encoding) scanner - lists MD files with mojibake sequences.
    ASCII-only source: PowerShell 5.1 reads .ps1 as ANSI, so no non-ASCII allowed.
.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .ai\scripts\find-mojibake.ps1
#>
[CmdletBinding()]
param([string]$Root = 'C:\versacoder\.ai')

$ErrorActionPreference = 'Stop'

# Same rule as measure-vault.ps1 section 5:
# sequences that appear when UTF-8 text was decoded as Latin-1 and re-encoded.
$pat = [regex]"(?:\u00C3\u00A4|\u00C3\u00BC|\u00C3\u00B6|\u00E2\u20AC|\u00EF\u00BB\u00BF|\uFFFD)"

$files = Get-ChildItem -LiteralPath $Root -Recurse -Filter *.md -File |
         Where-Object { $_.FullName -notmatch '\\\.obsidian\\' }

$bad = New-Object System.Collections.Generic.List[string]
foreach ($f in $files) {
    $raw = [IO.File]::ReadAllText($f.FullName, [Text.Encoding]::UTF8)
    if ($pat.IsMatch($raw)) { $bad.Add($f.FullName) }
}

Write-Host ("Mojibake file count: " + $bad.Count)
foreach ($b in $bad) {
    Write-Host ("  " + $b.Substring($Root.Length + 1))
}
