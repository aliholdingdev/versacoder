---
title: "session-checkpoint-persistence - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Checkpoint (gate karsilastirmasi), teslim kutusu, hibrit bellek (oturum ici + vault) ve oturum kapanis kayitlari"
---

# Checkpoint, Teslim ve Oturum Kaliciligi

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../VISION.md) · [CLAUDE](../../CLAUDE.md) · [spec indeks](../../spec/index.md)
**Küme + Hub:** [engine index](index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (engine · 1/8):** [agent-streaming-sse](agent-streaming-sse.md) · [diff-approval-gate](diff-approval-gate.md) · [free-model-router](free-model-router.md) · [parallel-batch-limit](parallel-batch-limit.md) · [protocol-tool-schema](protocol-tool-schema.md) · [terminal-cli-surface](terminal-cli-surface.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Bir is ne zaman "bitti" sayilir, kanit nereye yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Checkpoint, **kanitin gate kriteriyle birebir karsilastirildigi** duraktir; "neredeyse gecti" diye durum yoktur — ya kriterlerin tamami kanitlidir (GREEN) ya da is RED ile doner. Ajan checkpoint'i **hazirlar**, gate'i **kapatmaz**; kapatan her zaman **kullanici**dir. Kanitsiz kapatma G5 ihlalidir ve gate yeniden acilir. Kalicilik iki katmandadir: **oturum ici** (transcript, gecici) ve **vault** (`log.md` + `MEMORY.md`, kalici).

```text
+---------------------------------------------------------------+
| is -> CHECKPOINT (4 soru, hepsi evet degilse RED)              |
|          |                                                     |
|    +-----+------+                                              |
|    |            |                                              |
| GREEN        RED -> onceki bilesene don                        |
|    |                                                          |
|    v                                                          |
| KANIT KUTUSU (4 satir) -> kullanici gate'i KAPATIR             |
|    |                                                           |
|    v                                                           |
| log.md tek satir + MEMORY.md 1-3 madde -> oturum KAPANIR       |
+---------------------------------------------------------------+
```

---

## §1 Checkpoint'in 4 sorusu

| # | Soru | Hayir ise |
|---|---|---|
| 1 | Gate'in kabul kriteri maddenin icinde birebir sayiliyor mu? | Kanit kutusu eksik -> RED |
| 2 | Cikti **calistirilmis komutun** ekli mi (build/test/tarama)? | G5 ihlali -> madde silinir |
| 3 | Ilgili **bacaklar** da mi yesil (`net48` + `net10.0-windows`)? | Tek bacak kanit degildir (G6) |
| 4 | Bagli dosyalar (frontmatter, izinli link) temiz mi? | Bozuk link -> vault RED |

**Kanit kutusu (4 satir):**

```text
KANIT   : <komut> -> <sayilar>      (CIKTI EKI zorunlu)
DOSYA   : <yazilan dosya yollari>
EKSIK   : YOK   (varsa: DOĞRULAMA GEREKLI maddesi)
SONRAKI : <adim, ~ sure>
```

> Bu 4 satir, ajanin **5 alanli** cikti kutusuyla (AGENTS §2) **ayni sey degildir**; biri digerinin yerine gecemez (SSOT).

---

## §2 Gate tablosu

| Gate | Ne kapatir | Ureten | Kapatan |
|---|---|---|---|
| GATE-004a | Vault yesil: 500 MD · 0 bozuk link · 0 mojibake | `architect` | kullanici |
| **GATE-003** | 7 wireframe + kullanici onayi (HARD STOP) | `ui-designer` | **yalniz kullanici** |
| GATE-004 | Iskelet + 2 bacak build + ilk commit | `architect` + `developer` | kullanici |
| GATE-005 | Test + NetArchTest 0 ihlal | `tester` | kullanici |
| GATE-006 | Teslim (4 kanit satiri) | oturum | kullanici |

---

## §3 Hibrit bellek (VISION §3.1)

| Katman | Ne saklar | Ne zaman yazilir | Kanit durumu |
|---|---|---|---|
| Oturum ici bellek (`l6`) | transcript, gecmis durumu, tool akisi | akis surerken; 8 MB'da compaction | ADR-008 |
| Kalici vault (`.ai/`) | kural, karar, envanter, rapor | oturum sonunda, gate gecisinde | vault kanitlari |
| `MEMORY.md` | oturumlar arasi durum | oturum kapanisinda | zorunlu, silinemez |
| `log.md` | kural degisikligi + kalite raporu | degisiklik aninda, tarihli | tarihli kayit zorunlu |

**Sinir:** vault tarafi **kabul edilmis** (isleyen surec); urun ici otomatik senkron **planlidir** (`DOĞRULAMA GEREKLI`). Ikisini ayni cumleye yazmak en kolay kanitsiz iddia hatasidir.

---

## §4 Oturum kapanisi (4 zorunlu madde)

| # | Madde | Icerik | Eksikse |
|---|---|---|---|
| 1 | log.md | tek satir: ne yapildi + kanit + sonraki adim | kapanis yok |
| 2 | MEMORY.md | 1-3 madde: unutulmazsa maliyeti olan | bilgi kaybi |
| 3 | WIP durumu | WIP **varsa RED** (kapanisa izin verilmez) | kapanis RED |
| 4 | Kapanis kutusu | 4 satir: `KARAR / LOG / HAFIZA / KANIT` | kaba metin |

Devre (baska ajana gecis) ayrica **4 madde** ister: `NE URETILDI · KANIT · BILINEN EKSIK · SONRAKI ADIM`; `KANIT` yazilamiyorsa devre **G5 RED** ile geri cevrilir.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `engine.md` §1.5 | checkpoint 4 sorusu, gate tablosu, kanit kutusu |
| `engine.md` §1.6 | teslim kutusu + oncesi 5 kontrol |
| `VISION.md` §3.1 | hibrit bellek iki katman, sinir cumlesi |
| `prompts\session-kapanis-prompt.md` | kapanis 4 maddesi, WIP RED |
| `checklists\session-handoff-check.md` | devre paketi 4 maddesi, geri cevrim ornekleri |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; urun ici bellek senkronu bu etiketle kalir.
