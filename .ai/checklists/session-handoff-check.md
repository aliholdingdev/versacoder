---
title: "Oturum Devre (Handoff) Kontrol Listesi - Session Handoff Check"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Bir ajan isi baska ajana devrederken paketin gecerliligi - 4 madde eksikse devre gecersizdir"
---

# Oturum Devre (Handoff) Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [AGENTS protokolu](../AGENTS.md)

> **Tek cumlelik sorumluluk:** *Bu devre (handoff) gecerli mi* sorusunun tek cevabidir.
> Paket 4 madde disindaysa alan rol ise **baslamaz**; devre geri cevrilir ve is kaybinedir.

---

## §0 Ozet (Answer First)

Devre paketi **tam 4 madde** ister: `NE URETILDI` · `KANIT` · `BILINEN EKSIK` · `SONRAKI ADIM`. `KANIT` yazilamiyorsa (build/test/olcum ciktisi yoksa) devre **G5 ihlali** ile reddedilir. GATE onayi gereken islerde (GATE-003 vb.) devre **onaydan once durur**.

---

## §1 Zorunlu 4 Madde

| # | Madde | Icerik | Eksikse |
|---|---|---|---|
| 1 | **NE URETILDI** | dosya yollari (tam) + tek cumle ne oldugu | devre gecersiz |
| 2 | **KANIT** | build / test / link tarama ciktisi (sayilarla) | **G5 RED** |
| 3 | **BILINEN EKSIK** | eksik ya da `NEEDS_VALIDATION` maddeleri; yoksa `YOK` | belirsizlik kalir |
| 4 | **SONRAKI ADIM** | hedef rolun baslayacagi TEK somut ilk adim | is buyur |

---

## §2 Gecerlilik Kontrolu

| # | Soru | Kanit | Gecis |
|---|---|---|---|
| 1 | Paket tam 4 madde mi? | devre metni | [ ] |
| 2 | 1. maddedeki dosya yollari diskte var mi? | `Test-Path` | [ ] |
| 3 | 2. maddedeki kanit bu oturuma ait mi (tarih/sayi)? | komut ciktisi | [ ] |
| 4 | 3. madde `YOK` diyor mu, yoksa eksikler tek tek mi? | metin | [ ] |
| 5 | 4. madde tek ve somut adim mi ("devam et" degil)? | metin | [ ] |

---

## §3 Gate ve Yetki Kontrolu

| # | Durum | Aksiyon |
|---|---|---|
| 1 | Is **GATE-003** onayi bekliyor | devre **durur**; kod yazimi yasak (ADR-025) |
| 2 | Is baskasinin **X alanina** giriyorsa (bkz. AGENTS.md) | rol degisir, devre o role yapilir |
| 3 | **Ayni is 2+ kez** geri geldi (RED dongusu) | 3. denemeden sonra 3-STRIKE escalation |
| 4 | Kanit **tahmini** ("gecirir diye dusunuyorum") | devre gecersiz, G5 |
| 5 | Yikici islem (silme / git reset) var | **onay sonrasi** devre gecerli (G7) |

---

## §4 Red Ornekleri

| Hatali devre | Neden | Dogrusu |
|---|---|---|
| "Kodu yazdim, bakar mısın" | 1. ve 2. madde yok | dosya yolu + komut ciktisi |
| "Build gecti diye dusunuyorum" | kanit tahmini | ciktiyi yapisdir, sayi ver |
| "Birkac eksik var ama onemli degil" | 3. madde belirsiz | maddeleri tek tek ya da `YOK` |
| "Devam et" | 4. madde yok | hedefin ilk somut adimi |
| 4 madde tam, gate acik | G12 / ADR-025 | once WORKFLOW.md gate kontrolu |

---

## §5 Cikti BiCimi

| Alan | Deger |
|---|---|
| Sonuc | `GECERLI` ya da `GERI CEVRILDI` |
| Geri cevrim nedeni | eksik madde no (1..4) ya da G5/G7/G12 |
| Kanit | devre paketinin 4 maddesi |
| Sonraki | alici rolun §2'deki ilk `[ ]` maddesi |

**Ilk adim (<2 dk):** Elindeki devre paketini §1 tablosuna koy; 4 madde de dolu mu, tek tek isaretle.
