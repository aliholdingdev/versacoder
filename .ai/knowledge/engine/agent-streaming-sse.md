---
title: "agent-streaming-sse - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Saglayici SSE akisi -> l5 IAgentStreamSink -> 05-ai-chat ekrani: 5 adimli kullanici akisi, 8 MB compaction, erisilebilirlik"
---

# Agent Streaming (SSE) Akisi

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../definitions/vision.md) · [ADR-008](../../.decisions/accepted/adr-008-transcript-compaction.md) · [ADR-011](../../.decisions/accepted/adr-011-llm-provider-order.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (engine · 1/8):** [diff-approval-gate](diff-approval-gate.md) · [free-model-router](free-model-router.md) · [parallel-batch-limit](parallel-batch-limit.md) · [protocol-tool-schema](protocol-tool-schema.md) · [session-checkpoint-persistence](session-checkpoint-persistence.md) · [terminal-cli-surface](terminal-cli-surface.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Saglayicidan gelen token akisi hangi katmandan gecer, ekranda ne gorunur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Streaming yaniti **l5 Protocol** uzerinden akar: saglayici SSE akisi `IAgentStreamSink` portuna (l0) duser, `05-ai-chat` ekrani satir satir gosterir. **Gizli islem yoktur** — tool cagrisinin kendisi de ekranda yazilir. Tampon 8 MB olursa yerinde compaction (ADR-008) calisir; saglayici sirasi ADR-011'dedir (ucretsiz -> yerel -> acik), her saglayicida **streaming zorunludur**.

```text
+---------------------------------------------------------------+
| saglayici SSE  ->  l5 Protocol  ->  IAgentStreamSink (l0 port) |
|      |                                        |               |
|      | 8 MB tampon -> yerinde compaction      v               |
|      | (ADR-008)                     05-ai-chat satiri         |
|      +--> tool satiri (gizli degil)  ->  diff onay blogu      |
+---------------------------------------------------------------+
```

---

## §1 5 adimli kullanici akisi (VISION §2.7)

| # | Adim | Ne olur | Katman |
|---|---|---|---|
| 1 | Istek | kullanici yazar; `l6` oturumu baslatir, `l5` mesaja cevirir | l6 -> l5 |
| 2 | Akis | saglayici yaniti streaming akar; 8 MB'da yerinde compaction | l5 (+ ADR-008) |
| 3 | Tool | cagri satir olarak gosterilir, sonuc satirin altinda ozetlenir | l5 |
| 4 | Diff | degisiklik onerisi diff blogu acilir; onay / red | l2 durum makinesi |
| 5 | Sonuc | onay -> `04`'e yazim; red -> dosya etkisi geri alinmaz (zaten yazilmadi) | l2 -> 04 |

---

## §2 Besleyen katmanlar (ADR-028)

| Katman | Bu akistaki gorevi |
|---|---|
| `l5` Protocol | Mesaj semasi, tool sozlesmesi, SSE streaming |
| `l6` Host | Oturum, saglayici secimi, yeniden deneme |
| `l2` Application | Diff uretimi ve onay durum makinesi |
| `l3` Crosscutting | Tool cagrisi ve karar log'u |

---

## §3 Gelen veri tablosu

| Veri | Gelen katman | Kaynak | Kanit durumu |
|---|---|---|---|
| Streaming yanit | `l5` | Saglayici SSE akisi | ADR-011 (ucretsiz -> yerel -> acik) |
| Tool cagrisi satiri | `l5` | Tool sozlesmesi | ADR-006 |
| Diff onerisi | `l5` -> `l2` | Tool sonucu + yerel karsilastirma | `DOĞRULAMA GEREKLI` |
| Baglam / transcript | `l6` | Oturum ici bellek | ADR-008 (8 MB compaction) |

---

## §4 Erisilebilirlik ve hata kurallari

| # | Kural | Kaynak |
|---|---|---|
| 1 | Token akisinda okuyucu surekli yeniden okumaz; yanit tamaminda **odakla** bildirilir | VISION §2.7 (`DOĞRULAMA GEREKLI` olcum) |
| 2 | Tool satiri yalniz ikon degil, acik metin tasiyor | VISION §2.7 |
| 3 | Diff onay butonu etiketi eylem adidir (Onayla / Reddet) | VISION §2.7 |
| 4 | Saglayici hatasi degerdir; dongu cokmez, `ToolError`/red olarak doner | ADR-006 D4 |
| 5 | Model erisilemezse istek **red** edilir, "gecici" denip gecilmez | AGENTS §5.1 (2026-09-29) |

---

## §5 Iki bacak

| Konu | net48 | net10.0-windows |
|---|---|---|
| SSE istemcisi | `l5` ayni sozlesme, iki bacakta ayni imza | ayni |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalir |
| Tampon bellek | framework GC | .NET 10 GC farkli -> olcum ayri yapilir |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `VISION.md` §2.7 | 5 adimli akis, katman tablosu, gelen veri tablosu, erisilebilirlik notu |
| `.decisions\accepted\ADR-028-layered-baseline-9-projects.md` | l5 N33-N37, `IAgentStreamSink` l0 portu |
| `.decisions\accepted\ADR-008-transcript-compaction.md` | 8 MB yerinde compaction |
| `.decisions\accepted\ADR-011-llm-provider-order.md` | saglayici sirasi, streaming zorunlulugu |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; odakla bildirimi olcumu ve diff uretim yolu bu etiketle kalir.
