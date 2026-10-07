---
title: "ADR-025 — GATE-003: wireframe onayı HARD STOP"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "7 ekran onaylanmadan .Form.cs/.Designer.cs/.resx yazılmaz"
tags: [adr, gate, ui, hard-stop, gate-003]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Onaysız Form kodu yazılsaydı tasarım değişince kod iki kez yazılır, onay
zinciri koparıdı.

## §2 Karar

7 ekranın wireframe'i **GATE-003'te onaylanmadan** hiçbir
`.Form.cs/.Designer.cs/.resx` yazılmaz. Kapı **HARD STOP**tur; ajan kapıyı
hazırlar, asla kapatmaz (yalnız kullanıcı kapatır).

## §3 Alternatifler

- **Onaysız kod:** iki kez iş + kopuk onay zinciri. ❌
- **Hard stop (seçilen):** tasarım önce kod sonra. ✅

## §4 Sonuçlar

**Ters-senaryo:** Onaysız Form kodu yazılsaydı tasarım değişince kod iki kez
yazılır, onay zinciri koparıdı.

**Yakalama:** `gate-003-ui-onay`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| GATE-003 onay çıktısı | 7 ekran wireframe onayı = 7 |

## §6 Kaynak

- brain.md §1 satır 025 · §1.1 · §1.2

## §7 İlgili

- [ADR-015-winforms-form-triple-designer.md](adr-015-winforms-form-triple-designer.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
