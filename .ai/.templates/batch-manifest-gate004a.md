---
title: "GATE-004a Batch Manifestosu - 347 Dosya Kapsami"
type: index
category: templates
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "GATE-004a icin uretilecek tum yeni MD dosyalarinin klasor, sayi ve tema listesi - subagent gorev spec'i burasi"
---

# GATE-004a Batch Manifestosu

**Zorunlu Baglantilar:** [vault index](../index.md) · [new-doc-contract](new-doc-contract.md) · [ADR-003](../.decisions/accepted/ADR-003-vault-structure-500md-frontmatter.md) · [ADR-013](../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) · [CLAUDE.md](../CLAUDE.md)

> **Tek cumlelik sorumluluk:** *Hangi batch hangi klasore kac dosya, hangi temada yazar* sorusunun tek cevabidir.
> Uretim bu dosyaya gore yapilir; sapma = link/kapsam riski.

---

## §0 Ozet (Answer First)

147 mevcut → **hedef ≥500 MD**. Kapsanan yeni dosya: **~347** (index/sozlesme dosyaları haric). Her batch `new-doc-contract.md`'ye uyar, batch sonunda `check-links.ps1` = **0 bozuk**.

---

## §1 Batch Tablosu

| # | Klasor | Adet | Tema | Durum |
|---|---|---|---|---|
| B1 | `knowledge/devexpress/` | **60** | v26.1 kontrol ailesi (Ribbon, Grid, TreeList, Docking, Editors, Wizard, Theme, Data-binding, TFM uyumu) | ⏳ |
| B2 | `knowledge/winforms/` | **30** | Form yasam dongusu, designer+resx, DI, async, layout, kaynak/olcu, erisilebilirlik | ⏳ |
| B3 | `knowledge/data/` | **25** | ADO.NET + Dapper, parameterized SQL, migration+version tablosu, provider (SqlClient 5.2.x / Sqlite), secret, timeout | ⏳ |
| B4 | `knowledge/csharp/` | **25** | C# 7.3 (net48) vs C# 14 (net10), nullable, async, generics, hata yonetimi, interop, DI container | ⏳ |
| B5 | `architecture/l0..l7/` | **52** | Her katalog dugumu (N01..N52) icin bir **dugum detay** dosyasi: arayuz, akis, test plani, TFM | ⏳ |
| B6 | `spec/` | **35** | Katman+konu şartnameleri (UI, editor, ajan, oturum, tool, guvenlik, performans, dagitim) | ⏳ |
| B7 | `.decisions/accepted/` | **20** | ADR-006..011 (planli) + ADR-014..027 (yeni) | ⏳ |
| B8 | `checklists/` | **30** | Gate + inceleme + sevk listeleri (her biri kanit bekleyen sorular) | ⏳ |
| B9 | `prompts/` | **30** | Rol bazli promptlar (architect, developer, reviewer, tester, devops, docs, security...) | ⏳ |
| B10 | `playbooks/` | **20** | Tekrar eden is akisleri (form, katman, migration, ADR, release, debug, refactor...) | ⏳ |
| B11 | `.rules/` | **15** | Ek kural dosyalari (ui, async, logging, caching, i18n, dosya sistemi, git, paket...) | ⏳ |
| B12 | `reports/` | **10** | Rapor 02..09 + 2 ek rapor (PART 4/5/6/7/8 kanitlari) | ⏳ |
| B13 | `ui-design/` | **10** | 7 wireframe + index + flows + component-inventory (**PART 4 - GATE-003**) | ⏳ (ayri gate) |

**Toplam yeni dosya ≈ 347** (+6 indeks/sozlesme zaten yazildi = 153 → **~500**).

---

## §2 Batch Kurallari (tum B1..B12 icin)

1. Her dosya `new-doc-contract.md` §1-§5 gecer: UTF-8, frontmatter 8 alan, §0 Ozet, en az 1 tablo, maks 5 madde.
2. **Link sadece** ilgili klasorun `new-doc-contract.md` §3 izinli hedefleri + kardes dosyalar (adlari tam yazilir).
3. Her batch **kendi klasorunun `index.md`'sini** gunceller/kayit satiri ekler (varsa).
4. Batch sonu: `check-links.ps1` → 0 bozuk. Bozuk varsa **o dosyayi duzelt**, raporla.
5. **Yasak:** EF Core · string-concat SQL · DevExpress v26.1 disi · Office2010 disi ribbon · CoreMusic/versacoder.ai'dan kopya metin (ADR-002).

---

## §3 Ayri Kural: B13 (PART 4 wireframe)

B13 **GATE-003** onayi ister: UTF-8 box-drawing wireframe, ekran listesi **master prompt SSOT**
(`01-start-page, 02-main-ide, 03-solution-explorer, 04-code-editor, 05-ai-chat, 06-agent-panel, 07-settings`),
format `.templates/ui-design/wireframe-template.md`. Koddan **once** kullanici onayi alinir.

---

## §4 REZERVE DOSYA ADLARI (zorunlu - bunlari uretmeyen batch link kirar)

Indeks dosyalari asagida adlari gecen dosyalara link atmistir; **bu adlar birebir** uretilmek zorundadir.

| Batch | Rezerve adlar (ilk sirada uretilmeli) |
|---|---|
| B1 `knowledge/devexpress/` | `index.md` (60 dosyanin kayit defteri) |
| B2 `knowledge/winforms/` | `index.md` |
| B3 `knowledge/data/` | `index.md` |
| B4 `knowledge/csharp/` | `index.md` |
| B8 `checklists/` | `gate-004a-vault-kapanis.md` · `gate-003-ui-onay.md` · `gate-004-ilk-commit.md` · `gate-005-test-kapsami.md` + 26 yeni |
| B9 `prompts/` | `architect-prompt.md` · `developer-prompt.md` · `reviewer-prompt.md` · `tester-prompt.md` + 26 yeni |
| B10 `playbooks/` | `yeni-katman-ekleme.md` · `yeni-form-tasarlama.md` · `migration-ekleme.md` · `adr-acma.md` + 16 yeni |
| B11 `.rules/` | `index.md` (9 mevcut + 15 yeni = 24 dosyanin kayit defteri) |

Kalan 3 link (`knowledge/index.md` → 4 alt index) B1-B4'in `index.md`'leri ile kapanir.

## §5 Dogrulama (GATE-004a kapanisi)

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1      # BOZUK link : 0
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1 # PASS (5440+)
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1    # TotalMD >= 500, NoFrontmatter 0, Mojibake 0
```

> `scripts/` (AŞAMA C) diskte yoksa §5 betikleri çalışmaz: §10'daki betiksiz bloklar aynı
> sayaçları üretir. GATE-004a'nın **kapıyı açan** ölçümü budur — tahmin değil, komut çıktısı.

---

## §6 Manifesto Şeması (§1–§4 Alanlarının Tanımı)

### §6.1 Batch Satırı (§1 Tablosu)

| # | Alan | Tanım | Zorunlu değer | İhlali |
|---|---|---|---|---|
| 1 | `#` | batch kimliği, benzersiz ve artan | `B1`…`B13` | iki `B1` = manifesto kırık |
| 2 | `Klasor` | `.ai/` köküne göre bölüm yolu, sondaki `/` dâhil | §1'deki 13 değerden biri | kendi uydurma yolu = kapsam dışı üretim |
| 3 | `Adet` | o batch'in ürettiği **yeni** dosya sayısı (indeks/sozlesme hariç) | tam sayı, §6.3 toplamına katılır | toplam ≠ 347 → §11 FM1 |
| 4 | `Tema` | dosyaların ortak konusu, virgülle 2–9 sözcük | okunabilir tema | "genel" gibi muğlak tema = red |
| 5 | `Durum` | §8 sözlüğündeki tek değer | `⏳` · `✅` · `❌` · `⏸` | `WIP` · `~` · boş = red |

### §6.2 Batch Kapanış Kaydı (batch biterken yazılır)

| # | Alan | İçerik | Kanıt türü |
|---|---|---|---|
| 1 | `Batch` | `B1`…`B12` (B13 ayrı gate — §3) | kimlik |
| 2 | `Hedef / Yazılan` | §1 `Adet` değeri / diskte gerçekten yazılan | §10/1 sayım |
| 3 | `Ölçüm` | satır · KB · kırık link · mojibake · frontmatter | §10/2–§10/4 çıktısı |
| 4 | `Eksik` | §4 rezerve adlarından üretilmeyen | §9 taraması |
| 5 | `Karar` | `ONAY` ya da `RED (<gerekçe>)` | imzalı tek satır |

### §6.3 Toplum Kuralı (Aritymetik Zorunluluk)

| # | Kural | Değer | Denetim |
|---|---|---|---|
| 1 | B1..B13 `Adet` satır toplamı | manifesto beyanı **≈ 347** (§1) | §10/1 çıktısı — fark ≠ 0 ise §11 FM1 |
| 2 | Mevcut + yeni | 147 + 347 ≈ 494; indeks/sozlesme ile **~500** | §5 `TotalMD >= 500` |
| 3 | Rezerve adlar (§4) | toplam **üretilmek zorunda** — sayıları `Adet`'e dâhil | §9 taraması = 0 eksik |
| 4 | B13 | **ayrı gate** (GATE-003); §1 toplamına dâhil edilmez ama satırı silinmez | §3 |

> Toplam uymuyorsa manifesto düzeltilir, dosya sayısı **uydurularak** toplama uydurulmaz:
> hedef sayı esastır (G9 — kanıtsız sayı yazılmaz). Satır toplamı ile §1 beyanı arasındaki
> fark **gizlenmez**, §10/1 çıktısıyla raporlanır.

---

## §7 GATE-004a Akışı

Kapı zinciri ([WORKFLOW](../../WORKFLOW.md) §1): `GATE-004a (vault) → GATE-003 (UI, HARD STOP) → GATE-004 → GATE-005 → GATE-006`.

| Adım | İş | Çıktı | Kanıt | Çıkış koşulu |
|---|---|---|---|---|
| 1 | Bu manifesto + [new-doc-contract](new-doc-contract.md) okunur | batch listesi + kural | okundu kaydı | — |
| 2 | Batch seçilir (§1 sırası) | tek batch kapsamı | `#` satırı | — |
| 3 | Dosyalar üretilir (§2 kuralları) | MD + `index.md` kaydı | dosya listesi | — |
| 4 | Ölçüm koşulur | 5 sayaç | §10 blokları | hepsi hedefte |
| 5 | Eksik tarama | §4 rezerve adlar | §9 komutu | eksik = 0 |
| 6 | `❌` varsa | o batch'e dönülür | §11 hata modu | tekrar adım 3 |
| 7 | B1..B12 `✅` | gate kapanır | Quality Report | GATE-003'e geçilir |

**Gate kapanış kriterleri (hepsi ve):**

| # | Kriter | Hedef | Kaynak |
|---|---|---|---|
| K-1 | Bozuk link | 0 | §10/4 |
| K-2 | Mojibake | 0 | §10/3 |
| K-3 | Frontmatter eksik | 0 | §10/3 |
| K-4 | `TotalMD` | ≥ 500 | §10/5 |
| K-5 | Rezerve ad eksik | 0 | §9 |
| K-6 | Yeni dosya boyutu | 3-8 KB sınıfı (şablonlar hariç) | [new-doc-contract](new-doc-contract.md) §10 |

> **Sıra kuralı:** GATE-004a kapalıyken GATE-003'e geçilmez; GATE-003 onaysız `src/**` kodu
> yazılmaz (G8). Kapıyı yalnız kullanıcı açar — bu manifesto **kanıt üretir**, kapı **açmaz**.

---

## §8 Satır Durumu Sözlüğü

§1 `Durum` sütunu ve batch kapanış kaydı aynı alfabeyi kullanır.

| # | Sembol | Ad | Ne demek | Hangi kanıtla geçer |
|---|---|---|---|---|
| 1 | `⏳` | planlı | hedef tanımlı, henüz üretilmedi | — (başlangıç değeri) |
| 2 | `✅` | tamam | hedef adet yazıldı + §10 sayaçları hedefte + §9 eksik 0 | komut çıktısı + Quality Report |
| 3 | `❌` | red | ölçüm hedefte değil ya da §11 hata modu tuttu | red gerekçesi tek satır |
| 4 | `⏸` | kapalı kapı | gate BLOCKED (ör. B13 GATE-003 onayı bekler) | kapı kaydı |
| 5 | `➖` | kapsam dışı | satır manifesto dışında (silinmez, not düşülür) | karar satırı |

**Geçiş kuralları:**

| # | Geçiş | Koşul | Yapılmazsa |
|---|---|---|---|
| 1 | `⏳ → ✅` | adım 4 + 5 birlikte geçer | kanıtsız "bitti" (R15) |
| 2 | `✅ → ❌` | ölçüm bozulursa anında | bozuk sayı raporlanır |
| 3 | `❌ → ✅` | düzelt + yeniden ölç | eski `✅` sayılır, gate erken kapanır |
| 4 | `⏳ → ⏸` | kapı kaydı varsa | kapı atlanmış görünür |
| 5 | `WIP` · `~` · boş | **yasak** — tek değerli alfabe | manifesto okunmaz hâle gelir |

---

## §9 Eksik Dosya Taraması

§4'teki rezerve adlar **birebir** üretilmek zorundadır; üretilmezse indekslerdeki linkler
kırılır. Betik yoksa (AŞAMA C) bu blok iş görür:

```powershell
$root = 'C:\versacoder\.ai\.templates'   # rezerve adlar §4 tablosundan okunur
$rezerve = @(
  'index.md',
  'gate-004a-vault-kapanis.md','gate-003-ui-onay.md','gate-004-ilk-commit.md','gate-005-test-kapsami.md',
  'architect-prompt.md','developer-prompt.md','reviewer-prompt.md','tester-prompt.md',
  'yeni-katman-ekleme.md','yeni-form-tasarlama.md','migration-ekleme.md','adr-acma.md'
)
$eksik = $rezerve | Where-Object { -not (Test-Path (Join-Path $root $_)) }
'rezerve {0} · eksik {1}' -f $rezerve.Count, @($eksik).Count
$eksik          # hedef: bu liste BOS (batch kapanmadan)
```

| # | Tarama | Ne yakalar | Sıklık |
|---|---|---|---|
| 1 | Rezerve ad (§4) | indeks linklerini kıracak eksik dosya | her batch sonu |
| 2 | Batch adet (§10/1) | `Adet` ↔ disk farkı | her batch sonu |
| 3 | Tema sızması | §1 teması dışında dosya | her batch sonu |
| 4 | Klasör sapması | `.ai/` dışında üretilmiş dosya | her batch sonu |
| 5 | Çift kayıt | aynı adla iki dosya (big/küçük harf farkı dâhil) | her batch sonu |

> Windows `Test-Path` büyük/küçük harf duyarlı değildir; **çift kayıt** adayları için dosya
> adları `Sort-Object` + birebir karşılaştırma ile ayrıca kontrol edilir (§10/1).

---

## §10 Doğrulama Blokları (Betiksiz — salt okunur)

### §10/1 Batch Adet ve Toplam

```powershell
$root = 'C:\versacoder\.ai\.templates'
$adet = @{ 'B1'=60; 'B2'=30; 'B3'=25; 'B4'=25; 'B5'=52; 'B6'=35; 'B7'=20;
           'B8'=30; 'B9'=30; 'B10'=20; 'B11'=15; 'B12'=10; 'B13'=10 }
$toplam = ($adet.Values | Measure-Object -Sum).Sum
'manifesto beyani 347 · satir toplami {0} · fark {1} (fark != 0 ise §11 FM1)' -f $toplam, ($toplam - 347)
(Get-ChildItem $root -Recurse -Filter *.md).Count   # diskteki gercek MD sayisi
```

### §10/2 Satır ve KB (G6 · şablon sınıfı)

```powershell
Get-ChildItem 'C:\versacoder\.ai\.templates' -Filter *.md |
  ForEach-Object { [PSCustomObject]@{
      Dosya = $_.Name
      Satir = (Get-Content $_.FullName -Encoding UTF8).Count
      KB    = [math]::Round($_.Length / 1KB, 1)
      G6    = ((Get-Content $_.FullName).Count -ge 480) } } |
  Sort-Object Satir | Format-Table -AutoSize
# hedef: sozlesme/manifesto/sablon dosyalari G6 = True
```

### §10/3 Frontmatter ve Mojibake (toplu)

```powershell
$alan = '^(title|type|category|status|version|updated|authority|scope):'
Get-ChildItem 'C:\versacoder\.ai\.templates' -Recurse -Filter *.md | ForEach-Object {
  $ilk = Get-Content $_.FullName -Encoding UTF8 -TotalCount 12
  $r   = Get-Content $_.FullName -Raw -Encoding UTF8
  [PSCustomObject]@{
    Dosya      = $_.Name
    Alan       = @($ilk | Where-Object { $_ -match $alan }).Count
    Ayirac     = @($ilk | Where-Object { $_ -eq '---' }).Count
    Mojibake   = [regex]::Matches($r, '\u00C3.|\u00E2\u20AC').Count
    YerTutucu  = [regex]::Matches($r, '\x7B\x7B[^\x7D]+\x7D\x7D').Count
  }
} | Where-Object { $_.Alan -ne 8 -or $_.Ayirac -lt 2 -or $_.Mojibake -gt 0 } | Format-Table -AutoSize
# hedef: bu cikti BOS (eksik frontmatter 0 · mojibake 0 · kalip disi yer tutucu 0)
```

### §10/4 Bağlantı Kırık Sayacı

```powershell
$hedefler = @('new-doc-contract.md','batch-manifest-gate004a.md','llm-provider-template.md','index.md','CLAUDE.md')
foreach ($f in $hedefler) {
  $p = Join-Path 'C:\versacoder\.ai\.templates' $f
  $base = Split-Path $p
  $k = [regex]::Matches((Get-Content $p -Raw -Encoding UTF8), '\]\(([^)#]+)\)') |
       ForEach-Object { $_.Groups[1].Value } |
       Where-Object { $_ -notmatch '^(https?:|mailto:)' } | Sort-Object -Unique |
       Where-Object { -not (Test-Path (Join-Path $base $_)) }
  '{0,-30} kirik {1}' -f $f, @($k).Count
}
# hedef: YENI eklenen baglantida kirik 0; eski AŞAMA C hedefleri ayrica bilinir (bkz belge §8.1)
```

### §10/5 Quality Report Kalıbı

```markdown
## Quality Report — GATE-004a batch <Bn> (YYYY-MM-DD)
Adet: <yazilan>/<hedef> · olcum: link 0 · mojibake 0 · frontmatter 0 · TotalMD <sayi>
Rezerve eksik 0 · Durum: ⏳ -> ✅ | ❌ (<gerekçe>) · Karar: ONAY | RED
```

---

## §11 Hata Modları (FM1–FM10)

| # | Hata modu | Belirti | Kök neden | Düzeltme |
|---|---|---|---|---|
| FM1 | Adet toplamı tutmuyor | B1..B13 toplamı ≠ 347 | satır silinmiş ya da `Adet` değişmiş | manifesto §1'e dön, satırı geri ekle |
| FM2 | Rezerve ad üretilmemiş | §9 taraması boş değil | §4 atlanmış | adı **birebir** üret (link kırılır) |
| FM3 | Tema dışı dosya | batch'te başka konu dosyası | kapsam genişletilmiş | dosyayı doğru batch'e taşı |
| FM4 | Çift kayıt | aynı ad iki kez | iki agent aynı işi yapmış | fazlayı sil, tek `✅` bırak |
| FM5 | Kanıtsız `✅` | ölçüm çıktısı yok | adım 4 atlanmış | §10 bloklarını çalıştır, çıktıyı yapıştır |
| FM6 | Eski `✅` ile kapanış | ölçüm bozulmuş ama durum `✅` | `✅ → ❌` geçişi unutulmuş | durumu `❌` çek, yeniden ölç |
| FM7 | B13'ün GATE-003'siz üretilmesi | wireframe dosyaları var, onay yok | kapı atlanmış (G8) | dosyaları `⏸` yap, onaya dön |
| FM8 | Boyut sınıfı karışması | şablon < 480 satır ya da rapor > 8 KB | [new-doc-contract](new-doc-contract.md) §10 uygulanmamış | sınıfa göre böl/büyüt |
| FM9 | Yeni link AŞAMA C hedefe | `](../.rules/…)` gibi | §8.1 satır 4 ihlali | linki kaldır, yol metnine çevir |
| FM10 | Kanıtsız toplam | "500 MD" komut çıktısız | G9 ihlali | §10/1 çıktısını ekle ya da `NEEDS_VALIDATION` |

---

## §12 G1..G12 Eşlemesi (Manifesto → Guardrail)

| ID | Guardrail (tek satır) | Manifesto'daki yeri | Tespit |
|---|---|---|---|
| G1 | Harici vault'tan içerik kopyalanamaz; yalnız yapı referansı | §2.5 yasak · FM3 temas denetimi | metin karşılaştırması + `coremusic` taraması |
| G2 | EF Core yasak — ADO.NET + Dapper, parametreli sorgu | §2.5 yasak | `EntityFramework` geçen satır = 0 |
| G3 | Frontmatter'sız Markdown yasak (8 alan) | §7 K-3 · [contract](new-doc-contract.md) §9 | §10/3 `Alan = 8` |
| G4 | Üretilen dosyada yer tutucu kalmaz (0 adet) | §7 K-4 · §11 FM8 | §10/3 `YerTutucu = 0` |
| G5 | Türkçe zorunlu; mojibake yasak | §7 K-2 · §11 FM9 | §10/3 `Mojibake = 0` |
| G6 | < 480 satırlık şablon reddedilir | §7 K-4 · §10/2 sütunu `G6` | `Satir -ge 480` |
| G7 | Bozuk bağlantı yasak | §7 K-1 · §11 FM9 | §10/4 `kirik 0` (yeni linkte) |
| G8 | GATE-003 onaysız ekran/kod üretilmez | §3 B13 ayrı gate · §8 `⏸` | kapı kaydı |
| G9 | ADR'siz mimari karar yok; kanıtsız sayı yazılmaz | §6.3 not · §11 FM10 | kanıt satırı / `NEEDS_VALIDATION` |
| G10 | Concat SQL yasak; `@pN` zorunlu | §2.5 yasak · B3 teması | sorgu örneklerinde `@p1..@pN` |
| G11 | `TreatWarningsAsErrors` + %90 coverage | bu gate'in konusu değil (GATE-005) | `dotnet test` çıktısı |
| G12 | Envantere içerik kopyalanamaz (SRP) | §6.3 · §11 FM4 | envanter satırı = kayıt, içerik değil |

> Eşlemenin okunuşu: **her batch satırı** (§1) bir guardrail'e değil, **her guardrail** bir
> denetim noktasına (§7 + §10) bağlanır; denetim noktası olmayan guardrail = manifesto boşluğu.

---

## §13 Batch Uygulama Rehberi (B1–B13)

Her batch için dört sabit: **iskelet nereden**, **tema ne**, **tipik hata ne**, **kanıt ne**.
İskelet tümünde ortaktır: `new-doc-contract.md` §0–§6 + §7.1 adım tablosu.

### §13.1 B1 · `knowledge/devexpress/` — 60 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | v26.1 kontrol ailesi: Ribbon · Grid · TreeList · Docking · Editors · Wizard · Theme · Data-binding · TFM uyumu |
| 2 | Sabitler | DevExpress **Universal 2026 / v26.1** tek sürüm · ribbon **yalnız** `RibbonControlStyle.Office2010` |
| 3 | Tipik hata | sürüm sapması ("en güncel") · Office2010 dışı stil · kanıtsız kontrol davranışı |
| 4 | Kapanış | 60/60 · `knowledge/` alt index kaydı · §10/2–/4 çıktıları |

### §13.2 B2 · `knowledge/winforms/` — 30 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | form yaşam döngüsü · designer + resx · DI · async · layout · kaynak/ölçü · erişilebilirlik |
| 2 | Sabitler | TFM `net48` + `net10.0-windows`; `#if` yalnız platform farkında |
| 3 | Tipik hata | `async void` yayını · designer dosyasına elle müdahale · tek TFM varsayımı |
| 4 | Kapanış | 30/30 · alt index kaydı · §10/4 kırık 0 |

### §13.3 B3 · `knowledge/data/` — 25 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | ADO.NET + Dapper · parametreli SQL · migration + version tablosu · provider (SqlClient / Sqlite) · secret · timeout |
| 2 | Sabitler | **EF Core yasak** (G2) · her sorgu `@p1..@pN` (G10) · string concat yok |
| 3 | Tipik hata | `SELECT *` kanıtsız · migration'sız şema · anahtarın dokümanda yazılması (G12) |
| 4 | Kapanış | 25/25 · `EntityFramework` taraması = 0 · alt index kaydı |

### §13.4 B4 · `knowledge/csharp/` — 25 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | C# 7.3 (`net48`) vs C# 14 (`net10`) · nullable · async · generics · hata yönetimi · interop · DI |
| 2 | Sabitler | her madde **iki TFM** açısından yazılır; tek sürüm iddiası red |
| 3 | Tipik hata | `net10` özelliğini `net48`'e taşımak · nullable sözleşmesinin yarım yazılması |
| 4 | Kapanış | 25/25 · alt index kaydı · §10/4 kırık 0 |

### §13.5 B5 · `architecture/l0..l7/` — 52 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | katalog düğümü (N01..N52) başına tek dosya: arayüz · akış · test planı · TFM |
| 2 | Sabitler | katman yönü `l7 → l6 → l5 → l4 → l2 → l1 → l0`; `l3` yalnız `l0`; `l2`↔`l4` ok yok |
| 3 | Tipik hata | düğüm adı ile katalog sapması · koda bakmadan "doğrulandı" iddiası (G9) |
| 4 | Kapanış | 52/52 · her dosyada yön tablosu · `NEEDS_VALIDATION` sayımı raporlanır |

### §13.6 B6 · `spec/` — 35 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | katman + konu şartnameleri: UI · editor · ajan · oturum · tool · güvenlik · performans · dağıtım |
| 2 | Sabitler | şartname **test edilebilir** madde yazar ("olmalı" + ölçüt); belirsiz madde red |
| 3 | Tipik hata | şartnamenin tasarım anlatımına dönmesi · ADR'siz mimari karar (G9) |
| 4 | Kapanış | 35/35 · her şartnamenin ADR/bağlantı satırı · §10/4 kırık 0 |

### §13.7 B7 · `.decisions/accepted/` — 20 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | planlı `ADR-006..011` + yeni `ADR-014..027` |
| 2 | Sabitler | ad `ADR-NNN-<kebab>.md` · 5 bölüm (Durum/Bağlam/Karar/Sonuçlar/Alternatifler) · 3-8 KB |
| 3 | Tipik hata | numara çakışması/boşluğu · kararın başka dosyada tekrarı (G12) |
| 4 | Kapanış | 20/20 · numara aralığı **boşluksuz** · `adr` kaydı güncellenir |

### §13.8 B8 · `checklists/` — 30 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | gate + inceleme + sevk listeleri; **her madde kanıt bekleyen soru** |
| 2 | Sabitler | §4'teki 4 rezerve ad **birebir** üretilir (indeksler onlara link atmıştır) |
| 3 | Tipik hata | kanıtsız "✓" maddesi · rezerve adın yazımının sapması |
| 4 | Kapanış | 30/30 · §9 taraması = 0 eksik |

### §13.9 B9 · `prompts/` — 30 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | rol bazlı promptlar: architect · developer · reviewer · tester · devops · docs · security |
| 2 | Sabitler | rol · görev · çıktı biçimi · yasaklar · örnek çıktı (contract §7.5); anahtar **asla** |
| 3 | Tipik hata | kural metninin prompta gömülmesi (G12) · sır/anahtar metni (P1) |
| 4 | Kapanış | 30/30 · `sk-` taraması = 0 · §10/3 mojibake 0 |

### §13.10 B10 · `playbooks/` — 20 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | tekrar eden iş akışları: form · katman · migration · ADR · release · debug · refactor |
| 2 | Sabitler | adım tablosu + kanıt sütunu; "bitti" komut çıktısız yazılmaz (G9) |
| 3 | Tipik hata | adımın kanıtsız olması · playbookun şablonu tekrar etmesi (G12) |
| 4 | Kapanış | 20/20 · §10/2 satır/KB raporu |

### §13.11 B11 · `.rules/` — 15 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | ek kural dosyaları: ui · async · logging · caching · i18n · dosya sistemi · git · paket |
| 2 | Sabitler | `index.md` (9 mevcut + 15 yeni = 24) **rezerve** — kayit defteri ilk yazılır |
| 3 | Tipik hata | kuralın şablonda tekrarı (G12) · kural metninin kök dosyaya taşınması |
| 4 | Kapanış | 15/15 + `index.md` · kural ID'si çakışmasız |

### §13.12 B12 · `reports/` — 10 dosya

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | rapor 02..09 + 2 ek rapor (PART 4/5/6/7/8 kanıtları) |
| 2 | Sabitler | her sayı komut çıktılı (G9) · 3-8 KB · seri kanıtı kendi raporunda |
| 3 | Tipik hata | kanıtsız toplam (FM10) · bir rapora 8 KB üstü yığılma (contract §10.3) |
| 4 | Kapanış | 10/10 · ölçümler yapıştırılmış · §10/4 kırık 0 |

### §13.13 B13 · `ui-design/` — 10 dosya (AYRI GATE)

| # | Kural | Değer |
|---|---|---|
| 1 | Tema | 7 wireframe + index + flows + component-inventory |
| 2 | Sabitler | ekran listesi **master prompt SSOT**: `01-start-page · 02-main-ide · 03-solution-explorer · 04-code-editor · 05-ai-chat · 06-agent-panel · 07-settings` |
| 3 | Tipik hata | GATE-003 onayı olmadan üretim (G8) · Unicode kutu çizimi (§2) |
| 4 | Kapanış | onay **öncesi** `⏸`; onay sonrası 10/10 + tarihli kapı kaydı |

---

## §14 Sıra ve Paralellik

| # | Grup | Batch'ler | Kural |
|---|---|---|---|
| 1 | Paralel A | B1 · B2 · B3 · B4 | farklı `knowledge/` alt klasörleri; her biri kendi `index.md`'sini günceller |
| 2 | Sıralı B | B7 → B5 · B6 | ADR numaraları önce sabitlenir; mimari/spec ADR'ye bağlanır |
| 3 | Paralel C | B8 · B9 · B10 · B11 | birbirine link vermeyen dört klasör; §4 rezerve adları korunur |
| 4 | Son | B12 | raporlar diğer batch'lerin kanıtlarını **toplar** — en sona |
| 5 | Ayrı | B13 | GATE-003 kapısı; gruplara girmez (§13.13) |

- **Paralel = farklı klasör.** Aynı klasörde iki iş = §11 FM4 (çift kayıt).
- Geri dönüş serbest (eksik kanıt → o batch'e dön), **ileri sıçrama yasak** (ölçüm olmadan `✅`).

---

## §15 Red Team ve Referanslar

### §15.1 Sık Yapılan Hatalar

| # | Hata | Yakalama | Doğrusu |
|---|---|---|---|
| 1 | Manifesto satırı yerine "yaklaşık" sayı yazmak | §10/1 fark çıktısı | §6.3 — fark gizlenmez, FM1 ile düzeltilir |
| 2 | Rezerve adı yarım yazmak (`gate-004a.md`) | §9 eksik listesi | §4'teki ad **birebir** |
| 3 | Ölçümden önce `✅` basmak | §8 geçiş kuralı 1 | adım 4 + 5 birlikte |
| 4 | B13'ü normal batch gibi üretilmek | §13.13 `⏸` | GATE-003 onayı koddan önce |
| 5 | Aynı klasörde iki agent | §11 FM4 | paralellik = farklı klasör (§14) |
| 6 | Toplamı dosya sayısına uydurmak | §6.3 not | hedef sayı esastır (G9) |
| 7 | Eski kırık linki "yeni hata" sanmak | §10/4 notu | eski AŞAMA C hedefleri kasıtlıdır |

### §15.2 Referanslar

| # | Kaynak | Ne verir | Bağlantı |
|---|---|---|---|
| 1 | Sözleşme | frontmatter · link · boyut · red nedenleri | [new-doc-contract](new-doc-contract.md) |
| 2 | Envanter | kalite sözleşmesi §3.7 · G1..G12 · şablon satırları | [index](index.md) |
| 3 | Kapı zinciri | `GATE-004a → 003 → 004 → 005 → 006` · kanıt nerede | [WORKFLOW](../../WORKFLOW.md) |
| 4 | Kök kural | rol · G1..G12 özeti · katman listesi | [CLAUDE](../../CLAUDE.md) |
| 5 | Şablon kuralı | `.templates/` dizin kuralı · okuma sırası | [CLAUDE (klasör)](CLAUDE.md) |

> **SSUT:** bu manifesto GATE-004a'da **hangi dosyanın üretileceğinin** tek kaynağıdır;
> **nasıl** üretileceği `new-doc-contract.md`'de, **durumu** `.ai/log.md`'dedir — üçü de
> birbirine kopya satır yazmaz (G12).

---

## §16 Manifesto Bakım Kuralları

Manifesto yaşayan bir belgedir: batch kapandıkça `Durum` değişir, yeni batch eklendikçe
satır açılır. Değişiklik sınıflandırması:

| # | Değişiklik | Sürüm | Zorunlu kayıt | Örnek |
|---|---|---|---|---|
| 1 | Satır `Durum` geçişi (`⏳ → ✅` / `✅ → ❌`) | `patch` | §6.2 kapanış kaydı + ölçüm çıktısı | B1 tamamlandı |
| 2 | Yeni batch satırı (B14+) | `minor` | §6.1 şemasına uyan satır + §6.3 toplamı yeniden | yeni klasör açıldı |
| 3 | `Adet` düzeltmesi | `minor` | §10/1 fark çıktısı + gerekçe tek satır | sayım hatası |
| 4 | Satır silme / `#` yeniden adlandırma | `major` | bağlantı veren dosyalar + oturum kaydı | B5 ikiye bölündü |
| 5 | §13 rehberine kural/istisna ekleme | `minor` | Quality Report | yeni tipik hata |
| 6 | §4 rezerve ad değişimi | `major` | §9 taraması yeniden + indeks düzeltmesi | ad yazımı |

**Bakım kuralı listesi (en fazla 5 madde):**

1. **Ölçüm önce, durum sonra:** `✅` yalnız §10 çıktılarıyla basılır (§8 geçiş 1).
2. **Toplam uydurulmaz:** `Adet` değişince §6.3 farkı gizlenmez, FM1 ile raporlanır.
3. **Rezerve ad korunur:** §4 adı değişirse indeks linkleri kırılır → `major` + tarama.
4. **Silme yok:** kapsam dışı kalan satır `➖` olur, tablodan çekilmez (§8 satır 5).
5. **Tek yazan:** aynı batch'i iki oturum değiştiremez — ikincisi FM4 (çift kayıt) üretir.
