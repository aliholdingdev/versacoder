# debug-probe.ps1 - satir 279 ve 587-590 icin char code + iki regex sonucu
$ErrorActionPreference = 'Stop'
$Kaynak = "G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md"
$lines = [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)
function Show-Line([int]$n) {
    $l = $lines[$n-1]
    $cps = ($l.ToCharArray() | ForEach-Object { '{0:X4}' -f [int]$_ }) -join ' '
    $m1 = $l -match '^-\s+VersaCoder\.[A-Za-z0-9_.]+'
    $m2 = $l -match '^\s*-\s+VersaCoder\.'
    Write-Output ("satir {0} len={1} m1(icerik)={2} m2(baslangic)={3}" -f $n, $l.Length, $m1, $m2)
    Write-Output ("   char: {0}" -f $cps)
}
Show-Line 279
Show-Line 280
Show-Line 587
Show-Line 588
# tier3 araligi istatistik
$m1c = 0; $m2c = 0; $toplam = 0
for ($i = 278; $i -lt 515; $i++) {
    $l = $lines[$i]
    if ($l -match '^\s*-\s+VersaCoder\.') { $toplam++ ; if ($l -match '^-\s+VersaCoder\.[A-Za-z0-9_.]+') { $m1c++ } }
}
Write-Output ("Tier3 aralik: m2 gecen={0}, iclerinden m1 gecen={1}" -f $toplam, $m1c)
