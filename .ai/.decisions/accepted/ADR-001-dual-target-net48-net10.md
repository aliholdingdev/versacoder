---
title: "ADR-001 — Çift TFM: net48 + net10.0-windows"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Tüm projeler iki hedefte derlenir; #if yalnız platform farkına izin verilir"
tags: [adr, build, tfm, lsp]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

VersaCoder masaüstü bir IDE platformudur; hedef kitle hem eski (.NET Framework 4.8)
hem modern (.NET 10 Windows) ortamda çalışan kullanıcılardır. Tek TFM seçimi bir
bacakta gerçek kullanıcı deneyimini hiç denenmez kılar.

## §2 Karar

Tüm `.csproj` dosyaları `net48` **ve** `net10.0-windows` çift hedefiyle derlenir.
`#if` (conditional compilation) yalnız platform/API farkı gerektiğinde kullanılır;
davranış çoğaltması için değil. `nullable` her projede açıktır (`<Nullable>enable</Nullable>`,
yan kural — ADR-028 §3 bağlantılı).

## §3 Alternatifler

- **Tek TFM (net48):** modern API'ler kullanılamaz, ürün eski kalır. (INFERRED)
- **Tek TFM (net10):** eski kurumsal ortamlarda çalışamaz, dağıtım daralır. (INFERRED)
- **Çift TFM (seçilen):** iki bacak test yükü var (ADR-019 ile karşılanır). ✅

## §4 Sonuçlar

**Ters-senaryo:** Tek TFM seçilseydi öbür bacak hiç gerçek kullanıcıda denenmezdi;
legacy ekran ya modern API tarafı feda edilirdi.

**Yakalama:** `dual-tfm-build-check` + "tek TFM'de yeşil test" satırı (brain §4.1).

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| her `.csproj` TargetFramework satırı | `dotnet build` × 2 = 0 hata |

## §6 Kaynak

- brain.md §1 satır 001 · §1.1 · §1.2
- .ai/CLAUDE.md §5.3 (katman × proje tablosu, çift TFM sütunu)

## §7 İlgili

- [[.decisions/accepted/adr-019-dual-leg-build-test|ADR-019-dual-leg-build-test.md]] — kanıt kuralı
- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028-layered-baseline-9-projects.md]] — proje baz
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
