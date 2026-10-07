# add-bom.ps1 - UTF-8 BOM'suz .ps1 dosyasina BOM ekler (PS 5.1 icin)
$ErrorActionPreference = 'Stop'
$targets = @(
    'C:\versacoder\.ai\scripts\gen-layer-5000.ps1',
    'C:\versacoder\.ai\scripts\debug-parse.ps1',
    'C:\versacoder\.ai\scripts\debug-bytes.ps1',
    'C:\versacoder\.ai\scripts\debug-dedup.ps1'
)
foreach ($p in $targets) {
    $bytes = [System.IO.File]::ReadAllBytes($p)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    if ($hasBom) {
        Write-Output "BOM zaten var: $p"
    } else {
        $text = [System.IO.File]::ReadAllText($p, (New-Object System.Text.UTF8Encoding($false)))
        [System.IO.File]::WriteAllText($p, $text, (New-Object System.Text.UTF8Encoding($true)))
        Write-Output "BOM eklendi: $p"
    }
}
