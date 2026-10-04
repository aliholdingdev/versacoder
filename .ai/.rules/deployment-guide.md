---
title: "Dağıtım Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "İki bacak paketleme, DevExpress runtime şartları, CI matrisi, release adımları, rollback, secret dağıtımı"
---

# Dağıtım Kuralları (deployment-guide)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](coding-standards.md) · [performance-guidelines](performance-guidelines.md) · [security-rules](security-rules.md) · [test-strategy](../spec/test-strategy.md) · [ADR-004](../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [ADR-005](../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cümlelik sorumluluk:** *Hangi bacak, hangi araçla, hangi **CI adımı** sonunda ve nasıl **geri alınır*** sorusunun tek cevabıdır.
> Dağıtım = kod değil, **kanıttır**: her adım `reports/` altına çıktı bırakır (ROLE.md §6).

---

## §0 Özet (Answer First)

**İki bacak ayrı publish edilir:** `net48` → self-contained değil (FX kurulu makine), `net10.0-windows` → `win-x64` RID. CI matrisi 4 hücre (2 TFM × {build, test}); DevExpress v26.1 runtime DLL'leri çıktının yanında gelir. DB dağıtımı **saf SQL migration** + `schema_version` (EF yok). Rollback = önceki sürüm artıfatı + migration geri alma scripti.

---

## §1 Dağıtım matrisi

| # | Bacak | Yayın biçimi | RID | Çıktı | Not |
|---|---|---|---|---|---|
| P1 | `net48` | `dotnet publish -f net48` | `win-x86/x64` | `publish/net48/` | .NET Framework 4.8 kurulu olmalı |
| P2 | `net10.0-windows` | `dotnet publish -f net10.0-windows -r win-x64 --self-contained false` | `win-x64` | `publish/net10.0-windows/` | runtime dağıtım tercihen framework-dependent |
| P3 | CLI | `VersaCoder.Cli` iki bacakta da | aynı | `publish/<tfm>/cli/` | GUI'siz ortamlar için |
| P4 | DevExpress | v26.1 assembly'leri çıktının yanında | — | otomatik | lisan/`DesigtimeLicenseProvider` yok |
| P5 | Veri | `.sql/migrations/NNN-*.sql` paketlenir | — | `publish/sql/` | `IMigrationRunner` açılışta uygular |

> **YASAK:** DevExpress sürümü dışında referans; `net48` çıktısını .NET 10 runtime'ında çalıştırmak; migration'ı uygulama dışında elle uygulamak.

---

## §2 CI matrisi (build → test → publish)

```
+------------------------------------------------------------------------+
|  CI PIPELINE  (her hücre ayrı job; 4 hücre yeşil olmadan publish YOK) |
+------------------------------------------------------------------------+
|                                                                        |
|  +--------------+   +--------------+   +--------------+   +---------+  |
|  | 1 restore    |--▶| 2 build      |--▶| 3 test       |--▶|4 publish|  |
|  | CPM (nuget)  |   | -f net48     |   | -f net48     |   | iki TFM |  |
|  |              |   | -f net10.0   |   | -f net10.0   |   |         |  |
|  +--------------+   +------+-------+   +------+-------+   +----+----+  |
|                            |                  |                |       |
|                     TreatWarningsAsErrors  coverage>=90%   artifact    |
|                            |                  |                |       |
|                            ▼                  ▼                ▼       |
|                     GATE-004 (yeşil)   GATE-005 (yeşil)   sürüm etiketi|
|                                                                        |
|  Ek adımlar:  scripts/check-links.ps1 (0 bozuk link) · NetArchTest     |
+------------------------------------------------------------------------+
```

| Adım | Komut | Şart | Kanıt |
|---|---|---|---|
| Restore | `dotnet restore VersaCoder.slnx` | 0 hata | NuGet log |
| Build | `dotnet build -c Release` iki `-f` | 0 hata/uyarı | build çıktısı (GATE-004) |
| Test | `dotnet test` iki `-f` | 0 failed, ≥ %90 | TRX/konsol (GATE-005) |
| Link | `scripts/check-links.ps1` | 0 bozuk | konsol (GATE-004a) |
| Publish | `dotnet publish` iki TFM | artifact + checksum | sürüm notu |

---

## §3 Ortam ve secret

| # | Kural |
|---|---|
| S1 | Connection string **düz metin dosyada durmaz** — DPAPI (`ProtectedData`) ile şifreli saklama (ADR-005) |
| S2 | CI secret'ları değişken ortamdan okunur; repo'ya commit **yasak** (`.gitignore`: `*.secret`, `appsettings.Local.json`) |
| S3 | `keys.md` yalnız **anahtar kelime haritası**; gerçek anahtar oraya yazılmaz (PART 7) |
| S4 | LLM API anahtarı ilk çalıştırmada prompt ile alınır, secret store'a yazılır |

---

## §4 Release ve rollback

| Aşama | Eylem | Geri alma |
|---|---|---|
| R1 | Sürüm etiketi (`vMAJOR.MINOR.PATCH`) + `reports/` changelog | etiketi kaldır |
| R2 | İki TFM artifact'i + `sql/` + checksum | önceki artifact |
| R3 | DB migration **önce yedek alır** (`schema_version` + `backup/`) | `NNN-*.down.sql` (varsa) yoksa yedekten geri yükle |
| R4 | Açılış doğrulama (health: host ≤ 2 s) | önceki sürümü başlat |

> **KURAL:** Geri alma planı **yayından önce** yazılır; planı olmayan release = `GATE-006` RED.

---

## §5 Kabul kriterleri

- [ ] İki TFM publish çıktısı artifact'te
- [ ] CI 4 hücre yeşil (build + test × 2 TFM)
- [ ] `check-links.ps1` → 0 (GATE-004a)
- [ ] Coverage ≥ %90 (GATE-005)
- [ ] Geri alma planı release notunda yazılı (GATE-006)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (devops-engineer).
