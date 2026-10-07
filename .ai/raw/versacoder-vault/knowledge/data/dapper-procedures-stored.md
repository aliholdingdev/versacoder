---
title: "dapper-procedures-stored - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Depolama proseduru politikasi ve Dapper ile calistirma deseni; SQLite test bacakta uyum notu"
---

# dapper-procedures-stored — Depolama Prosedürleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [data index](../index.md) · [ado-net-command](ado-net-command.md)

> **Tek cümlelik sorumluluk:** *VersaCoder'da depolama prosedürü kullanılır mı; kullanılırsa Dapper ile nasıl çağrılır ve iki sağlayıcıda nasıl taşınır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**Varsayılan: hayır.** İş mantığı prosedüre taşınmaz; sorgu gövdesi repository'de sabit, parametreli SQL olarak durur (DA-06) — çünkü test sağlayıcısı **SQLite'tır ve SQLite'ın depolama prosedürü yoktur** (ADR-012 D3). Prosedür zorunlu bir kurumsal istekse çağrım deseni `CommandType.StoredProcedure` + parametreli `DynamicParameters` + açık `CommandTimeout`'tur ve bu, sahte/denetlenebilir bir **dialect köprüsü** ile repository arkasında gizlenir (**NEEDS_VALIDATION**: prosedürle eşdeğer SQLite dalı mimari karar ister).

---

## §1 Doğru kullanım (SQL Server dalı)

```csharp
const string proc = "usp_Sessions_ListRecent";

var p = new DynamicParameters();
p.Add("Top", take, DbType.Int32);
p.Add("Since", sinceUtc, DbType.DateTime2);

var rows = await connection.QueryAsync<SessionRow>(
    new CommandDefinition(proc, p, commandType: CommandType.StoredProcedure,
        commandTimeout: DbTimeouts.Report, cancellationToken: cancellationToken));
```

---

## §2 Politika tablosu

| Durum | Karar | Gerekçe |
|---|---|---|
| Yeni iş | **parametreli metin SQL** | taşınabilir, denetlenebilir (DA-03) |
| İş kuralı mantığı | repository/servis katmanında | prosedüde iş mantığı = kör kod |
| Şema değişikliği | migration dosyası (DA-07) | `ALTER PROCEDURE` kodda yasak |
| Kurumsal zorunluluk | `CommandType.StoredProcedure` + desen | tek noktada gizleme |
| SQLite entegrasyon testi | aynı repo imzası, diallekt gövde | ADR-012 D3 |

---

## §3 Çağrım kuralları

| # | Kural | Yasak |
|---|---|---|
| 1 | Prosedür adı **sabit metin** (`const string`) | adı interpolasyonla kurmak |
| 2 | Tüm argümanlar parametre (`DynamicParameters`) | `$"usp_X {deger}"` |
| 3 | `CommandTimeout` açık (`Report` 60 sn) | `0` |
| 4 | Dönüş değeri/çıktı parametresi `Get<T>` ile alınır | sessiz `DBNull` |
| 5 | Prosedür sürümü migration'da (`CREATE OR ALTER`) | kod içinde `ALTER` |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CommandType.StoredProcedure` | var (SQL Client 5.2.x) | var (aynı paket) |
| `Microsoft.Data.Sqlite` | **yok** (ADR-012 D4) — prosedür çağrısı **imkânsız** | prosedür **yok** → metin SQL dalı |
| Çift implementasyon | yalnız `#if` değil, **dialect seçimi** ile | aynı |
| `SqlParameter` yönlendirme | `DbType` + `ParameterDirection` | aynı |

> **NOT:** Prosedür çağrısı ile metin-SQL dalı arasındaki fark `#if` ile değil, **`ISqlDialect`/repository seçimi** ile yönetilir; `#if` yalnız platform API farkı içindir (ADR-001).

---

## §5 Kardeşler

[ado-net-command](ado-net-command.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)
