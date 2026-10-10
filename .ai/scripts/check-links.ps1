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

    # 2) goreli markdown linkleri: ](hedef) -> "M|hedef" (dosya dizinine goreli)
    foreach ($m in [regex]::Matches($work, '\]\(([^)\s]+)\)')) {
        $target = $m.Groups[1].Value.Trim()
        if ($target -match '^(https?:|mailto:|ftp:|file:|vscode:|#)') { continue }
        $target = ($target -split '#')[0]
        if ([string]::IsNullOrWhiteSpace($target)) { continue }
        $target = [Uri]::UnescapeDataString($target)
        $target = $target.Trim('"', "'")
        if ([string]::IsNullOrWhiteSpace($target)) { continue }
        "M|$target"      # PowerShell: cikan degerler toplanir (yield yerine)
    }

    # 3) Obsidian wikilinkleri: [[yol]] / [[yol|metin]] / [[yol#anchor]]
    #    -> "W|yol" (vault ROOT'una goreli; Uzanti yoksa .md eklenir) - D50
    foreach ($m in [regex]::Matches($work, '\[\[([^\[\]|]+)(?:#[^\[\]|]*)?(?:\|[^\[\]]*)?\]\]')) {
        $target = $m.Groups[1].Value.Trim()
        if ([string]::IsNullOrWhiteSpace($target)) { continue }
        $target = [Uri]::UnescapeDataString($target)
        if ($target -match '^(https?:|mailto:)') { continue }
        "W|$target"
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

    foreach ($raw in Get-LinkTargets -Text $text) {
        $kind   = $raw.Substring(0, 1)
        $target = $raw.Substring(2)

        if ($kind -eq 'W') {
            # yer tutuculu ornek (sablon govdesi, G4 istisnasi) -> sayilmaz
            if ($target -match '<[^>]+>') { continue }
            # wikilink: vault ROOT'una goreli (uzanti YOKSA .md ekle; dizin SONU / aynen) - D50
            $wt = $target
            if (($wt -notmatch '\.\w+$') -and ($wt -notmatch '/$')) { $wt = "$wt.md" }
            $resolved = Join-Path $rootFull $wt
            if (-not (Test-Path -LiteralPath $resolved)) {
                # uzantisiz aday (dizin veya uzantisiz dosya)
                if (Test-Path -LiteralPath (Join-Path $rootFull $target)) {
                    $resolved = Join-Path $rootFull $target
                } else {
                    # Obsidian shortest-path: tek eslesme kabul (kisa ad VEYA path suffix) - D50
                    $base = [IO.Path]::GetFileNameWithoutExtension(($target -replace '\\','/').TrimEnd('/'))
                    if ($base) {
                        $hits = @(Get-ChildItem -LiteralPath $rootFull -Recurse -Filter "$base.md" -File -ErrorAction SilentlyContinue |
                                  Where-Object { $_.FullName -notmatch '\\\.obsidian\\' })
                        if ($hits.Count -eq 1) { $resolved = $hits[0].FullName }
                    }
                }
            }
        } else {
            $isAbs = [regex]::IsMatch($target, '^[a-zA-Z]:[\\/]')
            $resolved = if ($isAbs) { $target } else { Join-Path $dir $target }
            if ((-not $isAbs) -and (-not (Test-Path -LiteralPath $resolved))) {
                # uzantisi kalmis markdown hedefi: .md aday + shortest-path (D50 karma gecis)
                if ((Test-Path -LiteralPath "$resolved.md")) { $resolved = "$resolved.md" }
                elseif ($target -notmatch '\.\w+$') {
                    $base = [IO.Path]::GetFileNameWithoutExtension(($target -replace '\\','/').TrimEnd('/'))
                    if ($base) {
                        $hits = @(Get-ChildItem -LiteralPath $rootFull -Recurse -Filter "$base.md" -File -ErrorAction SilentlyContinue |
                                  Where-Object { $_.FullName -notmatch '\\\.obsidian\\' })
                        if ($hits.Count -eq 1) { $resolved = $hits[0].FullName }
                    }
                }
            }
        }

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
