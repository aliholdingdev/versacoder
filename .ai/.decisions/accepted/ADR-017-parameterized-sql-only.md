---
title: "ADR-017 — Parametreli SQL tek doğrudur"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "@p1..@pN zorunlu; string-concat SQL = otomatik RED; CommandTimeout açık"
tags: [adr, data, security, sql, g2]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Concat SQL kalsaydı enjeksiyon doğrudan veritabanına açılırdı; kod incelemesi
bunu yakalayamayabilir.

## §2 Karar

- Tüm SQL **parametreli**: `@p1..@pN`.
- String-concat SQL (`"SELECT ..." + değişken`) = kod incelemesinde **otomatik RED** (G2).
- Her komutta `CommandTimeout` açık set edilir.

## §3 Alternatifler

- **Concat SQL:** enjeksiyon. ❌
- **Parametreli (seçilen):** güvenli + plan cache. ✅
- **ORM:** ADR-005 ile yasak. ❌

## §4 Sonuçlar

**Ters-senaryo:** Concat SQL kalsaydı enjeksiyon doğrudan veritabanına açılırdı.

**Yakalama:** `sql-injection-review` + `"SELECT ..." + değişken` taraması.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| tüm SQL çağıran nokta | concat deseni = 0; `@p` kullanılmayan sorgu = 0 |

## §6 Kaynak

- brain.md §1 satır 017 · §1.1 · §1.2
- .ai/CLAUDE.md G2 · §1.1 KOTU/İYİ örneği

## §7 İlgili

- [ADR-005-ado-net-dapper-no-ef-core.md](adr-005-ado-net-dapper-no-ef-core.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
