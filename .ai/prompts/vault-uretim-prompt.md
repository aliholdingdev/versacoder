---
title: "Vault Uretim Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.2.0
updated: 2026-10-04
authority: SSOT
scope: "C:\versacoder\.ai vault'unu C:\versacoder.1\.ai kaynagindan uretmek icin kopyala-yapistir prompt: girdi envanteri (kaynak 16 klasor + hedefte mevcut 3 klasor + kok belge/G arsiv referanslari), tasinma kurali, 7 adim, 6 faz, olcum hedefleri"
---

# Vault Uretim Promptu

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu proje icin .ai vault'u hangi kaynaktan, hangi
> kurala gore, hangi sirayla uretirim* sorusunun tek cevabidir.

> **Web arastirma kurali:** once `.ai/.templates/` sablonlarini oku; exa web
> search MCP ile asagidaki sorgulari calistir; sonuclari sablon biciminde ve
> `.ai/**` icindeki ilgili dosyalara yaz (tek toplama noktasi asagida belirtilir).

### A) Stack arastirmasi (VersaCoder: `net48` + `net10.0-windows`, DevExpress v26.1)

Web Search : .NET Framework 4.8 WinForms supported operating systems Windows 10 11
Web Search : .NET 10.0-windows WinForms migration from .NET Framework 4.8 breaking changes
Web Search : DevExpress v26.1 RibbonControl RibbonControlStyle Office2010 usage
Web Search : C# WinForms Form UI code lives in *.Designer.cs partial class generated layout
Web Search : ADO.NET Dapper parameterized query pattern entity framework not required

### B) Github kaynak kod arastirmasi (benzer proje analizi)

> Amac: benzer bir acik kaynak projeyi incele, yapilandirmasini analiz et,
> VersaCoder icin uyarlama notu yaz; kopyalama degil, desen cikarimi.

Web Search : opencode github open source AI coding agent
Web Search : claude code leaked source github analysis
Web Search : AI code editor open source projects github
Web Search : NVIDIA NIM API provider integration
Web Search : OpenRouter API provider integration
Web Search : cursor IDE open source alternative github

### C) Yapay zeka prompt / skill / command arastirmasi (toplama noktasi)

> Toplanan tum cevaplar tek dosyada toplanir:
> `.ai/prompts/web-prompt-derlemesi.md` (dağınık not oraya yazilir, buraya degil).

Web Search : ChatGPT system prompt examples
Web Search : Claude prompt official documentation examples
Web Search : prompt examples collection
Web Search : Cursor prompt rules examples
Web Search : skill.md prompt examples
Web Search : slash command prompt examples
Web Search : plan mode prompt examples
Web Search : plan chat agent skill command template system prompt templates examples
 
---

## §0 Ozet (Answer First)

- **Kaynak saglam:** `C:\versacoder.1\.ai` = 16 klasor + 533 MD + 14 PS1 script; hicbiri hedefte yok.
- **Hedef kismen kuruldu (olcum 2026-10-04):** `C:\versacoder\.ai` = 14 kok belge + 3 klasor (`.templates` 38 MD · `prompts` 2 · `reports` 2) = **56 MD**; 771 linkin **257'si kirik** (514 iyi).
- **Kural:** tasima = arac (sablon/script/checklist) byte kopya, bilgi (knowledge/spec/architecture) uyarlama; kok belgeler 14 dosya olarak KALIR, uzerine yazilmaz.
- **Sira:** once klasor + sablon + script (linkler hedefe kavussun), sonra bilgi icerigi, en son index ve olcum.
- **Kapanis:** 4 sayac hedefte ise GATE-004a'ya kullanici onayi sunulur; ajan kapiyi kapatmaz.

---

## §1 Girdi envanteri (kaynak: C:\versacoder.1\.ai)

| # | Klasor | MD | Ne alinir | Nasil tasinir |
|---|---|---|---|---|
| 1 | `.templates/` | 36 | sablon + `new-doc-contract.md` (frontmatter 8 alan, link yasagi) | **byte kopya** (arac) |
| 2 | `.rules/` | 18 | 16 kural + index (EF Core yasagi, DevExpress 26.1, test, guvenlik) | **byte kopya** + tarih guncelle |
| 3 | `.decisions/` | 32 | ADR-001..028 + `adr-index.md` (ADR-028 katman temeli) | **byte kopya** |
| 4 | `checklists/` | 32 | gate listeleri (004a/003/004/005) + `layer-boundary-check` 13 soru | **byte kopya** |
| 5 | `scripts/` | 0 MD / 14 PS1 | `measure-vault`, `check-links`, `find-mojibake`, `count-components` | **byte kopya** |
| 6 | `.agents/` | 7 | architect/developer/reviewer/tester/security-auditor/ui-designer/general | **byte kopya** |
| 7 | `.sql/` | 1 | migration + `schema_version` ornegi | **byte kopya** |
| 8 | `spec/` | 45 | davranis tanimlari + `test-strategy.md` | uyarla (proje adlari hedefe) |
| 9 | `architecture/` | 129 | 00-overview + catalog + l0..l7 katman dosyalari | uyarla (link yollari) |
| 10 | `knowledge/` | 131 | bilgi notlari | uyarla (gerekli olan) |
| 11 | `playbooks/` | 19 | surec adimlari | uyarla |
| 12 | `prompts/` | 46 | ajan promptlari + bu dosyanin kardesleri | uyarla |
| 13 | `reports/` | 15 | kapsam analizi, teslim, bilinen eksikler | uyarla |
| 14 | `ui-design/` | 14 | 01..07 wireframe + envanter + flows | uyarla (GATE-003 girdisi) |
| 15 | `TODOS/` | 7 | 5 faz gorev listesi | uyarla |
| 16 | `plans/` | 1 | plan notu | uyarla |

**Kok dosya istisnasi:** kaynaktaki `.ai` icinde kok MD **yok**; hedefteki 14 kok
belge (`index, CLAUDE, WORKFLOW, ROLE, AGENTS, VISION, PROJECTS, brain, keys,
glossary, MEMORY, log, engine, ULTRA-THINKING`) mevcuttur ve **uzerine yazilmaz**
(G7). Yapilacak tek is: bu belgelerin linklerinin yeni klasorlere kavusmasi.

**Hedefte mevcut 3 klasor (olcum 2026-10-04):** `.templates/` (38 MD),
`prompts/` (2 MD), `reports/` (2 MD) hedefte zaten var; bunlar kaynakla
**birlestirilir, ezilmez** - ayni dosyanin hedefteki hali farkliysa G7 onayi gerekir.

**Dis referans (salt okunur, tasinma kapsami disi):**
- `C:\versacoder\AGENTS.md` = oturum boot talimati (78 satir) - okunur, yazilmaz.
- `C:\versacoder\CLAUDE.md`, `README.md`, `WORKFLOW.md` = **0 satir / 0 bayt**
  (olcum 2026-10-04): bos ve kapsam disi; doldurulmasi ayri bir is, G7 onayi ister.
- `G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\`
  = `versacoder-analiz-el10\` **92 part** + `coremusic-vault-template.md` ve
  `versacoder.*` ile birlikte **7 MD**; yalnizca **yapi/ornek** kaynagidir,
  icerik kopyalanmaz (G1).

---

## §2 Tasima kurali (hangisi kopya, hangisi yazim)

| # | Dosya turu | Kural | Gerekce |
|---|---|---|---|
| 1 | Sablon, script, checklist, ajan profili, ADR, SQL | byte kopya + `updated` tarihi bugun | Bunlar arac/karardir; yeniden yazim kopya hedefi degildir |
| 2 | knowledge, spec, playbooks, reports, ui-design, TODOS | kopyala -> link/frontmatter/tarih uyar -> proje adi dogrula | Hedef vault'un kendi gercegi farkli olabilir (G9) |
| 3 | 14 kok belge | DOKUNMA | SSOT; uzerine yazim G7 onayi ister |
| 4 | Bilinmeyen / eski iddia | `DOĞRULAMA GEREKLI` etiketi | Etiketsiz iddia uydurmadir (G9) |
| 5 | Kaynakta olup hedefte gerekmeyen dosya | yeni klasor indexine "kapsam disi" satiri | envantersiz dosya = bozuk link (G4) |

> **Kullanici onayi (bu promptun gecerlilik kosulu):** kaynak `C:\versacoder.1\.ai`
> bu is icin **acik kaynak** ilan edilmistir; §2 madde 1'deki byte kopya G1'in
> "eski vault'tan kopyalama" yasagini bu klasorler icin **gectersiz kilar**.
> Madde 2 ve 3 kapsam disindaki her kopya G1 ihlalidir ve geri alinir.

---

## §3 Hedef yapinin mevcut durumu (olcum 2026-10-04)

```text
KAYNAK  C:\versacoder.1\.ai  -> 16 klasor / 533 MD / 14 PS1 / 0 kok MD
HEDEF   C:\versacoder\.ai    -> 3 klasor / 56 MD / 771 link / 257 KIRIK (2026-10-04)
FARK    13 klasor + 14 PS1 eksik; hedefteki 3 klasor kaynakla birlestirilir
```

Kirik link dagilimi (olcum 2026-10-04, hedef bazli azalan): `.decisions/accepted/ADR-028 47 ·
checklists/layer-boundary-check 30 · .decisions/adr-index 20 · architecture/index 16 ·
reports/ 18 · .rules/index 9 · spec/index 6 · digerleri 1-4 arasi (toplam 257)`.

---

## §4 PROMPT (KOPYALA-YAPISTIR - tam metin)

> **ROL:** Vault ureticisi - kaynagi uyarlar, kok belgeye dokunmaz.
> **KONU:** `C:\versacoder.1\.ai` (kaynak) -> `C:\versacoder\.ai` (hedef)
>
> **GOREV:** Asagidaki 7 adimi sirayla uygula, `(n/7)` yaz. Once cevap:
> `VAULT: <faz adi>` + tek satir gerekce. Adim kanitsiz gecilmez.
>
> **GIRDILER (okuma sirasi):**
> 1. `C:\versacoder\.ai\CLAUDE.md` (G1..G12) + `C:\versacoder\.ai\WORKFLOW.md` (5 gate)
> 2. `C:\versacoder\.ai\AGENTS.md` (5 alanli kutu, 3 paralel ajan, 3-STRIKE)
> 3. `C:\versacoder.1\.ai\.templates\new-doc-contract.md` (frontmatter 8 alan)
> 4. `C:\versacoder.1\.ai\prompts\index.md` (kaynak prompt envanteri)
> 5. `C:\versacoder\AGENTS.md` (oturum boot, 78 satir) - OKUNUR, yazma YASAK;
>    root `CLAUDE/README/WORKFLOW.md` 0 bayt oldugu icin girdi sayilmaz
> 6. G: arsiv (salt okunur ornek): `...\Versacoder\new\versacoder-analiz-el10\`
>    92 part + `new\` icerigi 7 MD - yalnizca yapi referansi (G1)
> 7. Shell: `pwsh` KURULU DEGIL - tum komutlar `powershell -NoProfile
>    -ExecutionPolicy Bypass -File ...` ile kosulur (olcum 2026-10-04)
>
> **ADIMLAR (7):**
>
> | # | Adim | Kanit |
> |---|---|---|
> | 1 | Kaynagi salt okunur envanterle: klasor + MD sayisi (§1 tablosu) | dosya sayisi ciktisi |
> | 2 | 16 klasoru hedefte AC, iclerine `index.md` varsa kopyala | `Test-Path = True` (16 satir) |
> | 3 | Byte kopya paketini tasi: `.templates .rules .decisions checklists scripts .agents .sql` | kopyalanan dosya sayisi + `updated` tarih guncellemesi |
> | 4 | Uyarlama paketini tasi: `spec architecture knowledge playbooks prompts reports ui-design TODOS plans` - link yollarini hedefe gore duzelt, proje adlarini dogrula | her dosyada ilk satir `---`, link hedefi var |
> | 5 | `prompts/index.md` olustur; `keys.md` ve `index.md` §2 envanterini yeni klasorlerle guncelle (kok belgede silme yok, satir EKLEME) | envantere eklenen satir sayisi |
> | 6 | Olcum kos: `measure-vault.ps1`, `check-links.ps1`, `find-mojibake.ps1`, `count-components.ps1` | 4 komut ciktisi (sayilar) |
> | 7 | Eksikleri kapat: kirik link hedefi varsa duzelt, yoksa linki kaldir; sonra olcumu TEKRAR kos | 2. olcum ciktisi (BOZUK=0 hedefi) |
>
> **YASAKLAR (birine aykiriysa DUR ve raporla):**
> - 14 kok belgenin icigini degistirmek (G7 onayi olmadan) YASAK.
> - Klasor disina (`C:\versacoder.1` haric, `*.old.*`) yazmak YASAK (G1).
> - Frontmattersiz MD, placeholder (`{{...}}`), `WIP`, `TBD` YASAK (G4).
> - Unicode kutu cizim (`U+2500-U+257F`), emoji YASAK; kutu `+ - |` ile.
> - EF Core / string-concat SQL ornegi tasmak YASAK (G2).
> - `git add -A` ve `git commit` YASAK (ilk commit yalniz GATE-004'te).
> - Olculmemis sayi yazmak YASAK; sayac ciktisi yoksa `DOĞRULAMA GEREKLI` (G9).
> - Hedefteki mevcut `.templates prompts reports` dosyalarini ezme YASAK;
>   once farki goster, farkliysa G7 onayi bekle (madde 2/3 disi her yazim G1 ihlali).
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `VAULT: <faz>` + tek satir gerekce.
> 2. Faz tablosu: `# | adim | durum | kanit (komut ciktisi)`.
> 3. En fazla 5 madde: acik kalan eksik (`DOĞRULAMA GEREKLI` dahil).
> 4. 4 sayac sonucu: `MD=NNN, FM=0 eksik, BOZUK=NN, MOJIBAKE=0`.
> 5. 5 alanli ajan kutusu (AGENTS §2) - `KANIT` satiri sayi icerir.
>
> **KAPANIS:** 4 sayac hedefteyse GATE-004a kanit kutusunu hazirla ve
> **kullanici onayi bekle**; ajan kapiyi kapatmaz (G12).

---

## §5 Faz plani (6 faz, ayni anda en fazla 3 ajan)

| # | Faz | Kapsam | Tahmini | Cikis kosulu |
|---|---|---|---|---|
| 1 | Iskelet | 16 klasor + index dosyalari | ≈15 dk | `Test-Path` 16/16 True |
| 2 | Arac paketi | `.templates .rules .decisions checklists scripts .agents .sql` | ≈35 dk | 140 dosya kopyalandi, `updated` guncel |
| 3 | Bilgi paketi | `spec architecture knowledge` | ≈60 dk | 305 dosya, 0 kirik link |
| 4 | Uretim paketi | `playbooks prompts reports ui-design TODOS plans` + `prompts/index.md` | ≈45 dk | 104 dosya + index guncel |
| 5 | Navigasyon | hedef `index.md` §2, `keys.md` envanter satirlari | ≈20 dk | envantere eklenen satirlar |
| 6 | Olcum + onarim | 4 script + kirik link temizligi + tekrar olcum | ≈30 dk | BOZUK=0, MOJIBAKE=0 |

- **Batch kurali:** ayni anda en fazla 3 ajan; ayni dosyaya iki yazici yok (AGENTS §5).
- **Cakisma kurali:** hedefte mevcut 3 klasorde (`.templates prompts reports`)
  kopyalamadan once diff goster; ayni dosya farkliysa EZME, G7 onayina git.
- **Toplam:** ≈3 sa 45 dk (kaba); asim 20 dk'yi gecerse dur + raporla (WORKFLOW §2 giris).
- **Sira degistirilemez:** faz 2 tamamlanmadan faz 3-4 baslamaz - linkler hedefe kavusmadan uyarlama anlamsizdir.

---

## §6 Kabul olcutleri (GATE-004a girdisi)

| # | Sayac | Hedef | Komut |
|---|---|---|---|
| 1 | Toplam MD + eksik frontmatter | ≥ 500 MD · FM=0 eksik | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` |
| 2 | Bozuk link | **0** | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` |
| 3 | Mojibake | **0** | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` |
| 4 | Katalog kaydi | **PASS** (5440, marj 0) | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` |

Ek kosullar: 14 kok belge boyutu ve icerigi degismedi (boyut `Get-Item .Length`
ile karsilastirilir) · diyagramlarda Unicode kutu cizim **0 satir** · her yeni
MD'de frontmatter 8 alan (`title,type,category,status,version,updated,authority,scope`).

> **Gecici kanit (olcum 2026-10-04):** `pwsh` kurulu degil ve hedefte `scripts/`
> henuz yok (kaynakta 14 PS1). Faz 2 oncesi olcum bu sekilde alinir:
> `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder.1\.ai\scripts\check-links.ps1 -Root C:\versacoder\.ai`
> -> `Taranan MD 56 · Iyi link 514 · BOZUK link 257`. Faz 2'den sonra §6'daki
> hedef `scripts/` komutlari gecerli olur.

---

## §7 RED sinyalleri

| # | Sinyal | Neden | Duzeltme |
|---|---|---|---|
| 1 | Kok belge icerigi degismis | G7 + G1 | geri al; yalniz envantere satir ekle |
| 2 | BOZUK > 0 | G4 | hedefi olustur ya da linki kaldir, olcumu tekrar kos |
| 3 | `{{placeholder}}` / `WIP` | G4 | temizle ya da dosyayi eksik isaretle |
| 4 | Kaynakla ayni cumle + hedefe uyarlanmamis link | G1 (madde 2 disi) | linki hedefe gore duzelt |
| 5 | Kanitsiz "bitti" | G5 | 4 sayacin ciktisini yaz, yoksa `DOĞRULAMA GEREKLI` |

---

## §8 Butce

| Kalem | Butce | Asiminda |
|---|---|---|
| Faz 1-2 (iskelet + arac) | ≈50 dk | dosya sayisini 3'e bol |
| Faz 3-4 (bilgi + uretim) | ≈105 dk | klasor bazli gec, ayni anda 3 ajan |
| Faz 5-6 (navigasyon + olcum) | ≈50 dk | olcumu atlama - kanit burada uretilir |
| Tek ajan ciktisi | ≈8 KB | detayi `MEMORY.md`'e tasi |

---

## §9 Kanit kutusu (faz kapanisinda)

```text
+--------------------------------------------------+
| BASLIK : vault uretimi <faz> tamamlandi          |
| KAPSAM : .ai/<klasorler> (dosya sayisi ile)      |
| KARAR  : arac byte kopya, bilgi uyarlama (S2)    |
| RİSK   : <en fazla 1 madde; yoksa "yok">         |
| KANIT  : measure -> MD=NNN, FM=0 | links -> 0    |
|          | mojibake -> 0 | count -> PASS         |
+--------------------------------------------------+
```
