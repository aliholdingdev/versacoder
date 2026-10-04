---
title: "ADR-012 — İki Bacakta Veri Sağlayıcı (SqlClient 5.x + Sqlite)"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "iki bacakta veri saglayici: SqlClient 5.x + Sqlite secimi, baglanti ve sinir katmani etkisi"
---

# ADR-012 — İki Bacakta Veri Sağlayıcı

**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) · [data-access-rules](../../.rules/data-access-rules.md) · [deployment-guide](../../.rules/deployment-guide.md) · [test-strategy](../../spec/test-strategy.md)

> **Tek cümlelik karar:** SQL Server sağlayıcısı **`Microsoft.Data.SqlClient` 5.x — her iki bacakta aynı major**; yerel/CI verisi **`Microsoft.Data.Sqlite`** ile tek bacakta **`net10.0-windows`** üzerinde doğrulanır; EF Core yok (ADR-005).

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | `net48` + `net10.0-windows` iki bacakta da `.csproj` aynı paketi referanslamalı (CPM tek noktada) | ADR-001 · `WORKFLOW.md` PART 5 |
| C2 | `Microsoft.Data.SqlClient` **6.x** .NET Standard desteğini düşürdü → `net48` bacağı kırılır | `reports/01-web-kaynaklari.md` C5 |
| C3 | Dapper her iki bacakta aynı sürümde (`2.x`) çalışır | ADR-005 |
| C4 | `Microsoft.Data.Sqlite` sürümlerinin `net48` uyumu **web'den doğrulanmadı** | `reports/01-web-kaynaklari.md` C6 → **NEEDS_VALIDATION** |
| C5 | EF Core yasağı kalıcı (ADR-005 · G2) | `CLAUDE.md` G2 |

---

## §2 Karar

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | SQL Server | `Microsoft.Data.SqlClient` **5.2.x** (CPM'te tek sürüm, iki bacak aynı) | C2 |
| D2 | Dapper | `2.1.x` — her iki bacak | ADR-005 |
| D3 | SQLite | `Microsoft.Data.Sqlite` **8.0.x** — **yalnız `net10.0-windows` entegrasyon testlerinde** | C4 |
| D4 | `net48` SQLite | **YOK** (test edilmemiş kombinasyon kodda kullanılmaz); `net48` veri testleri SqlServer + sahte sağlayıcı ile | C4 |
| D5 | Sahte DB | **yok** — entegrasyon testi gerçek şema üzerinde (ADR-005) | `spec/test-strategy.md` T6 |
| D6 | Şema | saf SQL migration + `schema_version` + `IMigrationRunner` | ADR-005 |

**Nihai `NEEDS_VALIDATION` maddesi (PART 5'te kapatılır):**
`Microsoft.Data.Sqlite` `net48` bacağında derleniyor mu? → `dotnet build -f net48` + `Directory.Packages.props` teyidi → **Paket sürümü doğrulanırsa D4 gevşetilir; doğrulanmazsa D4 kalır.**

---

## §3 Alternatifler

| # | Alternatif | Artı (+) | Eksi (−) | Neden reddedildi |
|---|---|---|---|---|
| A1 | SqlClient **6.x** | Yeni özellikler | .NET Standard 6.x'te düşürüldü → `net48` restore/derleme kırılır | İki bacak şartı (ADR-001) |
| A2 | **System.Data.SqlClient** (Framework yerleşik) | `net48`'de ek paket yok | `net10.0-windows` bacağında ayrı paket + farklı davranış → iki kod yolu | Tutarlılık (`#if` yalnız platform değil, sağlayıcı da olurdu) |
| A3 | **EF Core** | Migration/query builder | **yasak** (G2 · ADR-005) + `net48` sürüm karmaşası | ADR-005 reddediyor |
| A4 | SQLite **tek sağlayıcı** (SQLite everywhere) | Tek paket, dosya kolaylığı | Çok kullanıcılı/lock davranışı, `net48` desteği doğrulanmadı, gerçek hedef SQL Server | C4 + kapsam |
| A5 | İki bacakta **farklı** SqlClient sürümü (CPM koşullu) | Her bacak en yeni sürüm | CPM'te koşullu sürüm = iki farklı Dapper/SqlClient davranışı, `#if` artar | Sürüm kilidi (G8) + test yüzeyi |

---

## §4 Sonuçlar

**Artı (+)**

1. Tek paket + tek sürüm → `Directory.Packages.props` tek satır, **iki bacak aynı SQL davranışı**.
2. `net48` restore riski ortadan kalkar (6.x'in NS düşürme problemi).
3. SQLite yalnız test ortamında → üretim davranışı **tek gerçek** sağlayıcıda.
4. `#if` yalnız platform köprüsünde kalır (ADR-001 ile uyumlu).

**Eksi (−) / Riskler**

1. SqlClient 5.x **security fix** penceresi izlenmeli → `deployment-guide` §5 / NuGet denetimi.
2. `net48` + SQLite kombinasyonu **bilinmiyor** → D4 ile kapsam dışı; PART 5 teyidi gerekli (**NEEDS_VALIDATION**).
3. SqlClient 5.x sürümü teyit edilmeden kilitlenirse GATE-004'te sürpriz restore hatası → PART 5'te ilk iş `dotnet restore`.

---

## §5 Kabul Kriterleri

- [x] Karar + 5 alternatif + (+)/(−) yazıldı
- [x] `NEEDS_VALIDATION` maddesi açıkça tanımlandı (D4 / §2)
- [ ] PART 5: `dotnet restore` + `build -f net48` + `build -f net10.0-windows` yeşil (GATE-004)
- [ ] `Directory.Packages.props` sürümü `reports/` raporuna yapıştırıldı
- [ ] `.rules/data-access-rules.md` §sürüm satırı ile uyum
- [ ] EF Core = 0 referans (NetArchTest / NuGet taraması)

**İlgili:** [ADR-001](ADR-001-dual-target-net48-net10.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
