---
title: "command-timeout-policy - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "CommandTimeout degerleri ve DbTimeouts sabitleri: 15 / 60 / 300 sn, 0 yasak, timeout ile CancellationToken farki (DA-09)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# command-timeout-policy - Komut Zaman Asimi Politikasi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/async-rules|async-rules]] · [[.rules/error-handling-rules|error-handling-rules]] · [[.rules/testing-rules|testing-rules]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Her komutun `CommandTimeout` degeri nedir, `0` neden yasaktir, timeout ile iptal farki nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Degerler kodda serbest yazilmaz, `DbTimeouts` sabitlerinde toplanir: normal sorgu **15 saniye**, rapor/export **60 saniye**, migration/bootstrap **300 saniye**. `CommandTimeout = 0` (sinirsiz) **yasaktir** (DA-09); bu, ekranin donmasi ve baglanti havuzunun tukemesi demektir. Timeout **veritabani tarafinda** beklemeyi keser, `CancellationToken` ise **istemci tarafinda** iptal eder; ikisi farkli katmandadir ve **ikisi de** verilir.

```csharp
public static class DbTimeouts
{
    public const int Query     = 15;    // liste / tek satir
    public const int Report    = 60;    // rapor / export / toplu is
    public const int Migration = 300;   // sefma kurulumu, bootstrap
}

var rows = await connection.QueryAsync<SessionRow>(
    new CommandDefinition(sql, param,
        commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken));
```

---

## §1 Deger tablosu

| Is | Sabit | Saniye | Asilirsa |
|---|---|---|---|
| Ekran / liste sorgusu | `DbTimeouts.Query` | 15 | hata UI'a tasinir, kullanici tekrar dener |
| Rapor / export / toplu yazma | `DbTimeouts.Report` | 60 | is kuyruga alinir ya da alarm verilir |
| Migration / sema bootstrap | `DbTimeouts.Migration` | 300 | uygulama **baslamaz**, log yazar |
| Saglik kontrolu (`SELECT 1`) | `DbTimeouts.Query` | 15 | sagliksiz isaretlenir |
| Sinirsiz | `0` | - | **YASAK** (DA-09) |

---

## §2 Kural tablosu (DA-09)

| # | Kural | Yasak |
|---|---|---|
| 1 | Sabitler tek yerde (`DbTimeouts`) | dagilmis sayisal literal |
| 2 | Her `DbCommand` ve Dapper cagrisinda acik `commandTimeout` | hic yazmamak |
| 3 | `0` ve `Timeout.Infinite` toplam **0 adet** | "sonsuz iyi olsun" |
| 4 | Timeout + `CancellationToken` birlikte verilir | yalniz biri |
| 5 | Uzun is kisa paketlere bolunur | tek transaction icinde 60 sn uzeri bekleme |

---

## §3 Timeout mu, iptal mi

| Katman | Araç | Ne yapar | Kim baslatir |
|---|---|---|---|
| Veritabani | `CommandTimeout` | beklemeyi keser, hata dondurur | saglayici |
| Istemci | `CancellationToken` | calismayi iptal eder | UI / ust katman |
| Ikisi birlikte | `CommandDefinition` | iki katman da korumada | kod yazan |
| Yalniz timeout | `commandTimeout` yalniz | kullanici iptal edemez | hata |
| Yalniz iptal | `ct` yalniz | sunucu tarafinda is surer | israf |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CommandTimeout` varsayilani | baglanti metnindeki deger (15) | ayni |
| `CancellationToken` etkisi | iptal, kural olarak beklenen davranis | daha kesin iptal |
| Sure olcumu | `Stopwatch` ile test edilir | ayni |
| Timeout davranisi | **ayni sabitler, ayni test** | ayni |

> **NOT:** Timeout hatalari `SqlException` (595 / -2) ya da `SqliteException` (`SQLITE_BUSY` / `Interrupt`) olarak gelir; repository bunu tek bir domain hatasina cevirir - formda ham saglayici exception'i gorunmez (DA-04).

---

## §5 Teşhis

| Belirti | Kok neden | Mudahale |
|---|---|---|
| Hep 15 sn'de patlama | yavas plan ya da kilit | sorgu ve indeks incelemesi |
| Rapor hep 60 sn'de patlama | is boyutu buyuk | paketlere bol, `DbTimeouts.Report` kalmali |
| Migration 300 sn'de patlama | sefma islemi agir | uygulama baslamaz, log okunur |
| Iptal calismiyor | `ct` gecmemis | her imzaya `CancellationToken` ekle |
| Sifir gormus cagri | sabit unutulmus | kod taramasi: `CommandTimeout = 0` = 0 adet |

---

## §6 Kardesler

[[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]]

---

## §7 Kisa karar

15 / 60 / 300 uclusunden baska sayi yazilmaz; `0` hicbir kosulda gecmez. Timeout ve `CancellationToken` ayni cagrida birlikte bulunur - biri digerinin yerine degildir.

---

## §8 Sik sorulan

| Soru | Cevap |
|---|---|
| Varsayilan neden 15? | `DbTimeouts.Query` sabiti belirler, saglayiciya birakilmaz |
| Timeout ile iptal ayni sey mi? | Hayir; farkli katman, ikisi de gerekli |
| Uzun is icin 0 kullanilir mi? | Hayir; `DbTimeouts.Report` (60) ve paketleme |
| Hata UI'a nasil cikar? | repository tek domain hatasina cevirir (DA-04) |
| Testte nasil dogrulanir? | `Stopwatch` ile sure testi (testing-rules) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `command-timeout-policy.md` | `DbTimeouts` 15/60/300, `0` yasagi, timeout - iptal ayrimi, saglayici hata kodlari |
| `ado-net-command.md` | her `DbCommand` icin acik timeout kurali (DA-09) |
| `dapper-async-patterns.md` | `CommandDefinition` ile birlikte `cancellationToken` tasma |
| `connection-pooling-notes.md` | timeout - havuz bekleme iliskisi |
| `dapper-procedures-stored.md` | prosedur cagrisinda `DbTimeouts.Report` kullanimi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
