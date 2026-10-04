---
title: "VersaCoder — Proje Iskeleti Raporu (PART 5 / GATE-004)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "PART 5 hedef iskelet kararlari (slnx, CPM, proje agaci) ve bu vault'taki uygulama durumu - kanit yoksa satir DOĞRULAMA GEREKLI kalir"
---

# VersaCoder — Proje Iskeleti Raporu (PART 5)

**Zorunlu Baglantilar:** [index](index.md) · [00-kapsam-analizi](00-kapsam-analizi.md) · [01-web-kaynaklari](01-web-kaynaklari.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [spec/index](../spec/index.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *PART 5 hangi cozum/proje dosyalarini uretecek, hangi paket surumlerine kilitleyecek ve kanit ne zaman yapistirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Bu rapor HENUZ UYGULANMADI durumunu yazar: `C:\versacoder` kokunde 2026-09-30 olcumunde yalniz `.ai/` ve `.git/` var — `src/`, `tests/`, `VersaCoder.slnx`, `Directory.Build.props`, `Directory.Packages.props` diskte degil.** Kaynak vault'ta kararlastirilan hedef iskelet asagida aynen aktariliyor (2 uygulama + L0..L7 + 3 test projesi, CPM tek surum kaynagi); bu vault'a gore durum **`DOĞRULAMA GEREKLI` / bekliyor**. Build kanit kutulari bos: `[DOTNET_BUILD_CIKTISI]` yapistirilmadi, ilk git commit yok → **GATE-004 kapali**; kanitsiz "build gecti" yazilmadi (G5). GATE-004a da acik: MD **444/500** (2026-09-29), bkz. [07-dogrulama-raporu](07-dogrulama-raporu.md).

---

## §1 Hedef Proje Agaci (karar — uygulanmadi)

```text
C:\versacoder\
+-- VersaCoder.slnx
+-- Directory.Build.props          (TFM: net48 + net10.0-windows, LangVersion)
+-- Directory.Packages.props       (CPM - tek surum kaynagi)
+-- src\
|   +-- VersaCoder.Desktop\        (WinForms - RibbonForm / Office2010)
|   +-- VersaCoder.Cli\            (konsol - ayni katmanlari kullanir)
|   +-- VersaCoder.Abstractions\   L0
|   +-- VersaCoder.Domain\         L1
|   +-- VersaCoder.Application\    L2
|   +-- VersaCoder.CrossCutting\   L3
|   +-- VersaCoder.Infrastructure\ L4  (ADO.NET + Dapper, SqlClient 5.2.x)
|   +-- VersaCoder.Protocol\       L5  (tool/agent sozlesmesi)
+-- tests\
    +-- VersaCoder.UnitTests\  +-- VersaCoder.IntegrationTests\  +-- VersaCoder.ArchTests\
```

> `#if` **yalniz** platform farkinda; TFM gerekcesi her seferinde yazilir (CLAUDE.md G6). Katman ve proje sayisi [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) ile kilitli; `.slnx` + CPM karari ADR-014'tedir (dosya adi kayit, link `DOĞRULAMA GEREKLI`).

---

## §2 Paket / Surum Kilidi (CPM)

| Paket | Surum | Kaynak | Hedef bacak |
|---|---|---|---|
| DevExpress Universal | **v26.1** tek surum; ribbon yalniz `RibbonControlStyle.Office2010` | ADR-004 · `.rules/devexpress-rules.md` | her iki |
| `Microsoft.Data.SqlClient` | **5.2.x** | ADR-012 · 01-web §2 | her iki |
| Dapper (+ ADO.NET) | micro-ORM; **EF Core 0 referans** | ADR-005 · ADR-016 | her iki |
| `Microsoft.Data.Sqlite` | net48 + net10 ikili | **DOĞRULAMA GEREKLI** → NuGet teyidi | — |
| xUnit + NSubstitute + NetArchTest | CPM'te tek yerden | `spec/test-strategy.md` | test |

**Yasaklar:** EF Core · string-concat SQL (`@p1` parameterized zorunlu) · DevExpress v26.1 disi · Office2010 disi ribbon.

---

## §3 Bu Vault'taki Durum (2026-09-30 olcumu)

| # | Cikti | Beklenen dosya | Diskte | Durum |
|---|---|---|---|---|
| 1 | Cozum dosyasi | `VersaCoder.slnx` | yok | **DOĞRULAMA GEREKLI** |
| 2 | Ortak ozellik | `Directory.Build.props` | yok | **DOĞRULAMA GEREKLI** |
| 3 | Paket kilitleri | `Directory.Packages.props` (CPM) | yok | **DOĞRULAMA GEREKLI** |
| 4 | Iki uygulama | `VersaCoder.Desktop` · `VersaCoder.Cli` | yok | **DOĞRULAMA GEREKLI** |
| 5 | Katman + test projeleri | L0..L7 + 3 test projesi | yok | **DOĞRULAMA GEREKLI** |
| 6 | **Ilk git commit** | GATE-004 kosulu | yok | bekliyor |

**Sart:** bu satirlarin hepsi `bekliyor` oldugu icin bu rapor "tamamlandi" diyemez; dolduracak olan `developer`, denetleyen `reviewer`'dir.

---

## §4 Build Kaniti (bos kutular)

| Bacak | Komut | Beklenen | Yapistirilacak cikti |
|---|---|---|---|
| `net48` | `dotnet build VersaCoder.slnx -f net48` | 0 hata | `[DOTNET_BUILD_CIKTISI]` |
| `net10.0-windows` | `dotnet build VersaCoder.slnx -f net10.0-windows` | 0 hata | `[DOTNET_BUILD_CIKTISI]` |
| Restore (CPM) | `dotnet restore` | NU hata 0, tek surum | `[KANIT_BEKLIYOR]` |

> **KURAL (G5):** bu kutular doldurulmadan bu raporda "build gecti" yazilmaz; ayrinti [07-dogrulama-raporu](07-dogrulama-raporu.md) §1'de yasar.

---

## §5 Kapi Durumu

| Gate | Kapanis kosulu | Bu vault'taki gercek | Durum |
|---|---|---|---|
| GATE-004a | ≥500 MD · 0 bozuk link · ≥5000 kayit · 0 mojibake | MD 444/500; link 0; mojibake 0 | **ACIK** (MD kriteri eksik) |
| GATE-004 | iki bacak build 0 hata + **ilk git commit** | `src/` yok, commit yok | **ACIK** |
| GATE-005 | `dotnet test` yesil + NetArchTest 0 | test yok | **ACIK** → [05-katman-kodlama-raporu](05-katman-kodlama-raporu.md) |

Kapanis listesi: [gate-004a-vault-kapanis](../checklists/gate-004a-vault-kapanis.md) · [checklists/index](../checklists/index.md).

---

## §6 Risk

| # | Risk | Etki | Kapanis kosulu |
|---|---|---|---|
| R1 | Iskelet kurulmadan kod denemesi | her dosya G12 disi is emri sayilir | once GATE-003 onayi, sonra `.slnx` |
| R2 | CPM'siz paket eklenmesi | surum sapmasi, iki bacak restore catlakligi | `Directory.Packages.props` tek kaynak |

---

## §7 Sure

| Is | Sahip | Tahmin |
|---|---|---|
| `.slnx` + `Directory.*.props` (CPM) kurulumu | architect | ≈30 dk |
| 2 uygulama + L0..L7 + 3 test projesi iskeleti | developer | ≈45 dk |
| Paket restore + iki bacak build kaniti | developer | ≈15 dk |
| **Toplam PART 5** | — | **≈90 dk** (WORKFLOW §2.5) |

---

## §8 Kapsam Disi

| Bu rapor NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| UI onayi ve wireframe durumu | [03-ui-design-raporu](03-ui-design-raporu.md) |
| Katman kodu ve test sonuclari | [05-katman-kodlama-raporu](05-katman-kodlama-raporu.md) |
| Vault olcumlerinin tamami | [07-dogrulama-raporu](07-dogrulama-raporu.md) |
| Mimari katman/proje karari | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](../architecture/index.md) |
| Kapsam disi isler | [00-kapsam-analizi](00-kapsam-analizi.md) |

---

## §9 Kanit

| # | Kanit | Kaynak | Olcum |
|---|---|---|---|
| 1 | `C:\versacoder` kokunde yalniz `.ai/` + `.git/` | dizin listesi, 2026-09-30 | `src/` = 0 |
| 2 | `.slnx`, `Directory.*.props` yok | dosya arama, 2026-09-30 | 0 dosya |
| 3 | Ilk git commit yok | `.git/` var, `src/` yok; GATE-004 kaydi | bekliyor |
| 4 | Vault MD 444/500 | `scripts/measure-vault.ps1` olcumu, 2026-09-29 | 444/500 |
| 5 | Build ciktisi yok | bu dosya §4 kutulari bos | `[DOTNET_BUILD_CIKTISI]` |

Kaynak dosya adlari: `04-proje-iskeleti-raporu.md` (bu dosya), `00-kapsam-analizi.md`, `01-web-kaynaklari.md`, `07-dogrulama-raporu.md`. Olcum araci: `.ai/scripts/measure-vault.ps1`.
