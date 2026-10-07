---
title: "VersaCoder — Migration Şartnamesi (Saf SQL + Sürüm Tablosu)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Saf SQL migration dosyaları, `__Migrations` sürüm tablosu, checksum kilidi ve geri alma (down) politikası — EF Core'suz şema değişimi"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Migration Şartnamesi (`spec/migration-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [architecture/index](../architecture/index.md) · [data-layer-spec](data-layer-spec.md) · [host-layer-spec](host-layer-spec.md) · [deployment-spec](deployment-spec.md)

> **Tek cümlelik sorumluluk:** *Şema nasıl değişir, hangi sıra ve checksum ile uygulanır, hata/geri alma durumunda ne olur* sorusunun tek cevabıdır.
> Dosya adlandırma ve tablo kolonlarının **gözlemlenmiş kaynağı** `.ai/.sql/README.md`'dir; bu belge **davranış sözleşmesidir** (kaynakla çelişirse `NEEDS_VALIDATION` + `architect`).

---

## §0 Özet (Answer First)

Şema değişimi **yalnız saf SQL dosyalarıyla** olur (EF Core `migrate` **yok**, G2): dosyalar sırayla uygulanır, her biri `__Migrations` tablosuna **checksum (SHA-256) ile** kaydedilir; uygulanmış dosyanın içeriği sonradan değişmişse uygulama **başlamaz** (fail-fast), her migration'ın **`down` bloğu ya da gerekçeli `irreversible` notu** vardır ve yıkıcı migration **onaysız çalışmaz** (SEC-15, G7).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Migration dosyası davranışı, sıralama, idempotentlik | Sorgu/transaction kuralları: [data-layer-spec](data-layer-spec.md) |
| 2 | `__Migrations` sürüm tablosu + checksum kilidi | Şemanın **kullanan** kodu: [infra-layer-spec](infra-layer-spec.md) |
| 3 | Geri alma (down) ve yedek politikası | Dağıtımda `sql/` yerleşimi: [deployment-spec](deployment-spec.md) |
| 4 | Uygulama zamanı (açılışta) ve timeout | Açılış sırası: [host-layer-spec](host-layer-spec.md) |
| 5 | Yıkıcı işlem onayı (DROP/ALTER genişlemesi) | Onay mercii: [WORKFLOW](../governance/workflow.md) §3 |

> **YASAK:** EF Core migration · kod içinde `ALTER/CREATE/ DROP` çalıştırma (DA-07) · uygulanmış dosyayı sonradan değiştirmek · string birleştirme ile SQL · onaysız yıkıcı migration (SEC-15).

---

## §2 Davranış

### §2.1 Sürüm tablosu (`__Migrations`)

| Kolon | Tip | Anlam |
|---|---|---|
| `Id` | INTEGER PK | Artan sıra |
| `Version` | TEXT UNIQUE | Dosya adının numara kısmı (ör. `0001`) |
| `Name` | TEXT | Örnek: `initial_schema` |
| `Checksum` | TEXT | SHA-256 dosya gövdesi — **değişen dosya reddedilir** |
| `AppliedAtUtc` | TEXT | ISO-8601 UTC |
| `ExecutionMs` | INTEGER | Süre ölçümü (performans kanıtı) |

### §2.2 Uygulama algoritması (açılışta)

```
  [1] baglantiyi ac (cozulmus secret) 
  [2] __Migrations tablosu yoksa olustur
  [3] .sql/*.sql dosyalarini SIRAYLA enumerate et
  [4] uygulanmamis dosya: transaction icinde calistir + kaydet
  [5] uygulanmis + checksum farkli  ->  DUR (app baslamaz, fail-fast)
  [6] idempotentlik: IF NOT EXISTS  ->  ikinci calistirma hata vermez
  [7] commit -> log satiri
```

| # | Kural | Değer | İhlalde |
|---|---|---|---|
| 1 | Sıra | Numara artan (`0001`, `0002`, …) | Sıra atlansa RED |
| 2 | Dosya gövdesi | Uygulandıktan sonra **kilitli** | Yeni dosya açılır, eskisi değişmez |
| 3 | Idempotentlik | `IF NOT EXISTS` / `CREATE ... IF NOT EXISTS` | İkinci çalıştırmada hata = RED |
| 4 | Timeout | Migration komutu **120 sn** (genel 15–30 sn) | Süre aşımı → hata + rapor |
| 5 | Yıkıcı iş | `DROP`/toplu veri silme = **onay** (G7/G11) | Onaysız = RED |

### §2.3 Geri alma (down)

| Durum | Davranış | Kanıt |
|---|---|---|
| `down` bloğu varsa | Dosyanın altındaki `-- down` yorumu çalıştırılır | Geri alma testi (temp DB) |
| Geri alınamazsa | Dosyada açıkça `-- irreversible` + **gerekçe** | Yedek zorunlu |
| Onaysız yıkıcı migration | Çalıştırılmaz | SEC-15 denetimi |
| Bozuk sürüm (checksum) | Uygulama **başlamaz**, net hata + kurtarma yönergesi | Fail-fast testi |

---

## §3 TFM Notu

| Bacak | Migration etkisi | Not |
|---|---|---|
| `net48` | ADO.NET provider ile aynı migration kodu | `#if` gerekmez |
| `net10.0-windows` | Aynı | Davranış farkı = RED |
| Dosyalar | `.sql` dosyaları **koda gömülmez**, çıktıya kopyalanır (`publish/sql/`) | Paketleme: [deployment-spec](deployment-spec.md) |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| MI-01 | Bozuk DB üzerinde sıfırdan kurulum | Temp SQLite → tüm migration'lar uygulanır |
| MI-02 | İkinci çalıştırma hata vermez | Aynı DB'de 2. kez = 0 hata (idempotent) |
| MI-03 | Checksum ihlali uygulamayı durdurur | Dosyayı değiştir → açılış RED |
| MI-04 | `down` bloğu geri alır | Temp DB'de up → down → şema eski hâli |
| MI-05 | String-concat SQL yok | `.sql` + `.cs` taraması = 0 satır (G2) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
