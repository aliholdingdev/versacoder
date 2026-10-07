---
title: "sql-injection-defense — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "SQL injection savunmasi: parameterized-only, identifier allowlist, LIKE/ORDER BY/dinamik parca kurallari, tarama testleri"
---

# sql-injection-defense — SQL Injection Savunması

**Zorunlu Baglantilar:** [vault index](../../index.md) · [CLAUDE.md (G2)](../../CLAUDE.md) · [coding standards](../../.rules/coding-standards.md) · [parameterized-queries](parameterized-queries.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [connection-string-secrets](connection-string-secrets.md)
**Küme + Hub:** [knowledge index](../index.md) · [data index](index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (data · 7/25):** [ado-net-connection](ado-net-connection.md) · [audit-columns-convention](audit-columns-convention.md) · [command-timeout-policy](command-timeout-policy.md) · [concurrency-optimistic](concurrency-optimistic.md) · [connection-pooling-notes](connection-pooling-notes.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-query-mapping](dapper-query-mapping.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [migration-rollback](migration-rollback.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cumlelik sorumluluk:** *Injection'ın her giriş kapısı nasıl kapatılır, kapatılamayan tek kapıyı (adres) nasıl denetlenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Tek savunma hattı **parameterized-only**dır: değer hiçbir SQL metnine girmez; birleştiren satır çıkarsa build RED (G2). Parametrelenemeyen **adres** (tablo/kolon adı, sıralama yönü, prosedür adı) allowlist ile sabit dallara bölünür. Yan katmanlar: veritabanı hesabı **en az yetki** ile açılır (`SELECT/INSERT/UPDATE` var, `DROP` yok), ham exception metni UI'a çıkmaz, CI'daki tarama testi birleştirme satırı için **0** bekler.

## §1 Kapi tablosu

| Kapı | Saldırı | Savunma |
|---|---|---|
| Değer | `' OR '1'='1` | `@ad` parametresi (DA-03) |
| `IN` listesi | `1); DROP ...` | Dapper `IN @Ids` genişletme |
| `LIKE` | `%` / `_` kaçışı | `@Pattern` + gerekirse `ESCAPE '\'` |
| `ORDER BY` kolonu | `id; --` | allowlist → sabit SQL dalı |
| Tablo/kolon adı | dinamik adres | allowlist + `QuoteIdentifier` |
| Prosedür adı | interpolasyon | `const string` + `CommandType.StoredProcedure` |
| Hata mesajı | ham SQL/şema sızıntısı | genel mesaj; ayrıntı yalnız log'da |

## §2 Allowlist ornegi

```csharp
private static readonly HashSet<string> Sortable =
    new HashSet<string>(StringComparer.Ordinal) { "CreatedAt", "Title", "Id" };

if (!Sortable.Contains(sortColumn))
    throw new ArgumentException("sortColumn", sortColumn);

// govde sabit: izin verilen tek sorgu
const string sql = "SELECT Id, Title FROM Sessions ORDER BY CreatedAt DESC";
```

Kullanıcı adı reddedilir; seçilecek SQL dalı önceden bilinir.

## §3 Kural tablosu (DA-03 · DA-06 · SEC-03)

| # | Kural | Kanıt |
|---|---|---|
| 1 | SQL birleştiren satır = **0** | CI kod taraması |
| 2 | Kullanıcı girdisi yalnız parametre olarak | kod incelemesi |
| 3 | Adres allowlist'i + sabit SQL dalı | allowlist dışı test → exception |
| 4 | DB hesabı en az yetki | sunucudaki rol kaydı |
| 5 | Ham exception metni UI'a gitmez | log'da ayrıntı, UI'da genel mesaj |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Parametre API | `SqlParameter` 5.2.x | aynı paket, aynı davranış |
| Tarama testi | aynı regex, her iki bacakta CI | aynı |
| Doğrulama (girdi temizleme) | tek başına yeterli değil | aynı |
| Kapsam | savunma sağlayıcıdan bağımsız | aynı |

"Kaçış" bir savunma değil, hata kaynağıdır: kaçış kuralı SQL sürümüne göre değişir; kaçış yalnız `LIKE` kalıbında ve **parametre içinde** düşünülür.

## §5 Katmanlar

| Katman | Ne yapar | Ne yapamaz |
|---|---|---|
| Parametre | değeri metinden ayırır | adresi korumaz |
| Allowlist | adresi sabitler | değeri temizlemez |
| En az yetki | zararı sınırlar | sorguyu düzeltebilmez |
| Hata gizleme | bilgi sızdırmasını keser | saldırıyı engellemez |
| CI taraması | regresyonu erken yakalar | tek başına koruma değildir |

## §6 Denetim listesi

- `$"SELECT` · `$"INSERT` · `$"UPDATE` · `$"DELETE` · `+ "SELECT` → **0** satır
- `string.Format` + SQL → **0**
- Prosedür adı `const` (bkz. [dapper-procedures-stored](dapper-procedures-stored.md))
- Dinamik `ORDER BY`/kolon → allowlist
- DB rolünde DDL yetkisi **yok**

## §7 Dogrulama notlari

Tarama regex'inin hangi iş akışında (CI hedefi, pre-commit) çalıştığı ve ihlalde build'in nasıl RED edildiği vault'ta birebir tanımlanmamıştır — **DOĞRULAMA GEREKLI**. Kural G2 olarak bilinir; otomasyonu için ayrı kayıt gerekir.

## §8 Kardesler

[parameterized-queries](parameterized-queries.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [connection-string-secrets](connection-string-secrets.md) · [ado-net-command](ado-net-command.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `sql-injection-defense.md` (kaynak) | kapı tablosu, allowlist örneği, DA-03/DA-06/SEC-03 kuralları, TFM farkları, denetim listesi |
| `parameterized-queries.md` | `@ad` zorunluluğu ve IN/LIKE/ORDER BY kalıpları |
| `connection-string-secrets.md` | hesap bilgisinin sır olarak korunması |
| `dapper-procedures-stored.md` | prosedür adının `const` olması |
| `ado-net-command.md` | parametrenin komut nesnesine bağlanması |
