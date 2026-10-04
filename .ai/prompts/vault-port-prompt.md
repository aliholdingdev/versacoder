---
title: "Vault Port Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Baska vault'tan bilgi tasima (port) icin ajana verilen is emri: bilgi olarak alma, yeniden yazim, kopyalama yasagi ve dogrulama"
---

# Vault Port Promptu

**Zorunlu Baglantilar:** [index](index.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [VISION](../VISION.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Başka vault'taki hangi bilgi buraya taşınır,
> nasıl yeniden yazılır* sorusunun, kopyalama yasağını koruyan tek cevabıdır.

---

## §0 Ozet (Answer First)

Port = **bilgiyi al, metni kopyalama.** Harici vault'lar (ör. `C:\www\...`
altı) yalnızca **yapı referansı** olarak okunur; cümle, tablo ya da şablon
birebir taşınmaz (G1 / ADR-002). Bu prompt taşınacak bilgiyi **kendi
cümlemizle yeniden yazar**, kaynağını düz metin olarak kaydeder ve
`NEEDS_VALIDATION` ile doğrulanacak yerleri işaretler.

| # | Taşınabilir | Taşınmaz |
|---|---|---|
| 1 | Dosya adı / klasör yapısı | Cümle, paragraf, tablo metni |
| 2 | Karar fikri (neden öyle) | Hazır şablon gövdesi |
| 3 | Sayı/ölçüm (kaynak gösterilerek) | Emoji / bozuk kodlama |
| 4 | Eksik listesi (ne var ne yok) | Yasağı ihlal eden örnek kod |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [CLAUDE](../CLAUDE.md) | G1 kopyalama yasağı, G5 kanıt | tam |
| 2 | [new-doc-contract](../.templates/new-doc-contract.md) | yeni dosya sözleşmesi | §0-§3 |
| 3 | [VISION](../VISION.md) | taşınan bilginin vizyona uyumu | tam |
| 4 | [AGENTS](../AGENTS.md) | kutu, devir, eşzamanlılık | §2-§5 |
| 5 | [keys](../keys.md) | taşınan terimlerin karşılığı | §2 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Bilgi portcusu - yeniden yazar, kopyalamaz.
> **KONU:** <kaynak vault / klasor> -> <hedef dosya>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once cevap:
> `PORT: <bilgi basligi>` + tek satir gerekce.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Kaynaktan cumle/tablo kopyalamak yasak (G1); yalnizca yeniden yazim.
> - Kaynagi dogrulanmamis bilgiyi "kesin" yazmak yasak; `NEEDS_VALIDATION`.
> - Yer tutucu (`{{...}}`) ile yayinlamak yasak (G4).
> - EF Core / string-concat SQL ornegi tasimak yasak (G2/G10).
> - Kutusuz teslim yasak (AGENTS §2).
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `PORT: ...` + tek satir gerekce.
> 2. En fazla 5 madde: tasinan bilgi kalemleri (kendi cumlemizle).
> 3. Kaynak tablosu: `# | kaynak (duz metin) | ne alindi | durum`.
> 4. Kanit: hedef dosya yolu + boyut + `check-links -> 0`.
> 5. 5 alanli kutu.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kaynak vault'u salt okunur aç | dosya listesi |
| 2 | Alınacak bilgiyi 5 maddede seç | 5 madde |
| 3 | Her kalemi kendi cümlemizle yeniden yaz | metin |
| 4 | Kaynağı düz metin kaydet + doğrulanacak yerleri işaretle | `NEEDS_VALIDATION` |
| 5 | Hedef dosyayı sözleşme (frontmatter 9 alan) ile yaz | dosya yolu |
| 6 | Link + boyut taraması, kutu | 0 bozuk link |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : klasor yapisi port edildi               |
| KAPSAM : .ai/prompts/, .ai/playbooks/ (yeni MD)  |
| KARAR  : yalnizca YAPI alindi; metinler bastan   |
|          yazildi (G1 / ADR-002)                   |
| RİSK   : kaynaktaki eski kural vault'ta farkli   |
|          olabilir - NEEDS_VALIDATION isaretlendi  |
| KANIT  : 20 prompt + 18 playbook / 0 bozuk link   |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Yeniden yazım | kaynak metinle birebir aynı cümle yok |
| Kaynak | her bilgi kaleminde düz metin kaynak var |
| Doğrulama | kanıtsız iddia `NEEDS_VALIDATION` |
| Sözleşme | frontmatter 9 alan, §0-§9 iskeleti |
| Kutu | 5 alan dolu, `KANIT` sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| Kaynakla birebir aynı cümle | G1 ihlali | yeniden yaz |
| "kesin" ama kaynak yok | G5 | `NEEDS_VALIDATION` koy |
| Yer tutucu sızdı | G4 | temizle, yeniden ölç |
| EF/concat örnek | G2/G10 | örnekleri çıkart |
| Kutusuz teslim | AGENTS §2 | kutuyu iste |

---

## §6 Yasaklar

1. **Birebir kopyalama yasak** (G1 / ADR-002) — yalnız yeniden yazım.
2. **Doğrulanmamış bilgiyi kesin sunmak yasak** — etiket zorunlu.
3. **Yer tutuculu yayın yasak** (G4) — üretimde 0.
4. **Yasağı ihlal eden örnek taşımak yasak** — EF / concat SQL.
5. **Kutusuz teslim yasak** — 5 alan zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Kaynak tarama | ≈10 dk | kapsamı daralt |
| Bilgi seçimi (5 madde) | ≈10 dk | ikinci oturuma böl |
| Yeniden yazım | ≈25 dk | dosya başına böl |
| Sözleşme + tarama | ≈10 dk | ölçmeden kapatma |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [docs-prompt](docs-prompt.md) | port edilen bilginin belgeye dönüşmesi |
| [link-taramasi-prompt](link-taramasi-prompt.md) | port sonrası link denetimi |
| [risk-degerlendirme-prompt](risk-degerlendirme-prompt.md) | doğrulanacak maddelerin önceliği |
| [session-kapanis-prompt](session-kapanis-prompt.md) | port kaydının devri |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Kopyalama | birebir cümle = 0 | karşılaştırma |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
