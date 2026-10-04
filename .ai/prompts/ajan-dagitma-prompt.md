---
title: "Ajan Dagitma Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Bir isin AGENTS §1 matrisine gore hangi ajana gidecegini, kac ajanin paralel calisacagini ve devir paketinin nasil yazilacagini belirleyen is emri"
---

# Ajan Dagitma Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [ROLE](../ROLE.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu iş kime gider* sorusunun, matriste ve
> eşzamanlılık sınırında kalınarak verilen tek cevabıdır.

---

## §0 Ozet (Answer First)

Dağıtım kuralı basittir: iş [AGENTS](../AGENTS.md) §1 matrisindeyse atanmış
ajana, yoksa `general`'a gider. Aynı anda **en fazla 3** ajan çalışır; aynı
dosyaya iki yazıcı verilmez. Bu prompt dağıtımdan sonra **devir paketini**
(4 madde) ve **kutu beklentisini** de zorunlu kılar.

| # | İş tipi | Ajan | Kanıt |
|---|---|---|---|
| 1 | Mimari / katman kararı | `architect` | ADR taslağı + 13 soru |
| 2 | Kod yazımı / refactor | `developer` | diff + build + test |
| 3 | İnceleme / review | `reviewer` | suç dosya + satır no |
| 4 | Test / kapsam | `tester` | `dotnet test` çıktısı |
| 5 | Güvenlik | `security-auditor` | OWASP kalemi + severity |
| 6 | UI / ekran | `ui-designer` | ekran adı (`01..07`) |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [AGENTS](../AGENTS.md) | §1 matris, §1.1 profiller, §5 eşzamanlılık | §1, §5 |
| 2 | [WORKFLOW](../WORKFLOW.md) | blok sırası A seri / B paralel / C | §3 |
| 3 | [ROLE](../ROLE.md) | devir zinciri 3 hamle, tek soru kuralı | §3, §5 |
| 4 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | katman yönü: hangi iş hangi katmanda | §2.1 |
| 5 | [layer-boundary-check](../checklists/layer-boundary-check.md) | mimari etkili çıktı için 13 soru | tam |

> **Sınır testi (ilk EVET yeter):** "İş `l0..l7` katmanına, `.Designer.cs`'e,
> OWASP kalemine ya da `dotnet test` çıktısına dokunuyor mu?" → EVET ise
> `general` **değildir**; matristeki uzmana gider.

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Gorev dagitici - AGENTS matrisi uygulayicisi.
> **KONU:** <is-tanimi> · <dosya listesi>
>
> **GOREV:** Asagidaki §2.1'deki 6 adimi sirayla uygula, `(n/6)` yaz.
> Dagitimi **once** cevapla, gerekceyi sonra ver.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Matriste karsiligi olmayan isi uzmana vermek yasak; `general` kullan.
> - Ayni anda 3'ten fazla ajan cagirmak yasak (rate limit kaydi).
> - Ayni dosyaya iki yazici vermek yasak; once seriley al.
> - Devir paketini 4 maddeden kisa yazmak yasak (AGENTS §3).
> - Kutusuz ajan ciktisini islemek yasak (AGENTS §2).
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `AJAN: <ad> | PARALELLIK: n/3` + tek satir gerekce.
> 2. Sonra en fazla 5 madde: riskler (gec/erken RED), dosya sahipligi,
>    siralanmasi gereken bagimliliklar.
> 3. Devir paketi: `KALAN IS / DOSYA / KANIT / SINIR` - 4 madde eksiksiz.
> 4. Kanit: matris satiri + ajan kutusu beklentisi.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | İşi §1 matrisine yerleştir; yoksa `general` de | matris satırı |
| 2 | §1.1 profilinden `MALIYET` satırını oku | gerekçe cümlesi |
| 3 | Aynı dosyaya ikinci yazıcı var mı bak | dosya listesi |
| 4 | Paralel sayacı say (≤ 3) | `n/3` değeri |
| 5 | Devir paketini 4 maddeyle yaz | 4/4 madde |
| 6 | Her ajandan 5 alanlı kutu iste | kutu beklentisi satırı |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : l4 test yazimi tester'a dagitildi        |
| KAPSAM : tests/VersaCoder.Tests/**, WORKFLOW §3   |
| KARAR  : matris satiri 4 (test -> tester); paralel|
|          2/3; ayni dosyaya ikinci yazici yok       |
| RİSK   : tester altyapisi hazir degilse general   |
|          degil, tester kurulumu once donecek       |
| KANIT  : AGENTS §1 satir 4 + devir paketi 4/4     |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Atama | §1 matrisinden satır gösteriliyor ya da `general` |
| Paralellik | `n/3` biçiminde sayı, 3'ü geçmiyor |
| Devir | 4 madde dolu (`KALAN IS / DOSYA / KANIT / SINIR`) |
| Kutu | 5 alan ASCII `+ - \|` ile çizilmiş |
| Gerekçe | §1.1 `MALIYET` satırından tek cümle |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| 4'ten fazla ajan çağrısı | rate limit kaza kaydı tekrarlanır | 3'e indir |
| Mimari işe `developer` ataması | ters ok riski (§1.2 madde 1) | `architect` |
| Devirde 2 madde | eksik devir taslaktır | 4 maddeyi tamamlat |
| Kutusuz yanıt | kaba metin | aynı kutuyu tekrar iste |
| Aynı dosyada iki yazıcı | ezme riski (AGENTS §7) | seriley al |

---

## §6 Yasaklar

1. **Matriste olmayan işe uzman atamak yasak** — `general` vardır.
2. **3'ten fazla paralel ajan yasak** — 2026-09-29 kaza kaydı (AGENTS §5.1).
3. **Aynı dosyaya iki yazıcı yasak** — önce kanıt, sonra sahiplik.
4. **Kutusuz teslim yasak** — 5 alan zorunlu.
5. **Tek seferde 8 dosyadan fazlası yasak** — bütçe (AGENTS §6).

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Matris okuma + atama | ≈5 dk | iş durur, `general`'a düşür |
| Paralel plan + sahiplik | ≈5 dk | dosyayı böl |
| Devir paketi yazımı | ≈5 dk | açıklamayı kısalt, 4 maddeyi koru |
| Kutu toplama (blok sonu) | ≈5 dk | kutusuz ajan sayılmaz |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [gate-acma-prompt](gate-acma-prompt.md) | dağıtımdan **sonra** kapı kapanışı |
| [developer-prompt](developer-prompt.md) | kod işi `developer`'a gittiğinde |
| [reviewer-prompt](reviewer-prompt.md) | inceleme işi `reviewer`'a gittiğinde |
| [risk-degerlendirme-prompt](risk-degerlendirme-prompt.md) | atama riskinin önceliklendirilmesi |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Matris tutarlılığı | AGENTS §1 ile aynı ajan adları | çapraz okuma |
| Paralellik | sayı ≤ 3 yazılı | `n/3` satırı |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
