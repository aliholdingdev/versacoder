---
title: "ADR-016 — DbConnection yalnız l4 ado-dapper'da yaşar"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-005 geçerli; ek sınır: başka katman bağlantı tutamaz"
tags: [adr, data, layers, connection, scope]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Bağlantı üst katmanda tutulsaydı katman testi izole edilemez, transaction
bütünlüğü dağılırdı.

## §2 Karar

ADR-005 (ADO.NET + Dapper) geçerlidir; ek sınır: **`DbConnection` yalnız
`l4 ado-dapper` katmanında** bulunur. Başka katman (l2, l5, l6, l7) bağlantı
**tutamaz**; yalnız `l0` portuna (`IRowWriter`/`IDbExecutor` gibi) bakar.

## §3 Alternatifler

- **Bağlantı serbest:** test izolasyonu kaybı, transaction dağınıklığı. ❌
- **Tek sahibi l4 (seçilen):** port üzerinden DIP. ✅

## §4 Sonuçlar

**Ters-senaryo:** Bağlantı üst katmanda tutulsaydı katman testi izole edilemez,
transaction bütünlüğü dağılırdı.

**Yakalama:** `data-access-check` + NetArchTest "bağlantı yalnız l4" kuralı.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| Infrastructure `ado-dapper` | `DbConnection` yalnız l4; NetArchTest RED = 0 |

## §6 Kaynak

- brain.md §1 satır 016 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005-ado-net-dapper-no-ef-core.md]]
- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028-layered-baseline-9-projects.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
