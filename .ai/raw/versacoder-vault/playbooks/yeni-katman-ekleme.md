---
title: "yeni-katman-ekleme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "l0..l7 mimarisine yeni katman veya dugum ekleme adimlari - ADR, katalog uretimi, build kaniti ve geri alma"
---

# yeni-katman-ekleme — Katman/Dugum Ekleme

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [ADR acma](adr-acma.md)

> **Tek cumlelik sorumluluk:** *Mimariye yeni katman veya dugum hangi sirayla eklenir, kaniti nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Yeni katman/dugum yalniz `architect` acar; **kod bu isin parcasi degildir** (GATE-003 acikken `.cs` yazmak G12 ihlalidir). Sira: karar -> ADR -> `architecture/l*/` node dosyasi -> `build-catalog.ps1` -> `count-components.ps1` -> `check-links.ps1`. Toplam ≈25 dk. Kanit yoksa is bitmis sayilmaz (G5): katalog uretilmeden "tamam" denmez.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Katman baglilik yonu biliniyor (ice bagimlilik yalniz `interface`) | ADR-013 + `.ai/architecture/index.md` |
| 2 | Mevcut dugum listesi okunmus (N01..N52 dolu mu bos mu) | `pwsh -File .ai\scripts\count-components.ps1` |
| 3 | Karar en az 2 alternatifli yazilmis | `.ai/.decisions/accepted/ADR-0NN-*.md` |
| 4 | Kod yazilacaksa GATE-003 kapali | [WORKFLOW.md](../WORKFLOW.md) §3 |
| 5 | `.rules/` karsiligi bu karari zorunlu kiliyor | `.ai/.rules/coding-standards.md` |

---

## §2 Adimlar

**Toplam ≈25 dk** · yapan: `architect`

1. **Karari ver + ADR ac** — 2 alternatif, (+)/(−) sonuc ve `hedef_bacak` alanini doldur. ADR adimlari [adr-acma](adr-acma.md)'da.
   - Dosya: `.ai/.decisions/accepted/ADR-0NN-<konu>.md`
   - Cikti: `status: accepted` satiri + `log.md` kaydi

2. **Node/katman dosyasini yaz** — `architecture/l<N>/` altinda node detay dosyasi (arayuz, akis, test plani, TFM).
   - Ornek: `.ai\architecture\l2\N17-<node>.md`
   - Cikti: frontmatter'li, en az 1 tablo, 2 bacak (`net48` + `net10.0-windows`) yazilmis

3. **Katalogu yeniden uret** — tohum kartlari ve node ozetlerini script basar; elle tablo yazma.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\build-catalog.ps1 -Force`
   - Cikti: `architecture/catalog/` icinde node ozet + kart dosyalari (UTF-8, BOM'suz)

4. **Sayiyi ve linki dogrula** — kayit sayisi eski degilse ya da bozuk link cikarsa adim 2'ye don.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\count-components.ps1`
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `PASS` + `BOZUK link : 0`

5. **Defteri ve `.rules/`'u guncelle** — kardes dosya satiri + kural maddesi; sonra devre paketi.
   - Dosya: `.ai\index.md` veya ilgili `.rules/*.md` + `.ai\log.md`
   - Cikti: 4 maddelik devre paketi (`developer`'a)

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Katalog sayaci | `count-components.ps1` -> `PASS` (>= 5440 kayit) |
| K2 | Link taramasi | `check-links.ps1` -> `BOZUK link : 0` |
| K3 | ADR dosyasi | `status: accepted` + `hedef_bacak: "net48 + net10.0-windows"` |
| K4 | Rollback denemesi | §4 adim 1-3 calisiyor, eski dosyalar geri gelmis |

**Ornek cikti (K1):**

```text
=== KATALOG KAYIT SAYISI ===
Node dosyasi : 52   Kayit : 5440   Min : 5000
PASS
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni node dosyasi | `.ai\architecture\l<N>\<node>.md` dosyasini sil |
| 2 | Katalog uretimi | `pwsh -File C:\versacoder\.ai\scripts\build-catalog.ps1 -Force` (eski tohumlarla yeniden bas) |
| 3 | ADR | `status: accepted` -> `status: superseded` + yerine gerekce yaz |
| 4 | `log.md` + index satiri | satiri `REVERT <tarih>` olarak isaretle, silme |
| 5 | Kod varsa (GATE-003 sonrasi) | `git revert <sha>` ya da `.cs` dosyalarini sil + `dotnet build` |

