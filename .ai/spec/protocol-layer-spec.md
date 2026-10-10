---
title: "VersaCoder — Protokol Katmanı Şartnamesi (l5 Protocol)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "l5 (N33–N37) katman sınırı, sürümleme el sıkışması ve fail-closed hata davranışı — ADR-013 l5 ile tutarlı tek kaynak"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Protokol Katmanı Şartnamesi (`spec/protocol-layer-spec.md`)

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[architecture/index|architecture/index]] · [[spec/protocol-spec|protocol-spec]] · [[spec/tool-contract-spec|tool-contract-spec]] · [[spec/session-spec|session-spec]]

> **Tek cümlelik sorumluluk:** *l5 hangi mesajları/şemaları taşır, hangi katmanlara bakmaz ve sürüm uyuşmazlığında nasıl davranır* sorusunun tek cevabıdır.
> Mesaj zarfının **içeriği** (tip adları, alanlar) [[spec/protocol-spec|protocol-spec]]'tadır; bu belge **katman sınırı + sürüm kuralı** ile sınırlıdır (SSOT).

---

## §0 Özet (Answer First)

l5, **saf veri + doğrulama** katmanıdır: şema, mesaj, transcript kaydı, döngü politikası ve sürüm/hata kodunu tek yerde tutar; **l4'e (I/O), l6'ya (host) ve l7'ye (UI) bakamaz**, bağımlılık yalnız l0 arayüzlerinedir — sürüm el sıkışması başarısızsa bağlantı **açılmaz** (fail-closed), uyumsuz sürümle "yine de çalıştırma" yoktur.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | l5'in 5 alt katmanı: N33 tool-schema · N34 messages · N35 session-transcript · N36 loop-policy · N37 versioning-errors | Mesaj alanlarının **dizimi**: [[spec/protocol-spec|protocol-spec]] §4 |
| 2 | Sürüm el sıkışması (`protocol.hello`) ve uyumsuzluk davranışı | Tool şeması/izin/sandbox: [[spec/tool-contract-spec|tool-contract-spec]] |
| 3 | Fail-closed hata fırlatma kuralı ve hata kodu **sahipliği** | Hata kodu listesi (`VC-E-*`): [[spec/protocol-spec|protocol-spec]] §5 |
| 4 | Döngü politikası (step limit) ve compaction **tetikleyici** girdisi | Bütçe değerleri ve compaction akışı: [[spec/context-budget-spec|context-budget-spec]] |
| 5 | Transcript girdisinin **biçimi** (Seq, Kind) | Oturum durumu/kalıcılığı: [[spec/session-spec|session-spec]] |

> **YASAK:** l5 içinde `DbConnection`, `HttpClient`, `Process`, `Form` kullanmak; şemayı kod içinde string ile kurup doğrulamamak; bilinmeyen sürümü sessizce kabul etmek.

---

## §2 Davranış

### §2.1 Katman sınırı (ADR-013 §2.3 ile ayni yön)

| # | Düğüm | Sorumluluk | Bakabilir | Bakamaz |
|---|---|---|---|---|
| N33 | `tool-schema` | Tool tanımı + parametre şeması doğrulama | l0, BCL | l4, l6, l7 |
| N34 | `messages` | Mesaj zarfı tipleri ve okuma/yazma | l0 | l4 (transport), l7 |
| N35 | `session-transcript` | Sıralı girdi biçimi (`Seq`, `Kind`) | l0, `session-spec` | l4 (SQLite), l6 |
| N36 | `loop-policy` | Step limiti, yineleme kuralı | l0 | LLM çağrısı (l4) |
| N37 | `versioning-errors` | Sürüm eşleşmesi + hata kodu üretimi | l0 | hata kaydını diske yazmak (l4) |

### §2.2 Akış (UTF-8 kutu-cizim)

```
  l7 UI / l6 Host
        |  (yalnız nesne çağrısı)
        v
  +-----l5------------------------------------------------+
        |  1) surum eslesmesi (N37)  -- uyusmazsa  HATA    |
        |  2) sarmaclama/dogrulama  (N34/N33)              |
        |  3) loop-policy kontrolu   (N36)                  |
        |  4) transcript bicimi      (N35)                  |
        +-----+----------------------+---------------------+
              | basarili              | basarisiz (fail-closed)
              v                       v
        l4 llm-http / tool-runner    Result<T> hatasi (VC-E-*)
        (bu belge disi)              -> uygulama durdurur
```

### §2.3 Sürümleme (N37)

| # | Kural | Değer | İhlalde |
|---|---|---|---|
| V1 | Tek sürüm alanı | `protocolVersion` = `MAJOR.MINOR` (ör. `1.0`) | Alan yoksa el sıkışması RED |
| V2 | El sıkışma | Bağlantı ilk mesajında sürüm gönderilir; karşı taraf **aynı MAJOR** değilse bağlantı açılmaz | `VC-E-01xx` + oturum `failed` |
| V3 | MAJOR | Kırıcı değişiklik (alan silme/yeniden adlandırma) → **ADR + G8** onayı | Sürüm artmadan kırıcı değişiklik → `reviewer` RED |
| V4 | MINOR | Geriye uyumlu ekleme (yeni isteğe bağlı alan) | Eski istemci tarafı yeni alanı **yutar** (`additionalProperties: false` değilse) |
| V5 | PATCH | Yalnız açıklama/düzeltme; **yeni hata kodu MINOR** sayılır | Kod ekleyip sürümü değiştirmemek → sürüm sapması RED |

### §2.4 Fail-closed davranışı (N37 + N33)

| Durum | Davranış | Sonuç |
|---|---|---|
| Bilinmeyen mesaj tipi | İşlenmez, hata döner | Görev `failed` (session-spec §2.2) |
| Şema doğrulaması başarısız | Tool gövdesi **çalışmaz** | `tool-spec` §4 ile aynı kapı |
| Sürüm MAJOR farklı | Bağlantı açılmaz | Yeniden deneme yok; kullanıcıya net mesaj |
| LoopPolicy aşıldı | Yeni tool çağrısı reddedilir | task `failed`, transcript'e yazılır |
| Hata kodu bulunamadı | Uydurma kod **yazılmaz** | `UNKNOWN` + `NEEDS_VALIDATION` (G9) |

---

## §3 TFM Notu

| Bacak | l5 durumu | Not |
|---|---|---|
| `net48` | Aynı derlenir | Yalnız BCL + l0; **`#if` gerekmez** |
| `net10.0-windows` | Aynı derlenir | İkisi arasında davranış farkı varsa **önce `NEEDS_VALIDATION`** |
| Ortak kaynak | `#if` yalnız platform API farkında (ADR-001 / CS-12) | İş mantığı `#if` ile bölünmez |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| PL-01 | Geçerli sürümle el sıkışma başarılı | `ProtocolVersionTests` yeşil (net48 + net10) |
| PL-02 | MAJOR uyuşmazlığı bağlantı açtırmıyor | Fail-closed testi: hata + oturum `failed` |
| PL-03 | Bilinmeyen mesaj tipi işlenmiyor | Round-trip JSON testi (`test-strategy` §3 l5 satırı) |
| PL-04 | `#if` içi iş mantığı yok | NetArchTest + `Select-String '#if'` raporu = yalnız platform satırı |
| PL-05 | l5 → l4 referansı 0 | `Architecture/LayerRulesTests.cs` yeşil |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
