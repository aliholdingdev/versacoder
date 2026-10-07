# fill-gate004a.ps1 - GATE-004a checklist'ini olculmus kanit degerleriyle doldurur
# Sahip: architect - Guncelleme: 2026-09-30 - authority: SSOT
# Kural: yalniz [ ] -> [x] + Kanit satirina "SONUC" degeri eklenir.
# Soru 7 BILINCLI isaretlenmez (authority/updated istisnasi - kullanici karari).
# Not: PS 5.1 bu dosyayi ANSI okur - betikte saf ASCII.
[CmdletBinding()]
param([string]$Path = 'C:\versacoder\.ai\checklists\gate-004a-vault-kapanis.md')
$ErrorActionPreference = 'Stop'

$res = @{}
$res['1']  = 'SONUC: TotalMD = 500 (esik >= 500), measure exit=0'
$res['2']  = 'SONUC: NoFrontmatter = 0'
$res['3']  = 'SONUC: BOZUK link : 0 / Iyi link : 5699, check-links exit=0'
$res['4']  = 'SONUC: Mojibake file count: 0, find-mojibake exit=0'
$res['5']  = 'SONUC: TOPLAM KAYIT 5440 (esik >= 5000), Hatali format 0, SONUC PASS, exit=0'
$res['6']  = 'SONUC: 14/14 dosya diskte, hepsinin ilk satiri ---'
$res['8']  = 'SONUC: 4 x True (Test-Path hepsi)'
$res['9']  = 'SONUC: 4 hedef checklists/index.md icerinde; check-links 0 oldugundan kirik listesinde degiller'
$res['10'] = 'SONUC: 0 satir - src/ yok (0 csproj, EntityFrameworkCore 0 satir); kod yazilinca GATE-004 tekrar kosacak'
$res['11'] = 'SONUC: hedefte git status = "?? .ai/" (1 satir); C:\www altinda bizim yazdigimiz dosya yok (oradaki 5 untracked = eski proje artigi, onceden vardi)'
$res['12'] = 'SONUC: 0 kritik tekrar - 37 tekrarli uzun satirin hepsi kalip: sablon esleri 2 cift, katalog kalip satiri 32 dosya, boundary-check satiri 2 dosya; veri/karar tekrari yok'
$res['13'] = 'SONUC: log.md satir 169 - "22 | 2026-09-30 | dort kapinin final kosusu | measure=0/links=0/mojibake=0/count=PASS | GATE-004a kullanici onayi bekliyor"'
$res['14'] = 'SONUC: 0 dosya (Get-ChildItem *.old.* = 0)'

$lines = [System.Collections.Generic.List[string]]::new()
foreach ($l in [IO.File]::ReadAllLines($Path)) { $lines.Add($l) }

$cur = ''
$ticked = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^- \[ \] (\d+)\.') {
        $cur = $Matches[1]
        if ($res.ContainsKey($cur)) {
            $lines[$i] = $lines[$i].Replace('- [ ] ', '- [x] ')
            $ticked++
        }
        continue
    }
    if ($cur -ne '' -and $res.ContainsKey($cur) -and $lines[$i] -match '^\s*\*\*Kanit:\*\*') {
        $lines[$i] = $lines[$i].TrimEnd() + ' => ' + $res[$cur]
        $res.Remove($cur)
        $cur = ''
    }
}

# Soru 7: istisna notu ekle (isaret YOK - kullanici karari)
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^- \[ \] 7\.') {
        $lines.Insert($i + 2, '  **ISTISNA (kullanici karari):** olcum - 500/500 dosyada frontmatter var (alan 8-9), authority satiri eksik 0; dagilim: SSOT 426 | derived 55 (catalog - ADR-013ten turetilmis) - ".ai/AGENTS.md" 16 (prompts). `updated` literal 2026-09-25 yerine guncel tarih (2026-09-29/30). Secenek: A) bu istisnayi kabul et (dogru etiketler korunur) B) hepsini SSOT + 09-25 yap (uydurma olur, RED).')
        break
    }
}

[IO.File]::WriteAllText($Path, ($lines -join "`r`n"), (New-Object System.Text.UTF8Encoding($false)))
Write-Host ("  isaretlenen madde: {0} / 13 (soru 7 bilincli isaretsiz)" -f $ticked)
exit 0
