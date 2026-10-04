---
title: "ADR Yazim Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "ADR-NNN uretimi icin architect ajana yapistirilan is emri: 6 bolum, en az 2 alternatif, kabul/sonuc tablosu, kanit"
---

# ADR Yazim Promptu

**Zorunlu Baglantilar:** [index](index.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [adr-index](../.decisions/adr-index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md)

> **Tek cumlelik sorumluluk:** *Bu karar neden alındı, hangi alternatifler
> elendi* sorusunun, tek dosyada (ADR) yaşayan tek cevabıdır.

---

## §0 Ozet (Answer First)

ADR **geri alınabilir kararların** kaydıdır; kural `.rules/`'ta, gate
[WORKFLOW](../WORKFLOW.md)'da yaşar — ADR bunlara karar verir, tekrarlamaz.
Yapı: **Bağlam → Karar → Alternatifler → Sonuçlar** (+ bu vaultta `§1` tablo
görünümü). Zorunlu: **en az 2 alternatif**, her biri için `(+)/(−)`, ve
`status: accepted` olmadan kapı sayılmaz. Örnek kalıp: ADR-028 (8 makro
katman / 52 düğüm / 9 proje, 3 turlu oylama kaydıyla).

| # | ADR tipi | Nerede yaşar | Kapı |
|---|---|---|---|
| 1 | Mimari / katman | `.decisions/accepted/` | GATE-004a |
| 2 | Teknoloji / paket | `.decisions/accepted/` | G8 onayı |
| 3 | Veri erişimi | `.decisions/accepted/` | GATE-005 |
| 4 | UI / ekran sözleşmesi | `.decisions/accepted/` | ADR-025 gate'i |
| 5 | Reddedilen alternatif | aynı dosya `§3` içinde | — |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | kalıp: bağlam tablosu, 3 tur, sonuç | tam |
| 2 | [adr-index](../.decisions/adr-index.md) | numara ataması, kabul durumu | tam |
| 3 | [AGENTS](../AGENTS.md) | mimari iş kime, kutu biçimi | §1-§2 |
| 4 | [layer-boundary-check](../checklists/layer-boundary-check.md) | karar 13 sorudan geçer mi | tam |
| 5 | [new-doc-contract](../.templates/new-doc-contract.md) | frontmatter + link kuralları | §1-§3 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** ADR yazari - mimari karar kayit sorumlusu.
> **KONU:** <karar basligi> · ADR-<NNN>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once cevap: `KARAR:
> <tek cumle>` + tek satir gerekce.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - 1 alternatifli ADR yasak: **en az 2 alternatif** zorunlu.
> - ADR disinda "karar:" cumlesi yazmak yasak (G9) - kural `.rules/`'ta.
> - Kanitsiz sayi/iddia yasak (G5); bilinmeyen `NEEDS_VALIDATION`.
> - EF Core / tek bacakli gerekce yasak (G2 / G6).
> - Kutusuz teslim yasak (AGENTS §2).
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `KARAR: <tek cumle>` + gerekce.
> 2. Alternatif tablosu: `# | alternatif | (+) | (−) | sonuc` (>= 2 satir).
> 3. En fazla 5 madde: etkilenen dosyalar + takip eden kapi.
> 4. Kanit: ADR dosya yolu + `adr-index` kaydi + `log.md` satiri.
> 5. 5 alanli kutu.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Bağlamı tabloya yaz (gerçek + kanıt) | §1 tablosu |
| 2 | En az 2 alternatif üret, (+)/(−) yaz | alternatif satırları |
| 3 | Kararı ver ve `status: accepted` ile aç | dosya yolu |
| 4 | Sonuçları `(+)/(−)/risk` olarak yaz | §4 tablosu |
| 5 | `adr-index` + `log.md` kayıtlarını düş | 2 kayıt satırı |
| 6 | 13 soru / takip eden kapı bağlantısı | kapı adı |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : ADR-029 dual-TFM test politikasi acildi |
| KAPSAM : .decisions/accepted/ADR-029-*.md (yeni) |
| KARAR  : testler iki bacakta da kosulur; net48   |
|          baci yalnizca uyumluluk testleri         |
| RİSK   : test suresi 2 kat artabilir - sure       |
|          butcesi GATE-005'te olculmeli             |
| KANIT  : adr-index -> ADR-029 satiri / 3 alternatif|
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Alternatif | ≥ 2 satır, her satırda `(+)/(−)` |
| Bağlam | gerçek + kanıt sütunu dolu |
| Sonuç | `(+)/(−)/risk` ayrı ayrı yazılı |
| Kayıt | `adr-index` + `log.md` satırı var |
| Kutu | 5 alan dolu, `KANIT` yol + sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| Tek alternatif | G5/G9 riski | ikinci alternatif üret |
| Karar `.rules/`'ta da | çifte gerçek (SSUT) | tek kaynak, diğeri link |
| Kanıtsız sayı | G5 | ölç ya da `NEEDS_VALIDATION` |
| `status` eksik | kapı sayılmaz | `accepted` ya da `proposed` |
| Kutusuz teslim | AGENTS §2 | kutuyu iste |

---

## §6 Yasaklar

1. **Tek alternatifli karar yasak** — en az 2.
2. **ADR dışı karar cümlesi yasak** (G9) — kural `.rules/`'ta.
3. **Kanıtsız sayı yasak** (G5) — ölçüm ya da etiket.
4. **Tek bacaklı gerekçe yasak** (G6) — iki TFM.
5. **Kutusuz teslim yasak** — 5 alan zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Bağlam + kanıt toplama | ≈10 dk | kapsamı daralt |
| Alternatif üretimi | ≈15 dk | tek soruya indir |
| Yazım + kayıt | ≈15 dk | dosyayı böl |
| 13 soru + kapı | ≈5 dk | mimariye böl |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [architect-prompt](architect-prompt.md) | karar henüz verilmeden önce |
| [panel-oylama-prompt](panel-oylama-prompt.md) | çok turlu oylama gerekiyorsa |
| [reviewer-prompt](reviewer-prompt.md) | ADR metninin bağımsız incelemesi |
| [gate-acma-prompt](gate-acma-prompt.md) | takip eden kapı kapanışı |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Alternatif | ≥ 2 satır | §2.1 madde 2 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
