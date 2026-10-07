# debug-parse.ps1 - taban parse neden eksik, satir bazli goster
$ErrorActionPreference = 'Stop'
$Kaynak = "G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md"
$lines = [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)
Write-Output ("Toplam satir: {0}" -f $lines.Count)
$tierNo = $null
$counts = @{}
$skip = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    $l = $lines[$i]
    if ($l -match '^### Tier (\d+)') { $tierNo = [int]$Matches[1]; if (-not $counts.ContainsKey($tierNo)) { $counts[$tierNo] = 0 }; continue }
    if ($tierNo -ne $null) {
        if ($l -match '^\s*-\s+VersaCoder\.') {
            if ($l -notmatch '^-\s+VersaCoder\.[A-Za-z0-9_.]+') { if ($skip.Count -lt 15) { $skip += ("satir {0}: regex GECMEDI: [{1}]" -f ($i+1), $l) } }
            else { $counts[$tierNo]++ }
        }
    }
}
foreach ($k in ($counts.Keys | Sort-Object)) { Write-Output ("Tier {0}: {1}" -f $k, $counts[$k]) }
Write-Output '--- regex gecmeyen ornekler ---'
foreach ($s in $skip) { Write-Output $s }
