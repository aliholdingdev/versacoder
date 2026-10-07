---
title: "null-handling-db-mapping - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "SQL NULL ile C# null/unbox iliskisi: nullable tipler, DBNull, varsayilan deger tuzagi, ISO-8601 tarih donusumu (DA-08)"
---

# null-handling-db-mapping — NULL ve Eşleme

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [data index](../index.md) · [dapper-query-mapping](dapper-query-mapping.md)

> **Tek cümlelik sorumluluk:** *Veritabanındaki `NULL`, C#'ta hangi tüpe düşer; `default` tuzağı nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`NULL` kolon → **nullable tip** (`string?`, `DateTime?`, `int?`); nullable olmayan bir tipte `default` oluşur (`0`, `0001-01-01`, `false`) ve bu **sessizce yanlış veri** üretir (DA-08). Dapper `DBNull.Value` dönüşümünü yapar; elle `if (x == DBNull.Value)` yazmak repository'de gereksizdir. Tarihler **UTC + ISO-8601** olarak saklanır, okumada `DateTimeKind.Utc` korunur (DA-15) — yerel saat karması raporları bozar.

---

## §1 Dogru kullanim

```csharp
public sealed class SessionRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;   // NOT NULL kolon
    public DateTime? DeletedAt { get; set; }            // NULL olabilir kolon
    public int RetryCount { get; set; }                 // DEFAULT 0 kolonu
}

var row = await connection.QuerySingleOrDefaultAsync<SessionRow>(sql, new { Id = id },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §2 Donusum tablosu

| SQL kolonu | C# tipi | `NULL` geldiğinde | Tuzak |
|---|---|---|---|
| `TEXT NOT NULL` | `string` | hata yerine `?? string.Empty` (bilinçli) | `null` sızması |
| `TEXT NULL` | `string?` | `null` | `""` ile `null` karışması |
| `datetime2 NULL` | `DateTime?` | `null` | `0001-01-01` |
| `INTEGER NOT NULL DEFAULT 0` | `int` | `0` | `null` beklemek |
| `INTEGER NULL` | `int?` | `null` | `default(int)` = `0` |
| `BIT NOT NULL` | `bool` | `false` | `true/false` anlamı kaybı |
| `BLOB` | `byte[]` | `null` / boş dizi | boyut tahmini |

---

## §3 Kural tablosu (DA-08 · DA-15)

| # | Kural | Yasak |
|---|---|---|
| 1 | Kolon nullable ise C# tipi `?` | `DateTime` + `default` gizleme |
| 2 | `SELECT` listesi satır tipiyle hizalı | eşleşmeyen kolonun sessizce atlanması |
| 3 | Tarih `DateTimeKind.Utc`, kaynak `TimeProvider` | `DateTime.Now` (CS/DA-15) |
| 4 | Metin `NOT NULL` + kod tarafında anlamlı varsayılan | `null` metinle `string.IsNullOrWhiteSpace` yarışı |
| 5 | Eşleme **testle** kanıtlanır (gerçek şema) | yalnız `default` değerle geçen test |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Nullable anotasyonu (`#nullable enable`) | proje düzeyinde **kapalı** olabilir | açık — `string?` uyarısı verir |
| Nullable `DateTime` | var | var |
| `record`/`init` | yok (C# 7.3) — `sealed class` + `set` | `record` yazılabilir |
| `DBNull` davranışı | Dapper **2.1.x** — aynı | aynı |
| Zaman biçimi | ISO-8601 `…Z` | aynı — sağlayıcı farkı `ISqlDialect`'te |

> **NOT:** İki bacak arasında **aynı satır tipi** kullanılır; nullable kapsamı (NRT) proje genelinde ADR-001 kapsamında tek karardır (**NEEDS_VALIDATION**).

---

## §5 Kardeşler

[dapper-query-mapping](dapper-query-mapping.md) · [audit-columns-convention](audit-columns-convention.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sqlite-provider-setup](sqlite-provider-setup.md)
