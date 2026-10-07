---
title: "sql-injection-defense - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "SQL injection savunmasi: parameterized-only, identifier allowlist, LIKE/ORDER BY/DINAMIK parca kurallari, tarama testleri"
---

# sql-injection-defense — SQL Injection Savunması

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [parameterized-queries](parameterized-queries.md)

> **Tek cümlelik sorumluluk:** *Injection'ın her giriş kapısı nasıl kapatılır, kapatılamayan tek kapıyı (adres) nasıl denetlenir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Tek savunma hattı **parameterized-only**: değer **hiç** SQL metnine girmez (`G2` ihlali → build RED). Parametrelenemeyen **adres** (tablo/kolon adı, sıralama yönü) **allowlist** ile sabit dallara bölünür. Ek katmanlar: en az yetki (DB kullanıcısı `SELECT/INSERT/UPDATE` yetkili, `DROP` değil), hata mesajlarında ham SQL **sızdırılmaz**, tarama testi (`$"SELECT` · `+ "SELECT` · `string.Format(` + SQL) CI'da **0** bekler.

---

## §1 Kapi tablosu

| Kapı | Saldırı | Savunma |
|---|---|---|
| Değer | `' OR '1'='1` | `@ad` parametresi (DA-03) |
| `IN` listesi | `1); DROP…` | Dapper `IN @Ids` genişletme |
| `LIKE` | `%`/`_` kaçışı | `@Pattern` + gerekirse `ESCAPE '\'` |
| `ORDER BY` kolonu | `id; --` | **allowlist** → sabit SQL dalı |
| Tablo/kolon adı | dinamik adres | allowlist + `QuoteIdentifier` |
| Prosedür adı | interpolasyon | `const string` + `CommandType.StoredProcedure` |
| Hata mesajı | ham SQL/şema sızıntısı | genel hata, ayrıntı log'da |

---

## §2 Allow ornegi

```csharp
private static readonly HashSet<string> Sortable =
    new HashSet<string>(StringComparer.Ordinal) { "CreatedAt", "Title", "Id" };

if (!Sortable.Contains(sortColumn)) throw new ArgumentException("sortColumn", sortColumn);

const string sql = "SELECT Id, Title FROM Sessions ORDER BY CreatedAt DESC";   // sabit govde
```

---

## §3 Kural tablosu (DA-03 · DA-06 · SEC-03)

| # | Kural | Kanıt |
|---|---|---|
| 1 | SQL birleştiren satır = **0** | kod taraması (CI) |
| 2 | Kullanıcı girdisi **yalnız** parametre olarak | kod incelemesi |
| 3 | Adres allowlist'i + sabit SQL dalı | allowlist dışı test → exception |
| 4 | DB hesabı **en az yetki** | sunucu tarafında rol kaydı |
| 5 | Ham exception metni UI'a **gitmez** | log'da ayrıntı, UI'da genel mesaj |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Parametre API | `SqlParameter` 5.2.x | aynı paket, aynı davranış |
| Tarama testi | aynı regex, her iki bacakta CI | aynı |
| Doğrulama (girdi temizleme) | **tek başına yeterli değil** | aynı |
| Kapsam | savunma katmanı **sağlayıcıdan bağımsız** | aynı |

> **NOT:** "Escape etmek" bir savunma değil, hata kaynağıdır (SQL sürümüne göre kaçış kuralı değişir). Kaçış yalnız `LIKE` kalıbında ve parametre **içinde** düşünülür.

---

## §5 Denetim listesi

- [ ] `$"SELECT` · `$"INSERT` · `$"UPDATE` · `$"DELETE` · `+ "SELECT` → **0** satır
- [ ] `string.Format` + SQL → **0**
- [ ] `CommandType.StoredProcedure` adı `const` (ilgili dosya: [dapper-procedures-stored](dapper-procedures-stored.md))
- [ ] Dinamik `ORDER BY`/kolon → allowlist
- [ ] DB rolü: DDL yetkisi **yok**

---

## §6 Kardesler

[parameterized-queries](parameterized-queries.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [connection-string-secrets](connection-string-secrets.md) · [ado-net-command](ado-net-command.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md)
