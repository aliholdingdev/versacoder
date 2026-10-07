---
title: "ADR-018 — Secret'lar DPAPI ile şifreli yerel depoda; keys.md yalnız harita"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "keys.md değer içermez; repoda/koda anahtar = RED"
tags: [adr, security, secrets, dpapi]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Anahtar koda/repoya girerse depo kamuya açılır; DPAPI olmasa yerel dosya düz
metin okunur.

## §2 Karar

- Secret'lar **DPAPI ile şifreli yerel depoda** tutulur.
- `keys.md` **yalnız harita**dır (anahtar türü → nerede saklandığı referansı);
  **değer içermez** (D17: vault salt referans, secret yok).
- Repoda/koda API anahtarı = otomatik RED.

## §3 Alternatifler

- **.env / koda gömme:** depo sızması = tam erişim kaybı. ❌
- **Düz metin yerel dosya:** okunabilirlik riski. ❌
- **DPAPI + harita (seçilen):** gizlilik + denetlenebilirlik. ✅

## §4 Sonuçlar

**Ters-senaryo:** Anahtar koda/repoya girerse depo kamuya açılır; DPAPI olmasa
yerel dosya düz metin okunurdu.

**Yakalama:** `secret-leak-check`, `security-review-checklist`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| secret store + `keys.md` | `keys.md` içinde değer = 0; repoda anahtar = 0 |

## §6 Kaynak

- brain.md §1 satır 018 · §1.1 · §1.2
- .ai/CLAUDE.md G-ek (secret kodda yok) · D17 kararı

## §7 İlgili

- [ADR-011-llm-provider-order.md](ADR-011-llm-provider-order.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
