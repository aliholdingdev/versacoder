---
title: "free-model-router - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Ucretsiz model router: saglayici sirasi (OpenRouter /free -> Ollama -> OpenAI-uyumlu uc), streaming zorunlulugu, anahtar DPAPI, offline-first"
---

# Free-Model Router - Saglayici Sirasi

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../definitions/vision.md) · [ADR-011](../../.decisions/accepted/adr-011-llm-provider-order.md) · [security-rules](../../.rules/security-rules.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (engine · 1/8):** [agent-streaming-sse](agent-streaming-sse.md) · [diff-approval-gate](diff-approval-gate.md) · [parallel-batch-limit](parallel-batch-limit.md) · [protocol-tool-schema](protocol-tool-schema.md) · [session-checkpoint-persistence](session-checkpoint-persistence.md) · [terminal-cli-surface](terminal-cli-surface.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Istek hangi sirayla saglayicilara gider, ag kesilince ne olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Router tek saglayiciya baglanmaz: once **OpenRouter `/free`** havuzu, sonra **yerel Ollama**, sonda **OpenAI-uyumlu uc** denenir; her adimda **streaming zorunludur**, anahtar **yalniz sifreli depodan** (DPAPI) okunur. Gercek faydasi, ag kesilince isin durmasi degil, **yerel modelle akisin surmesidir** (offline-first). Erisilemeyen model **red** edilir; "gecici" denip gecilmez (2026-09-29 kaza kaydi).

```text
+---------------------------------------------------------------+
| 1) OpenRouter /free   -> uygunsa akis baslar                  |
|      |  red/hata                                         |
|      v                                                      |
|  2) Ollama (yerel)    -> ag yokken anahtar gerekmez          |
|      |  red/hata                                         |
|      v                                                      |
|  3) OpenAI-uyumlu uc  -> yapilandirmissa                     |
|      |                                                     |
|      v                                                     |
|  HEPSI RED -> istek RED (Model unavailable), dongu cokmez     |
|  her adimda: streaming ZORUNLU + anahtar = DPAPI             |
+---------------------------------------------------------------+
```

---

## §1 Sira tablosu

| # | Sira | Saglayici | Ne zaman devrede | Anahtar kaynagi | Kanit durumu |
|---|---|---|---|---|---|
| 1 | Birinci | OpenRouter `/free` | ucretsiz havuz erisilebilirken | sifreli depo (DPAPI) | Sira ADR-011 |
| 2 | Ikinci | Ollama (yerel) | ag yokken / birinciden donuyorken | yerel uc nokta (anahtar yok) | offline-first davranis |
| 3 | Ucuncu | OpenAI-uyumlu uc | yapilandirmissa | sifreli depo | `DOĞRULAMA GEREKLI` |
| 4 | Hepsi | Streaming zorunlu | her saglayicida | — | ADR-011 |
| 5 | Red | Erisilemeyen model | istek aninda | — | 2026-09-29 kaza kaydi |

**Sinir:** sira **kabul edilmistir**; ucretsiz model havuzunun guncel envanteri **planlidir** -> envanter yoksa `07-settings` listesi elle dogrulanmalidir (`DOĞRULAMA GEREKLI`).

---

## §2 Katman gorevi (ADR-028)

| Katman | Gorev |
|---|---|
| `l5` Protocol | saglayici uc noktasi ve streaming yapilandirmasi, SSE akisi |
| `l6` Host | saglayici secimi, yeniden deneme, oturum |
| `l4` Infrastructure | DPAPI secret store + saglayici soyutlamasi |
| `l7` UI (`07-settings`) | model/endpoint secimi, baglanti testi rozeti |

Anahtar **asla duz metin log'a yazilmaz**; "Baglanti testi" dugmesi sonucu rozet olarak doner ve saglayici erisilemezse yalniz o saglayici pasifleseur, ekran calismaz kalir.

---

## §3 Kural tablosu

| # | Kural | Kaynak |
|---|---|---|
| 1 | Sira ADR-011'dedir; istek sirayi atlamaz | ADR-011 |
| 2 | Her saglayicida streaming zorunlu | VISION §3.4 |
| 3 | Anahtar yalniz sifreli depodan (DPAPI), log'a degil | security-rules / ADR-018 |
| 4 | Model erisilemezse istek **red**; "gecici" deneme strike degil ama `UNKNOWN` raporlanir | AGENTS §4.3 / §5.1 |
| 5 | Erisilemez saglayici pasifleseir; ekran/konsol calismaya devam eder | VISION §2.9 |

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Sira kodda ADR-011 sirasina uygun | kod incelemesi |
| 2 | Anahtar okuma noktasi DPAPI depodan | kod incelemesi |
| 3 | Log ciktisinda anahtar degeri = 0 | grep |
| 4 | Ag yokken yerel saglayici ile akis suruyor | elle test (`DOĞRULAMA GEREKLI`) |
| 5 | Red yolunda dongu cokmuyor, hata deger olarak donuyor | `dotnet test` |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `VISION.md` §3.4 | 5 satirlik sira tablosu, sinir cumlesi, offline-first gerekce |
| `.decisions\accepted\ADR-011-llm-provider-order.md` | saglayici sirasi karari |
| `AGENTS.md` §5.1 / §4.3 | 2026-09-29 model kaza kaydi, dissal kesinti kurali |
| `.rules\security-rules.md` | anahtar DPAPI, log'a sifir siza |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; ucretsiz havuz envanteri ve ag-kesinti elle testi bu etiketle kalir.
