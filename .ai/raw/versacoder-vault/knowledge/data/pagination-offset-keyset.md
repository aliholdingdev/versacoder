---
title: "pagination-offset-keyset - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Sayfalama: OFFSET vs keyset (cursor), dialekt PagingClause, sayfa siniri ve kayip/satlama riskleri"
---

# pagination-offset-keyset — Sayfalama Stratejileri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [data index](../index.md) · [dapper-multi-map](dapper-multi-map.md)

> **Tek cümlelik sorumluluk:** *OFFSET mi keyset (cursor) mi, hangi durumda hangisi, iki sağlayıcıda nasıl yazılır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Küçük sayfalar (ekran, ilk 1-2 sayfa) için **OFFSET/LIMIT** yeterlidir; derin sayfalama ve sürekli artan akış (mesaj listesi, log) için **keyset** (imleç) kullanılır — `WHERE CreatedAt < @Cursor ORDER BY CreatedAt DESC LIMIT @Take`. Keyset, OFFSET'in iki kusurunu çözer: **satır kayması** (sayfa arası ekleme) ve **derinde O(offset) tarama**. Sayfa boyutu **sınırlıdır** (varsayılan 50, tavan 200); `LIMIT` değeri parametredir, metne **girmez** (DA-03).

---

## §1 OFFSET ve keyset

```csharp
// OFFSET — dialekt ile (ISqlDialect.PagingClause)
const string offsetSql = @"
    SELECT Id, Title, CreatedAt FROM Sessions
    WHERE DeletedAt IS NULL
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";       // SQLite: LIMIT @Take OFFSET @Skip

// KEYSET — imlec, derin sayfalama icin
const string keysetSql = @"
    SELECT Id, Title, CreatedAt FROM Sessions
    WHERE DeletedAt IS NULL AND (@Cursor IS NULL OR CreatedAt < @Cursor)
    ORDER BY CreatedAt DESC
    LIMIT @Take";
```

---

## §2 Secim tablosu

| Kriter | OFFSET/LIMIT | KEYSET |
|---|---|---|
| Derin sayfa performansı | O(offset) — yavaşlar | O(log n) — sabit |
| Sayfa arası ekleme | satır **kayması/duplike** | tutarlı |
| "Sayfa 3" butonu | evet | hayır (yalnız ileri/geri) |
| Toplam kayıt sayısı | ayrı `COUNT(*)` gerekir | ayrı `COUNT(*)` gerekir |
| Sağlayıcı farkı | `OFFSET..FETCH` vs `LIMIT` | taşınabilir (koşul + sıralama) |
| Uygulama | kısa listeler, yönetim ekranı | akış/derin gezinme |

---

## §3 Kural tablosu

| # | Kural | Yasak |
|---|---|---|
| 1 | Sayfa boyutu parametre + **tavan** (200) | `LIMIT 999999` |
| 2 | Sıralama **kararlı**: `ORDER BY CreatedAt DESC, Id DESC` | eşit anahtarlı sıralama (sayfa atlama kopar) |
| 3 | SQL sayfa parçası `ISqlDialect.PagingClause` | dialekti repository'de `#if` ile seçmek |
| 4 | Toplam sayı ayrı ve gerektiğinde (UI maliyeti) | her sayfada `COUNT(*)` |
| 5 | `@Skip/@Take` parametreli | metne gömülü sayı |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| SQL sözdizimi | sağlayıcıya göre — **kod aynı** | aynı |
| `ISqlDialect.PagingClause` | `net48`/`net10` aynı implementasyon | aynı |
| Değer `LIMIT` | Dapper 2.1.x | aynı |
| Akış (`IAsyncEnumerable`) | yok — `IReadOnlyList` döner | var — yalnız net10 kodu |

---

## §5 Kardeşler

[dapper-multi-map](dapper-multi-map.md) · [parameterized-queries](parameterized-queries.md) · [concurrency-optimistic](concurrency-optimistic.md) · [repository-pattern-thin](repository-pattern-thin.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)
