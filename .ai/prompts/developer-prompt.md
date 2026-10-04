---
title: "Developer Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Kod yazimi / refactor isi icin developer ajana yapistirilan tam prompt: 7 adim, kirmizi->yesil kurali, iki bacak TFm, kanit kutusu"
---

# Developer Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu kod neyi değiştiriyor, hangi katmanda,
> hangi kanıtla* sorusunu 7 adımda cevaplayan iş emridir.

---

## §0 Ozet (Answer First)

Kod işi `developer`'ındır; mimari karar önce `architect`'ten gelmişse o
karar **bağlayıcıdır** (ADR-028 yönü: `l7..l0`, `l4 -> l2` doğrudan ok yok).
Bu prompt kullanılmadan önce **GATE-003 kapalıysa kod yazılmaz** — onaysız
her `.cs` geri alınır. Çıktı: diff + `dotnet build` + `dotnet test` üçlüsü.

| # | Kullanım anı | Kardeş prompt |
|---|---|---|
| 1 | Yeni use-case metodu / servis yazımı | [refactor-prompt](refactor-prompt.md) |
| 2 | Kırmızı testi yeşile çevirme | [tester-prompt](tester-prompt.md) |
| 3 | ADR-028'de atanmış katmana dosya ekleme | [architect-prompt](architect-prompt.md) |
| 4 | Hata ayıklama kilitliyse | [debugger-prompt](debugger-prompt.md) |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [WORKFLOW](../WORKFLOW.md) | gate durumu, GATE-003 HARD STOP | §1, §2.2 |
| 2 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | hangi katman, hangi proje, hangi yön | §2.1 |
| 3 | [AGENTS](../AGENTS.md) | 5 alanlı kutu, devir paketi, 3-STRIKE | §2-§4 |
| 4 | [layer-boundary-check](../checklists/layer-boundary-check.md) | mimari etki varsa 13 soru | tam |
| 5 | [keys](../keys.md) | anahtar -> ilgili `.rules/` dosyası | §2 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Senior .NET Developer - net48 + net10.0-windows, DevExpress
> Universal v26.1, ADO.NET + Dapper.
> **KONU:** <degisiklik-tanimi> · <etkilenen dosyalar>
>
> **GOREV:** §2.1'deki 7 adimi uygula, `(n/7)` yaz. Once kirmizi (mevcut
> durum + test), sonra yesil (duzeltme + kanit).
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - EF Core **yok**; veri erisimi ADO.NET + Dapper, sorgu **parameterized**
>   (`@p1`), string-concat SQL yasak.
> - DevExpress yalniz **v26.1**; ribbon yalniz `RibbonControlStyle.Office2010`.
> - Tek bacakli degisiklik yasak: `net48` **ve** `net10.0-windows` gerekcelenir.
> - GATE-003 kapaliyken kod yazmak yasak (G12).
> - Kanitsiz "bitti" yasak; `NoWarn` ile hata gizlemek yasak.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `YAPIYORUM: <degisiklik>` + tek satir gerekce.
> 2. Sonra en fazla 5 madde: etkilenen dosyalar, katman yonu, test etkisi.
> 3. Kanit: `dotnet build -> 0 hata (2 bacak)` + `dotnet test -> X gecti, 0 kaldi`.
> 4. Ilerleme: her adimda `(n/7)`.
> 5. 5 alanli kutu (AGENTS §2) - kutusuz cikti islenmez.

### §2.1 Adimlar (n/7)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Gate durumunu sor (GATE-003 açık mı?) | gate satırı |
| 2 | ADR-028 katmanını ve yönünü doğrula | katman adı + ok yönü |
| 3 | Kırmızı: davranışı anlatan testi yaz/yürüt | `dotnet test` kırmızı |
| 4 | Minimal düzeltmeyi yaz (tek sorumluluk) | diff |
| 5 | İki bacakta derle | `dotnet build` × 2 = 0 hata |
| 6 | Testleri yeşile çevir, yeni yolu da kapsa | `dotnet test` yeşil |
| 7 | Kutuyu doldur, `log.md`'ye satır düş | kutu + kayıt |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : SessionStore UTC kaydirma duzeltildi    |
| KAPSAM : src/.../Store/SessionStore.cs (42->51)  |
| KARAR  : donusturme tek noktada; tekrar = iki     |
|          kopya, birlesik testte gorunur           |
| RİSK   : eski kayitlar yerel saatte - migration   |
|          NEEDS_VALIDATION                         |
| KANIT  : build -> 0 hata (2 bacak) /              |
|          test -> 12 gecti, 0 kaldi                |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Build | iki TFM'de de 0 hata, `NoWarn` satırı yok |
| Test | `dotnet test` 0 başarısız, atlama yok |
| Katman | yön ADR-028 §2.1 ile aynı, `l4 -> l2` yok |
| Diff | tek sorumluluk; ilgisiz dosya değişmemiş |
| Kutu | 5 alan dolu, `KANIT` somut (sayı) |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| "derlendi" ama komut yok | kanıtsız (G5) | çıktıyı yapıştır |
| EF Core paketi/`using` | G2 CRITICAL | paketi sil, Dapper'a çevir |
| Ters ok (`l2 -> l4`) | ADR-028 ihlali | portu `l0`'a, uygulamayı `l4`'e |
| Tek bacakta yeşil | G6 | diğer TFM'de de derle |
| Kutusuz yanıt | AGENTS §2 | kutuyu iste |

---

## §6 Yasaklar

1. **EF Core ve string-concat SQL yasak** (G2) — sorgu `@p1..@pN`.
2. **v26.1 dışı DevExpress ve Office2010 dışı ribbon yasak** (G3).
3. **Onaysız kod yazma yasak** — GATE-003 HARD STOP (G12).
4. **Kanıtsız "bitti" yasak** — build/test çıktısı zorunlu (G5).
5. **Başka ajanın dosyasına yazma yasak** — tek sahip (AGENTS §7).

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Gate + katman doğrulama | ≈10 dk | işi durdur |
| Kırmızı test | ≈15 dk | kapsamı küçült |
| Düzeltme + build | ≈25 dk | 3-STRIKE: yöntemi değiştir |
| Test + kutu + kayıt | ≈15 dk | özetle |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [refactor-prompt](refactor-prompt.md) | davranış değişmeyecek yeniden yazım |
| [debugger-prompt](debugger-prompt.md) | nedeni bilinmeyen hata/çökme |
| [reviewer-prompt](reviewer-prompt.md) | kod bittikten sonra inceleme |
| [security-prompt](security-prompt.md) | OWASP/parametreleme denetimi |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Build iddiası | 2 bacak `0 hata` cümlesi var | §2.1 madde 5 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
