---
title: "ADR-024 — CoreMusic vault dokunulmaz"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "C:\www\coremusic.net\.ai — yalnız yapı referansı, içerik kopyalanmaz"
tags: [adr, reference, readonly, protection]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Başka vault'a yazılsaydı ADR-002 ihlali ve başkasının verisinin bozulması birlikte
oluşurdu.

## §2 Karar

`C:\www\coremusic.net\.ai` (CoreMusic vault) **dokunulmaz**. Yalnız yapı
referansı alınır; içerik kopyalanmaz (G1/ADR-002).

## §3 Alternatifler

- **Yazma / kopya:** mülkiyet ihlali. ❌
- **Yalnız yapı referansı (seçilen):** koruma. ✅

## §4 Sonuçlar

**Ters-senaryo:** Başka vault'a yazılsaydı ADR-002 ihlali ve başkasının verisinin
bozulması birlikte oluşurdu.

**Yakalama:** `documentation-check` + kopya blok taraması.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `C:\www\coremusic.net\.ai` | o vault'a yazma işlemi = 0 |

## §6 Kaynak

- brain.md §1 satır 024 · §1.1 · §1.2

## §7 İlgili

- [ADR-002-vault-ownership-no-copy.md](adr-002-vault-ownership-no-copy.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
