# debug-dedup.ps1 - gen parse'unin KOPYASI: ham sayim + tier7 tekrarlarinin kaynagi
$ErrorActionPreference = 'Stop'
$Kaynak = "G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md"
$taban = @{}
$tierNo = $null
foreach ($satir in [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)) {
    if ($satir -match '^### Tier (\d+)') { $tierNo = [int]$Matches[1]; $taban[$tierNo] = New-Object System.Collections.Generic.List[string] }
    elseif ($tierNo -ne $null -and $satir -match '^-?\s*-\s+(VersaCoder\.[A-Za-z0-9_.]+)') { $taban[$tierNo].Add($Matches[1]) }
}
Write-Output '--- ham sayim (girintili satirlar dahil, ^-?\s*- duzeltilmis) ---'
$rawTotal = 0
foreach ($k in ($taban.Keys | Sort-Object)) { Write-Output ("Tier {0}: {1}" -f $k, $taban[$k].Count); $rawTotal += $taban[$k].Count }
Write-Output ("Ham toplam: {0}" -f $rawTotal)

# gen script'in ESKI regexi ile de karsilastir (^- ...)
$taban2 = @{}
$tierNo = $null
foreach ($satir in [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)) {
    if ($satir -match '^### Tier (\d+)') { $tierNo = [int]$Matches[1]; $taban2[$tierNo] = New-Object System.Collections.Generic.List[string] }
    elseif ($tierNo -ne $null -and $satir -match '^-\s+(VersaCoder\.[A-Za-z0-9_.]+)') { $taban2[$tierNo].Add($Matches[1]) }
}
Write-Output '--- eski regex (^-, girinti yasak) ---'
$raw2 = 0
foreach ($k in ($taban2.Keys | Sort-Object)) { Write-Output ("Tier {0}: {1}" -f $k, $taban2[$k].Count); $raw2 += $taban2[$k].Count }
Write-Output ("Eski toplam: {0}" -f $raw2)

# tier7 tekkik: ham listede tekrar var mi?
$l7 = $taban[7]
$uniq = New-Object 'System.Collections.Generic.HashSet[string]'
$dup = @()
foreach ($x in $l7) { if (-not $uniq.Add($x)) { $dup += $x } }
Write-Output ("Tier7 ham: {0}, benzersiz: {1}, liste-ici tekrar: {2}" -f $l7.Count, $uniq.Count, $dup.Count)
if ($dup.Count -gt 0) { Write-Output ("  ornek tekrar: {0}" -f $dup[0]) }

# global dedup simülasyonu (tier 0..8 sirasiyla)
$global = New-Object 'System.Collections.Generic.HashSet[string]'
foreach ($k in (0..8)) {
    if (-not $taban.ContainsKey($k)) { continue }
    $eklenen = 0; $tekrarlanan = 0; $ornek = $null
    foreach ($x in $taban[$k]) { if ($global.Add($x)) { $eklenen++ } else { $tekrarlanan++; if (-not $ornek) { $ornek = $x } } }
    Write-Output ("Tier {0}: yeni={1} tekrar={2} ornek={3}" -f $k, $eklenen, $tekrarlanan, $ornek)
}
