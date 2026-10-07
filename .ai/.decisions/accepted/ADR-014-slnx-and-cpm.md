---
title: "ADR-014 — Çözüm VersaCoder.slnx + CPM (Directory.Packages.props)"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Paket sürümleri yalnız CPM'de; .csproj'da Version satırı yok"
tags: [adr, build, cpx, slnx, packages]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

`.csproj` başına `Version` satırı olsaydı paket × proje sürüm sapması restore'u
kırardı.

## §2 Karar

- Çözüm dosyası: **`VersaCoder.slnx`**.
- Paket sürümleri **yalnız** `Directory.Packages.props` (CPM) içinde yaşar;
  `.csproj`'larda `Version` satırı **yoktur**.
- İkili çözüm dosyası senaryosu (`.slnx` + `.sln` senkron, diff = 0) layering-5000
  davranışında tanımlıdır (spec §2.5); senkronluk denetimi build kapısında.

## §3 Alternatifler

- **Proje başına Version:** sürüm sapması. ❌
- **CPM (seçilen):** tek sürüm kaynağı. ✅

## §4 Sonuçlar

**Ters-senaryo:** `.csproj` başına `Version` satırı olsaydı paket × proje sürüm
sapması restore'u kırardı.

**Yakalama:** `dependency-update-check` + `.csproj` içinde `Version` = 0 araması.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `VersaCoder.slnx` + `Directory.Packages.props` | `.csproj` içinde `Version` satırı = 0 |

## §6 Kaynak

- brain.md §1 satır 014 · §1.1 · §1.2

## §7 İlgili

- [adr-index](../adr-index.md) · [brain](../brain.md)
