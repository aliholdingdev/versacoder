---
title: "dapper-multi-map - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Dapper coklu esleme: splitOn ile ikili / haritali sorgu, baba-cocuk, QueryMultiple ile birden cok result-set (DA-16)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# dapper-multi-map - Coklu Esleme ve QueryMultiple

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.rules/coding-standards|coding-standards]] · [[.rules/testing-rules|testing-rules]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Tek sorguda birden fazla tip nasil eslenir, `splitOn` ne ise yarar, birden cok result-set nasil okunur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Iki tabloyu tek sorguda okumak icin `QueryAsync<T1, T2, TR>(map, splitOn: "Id")` kullanilir: Dapper SELECT listesini `splitOn` kolonunda **boler**, onceki kolonlar `T1`'e, sonrakiler `T2`'ye gider. Ayni tipin cok satiri (baba - cocuk) icin **`QueryMultiple` ya da iki ayri sorgu** tercih edilir; `buffered: false` yalniz uzun akista kullanilir. Dongu icinde sorgu (N+1) yasaktir: ekran acilisinda toplam sorgu **10 veya daha az** olmalidir (DA-16).

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

## §1 Coklu esleme secim tablosu

| Durum | Arac | `splitOn` |
|---|---|---|
| 2 tip, 1 satir (JOIN) | `QueryAsync<T1, T2, TR>` | ilk ortak kolon (`Id`) |
| 3 tip | `QueryAsync<T1, T2, T3, TR>` | `splitOn: "Id,Id"` |
| Baba + N cocuk | `QueryMultiple` ya da 2 ayri sorgu | - |
| Birden cok result-set | `connection.QueryMultiple(...)` + `reader.ReadAsync<T>()` | - |
| Sayi / oy sayimi | `COUNT` ve satir ayri okunur | - |

---

## §2 Kural tablosu (DA-05 - DA-16)

| # | Kural | Yasak |
|---|---|---|
| 1 | `splitOn` kolonu SELECT listesinde **tekrar** gelmeli | bolunmus kolon eksik -> sessiz `default` |
| 2 | Map fonksiyonu saf ve kisa | map icinde DB cagrisi (gizli N+1) |
| 3 | `buffered: false` yalniz uzun akista | her sorguda kore `false` |
| 4 | Cocuk koleksiyonu satir tipinde (`List<>`) | `dynamic` koleksiyon |
| 5 | Dongu icinde sorgu = **0** (DA-16) | `foreach` + `QueryAsync` |

---

## §3 QueryMultiple okuma

| Adim | Islem | Not |
|---|---|---|
| 1 | `connection.QueryMultiple(sql, param, ...)` | tek baglanti, tek komut |
| 2 | `reader.ReadAsync<T>()` | ilk result-set |
| 3 | `reader.ReadAsync<T>()` | siradaki result-set |
| 4 | `reader.IsConsumed` kontrolu | okunmamis set kalmamali |
| 5 | `using` ile kapatma | `GridReader` de kisa omurludur |

Tek komutta cok set okumak, ayri baglanti ve ayri komut sayisini dusurur; buna karsilik okunmayan set yarim veri demektir.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `QueryAsync` coklu imza | Dapper **2.1.x** - ayni | ayni |
| `IAsyncEnumerable` akis | yok / paket gerekir | var - yalniz net10 kodu |
| `await foreach` | C# 8 - paylasilan koda girmez | yazilabilir |
| Bellek davranisi | `buffered` varsayilani **ayni** | ayni |

> **NOT:** Haritalama kodu saglayicidan bagimsizdir; SQLite test dali ile SQL Server uretim dali ayni `QueryAsync<...>` cagrisini calistirir.

---

## §5 Kardesler

[[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]]

---

## §6 Kisa karar

Iki tip -> `splitOn`; baba - cocuk -> `QueryMultiple` ya da iki sorgu; dongu icinde sorgu -> hicbir kosulda degil. `splitOn` kolonu SELECT listesinde tekrar edilmelidir, aksi halde Dapper sessizce `default` deger birakir.

---

## §7 Teshis

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Ikinci tip hep `default` | `splitOn` kolonu SELECT'te yok | kolonu tekrar ekle |
| Ekran cok yavas | dongu icinde sorgu (N+1) | JOIN veya `QueryMultiple` |
| Yarim veri geliyor | set okunmadan kaldi | sirayla tum `ReadAsync` cagrilari |
| Bellek yuksek | kore `buffered: false` | varsayilana don |
| Map icinde DB cagrisi | gizli N+1 | map'i saf birak |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Ekran acilisinda sorgu sayisi <= 10 | sayac / log (DA-16) |
| `foreach` + `QueryAsync` aramasi = **0** | kod taramasi |
| `splitOn` kolonlari SELECT'te mevcut | sorgu incelemesi |
| Sonuc setleri tam okunuyor | entegrasyon testi |

> Baba - cocuk icin `QueryMultiple` ile iki sorgu arasindaki secim proje kararidir: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dapper-multi-map.md` | `splitOn` ornegi, secim tablosu, DA-05 / DA-16 kurallari, iki bacak farki |
| `dapper-query-mapping.md` | satir tipi ve `QueryAsync<T>` esleme davranisi |
| `dapper-parameters-dynamic.md` | anonim nesne parametre yazimi |
| `dapper-async-patterns.md` | `CommandDefinition` ile iptal ve timeout |
| `ado-net-command.md` | `NextResult` / `QueryMultiple` karsilastirmasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
