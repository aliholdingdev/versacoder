<#
.SYNOPSIS
    .ai/architecture/catalog altindaki bilesen kayit sayisini dogrular (ADR-013).
.DESCRIPTION
    Kayit = '| NNN-### |' ile baslayan tablo satiri (8 sutun). Baslik, ayrac ve
    kod bloglari sayilmaz. Ayni zamanda dosya sayisini (53) ve katalog disindaki
    kayitlarin olmadigini raporlar.
.EXAMPLE
    pwsh -File .ai/scripts/count-components.ps1
    pwsh -File .ai/scripts/count-components.ps1 -MinRecords 5000   # CI
.NOTES
    Sahip: architect · Guncelleme: 2026-09-25 · authority: SSOT
    Bkz. .decisions/accepted/ADR-013-layered-architecture-52-nodes.md §2.2
#>
[CmdletBinding()]
param(
    [string]$Root = '',
    [int]$MinRecords = 5000,
    [int]$MinNodeFiles = 52
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($Root)) {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    $Root      = Join-Path (Split-Path -Parent $scriptDir) 'architecture\catalog'
}

if (-not (Test-Path -LiteralPath $Root)) {
    Write-Host "  [X] Katalog dizini yok: $Root" -ForegroundColor Red
    Write-Host "      Once: pwsh -File .ai/scripts/build-catalog.ps1 -Force" -ForegroundColor Yellow
    exit 1
}

$files = Get-ChildItem -LiteralPath $Root -Recurse -Filter *.md -File
$recordPattern = '^\|\s*N\d{2}-\d{3}\s*\|'

$total   = 0
$rows    = New-Object System.Collections.Generic.List[object]
$badFmt  = 0

foreach ($f in $files) {
    $lines   = [IO.File]::ReadAllLines($f.FullName)
    $count   = 0
    $weakCol = 0
    foreach ($l in $lines) {
        if ($l -match $recordPattern) {
            $count++
            # 8 sutun kontrolu: satirda en az 9 boru olmali (8 sutun + 1)
            $pipes = ([regex]::Matches($l, '\|')).Count
            if ($pipes -lt 9) { $weakCol++ }
            # durum sutunu planli/yapildi/testli disinda olmamali
            if ($l -notmatch '\|\s*(planli|yapildi|testli)\s*\|\s*$') { $badFmt++ }
        }
    }
    $total += $count
    if ($count -gt 0) {
        $rel = $f.FullName.Substring($Root.Length + 1)
        $rows.Add([pscustomobject]@{ File = $rel; Records = $count; BadFormat = $weakCol })
    }
}

$nodeFiles  = ($rows | Where-Object { $_.File -notmatch '^\s*index\.md$' }).Count
$indexFile  = Test-Path (Join-Path $Root 'index.md')
$pass       = ($total -ge $MinRecords) -and ($nodeFiles -ge $MinNodeFiles) -and $indexFile -and ($badFmt -eq 0)

Write-Host ''
Write-Host '==============================================================' -ForegroundColor Cyan
Write-Host '  KATALOG SAYIM RAPORU  (ADR-013 §2.2 / GATE-004a)' -ForegroundColor Cyan
Write-Host '==============================================================' -ForegroundColor Cyan
Write-Host ("  Katalog kok      : {0}" -f $Root)
Write-Host ("  Kayit dosyasi    : {0}  (esik {1})" -f $nodeFiles, $MinNodeFiles) -ForegroundColor $(if ($nodeFiles -ge $MinNodeFiles) { 'Green' } else { 'Red' })
Write-Host ("  index.md         : {0}" -f $(if ($indexFile) { 'VAR' } else { 'YOK' }))
Write-Host ("  TOPLAM KAYIT     : {0}  (esik >= {1})" -f $total, $MinRecords) -ForegroundColor $(if ($total -ge $MinRecords) { 'Green' } else { 'Red' })
Write-Host ("  Hatali format    : {0}" -f $badFmt) -ForegroundColor $(if ($badFmt -eq 0) { 'Green' } else { 'Red' })
Write-Host '--------------------------------------------------------------' -ForegroundColor Cyan
$mins = ($rows | Sort-Object Records | Select-Object -First 3)
$maxs = ($rows | Sort-Object Records -Descending | Select-Object -First 3)
Write-Host '  En az kayitli 3 :' -ForegroundColor Yellow
foreach ($r in $mins) { Write-Host ("    {0,4}  {1}" -f $r.Records, $r.File) }
Write-Host '  En cok kayitli 3:' -ForegroundColor Yellow
foreach ($r in $maxs) { Write-Host ("    {0,4}  {1}" -f $r.Records, $r.File) }
Write-Host '==============================================================' -ForegroundColor Cyan
if ($pass) {
    Write-Host '  SONUC            : PASS' -ForegroundColor Green
    exit 0
} else {
    Write-Host '  SONUC            : FAIL' -ForegroundColor Red
    if ($total -lt $MinRecords) { Write-Host ("      Eksik kayit: {0}" -f ($MinRecords - $total)) -ForegroundColor Red }
    exit 1
}
