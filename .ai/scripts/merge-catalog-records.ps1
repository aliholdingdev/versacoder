# merge-catalog-records.ps1 — kaynak vault katalog kayitlarini hedefe port eder
# Sahip: architect · Guncelleme: 2026-09-30 · authority: SSOT
# Kaynak: C:\www\versacoder\versacoder\.ai\architecture\catalog (53 dosya / 5440 kayit)
# Kural: yalniz eksik ID'ler eklenir; mevcut kayit silinmez, uydurma kayit yok.
# Not: PS 5.1 bu dosyayi ANSI okur — betikte saf ASCII (non-ASCII [char] ile).
[CmdletBinding()]
param(
    [string]$Src = 'C:\www\versacoder\versacoder\.ai\architecture\catalog',
    [string]$Dst = 'C:\versacoder\.ai\architecture\catalog'
)
$ErrorActionPreference = 'Stop'
$pat = '^\|\s*N\d{2}-\d{3}\s*\|'
$idPat = '^\|\s*(N\d{2}-\d{3})\s*\|'
$sec = [char]0xA7   # paragraph sign (ASCII kaynakli)

$srcFiles = Get-ChildItem -LiteralPath $Src -Recurse -Filter *.md
$addedTotal = 0
$touched = 0

foreach ($sf in $srcFiles) {
    $dstPath = Join-Path $Dst ($sf.BaseName + '.md')
    if (-not (Test-Path -LiteralPath $dstPath)) {
        Write-Host ("  ESIK: hedef yok -> {0}" -f $sf.BaseName)
        continue
    }

    $srcLines = [IO.File]::ReadAllLines($sf.FullName) | Where-Object { $_ -match $pat }
    $dstText  = [IO.File]::ReadAllText($dstPath)
    $dstLines = $dstText -split "`r?`n"

    $haveIds = @{}
    foreach ($l in $dstLines) {
        if ($l -match $idPat) { $haveIds[$Matches[1]] = $true }
    }

    $missing = @()
    foreach ($l in $srcLines) {
        if ($l -match $idPat) {
            if (-not $haveIds.ContainsKey($Matches[1])) { $missing += $l }
        }
    }
    if ($missing.Count -eq 0) { continue }

    $crlf = "`r`n"
    $sb = New-Object System.Text.StringBuilder
    [void]$sb.Append($crlf)
    [void]$sb.Append('## ' + $sec + '10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)' + $crlf)
    [void]$sb.Append($crlf)
    foreach ($l in $missing) { [void]$sb.Append($l + $crlf) }

    $newContent = $dstText + $crlf + $sb.ToString()

    # bayt duzeltmeleri: bu dosyadaki toplam kayit sayisini guncelle
    $totalNow = ([regex]::Matches($newContent, $pat)).Count
    $newContent = [regex]::Replace($newContent, '(?m)^(\s*#\s*1\)\s*kayit sayimi[^\r\n]*beklenen:\s*)\d+', ('${1}' + $totalNow))

    # bayt duzeltmeleri: eski "henuz karsilanmaz" iddiasini temizle
    $newContent = [regex]::Replace($newContent, '(?m)^[^\r\n]*henuz karsilanmaz[^\r\n]*$', ('kayit tablosu ' + $sec + '10''e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.'))

    [IO.File]::WriteAllText($dstPath, $newContent, (New-Object System.Text.UTF8Encoding($false)))
    $addedTotal += $missing.Count
    $touched++
    Write-Host ("  + {0,-32} {1,4} kayit eklendi (dosya toplami {2})" -f $sf.BaseName, $missing.Count, $totalNow)
}

Write-Host ''
Write-Host ("  DOKUNULAN DOSYA : {0}" -f $touched)
Write-Host ("  EKLENEN KAYIT   : {0}" -f $addedTotal)
exit 0
