---
title: "Layer 5000 - 9 Tier Namespace-Modul Parcacik Indeksi (5834 parcacik)"
type: index
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "layer-5000 dizini giris dosyasi: Tier bazli parcacik sayimlari, esik kaniti ve ADR baglantisi"
---

# Layer 5000 - 9 Tier Parcacik Indeksi (5834)

**Zorunlu Baglantilar:** [mimari index](../index.md) · [catalog](../catalog/index.md) · [layer-rules](../../.rules/layer-rules.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md)

> **Tek cumlelik sorumluluk:** *9 Tier altinda toplam 5834 namespace-modul parcacigi hangi dosyada, esik 5000 gecti mi* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

**TOPLAM 5834 parcacik -> esik 5000 -> PASS**. Satirlar namespace-modul kaydidir, `.csproj` DEGIL; 9+2 proje sabittir (ADR-028/030). Taban 618 giris `mimari-02.md` (K3) dosyasindan dogrudan okunur; genisletme `scripts/gen-layer-5000.ps1` ile deterministik uretilir.

| Tier | Dosya | Katman | Proje | Taban (K3) | Genisletme | Toplam |
|---|---|---|---|---|---|---|
| 0 Foundation | [T0-foundation](T0-foundation.md) | l0 | VersaCoder.Abstractions | 49 | 517 | 566 |
| 1 Domain | [T1-domain](T1-domain.md) | l1 | VersaCoder.Domain | 100 | 1020 | 1120 |
| 2 Application | [T2-application](T2-application.md) | l2 | VersaCoder.Application | 86 | 896 | 982 |
| 3 Infrastructure | [T3-infrastructure](T3-infrastructure.md) | l4 | VersaCoder.Infrastructure | 102 | 1008 | 1110 |
| 4 Protocol | [T4-protocol](T4-protocol.md) | l5 | VersaCoder.Protocol | 30 | 351 | 381 |
| 5 CrossCutting | [T5-crosscutting](T5-crosscutting.md) | l3 | VersaCoder.Crosscutting | 20 | 306 | 326 |
| 6 Host | [T6-host](T6-host.md) | l6 | VersaCoder.Host | 14 | 270 | 284 |
| 7 UI | [T7-ui](T7-ui.md) | l7 | VersaCoder.Desktop + Cli | 1 | 660 | 661 |
| 8 Tests | [T8-tests](T8-tests.md) | test | VersaCoder.Tests* | 28 | 376 | 404 |

| Olcut | Deger |
|---|---|
| Toplam parcacik | 5834 |
| Esik | 5000 (sabit, ADR-030) |
| Marj | 834 |
| Taban K3 | 484 |
| Sonuc | PASS |

## §1 Kurallar

1. Her satir **namespace-modul parcacigidir**; yeni satir = yeni tip/kayit, **yeni `.csproj` degil** (ADR-030, layer-rules LAY-04/05).
2. Makro katman yonu [layer-rules](../../.rules/layer-rules.md) 8x8 matrisine tabidir; bu dosya yon degistirmez.
3. Durum: `planli`; kod yazildikca tip kaydi [catalog](../catalog/index.md) altinda `yazildi`ye doner.
4. Esik 5000 dusurulemez (ADR-030 §2.3); sayim bu dosyanin satir sayisidir.

## §2 Uretim

```powershell
powershell -NoProfile -File .ai\scripts\gen-layer-5000.ps1   # cikti: PASS/FAIL + satir sayisi
```

## §3 Kanit

```text
script   : gen-layer-5000.ps1
kaynak   : versacoder.mimari-02.md (K3, 484 taban giris)
uretim   : 2026-10-02
toplam   : 5834 / esik 5000 -> PASS
```

**Degisiklik gunlugu:** 2026-10-02 - ilk uretim (gen-layer-5000.ps1).
