---
title: "dapper-async-patterns - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Dapper async kullanimi: QueryAsync / ExecuteAsync, CommandDefinition + CancellationToken, ConfigureAwait, deadlock kacinma"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# dapper-async-patterns - Async Davranis Kurallari

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/async-rules|async-rules]] · [[.rules/coding-standards|coding-standards]] · [[.rules/error-handling-rules|error-handling-rules]] · [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Veri erisimi async'i nerede baslar, iptal nasil tasinir, WinForms'ta deadlock nasil onlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sorgular `QueryAsync` / `ExecuteAsync` ile calisir; her cagrida `CancellationToken` ve `commandTimeout: DbTimeouts.Query` tasınir. **Kutuphane kodunda** `.Result`, `.Wait()` ve `GetAwaiter().GetResult()` **yasaktir** - ozellikle `net48` WinForms `SynchronizationContext` icinde ekrani dondurur. Tek kural: async zincirin tamami `await` ile, kutuphane katmaninda `ConfigureAwait(false)` ile yurur; senkron sarmalayici yazilmaz.

```csharp
public async Task<SessionRow?> GetAsync(Guid id, CancellationToken cancellationToken)
{
    const string sql =
        "SELECT Id, Title FROM Sessions WHERE Id = @Id AND DeletedAt IS NULL";

    using (var connection = _connectionFactory.Create())
    {
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return await connection.QuerySingleOrDefaultAsync<SessionRow>(
            new CommandDefinition(sql, new { Id = id },
                commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }
}
```

---

## §1 Kural tablosu

| # | Kural | Neden |
|---|---|---|
| 1 | Her async veri metodu `CancellationToken` alir (son parametre) | iptal edilebilir ekran |
| 2 | Her cagrida `CommandTimeout` | iptal != timeout, ikisi de gerekli |
| 3 | Kutuphane katmaninda `ConfigureAwait(false)` | WinForms `SynchronizationContext` kilitlenmesi |
| 4 | `.Result` / `.Wait()` = **0 adet** | deadlock + `AggregateException` kaybi |
| 5 | Senkron sarmalayici (`Get().Result`) **yazilmaz** | cift API yuzeyi, bloke olan ekran |

---

## §2 Async secim tablosu

| Is | Cagri | Not |
|---|---|---|
| Satir oku | `QueryAsync<T>` | `CommandDefinition` ile iptal |
| Tek satir | `QuerySingleAsync<T>` / `QuerySingleOrDefaultAsync<T>` | fazla satir = exception (kasitli) |
| Yaz | `ExecuteAsync` | etkilenen satir sayisi doner |
| Skaler | `ExecuteScalarAsync<T>` | `COUNT` / `EXISTS` |
| Cok set | `QueryMultipleAsync` | `CommandDefinition` icinde |
| Baglanti acma | `OpenAsync(ct)` | async metotta senkron `Open()` yasak |

---

## §3 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `ConfigureAwait` | gerekli ve yeterli | ayni |
| `await using` | yok - klasik `using ( ... ) { }` | yazilabilir |
| Async `Main` | C# 7.1+ beraberinde geldi | var |
| Eszamanlilik testi | ekran `SynchronizationContext` altinda **ayni** test | ayni |
| Dapper iptal | `CommandDefinition.CancellationToken` **2.1.x** | ayni |

> **UYARI:** `net48` bacakinda senkron DB cagrisi (`.Result`) ile `net10`'da gecmek yanilticidir: her iki bacakta da test, iptal ve timeout davranisi **ayni** senaryoyla kanitlanir.

---

## §4 Iptal - timeout karsilastirmasi

| Katman | Araç | Devreye girer | Sonuc |
|---|---|---|---|
| Istemci | `CancellationToken` | ust katman / UI | calisma durur |
| Dapper | `CommandDefinition.CancellationToken` | sorgu cagrisinda | baglanti serbest birakilir |
| Saglayici | `commandTimeout` | sunucu tarafinda | hata doner |
| Kutuphane | `ConfigureAwait(false)` | devam eden is | deadlock onlenir |

Dortu birlikte kullanilir; hicbiri digerinin yerine gecmez.

---

## §5 Kardesler

[[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/ado-net-command|ado-net-command]]

---

## §6 Kisa karar

`await` + `ConfigureAwait(false)` + `CancellationToken` + `commandTimeout`. Kutuphane katmaninda `.Result` ve `.Wait()` goruldugunde inceleme kirmizi; senkron sarmalayici istisna degil, hata sayilir.

---

## §7 Teshis

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Ekran donuyor, CPU bos | `.Result` UI thread'ini bekliyor | `await` + `ConfigureAwait(false)` |
| Hata `AggregateException` icinde | `.Wait()` sarmalamis | `await` ile yay |
| IPTal calismiyor | `ct` imzada yok | son parametre olarak `CancellationToken` |
| Timeout hep 15 sn | `commandTimeout` yazilmamis | `DbTimeouts.Query` ekle |
| Senkron sarmalayici var | cift yuzey | sil, async zinciri tek birak |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| `.Result` / `.Wait()` aramasi = **0** | kod taramasi |
| Her async veri metodu `CancellationToken` aliyor | imza incelemesi |
| Her cagrida `commandTimeout` yazili | kod incelemesi |
| `net48` ekrani altinda ayni test geciyor | eszamanlilik testi |

> Kutuphane disindaki (ekran katmanindaki) `await` tercihi proje kararidir: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dapper-async-patterns.md` | `CommandDefinition` ornegi, `.Result` yasagi, `ConfigureAwait` kurali, secim tablosu |
| `ado-net-connection.md` | `OpenAsync(ct)` ve `using` yasam dongusu |
| `command-timeout-policy.md` | timeout - iptal ayrimi ve `DbTimeouts` degerleri |
| `ado-net-command.md` | ham `Execute*Async` karsiliklari |
| `dapper-multi-map.md` | `QueryMultipleAsync` cagri bicimi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
