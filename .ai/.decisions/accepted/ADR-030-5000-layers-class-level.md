---
title: "ADR-030 — 5000+ katman = sınıf/namespace düzeyi kayıt; proje sabit 9+2"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her satır namespace-modül parçacığıdır, .csproj DEĞİL; ADR-028/030: 9+2 proje sabit"
tags: [adr, architecture, 5000, catalog, nodes]
source: "spec/layering-5000-spec.md v2.1.0 + brain.md bağlamı — 2026-10-07 yeniden derleme"
supersedes: ""
---

## §1 Bağlam

"5000+ katman" ifadesi belirsiz kalsaydı ya proje sayısına (5000 `.csproj`) ya
da kayda yorumlanabilirdi; ilki restore/IDE çökertir (ADR-028 §3 A3).

## §2 Karar

**5000+ katman = sınıf/namespace düzeyi kayıt.** Üç eksen ayrı ölçülür:

| Eksen | Bugün (2026-10-02) | Hedef |
|---|---|---|
| Tip (katalog satırı) | 5440 | ≥5000 — ASILDI |
| Düğüm (namespace/folder) | 44 alt + 8 makro = 52 | ≥5000 — EKSİK |
| Yön grubu = üretim `.dll` | 9 (l0..l8) | ≥5000 — EKSİK |

- **ARA KADEME YOKTUR** (100/500/1000 silindi — kullanıcı kararı).
- Proje sayısı **sabit 9+2** (ADR-028); 5000 `.csproj` hedefi DEĞİLDİR.
- Sayım `scripts/count-components.ps1` + `gen-layer-5000.ps1` ile yapılır (ADR-027).
- Tek "katman" kelimesi tek bir sayaçla kanıtlanamaz (G5); iki sayaç ayrı raporlanır.

## §3 Alternatifler

- **5000 .csproj:** restore çöker. ❌
- **Ara kademeler (100/500/1000):** hedef bulanıklaşırdı — silindi. ❌
- **Sınıf/namespace düzeyi + sabit 9+2 proje (seçilen):** ölçeklenebilir. ✅

## §4 Sonuçlar

**Ters-senaryo:** Belirsizlik kalsaydı proje sayısına yorumlanır, IDE çökerdi.

**Yakalama:** `count-components.ps1` + `layer-5000-check.md`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `architecture/layer-5000/` + katalog | ≥5000 kayıt (tip ekseni), düğüm ekseni ayrı rapor |

## §6 Kaynak

- vault spec/layering-5000-spec.md (v2.1.0, 2026-10-02) — ADR-030'un kaynağı
- brain.md §1 satır 027/028 bağlamı + scripts/gen-layer-5000.ps1 başlık notu
- YENİ İNŞA notu: orijinal ADR-030 metni diskte yoktu; karar spec dosyasından
  + brain özetlerinden yeniden derlendi (G9)

## §7 İlgili

- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028-layered-baseline-9-projects.md]]
- [[.decisions/accepted/adr-013-layered-architecture-52-nodes|ADR-013-layered-architecture-52-nodes.md]]
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
