---
title: "TODOS Faz 03 — Faz D: C:\\versacoder Kod İskeleti (GATE-004)"
type: todo
category: todos
status: blocked
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "9+2 proje iskeleti, CPM, TFM net48;net10.0-windows, NetArchTest, ilk commit adimlari — GATE-004a onayindan sonra acilir"
tags: [ops]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Faz 03 — Faz D: Kod İskeleti (GATE-004)

**Zorunlu Bağlantılar:** [[ops/TODOS/index|TODOS index]] · [[ops/TODOS/01-vault-kapanis|Faz 01]] · [[checklists/gate-004-ilk-commit|gate-004-ilk-commit]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[checklists/dual-tfm-build-check|dual-tfm-build-check]] · [[checklists/layer-boundary-check|layer-boundary-check]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]]

> **Tek cümlelik sorumluluk:** `C:\versacoder` içine ADR-028'e uygun 9+2 proje

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: BLOKE — src/ · tests/ · .sln YOK**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| src/ (9 uretim projesi) | **YOK** | 9 proje | BLOKE |
| tests/ (2 test projesi) | **YOK** | 2 proje | BLOKE |
| VersaCoder.slnx | **YOK** | 1 cozum | BLOKE |
| global.json · Directory.Packages.props | **YOK** | VAR | BLOKE |
| git tracked src dosyasi | **0** | — | GATE-004 henuz acilmadi |
| dotnet --version | 10.0.301 | 10.x | OK (SDK hazir) |

> GATE-004 on kosulu: GATE-003 teyidi (faz 04) + onay. Kod yazimi G12 ile kapali degil, ama iskelet kurulmadan commit atilamaz.

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).

> iskeletinin nasıl kurulacağının ve ilk commit'in hangi kapıdan geçeceğinin tek listesidir.

**Durum: KAPALI** (01 GATE-004a onayı + 02 cevapları gelmeden açılmaz).
**HARD STOP:** bu fazda **tek satır iş mantığı yazılmaz** — yalnız iskelet.

---

## §0 Özet (Answer First)

| Proje | Katman | TFM | Rol |
|---|---|---|---|
| `VersaCoder.Abstractions` | l0 | `net48;net10.0-windows` | portlar/arayüzler |
| `VersaCoder.Domain` | l1 | çift | varlıklar, kurallar |
| `VersaCoder.Application` | l2 | çift | handler/use-case |
| `VersaCoder.Crosscutting` | l3 | çift | log/cache/saat/config |
| `VersaCoder.Infrastructure` | l4 | çift | ADO.NET+Dapper, fs, HTTP |
| `VersaCoder.Protocol` | l5 | çift | tool/agent şeması |
| `VersaCoder.Host` | l6 | çift | DI, bootstrap |
| `VersaCoder.Desktop` | l7 | çift | WinForms (Exe) |
| `VersaCoder.Cli` | l7 | çift | konsol (Exe) |
| `VersaCoder.Tests` + `VersaCoder.ArchitectureTests` | test | çift | test + NetArchTest |

- **CPM zorunlu:** `Directory.Packages.props` + `Directory.Build.props` (ADR-028 §2.3 madde 1).
- **Yasak:** EF Core, `net8` tek TFM, DevExpress v26.1 dışı sürüm, `Directory.Packages.props` yoksa commit.

---

## §1 Adımlar (sırayla)

| # | Adım | Komut/İş | Kanıt |
|---|---|---|---|
| 1 | Çözüm + CPM | `sln` + `Directory.Build.props` + `Directory.Packages.props` | dosya diskte |
| 2 | 9 üretim projesi | `dotnet new classlib/winexe/console` ×9 | `dotnet sln list` = 11 |
| 3 | TFM çift bacak | her `.csproj`: `<TargetFrameworks>net48;net10.0-windows</TargetFrameworks>` | `dotnet build` 0 hata |
| 4 | Proje referansları | ADR-028 yönü (lX → yalnız < X, izinli matris) | `dotnet list reference` |
| 5 | Test paketleri (CPM) | `Microsoft.NET.Test.Sdk`, `xunit`/`MSTest`, `NetArchTest`, `Dapper` | versiyonlar CPM'de tek nokta |
| 6 | 6 düzeltme | ADR-028 §2.3 (yön matrisi `l4→l2 ✗`/`l5→l4 ✗`, "sekiz", N↔klasör, `src\` kanıtı, TFM/portlar) | checklist işaretli |
| 7 | İlk build | `dotnet build -m` | 0 hata |
| 8 | İlk test | `dotnet test` | yeşil (boş da olsa 0 hata) |
| 9 | NetArchTest iskeleti | 8×8 matris = 0 ihlal | ArchitectureTests yeşil |
| 10 | İlk commit | gate-004 maddeleri | commit hash |

- Adım 3 RED ise **4'e dön** (yanlış referans yönü en sık hata).
- Adım 6, mimari doküman ile proje dosyası arasındaki **çelişkiyi** kapatır — atlanamaz.

---

## §2 Yasaklar (bu fazda)

| # | Yasak | Neden |
|---|---|---|
| 1 | İş mantığı / gerçek kod | iskelet fazı; kod = faz 05 |
| 2 | EF Core paketi | ADR-005 G2, build RED |
| 3 | Tek TFM (`net10.0-windows` yalnız) | ADR-001 ihlali |
| 4 | DevExpress v26.1 dışı | ADR-004 G3 |
| 5 | `Directory.Packages.props` olmadan commit | ADR-028 §2.3 madde 1 |

---

## §3 Kabul Kriterleri (gate-004)

- [ ] 11 proje `dotnet sln list` görünüyor
- [ ] Her proje çift TFM; `dotnet build -m` **0 hata**
- [ ] CPM tek noktada; paket versiyonu `.csproj` içinde tekrar yok
- [ ] Proje referansları ADR-028 yön matrisine uygun (ters ok 0)
- [ ] `NetArchTest` 8×8 = **0 ihlal**
- [ ] [[checklists/dual-tfm-build-check|dual-tfm-build-check]] tümü [x]
- [ ] [[checklists/gate-004-ilk-commit|gate-004-ilk-commit]] [x]
- [ ] İlk commit atıldı (hash `log.md`'de)

---

## §4 Bağımlılıklar (ne biterse açılır)

| # | Bağımlılık | Nereden |
|---|---|---|
| 1 | GATE-004a onayı | [[ops/TODOS/01-vault-kapanis|Faz 01]] |
| 2 | 4 cevap (özellikle birim/yer) | [[ops/TODOS/02-5000-plani-sorulari|Faz 02]] |
| 3 | ADR-028 §2.3 6 düzeltme | vault (kabul edildi) |
| 4 | Paket sürüm teyidi (`Microsoft.Data.Sqlite` net48?) | `reports/10-bilinen-eksikler` NEEDS_VALIDATION 1 |

- Madde 4 açık kalmışsa **`DOĞRULAMA GEREKLI`** ile ilerle, sürümü tahmin etme.

---

## §5 Kanıt

| # | Kanıt | Beklenen |
|---|---|---|
| P3-1 | `dotnet sln list` | 11 proje |
| P3-2 | `dotnet build -m` | 0 hata |
| P3-3 | `dotnet test` | yeşil |
| P3-4 | NetArchTest çıktısı | 0 ihlal (8×8) |
| P3-5 | commit hash | `log.md` kaydı |
| P3-6 | `check-links.ps1` | vault hâlâ 0 bozuk |
