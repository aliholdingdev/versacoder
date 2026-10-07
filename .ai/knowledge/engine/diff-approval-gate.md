---
title: "diff-approval-gate - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Diff onay blogu: l2 durum makinesi, 05 -> onay -> 06 -> 04 akisi, red edilirse dosya etkisi yok, GATE-003 HARD STOP"
---

# Diff Onay Blogu ve Gate

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../definitions/vision.md) · [ADR-025](../../.decisions/accepted/adr-025-gate003-ui-hard-stop.md) · [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (engine · 5/8):** [free-model-router](free-model-router.md) · [parallel-batch-limit](parallel-batch-limit.md) · [terminal-cli-surface](terminal-cli-surface.md)

> **Tek cumlelik sorumluluk:** *Bir dosya degisikligi ne zaman yazilir, onay kimin elindedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Degisiklik onerisi **diff blogu** olarak acilir; kullanici **Onayla / Reddet** der. Onay -> `04-code-editor`'e yazim talebi; red -> **dosya etkisi geri alinmaz, cunku zaten yazilmadi**. Diff uretimi ve onay durum makinesi `l2 Application`'dadir; UI'da gate **GATE-003 = HARD STOP**tur ve kapatani **yalniz kullanici**dir (ADR-025). Onay zinciri: `05` -> onay -> `06` -> `04` (F3 akisi).

```text
+---------------------------------------------------------------+
| tool sonucu -> l2 diff uretimi -> 05-ai-chat diff blogu        |
|                                    |                           |
|                          +---------+---------+                 |
|                          |                   |                 |
|                      ONAYLA              REDDET                |
|                          |                   |                 |
|                          v                   v                 |
|                 04'e yazim talebi    dosya YAZILMADI           |
|                          |            (geri alim yok)          |
|                          v                                     |
|              GATE-003 = HARD STOP -> yalniz kullanici kapatir  |
+---------------------------------------------------------------+
```

---

## §1 Durum makinesi (l2)

| Durum | Gecis | Tetik | Sonuc |
|---|---|---|---|
| `oneri` | `inceleniyor` | diff blogu acildi | kullanici okur |
| `inceleniyor` | `onaylandi` | Onayla | `04`'e yazim talebi |
| `inceleniyor` | `reddedildi` | Reddet | dosya etkisi yok |
| `onaylandi` | `yazildi` | `04` yazimi bitti | kanit satiri eklenir |
| herhangi | `RED` | kanit yok / gate kapanmadi | F5 ile tekrar |

---

## §2 Kurallar

| # | Kural | Kaynak |
|---|---|---|
| 1 | Onay verilmeden dosya yazimi **yok** | ADR-025 |
| 2 | Reddedilen degisiklikte "geri alma" gerekmez: dosya zaten yazilmadi | VISION §2.7 adim 5 |
| 3 | UI kodu / `.Designer.cs` degisikligi bu akistan gecer: `05` -> onay -> `06` -> `04` | VISION §2.10 madde 6 (U5) |
| 4 | Diff'i ureten katman `l2`'dir; `l2` -> `l4` dogrudan ok **YOK** | ADR-028 yon kurali |
| 5 | Kanitsiz "bitti" yazisi uretilmez; gate kanit kutusu 4 satirdir | G5 / WORKFLOW §2 |

---

## §3 GATE-003 karsilastirmasi

| Alan | Ajan kutusu (AGENTS §2) | Gate kanit kutusu (WORKFLOW §2) |
|---|---|---|
| Alan sayisi | 5 (`BASLIK/KAPSAM/KARAR/RİSK/KANIT`) | 4 (`KANIT/DOSYA/EKSIK/SONRAKI`) |
| Kaptan | ajan | **yalniz kullanici** |
| Ayni sey mi? | **Hayir** — biri digerinin yerine gecmez (SSOT) | |

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Her diff blogunda Onayla / Reddet butonu acik metinli | ekran incelemesi |
| 2 | Onaysiz dosya yazimi testi: bekleme durumu | `dotnet test` |
| 3 | Red yolu testi: dosya icerigi degismemis | test ciktisi |
| 4 | `.Designer.cs` degisiklikleri F3 zincirinden gecti | onay kaydi |
| 5 | GATE-003 kapanisi kullanici ile | gate kaydi |

---

## §5 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Wizard onay adimlari | [wizard-approval-flow](wizard-approval-flow.md) |
| Streaming + diff satiri | [agent-streaming-sse](agent-streaming-sse.md) |
| Checkpoint / kanit kutusu | [session-checkpoint-persistence](session-checkpoint-persistence.md) |
| Arac sozlesmesi | [protocol-tool-schema](protocol-tool-schema.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `VISION.md` §2.7 | adim 4-5: diff blogu, onay/red, yazim talebi |
| `VISION.md` §2.10 | F3 akisi (`05` -> onay -> `06` -> `04`), U5 sahipligi |
| `.decisions\accepted\ADR-025-gate003-ui-hard-stop.md` | GATE-003 HARD STOP, yalniz kullanici |
| `.decisions\accepted\ADR-028-layered-baseline-9-projects.md` | l2 diff durum makinesi, yon kurali |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; durum makinesi gecislerinin spec karsiligi bu etiketle kalir.
