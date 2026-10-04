---
title: ".decisions — Klasör Girişi (ADR defterine işaretçi)"
type: index
category: decisions
status: active
version: 2.0.0
updated: 2026-10-04
authority: SSOT
scope: "Klasör girişi: ADR durumunun tek gerçeği adr-index.md'dedir; bu dosya yalnız yönlendirir"
---

# `.decisions/` — Klasör Girişi

**Zorunlu bağlantılar:** [[../index.md]] · [[../CLAUDE.md]] · [[../WORKFLOW.md]] · [adr-index.md](adr-index.md) · [[../reports/00-kapsam-analizi.md]]

> **Tek cümlelik sorumluluk:** ADR dosyalarının nerede durduğunu ve **durumlarının nerede okunacağını** söyler; numara/durum/tarih sormak için bu dosyaya değil, deftere gidilir.

---

## §0 Özet (Answer First)

- **Kayıt defteri (SSOT): [adr-index.md](adr-index.md)** — 29 karar (ADR-001..029): 28 `accepted` + 1 `superseded` (ADR-020 → [ADR-029](accepted/ADR-029-ascii-box-drawing-only.md)).
- **Bu dosya ikinci defter değildir** (ADR-021 SRP; "iki defter = iki gerçek" — `log.md` satır 9 uyarısı burada kapanır).
- Eski sürüm (v1.0.0) 7 ADR + 6 taslak listeliyordu; ADR-006..029 diskte `accepted/` altındadır ve sayımlar defterde tutulur.

---

## §1 Klasör yapısı

```text
.ai/.decisions/
|-- adr-index.md   # ADR durumunun TEK SSOT'u (tablo: no · slug · status · tarih · supersedes)
|-- accepted/      # ADR-001 .. ADR-029 (29 dosya; ADR-020 status: superseded)
|-- draft/         # planli -- ilk dosyada olusturulur
|-- rejected/      # planli -- gerekce zorunlu
```

## §2 Ne zaman bu dosya, ne zaman defter

| Soru | Kaynak |
|---|---|
| Hangi ADR hangi durumda? Supersede zinciri? | [adr-index.md](adr-index.md) §3.4 |
| Yeni ADR nasıl açılır / numara nasıl bulunur? | [adr-index.md](adr-index.md) §3.7 |
| Karar neden verildi (gerekçe + alternatif)? | İlgili `ADR-NNN-*.md` gövdesi |
| Klasörde hangi dosyalar var? | Bu dosya §1 |

---

## §3 Doğrulama

- [x] Frontmatter 8 alan, `authority: SSOT` (2026-10-01)
- [x] Bu dosyada numara/durum tablosu **yok** — tek sayı kaynağı adr-index.md §3.4
- [x] `scope` ifadesi "tek kayıt defteri" iddiasından vazgeçildi (adr-index.md = defter)
