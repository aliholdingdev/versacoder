---
title: "connection-pooling-notes - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Baglanti havuzu davranisi: neden using yeterli, havuz tukemesi belirtileri, ClearPool, SQLite'ta havuz beklenmez"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# connection-pooling-notes - Baglanti Havuzu Notlari

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.rules/coding-standards|coding-standards]] · [[.rules/error-handling-rules|error-handling-rules]] · [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Baglanti kapatinca ne olur, havuz nasil tukanir, tukince ne gorunur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

SQL Client **baglantilari havuzlar**: `Close()` fiziksel baglantiyi kesmez, havuza iade eder - bu yuzden **metot ici `using` hem dogru hem ucuzdur**. Havuz tukemesi, dispose edilmeyen baglantilarin sayisinin `Max Pool Size` degerine (varsayilan **100**) ulasmasidir; belirti **"The timeout period elapsed prior to obtaining a connection from the pool"** hatasidir ve `DbTimeouts.Query` suresi icinde patlar. SQLite saglayicisinda havuzlama **beklenmez**; oradaki sorun `busy_timeout` / kilit taraftadir.

```csharp
// DOGRU - her cagri kisa omurlu, havuz devrede
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    // ... sorgu
}   // Close() -> havuza iade (fiziksel baglanti acik kalir)

// YANLIS - dispose edilmeyen baglanti havuzu tuketir (max 100)
var leaked = _connectionFactory.Create();
await leaked.OpenAsync(cancellationToken);
```

---

## §1 Ayar tablosu

| Anahtar | Varsayilan | Ne zaman degistirilir |
|---|---|---|
| `Pooling` | `true` | yalniz tani / sorun giderme |
| `Max Pool Size` | `100` | yuksek eszamanli rapor isi |
| `Min Pool Size` | `0` | surekli sicak servis |
| `Connection Lifetime` | `0` | yuklu dengeli onleme |
| `Connect Timeout` | `15` | ag gecikmesi varsa (ayri sure!) |
| `Connection Reset` | `true` | elle kapatma (nadir) |

---

## §2 Teshis tablosu

| Belirti | Kok neden | Mudahale |
|---|---|---|
| "obtaining a connection from the pool" hatasi | siza kalan baglanti (dispose yok) | `using` zorunlu, kod taramasi |
| Havuzda 100 acik baglanti | metot disinda tutulan `DbConnection` | DA-10 ihlali |
| Uzun transaction + yavas ekran | havuz dolu, transaction acik kalmis | kisa birim is |
| SQLite "database is locked" | havuz degil, **kilit** | `busy_timeout` + WAL |
| Sorgu surekli 15 sn'de timeout | plan / kilit ya da havuz beklemesi | `DbTimeouts` + log incelemesi |

> **Mudahale (test / ortam):** `SqlConnection.ClearAllPools()` yalniz tek bir yerde cagrilir (test temizligi ya da migration oncesi) - uretimde kore cagirmak baglanti kirar.

---

## §3 Havuz ve timeout iliskisi

| Durum | Timeout davranisi | Sonuc |
|---|---|---|
| Havuz bossa | baglanti edinimi `Connect Timeout` icinde | hata erken gelir |
| Havuz dolu ve tukendiyse | beklemeyi `CommandTimeout` keser | havuz hatasi gorunur |
| `CommandTimeout = 0` | bekleme sinirsiz | ekran donar - yasak (DA-09) |
| Transaction uzun sürerse | havuz kirigi | kisa birim is (DA-13) |
| IPTal erken yapilirsa | baglanti hizla iade edilir | havuz toparlanir |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Havuzlama | `Microsoft.Data.SqlClient` 5.2.x - **var** | **ayni 5.2.x** |
| Havuz anahtarlari | ayni metin anahtarlari | ayni |
| `SqliteConnection` | saglayici **kullanilmaz** | havuzlama beklenmez; dosya / kilit yonetimi |
| `ClearPool` davranisi | ayni API | ayni |
| Olcum | performans testi her iki bacakta | ayni metrik |

---

## §5 Kardesler

[[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]]

---

## §6 Kisa karar

`using` yaz, `Max Pool Size` 100'ü elleme, `ClearAllPools` sadece test ve migration'da. Havuz hatasi gorulurse ilk bakilacak yer dispose edilmis mi diye kod taramasidir; ayar degistirmek ikinci adimdir.

---

## §7 Soru - cevap

| Soru | Cevap |
|---|---|
| `Close()` baglantiyi kapatir mi? | Hayir, havuza iade eder |
| `using` neden yeterli? | `Close()` + iade otomatiktir |
| `Max Pool Size` neden 100? | varsayilan degerdir, bilincli degistirilmez |
| SQLite'ta ayni gecerli mi? | Hayir, orada kilit ve `busy_timeout` oncedir |
| Havuz hatasi neden 15 sn'de gelir? | `DbTimeouts.Query` bekleme siniridir |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Metot disi `DbConnection` aramasi = **0** | kod taramasi |
| `CommandTimeout = 0` aramasi = **0** | kod taramasi |
| Havuz metrigi iki bacakta ayni | performans testi |
| SQLite kilit senaryosu testte geciyor | entegrasyon testi |

> `Max Pool Size` degerinin proje icin dogru olup oldugu olcusuzdur: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `connection-pooling-notes.md` | havuz davranisi, 100 varsayilani, ayar ve tesis tablolari, `ClearAllPools` uyarisi |
| `ado-net-connection.md` | `using` yasam dongusu ve DA-10 kurali |
| `command-timeout-policy.md` | `DbTimeouts.Query` 15 sn ve `0` yasagi |
| `connection-string-secrets.md` | baglanti metni ayarlari (`Connect Timeout`, `Encrypt`) |
| `dual-tfm-provider-choice.md` | SqlClient 5.2.x ortak surum, SQLite yalniz net10 test |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
