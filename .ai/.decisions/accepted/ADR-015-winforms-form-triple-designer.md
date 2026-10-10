---
title: "ADR-015 — Her ekran = .Form.cs + .Form.Designer.cs + .resx üçlüsü"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Designer/resx yalnız tasarımcıdan; son söz ui-designer (GATE-003)"
tags: [adr, ui, winforms, designer, gate-003]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Designer elle düzenlenirse kaynak ürün bozulur, ekran tasarımcıda açılmaz ve
geri alınamaz.

## §2 Karar

Her ekran **`.Form.cs + .Form.Designer.cs + .resx` üçlüsü** olarak yaşar.
Designer/resx **yalnız tasarımcıdan** üretilir; el ile düzenleme yasak.
Son söz: `ui-designer` rolü + **GATE-003 onayı** (7 ekran wireframe onayı olmadan
`.Form.cs` yazılmaz — ADR-025).

## §3 Alternatifler

- **Elle Designer düzenleme:** bozulmuş .resx. ❌
- **Üçlü + tasarımcı + onay (seçilen):** geri alınabilir tasarım. ✅

## §4 Sonuçlar

**Ters-senaryo:** Designer elle yazılsaydı kaynak ürün bozulur, ekran tasarımcıda
açılmaz ve geri alınamazdı.

**Yakalama:** `winforms-designer-check`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| 7 ekranın `.Form.cs/.Designer.cs/.resx` üçlüsü | her üçlü dosya var, tasarımcıda açılıyor |

## §6 Kaynak

- brain.md §1 satır 015 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-025-gate003-ui-hard-stop|ADR-025-gate003-ui-hard-stop.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
