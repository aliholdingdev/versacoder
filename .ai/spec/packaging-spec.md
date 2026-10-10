---
title: "VersaCoder — Paketleme Şartnamesi (CPM / Kilit)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "NuGet Central Package Management, Directory.Build.props sorumluluğu ve sürüm kilidi (DevExpress v26.1 · SqlClient 5.2.x) — yeni paket eklemenin tek yolu"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Paketleme Şartnamesi (`spec/packaging-spec.md`)

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[architecture/index|architecture/index]] · [[spec/build-spec|build-spec]] · [[spec/release-spec|release-spec]] · [[spec/deployment-spec|deployment-spec]]

> **Tek cümlelik sorumluluk:** *Bir NuGet paketi nereye, hangi sürümle ve kimin onayıyla girer; proje dosyaları hangi ortak ayarları paylaşır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Tüm paket sürümleri **tek yerde** (`Directory.Packages.props`, CPM açık) yaşar; `.csproj` içinde sürüm yazmak **yasağıdır**. Kilidi korunan iki kritik paket: **DevExpress Universal 26.1** (ADR-004) ve **Microsoft.Data.SqlClient 5.2.x** (ADR-005/ADR-012); yeni paket eklemek **ADR + kullanıcı onayı (G8)** ister.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | `Directory.Packages.props` (sürümler) + `Directory.Build.props` (ortak MSBuild) | Paketlerin **derlenmesi**: [[spec/build-spec|build-spec]] |
| 2 | Sürüm kilidi ve floating yasağı (SEC-09) | Yayın/paket çıkarma: [[spec/release-spec|release-spec]] |
| 3 | Yeni paket ekleme akışı (ADR + G8) | Dağıtım artifact'i: [[spec/deployment-spec|deployment-spec]] |
| 4 | DevExpress v26.1 ve SqlClient 5.2.x kilidi | Kullanım kuralları: `.rules/devexpress-rules.md` |
| 5 | NuGet kaynakları ve restore davranışı | CI adımları: [[spec/build-spec|build-spec]] |

> **YASAK:** `.csproj`'te elle `Version=` · floating (`*`, `1.*`, version range) · CPM dışı ikinci sürüm kaynağı · DevExpress v26.1 dışı sürüm · EF Core paketi (G2).

---

## §2 Davranış

### §2.1 Dosya sorumlulukları

| Dosya | Ne taşır | Sahip | Değişiklik kuralı |
|---|---|---|---|
| `Directory.Packages.props` | **Tüm** paket sürümleri (`ManagePackageVersionsCentrally`) | architect | Yeni satır = ADR + G8 |
| `Directory.Build.props` | Ortak özellik: `Nullable`, `TreatWarningsAsErrors`, TFM listesi, analyzers | architect | `net48` + `net10.0-windows` birlikte |
| `Directory.Build.targets` | Ortak hedef/kontroller (yayın öncesi denetim) | architect | Hedef eklemek build spec'ine işlenir |
| `.csproj` | **Yalnız** `PackageReference` (sürüm yok) + proje referansı | developer | Sürüm yazan `.csproj` = RED |

### §2.2 Kilit tablosu

| Paket | Sürüm | Neden kilitli | Kırılırsa |
|---|---|---|---|
| `DevExpress.*` (Universal) | **26.1** tek sürüm | ADR-004; iki sürüm = G3 ihlali | Build geçersiz, `reviewer` RED |
| `Microsoft.Data.SqlClient` | **5.2.x** (aynı alt sürüm) | ADR-005/ADR-012; net48 + net10 uyumu | Veri katmanı davranışı sapar |
| `Microsoft.Data.Sqlite` | CPM'te tek değer | Test/yerel DB determinizmi | Testler farklı şemada çalışır |
| `Dapper` | CPM'te tek değer | Tek ORM dışı ince katman | DA-05 sapması |
| `xunit` / `NSubstitute` / `NetArchTest` | CPM'te tek değer | Test altyapısı tutarlılığı | GATE-005 kanıtı bozulur |

### §2.3 Yeni paket ekleme akışı

```
  1) ihtiyac var mi?  -- hayir --▶ YAGNI, ekleme
        |
      evet
        v
  2) BCL'de / platformda var mi? -- var --▶ stdlib'i kullan
        |
      yok
        v
  3) architect ADR acar (alternatif >= 2, +/- sonuc)
        v
  4) KULLANICI ONAYI (G8) -- red --▶ paket eklenmez
        v
  5) Directory.Packages.props'a TEK satir (sabit surum)
        v
  6) restore + build iki bacak --▶ log.md kaydi + rapor
```

| # | Denetim | Beklenen |
|---|---|---|
| 1 | `.csproj` içinde `Version=` araması | **0 satır** |
| 2 | `Directory.Packages.props` içinde floating deseni (`*`) | **0 satır** (SEC-09) |
| 3 | DevExpress sürüm satırı sayısı | **1** farklı değer: 26.1 (G3) |
| 4 | `EntityFrameworkCore` araması | **0 satır** (G2) |

---

## §3 TFM Notu

| Bacak | Paket etkisi | Kural |
|---|---|---|
| `net48` | Paket TFM'i `net48` ya da `netstandard2.0` destekli olmalı | Desteklemeyen paket (ör. modern EF) **giremez** (DA-02 gerekçesi) |
| `net10.0-windows` | Modern TFM | Aynı paket, aynı sürüm |
| Farklı sürüm ihtiyacı | — | ADR + G8; iki ayrı sürüm **asla** |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| PK-01 | CPM açık, tek sürüm kaynağı | `Directory.Packages.props` + restore çıktısı |
| PK-02 | `.csproj`'te elle sürüm yok | Tarama = 0 satır |
| PK-03 | Kilitli paketler tek sürümde | DevExpress=26.1, SqlClient=5.2.x tarama sonucu |
| PK-04 | İki bacak restore | `dotnet restore` + `dotnet build -f net48` / `-f net10.0-windows` = 0 hata |
| PK-05 | Kayıtsız paket yok | Paket listesi ↔ ADR listesi eşleşmesi (`reports/` notu) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
