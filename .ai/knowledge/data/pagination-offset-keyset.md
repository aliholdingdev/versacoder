---
title: "pagination-offset-keyset — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Sayfalama: OFFSET vs keyset (cursor), dialekt PagingClause, sayfa siniri ve kayip/satlama riskleri"
---

# pagination-offset-keyset — Sayfalama Stratejileri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [rules index](../../.rules/index.md) · [parameterized-queries](parameterized-queries.md) · [dapper-multi-map](dapper-multi-map.md) · [repository-pattern-thin](repository-pattern-thin.md)
**Küme + Hub:** [data index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (data · 6/25):** [ado-net-command](ado-net-command.md) · [ado-net-connection](ado-net-connection.md) · [audit-columns-convention](audit-columns-convention.md) · [command-timeout-policy](command-timeout-policy.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [dapper-query-mapping](dapper-query-mapping.md) · [migration-rollback](migration-rollback.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [sql-injection-defense](sql-injection-defense.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cumlelik sorumluluk:** *OFFSET mi keyset (cursor) mi, hangi durumda hangisi, iki sağlayıcıda nasıl yazılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Ekran ve ilk birkaç sayfa için **OFFSET/LIMIT** yeterlidir; derin gezinme ve sürekli artan akış (mesaj, log) için **keyset** kullanılır: `WHERE CreatedAt < @Cursor ORDER BY CreatedAt DESC LIMIT @Take`. Keyset, OFFSET'in iki kusurunu birden kapatır — sayfa arasına satır girince oluşan **kayıp/duplike** ve derin sayfada **O(offset) tarama**. Sayfa boyutu sınırlıdır: varsayılan 50, tavan 200; `LIMIT`/`FETCH` değeri parametre olarak bağlanır, SQL metnine **yazılmaz** (DA-03).

## §1 Iki kalip

```csharp
// OFFSET — sayfa parçasi dialektten gelir (ISqlDialect.PagingClause)
const string offsetSql = @"
    SELECT Id, Title, CreatedAt FROM Sessions
    WHERE DeletedAt IS NULL
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";   // SQLite: LIMIT @Take OFFSET @Skip

// KEYSET — imleçle ileri sarma
const string keysetSql = @"
    SELECT Id, Title, CreatedAt FROM Sessions
    WHERE DeletedAt IS NULL AND (@Cursor IS NULL OR CreatedAt < @Cursor)
    ORDER BY CreatedAt DESC
    LIMIT @Take";
```

## §2 Secim tablosu

| Kriter | OFFSET/LIMIT | KEYSET |
|---|---|---|
| Derin sayfa performansı | O(offset), yavaşlar | O(log n), sabit kalır |
| Sayfa arası ekleme | satır kayması / duplike | tutarlı |
| "Sayfa 3" butonu | evet | hayır — yalnız ileri/geri |
| Toplam kayıt | ayrı `COUNT(*)` gerekir | ayrı `COUNT(*)` gerekir |
| Sağlayıcı farkı | `OFFSET..FETCH` vs `LIMIT` | taşınabilir (koşul + sıralama) |
| Tipik kullanım | yönetim ekranı, kısa liste | akış, derin gezinme |

## §3 Kural tablosu

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Sayfa boyutu parametre + tavan (200) | `LIMIT 999999` |
| 2 | Kararlı sıralama: `ORDER BY CreatedAt DESC, Id DESC` | eşit anahtarlı sıralama (sayfa kopar) |
| 3 | Sayfa parçası `ISqlDialect.PagingClause` | repository'de `#if` ile dialect seçmek |
| 4 | Toplam sayı ayrı ve gerektiğinde | her sayfada `COUNT(*)` |
| 5 | `@Skip` / `@Take` / `@Cursor` parametreli | metne gömülü sayı |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| SQL sözdizimi | sağlayıcıya göre — kod aynı | aynı |
| `ISqlDialect.PagingClause` | aynı implementasyon | aynı |
| Değer `LIMIT` | Dapper 2.1.x | aynı |
| Akış dönüşü | yok → `IReadOnlyList` | `IAsyncEnumerable` yalnız net10 kodu |

Sayfa parçası değişse de gövde sabit kalır; farklılık tek yerdedir (bkz. [parameterized-queries](parameterized-queries.md)).

## §5 Kayip ve satir kaymasi

| Durum | OFFSET sonucu | KEYSET sonucu |
|---|---|---|
| Sayfa 2'ye satır eklendi | 1 satır atlanır | etkilenmez |
| Sayfa 2'den satır silindi | bir satır tekrar görünür | etkilenmez |
| 1000. sayfa | tarama derinleşir | imleç konumu sabit |
| Eşit `CreatedAt` | sıralama kopabilir | `Id` ikinci anahtarla çözülür |

## §6 Sik hatalar

- `ORDER BY` anahtarını tek kolona indirmek (eşitlikte sayfa kopar)
- Offset'i metin birleştirmeyle yazmak (G2 ihlali)
- `COUNT(*)`'i her sayfada çağırmak
- Cursor'ı imzalamadan/serileştirmeden istemciye vermek
- Tavanı açıp `Take = 10000` istemek

## §7 Dogrulama notlari

Cursor değerinin serileştirme biçimi (ISO-8601 metin mi, ikili mi) ve imzalama zorunluluğu için vault'ta açık bir kayıt yoktur — **DOĞRULAMA GEREKLI**. Kayıt eklenene kadar cursor yalnız sunucu tarafında üretilir ve güvenli kanaldan taşınır.

## §8 Kardesler

[dapper-multi-map](dapper-multi-map.md) · [parameterized-queries](parameterized-queries.md) · [concurrency-optimistic](concurrency-optimistic.md) · [repository-pattern-thin](repository-pattern-thin.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `pagination-offset-keyset.md` (kaynak) | iki SQL kalıbı, seçim tablosu, 5 kural, 50/200 sayfa sınırı, TFM farkları |
| `parameterized-queries.md` | `@Skip`/`@Take`/`LIMIT`'in parametre olması |
| `repository-pattern-thin.md` | sorgunun yalnız repository'de oluşu |
| `sqlserver-provider-setup.md` · `sqlite-provider-setup.md` | `OFFSET..FETCH` ve `LIMIT` dialect ayrımı |
| `concurrency-optimistic.md` | sayfa arası eşzamanlı değişiklik riski |
