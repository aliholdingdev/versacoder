---
title: "command-timeout-policy - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "CommandTimeout degerleri ve DbTimeouts sabitleri: 15/60/300 sn, 0 yasak, timeout ile CancellationToken farki (DA-09)"
---

# command-timeout-policy — Komut Zaman Aşımı Politikası

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [ado-net-command](ado-net-command.md)

> **Tek cümlelik sorumluluk:** *Her komutun `CommandTimeout` değeri nedir, `0` neden yasaktır, timeout ile iptal farkı nedir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Değerler **kodda serbest yazılmaz**, `DbTimeouts` sabitlerinde toplanır: normal sorgu **15 sn**, rapor/export **60 sn**, migration/bootstrap **300 sn**. `CommandTimeout = 0` (sınırsız) **yasaktır** (DA-09) — UI donması ve havuz tükenmesi demektir. Timeout, **veritabanı tarafında** beklemeyi keser; `CancellationToken` ise **istemci tarafında** iptal eder; ikisi farklı katmandır ve **ikisi de** verilir.

---

## §1 Doğru kullanım

```csharp
public static class DbTimeouts
{
    public const int Query     = 15;    // liste/tek satir
    public const int Report    = 60;    // rapor / export / toplu is
    public const int Migration = 300;   // sefma kurulumu, bootstrap
}

var rows = await connection.QueryAsync<SessionRow>(
    new CommandDefinition(sql, param,
        commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken));
```

---

## §2 Değer tablosu

| İş | Sabit | Saniye | Aşılırsa |
|---|---|---|---|
| Ekran/liste sorgusu | `DbTimeouts.Query` | 15 | hata UI'a taşınır, kullanıcı tekrar dener |
| Rapor / export / toplu yazma | `DbTimeouts.Report` | 60 | iş kuyruğa/alarm |
| Migration / şema bootstrap | `DbTimeouts.Migration` | 300 | uygulama **başlamaz**, log yazar |
| Sağlık kontrolü (`SELECT 1`) | `DbTimeouts.Query` | 15 | sağlıksız işaretlenir |
| Sınırsız | `0` | — | **YASAK** (DA-09) |

---

## §3 Kural tablosu

| # | Kural | Yasak |
|---|---|---|
| 1 | Sabitler tek yerde (`DbTimeouts`) | dağılmış sayısal literal |
| 2 | Her `DbCommand`/Dapper çağrısında açık `commandTimeout` | hiç yazmamak (15'e düşer ama okunmaz) |
| 3 | `0` / `Timeout.Infinite` **= 0 adet** | "sonsuz iyi olsun" |
| 4 | Timeout + `CancellationToken` birlikte | yalnız biri |
| 5 | Uzun iş kısa paketlere bölünür | tek transaction içinde 60 sn+ bekleme |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CommandTimeout` varsayılanı | bağlantı metnindeki değer (15) | aynı |
| `CancellationToken` etkisi | iptal, kural olarak beklenen davranış | daha kesin iptal (`SqlCommand.Cancel`) |
| Süre ölçümü | `Stopwatch` ile test edilir | aynı |
| Timeout davranışı | **aynı sabitler, aynı test** | aynı |

> **NOT:** Timeout hataları `SqlException` (595/‑2) ya da `SqliteException` (`SQLITE_BUSY`/`Interrupt`) olarak gelir; repository bunu tek bir domain hatasına çevirir — formda ham sağlayıcı exception'ı görünmez (DA-04).

---

## §5 Kardeşler

[dapper-async-patterns](dapper-async-patterns.md) · [ado-net-command](ado-net-command.md) · [connection-pooling-notes](connection-pooling-notes.md) · [migration-version-table](migration-version-table.md) · [sqlite-provider-setup](sqlite-provider-setup.md)
