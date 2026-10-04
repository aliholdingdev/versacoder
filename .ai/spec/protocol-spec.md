---
title: "VersaCoder — Protokol Şartnamesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Protocol katmanı sözleşmesi — mesaj zarfı (envelope), oturum yaşam döngüsü mesajları, tool-call contract, hata kodları (VC-E-NNNN), opencode v1.18.32 sürüm uyumu, serileştirme ve timeout/retry kuralları (VC-PRC-NNN)"
---

# VersaCoder — Protokol Şartnamesi (`spec/protocol-spec.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [versacoder-spec.md](./versacoder-spec.md) · [session-spec.md](./session-spec.md) · [tool-spec.md](./tool-spec.md) · [test-strategy.md](./test-strategy.md) · [../reports/00-kapsam-analizi.md](../reports/00-kapsam-analizi.md)

> **Tek cümlelik sorumluluk:** *VersaCoder bileşenleri (UI ↔ Host ↔ Protocol ↔ agent çekirdeği ↔ LLM gateway) birbirine hangi mesajlarla, hangi kurallarla konuşur* sorusunun tek cevabıdır.
> Gereksinim kimliği: **`VC-PRC-NNN`** · Hata kodu: **`VC-E-NNNN`** · Bu belge `keys.md` #16 (Protocol, mesaj şeması) karşılığıdır.

---

## §1 Kapsam ve Hedefler

**İçinde:** mesaj zarfı, mesaj türleri, oturum yaşam döngüsü sinyalleri, tool-call çevrimi, hata kodu sistemi, sürüm/uyumluluk, serileştirme, timeout/retry.

**Dışında:** oturum *durum modeli* (`session-spec.md`), tool *şeması ve izinleri* (`tool-spec.md`), LLM sağlayıcı seçimi (`versacoder-spec.md` OQ-2).

> **KURAL:** Protokol **sözleşmedir**: iki taraf da tek belgeden okur; taraflardan biri kendi haline ek yapamaz (SSOT).
> **YASAK:** Bilinmeyen alan/tip sessizce yutulup **davranış** üretmesi; hata kodu uydurmak (yalnız `VC-E-NNNN` kayıtlı).

### §1.1 Referans Uyumu

| | |
|---|---|
| Referans | opencode mimarisi **v1.18.32** (6 ana paket, `packages/protocol` dâhil) |
| Dayanak | `REFERENCE_VERIFIED` — `reports/00-kapsam-analizi.md` §1.3 |
| Kural | Mesaj **biçimleri** mümkün olduğunca referansla hizalı; farklar §7.2'de listelenir |
| Yasak | Referans sürüm adı/tipi/sayısını **doğrulanmadan** "birebir aynı" iddia etmek (G9) |

---

## §2 Mesaj Zarfı (Envelope)

### §2.1 ASCII Şema

```
+--------------------------------------------------------------+
| Envelope (tek nesne, her mesaj icin ayni kabuk)               |
|                                                              |
|  protocol   : "1.0"        sozlesme surumu (§7)              |
|  id         : "<guid>"     mesaj id - idempotency (§8.3)     |
|  type       : "<mesaj tipi>"            (§3 tablosu)         |
|  sessionId  : "<guid>|null" oturum baglantisi (varsa)        |
|  ts         : ISO-8601 UTC zaman damgasi                     |
|  replyTo    : "<guid>|null" istek-yanit eslesmesi            |
|  payload    : { ... }      tipli govde (§4)                  |
|  error      : { ... } | null  yalniz hata mesajlarinda       |
+--------------------------------------------------------------+
        |                                     ^
        v                                     |
  +-----------+     +------------+     +-------------+
  | UI / CLI  | --> |  Protocol  | --> | Agent core  |
  +-----------+     |  (katman)  |     |  (calistirici)|
        ^           +-----+------+     +------+------+
        |                 |                   |
        |  olay (event)    |  saglayici istek  | tool.call
        |                 v                   v
        |           +--------------+   +---------------+
        +-----------|  LLM gateway |   | Tool sistemi  |
                    +--------------+   +---------------+
```

### §2.2 Örnek Zarf

```json
{
  "protocol": "1.0",
  "id": "8f0f2f6e-2a51-4d4e-9a11-0b7a3a4c1d21",
  "type": "tool.call",
  "sessionId": "c1b1d0a2-6b8e-4a5c-9a17-3f6a0d2e77aa",
  "ts": "2026-09-25T10:15:03.412Z",
  "replyTo": null,
  "payload": {
    "callId": "call-00042",
    "tool": "read",
    "arguments": { "path": "src/VersaCoder.Domain/Session.cs" }
  },
  "error": null
}
```

> **YASAK:** Zarf dışı alan eklemek; `payload` içinde ikinci bir zarf; ikili (binary) gövde.

---

## §3 Mesaj Türleri ve Oturum Yaşam Döngüsü

| `type` | Yön | Anlamı | Oturuma bağ |
|---|---|---|---|
| `session.create` | istemci → çekirdek | yeni oturum/görev aç | — |
| `session.start` | istemci → çekirdek | `created → running` | evet |
| `session.pause` | istemci → çekirdek | `running → paused` | evet |
| `session.resume` | istemci → çekirdek | `paused → running` | evet |
| `session.cancel` | istemci → çekirdek | iptal (`→ failed/cancelled`) | evet |
| `session.event` | çekirdek → istemci | durum/ilerleme değişimi (streaming) | evet |
| `chat.message` | istemci → çekirdek | kullanıcı mesajı | evet |
| `chat.chunk` | çekirdek → istemci | streaming yanıt parçası | evet |
| `tool.call` | çekirdek → araç | tool çağrısı isteği | evet |
| `tool.result` | araç → çekirdek | tool sonucu/hatası | evet |
| `permission.request` | çekirdek → istemci | izin sorusu (`ask`) | evet |
| `error` | herhangi → karşı | hata (`error` bloğu dolu) | opsiyonel |
| `ping` / `pong` | iki yön | canlılık + RTT ölçümü | hayır |
| `protocol.hello` | el sıkışma | sürüm + yetenek müzakeresi (§7) | hayır |

### §3.1 Durum Geçiş Tablosu (özet — tamı `session-spec.md`)

```
        create           start            pause
  (yok) ------> created ------> running ------> paused
                            |      ^   |          |
                            |      |   | resume   | start (kural disi->RED)
                            |      +---+ <--------+
                            |
                cancel      |      finish
                            +--> failed ------> completed
                                 (hata/iptal)   (basari)
```

| # | Geçiş | Tetikleyen mesaj | Geçersizse |
|---|---|---|---|
| 1 | `created → running` | `session.start` | `VC-E-0101` |
| 2 | `running → paused` | `session.pause` | `VC-E-0102` |
| 3 | `paused → running` | `session.resume` | `VC-E-0103` |
| 4 | `running/paused → failed` | `session.cancel` ya da hata | `VC-E-0104` |
| 5 | `running → completed` | görev bitişi (çekirdek) | `VC-E-0105` |
| 6 | `completed/failed → *` | — | **RED**: terminal durum |

---

## §4 Tool-Call Çevrimi (özet — tamı `tool-spec.md`)

```
  cekirdek                      izin katmani                 arac
     |  tool.call(callId,tool,args)   |                         |
     |------------------------------->|  allow / ask / deny     |
     |                                |--- ask --> permission.  |
     |                                |<--------- request       |
     |                                |--- allow -------------->|
     |                                |                         |
     |  tool.result(callId, ok|error, content, durationMs)      |
     |<-------------------------------|<------------------------|
```

| Kural | Değer |
|---|---|
| `callId` | çekirdek üretir, **tekrarsız**; `tool.result` aynı `callId` ile döner |
| Zaman aşımı | tool başına `ToolTimeouts` (varsayılan 30 sn; shell 120 sn — `tool-spec`) |
| İzin | `permission.request` içinde araç + özet argüman; cevap `allow/ask/deny` |
| Hata | `tool.result.error = { code: "VC-E-02xx", message }` — model'e veri olarak döner (SEC-14) |
| Tekrar | Aynı `callId` için **ikinci** `tool.result` sayılır ve reddedilir (`VC-E-0210`) |

---

## §5 Hata Kodları (`VC-E-NNNN`)

| Aralık | Alan | Örnek kod | Anlamı |
|---|---|---|---|
| 0001–0099 | protokol/serileştirme | `VC-E-0001` geçersiz JSON · `VC-E-0002` zarf alanı eksik · `VC-E-0003` bilinmeyen `type` · `VC-E-0004` sürüm uyuşmazlığı · `VC-E-0005` zaman aşımı · `VC-E-0006` tekrar (aynı `id`) | bağlantı **kapanmaz**, mesaj reddedilir |
| 0100–0199 | oturum | `VC-E-0101..0106` geçersiz durum geçişi · `VC-E-0110` oturum bulunamadı · `VC-E-0111` kilitli (yazıcı var) | oturum durumu korunur |
| 0200–0299 | tool | `VC-E-0201` bilinmeyen araç · `VC-E-0202` parametre şeması geçersiz · `VC-E-0203` **izin reddedildi** · `VC-E-0204` yol sınırı dışı · `VC-E-0205` tool timeout · `VC-E-0210` mükerrer sonuç | görev `failed` ya da devam (araç bağımlı) |
| 0300–0399 | sağlayıcı/LLM | `VC-E-0301` sağlayıcıya ulaşılamadı · `VC-E-0302` yetkisiz · `VC-E-0303` rate limit · `VC-E-0304` geçersiz istek · `VC-E-0305` akış koptu | retry (§8.2) uygunsa denenir |
| 0400–0499 | veri/depolama | `VC-E-0401` migration checksum uyuşmazlığı · `VC-E-0402` bağlantı yok · `VC-E-0403` constraint · `VC-E-0404` timeout | fail fast (DA-09/DA-11) |
| 0500–0599 | güvenlik | `VC-E-0501` secret çözülemedi · `VC-E-0502` yetki yok · `VC-E-0503` yıkıcı işlem onaysız | **RED**, loglanır (audit) |

```json
{
  "type": "error",
  "payload": null,
  "error": {
    "code": "VC-E-0203",
    "message": "Araç izni reddedildi: shell",
    "retryable": false,
    "detail": "denied-by-user"
  }
}
```

> **KURAL:** `message` kullanıcıya görünebilir (Türkçe), `detail` dahili (İngilizce, secret içermeyecek — SEC-13).
> **YASAK:** Kod uydurmak; stack trace / SQL metnini `message`/`detail`'e koymak.

---

## §6 Gereksinimler (`VC-PRC-NNN`)

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRC-001 | Her mesaj §2.1 zarfını kullanır | Eksik `id/type/protocol` → `VC-E-0002`, kabul edilmez |
| VC-PRC-002 | `id` istemci tarafında üretilir ve benzersizdir | Aynı `id` ile tekrar → `VC-E-0006`, ikinci iş yapılmaz |
| VC-PRC-003 | `sessionId` taşımayan tek tip: `ping`/`pong`/`protocol.hello` | Oturumsuz başka mesaj → `VC-E-0002` |
| VC-PRC-004 | Durum geçişleri §3.1 tablosuna uyar | Geçersiz geçiş mesajı → ilgili `VC-E-01xx`; durum **değişmez** |
| VC-PRC-005 | `tool.call`/`tool.result` çifti `callId` ile eşleşir | Eşleşmeyen sonuç → `VC-E-0210`; mükerrer sonuç reddedilir |
| VC-PRC-006 | İzin `ask` olan tool, sonuçtan **önce** `permission.request` gönderir | `ask` aracı onaysız çalışırsa test RED |
| VC-PRC-007 | Hata mesajları yalnız kayıtlı `VC-E-NNNN` kodunu kullanır | Kayıtsız kod → test RED |
| VC-PRC-008 | Bağlantı açılışında `protocol.hello` ile sürüm müzakeresi yapılır | Uyumsuz major sürüm → `VC-E-0004`, kibar kapanış |
| VC-PRC-009 | Bilinmeyen `type` → `VC-E-0003` ama bağlantı **kopmaz** | Bağlantı açık kalır; istemci hatayı görür |
| VC-PRC-010 | Bilinmeyen **alan** yok sayılır (ileri uyum) | Eklenen yeni alan eski istemciyi kırmaz |
| VC-PRC-011 | Serileştirme §9 kurallarına uyar (UTF-8 JSON, camelCase, ISO-8601 UTC) | Round-trip testi birebir aynı mantık verisi |
| VC-PRC-012 | `ping` en az 15 sn'de bir; `pong` 5 sn içinde | Süreyi aşan bağlantı kopmuş sayılır |
| VC-PRC-013 | Zaman aşımı ve retry §8'e uyar | `retryable:false` asla tekrar denenmez |
| VC-PRC-014 | Akış (`chat.chunk`) sıralıdır: aynı `id` içinde artan `seq` | Sıra atlanırsa istemci boşluk/düzeltme bildirir |
| VC-PRC-015 | Zarf içinde secret/connection string bulunmaz | Tarama: `Password=`/`sk-` = 0 (SEC-08) |

---

## §7 Sürüm ve Uyumluluk

### §7.1 Sürümleme Kuralları

| # | Kural |
|---|---|
| 1 | `protocol` alanı `"MAJOR.MINOR"` biçimindedir; başlangıç **`1.0`** |
| 2 | **MINOR**: geri uyumlu ekleme (yeni isteğe bağlı alan/yeni `type`) → eski taraf bilinmeyeni yok sayar (PRC-010) |
| 3 | **MAJOR**: alan silme/değiştirme/semantik değişimi → `protocol.hello` müzakeresi reddeder (`VC-E-0004`) |
| 4 | Desteklenen major listesi tek yerde sabittir (Protocol katmanı sabiti) |
| 5 | Uyumluluk testi: önceki minor sürümle **kayıtlı mesaj fixture**'ları round-trip eder (test-strategy §regresyon) |

### §7.2 opencode v1.18.32 Referans Farkları

| # | Konu | Referans (v1.18.32) | VersaCoder kararı | Dayanak |
|---|---|---|---|---|
| 1 | Paket düzeni | `packages/protocol` + 32 paket daha | Karar: tek `VersaCoder.Protocol` assembly | `REFERENCE_VERIFIED` (rapor §1.3) |
| 2 | Mesaj zarfı alanları | opencode şeması okunup hizalanacak | Bu belge §2 **esas**; farklar burada listelenir | `NEEDS_VALIDATION` — PART 7 |
| 3 | Tool adları | `read`/`write`/`edit`/`bash`/`grep` benzeri | Aynı **anlamda** adlar (`tool-spec` §6) | `SOURCE_VERIFIED` (rapor §1.3 tool set) |
| 4 | Yeni alanlar | — | `VC-E-NNNN` hata kodu uzayı **VersaCoder'a özgü** | — |

> **UYARI:** Satır 2'de `NEEDS_VALIDATION`: opencode `packages/protocol` içeriği PART 7'de okunup bu tablo güncellenir. Doğrulanmadan "birebir uyum" cümlesi yazılmaz (G9).

---

## §8 Timeout, Retry, İdempotency

### §8.1 Zaman Aşımı Değerleri

| Kapsayıcı | Varsayılan | Aşarsa |
|---|---|---|
| Bağlantı kurulumu | **10 sn** | `VC-E-0301`, retry (§8.2) |
| `ping → pong` | **5 sn** | kopmuş say, yeniden kur |
| Tool çağrısı | **30 sn** (shell **120 sn**) | `VC-E-0205`, araç iptal edilir |
| LLM ilk token | **30 sn** | `VC-E-0304`/`VC-E-0301`, retry uygunsa |
| Toplam görev | yok (kullanıcı iptal eder) | — |

### §8.2 Retry Politikası

| # | Kural |
|---|---|
| 1 | Yalnız `retryable: true` hatalar denenir (çoğunlukla `VC-E-0301/0303/0305`) |
| 2 | En fazla **3** deneme; üstel bekleme **500 ms → 1 sn → 2 sn** (+%20 rastgele saçılma) |
| 3 | `VC-E-0303` (rate limit) → sunucunun `retry-after` süresi varsa onu kullan |
| 4 | Tool çağrısı **otomatik tekrarlanmaz** (yan etki riski) — yalnız izinli ve `idempotent:true` işaretli araçlar |
| 5 | Deneme sayacı `session.event` ile kullanıcıya görünür |

### §8.3 İdempotency

```
istemci                    cekirdek
  |  id = A  (session.create)   |
  |---------------------------->|  -> oturum olustur (1. kez)
  |  id = A  (tekrar, timeout)  |
  |---------------------------->|  -> VC-E-0006 + ilk cevabi dondur
                                 |     (yeni oturum ACILMAZ)
```

---

## §9 Serileştirme Kuralları

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | Biçim | **JSON, UTF-8 (BOM'suz)** tek nesne — mesaj başına bir zarf (NDJSON akışında satır satır) |
| 2 | Adlandırma | **camelCase** alan adları; C# tarafında `PascalCase` property (dönüştürücü tek yerde) |
| 3 | Zaman | **ISO-8601 UTC**, milisaniye (`2026-09-25T10:15:03.412Z`) — `DA-15` ile aynı |
| 4 | Sayı | JSON sayı; para/ondalık **ayrı** (`decimal`, invariant kültür) — protokolde para yok |
| 5 | Binary | Base64 + `encoding` alanı; `payload` içinde ham ikili **yok** |
| 6 | Null | Eksik alan `null` değil **yok** sayılır; zorunlu alan yoksa `VC-E-0002` |
| 7 | Kütüphane | `System.Text.Json` + kayıtlı `JsonSerializerContext` (uyumluluk/kaynak üretimi) — `BinaryFormatter` vb. yasak (SEC-10) |
| 8 | Round-trip | Gönderilen mesaj alıcıda **mantık olarak birebir** çıkmalı (test: `VC-PRC-011`) |

---

## §10 Açık Sorular (OPEN QUESTIONS)

| ID | Soru | Etkilediği | Karar zamanı |
|---|---|---|---|
| OPQ-1 | opencode `packages/protocol` alan adları birebir mi alınacak, yoksa §2 zarfı mı esas? | §7.2 satır 2 | PART 7 |
| OPQ-2 | Transport: in-process event mi, named pipe/stdio mı? (masaüstü tek süreç varsayımı) | §2 diyagramı | PART 5–7 |
| OPQ-3 | NDJSON akışında çok büyük mesaj (>1 MB) kırma kuralı | §9 kural 5 | PART 7 |
| OPQ-4 | `chat.chunk` `seq` sırası bozulursa istemci kurtarma mı, yeniden talep mi yapar? | PRC-014 | PART 7 |

---

## §11 Kabul Kriterleri / Doğrulama

- [ ] §2 zarfı + örnek; §3 mesaj türleri + geçiş tablosu; §4 tool çevrimi
- [ ] Hata kodu aralıkları 6 alan, örnekler kayıtlı
- [ ] `VC-PRC-001..015` test edilebilir + kabul kriterli
- [ ] Sürüm kuralı (MAJOR/MINOR) + opencode fark tablosu `NEEDS_VALIDATION` işaretli
- [ ] Timeout/retry sayıları somut; idempotency ASCII ile gösteriliyor
- [ ] Açık sorular açıkça işaretli; diyagramlar saf ASCII
- [ ] Bağlantılar yalnız mevcut dosyalara

---

## §12 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | opencode envanteri (v1.18.32, 33 paket) | `../reports/00-kapsam-analizi.md` §1.3 |
| 2 | Anayasa | `../CLAUDE.md` |
| 3 | Oturum modeli (durum makinesi) | `session-spec.md` |
| 4 | Tool şeması/izin/sandbox | `tool-spec.md` |
| 5 | Ürün gereksinimleri | `versacoder-spec.md` |
| 6 | Protokol testleri | `test-strategy.md` |
