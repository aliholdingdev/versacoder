# debug-bytes.ps1 - gecmeyen satirin bayt/unicode haritasini goster
$ErrorActionPreference = 'Stop'
$Kaynak = "G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md"
$lines = [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)
foreach ($n in @(87, 279, 280, 638)) {
    $l = $lines[$n-1]
    $cps = ($l.ToCharArray() | Select-Object -First 30 | ForEach-Object { '{0:X4}' -f [int]$_ }) -join ' '
    Write-Output ("satir {0}: [{1}]" -f $n, $l)
    Write-Output ("   ilk30 char: {0}" -f $cps)
}
