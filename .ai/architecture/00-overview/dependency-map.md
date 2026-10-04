---
title: "Bagimlilik Haritasi - 8x8 Yon Matrisi ve Denetim"
type: architecture
category: architecture
status: active
version: 2.0.0
updated: 2026-10-04
authority: SSOT
scope: "l0..l7 arasinda izinli/yasak dogrudan kenarlar, kirmiz cizgiler, ihlal denetim komutlari (gercek src\ yollari), iki adet DOGERLAMA GEREKLI hucre + 5000 yon grubu icin uretilen kural seti (5000x5000 = 25.000.000 hucre, elle YAZILMAZ uretilir)"
---

# Bagimlilik Haritasi - Yon Matrisi ve Denetim

**Zorunlu Baglantilar:** [mimari index](../index.md) · [system-overview](system-overview.md) · [catalog index](../catalog/index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** *Hangi katman hangisine dogrudan referans verebilir, ihlal nasil yakalanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Matris tek kaynaktir: 18 izinli (OK) kenar, 38 yasak (X) kenar, 8 ayni-katman (-) hucresi (2026-10-03 sayimi - §2 tablosu kazanir; eski 20/40/4 yazimi bayatti, diagonda 8 `-` bulunur).** Katman sirasi `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalniz `l0`'a bakar; `l2` ile `l4` arasinda dogrudan ok **YOK** (repository portu `l0`'da, uygulamasi `l4`'te). ADR-028'teki iki `?` bu dosyada kapatildi: `l4 -> l0 = OK`, `l5 -> l4 = X`. Gelen `l3` sutunu `X` korunur ama ADR-028 sessizdir -> **DOGERLAMA GEREKLI** (§2.1/5).

| Olcut | Deger |
|---|---|
| Matris | 8 x 8 = 64 hucre (18 OK · 38 X · 8 -) |
| Zincir (inşa sirasi) | `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` |
| Kapali dogrudan kenar | `l5 -> l4` · `l4 -> l2` · `l2 -> l4` (hepsi port uzerinden) |
| Denetim | NetArchTest + 3 PowerShell taramasi (§4) |
| Acik soru | gelen `l3` sutunu (§2.1, madde 5) |

**Kural:** bir katman dosyasi ile bu matris celisirse bu matris kazanir; matris degilirse once ADR-028 degisir, sonra bu dosya guncellenir.

> **REVIZYON 2.0.0 (2026-10-02, ADR-030 v2.1.0):** hedef 5000 yon grubu = 5000 fiziksel `.dll` (K bandi, [k5000-dll-list](../layer-5000/k5000-dll-list.md)). 8x8 matris **olcum olarak kalir** (18 OK / 38 X / 8 -), yeni 5000 grup bu deseni **bozmamalidir**. 5000x5000 = 25.000.000 hucre **elle yazilmaz** — kural seti **uretilir** (§6).

---

## §6 5000 Yon Grubu icin Kural Uretimi (YENI - elle YAZILMAZ)

**Sorun:** 5000 yon grubu -> matris **5000 x 5000 = 25.000.000 hucre**. Elle uretim
imkansiz (ADR-030 §2.3, A5 RED). Bu dosya kurali **uretir**, elle **yazmaz**.

### §6.1 Uretim kurali (tek satir, genel)

```text
l_i -> l_j  IZINLI  <=>  j <= i   (insa sirasi: l4999 -> ... -> l0)
                                 (yani alt katman, ust katmana bakamaz;
                                  ust katman alt katmana bakar)
l_i -> l_i  AYNI KATMAN  ->  '-'  (ayni `.dll` icinde bagimlilik, matris hucresi degil)
```

**Uygulama (NetArchTest):**

| # | Kural | Beklenen |
|---|---|---|
| 1 | `ProjectReference` yonu: hedef `l_j` j > i ise **RED** | 0 ihlal |
| 2 | Ters kenar (yukari referans) **RED** (R10) | 0 ihlal |
| 3 | Kapali kenarlar (§3 R1-R10) **her boyutta ayni** | 0 ihlal |
| 4 | 8x8 olculmus matris (§2) **dokunulmaz**; 5000 grup bu deseni **bozmaz** | 18 OK / 38 X / 8 - |

### §6.2 Olculmus matris vs uretilmis matris

| Matris | Boyut | Hucre | Kaynak | Ne icin |
|---|---|---|---|---|
| **Olculmus** (bugun) | 8 x 8 | 64 | bu dosya §2 (ADR-028) | NetArchTest `LayerRulesTests` (min 8 kural) |
| **Uretilmis** (hedef) | 5000 x 5000 | 25.000.000 | `tools/gen-matrix-5000.ps1` (§6.3) | her `.csproj` icin 1 satir kural |

**Iki matris CELISMEZ:** olculmus matris, uretilmis matrisin **ilk 8x8'idir**;
uretim kurali (§6.1) bu 8x8'i **aynen korur** (j <= i deseni 8x8'de de aynidir).
Celisirse **olculmus matris kazanir** (bu dosya §0), sonra uretim kurali guncellenir.

### §6.3 Uretim araci (planli - kod YOK)

```powershell
# tools/gen-matrix-5000.ps1  (K0'da yazilacak)
# 1. layer.json dosyalarini oku (K0000..K5499 dolu slotlar)
# 2. her `.csproj` icin 1 kural satiri uret: "l_i -> l_j izinli <=> j <= i"
# 3. NetArchTest kural dosyasina yaz (JSON, 5000 satir)
# 4. olculmus 8x8'i dogrula: ilk 8 satir §2 matrisiyle birebir olmali
```

**Beklenen cikti (K0'da olculur):**

| Arac | Cikti | Durum |
|---|---|---|
| `gen-matrix-5000.ps1` | 5000 satir kural (JSON) | **YAZILACAK** (K0) |
| NetArchTest `LayerRulesTests` | 0 failure | **YAZILACAK** (K0) |
| 8x8 dogrulama | ilk 8 satir §2 ile birebir | **OLCULMELI** |

> **DOGRULAMA GEREKLI:** 25.000.000 hucrelik matrisin kural uretim yolu ve NetArchTest
> kural dosyasi boyutu **henuz olculmedi** (ADR-030 §2.3). Bu dosya uretimi **emreder**,
> calisan ornegi **iddia etmez** (G9).

---

## §1 Okuma Kurali

| Terim | Anlam |
|---|---|
| **Referans** | `ProjectReference` veya `using` ile baska katman tipine bakmak |
| **OK / X / `-`** | izinli (ADR-028 §2.1+§2.3) · yasak (ihlal = NetArchTest RED) · ayni katman |
| **Yon** | satir = **bagimli olan** (from), sutun = **bagimli olunan** (to) |

---

## §2 Yon Matrisi (8 x 8)

| satır \ sutun | l0 | l1 | l2 | l3 | l4 | l5 | l6 | l7 |
|---|---|---|---|---|---|---|---|---|
| **l0** | - | X | X | X | X | X | X | X |
| **l1** | OK | - | X | X | X | X | X | X |
| **l2** | OK | OK | - | X | X | X | X | X |
| **l3** | OK | X | X | - | X | X | X | X |
| **l4** | OK | OK | X | X | - | X | X | X |
| **l5** | OK | OK | OK | X | X | - | X | X |
| **l6** | OK | OK | OK | X | OK | OK | - | X |
| **l7** | OK | X | OK | X | X | OK | OK | - |

**Sutun ozeti:** `l0`=l1..l7 · `l1`=l2,l4,l5,l6 · `l2`=l5,l6,l7 · `l3`=**hicbiri** (§2.1/5) · `l4`=l6 · `l5`=l6,l7 · `l6`=l7 · `l7`=hicbiri.

### §2.1 Hucre Notlari (celisiler dahil)

| # | Hucre | Karar | Kaynak / gerekce |
|---|---|---|---|
| 1 | `l4 -> l0` | **OK** (eski `?` kapandi) | portlar `l0`'a yazilir, uygulamasi `l4`'te (ADR-028 §2.3 madde 6: 4 port) |
| 2 | `l5 -> l4` | **X** (eski `?` kapandi) | tool cagrisi `l0` portundan gecer (ADR-028 §2.3 madde 2) |
| 3 | `l4 -> l2` | **X** | zincirdeki `l4 -> l2` **siradir, kenar degildir** (ADR-028 §2.3 madde 2) |
| 4 | `l2 -> l4` | **YOK** | repository portu `l0`'da; dogrudan ok iki yonlu akis dogurur (ADR-028 §2.1) |
| 5 | gelen `l3` sutunu (l1/l2/l4/l5/l6/l7 -> l3) | **X + DOGERLAMA GEREKLI** | ADR-028 yalniz `l3`'un **giden** okunu sinirlar, gelen oklara sessizdir; mimari index.md bu sutunu X yazar, kaynak vault haritasi ise `-> l3`'e izin verir. Yeni madde eklenene kadar **X** yazilir |
| 6 | `l7 -> l1`, `l7 -> l4` | **X** | ekran daima l2 DTO'suna baglanir |
| 7 | `l1 -> l3` ve diger giden `l3` oklari | **X** | `l3` yalniz `l0`'a bakar (ADR-028 §2.1) |

> **index.md guncellemesi:** [mimari index](../index.md) §4'teki iki `?` bu dosyayla kapandi; §4.1 satir 4-5 `DOGERLAMA GEREKLI` ile guncellenmelidir (GATE-004a oncesi).

---

## §3 Kirmizi Cizgiler

| # | Yasak | Kırilrsa | Gerekce |
|---|---|---|---|
| R1 | `l0` hicbir `VersaCoder.*` namespace'ine `using` yapamaz | CRITICAL | en alt sozlesme |
| R2 | `l3` yalniz `l0`'a bakar | CRITICAL | notr katman |
| R3 | `l1` `System.Data` / Dapper / dosya / DevExpress gormez | CRITICAL | saf C# katmani |
| R4 | `l2` `DbConnection`, SQL metni, DevExpress tipi gormez | HIGH | port anlamsizlasir |
| R5 | `l4` use case, handler, ribbon/form tipi gormez | HIGH | dongu riski |
| R6 | `l5` `DbConnection`/Dapper/dosya **dogrudan** gormez | HIGH | tool = port cagrisi |
| R7 | `l6` form, control, DevExpress tipi gormez | HIGH | kabuk degildir |
| R8 | `l7` entity (`l1`) ve altyapi (`l4`) gormez | HIGH | DTO zorunlu |
| R9 | Hicbir katman test projesine bakamaz | HIGH | test bagimliligi yasak |
| R10 | `ProjectReference` ters (yukari) olamaz | CRITICAL | hedef: 0 ters kenar |

---

## §4 Ihlal Denetimi (3 katman)

**1. Derleme ani:** izinli olmayan kenar `.csproj`'e hic yazilmaz.
**2. CI ani:** NetArchTest `LayerRulesTests`, §2 matrisine gore yazilir (8 kural minimum, bkz. [layer-boundary-check](../../checklists/layer-boundary-check.md)).
**3. Denetim ani (salt okunur):**

```powershell
# K1 - PROJECTREFERENCE GRAFI: cikti §2 matrisiyle birebir
Get-ChildItem 'C:\versacoder\src' -Recurse -Filter *.csproj |
  ForEach-Object { $f = $_.BaseName
    Select-String -Path $_.FullName -Pattern 'ProjectReference Include="([^"]+)"' -AllMatches |
    ForEach-Object { $_.Matches } | ForEach-Object {
      '{0} -> {1}' -f $f, [IO.Path]::GetFileNameWithoutExtension($_.Groups[1].Value) } }
```

```powershell
# K2 - YASAK USING: gercek proje yollari; cikti 0 satir OLMALI
$kalip = @{
  'VersaCoder.Domain'         = 'VersaCoder\.(Application|Infrastructure|Protocol|Host|Desktop|Cli)'
  'VersaCoder.Application'    = 'VersaCoder\.(Infrastructure|Protocol|Host|Desktop|Cli)'
  'VersaCoder.Infrastructure' = 'VersaCoder\.(Application|Protocol|Host|Desktop|Cli)'
  'VersaCoder.Protocol'       = 'VersaCoder\.(Infrastructure|Host|Desktop|Cli)'
  'VersaCoder.Host'           = 'VersaCoder\.(Desktop|Cli)' }
foreach ($d in $kalip.Keys) {
  $yol = "C:\versacoder\src\$d"
  if (-not (Test-Path $yol)) { "DIZIN YOK -> $d (kod henuz yok)"; continue }
  Get-ChildItem $yol -Recurse -Filter *.cs -ErrorAction SilentlyContinue |
    Select-String -Pattern ('^\s*using\s+' + $kalip[$d]) |
    ForEach-Object { 'USING IHLALI -> {0}:{1}' -f $d, $_.LineNumber } }
```

```powershell
# K3 - TEKNIK YASAKLAR: EF 0 · concat SQL 0 · DevExpress yalniz v26.1 · 3. TFM 0
$src = 'C:\versacoder\src'
if (-not (Test-Path $src)) { 'DIZIN YOK -> src/ henuz yok (kanit yok, PASS yazilmaz)' }
else {
  Get-ChildItem $src -Recurse -Filter *.cs | Select-String -Pattern 'EntityFramework|DbContext' |
    ForEach-Object { 'EF YASAK -> {0}:{1}' -f $_.Filename, $_.LineNumber }
  Get-ChildItem $src -Recurse -Filter *.cs | Select-String -Pattern 'SELECT[^"`]*\+\s*(str|@|\$)' |
    ForEach-Object { 'CONCAT SQL -> {0}:{1}' -f $_.Filename, $_.LineNumber }
  Get-ChildItem $src -Recurse -Filter *.csproj | Select-String -Pattern '<TargetFrameworks?>(?!net48|net10\.0-windows)' |
    ForEach-Object { 'TFM SAPMASI -> {0}' -f $_.Filename } }
```

> **Dikkat (ADR-028 §2.3 madde 5):** tarama yolu **gercek proje adlari** (`src\VersaCoder.Domain\...`) ile yazilir; `src/**/Domain/**` gibi genis kalip **sahte PASS** uretir.

| Komut | Beklenen |
|---|---|
| K1 | kenar listesi §2 matrisiyle birebir |
| K2 | 0 satir (dizin yoksa acik `DIZIN YOK`) |
| K3 | 0 satir (dizin yoksa acik `DIZIN YOK`) |
| NetArchTest | `0 failure` |

**Ihlal gorulurse:** (1) tipi `l0`'a tasi, (2) olay ile gevset, (3) ADR ile siniri yeniden kur - **asla** kenari ters cevirerek "cozme".

---

## §5 Dogrulama

- [ ] Frontmatter 8 alan · `authority: SSOT` · `updated: 2026-09-29`
- [ ] Matris 64 hucre, lejant 3 deger (OK / X / -)
- [ ] §2.1'de iki kapanmis `?` + bir `DOGERLAMA GEREKLI` satiri
- [ ] §4 komutlari yalniz `src\VersaCoder.*` gercek yollarini kullanir
- [ ] `check-links.ps1` -> bozuk link 0

**Kaynaklar:** [mimari index](../index.md) (katman adlari, N01-N44) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) (matrisin karar kaynagi) · [CLAUDE.md](../../CLAUDE.md) (G2/G5/G6) · [keys.md](../../keys.md) (anahtar eslemeleri).
