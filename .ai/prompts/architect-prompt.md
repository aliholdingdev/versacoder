---
title: "Mimari Analiz Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Katman/bagimlilik analizi ve 'bagi acilsin mi' onayi icin architect ajana yapistirilan is emri: 13 soru, 8x8 yon matrisi, karar tablosu"
---

# Mimari Analiz Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [architecture/index](../architecture/index.md)

> **Tek cumlelik sorumluluk:** *Bu bağ / dosya / proje mimariye uygun mu*
> sorusunu 13 soru ve yön matrisiyle cevaplayan analiz iş emridir.

---

## §0 Ozet (Answer First)

Baz karar ADR-028'dir: **8 makro katman (l0..l7) + 44 alt katman = 52 düğüm,
9 üretim + 1-2 test projesi.** İzinli yön `l7 -> l6 -> l5 -> l4 -> l2 -> l1 ->
l0`; `l3` yalnız `l0`'a bakar; `l4 -> l2` ve `l5 -> l4` **doğrudan ok YOK**.
Bu prompt bu matrisi uygular ve **kararı önce** verir.

| # | Analiz sorusu | Karar |
|---|---|---|
| 1 | `l4 -> l2` bağı açılsın mı? | **X** — port `l0`'da, uygulama `l4`'te |
| 2 | Yeni `.csproj` gerekli mi? | yalnız 3 kapı (sorumluluk / yön / TFM) |
| 3 | Yeni tip nereye yazılır? | N kodu -> klasör eşlemesi (ADR-028 §2.3) |
| 4 | TFM seti nedir? | iki bacak `net48` + `net10.0-windows` |
| 5 | Bağımlılık döngüsü var mı? | varsa RED, circular test 0 ihlal |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | katman/proje/yön kararları | §2 |
| 2 | [layer-boundary-check](../checklists/layer-boundary-check.md) | 13 soru, cevaplanmadan karar yok | tam |
| 3 | [architecture/index](../architecture/index.md) | katman indeksi ve katalog | tam |
| 4 | [AGENTS](../AGENTS.md) | mimari iş kime (§1), kutu (§2) | §1, §2 |
| 5 | [WORKFLOW](../WORKFLOW.md) | blok sırası A / B / C | §3 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Senior Software Architect - net48 + net10.0-windows, DevExpress
> Universal v26.1, ADO.NET + Dapper.
> **KONU:** <bag ya da dosya> - <l?.? -> l?.?>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once cevap: `KARAR:
> IZINLI | YASAK | NEEDS_VALIDATION` + tek satir gerekce.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - `l4 -> l2` ve `l5 -> l4` dogrudan ok acmak yasak (ADR-028 §2.1).
> - Tek bacakli gerekce yasak; her karar `net48` ve `net10.0-windows`
>   icin ayri gerekcelenir.
> - EF Core / yeni ORM onerisi yasak (G2, G8).
> - 13 soru cevaplanmis gibi karar vermek yasak.
> - Kanitsiz "uygun" yasak (G5); bilinmeyen `NEEDS_VALIDATION`.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `KARAR: ...` + tek satir gerekce.
> 2. Karar tablosu: `# | soru | cevap | kanit` (13 satir).
> 3. En fazla 5 madde: alternatif(ler), (+)/(−), etkilenen dosyalar.
> 4. Somut sure: `≈20 dk`.
> 5. 5 alanli kutu (AGENTS §2).

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | ADR-028 §2.1 yön satırını oku | ok yönü |
| 2 | 13 soruyu tek tek cevapla | 13/13 cevap |
| 3 | ≥ 2 alternatif üret, (+)/(−) yaz | alternatif tablosu |
| 4 | Kararı ver; gerekiyorsa ADR taslağı aç | dosya yolu |
| 5 | 8x8 matrisle NetArchTest kuralını eşle | matris satırı |
| 6 | Devir paketi (4 madde) hazırla | 4/4 madde |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : l4 -> l2 bağı RED - port l0'a tasindi    |
| KAPSAM : src/.../Application/..., layer-boundary  |
|          -check.md                               |
| KARAR  : YASAK - repository portu l0'da, uygulama |
|          l4'te; dogrudan ok ADR-028 §2.1 ile X    |
| RİSK   : l4'un tek projede sismesi - NetArchTest  |
|          matrisi ile denetlenecek                 |
| KANIT  : 13 soru 13/13 cevapli / matris -> 0 ihlal |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Karar | `IZINLI / YASAK / NEEDS_VALIDATION` ilk satırda |
| 13 soru | 13/13 cevaplanmış, çelişki yok |
| Alternatif | en az 2, her biri için (+)/(−) |
| Yön | matris ADR-028 §2.1 ile aynı kaynaktan |
| Kutu | 5 alan dolu, `KANIT` sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| 13 soru eksik | kanıtsız mimari karar (AGENTS §2.1) | soruları tamamla |
| Tek bacak gerekçe | G6 | iki bacak için ayrı yaz |
| Matris elle çizilmiş | NetArchTest'ten kopuk | aynı kaynağı bağla |
| Alternatifsiz karar | G5/G9 | ≥ 2 alternatif |
| Yeni `.csproj` gerekçesiz | ADR-028 3 kapısı | 3 kapıdan geçir |

---

## §6 Yasaklar

1. **Yasak okları açmak yasak** — `l4 -> l2`, `l5 -> l4`, `l2 <-> l4`.
2. **13 sorusuz mimari onay yasak** — cevap yoksa karar yok.
3. **Tek bacaklı gerekçe yasak** — iki TFM birlikte gerekçelenir.
4. **Yeni paket/ORM önerisi yasak** — kullanıcı onayı (G8).
5. **Kanıtsız "uygun" yasak** — `NEEDS_VALIDATION` dürüsttür.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Kaynak okuma + yön | ≈5 dk | işi durdur |
| 13 soru | ≈10 dk | analizi böl |
| Alternatif + karar | ≈10 dk | tek soruya indir |
| Kutu + devir | ≈5 dk | açıklamayı kısalt |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [architect-prompt](architect-prompt.md) | yeni ADR kararı gerekiyorsa |
| [adr-writer-prompt](adr-writer-prompt.md) | karar ADR'ye dönüştürülürken |
| [reviewer-prompt](reviewer-prompt.md) | ihlal şüphesi incelemeye giderse |
| [developer-prompt](developer-prompt.md) | karardan sonra kod üretimine geçiş |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| 13 soru | 13/13 satır | §2.1 madde 2 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
