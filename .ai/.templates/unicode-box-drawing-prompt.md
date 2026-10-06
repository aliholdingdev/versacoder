---
title: "Unicode Box-Drawing Promptu (EMEKLI)"
type: prompt
category: templates
status: superseded
version: 1.1.0
updated: 2026-10-01
authority: SSOT
scope: "EMEKLI: Unicode kutu-arti promptu kaldirildi; halefi ascii-box-drawing-prompt.md"
---

# Unicode Box-Drawing Promptu — EMEKLİ

**Zorunlu Bağlantılar:** [index](../index.md) · [templates index](index.md) · [halef: ascii-box-drawing-prompt](ascii-box-drawing-prompt.md)

> **EMEKLI (2026-10-01):** Bu promptun tüm içeriği halefine,
> [ascii-box-drawing-prompt.md](ascii-box-drawing-prompt.md), taşındığı için
> emekliye ayrıldı; artık kopyala-yapıştır prompt olarak çalıştırılmaz.
> Emekliliğin tek gerekçesi **içerik devri**dir: [ADR-029](../.decisions/accepted/ADR-029-ascii-box-drawing-only.md)
> ile çelişen dayatma metni halefe geçti, geriye yalnız karakter referansı kaldı.

- **Halef:** [ascii-box-drawing-prompt.md](ascii-box-drawing-prompt.md) (v2.0.0)
- **Kural kaynağı:** [ADR-029](../.decisions/accepted/ADR-029-ascii-box-drawing-only.md) · `CLAUDE.md` G4 (kutu taraması = 0)
- **Neden silinmedi:** bağlantı bütünlüğü (0 kırık link) + "silme yok, emeklilik var" (adr-index §3.6/3 ruhu).
- **Ölçüm notu (2026-10-06):** yukarıdaki "0 kırık link" ifadesi ilk yazım
  (2026-10-01) içindir; bugün aynı 6 bağlantının 3'ü henüz üretilmemiş omurga
  dosyalarına bakar (`../index.md` · ADR-029 ×2) ve diskte yok. Yeni eklenen her
  bağlantı yalnız diskte var olan bir hedefe gider (§6 D4 ile ölçülür).

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım. · 2026-10-01 — emekliye ayrıldı, içerik `ascii-box-drawing-prompt.md`'ye taşındı. · 2026-10-06 — §1–§7 eklendi (referans + dönüşüm + senaryo + hata modu + denetim + G eşlemesi); gerekçe düzeltildi: emekliye ayrılmanın nedeni içerik devridir.

---

## §1 EMEKLİ Dosyanın Durumu

**Tek cümlelik durum:** Bu dosya prompt üretmez; `U+2500-U+257F` bloğunun vault'taki
tek karakter referansıdır ve yalnız **okuma / çeviri / denetim** için açılır.

### §1.1 Bu Dosya Ne, Bu Dosya Değil

| # | Bu dosya | Bu dosya değil |
|---|---|---|
| 1 | Karakter cinsiyet tabloları (§2) | Üretim promptu — o [halefte](ascii-box-drawing-prompt.md) |
| 2 | ASCII ↔ Unicode dönüşüm referansı (§3) | Kullanım kararı — o ADR-029'da |
| 3 | Hata modu + denetim bilgisi (§5, §6) | Kural metni — o [CLAUDE.md](CLAUDE.md) §4.5'te |
| 4 | Arşiv (durum: `superseded`) | Aktif şablon — envanter satırı E6 "EMEKLİ" |

### §1.2 Ne Zaman Bakılır

| Durum | Yapılacak | Bölüm |
|---|---|---|
| Eski bir belgede Unicode kutu var, ASCII'ye çevrilecek | Eşleme tablosunu uygula, kaybı kayda geç | §3.2, §3.4 |
| Harici kaynaktan gelen Unicode diyagram okunacak | Glif + kod noktası eşini bul | §2 |
| Terminalde kutu karakterleri kare (tofu) görünüyor | Hata modunu teşhis et | §5.1 |
| ASCII diyagram yazılacak | Bu dosyaya değil, halefe git | [ascii-box-drawing-prompt.md](ascii-box-drawing-prompt.md) |
| Dosyanın hâlâ geçerli olup olmadığı sorulacak | `status: superseded` + §1.4 | §1.4 |

### §1.3 Emeklilik Kaydı

| Alan | Değer |
|---|---|
| Emeklilik tarihi | 2026-10-01 |
| Durum | `superseded` (silinmedi) |
| Halef | [ascii-box-drawing-prompt.md](ascii-box-drawing-prompt.md) — prompt bloğu + §1 gerekçe |
| Taşınan | Görev tanımı, karakter seti, zorunlular, doğrulama, şablonlar, çıktı formatı |
| Kalan (bu dosyada) | §2 karakter tabloları, §3 dönüşüm, §4 senaryo, §5 hata modu, §6 denetim, §7 eşleme |
| Bağlantı nedeni korundu | Silme yok, emeklilik var; hedefleri henüz üretilmemiş omurga dosyalarıdır (envanter §1.2, CF-15) |

### §1.4 Geri Açma Koşulu

Bu dosya yalnız şu üç koşul **hepsi** sağlandığında yeniden `active` olur:

1. Karar hücresi değişir ve bu değişiklik bir ADR ile kayda geçer (G9),
2. Halef `ascii-box-drawing-prompt.md` emekliye ayrılır ya da bölünür,
3. Envanter E6 satırı `EMEKLİ → YAZILDI` olarak tek geçişte güncellenir.

Koşul sağlanmadan `status` alanı değiştirilmez; değişiklik `version` minor +
Quality Report ile kaydedilir.

---
## §2 Karakter Cinsiyet Tabloları (`U+2500-U+257F`)

### §2.1 Blok Künyesi

| Ölçü | Değer | Kanıt |
|---|---|---|
| Kod aralığı | `U+2500` … `U+257F` | §6 D3 taraması |
| Kod noktası adedi | **128** (`0x257F − 0x2500 + 1`) | aritmetik |
| UTF-8 uzunluğu | **3 bayt/karakter** | `U+2500` → `E2 94 80`, `U+257F` → `E2 95 BF` (§6 D7) |
| Bayt deseni | `E2 94 80..BF` (2500–253F) · `E2 95 80..BF` (2540–257F) | §6 D7 |
| Hücre genişliği | Tek genişlik (CJK/emoji gibi **çift değil**) | §5.1/5 |
| ASCII karşılığı | Yok — indirgeme §3'te tanımlı | §3.2 |
| Üretilen belgede kutu karakteri | 0 | halef, "DOGRULAMA" bloğu (d) maddesi |

### §2.2 Cinsiyet Kavramı

Box-drawing'de **cinsiyet** = çizginin kalınlık/çiftlik durumudur; **yön** ise
hangi hücre kenarlarını birleştirdiğidir. Bir glifi bu iki eksen birlikte tanımlar.

| Cinsiyet | Tanım | Örnek | ASCII'ye düşerken |
|---|---|---|---|
| Hafif (light) | Tek ince çizgi | ─ │ ┌ | `-` `\|` `+` |
| Ağır (heavy) | Tek kalın çizgi | ━ ┃ ┏ | `-` `\|` `+` (kayıp: kalınlık) |
| Çift (double) | İki paralel çizgi | ═ ║ ╔ | `-` `\|` `+` (kayıp: çiftlik) |
| Kesikli (dashed) | Parçalı çizgi | ┄ ╌ | `-` (kayıp: kesik) |
| Yuvarlak | Kavisli köşe | ╭ ╮ | `+` (kayıp: kavis) |
| Diyagonal | Çapraz | ╱ ╲ ╳ | `/` `\` `X` |

**Cinsiyet birleştirme kuralı:** aynı hücrede kesişen çizgiler tek cinsiyette
olmalıdır. Farklı cinsiyetlerin ASCII'de karşılığı yoktur; dönüşümde tek cinsiyete
düşürülür ve bu kayıp §3.4'e yazılır.

### §2.3 Temel Çizgiler

| Kod | Glif | Türkçe ad | UTF-8 | ASCII karşılığı |
|---|---|---|---|---|
| `U+2500` | ─ | Hafif yatay | `E2 94 80` | `-` |
| `U+2501` | ━ | Ağır yatay | `E2 94 81` | `-` |
| `U+2502` | │ | Hafif dikey | `E2 94 82` | `\|` |
| `U+2503` | ┃ | Ağır dikey | `E2 94 83` | `\|` |

### §2.4 Köşe Ailesi (4 yön × 4 kalınlık)

| Yön | Hafif | Ağır | Ara kalınlıklar | Kod aralığı |
|---|---|---|---|---|
| Aşağı → sağ | `U+250C` ┌ | `U+250F` ┏ | ┍ ┎ (2) | `250C-250F` |
| Aşağı → sol | `U+2510` ┐ | `U+2513` ┓ | ┑ ┒ (2) | `2510-2513` |
| Yukarı → sağ | `U+2514` └ | `U+2517` ┗ | ┕ ┖ (2) | `2514-2517` |
| Yukarı → sol | `U+2518` ┘ | `U+251B` ┛ | ┙ ┚ (2) | `2518-251B` |

**Sayım:** her yönde 4 kod noktası × 4 yön = **16 köşe**. Ara kalınlık tek bir
kenarı ağırlaştırır (ör. ┍ = aşağı ağır + sağ hafif); ASCII'ye hepsi `+` düşer.

### §2.5 T-Bağı ve Dört Yol Aileleri

| Bağ | Hafif | Ağır | Anlam | Aralık |
|---|---|---|---|---|
| Dikey + sağa | `U+251C` ├ | `U+2523` ┣ | Devam + sağa dallanma | `251C-2523` (8) |
| Dikey + sola | `U+2524` ┤ | `U+252B` ┫ | Devam + sola dallanma | `2524-252B` (8) |
| Aşağı + yatay | `U+252C` ┬ | `U+2533` ┳ | Yukarıdan gelen kol + yatay | `252C-2533` (8) |
| Yukarı + yatay | `U+2534` ┴ | `U+253B` ┻ | Aşağıdan gelen kol + yatay | `2534-253B` (8) |
| Dört yol | `U+253C` ┼ | `U+254B` ╋ | Tam kesişim | `253C-254B` (16) |

**Toplam T/çapraz ailesi:** 8 + 8 + 8 + 8 + 16 = **48 kod noktası** (bloğun
%37,5'i). ASCII'de beşi de tek karaktere düşer: `+`.

### §2.6 Çift Çizgi Ailesi (`U+2550-U+256C`, 29 kod)

| Kod | Glif | Yön | ASCII karşılığı |
|---|---|---|---|
| `U+2550` | ═ | Yatay çift | `-` |
| `U+2551` | ║ | Dikey çift | `\|` |
| `U+2554` | ╔ | Aşağı-sağ | `+` |
| `U+2557` | ╗ | Aşağı-sol | `+` |
| `U+255A` | ╚ | Yukarı-sağ | `+` |
| `U+255D` | ╝ | Yukarı-sol | `+` |
| `U+2560` | ╠ | Dikey + sağa | `+` |
| `U+2563` | ╣ | Dikey + sola | `+` |
| `U+2566` | ╦ | Aşağı + yatay | `+` |
| `U+2569` | ╩ | Yukarı + yatay | `+` |
| `U+256C` | ╬ | Dört yol | `+` |

Kalan **18 kod** (`2552-2553`, `2555-2556`, `2558-2559`, `255B-255C`, `255E-255F`,
`2561-2562`, `2564-2565`, `2567-2568`, `256A-256B`) **hibrit** varyantlardır: bir
eksende çift, diğeri hafif. Hibritler ASCII'de ayrışmaz, tek cinsiyete düşer.

### §2.7 Yuvarlak, Diyagonal, Kesikli, Yarım

| Grup | Aralık | Adet | Glifler | ASCII karşılığı |
|---|---|---|---|---|
| Kesikli (üç/dört) | `2504-250B` | 8 | ┄┅┆┇┈┉┊┋ | `-` (kesik kaybı) |
| Çift kesikli | `254C-254F` | 4 | ╌╍╎╏ | `-` |
| Yuvarlak köşe | `256D-2570` | 4 | ╭╮╯╰ | `+` (kavis kaybı) |
| Diyagonal | `2571-2573` | 3 | ╱╲╳ | `/` `\` `X` |
| Yarım (hafif) | `2574-2577` | 4 | ╴ ╵ ╶ ╷ = sola · yukarı · sağa · aşağı | yok — komşuya katılır |
| Yarım (ağır) | `2578-257B` | 4 | ╸ ╹ ╺ ╻ | yok — komşuya katılır |
| Yarım tamamlayıcı | `257C-257F` | 4 | ╼ ╽ ╾ ╿ | yok — komşuya katılır |

**Kontrol toplamı:** 4 (temel) + 8 (kesikli) + 16 (köşe) + 48 (T/çapraz) +
4 (çift kesikli) + 29 (çift) + 4 (yuvarlak) + 3 (diyagonal) + 12 (yarım) = **128**
= blok kod noktası sayısı (§2.1). Sayım tutmazsa tablo eksiktir.

### §2.8 Cinsiyet Matrisi (Kesişim Kararı)

| Gelen cinsiyet | ASCII'ye çıkış | Korunabilir mi | Yöntem |
|---|---|---|---|
| Hafif + hafif | Tek `+` / `-` / `\|` | Tam | Doğrudan eşleme |
| Ağır + hafif (hibrit) | Tek cinsiyet (hafif) | Hayır | Kalınlık bilgisi §3.4'e kaydedilir |
| Çift + hafif | Tek cinsiyet (hafif) | Kısmen | Çiftlik başlık bandıyla anlatılır |
| Çift + çift | Tek `-` (çiftlik kaybolur) | Kısmen | Başlık `====` bandı (halef kural 5) |
| Kesikli + hafif | Düz `-` | Hayır | Kesiklik gerektiğinde ayrıca yazılır |
| Yuvarlak + hafif | Keskin `+` | Hayır | Geometri korunur, kavis kaybolur |
| Diyagonal + herhangi | `/` `\` `X` | Kısmen | Yalnız üç ASCII çapraz glifi var |

---
## §3 ASCII ↔ Unicode Dönüşüm Kuralları

### §3.1 Dönüşüm Yönü ve Kayıp Sözleşmesi

| Yön | Ne zaman | Kural |
|---|---|---|
| **Unicode → ASCII** | Vault belgesi üretirken (asıl yön) | §3.2 eşleme + §3.4 kayıp kaydı |
| **ASCII → Unicode** | Yalnız harici/legacy belge hazırlarken | §3.5 — vault çıktısında kullanılmaz |
| Okuma (değiştirme yok) | Harici kaynak incelenirken | §2 tabloları yeter |

**Kayıp sözleşmesi:** Unicode → ASCII **kayıplıdır** (lossy). Her dönüşümde
kaybolan bilgi (kalınlık, çiftlik, kavis, kesiklik) §3.4 tablosuna yazılır;
kaybı gizleyen "aynı görünüyor" iddiası kanıtsızdır (G9).

### §3.2 Unicode → ASCII Eşleme Tablosu

| Unicode | Kod | ASCII | Dönüşüm notu |
|---|---|---|---|
| ─ ━ | `2500` `2501` | `-` | Kalınlık kaybı |
| │ ┃ | `2502` `2503` | `\|` | Kalınlık kaybı |
| ┌ ┐ └ ┘ | `250C` `2510` `2514` `2518` | `+` | Köşe; yön ASCII'de konumdan anlaşılır |
| ├ ┤ ┬ ┴ ┼ | `251C` `2524` `252C` `2534` `253C` | `+` | T/çapraz tek karaktere iner |
| ┣ ┫ ┳ ┻ ╋ | `2523` `252B` `2533` `253B` `254B` | `+` | Ağır varyantlar da `+` |
| ═ ║ | `2550` `2551` | `-` `\|` | Çiftlik kaybı |
| ╔ ╗ ╚ ╝ ╠ ╣ ╦ ╩ ╬ | `2554`…`256C` | `+` | Çift köşe/T dahil |
| ╭ ╮ ╯ ╰ | `256D`…`2570` | `+` | Kavis kaybı |
| ╱ ╲ ╳ | `2571` `2572` `2573` | `/` `\` `X` | `X` büyük harf |
| ┄┅┆┇┈┉┊┋ | `2504-250B` | `-` | Kesiklik kaybı |
| ╌╍╎╏ | `254C-254F` | `-` | Çift kesiklik kaybı |
| ╴ ╵ ╶ ╷ | `2574-2577` | — | Komşuya katılır ya da silinir |
| ╸ ╹ ╺ ╻ ╼ ╽ ╾ ╿ | `2578-257F` | — | Aynı — §3.3.7 |

### §3.3 Sınır Durumları

**§3.3.1 Kesişim.** Unicode tek karakter ┼ verir; ASCII'de aynı işi tek `+`
yapar. Ancak `+` aynı zamanda köşe ve T-bağıdır: anlam **konumdan** çıkar
(hücrenin hangi kenarında `-`/`|` var). Çevrim sonunda her `+` için komşu
kenarlar sayılır; komşusu olmayan `+` = hata (§5.1/6).

**§3.3.2 T-bağı.** ├ → ASCII'de dallanma satır başındaki `+--` olur; dikey kol
`|` olarak devam eder. T'nin gövde yönü korunur, yalnız glif değişir:

```text
Unicode:             ASCII:
┌────┬────┐          +----+----+
│ A  │ B  │          | A  | B  |
├────┼────┤   -->    +----+----+
│ C  │ D  │          | C  | D  |
└────┴────┘          +----+----+
```

**§3.3.3 Çifte çizgi.** ═ / ║ ASCII'de `-` / `|` olur; **çiftlik** iki yolla
kurtarılır: (a) başlık satırı `======`, (b) üstte tek `===` bandı, gövde tek
çizgi. Çiftliği iki ayrı `-` hattıyla taklit etmek yanlıştır: iki hat tek hücre
kaplamaz, hizalama bozulur.

**§3.3.4 Yuvarlak köşe.** ╭ → `+`; kavis ASCII'de yoktur. Geometri (hangi
kenarların birleştiği) korunur, estetik kayıp §3.4'e yazılır.

**§3.3.5 Diyagonal.** ╱ ╲ ╳ → `/` `\` `X`. ASCII'de çapraz yalnız fence
içinde çizilir; kutu içinde çapraz gerekiyorsa ok (`->`, `<-`, `^`, `v`)
tercih edilir (halef KARAKTER SETI bloğu).

**§3.3.6 Kesikli çizgi.** ┄ → `-`; kesiklik korunacaksa satır arası noktalama
ya da ayrı bir "kesik" etiketi gerekir. Otomatik korunamaz — bilgi kaybı sayılır.

**§3.3.7 Yarım çizgi.** ╴ ╵ ╶ ╷ gibi tek kollu glifler ASCII'de anlamsızdır:
komşu hücreye **katılır** (varsayılan) ya da **çıkarılır**. Hangisinin
uygulandığı dönüşüm notunda belirtilir; sessizce atmak kanıtsız bırakır.

**§3.3.8 Cinsiyet karmaşası.** Tek kutuda hem ─ hem ═ varsa (hibrit) ASCII'ye
tek `-` düşer; ayrım gerekiyorsa başlık bandı kullanılır (§3.3.3).

**§3.3.9 Boyut sınırları.** Aynı hücre sayısı korunsa bile bayt büyür: 40
karakterlik kutu satırı Unicode'da 120 bayt, ASCII'de 40 bayttır (§2.1). Log
hattı uzunluk limiti varsa bu fark sınırlayıcıdır.

### §3.4 Kayıp Kaydı (Lossy Tablosu)

| # | Kaybolan bilgi | ASCII'de geri gelir mi | Kurtarma yolu |
|---|---|---|---|
| 1 | Kalınlık (hafif/ağır) | Hayır | Başlık bandı / etiket |
| 2 | Çiftlik (═) | Kısmen | `====` başlık bandı |
| 3 | Kavis (╭) | Hayır | Yok — geometri yeterli |
| 4 | Kesiklik (┄) | Hayır | Gerekirse ayrı satır etiketi |
| 5 | Tek kollu çizgi (╶) | Hayır | Komşuya katılır |
| 6 | Diyagonal yönü | Kısmen | `/` `\` ile iki yönde kalır |
| 7 | Orta kalınlık varyantı (┍ etc.) | Hayır | Yok — hafife düşer |
| 8 | Hibrit (çift+hafif) | Hayır | Tek cinsiyete + kayıp notu |

### §3.5 ASCII → Unicode Yönü (Yalnız Harici Belge)

1. Tek **cinsiyet** seç (varsayılan: hafif ─ │ ┌ ┐ └ ┘ ├ ┤ ┬ ┴ ┼).
2. Her `+` için **dört kenar** incele; hangi yönlerde çizgi varsa o T/köşe
   karşılığını seç (├ = sağa + dikey).
3. Tek genişlikte hizala: her satır aynı hücre sayısında (§4.2'de 64 kolon).
4. Üç tur doğrulama: (a) kod noktası taraması, (b) genişlik ölçümü,
   (c) cinsiyet tutarlılığı. 3. turda tutarsızsa blok raporlanır, uydurulmaz.

> Bu yön **vault üretim çıktısında kullanılmaz**; çıktı yönü daima ASCII'dir
> (üretim kuralı haleftedir).

### §3.6 Dönüşüm Sonrası Doğrulama

| # | Kontrol | Beklenen |
|---|---|---|
| 1 | Kalan `U+2500-U+257F` karakteri | 0 |
| 2 | Her satırın hücre genişliği | Eşit |
| 3 | Kapalı parantez (`+` … `+`) çifti | Eşit |
| 4 | Kayıp kaydı (§3.4) | Doldurulmuş |
| 5 | Dosya kodlaması | UTF-8, BOM yok, LF |

---
## §4 Kullanım Senaryoları

### §4.1 Konsol Çıktısı

| # | Senaryo | Gereken | Teşhis |
|---|---|---|---|
| 1 | Windows konsolu | Konsol kod sayfası UTF-8 | `chcp` (yazmaz, yalnız etkin sayfayı gösterir) |
| 2 | PowerShell penceresi | `[Console]::OutputEncoding.WebName` | `utf-8` değilse tofu riski |
| 3 | Yazı tipi | Font `U+2500` bloğunu içermeli | Lucida Console / Consolas destekler; **raster (varsayılan Terminal) font desteklemez** |
| 4 | Çıkış dosyası | UTF-8, BOM yok | §6 D2 |
| 5 | Decode yönü | Okuyan taraf UTF-8 bilmeli | CP1254 ile okunursa §5.1/3 |

**Konsolda okuma örneği (salt okunur):**

```powershell
'[Output] {0} · [Input] {1}' -f [Console]::OutputEncoding.WebName, [Console]::InputEncoding.WebName
chcp                      # etkin kod sayfasini yazar (degistirmez)
(Get-Item 'C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md').Length
```

### §4.2 Markdown Blok

| Kural | Değer | Neden |
|---|---|---|
| Sarmalayıcı | Üç tırnaklı kod bloğu | Renderer monospace fonta düşer |
| Kolon | 64 (standart) · en fazla 80 | [ascii-diagram-template](architecture/ascii-diagram-template.md) |
| Girinti | Kod bloğu içinde ek girinti yok | Girinti hizayı bozar |
| TAB | Yok — 2 boşluk | TAB genişliği ortama göre değişir |
| Satır sonu | LF | Vault dosyaları LF kullanır |
| Uzunluk | Her satır eşit hücre sayısı | Hizasız kutu okunmaz |
| Emoji/CJK | Yok | Çift genişlik hücreyi kaydırır (§5.1/5) |

**Doğru ve yanlış örnek:**

```text
OK   (64 kolon, tek genişlik):
+------------------------------+
| BASLIK                       |
+------------------------------+
| govde                        |
+------------------------------+

HATA (satir uzunluklari farkli):
+-----------------+
| BASLIK          |
+------------------------+
| govde                   |
+-----------------+
```

### §4.3 Log Biçimlendirme

| Alan | Örnek | Not |
|---|---|---|
| Kolon hizası | `│ ANSI   │ 200 │` | Sayım hücre (karakter) üzerinden yapılır, bayt değil |
| Hata kutusu | `┌─ HATA ─┐` başlık + gövde | Log'a Unicode kutu giriyorsa tüketici UTF-8 okumalı |
| Bayt maliyeti | 40 gliflik satır = 120 bayt | ASCII aynısı 40 bayt (§2.1) |
| Tercih | Log üretiminde ASCII `+ - \|` (halef alfabeti) | Heterojen tüketici varsa ASCII seçilir |
| Kırılma | tek satır politikası | Kutu satırı kırılırsa hizalama çöker |

### §4.4 Dış Kaynak Okuma ve Çeviri

| Adım | İş | Çıktı |
|---|---|---|
| 1 | Kaynağı olduğu gibi oku (değiştirme yok) | Ham Unicode diyagram |
| 2 | Kod noktalarını listele (§6 D3) | Glif envanteri |
| 3 | §3.2 ile ASCII'ye çevir | Eşleşen ASCII blok |
| 4 | §3.4 kayıp kaydını doldur | Kayıp listesi |
| 5 | §3.6 doğrulamasını koş | 0 kalan kutu karakteri |

### §4.5 Senaryo Seçim Tablosu

| Girdi | Çıktı | Hangi bölüm |
|---|---|---|
| Eski Unicode diyagram + yeni Markdown belge | ASCII kutu | §3.2 + §3.4 |
| Unicode diyagram + terminal logu | ASCII satır + bayt limiti | §4.3 |
| Unicode diyagram + sadece okuma | Dokunma | §4.4/1 |
| ASCII diyagram + Unicode çıktı talebi | Vault dışında | §3.5 |
| Bozuk hizalı Unicode kutu | Önce hizala, sonra çevir | §3.6 |

---

## §5 Hata Modları

### §5.1 Hata Modu Tablosu

| # | Hata modu | Belirti | Kök neden | Tespit | Düzeltme |
|---|---|---|---|---|---|
| 1 | Karakter desteklemeyen terminal | Kare/boşluk (tofu) | Font veya kod sayfası `U+2500` bloğunu taşımıyor | Glif görünmüyor ama dosyada var | ASCII'ye çevir (§3.2) ya da font/kod sayfası değiştir |
| 2 | Monospace olmayan yazı tipi | Sütunlar kaymış | Hücre sayısı eşit ama görsel genişlik farklı | Satır uzunlukları eşit, görünür hizasız | Monospace fonta geç, yeniden ölç |
| 3 | UTF-8 olmayan kod sayfası | Çift baytlı harf yığını (mojibake) | UTF-8 baytları CP1254/CP437 ile okundu | §6 D2 sayısı > 0 | Kod sayfasını UTF-8 yap, dosyayı yeniden kaydet |
| 4 | BOM'lu dosya | İlk satırda tanımsız üç sembol | UTF-8-BOM ile kaydedilmiş | §6 D2 BOM=VAR | BOM'u kaldır (vault: BOM YOK) |
| 5 | Çift genişlik karakter kayması | Bir satır 1 hücre fazla | CJK/emoji tek hücrede 2 hücre kaplar | §6 D5 genişlik ölçümü satır atlar | Kutu içine CJK/emoji koyma; ASCII'ye indir |
| 6 | Yalnız kalan `+` | Hiç `-`/`\|` komşusu olmayan `+` | Köşe yanlış eşleşmiş | §3.3.1 komşu sayımı | Bloğu yeniden çiz |
| 7 | Çift çizgi = iki tek hat | `-` üst üste iki satır | Çiftlik ASCII'de iki hatla taklit edilmiş | Gözden geçirme | Başlık bandına çevir (§3.3.3) |
| 8 | Yarım çizgi artığı | Satır ortasında kesik glif | ╶ çevrilmeden bırakılmış | §6 D3 | Komşuya kat (§3.3.7) |
| 9 | TAB sızıntısı | Hizası makineye göre değişir | Sekme ile hizalanmış | `Select-String -Pattern ([char]9)` | 2 boşlukla yeniden yaz |
| 10 | Kırık bağlantı | `](…)` hedefi diskte yok | Henüz üretilmemiş omurga dosyası | §6 D4 | Yalnız var hedeflere link ekle (G7) |
| 11 | Bayt/uzunluk sanal hizası | Bayt uzunluğu eşit, hücre değil | 3 baytlık glif 1 bayta indirgenmeden sayılmış | §6 D5 ile çelişen sayım | Hücre sayarak ölç |
| 12 | Emekli promptun kullanılması | Eski dosyadan kopyalanan dayatma metni | Dosya hâlâ `superseded` | §1.4 | Halefe git |

### §5.2 Teşhis Akışı

```text
Kutu gorunmuyor mu?
  +-- Dosyada glif yok        -> icerik zaten ASCII cevrilmis, hata degil (3.2)
  +-- Dosyada glif var        -> UTF-8 mi okunuyor?  (6 D2)
       +-- mojibake > 0       -> kod sayfasi hata modu #3
       +-- mojibake = 0       -> font hata modu #1 (raster font)
Hizasiz mi?
  +-- Monospace mi?           -> hata modu #2
  +-- Cift genislik var mi?   -> hata modu #5
  +-- TAB var mi?             -> hata modu #9
Bos mu gorunuyor?
  +-- BOM var mi?             -> hata modu #4
  +-- Dosya boyutu kucuk mu?  -> yazma yarida kalmis (6 D1)
```

### §5.3 Hata Modu → Bölüm Eşlemesi

| Hata modu | Kapanış bölümü | Komut |
|---|---|---|
| #1 tofu | §3.2 + §4.1 | §6 D7 |
| #2 font | §4.2 | §6 D5 |
| #3 mojibake | §4.1/5 | §6 D2 |
| #4 BOM | §4.2 | §6 D2 |
| #5 çift genişlik | §4.2 | §6 D5 |
| #6–#8 glif hataları | §3.3 | §6 D3 |
| #9 TAB | §4.2 | `Select-String` |
| #10 bağlantı | §7.1 (G7) | §6 D4 |

---
## §6 Denetim Komutları (Salt-Okunur PowerShell)

Aşağıdaki komutların **hiçbiri dosyayı değiştirmez**; yalnız okur ve ekrana yazar.
Her blok kendi `$d` değişkenini tanımlar, tek başına çalıştırılabilir.

```powershell
# D1 — Satir ve KB olcumu (G6: >= 480 satir · K15: 26-44 KB)
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'
'{0} satır · {1:N1} KB' -f (Get-Content $d -Encoding UTF8).Count, ((Get-Item $d).Length/1KB)
```

```powershell
# D2 — BOM ve mojibake sayaci (G5: BOM YOK · mojibake 0)
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'
$b=[System.IO.File]::ReadAllBytes($d)[0..2] -join ','
'BOM={0} · mojibake={1}' -f $(if($b -eq '239,187,191'){'VAR'}else{'YOK'}), `
  ([regex]::Matches((Get-Content $d -Raw -Encoding UTF8),'\u00C3.|\u00E2\u20AC')).Count
```

```powershell
# D3 — Kutu karakteri taramasi (bu referans dosyada bilerek > 0 cikar)
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'
([regex]::Matches((Get-Content $d -Raw -Encoding UTF8),'[\u2500-\u257F]')).Count
```

```powershell
# D4 — Baglanti denetimi (G7): her hedef diskte var mi?
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'; $dir=Split-Path $d
[regex]::Matches((Get-Content $d -Raw -Encoding UTF8),'\]\(([^)]+)\)') | ForEach-Object {
  $t=$_.Groups[1].Value
  if($t -match '^(https?|mailto):')                  { 'EXT     '+$t }
  elseif(Test-Path -LiteralPath (Join-Path $dir $t)) { 'OK      '+$t }
  else                                               { 'BROKEN  '+$t } }
```

```powershell
# D5 — Frontmatter 8 alan (G3) + satir uzunluk dagilimi (hizalama)
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'
Get-Content $d -Encoding UTF8 | Select-Object -First 9
(Get-Content $d -Encoding UTF8 | Select-Object -Skip 10 | ForEach-Object { $_.Length }) |
  Group-Object | Sort-Object Count -Descending | Select-Object -First 3 Name, Count
```

```powershell
# D6 — Yer tutucu taramasi (G4 hedefi: 0)
$d='C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md'
([regex]::Matches((Get-Content $d -Raw -Encoding UTF8),'\{\{[^}]+\}\}')).Count
```

```powershell
# D7 — UTF-8 bayt deseni dogrulamasi (U+2500 -> E2 94 80 · U+257F -> E2 95 BF)
([System.Text.Encoding]::UTF8.GetBytes([char]0x2500) | ForEach-Object { $_.ToString('X2') }) -join ' '
([System.Text.Encoding]::UTF8.GetBytes([char]0x257F) | ForEach-Object { $_.ToString('X2') }) -join ' '
```

```powershell
# D8 — Uretim belgesinde kutu karakteri sayaci (halef icin hedef: 0)
$uretim='C:\versacoder\.ai\.templates\ascii-box-drawing-prompt.md'
([regex]::Matches((Get-Content $uretim -Raw -Encoding UTF8),'[\u2500-\u257F]')).Count
```

> **Not:** D3 bu dosyada **bilerek 0'dan büyük** çıkar; bu dosya referans olduğu
> için glif içerir. Sıfır hedefi **üretilen** belgedir (D8).

### §6.1 Beklenen Çıktılar (2026-10-06 ölçümü)

| Komut | Beklenen | Yorum |
|---|---|---|
| D1 | ≥ 480 satır · ≥ 26,0 KB | G6 + K15 |
| D2 | BOM=YOK · mojibake=0 | G5 |
| D3 | > 0 | Referans dosyası glif içerir |
| D4 | EXT/OK/BROKEN satırları | BROKEN yalnız omurga hedefleri |
| D5 | 8 satır frontmatter | G3 |
| D6 | 0 | G4 |
| D7 | `E2 94 80` ve `E2 95 BF` | §2.1 künyesi |
| D8 | 0 | Halef dosya ASCII-only |

---

## §7 G1..G12 Eşlemesi ve Doğrulama

### §7.1 Guardrail Eşlemesi

| ID | Tek satır özet | Bu dosyadaki karşılığı | Kanıt |
|---|---|---|---|
| G1 | Harici vault'tan içerik kopyalanamaz | Tablolar bu dosya için üretildi; yalnız yapı referansı | §2–§3 |
| G2 | EF Core yerine ADO.NET + Dapper | İlgisiz — veri erişimi yok | — |
| **G3** | Frontmatter'sız Markdown yok (8 alan) | 8 alan birebir korundu, ilk 9 satırda | §6 D5 |
| G4 | Üretilen dosyada yer tutucu 0 | iki suslu parantezli yer tutucu hiç eklenmedi | §6 D6 |
| **G5** | Türkçe zorunlu; mojibake yasak | Tüm metin Türkçe; BOM YOK · mojibake 0 | §6 D2 |
| G6 | Şablon < 480 satır ise reddedilir | §1–§7 iskelet ≥ 480 satır | §6 D1 |
| **G7** | Bozuk bağlantı yasak | Yeni bağlantı yalnız var hedeflere | §6 D4 |
| G8 | GATE-003 onaysız ekran/kod | Ekran/kod üretilmedi | — |
| G9 | ADR'siz mimari karar; kanıtsız sayı | Karar ADR-029'da; sayılar §6 ile ölçülür | §1.3, §6.1 |
| G10 | Concat SQL yerine `@p1..@pN` | İlgisiz — SQL yok | — |
| G11 | `TreatWarningsAsErrors` + %90 coverage | İlgisiz — kod/test yok | — |
| G12 | Envantere içerik kopyalanamaz (SRP) | Yalnız bağlantı; envanter E6 satırı değiştirilmedi | [index](index.md) §7.3 |

### §7.2 Kalite Sözleşmesi Eşlemesi

| Madde | Beklenen | Ölçüm |
|---|---|---|
| K15 (KB aralığı) | 26–44 KB | §6 D1 |
| G6 (satır) | ≥ 480 satır | §6 D1 |
| G3 (frontmatter) | 8 alan · `status: superseded` | §6 D5 |
| G5 (Türkçe/mojibake) | mojibake 0 · BOM YOK | §6 D2 |
| G7 (bağlantı) | Yeni bağlantı = var hedef | §6 D4 |
| G4 (yer tutucu) | 0 | §6 D6 |

### §7.3 Quality Report Kalıbı

```markdown
## Quality Report — unicode-box-drawing-prompt.md (YYYY-MM-DD HH:MM)
- Ölçüm:      <satır> satır / <KB> KB   (G6 >= 480 · K15 26-44)
- BOM:        YOK    ·   Mojibake: 0    (G5)
- Bağlantı:   <toplam> bağlantı · yeni <yeni> · kırık <kirik>   (G7)
- Yer tutucu: 0 (G4)   ·   Frontmatter: 8/8 (G3)
- Durum:      superseded — EMEKLİ, halef ascii-box-drawing-prompt.md
- Karar:      ONAYLANDI | RED (<gerekçe>)
```

**İlgili:** [ascii-box-drawing-prompt](ascii-box-drawing-prompt.md) (halef,
üretim kuralı) · [templates index](index.md) (envanter E6) ·
[ascii-diagram-template](architecture/ascii-diagram-template.md) (64 kolon
standardı) · [CLAUDE.md](CLAUDE.md) (§4.5 G1..G12) · [adr-index](adr/adr-index.md)
(karar kaydı — silme yok, emeklilik var) · [session-log](session-log-template.md)
(oturum kaydı).

**SSUT:** Bu dosya emekli promptun **arşiv + referans** hâlidir; üretim kuralı
[halefte](ascii-box-drawing-prompt.md), envanter kaydı [index](index.md)'dedir.
Buraya kopyalanan her kural ikinci gerçek kaynaktır (G12).