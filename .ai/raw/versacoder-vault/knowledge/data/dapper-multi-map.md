---
title: "dapper-multi-map - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Dapper coklu esleme: splitOn ile ikili/haritali sorgu, one-to-many, QueryMultiple ile birden fazla result-set (DA-16)"
---

# dapper-multi-map — Çoklu Eşleme ve `QueryMultiple`

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [dapper-query-mapping](dapper-query-mapping.md)

> **Tek cumlelik sorumluluk:** *Tek sorguda birden fazla tip/nasıl eşlenir, `splitOn` ne işe yarar, tek sorguda birden çok result-set nasıl okunur* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

İki tabloyu tek sorguda okumak için `QueryAsync<T1, T2, TR>(map, splitOn: "Id")` kullanılır: Dapper SELECT listesini `splitOn` kolonunda **bölür**, önceki kolonlar `T1`'e, sonrakiler `T2`'ye gider. Aynı tipin çok satırı (baba-çocuk) için `buffered: false` ya da daha öngörülebilir olan **iki sorgu / `QueryMultiple`** tercih edilir. N+1 yerine bu desenler zorunludur (DA-16): ekran açılışında toplam sorgu ≤ 10.

---

## §1 Dogru kullanim

```csharp
const string sql = @"
    SELECT s.Id, s.Title, m.Id, m.Role, m.Content
    FROM Sessions s
    JOIN Messages m ON m.SessionId = s.Id
    WHERE s.Id = @SessionId
    ORDER BY s.CreatedAt, m.CreatedAt";

var rows = await connection.QueryAsync<SessionRow, MessageRow, SessionRow>(
    (s, m) => { s.Messages.Add(m); return s; },
    sql, new { SessionId = sessionId }, splitOn: "Id",
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §2 Coklu esleme secim tablosu

| Durum | Araç | `splitOn` |
|---|---|---|
| 2 tip, 1 satır (JOIN) | `QueryAsync<T1,T2,TR>` | ilk ortak kolon (`Id`) |
| 3 tip | `QueryAsync<T1,T2,T3,TR>` | `splitOn: "Id,Id"` |
| Baba + N çocuk | `QueryMultiple` ya da 2 ayrı sorgu | — |
| Birden çok result-set | `connection.QueryMultiple(...)` + `reader.ReadAsync<T>()` | — |
| Sayı/oylama | tek sorguda `COUNT` + satır ayrı okunur | — |

---

## §3 Kural tablosu (DA-05 · DA-16)

| # | Kural | Yasak |
|---|---|---|
| 1 | `splitOn` kolonu SELECT listesinde **tekrar** gelmeli | bölünmüş kolon eksik → sessiz `default` |
| 2 | Map fonksiyonu saf ve kısa | map içinde DB çağrısı (gizli N+1) |
| 3 | `buffered: false` yalnız uzun akışta | her sorguda kör `false` |
| 4 | Çocuk koleksiyonu satır tipinde (`List<>`) | `dynamic` koleksiyon |
| 5 | Döngü içinde sorgu = **0** (DA-16) | `foreach` + `QueryAsync` |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `QueryAsync` çoklu imza | Dapper 2.1.x — aynı | aynı |
| `IAsyncEnumerable` akış | yok / paket gerekir | var — yalnız net10 kodu |
| `await foreach` | C# 8 — paylaşılan koda girmez | yazılabilir |
| Bellek davranışı | `buffered` varsayılanı **aynı** | aynı |

> **NOT:** Haritalama kodu sağlayıcıdan bağımsızdır; SQLite test dalı ile SQL Server üretim dalı aynı `QueryAsync<...>` çağrısını çalıştırır (bkz. [dual-tfm-provider-choice](dual-tfm-provider-choice.md)).

---

## §5 Kardesler

[dapper-query-mapping](dapper-query-mapping.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-async-patterns](dapper-async-patterns.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [repository-pattern-thin](repository-pattern-thin.md)
