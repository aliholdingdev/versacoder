---
title: "Link Taramasi Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Vault ici baglanti taramasi icin ajana verilen is emri: hedef listesi, onarim stratejisi, 0 bozuk link kaniti"
---

# Link Taramasi Promptu

**Zorunlu Baglantilar:** [index](index.md) · [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [new-doc-contract](../.templates/new-doc-contract.md)

> **Tek cumlelik sorumluluk:** *Vault'ta hangi bağlantı kırık, ne yapılacak*
> sorusunun, tarama çıktısı ve onarım kararıyla verilen tek cevabıdır.

---

## §0 Ozet (Answer First)

Bozuk link **GATE-004a kırmızısıdır**: `check-links.ps1` çıktısında `BOZUK`
sayısı 0'dan büyükse kapı kapanmaz ([WORKFLOW](../WORKFLOW.md) §2.1). Bu
prompt taramayı koşar, her kırık hedefi **iki seçenekle** sınıflandırır
(link silinir ya da hedef üretilir) ve sayıyı 0'a indirir. Değişmez kural:
**var olmayan dosyaya link yazılmaz**, ad + `NEEDS_VALIDATION` bırakılır.

| # | Tarama alanı | Sıfır hedefi |
|---|---|---|
| 1 | Markdown `[x](yol)` bağlantıları | 0 bozuk |
| 2 | Aynı klasör kardeş dosya adları | 0 eksik |
| 3 | Kök `.ai/` derinliği hedefleri | 0 olmayan |
| 4 | Dış URL / `mailto:` | az kullan, kaydet |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [new-doc-contract](../.templates/new-doc-contract.md) | izinli link hedefleri tablosu | §3 |
| 2 | [CLAUDE](../CLAUDE.md) | G4 / G7 link kuralları | tam |
| 3 | [WORKFLOW](../WORKFLOW.md) | GATE-004a kabul kriteri | §2.1 |
| 4 | [AGENTS](../AGENTS.md) | kutu biçimi + `RİSK` kuralı | §2 |
| 5 | [index](../index.md) | kök navigasyonun beklediği hedefler | §3 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Vault baglanti denetcisi.
> **KONU:** <klasor ya da tum vault> · <tarih>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once cevap:
> `BOZUK: <sayi>` + tek satir gerekce. Sayi 0 degilse onarim kararini ver.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Var olmayan hedefe link yazmak yasak; ad + `NEEDS_VALIDATION` birak.
> - Hedefi uydurmak, dosyayi "var gibi" gostermek yasak (G5).
> - Izin disi hedefe (klasor alt katmanlari, baska hedef) link acmak yasak.
> - Taramayi kosturmadan "0 bozuk" demek yasak.
> - Onarimda 5'ten fazla madde acmak yasak; kalanini tabloya tasi.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `BOZUK: <sayi>` + tek satir gerekce.
> 2. Bulgu tablosu: `# | kaynak dosya : satir | hedef | karar`.
> 3. Karar: `SIL | URET | DUGRULE` - her satirda tek karar.
> 4. Kanit: tekrar kosulmus tarama -> `BOZUK: 0`.
> 5. 5 alanli kutu.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Tarama komutunu çalıştır | konsol çıktısı, sayı |
| 2 | Bozuk hedefleri dosya : satır listele | satır listesi |
| 3 | Hedefi izinli hedef listesiyle kıyasla | izin kararı |
| 4 | Kararı ver: sil / üret / düzelt | karar sütunu |
| 5 | Uygula (tek dosyaya birden çok yazıncı yok) | diff |
| 6 | Taramayı yeniden koş, kutuyu doldur | `BOZUK: 0` |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : 2 bozuk link onarildi                   |
| KAPSAM : .ai/prompts/index.md, .ai/playbooks/    |
|          index.md                                |
| KARAR  : hedef olmayan 2 link SILINDI; ad +      |
|          NEEDS_VALIDATION birakildi              |
| RİSK   : indeks §3 tablosu hala "rezerve" diyor  |
|          - guncelleme gerekiyor                  |
| KANIT  : check-links -> BOZUK: 0 (ikinci kosu)   |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Tarama | ikinci koşuda `BOZUK: 0` |
| Karar | her bulguda `SIL / URET / DUGRULE` var |
| Hedef | izinli hedef listesi aşılmamış |
| Kutu | 5 alan dolu, `KANIT` sayı içeriyor |
| Kayıt | değişiklik `log.md`'ye tarihli |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| `BOZUK > 0` | gate kapalı | kararı ver ve onar |
| Hedef üretilmiş gibi yazıldı | kanıtsız (G5) | ad + `NEEDS_VALIDATION` |
| Klavuz alt klasöre link | izin dışı | kural hedefine çevir |
| Tek turda "temiz" | tarama atlanmış | komutu gerçekten koş |
| 6+ madde | bütçe | en yüksekten başla |

---

## §6 Yasaklar

1. **Var olmayana link yazmak yasak** — ad + etiket yazılır.
2. **Taramasız "0 bozuk" demek yasak** — komut çıktısı zorunlu.
3. **İzin dışı hedefe link açmak yasak** — sözleşme §3 tablosu.
4. **Toplu ezme yasak** — dosya bazlı, tek sahip (AGENTS §7).
5. **Kutusuz teslim yasak** — 5 alan zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Tarama | ≈5 dk | işi durdur |
| Sınıflandırma | ≈10 dk | klasör başına böl |
| Onarım | ≈15 dk | en çok kullanılan dosyadan başla |
| Yeniden ölçüm + kutu | ≈5 dk | ölçmeden kapatma |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [mojibake-denetimi-prompt](mojibake-denetimi-prompt.md) | linklerle birlikte kodlama denetimi |
| [gate-acma-prompt](gate-acma-prompt.md) | GATE-004a kapanış paketi |
| [docs-prompt](docs-prompt.md) | hedef dosya üretilmesi gerekiyorsa |
| [session-kapanis-prompt](session-kapanis-prompt.md) | oturum sonu tarama kaydı |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Tarama iddiası | `BOZUK: 0` sayısal | §2.1 madde 6 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bu dosyada bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
