---
title: "N25 — ADO.NET + Dapper (l4 Infrastructure)"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "N25 dugumu: SQL yurutme cekirdegi - sorumluluk, giren/cikan, ADR-028 yasak yonleri, bagimlilik ve test stratejisi (kayit tablosu DEGIL)"
---

# N25 — ADO.NET + Dapper

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [kardes N26](N26-sqlite-provider.md)

> **Tek cumlelik sorumluluk:** *Bir sorgunun use case'ten veritabanına kadar **nasıl imzalandığı, hangi sırayla yürüdüğü ve hangi Result'a dönüştüğü*** sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

N25, l4'ün **SQL yürütme çekirdeğidir**: sorgu metni **her zaman sabittir**, değerler yalnızca parametre nesnesiyle taşınır ve her `DbCommand`'a zorunlu `CommandTimeout` yazılır (varsayılan 30 sn, migration/rapor 120 sn). `IDapperSession` üç imzayla çalışır: `QueryAsync<T>`, `ExecuteAsync`, `ExecuteScalarAsync` — hepsi `(string sql, object p, CancellationToken ct)` alır. String birleştirme ile SQL üretimi derleme geçse bile **RED**dir (G5).

---

## §1 Sorumluluk

| # | Yapar | Kanıt | Çıktı |
|---|---|---|---|
| 1 | Parametreli sorgu çalıştırma (`@p1..@pN` adlandırması) | `' OR 1=1--` değeri veri döndürmez | `IReadOnlyList<T>` |
| 2 | Şifre çözülmüş `DbConnection` üretimi (`IDbConnectionFactory`) | bağlantı testte kapanır | `DbConnection` |
| 3 | Açık kolonlu sorgu kurma, `SELECT *` üretmeme | kolon listesi sabit kodda | `ISqlCommandBuilder` |
| 4 | Tek transaction teknesi (`ITxWrapper`) | rollback en fazla 1 | `Result<bool>` |
| 5 | Süre + satır sayacı logu (`IDbLogger`, hassas değer yok) | log satırı üretilir | log |

**Yapmaz:** ekran/dosya/süreç çalıştırmaz, mesaj şeması doğrulamaz, DI kaydetmez.

---

## §2 GIRENLER

| # | Giren | Kaynak | Koşul |
|---|---|---|---|
| 1 | `string sql` (literal/const) | `l2` use case yerine `l4` içi şablon | concat yasak |
| 2 | Parametre nesnesi | tip güvenli DTO/anonim nesne | null alan `NULL` olur |
| 3 | `CancellationToken` | üst katman | iptal → `CANCELLED`, retry değil |
| 4 | Bağlantı ayarı | `l6` N40 (DPAPI çözülmüş) | düz metin loglanmaz |
| 5 | Şema sürümü | N28 migration | uyuşmazlık → `SCHEMA_DRIFT` |

---

## §3 CIKANLAR

| # | Çıkan | Alıcı | Hata yolu |
|---|---|---|---|
| 1 | `Result<T>` (başarılı) | `l2` | — |
| 2 | `Result.Fail(DB_TIMEOUT/DB_AUTH/DB_CONSTRAINT/DB_BUSY)` | `l2`/`l6` | ham istisna çıkmaz |
| 3 | `Result.Fail(SCHEMA_DRIFT)` | `l6` | deployment durur |
| 4 | Log kaydı | `l3` `ILogSink` | süre + satır sayısı |
| 5 | Yeni hata kodu talebi | **l0'a** eklenir | tabloda karşılığı yoksa RED |

---

## §4 Yasak Yonler (ADR-028 matrisi)

| Yön | Değer | Not |
|---|---|---|
| `l4 -> l2` | **X** | use case'leri tanımaz |
| `l4 -> l5`, `l6`, `l7` | **X** | şema, DI, ekran yok |
| `l2 -> l4` | **YOK** | port `l0`'da, uygulama `l4`'te |
| `l4 -> l0` | `?` | portlar `l0`'a yazılıyor → `DOĞRULAMA GEREKLİ` |
| Test → üretim | **X** | üretim kodu test katmanına bakamaz |

```text
 UseCase (l2) --l0 port--> IDapperSession (N25) --DbCommand(30sn)--> DbConnection
                                   |
                                   +--> Result<T> | Result.Fail  (ham istisna YOK)
```

---

## §5 N Kodlari

| N | Alt katman | İlişki |
|---|---|---|
| N25 | ado-dapper (bu dosya) | yürütme çekirdeği |
| N26 | sqlite-provider | varsayılan bağlantı |
| N27 | sqlserver-provider | kurumsal bağlantı |
| N28 | migrations | şema + `SCHEMA_DRIFT` kaynağı |
| N49 | makro l4 | `IInfrastructureCore` kit'i |

---

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Infrastructure` | `net48;net10.0-windows` | `DapperSession : IDapperSession, IDisposable` |
| `VersaCoder.Tests` | çift bacak | injection + timeout testleri |

`net48` C# 7.3 alt kümesi (nullable anotasyonu yok); `net10.0-windows` C# 14 + `#nullable enable`. Dapper **aynı sürüm** iki bacakta.

---

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tür |
|---|---|---|
| N25 | `l0` portları (`IDbConnectionFactory`, `Result`) | izinli |
| N25 | N26/N27 bağlantı nesneleri | katman içi |
| N25 | N28 `MigrationReport` | katman içi |
| N25 | `l2`, `l5`, `l6`, `l7` | **yasak** |
| N25 | EF Core / `DbContext` | **yasak** (ADR-005) |

Paketler: Dapper + `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` — sürüm tek kaynağı `Directory.Packages.props`.

---

## §8 Test Stratejisi

| # | Test | Tip | Beklenen |
|---|---|---|---|
| 1 | `' OR 1=1--` değeri satır döndürmez | birim | **RED** ile başlar |
| 2 | Timeout'suz `DbCommand` kurulamaz | derleme | imza zorlar |
| 3 | `SELECT *` metni üretilmez | statik | 0 eşleşme |
| 4 | 6 istisna → 6 doğru `Result` kodu | birim | tablo güdümlü |
| 5 | `Cancel()` sonrası havuz temiz (50 tekrar) | entegrasyon | kaçak 0 |

---

## §9 Kanit

| # | İddia | Kanıt | Beklenen |
|---|---|---|---|
| 1 | Dosya 2.600–8.500 bayt | `Get-Item .Length` | aralık içinde |
| 2 | Kardes link N26 geçerli | `check-links.ps1` | bozuk 0 |
| 3 | Frontmatter 9 alan | gözden geçirme | 9/9 |
| 4 | `dotnet test --filter Category=Infrastructure` | test çıktısı | `src/` yokken `DOĞRULAMA GEREKLİ` |
| 5 | Concat SQL = 0 | kod taraması | 0 satır |
