---
title: "wizard-approval-flow - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Wizard: niyet -> 5 adim -> kademeli onay; onay verilmeden tool cagrisi ve dosya yazimi yok (ADR-009 / ADR-025)"
---

# Wizard - Adim Adim Onay Akisi

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../VISION.md) · [ADR-009](../../.decisions/accepted/ADR-009-tool-execution-sandbox.md) · [ADR-025](../../.decisions/accepted/ADR-025-gate003-ui-hard-stop.md)

> **Tek cumlelik sorumluluk:** *Kullanici niyeti hangi adimlardan gecer, hangi noktada onay zorunludur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Wizard, tek yuksek seviyeli istegi (niyet) **adimlara boler** ve her adimda acik onay ister; **onay verilmeden tool cagrisi ve dosya yazimi gerceklesmez**. Boylece "hizli olsun" diye yikici islem otomatiklesmez. Onay modelinin kendisi kilitli (ADR-009 / ADR-025); eksik olan, sihirbazin kendi adim katalogu ve durum makinesi icin ayri bir spesifikasyondur.

```text
+---------------------------------------------------------------+
| 1 Niyet -> 2 Plan -> 3 Yapilandirma -> 4 Tool gate -> 5 Sonuc  |
|    |          |            |                |            |    |
|  girdi   bilgilendirme  ACIK ONAY     kademeli onay  kanit/KIRMIZI |
|                |            |                |                 |
|        adim listesi    onaysiz kayit      yikici is =          |
|        yoksa devam YOK   YOK              ek onay              |
+---------------------------------------------------------------+
```

---

## §1 5 adim tablosu

| Adim | Ne olur | Onay turu | Engel | Kaynak |
|---|---|---|---|---|
| 1. Niyet | kullanici tek cumle yazar | Yok (girdi) | — | ADR-009 (niyet modeli) |
| 2. Plan | sihirbaz adimlari listeler | Bilgilendirme | Adim listesi yoksa devam yok | `DOĞRULAMA GEREKLI` |
| 3. Yapilandirma | sabit ayarlar gosterilir | Acik onay | Onaysiz kaydetme yok | ADR-025 |
| 4. Tool/policy gate | tool cagrisi zinciri calisir | Kademeli onay | Yikici islem = ek onay | ADR-009 |
| 5. Sonuc + kanit | build/test ciktisi eklenir | Kabul / RED | Kanitsiz "bitti" yok | G5 |

---

## §2 Onay turleri

| Tur | Ne zaman | Ekranda | Ihlalde |
|---|---|---|---|
| Girdi | adim 1 | metin alani | — |
| Bilgilendirme | adim 2 | adim listesi + devam dugmesi | devam edilemez |
| Acik onay | adim 3 | onay kutusu (ADR-025 gate'i) | kayit yok, gate RED |
| Kademeli onay | adim 4 | her tool zinciri adiminda | yikici islem durur |
| Kabul / RED | adim 5 | kanitli sonuc | kanitsiz "bitti" = G5 |

---

## §3 Sinir ve durum

| Soru | Cevap | Kaynak |
|---|---|---|
| Onay modeli kabul edildi mi? | Evet — ADR-009 / ADR-025 | `.decisions/` |
| Wizard motorunun ayri spec'i var mi? | **Hayir** | VISION §3.2 |
| Ekran nerede acilir? | `02-main-ide` kabugu icinde gecici cozumle | `DOĞRULAMA GEREKLI` tasarim karari |
| Gate'i kim kapatir? | **Yalniz kullanici** (HARD STOP) | ADR-025 |
| Kanitsiz kapatma? | G5 ihlali, gate yeniden acilir | engine §1.5 |

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Adim 3 onayi alinmadan dosya yazimi yok | kod incelemesi |
| 2 | Yikici islem (silme / reset) ek onayli | ADR-025 / G7 |
| 3 | Adim 5'te kanit (build/test ciktisi) ekli | cikti eki |
| 4 | Kanitsiz "bitti" yazisi uretilmemis | grep |
| 5 | Wizard adim katalogu spec'i varsa `DOĞRULAMA GEREKLI` kaldirilmis | dosya kontrolu |

---

## §5 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Diff onay blogu | [diff-approval-gate](diff-approval-gate.md) |
| Checkpoint / gate kapanisi | [session-checkpoint-persistence](session-checkpoint-persistence.md) |
| Arac sozlesmesi (adim 4) | [protocol-tool-schema](protocol-tool-schema.md) |
| Ekran akisi | [VISION §2](../../VISION.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `VISION.md` §3.2 | 5 adim tablosu, sinir cumlesi, gecici cozum notu |
| `.decisions\accepted\ADR-009-tool-execution-sandbox.md` | niyet modeli, kademeli onay |
| `.decisions\accepted\ADR-025-gate003-ui-hard-stop.md` | acik onay, HARD STOP |
| `engine.md` §1.5 | gate kapanisi, kanitsiz kapatma = G5 |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; wizard adim katalogu spec'i ve ekrani acilim yolu bu etiketle kalir.
