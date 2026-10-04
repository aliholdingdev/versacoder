---
title: "N20 - Caching (TTL, Boyut Siniri, Eszamanlilik)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N20 - Caching (TTL, Boyut Siniri, Eszamanlilik)

**Zorunlu Baglantilar:** [index.md](index.md) | [N19-logging.md](N19-logging.md) | [N21-clock-time.md](N21-clock-time.md) | [N23-config-opts.md](N23-config-opts.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir degerin nereye kondugunu, ne zaman dusuruldugunu ve eszamanli erisimde guvenli oldugunu belirler.

## §0 Ozet (Answer First)

`ICache`, **TTL zorunlu**, **boyut siniri olan**, `ConcurrentDictionary` uzerinden **thread-safe** tek onbellektir (singleton). Anahtar kalibi `"<katman>:<islem>:<id>"`; **sonsuz TTL yasaktir**. Cache **her zaman opsiyoneldir**: cache yoksa sistem ayni sonucu porttan dogrudan alir (`NoCache` test ikizi bunu dogrular). Gecersizleme **write-through degil**: yazma sonrasi `Remove` zorunludur.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3-crosscutting |
| Proje | VersaCoder.Crosscutting |
| N kodu | N20, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Arayuz | `ICache` (singleton) |
| Varsayilan yon | yalniz l0 |

## §1 Sorumluluk

Tek sorumluluk: **guvenli, sinirli, opsiyonel onbellek**.

| Imza | Davranis | Not |
|---|---|---|
| `bool TryGet<T>(string key, out T value)` | anahtar yoksa/dusmusse `false` | **exception firlatmaz** |
| `void Set<T>(string key, T value, TimeSpan ttl)` | TTL **zorunlu** | `ttl <= 0` gecersiz |
| `void Remove(string key)` | tek anahtar siler | yazma sonrasi zorunlu |
| `void Clear()` | tamami | test/oturum sonu |

| Kural | Deger |
|---|---|
| Anahtar | `l2:list:projects` (`katman:islem:id`) |
| TTL | zorunlu, kisa (sn/dk) |
| Boyut | ust sinir dolunca eski kayit dususur |
| Eszamanlilik | `ConcurrentDictionary` + kilit |
| Gecersizleme | yazma sonrasi **`Remove`** |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Anahtar + deger + TTL | tuketici (l2, l5) |
| GIRENLER | Sahte saat (TTL testi) | test |
| GIRENLER | Port cagrisi (miss durumu) | l0 port (uygulama l4) |
| CIKANLAR | Hizli DTO donusu (hit) | tuketici |
| CIKANLAR | `false` (miss, hata degil) | tuketici portu cagirir |
| CIKANLAR | Bellek siniri davranisi | ic kural |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1/l2/l4..l7 | X | nötr kalmak (matris `l3 yalniz l0`) |
| Sonsuz TTL | X | bellek sizintisi |
| Statik `Dictionary` | X | eszamanlilik + sizinti |
| `DateTime.Now` ile TTL | X | iki bacak farki; sayac/saat sahtesi |
| NuGet caching paketi | X | G8 |

Izinli: l3 -> l0 (`Result`); tuketim ters ok ile.

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N20 | Caching | `ICache` + `MemoryCache` | VersaCoder.Crosscutting |
| N21 | Clock-Time | TTL testi icin saat | VersaCoder.Crosscutting |
| N05 | Constants | Anahtar fabrikasi (l0) | VersaCoder.Abstractions |
| N13 | Queries | Tuketici (l2) | VersaCoder.Application |
| N39 | DI registration | Singleton kaydi | VersaCoder.Host |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, anahtar sabitleri | asagi | tek izinli |
| l1..l7 | YOK | nötr |
| `System.Collections.Concurrent` | ic | BCL, ek paket yok |
| NuGet caching | YOK | G8 |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | TTL bitisi -> `false` (sabit saat ile) | once RED |
| Unit | Boyut siniri asilinca eski kayit duser | 100+ kayit |
| Unit | Eszamanli `Set`/`TryGet` | 8 parcacik |
| Unit | Anahtar kalibi `katman:islem:id` | regex |
| Unit | `NoCache` ile sistem ayni sonucu verir | opsiyonel olma |

## §7 Hata Durumlari

| Durum | Sonuc | Koruma |
|---|---|---|
| Anahtar yok / TTL bitti | `TryGet = false` (hata degil) | porttan oku |
| TTL `<= 0` | programlama hatasi | guard |
| Boyut siniri asildi | eski kayit dususu | ic kural, test edilir |
| Eszamanli `Set`+`TryGet` | tutarli okuma | `ConcurrentDictionary` |
| Yazma sonrasi silinmedi | bayat ekran | **`Remove` zorunlu** |

## §8 Kurallar (5 madde)

- Her `Set`'te TTL ver; sonsuz TTL yazma.
- Anahtari `katman:islem:id` kalibina uydur (fabrikadan uret).
- Yazmadan sonra `Remove` ile ilgili anahtarlari dus.
- Cache'i opsiyonel tasarla; `NoCache` ile ayni sonucu dogrula.
- TTL testini `DateTime.Now` ile **olcme**; sabit saat/sayac kullan.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
