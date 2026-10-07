---
title: "ADR-004 — DevExpress tek sürüm v26.1 + ribbon Office2010"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "DevExpress Universal v26.1 tek sürüm; ribbon stili yalnız Office2010"
tags: [adr, devexpress, ui, version]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

DevExpress sürümü/ribbon stili sapması `.resx` ve `.Form.Designer.cs` dosyalarını
tasarımcıda açılamaz hale getirir; ribbon düzeni kayar.

## §2 Karar

- DevExpress **tek sürüm v26.1** (sürüm ADR'den doğrulanır; "en güncel" iddiası yasak — G9).
- Ribbon stili yalnız **`RibbonControlStyle.Office2010`**.
- Ana pencere `RibbonForm`.

## §3 Alternatifler

- **Birden fazla sürüm:** .resx bozulması. ❌
- **Tek sürüm + tek stil (seçilen):** tutarlı tasarıcı deneyimi. ✅

## §4 Sonuçlar

**Ters-senaryo:** Sürüm/stil sapması olsaydı `.Designer.cs` ve `.resx` tasarımcıda
açılmaz, ribbon düzeni kayardı.

**Yakalama:** `ribbon-style-check`, `devexpress-control-check`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `Directory.Packages.props` | DevExpress ≠ 26.1 satırı = 0; Office2010 dışı stil = 0 |

## §6 Kaynak

- brain.md §1 satır 004 · §1.1 · §1.2
- .ai/CLAUDE.md G3 (DevExpress sürüm + ribbon)

## §7 İlgili

- [adr-index](../adr-index.md) · [brain](../../brain.md)
