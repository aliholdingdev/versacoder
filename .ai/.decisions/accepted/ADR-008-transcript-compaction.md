---
title: "ADR-008 — Transcript Boyutu ve Compaction (8 MB Eşiği)"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "oturum transcript'inin boyut sınırı, otomatik kırpma (compaction) tetikleyicisi, özet ve arşiv kuralı"
---

# ADR-008 — Transcript Boyutu ve Compaction (8 MB Eşiği)

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-007](ADR-007-agent-hierarchy-session.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cümlelik karar:** Bir oturumun transcript dosyası **8 MB**'ı aşarsa **yerinde compaction** uygulanır: son **20 tur** + üretilen özet kalır, özgün tam metin **silinmez**, arşive taşınır — karar verildi.

---

## §0 Özet (Answer First)

Transcript sınırsız büyürse iki şey ölür: bağlam bütçesi (`AGENTS.md` §6) ve `versacoder.db` sorgu performansı. Bu ADR eşiği **8 MB** olarak sabitler ve kırpmanın **asla veri kaybı** olmadığını söyler: özgün metin `transcript-archive/` altına taşınır, aktif dosyada yalnız özet + son 20 tur kalır. Özet LLM ile üretilir; üretilemezse **yerel kural tabanlı** kırpma (ilk 2 tur + son 20 tur) devreye girer. Eşik **bayt**tır, token tahmini değildir → token karşılığı `NEEDS_VALIDATION` olarak işaretlidir.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Master prompt PART 7 transcript boyutu + compaction politikasını istiyor | `adr-index.md` §2 · ADR-008 |
| C2 | Tek turda ≤ 8 dosya okunur, alt oturum açılışı ≤ 8 KB; bütçe aşımı kaliteyi düşürür | `AGENTS.md` §6.1 |
| C3 | Oturum kayıtları `versacoder.db` içinde (Dapper); uzun transcript satırı büyütür | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 4 |
| C4 | Oturum modeli ve `session_message` bağını ADR-007 kurdu | [ADR-007](ADR-007-agent-hierarchy-session.md) D6 |
| C5 | Bilinmeyen sayı "tahmin edilir" diye yazılamaz; işaretlenmek zorunda | `CLAUDE.md` G9 |

---

## §2 Karar

**Karar:** Aktif transcript **8 MB** eşiğinde otomatik compaction'a uğrar; özet + son 20 tur kalır, özgün metin arşive taşınır ve hiçbir kayıt silinmez — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Eşik | **8 MB** / oturum (özet + ekler birlikte) | C1 |
| D2 | Tetikleyici | transcript **yazma anında** ölçülür; eşik aşılırsa yazma öncesi compaction | C1 |
| D3 | Kalan | özet (≤ 2 KB) + **son 20 tur** (orijinal metin) | C2 |
| D4 | Arşiv | özgün transcript `transcript-archive/<sessionId>-<seq>.md`'ye taşınır → **silme yok** | C1 · G7 |
| D5 | Özet üretimi | önce LLM; başarısızsa yerel kural (ilk 2 + son 20 tur, özet başlık `> compacted`) | G9 |
| D6 | Manuel | kullanıcı komutu ile eşik beklenmeden compaction (yıkıcı değil: arşiv var) | `AGENTS.md` §6.2 |
| D7 | Arşiv üst sınırı | arşiv **sınırlandırılmaz**; yalnız `session_message` aktif tablosu kırpılır | C3 |

**Neden:** Token tahmini her sağlayıcıda farklı olduğundan (bkz. [ADR-011](ADR-011-llm-provider-order.md)) **bayt eşiği** tek ölçülebilir ortak ölçüttür ve `Get-Item … .Length` ile denetlenir. Silme yasağı, denetim izini korur: kırpma = **organize etme**, yok etme değil. Yerel yedeğin (D5) bulunması, LLM sağlayıcısı düşse bile döngünün kilitlenmemesini sağlar.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Boyut denetimi tek komutla kanıtlanır | `Get-ChildItem transcript -Filter *.md \| Where Length -gt 8MB` = 0 |
| Kayıp yok → denetim ve geri alma mümkün | arşiv dosyası mevcut (D4) |
| Sağlayıcı bağımsız (token hesabı yok) | bayt ölçümü, `AGENTS.md` §6 |
| Oturum açılışı hızlı kalır | aktif transcript ≤ 8 MB |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Özet, ayrıntıyı kaybedebilir (özellikli araç çıktıları) | özgün metin arşivde; gerektiğinde okunur |
| Compaction = LLM çağrısı → gecikme + maliyet | eşik aşılmadan çağrılmaz; yerel yedek (D5) |
| 8 MB'ın token karşılığı bilinmiyor | **NEEDS_VALIDATION** (aşağıda) |
| Arşiv sonsuz büyüyebilir | bilinçli karar (D7); saklama politikası ayrı iş |

**NEEDS_VALIDATION:** 8 MB'ın gerçek token karşılığı sağlayıcıya göre değişir → PART 7'de `OpenRouter /free` ve `Ollama` ile ölçülür, sonuç `reports/` raporuna yapıştırılır. Ölçüm gelene kadar **eşik 8 MB'da kalır**, token tarafına çevrilmez.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-007](ADR-007-agent-hierarchy-session.md) | related | oturum modeli, `session_message` tablosu ve bütçe kuralları |
| [ADR-011](ADR-011-llm-provider-order.md) | related | özet LLM çağrısının sağlayıcı sırası |
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | related | kalıcı saklama yolu (Dapper + saf SQL) |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | related | compaction dosya işlemleri de sandbox kurallarına tabi |
| [ADR-021](ADR-021-vault-ssot-single-index.md) | related | arşiv dosyaları vault MD sayımına girmez |

---

## §5 Kabul Kriterleri

- [ ] Eşik sabit kodda `8 * 1024 * 1024` olarak tanımlı (magic number değil, adlandırılmış sabit)
- [ ] Compaction sonrası aktif dosya ≤ 8 MB (ölçüm çıktısı raporda)
- [ ] Arşiv dosyası oluşmadan silme **yapılmaz** (test: arşiv yoksa işlem reddedilir)
- [ ] LLM özeti başarısızsa yerel kuralın devreye girdiğini gösteren test
- [ ] Her iki bacakta aynı kırpma sonucu (kontrakt testi)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + `AGENTS.md` §6.
