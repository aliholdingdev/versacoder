---
title: "connection-pooling-notes - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Baglanti havuzu davranisi: neden using yeterli, havuz tukemesi belirtileri, ClearPool, SQLite'ta havuz yok denecek kadar az"
---

# connection-pooling-notes — Bağlantı Havuzu Notları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [data index](../index.md) · [ado-net-connection](ado-net-connection.md)

> **Tek cümlelik sorumluluk:** *Bağlantı kapatınca ne olur, havuz nasıl tükenir, tükenince ne görünür* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

SQL Client **bağlantıları havuzlar**: `Close()` fiziksel bağlantıyı kesmez, havuza iade eder — bu yüzden **metot içi `using` hem doğru hem ucuzdur**. Havuz tükenmesi, sızan (dispose edilmeyen) bağlantıların sayısının `Max Pool Size`'a (varsayılan **100**) ulaşmasıdır; belirti **"The timeout period elapsed prior to obtaining a connection from the pool"** hatası ve `DbTimeouts.Query` içinde patlamasıdır. SQLite sağlayıcısında havuzlama **beklenmez**; orada sorun `busy_timeout`/kilit tarafıdır (DA-14).

---

## §1 Havuz yazilimi

```csharp
// DOGRU — her cagri kisa omurlu, havuz devrede
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    // ... sorgu
}   // Close() -> havuza iade (fiziksel baglanti acik kalir)

// YANLIS — dispose edilmeyen baglanti havuzu tuketir (max 100)
var leaked = _connectionFactory.Create();
await leaked.OpenAsync(cancellationToken);
```

---

## §2 Ayar tablosu

| Anahtar | Varsayılan | Ne zaman değiştirilir |
|---|---|---|
| `Pooling` | `true` | yalnız tanı/sorun giderme |
| `Max Pool Size` | `100` | yüksek eşzamanlı rapor işi |
| `Min Pool Size` | `0` | sürekli sıcak servis |
| `Connection Lifetime` | `0` | yüklü dengeli önleme |
| `Connect Timeout` | `15` | ağ gecikmesi varsa (ayrı süre!) |
| `Connection Reset` | `true` | elle kapatma (nadiren) |

---

## §3 Teşhis tablosu

| Belirti | Kök neden | Müdahale |
|---|---|---|
| "obtaining a connection from the pool" hatası | sızan bağlantı (dispose yok) | `using` zorunlu, kod taraması |
| Havuzda 100 açık bağlantı | metot dışında tutulan `DbConnection` | DA-10 ihlali |
| Uzun transaction + yavaş ekran | havuz dolu, transaction açık kalmış | kısa birim iş (DA-13) |
| SQLite "database is locked" | havuz değil, **kilit** | `busy_timeout` + WAL (DA-14) |
| Sorgu sürekli 15 sn'de timeout | plan/kilit ya da havuz beklemesi | `DbTimeouts` + log incelemesi |

> **Müdahale (test/ortam):** `SqlConnection.ClearAllPools()` yalnız tek bir yerde (test temizliği / migration öncesi) — üretimde kör çağırmak bağlantı kırar.

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Havuzlama | `Microsoft.Data.SqlClient` 5.2.x — **var** | **aynı 5.2.x** |
| Havuz anahtarları | aynı metin anahtarları | aynı |
| `SqliteConnection` | sağlayıcı **kullanılmaz** (ADR-012 D4) | havuzlama beklenmez; dosya/kilit yönetimi |
| `ClearPool` davranışı | aynı API | aynı |
| Ölçüm | performans testi her iki bacakta | aynı metrik |

---

## §5 Kardeşler

[ado-net-connection](ado-net-connection.md) · [command-timeout-policy](command-timeout-policy.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md)
