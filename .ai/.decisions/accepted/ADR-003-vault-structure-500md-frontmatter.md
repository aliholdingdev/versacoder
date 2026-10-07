---
title: "ADR-003 — Vault ölçek + frontmatter sözleşmesi"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Vault ≥500 MD hedefi; her .md 8 alan frontmatter; bozuk link = 0"
tags: [adr, vault, frontmatter, scale]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Vault, kontrolsüz büyüyünce aranamaz ve doğrulanamaz hale gelir. Frontmatter
düşerse link ve ölçüme kapısı da düşer.

## §2 Karar

- Vault **≥500 MD** hedefini taşır (GATE-004a eşiği).
- Her `.md` dosyası **8 alanlı YAML frontmatter** ile başlar
  (`title/type/category/status/version/updated/authority/scope`).
- **Bozuk link = 0** hedefi; kırık bağlantı korunmaz, onarılır ya da yeniden bağlanır.
- Yeni mimari ile frontmatter **11 alana** genişler: `tags`, `source`, `supersedes`
  eklenir (D23). Eski 8 alanın korunması zorunlu, 3 alan yeni dosyalarda zorunludur.

## §3 Alternatifler

- **Sınır yok:** arama/imleç kaybı. ❌
- **Ölçek + frontmatter sözleşmesi (seçilen):** denetlenebilirlik. ✅

## §4 Sonuçlar

**Ters-senaryo:** Dosyalar özet seviyesinde (1–16 KB) kalsaydı vault aranamaz;
frontmatter düşerse link ve ölçüme kapısı da düşerdi.

**Yakalama:** `vault-link-check` + bozuk link = 0 komutu.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| kök `.md` frontmatter + boyut bandı | NoFrontmatter = 0 |

> Boyut bandı notu: 2026-10-07 kararı D48 — "içerik doğrusu", sayısal boyut
> sınırı yoktur; GATE-004a'nın 10-16 KB kriteri revize edilmiştir.

## §6 Kaynak

- brain.md §1 satır 003 · §1.1 · §1.2
- D23 (frontmatter 11 alan) · D48 (boyut sınırı yok)

## §7 İlgili

- [adr-index](../adr-index.md) · [brain](../../brain.md) · [checklists/vault-link-check](../../checklists/vault-link-check.md)
