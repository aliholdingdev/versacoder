---
title: "Plans - 5000+ Katmanli Mimari Plan (Index)"
type: plan
category: plans
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "5000+ katmanli (tip kaydli) mimari planin dizini: birim/yer/kaynak/kapi kararlari, faz semasi ve olcum kurallari; Faz 02 cevaplari burada ilan edilir"
---

# Plans — 5000+ Katmanli Mimari Plan (Index)

**Zorunlu Baglantilar:** [TODOS index](../TODOS/index.md) · [Faz 02](../TODOS/02-5000-plani-sorulari.md) · [Faz 03](../TODOS/03-faz-d-iskelet.md) · [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layering-5000-spec](../spec/layering-5000-spec.md) · [WORKFLOW](../WORKFLOW.md)

> **Tek cumlelik sorumluluk:** 5000+ katman planinin *birim, yer, kaynak, kapì*
> kararlarinin tek kaynagi ve plan dosyalarinin dizin gobegidir.

**Durum: AKTIF** (Faz 02'deki 4 soru 2026-10-02'de cevaplandi; iskelet asagida).

---

## §0 Ozet (Answer First)

| # | Karar | Secim | Kaynak |
|---|---|---|---|
| 1 | Plan birimi | **her madde = ayri dosya** (5000 dosya) | Faz 02 §4 satir 1 (2026-10-02) |
| 2 | Plan yeri | **`.ai/plans/`** (bu dizin) | Faz 02 §4 satir 2 (2026-09-30) |
| 3 | Icerik kaynagi | **vault SSOT + port + web** (WEB GATE ile dis dogrulama) | Faz 02 §4 satir 3 |
| 4 | Derinlik + kapi | **%20'lik dilimler ara kapi** | Faz 02 §4 satir 4 |

---

## §1 Dizin semasi

```text
.ai/plans/
    index.md              <- bu dosya (kararlar + olcum)
    faz-01/<ID>.md        <- 1. %20 dilim (0..999)
    faz-02/<ID>.md        <- 2. %20 dilim (1000..1999)
    faz-03/<ID>.md        <- 3. %20 dilim (2000..2999)
    faz-04/<ID>.md        <- 4. %20 dilim (3000..3999)
    faz-05/<ID>.md        <- 5. %20 dilim (4000..4999+)
```

- **Dosya adi** = katalog ID'si ile ayni (`N01-001.md`); boyle katalog <-> plan
  diff'i otomatik olur (spec VC-LAY-009 ruhu).
- **5 faz x %20 = ara kapi** (Faz 02 soru 4 cevabi B): her faz sonunda
  `count-components.ps1` PASS + `check-links.ps1` 0 bozuk link **zorunlu**.
- Kaynak web ise maddede `kaynak:` satiri URL + tarih tasiir (G9; WEB GATE).

## §2 Olcum

| # | Arac | Beklenen | Kapi |
|---|---|---|---|
| 1 | `Get-ChildItem .ai/plans -Recurse -Filter *.md` | madde dosyasi sayisi (hedef 5000+) | her faz sonu |
| 2 | `check-links.ps1` | 0 bozuk link | her faz sonu |
| 3 | `count-components.ps1` | PASS (esik 5000) | GATE-004a |

**Degisiklik gunlugu:** 2026-10-02 - ilk yazim (Faz 02 cevaplari ile birlikte).
