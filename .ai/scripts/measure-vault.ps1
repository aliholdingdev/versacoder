#Requires -Version 5.1
param([string]$Root = 'C:\versacoder\.ai', [string]$Json = '')

$ErrorActionPreference = 'Stop'
$all = Get-ChildItem -Path $Root -Recurse -Filter *.md -File |
    Where-Object { $_.FullName -notlike '*.old.*' }

# 1) Klasör bazında envanter
$byFolder = $all | Group-Object { Split-Path $_.DirectoryName -Leaf } |
    Select-Object Name,
        @{n = 'Files'; e = { $_.Count } },
        @{n = 'KB'; e = { [math]::Round(($_.Group | Measure-Object Length -Sum).Sum / 1KB, 1) } } |
    Sort-Object KB -Descending

# 2) Kök dosyalar (`.ai\*.md`)
$rootFiles = Get-ChildItem -Path $Root -Filter *.md -File |
    Select-Object Name, @{n = 'KB'; e = { [math]::Round($_.Length / 1KB, 1) } } |
    Sort-Object Name

# 3) Frontmatter denetimi (ilk satır '---' olmalı)
$noFm = $all | Where-Object { (Get-Content $_.FullName -TotalCount 1) -ne '---' }

# 4) CR-only tarama (yalnızca CR içeren, LF içermeyen dosya)
$crOnly = $all | Where-Object {
    $raw = [IO.File]::ReadAllText($_.FullName)
    $raw.Contains("`r") -and -not $raw.Contains("`n")
}

# 5) Kodlama sağlığı: mojibake bayrağı
$bad = $all | Where-Object {
    $raw = [IO.File]::ReadAllText($_.FullName)
    $raw -match 'Ã¤|Ã¼|Ã¶|â€|ï»¿|�'
}

$summary = [pscustomobject]@{
    TotalMD          = $all.Count
    TotalKB          = [math]::Round(($all | Measure-Object Length -Sum).Sum / 1KB, 1)
    RootMD           = $rootFiles.Count
    RootUnder30KB    = @($rootFiles | Where-Object KB -lt 30).Count
    RootOver88KB     = @($rootFiles | Where-Object KB -gt 88).Count
    NoFrontmatter    = $noFm.Count
    CROnly           = $crOnly.Count
    Mojibake         = $bad.Count
}

Write-Host '=== ÖZET ==='
$summary | Format-List
Write-Host '=== KLASÖRLER ==='
$byFolder | Format-Table -AutoSize
Write-Host '=== KÖK DOSYALAR (30-88 KB bandı dışındakiler kırmızı) ==='
$rootFiles | Format-Table -AutoSize

$rootFiles | Where-Object { $_.KB -lt 30 -or $_.KB -gt 88 } |
    ForEach-Object { Write-Host ("BANT DIŞI: {0} = {1} KB" -f $_.Name, $_.KB) }

if ($noFm)   { Write-Host 'FRONTMATTER YOK:';  $noFm   | ForEach-Object { Write-Host $_.FullName } }
if ($crOnly) { Write-Host 'CR-ONLY:';           $crOnly | ForEach-Object { Write-Host $_.FullName } }
if ($bad)    { Write-Host 'MOJIBAKE:';          $bad    | ForEach-Object { Write-Host $_.FullName } }

if ($Json) {
    [pscustomobject]@{ Summary = $summary; Folders = $byFolder; Root = $rootFiles } |
        ConvertTo-Json -Depth 5 | Set-Content -Path $Json -Encoding UTF8
    Write-Host ("JSON: {0}" -f $Json)
}

# Sert kapı: hedefe uymayan vault yeşil dönmez
$fail = ($summary.TotalMD -lt 500) -or ($summary.NoFrontmatter -gt 0) -or
        ($summary.CROnly -gt 0) -or ($summary.Mojibake -gt 0) -or
        ($summary.RootUnder30KB -gt 0) -or ($summary.RootOver88KB -gt 0)
if ($fail) { exit 1 }
exit 0

