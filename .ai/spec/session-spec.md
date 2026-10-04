---
title: "VersaCoder — Oturum Şartnamesi (Session Spec)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Oturum modeli — durum makinesi (created/running/paused/completed/failed), SQLite kalıcılığı, transcript yapısı, resume/kurtarma ve eşzamanlılık kuralları (VC-SES-NNN)"
---

# VersaCoder — Oturum Şartnamesi (`spec/session-spec.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [versacoder-spec.md](./versacoder-spec.md) · [protocol-spec.md](./protocol-spec.md) · [tool-spec.md](./tool-spec.md) · [test-strategy.md](./test-strategy.md) · [../.rules/data-access-rules.md](../.rules/data-access-rules.md)

> **Tek cümlelik sorumluluk:** *Bir oturum (agent görevi) nedir, hangi durumlarda bulunur, nerede saklanır, nasıl devam ettirilir/kurtarılır, kim yazabilir* sorusunun tek cevabıdır.
> Gereksinim kimliği: **`VC-SES-NNN`**. Bu belge `keys.md` #20 (agent, session, orkestrasyon, streaming) karşılığıdır.

---

## §1 Kapsam

**İçinde:** oturum alan modeli, durum makinesi, SQLite şeması/kalıcılık, transcript (mesaj kaydı) yapısı, resume + crash recovery, eşzamanlılık.

**Dışında:** mesaj zarfı/hata kodları (`protocol-spec.md`), tool şeması+izin (`tool-spec.md`), LLM sağlayıcı seçimi (`versacoder-spec.md` OQ-2).

> **KURAL:** Oturum = **agent görevi + transcript**. Oturum verisinin **tek kaynağı SQLite**'tır; dosyaya serpiştirilmiş durum (`.session.txt` vb.) yoktur (SSOT).
> **YASAK:** Durum makinesi dışında geçiş; transcript'in düz metin dosyasında tutulması; silinmiş oturumun "yeniden canlandırılması" (yalnız arşiv/G7 onayı).

---

## §2 Alan Modeli ve Durum Makinesi

### §2.1 Alan Modeli

```
Session (oturum/gorev)
  +--------------------+------------------------------------------+
  | Id (guid)          | benzersiz anahtar                        |
  | ProjectId          | hangi proje (nullable: serbest sohbet)    |
  | Title              | kullaniciya gorunen ad                   |
  | State              | created|running|paused|completed|failed  |
  | Provider/Model     | ayarlardan anlik snapshot (§7 OQ)         |
  | CreatedAt/UpdatedAt| UTC, TimeProvider (DA-15)                |
  | StartedAt/EndedAt  | duruma gore nullable                     |
  | Version            | optimistik kilit (§6)                    |
  | ErrorCode          | failed ise VC-E-NNNN                     |
  | ParentSessionId    | resume/fork baglantisi (nullable)        |
  +--------------------+------------------------------------------+
             | 1
             |                                      ToolCall (transcript ici)
             | *                                       +-------------+
             v                                          | callId     |
  TranscriptEntry (sirali kayit)                        | tool       |
     +-----------------------------+                    | arguments  |
     | Id, SessionId, Seq          |                    | result     |
     | Kind  (§4)                  |                    | status     |
     | Role  (user/assistant/...)  |                    | durationMs |
     | Content (text/blob ref)     |                    | permission |
     | CreatedAt, Meta (json)      |                    +-------------+
     +-----------------------------+
```

### §2.2 Durum Makinesi (ASCII)

```
                    +------------------------------------------+
                    |                                          |
   [create]         v          [start]          [pause]        |
  --------->  +-----------+ ------------> +---------+ -----> +--------+
              | created   |               | running |       | paused |
              +-----------+ <------------ +----+----+ <----- +---+----+
                    |          [resume]        |  ^              |
                    |                          |  | [start: RED] |
                    |      [finish]            |  |              |
                    |      +---------+         |  |              |
                    +----->|completed| <-------+  |              |
                           +---------+            |              |
                                    [cancel/hata] |              |
                             +---------+ <--------+              |
                             | failed  | ------------------------+
                             +---------+   (retry = yeni oturum, §5.3)
                              (terminal)   [pause'dan cancel serbest]
```

### §2.3 Geçiş Tablosu

| # | Kaynak | Hedef | Tetik | Sonuç |
|---|---|---|---|---|
| T1 | — | `created` | `session.create` | transcript boş, `Version=0` |
| T2 | `created` | `running` | `session.start` | `StartedAt` yazılır |
| T3 | `running` | `paused` | `session.pause` | akış durdurulur; tool çağrıları iptal edilir |
| T4 | `paused` | `running` | `session.resume` | **tamamlanan adım tekrarlanmaz** (§5.2) |
| T5 | `running` | `completed` | görev bitişi (çekirdek) | `EndedAt` yazılır |
| T6 | `running`/`paused` | `failed` | hata ya da `session.cancel` | `ErrorCode` zorunlu |
| T7 | `completed`/`failed` | — | herhangi | **RED** `VC-E-0106` |
| T8 | `paused` | `running` (iptal yolunda) | `session.cancel` | `failed (cancelled)` — `VC-E-0503` benzeri ayrım |

> **UYARI:** `retry` = **yeni** oturum (`ParentSessionId` ile bağlı), eski oturumun durumu değiştirilmez.

---

## §3 Kalıcılık (SQLite)

### §3.1 Tablolar (şema sözleşmesi)

```sql
CREATE TABLE Sessions
(
    Id              TEXT PRIMARY KEY,          -- guid
    ProjectId       TEXT NULL,
    Title           TEXT    NOT NULL,
    State           TEXT    NOT NULL,          -- created|running|paused|completed|failed
    Provider        TEXT    NULL,
    Model           TEXT    NULL,
    CreatedAt       TEXT    NOT NULL,          -- ISO-8601 UTC
    UpdatedAt       TEXT    NOT NULL,
    StartedAt       TEXT    NULL,
    EndedAt         TEXT    NULL,
    Version         INTEGER NOT NULL DEFAULT 0,
    ErrorCode       TEXT    NULL,              -- VC-E-NNNN
    ParentSessionId TEXT    NULL
);

CREATE TABLE TranscriptEntries
(
    Id        TEXT PRIMARY KEY,
    SessionId TEXT NOT NULL REFERENCES Sessions(Id),
    Seq       INTEGER NOT NULL,                -- oturum ici siralama, 0'dan
    Kind      TEXT NOT NULL,                   -- §4
    Role      TEXT NULL,
    Content   TEXT NULL,                       -- metin (UTF-8)
    BlobRef   TEXT NULL,                       -- buyuk icerik dosya referansi
    Meta      TEXT NULL,                       -- json (tool bilgisi vb.)
    CreatedAt TEXT NOT NULL,
    UNIQUE (SessionId, Seq)
);

CREATE INDEX IX_Transcript_Session_Seq ON TranscriptEntries (SessionId, Seq);
CREATE INDEX IX_Sessions_State         ON Sessions (State, UpdatedAt);
```

### §3.2 Kalıcılık Kuralları

| # | Kural |
|---|---|
| 1 | Yazma **yalnız** `DbTransaction` içinde (DA-13); `State` + `Transcript` + `Version` birlikte değişir |
| 2 | Zaman `TimeProvider` UTC (DA-15); biçim ISO-8601 (protocol §9) |
| 3 | `Seq` oturum içinde **tekridarsız artan**; `UNIQUE(SessionId, Seq)` ihlali → `VC-E-0111` |
| 4 | Transcript metni şifreli **değil**; **secret kırmızıaltması** yazmadan önce uygulanır (SEC-08) |
| 5 | `> 64 KB` içerik `BlobRef` ile harici dosyaya (workspace altında) taşınır; DB hafif kalır |
| 6 | Silme yalnız kullanıcı onayıyla (G7); silme transcript + satırları cascade |
| 7 | Migration `V0xx__*.sql` + `__Migrations` checksum (DA-11) |

---

## §4 Transcript Yapısı

| `Kind` | `Role` | Ne taşır | Sıra |
|---|---|---|---|
| `user` | `user` | kullanıcı mesajı | gönderildiği sıra |
| `assistant` | `assistant` | model yanıtı (tamamlandıktan sonra) | sıra |
| `assistant_delta` | `assistant` | streaming parçası (opsiyonel, `protocol §8` `seq`) | akış sırasında; bitişte birleşir |
| `tool_call` | `tool` | araç + argüman özeti + `callId` | çağrı anı |
| `tool_result` | `tool` | sonuç/hata + `durationMs` | çağrının hemen ardından |
| `permission` | `system` | izin sorusu/cevabı | tool_call öncesi |
| `system` | `system` | kural/bağlam değişimi, uyarılar | olay anı |
| `error` | `system` | `VC-E-NNNN` + kısa mesaj | oluştuğu an |

```
transcript okuma sirasi (SessionId, Seq):
  0 user        "projeyi analiz et"
  1 system      "context: 3 dosya eklendi"
  2 assistant   "Dosyaları inceliyorum..."
  3 tool_call   read  {path: "..."}        callId=c-001
  4 tool_result ok    {bytes: 1042}        callId=c-001  12ms
  5 permission  ask   shell "git status"   -> allow
  6 tool_call   shell {cmd: "git status"}  callId=c-002
  7 tool_result ok                              45ms
  8 assistant   "Çalışma ağacı temiz..."
  9 error       VC-E-0301 (saglayici yok, tekrar denendi)
```

| # | Kural |
|---|---|
| 1 | Okuma **her zaman `Seq` artan**; UI ters sırayı ters çevirir |
| 2 | `tool_result`, ilgili `tool_call`'dan **asla önce** gelmez (uyumsuzsa `VC-E-0210`) |
| 3 | `error` girdisi oturumu `failed` yapmaz (uyar niteliğinde olabilir); **terminal** hata `Session.ErrorCode`'a da yazılır |
| 4 | Kırmızıaltma uygulanmış metin geri açılmaz (SEC-08) |
| 5 | Transcript **salt okunur** düzenlenemez; düzeltme = yeni `system` girdisi |

---

## §5 Resume ve Kurtarma

### §5.1 Uygulama Açılışında Kurtarma

```
uygulama acilisinda
   |
   v
SELECT Id FROM Sessions WHERE State = 'running'
   |
   +--> varsa: UPDATE Sessions
              SET State='paused', UpdatedAt=@now, Version=Version+1
              WHERE Id=@id AND State='running' AND Version=@v
   |
   v
UI: "Yarim kalan gorev bulundu (N) -> Devam et / Bırak"
```

| # | Kural |
|---|---|
| 1 | `running` ama süreç sahibi olmayan oturum → **`paused`** (asla `completed` sayılmaz) |
| 2 | Geçiş `UPDATE ... WHERE State='running'` koşullu yapılır (yarış güvenli) |
| 3 | `paused` oturum UI'da **görünür** ve bilgiyle (başlık + zaman) listelenir |
| 4 | Kurtarma yalnızca **tek** yerde (Host açılış servisi) |

### §5.2 Resume Semantiği (T4)

| Adım durumu | Resume davranışı |
|---|---|
| `tool_call` + `tool_result` kayıtlı | **tekrarlanmaz** |
| `tool_call` var, `tool_result` yok | araç **iptal edilmiş** sayılır → yeniden denenir (kullanıcı bilgilendirilir) |
| `assistant_delta` yarım | akış yeniden başlar; yarım parça `system` notuyla kapatılır |
| `permission` bekliyorsa | soru yeniden sunulur (eski cevap geçersiz) |

### §5.3 Retry / Fork

- `failed` oturum retry edilemez (T7); kullanıcı **yeni oturum** açar, `ParentSessionId` eskiyi gösterir.
- Fork (transcript kopyasıyla yeni oturum) kapsam dışıdır (→ `versacoder-spec.md` §6).

---

## §6 Eşzamanlılık Kuralları

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | **Tek yazıcı per oturum** | Aynı `SessionId` üzerinde eşzamanlı `running` yazan iki süreç yok (`VC-E-0111`) |
| 2 | **Optimistik kilit** | Her yazım `Version` artırır; `WHERE Version=@beklenen` uyuşmazsa RED (`VC-E-0111`) |
| 3 | **Sıra yazımı** | Transcript `Seq` tek bir kuyruktan atanır (oturum başına sayaç) |
| 4 | **Desktop + Cli aynı DB'yi paylaşabilir** | WAL + `busy_timeout` (DA-14); çakışma → kısa `SQLITE_BUSY` retry (≤ 3) sonra RED |
| 5 | **Okuma serbest** | UI okumaları yazıcıyı bloklamaz (WAL okuyucu-oku) |
| 6 | **Kilitli oturum** | `running` oturum ikinci `session.start` alamaz → `VC-E-0111` |
| 7 | **UI tarafı** | Event'ler tek dispatcher'dan akar; transcript listesi `Seq` ile merge edilir |

```
  Desktop (yazici A)                    Cli (yazici B)
  +------------------+                  +------------------+
  | Version=4 -> 5   |                  |  ayni oturum:     |
  | UPDATE ... v=4   |   <-- yaris -->  |  UPDATE ... v=4   |
  +------------------+                  +------------------+
       | kazanir                              | kaybeder: VC-E-0111
       v                                      v
  transcript'e yeni girdi (Seq=9)        kullaniciya hata, tekrar dene
```

---

## §7 Gereksinimler (`VC-SES-NNN`)

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-SES-001 | Oturum `created` durumunda saklanır ve listede görünür | Yeniden açılışta durum `created` |
| VC-SES-002 | Durum geçişleri yalnız §2.3 tablosundan | Geçersiz geçiş testi → `VC-E-0106`, durum değişmedi |
| VC-SES-003 | `failed` oturumda `ErrorCode` doludur | `failed` + boş kod → test RED |
| VC-SES-004 | Transcript `Seq` artan ve benzersiz | Aynı `(SessionId, Seq)` iki kez → `VC-E-0111` |
| VC-SES-005 | `tool_result`, `tool_call`'dan önce okunamaz | Sıra testi RED |
| VC-SES-006 | Yazım tek transaction | Ara durum (state değişmiş, transcript yok) testte üretilemez |
| VC-SES-007 | Açılışta `running` oturum `paused`'a düşer | Zorla kapanış simülasyonu sonrası yeniden açılış → `paused` |
| VC-SES-008 | Resume'da tamamlanan tool çağrıları tekrarlanmaz | Çağrı sayacı resume öncesi/sonrası eşit |
| VC-SES-009 | Aynı oturuma ikinci eşzamanlı yazıcı RED olur | Paralel test → biri kazanır, diğeri `VC-E-0111` |
| VC-SES-010 | Süre `EndedAt - StartedAt` doğru hesaplanır | Sabit `TimeProvider` ile birebir |
| VC-SES-011 | Transcript kırmızıaltmadan geçer | Kayıtta `sk-`/`Password=` = 0 (SEC-08) |
| VC-SES-012 | Silme onaya tabi (G7) | Onaysız silme denemesi RED |
| VC-SES-013 | `> 64 KB` içerik `BlobRef`'e taşınır | Büyük girdi DB boyutu altında kalır |
| VC-SES-014 | `paused`/`completed` oturum UI'da yeniden açılıp okunabilir | Transcript baştan sona `Seq` sırasıyla render |
| VC-SES-015 | CLI ve Desktop aynı oturum listesini görür | İki uygulama sırayla açıldığında liste eşit |

---

## §8 Açık Sorular (OPEN QUESTIONS)

| ID | Soru | Etkilediği | Karar zamanı |
|---|---|---|---|
| SQ-1 | `Provider/Model` oturuma **snapshot** mı, canlı ayar mı? (ayar değişince eski oturum ne olur?) | §2.1, VC-SES-010 | PART 5–7 |
| SQ-2 | Proje başına oturum **limiti** / arşivleme politikası (DB büyümesi) | §3, VC-SES-012 | PART 6 |
| SQ-3 | `assistant_delta`'lar kalıcı mı saklanır, birleştirilmiş `assistant` yeterli mi? | §4 | PART 7 |
| SQ-4 | Aynı anda birden fazla `running` oturum desteklenir mi? (eşzamanlı LLM çağrısı limiti) | §6 kural 6 | PART 7 |

---

## §9 Kabul Kriterleri / Doğrulama

- [ ] Durum makinesi ASCII + 8 satırlık geçiş tablosu (T1–T8)
- [ ] SQLite şeması parametreli/`TEXT` ISO-8601; `Version` kilit kolonu var
- [ ] Transcript `Kind` sözlüğü 8 değer + örnek sıra
- [ ] Resume matrisi (5.2) ve kurtarma akışı ASCII
- [ ] Eşzamanlılık 7 kural + yarış diyagramı
- [ ] `VC-SES-001..015` test edilebilir + kabul kriterli
- [ ] Açık sorular işaretli; diyagramlar saf ASCII
- [ ] Bağlantılar yalnız mevcut dosyalara; EF Core yok, ADO.NET+Dapper bağlamı korunmuş

---

## §10 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Ürün gereksinimleri (VC-PRD-020..025) | `versacoder-spec.md` |
| 2 | Mesaj/durum özetleri | `protocol-spec.md` |
| 3 | Tool sözleşmesi | `tool-spec.md` |
| 4 | Kalıcılık/transcript testi | `test-strategy.md` |
| 5 | Veri erişim kuralları (transaction/timeout/migration) | `../.rules/data-access-rules.md` |
| 6 | Anahtar haritası | `../keys.md` |
