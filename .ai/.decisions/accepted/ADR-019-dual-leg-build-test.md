---
title: "ADR-019 — Her değişiklik iki bacakta da build + test"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Tek bacak yeşil = bitmemiştir; TDD %90+ hedefi"
tags: [adr, test, quality, dual-leg, tdd]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Tek bacakta yeşil "bitti" sayılsaydı kullanıcı makinesinde kırılır, kanıt sahte
olurdu.

## §2 Karar

- Her değişiklik **iki bacakta da** (`net48` + `net10.0-windows`) build + test
  gerektirir; tek bacak yeşil = **bitmemiştir** (G5).
- TDD: test önce, implementasyon sonra; **%90+ coverage hedefi**
  (`gate-005-test-kapsami`, `coverage-gate-check`).
- Sonuç nesnesi `Result<T>` ile taşınır (brain §3 kural 1); exception yalnız
  beklenmeyen çökme içindir.

## §3 Alternatifler

- **Tek bacak yeterli:** kanıt eksik. ❌
- **İki bacak (seçilen):** LSP kanıtı. ✅

## §4 Sonuçlar

**Ters-senaryo:** Tek bacakta yeşil "bitti" sayılsaydı kullanıcı makinesinde
kırılır, kanıt sahte olurdu.

**Yakalama:** `dual-tfm-build-check`, `gate-005-test-kapsami`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `VersaCoder.Tests` koşuları | iki bacakta `dotnet test` = 0 başarısız |

## §6 Kaynak

- brain.md §1 satır 019 · §1.1 · §1.2 · §3 kural 3

## §7 İlgili

- [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001-dual-target-net48-net10.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
