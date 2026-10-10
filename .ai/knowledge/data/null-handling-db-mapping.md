---
title: "null-handling-db-mapping — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "SQL NULL ile C# null/unbox iliskisi: nullable tipler, DBNull, varsayilan deger tuzagi, ISO-8601 tarih donusumu (DA-08)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# null-handling-db-mapping — NULL ve Eşleme

**Zorunlu Baglantilar:** [[index|vault index]] · [[knowledge/index|knowledge index]] · [[.rules/coding-standards|coding standards]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Veritabanındaki `NULL`, C#'ta hangi tüpe düşer; `default` tuzağı nasıl önlenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`NULL` kolon eşleşirken **nullable C# tipine** bağlanır (`string?`, `DateTime?`, `int?`). Nullable olmayan bir tipte `NULL`, sessizce `default` üretir: `0`, `0001-01-01`, `false` — bu, kuralı bozmadan **yanlış veri** yaymaktır (DA-08). `DBNull` dönüşümünü Dapper yapar; repository'de `if (x == DBNull.Value)` kontrolü yazılmaz. Tarihler UTC ve ISO-8601 olarak saklanır, okumada `DateTimeKind.Utc` korunur (DA-15).

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

Satır tipi ile `SELECT` listesi birebir hizalanır: eksik kolon `default` ile dolar, fazla kolon sessizce atlanır.

## §2 Donusum tablosu

| SQL kolonu | C# tipi | `NULL` geldiğinde | Tuzak |
|---|---|---|---|
| `TEXT NOT NULL` | `string` | bilinçli `?? string.Empty` | `null` sızması |
| `TEXT NULL` | `string?` | `null` | `""` ile `null` karışması |
| `datetime2 NULL` | `DateTime?` | `null` | `0001-01-01` |
| `INTEGER NOT NULL DEFAULT 0` | `int` | `0` | `null` beklemek |
| `INTEGER NULL` | `int?` | `null` | `default(int)` = `0` |
| `BIT NOT NULL` | `bool` | `false` | `true/false` anlamı kaybı |
| `BLOB` | `byte[]` | `null` / boş dizi | boyut tahmini |

## §3 Kural tablosu (DA-08 · DA-15)

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Kolon nullable ise C# tipi `?` | `DateTime` + `default` ile gizleme |
| 2 | `SELECT` listesi satır tipiyle hizalı | eşleşmeyen kolonun sessizce atlanması |
| 3 | Zaman `DateTimeKind.Utc`, kaynak `TimeProvider` | `DateTime.Now` (CS / DA-15) |
| 4 | `NOT NULL` metinde anlamlı varsayılan kod tarafında | `null` metinle `IsNullOrWhiteSpace` yarışı |
| 5 | Eşleme gerçek şemayla test edilir | yalnız `default` ile geçen test |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Nullable anotasyonu (`#nullable enable`) | proje düzeyinde kapalı olabilir | açık — `string?` uyarısı verir |
| Nullable `DateTime` | var | var |
| `record` / `init` | yok (C# 7.3) → `sealed class` + `set` | `record` yazılabilir |
| `DBNull` davranışı | Dapper 2.1.x | aynı |
| Zaman biçimi | ISO-8601 `...Z` | aynı; sağlayıcı farkı `ISqlDialect` içinde |

İki bacakta **aynı satır tipi** kullanılır: ortak dil kesişimi `sealed class` + `set`'tir.

## §5 Kayip senaryolari

| Durum | Belirti | Önlem |
|---|---|---|
| Nullable tip yanlış seçimi | `0001-01-01` raporlarda dolaşır | kolon şemasına bak, `?` koy |
| `SELECT` ile tip uyumsuz | değer sessizce `default` | eşleme testi |
| Yerel/UTC karışması | saat kayması, sıralama bozuk | `DateTimeKind.Utc` + `TimeProvider` |
| `""` ve `null` aynı sanılır | filtreler şaşırır | `NOT NULL` + tek kaynak varsayılan |

## §6 Sik hatalar

- `int` alınıp "aslında boştu" denmesi
- Eşlemeyi elle `Convert.ToInt32` ile kapatmak
- Tarihi metin olarak okuyup sonra parse etmek
- Satır tipinde `dynamic` ile `default` gizlemek
- Testi in-memory sahte veriyle geçiştirmek

## §7 Dogrulama notlari

Nullable reference types kapsamının (NRT) proje genelinde açık mı kapalı mı olduğu tek karardır ve TFM'ler arasında farklı olamaz — **DOĞRULAMA GEREKLI**. NRT kararı verilmeden `string?` kullanımı iki bacakta farklı uyarı üretir.

## §8 Kardesler

[[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]]

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `null-handling-db-mapping.md` (kaynak) | doğru kullanım örneği, 7 satırlık dönüşüm tablosu, DA-08/DA-15 kuralları, TFM farkları |
| `dapper-query-mapping.md` | Dapper'ın satır eşleme ve `DBNull` dönüşümü |
| `audit-columns-convention.md` | `CreatedAt`/`DeletedAt` gibi kolonların NULL davranışı |
| `repository-pattern-thin.md` | satır tipinin `record`/sınıf, `dynamic` olmaması |
| `pagination-offset-keyset.md` | `DeletedAt IS NULL` filtrelerinin NULL okuması |
