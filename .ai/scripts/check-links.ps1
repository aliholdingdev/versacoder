<#
.SYNOPSIS
    .ai vault ic link butunlugu taramasi (GATE-004a kaniti).
.DESCRIPTION
    Her .md dosyasindaki goreli ](hedef) linklerini dogrular.
    Kod bloglari (``` ... ```) ve satir ici kod (`...`) icindeki
    ornekler link sayilmaz - sahte alarm uretmez.
    Cikti: toplam / iyi / bozuk + hedef bazli gruplama.
.EXAMPLE
    pwsh -File .ai/scripts/check-links.ps1
    pwsh -File .ai/scripts/check-links.ps1 -FailOnBroken   # CI: bozuk link varsa exit 1
.NOTES
    Sahip: architect · Guncelleme: 2026-09-25 · authority: SSOT
    Bkz. ../.decisions/index.md (ADR-003 link = 0 bozuk link)
#>
[CmdletBinding()]
param(
    [string]$Root = '',
    [switch]$FailOnBroken
)

$ErrorActionPreference = 'Stop'

# PS 5.1'de param() varsayilaninda $PSScriptRoot bos kalabilir -> calisma zamaninda belirle
if ([string]::IsNullOrWhiteSpace($Root)) {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path   # .ai\scripts
    $Root      = Split-Path -Parent $scriptDir                     # .ai
}

function Get-LinkTargets {
    param([string]$Text)

    # 1) kod bloglarini ve satir ici kodu temizle (link degillerdir)
    $work = [regex]::Replace($Text, '(?ms)^```.*?^```', ' ')
    $work = [regex]::Replace($work, '`[^`\r\n]*`', ' ')

    foreach ($m in [regex]::Matches($work, '\]\(([^)\s]+)\)')) {
        $target = $m.Groups[1].Value.Trim()
        if ($target -match '^(https?:|mailto:|ftp:|file:|vscode:|#)') { continue }
        $target = ($target -split '#')[0]
        if ([string]::IsNullOrWhiteSpace($target)) { continue }
        $target = [Uri]::UnescapeDataString($target)
        $target = $target.Trim('"', "'")
        if ([string]::IsNullOrWhiteSpace($target)) { continue }
        $target          # PowerShell: cikan degerler toplanir (yield yerine)
    }
}

$rootFull = (Resolve-Path -LiteralPath $Root).Path
$files = Get-ChildItem -LiteralPath $rootFull -Recurse -Filter *.md -File |
         Where-Object { $_.FullName -notmatch '\\\.obsidian\\' }

$ok = 0
$broken = New-Object System.Collections.Generic.List[object]

foreach ($f in $files) {
    $text = [IO.File]::ReadAllText($f.FullName)
    $dir = Split-Path -Parent $f.FullName
    $relFile = $f.FullName.Substring($rootFull.Length + 1)

    foreach ($target in Get-LinkTargets -Text $text) {
        $isAbs = [regex]::IsMatch($target, '^[a-zA-Z]:[\\/]')
        $resolved = if ($isAbs) { $target } else { Join-Path $dir $target }

        if (Test-Path -LiteralPath $resolved) {
            $ok++
        } else {
            $broken.Add([pscustomobject]@{ File = $relFile; Target = $target })
        }
    }
}

$mdCount = $files.Count
Write-Host ''
Write-Host '==========================================================' -ForegroundColor Cyan
Write-Host ('  .ai VAULT LINK TARAMASI  (GATE-004a)') -ForegroundColor Cyan
Write-Host '==========================================================' -ForegroundColor Cyan
Write-Host ("  Taranan MD      : {0}" -f $mdCount)
Write-Host ("  Iyi link        : {0}" -f $ok)
Write-Host ("  BOZUK link      : {0}" -f $broken.Count) -ForegroundColor $(if ($broken.Count -eq 0) { 'Green' } else { 'Red' })
Write-Host '==========================================================' -ForegroundColor Cyan

if ($broken.Count -gt 0) {
    Write-Host ''
    Write-Host '  HEDEF BAZLI GRUPLAMA (azalan):' -ForegroundColor Yellow
    $broken | Group-Object Target | Sort-Object Count -Descending |
        ForEach-Object { Write-Host ('    {0,3}x  {1}' -f $_.Count, $_.Name) }

    Write-Host ''
    Write-Host '  DOSYA BAZLI (ilk 40):' -ForegroundColor Yellow
    $broken | Select-Object -First 40 |
        ForEach-Object { Write-Host ('    {0}  ->  {1}' -f $_.File, $_.Target) }
    Write-Host ''
}

if ($FailOnBroken -and $broken.Count -gt 0) { exit 1 }
exit 0
