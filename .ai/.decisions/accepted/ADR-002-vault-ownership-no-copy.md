---
title: "ADR-002 — Vault içeriği elle sıfırdan yazılır; kopya yasak"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Referans kaynaklar salt okunur; hiçbir metin blok olarak kopyalanmaz"
tags: [adr, vault, g1, ownership]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Vault, prompt arşivi (G: sürücüsü) ve CoreMusic vault'u gibi dış kaynaklarla
besleniyor. Bu kaynakların içeriği doğrudan yapıştırılırsa başka projenin
kuralı burada geçerli hale gelir; iki vault çelişince hangisinin doğru olduğu
bilinmez.

## §2 Karar

Dış kaynaklar yalnız **bilgi kaynağıdır**; içerik kendi cümlelerimizle sıfırdan
yazılır. `Copy-Item`, blok yapıştırma ve birebir cümle kopyası yasaktır (G1).
Kurallar G-seti `.rules/` altında tek metin olarak yaşar (D20/D41).

## §3 Alternatifler

- **Doğrudan kopya:** hızlı ama mülkiyet kaybı + çelişki riski. ❌
- **Salt okunur referans + sıfırdan yazım (seçilen):** yavaş ama sahiplik net. ✅

## §4 Sonuçlar

**Ters-senaryo:** Kopya içerik başlasaydı başka projenin kuralı burada geçerli
olurdu; iki vault çelişince hangisinin doğru olduğu bilinmezdi.

**Yakalama:** `documentation-check` + cümle-bazlı spot check.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| tüm `.md` gövdeleri | yabancı/kopya blok = 0 |

## §6 Kaynak

- brain.md §1 satır 002 · §1.1 · §1.2
- .ai/CLAUDE.md G1 (Sıfırdan yazım)

## §7 İlgili

- [ADR-024-coremusic-vault-immutable.md](ADR-024-coremusic-vault-immutable.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
