---
title: "Mojibake Denetimi Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Vault dosyalarinda kodlama (UTF-8) denetimi ve bozuk kodlama onarimi icin ajana verilen is emri: tarama, sniffling, yeniden yazim kaniti"
---

# Mojibake Denetimi Promptu

**Zorunlu Baglantilar:** [index](index.md) · [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [new-doc-contract](../.templates/new-doc-contract.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Hangi dosya yanlış kodlanmış, nasıl yeniden
> yazılır* sorusunun, tarama çıktısı ve UTF-8 kanıtıyla verilen tek cevabıdır.

---

## §0 Ozet (Answer First)

Kodlama bozukluğu **GATE-004a kırmızısıdır**: `find-mojibake.ps1` çıktısında
dosya sayısı 0'dan büyükse kapı kapanmaz ([WORKFLOW](../WORKFLOW.md) §2.1).
Belirti, Türkçe karakterlerin yerine geçen **iki-bytelık ASCII artıkları** ve
bilinmeyen karakter işaretidir; bu prompt belirtiyi **örnekleyerek yazmaz**,
sayar ve dosyayı UTF-8 (BOM'suz) ile yeniden yazar. Kopyalama yasaktır (G1):
bozuk metin kaynağından alınmaz, yerinde yeniden yazılır.

| # | Denetim | Sıfır hedefi |
|---|---|---|
| 1 | Dosya kodlaması | UTF-8, BOM'suz |
| 2 | Bozuk kodlama işaretleri | dosya sayısı = 0 |
| 3 | Bilinmeyen karakter yer tutucusu | 0 satır |
| 4 | Satır sonu | CR-only yasak, LF/CR-LF serbest |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [new-doc-contract](../.templates/new-doc-contract.md) | kodlama kuralı (§5) ve ölçüm | §5-§6 |
| 2 | [CLAUDE](../CLAUDE.md) | G5 kanıt yasağı, G1 kopyalama yasağı | tam |
| 3 | [WORKFLOW](../WORKFLOW.md) | GATE-004a kabul kriteri | §2.1 |
| 4 | [AGENTS](../AGENTS.md) | kutu biçimi, `RİSK` kuralı | §2 |
| 5 | [keys](../keys.md) | kodlama ile ilgili anahtarlar | §2 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Kodlama denetcisi - UTF-8 sorumlusu.
> **KONU:** <klasor ya da dosya listesi> · <tarih>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once cevap:
> `MOJIBAKE FILE COUNT: <sayi>` + tek satir gerekce.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Bozuk metni ornek olarak dosyaya yazmak yasak; yalnizca sayilir.
> - BOM ile kaydetmek yasak: UTF-8 **BOM'suz**.
> - Bozuk metni baska vault'tan kopyalamak yasak (G1).
> - Sayisiz "kodlama temiz" demek yasak (G5).
> - 5'ten fazla madde acmak yasak; kalanini tabloya tasi.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `MOJIBAKE FILE COUNT: <n>` + tek satir gerekce.
> 2. Bulgu tablosu: `# | dosya | belirti | karar (YENIDEN YAZ | ATLA)`.
> 3. Onarim: dosya UTF-8 (BOM'suz) ile yeniden yazildi.
> 4. Kanit: tekrar kosulmus tarama -> `0` + boyut karsilastirmasi.
> 5. 5 alanli kutu.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kodlama taramasını çalıştır | dosya sayısı |
| 2 | Bozuk dosyaları listele | dosya yolu listesi |
| 3 | Her dosyanın içeriğini oku, doğru yazımı üret | yeniden yazılmış metin |
| 4 | UTF-8 (BOM'suz) olarak kaydet | bayt kontrolü |
| 5 | Taramayı yeniden koş | `0` |
| 6 | Boyut + link taraması, kutu | iki sayı |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : 3 dosya UTF-8 yeniden yazildi           |
| KAPSAM : .ai/prompts/*.md (3 dosya)              |
| KARAR  : yerinde yeniden yazim; baska vault'tan   |
|          kopyalama YOK (G1)                       |
| RİSK   : anlam kaybina karsi ikinci okuma gerekli |
|          - gozden gecirme reviewer'da             |
| KANIT  : find-mojibake -> 0 (ikinci kosu) /       |
|          BOM -> 0 satir                           |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Tarama | ikinci koşuda dosya sayısı = 0 |
| Kodlama | UTF-8, BOM = 0 |
| İçerik | örnek bozuk metin dosyaya yazılmamış |
| Kutu | 5 alan dolu, `KANIT` sayı içeriyor |
| Kayıt | değişiklik `log.md`'ye tarihli |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| Bozuk metin örneği dosyada | kural ihlali | örneği sil, yalnız say |
| "temiz" ama komut yok | G5 | taramayı koş |
| BOM tespit edildi | sözleşme ihlali | BOM'suz yeniden kaydet |
| Kaynaktan yapıştırma | G1 | yerinde yeniden yaz |
| 6+ madde | bütçe | en yüksekten başla |

---

## §6 Yasaklar

1. **Bozuk metni örnek olarak yazmak yasak** — yalnızca sayılır.
2. **BOM ile kaydetmek yasak** — UTF-8 BOM'suz.
3. **Harici vault'tan kopyalamak yasak** (G1) — yerinde yeniden yazım.
4. **Taramasız temizlik beyanı yasak** — komut çıktısı zorunlu (G5).
5. **Kutusuz teslim yasak** — 5 alan zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Tarama | ≈5 dk | işi durdur |
| Sınıflandırma | ≈5 dk | klasör başına böl |
| Yeniden yazım | ≈20 dk | dosyayı böl, tek tek |
| Yeniden ölçüm + kutu | ≈5 dk | ölçmeden kapatma |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [link-taramasi-prompt](link-taramasi-prompt.md) | kodlama ile birlikte link denetimi |
| [gate-acma-prompt](gate-acma-prompt.md) | GATE-004a kapanış paketi |
| [docs-prompt](docs-prompt.md) | içeriğin baştan yazılması gerekiyorsa |
| [mojibake-denetimi-prompt](mojibake-denetimi-prompt.md) | tekrar denetim (aynı iş) |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Tarama iddiası | dosya sayısı = 0 sayısal | §2.1 madde 5 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bu dosyada bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
