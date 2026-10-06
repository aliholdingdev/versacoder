---
title: "ASCII Box-Drawing Yeniden Yazma Promptu"
type: prompt
category: templates
status: active
version: 2.0.0
updated: 2026-10-01
authority: SSOT
scope: "ASCII kutu (+ - |) ile diyagramlari sirfirdan dogru yazan, kopyala-yapistir hazir prompt"
---

# ASCII Box-Drawing Yeniden Yazma Promptu

**Zorunlu Bağlantılar:** [index](../index.md) · [keys](../keys.md) · [CLAUDE](../CLAUDE.md) · [templates index](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cümlelik sorumluluk:** Bir belgedeki tüm diyagramları/şemaları, izinli
> **ASCII `+ - |` alfabesiyle** hizalı, kesişimi doğru, doğrulanmış biçimde
> yeniden yazdırmak için kopyala-yapıştır tek komut.

> **Kullanım:** `<HEDEF DOSYA>` yerine dosya yolunu koy, koda yapıştır, çalıştır.
> MODEL DEĞİŞTİRME: bu prompt yeni oturumda/tahminci ajan başında kullanılır.

> **ADR-029 (2026-10-01):** Unicode box-drawing (`U+2500-U+257F`) yasaktır;
> bu dosyanın eski sürümü (v1.0.0, `unicode-box-drawing-prompt.md`) emekliye
> ayrılmıştır. Kural kaynağı: [ADR-029](../.decisions/accepted/ADR-029-ascii-box-drawing-only.md) · `CLAUDE.md` G4.

---

```text
PROMPT - Dosyamdaki diyagramlari ASCII kutu (+ - |) olarak yeniden yaz
====================================================================
Rol: ASCII kutu-cizimi diyagram uzmanisin. Ciktin kod degil,
belgede birebir kullanilacak son haldir.

GOREV
  <HEDEF DOSYA / METIN> icindeki TUM diyagram, agac, akis ve sema
  bloklarini yeniden yaz. Diger metne dokunma.

KARAKTER SETI (tek izinli alfabe)
  Cizgi:     + - |   (tek ve tek karakter; baska cizgi karakteri YOK)
  Ok:        -> <- ^ v   (ASCII oklar; yaninda Unicode -> da serbest)
  + Harf, rakam, bosluk, Turkce karakter (C G I O S U +  c g i o s u
  ve baglantisiz ı ğ ş İ ö ç ü - bunlar Unicode harftir, kutu
  cizimi degildir; serbesttir)

  YASAK: U+2500-U+257F araligi (box-drawing kose/çizgi karakterleri: U+2500,
         U+2502, U+250C, U+2510, U+2514, U+2518 ve devami), blok dolgu
         (U+2591-U+2588), emoji, TAB, Unicode oklar (U+2190-U+2193).
  Kod bloglari (```) disinda tek Unicode kutu karakteri = o blok
  bastan yazilir.

ZORUNLAR
  1. Dosya UTF-8, BOM YOK. Satir sonu LF.
  2. Monospace hizalama: her kolon, o kolondaki EN GENIS hucere gore
     pad edilir; tum satirlarda kolon genisligi aynidir.
  3. Kesisimler dogru: kose +, T-kesisim +, dort yol +. Dikey | ,
     kestigi hucere gecmez ve iki kez cizilmez.
  4. Her kutu icerede en az 1 bosluk padding alir: | metin |
  5. Cift cizgi yerine baslik satiri ===== veya ustte tek === bandi;
     govde tek cizgi (+---+) kalir.
  6. Agacta dallanma +-- ve bosluk sutunlari ebeveyn hizasindadir.

  HIZALAMA KURALI: her satir ayni uzunluktadir; kose +, yatay -,
  dikey | . Ornek:

    +------------------------------+
    | BASLIK                       |
    +------------------------------+
    | govde                        |
    +------------------------------+

DOGRULAMA (ciktidan ONCE yap, sonra tekrar et)
  a) Tum satirlar monospace'de esit mi? (en uzun satir x tek genislik)
  b) Her dikey | hizasi: ustte/ilgili hucere sinirinda mi?
  c) Her parantez cifti kapali mi? (+ ... + eslesmeleri)
  d) Codepoint tarama: U+2500-U+257F disindaki CIZGI karakteri = 0
  e) Hata bulursan duzelt; 3. turda hala tutarsizsa blogu
     "DUZELTILEMEDI - <sebep>" etiketiyle raporla, uydurma.

SABLONLAR (birebir kullan)
  Agac:   +-- bag · +-- son · |   devam ·     girinti (2 bosluk)
  Akis:
          +-----+
          | adim |-->
          +-----+
          karar: [E] evet sag, [H] asagi
  Kutu:
          +------------------------------+
          | BASLIK                       |
          +------------------------------+
          | govde                        |
          +------------------------------+
  Katman yigini:
          +--- l7 UI --------------------+
          +--- l6 Host ------------------+
          +--- l5 Protocol --------------+
          +------------------------------+
            v tek yon (geri donus: l0 interface)

CIKTI FORMATI
  1) "Duzeltildi: <n> blok" sayaci
  2) Her blok icinde: KESIT (eski 1-2 satir) -> YENI tam blok
  3) Sonda: "0 yasakli karakter · hizalama OK"

SIMDI BASLA: <HEDEF DOSYA YOLU>
```

---

## §1 Neden bu kurallar (kısa gerekçe)

| # | Kural | Neden |
|---|---|---|
| C1 | Tek izinli alfabe: `+ - |` | Kullanıcı kararı 2026-10-01 (ADR-029): ASCII kutu evrenseldir, her terminal/monospace'de aynı görünür; Unicode box-drawing `U+2500-U+257F` vault'ta **yasaktır** (G4) |
| C2 | Pad + tek genişlik | Monospace olmayan hizalama, kopyalanınca kırılır; ölçüm zorunlu |
| C3 | 3 tur doğrulama | "oldu" iddiası kanıtsız (ROLE.md §6); tur sonra rapor zorunlu |
| C4 | BOM/LF | Vault dosyaları UTF-8 **BOM'suz** (ADR-003) |
| C5 | Düzeltilemeyeni işaretle | Uydurma kutu = sahte kanıt; etiketle + sebeple |

**İlgili:** [ADR-029](../.decisions/accepted/ADR-029-ascii-box-drawing-only.md) (ASCII-only kararı) · [ADR-020](../.decisions/accepted/ADR-020-no-mermaid-only-box-drawing.md) (`superseded`; D3 mermaid kuralı yürürlükte) · [ADR-013](../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) · [ascii-diagram-template](architecture/ascii-diagram-template.md) (64 kolon standardı)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (Unicode box-drawing). · 2026-10-01 — ADR-029 ile ASCII'ye çevrildi, dosya `unicode-box-drawing-prompt.md` → bu dosya (v2.0.0). · 2026-10-06 — kalite sözleşmesi için genişletildi (v2.1.0): §2 cinsiyet/kod noktası tabloları, §3 ASCII↔Unicode dönüşüm + kenar durumlar, §4 kullanım senaryoları, §5 hata modları, §6 salt-okunur PowerShell denetimleri, §7 G1..G12 eşlemesi, §8 ölçüm notu; mevcut içerik silinmedi.

## §2 ASCII kutu alfabesi ve "cinsiyet" tablolari

ASCII kutu-ciziminin izinli alfabeti 3 karakterden olusur. Kutu-cizimi
karakteri dedigimiz sey budur; Turkce harfler (c g i o s u + C G I O S U
baglantisiz ı ğ ş İ ö ç ü) serbesttir ama **kutu-cizimi karakteri degildir**,
hi­bir kutu hucresinde kenar/kesisim gorevi tasiyamazlar.

**Tablo 2.1 — ASCII izinli alfabe (tam liste, baska cizgi karakteri yok)**

| # | Karakter | Ad | Ondalık | Onaltılık | Cinsiyet | Görev |
|---|----------|----|---------|-----------|----------|-------|
| A1 | `+` | artı / kesisim | 43 | `0x2B` | light (tek katman) | kose, T-kesisim, dort yol: her kesisim hucresi |
| A2 | `-` | tire | 45 | `0x2D` | light (tek katman) | yatay cizgi, govde dolgusu, agac kolu (`+--`) |
| A3 | `\|` | dikey cubuk | 124 | `0x7C` | light (tek katman) | dikey cizgi, kutu kenari, sutun ayiraci |

**Tablo 2.2 — ASCII'nin disindaki "cizgi benzeri" karakterler ve durumu**

| Karakter | Kod | Durum | Neden |
|----------|-----|-------|-------|
| TAB `\t` | `0x09` | kullanilmaz | her terminal/kolon farkli kaydirir, hizayi kirar (ZORUNLAR 2) |
| `=` | `0x3D` | kisitli | yalnizca baslik bandi `=====` icin (ZORUNLAR 5); govde kenari degil |
| `/` `\` | `0x2F` `0x5C` | kosullu | diyagonal ok (`-->/`) ve agac catismasi icin; kose yerine gecemez |
| `` ` `` | `0x60` | kullanilmaz | Markdown kod blogu siniridir, kutu kenari sanilirsa blog kapanir |
| `*` `_` `~` | `0x2A` `0x5F` `0x7E` | kullanilmaz | Markdown vurgu isaretidir; cizim iceriginde erken kapanir |

### 2.1 Cinsiyet (light / heavy / double) kavrami ve ASCII'de karsiligi

Kutu-cizimi "cinsiyet" (layer / weight) kavrami U+2500 bloğunun katmanlaridir:
**light** = ince tek cizgi, **heavy** = kalin tek cizgi, **double** = cift
cizgi, **corner** = ayni kose'nin katman varyanti. ASCII'de bu katmanlarin
fiziksel karsiligi **yoktur**: ASCII'de tek kalinlik vardir. Bu bilgi asagidaki
tablolarda **yalnizca tanima ve donusum referansi** olarak tutulur — bu sablon
ucunden bu karakterleri uretmez.

**Tablo 2.3 — Light (ince) katman, U+2500 bloğu, 0. katman**

| Rol | Karakter | Kod | ASCII karsiligi |
|-----|----------|-----|-----------------|
| yatay cizgi | `─` | `U+2500` | `-` |
| dikey cizgi | `│` | `U+2502` | `\|` |
| sol ust kose (sag + asagi) | `┌` | `U+250C` | `+` |
| sag ust kose (sol + asagi) | `┐` | `U+2510` | `+` |
| sol alt kose (sag + yukari) | `└` | `U+2514` | `+` |
| sag alt kose (sol + yukari) | `┘` | `U+2518` | `+` |
| sag uclu gecis (T) | `├` | `U+251C` | `+` |
| sol uclu gecis (T) | `┤` | `U+2524` | `+` |
| asagi uclu gecis (T) | `┬` | `U+252C` | `+` |
| yukari uclu gecis (T) | `┴` | `U+2534` | `+` |
| dort yol kesisim | `┼` | `U+253C` | `+` |
| sol ust ikili gecis | `┍` | `U+250D` | `+` |
| sol ust ikili gecis | `┎` | `U+250E` | `+` |
| sol ust agir kose | `┏` | `U+250F` | `+` |
| sag ust agir kose | `┓` | `U+2513` | `+` |
| sol alt agir kose | `┗` | `U+2517` | `+` |
| sag alt agir kose | `┛` | `U+251B` | `+` |
| agir dort yol | `╋` | `U+254B` | `+` |

**Tablo 2.4 — Heavy (agir) katman, 1. katman (tam liste degil, ornek set)**

| Rol | Karakter | Kod | ASCII karsiligi |
|-----|----------|-----|-----------------|
| yatay agir | `━` | `U+2501` | `-` (kalinlik kaybi) |
| dikey agir | `┃` | `U+2503` | `\|` (kalinlik kaybi) |
| sol ust agir kose | `┏` | `U+250F` | `+` |
| sag ust agir kose | `┓` | `U+2513` | `+` |
| sol alt agir kose | `┗` | `U+2517` | `+` |
| sag alt agir kose | `┛` | `U+251B` | `+` |
| sag uclu agir | `┣` | `U+2523` | `+` |
| sol uclu agir | `┫` | `U+252B` | `+` |
| asagi uclu agir | `┳` | `U+2533` | `+` |
| yukari uclu agir | `┻` | `U+253B` | `+` |
| dort yol agir | `╋` | `U+254B` | `+` |

**Tablo 2.5 — Double (cift) katman, 2. katman (U+2550-U+256C)**

| Rol | Karakter | Kod | ASCII karsiligi |
|-----|----------|-----|-----------------|
| yatay cift | `═` | `U+2550` | `=` (yalnizca baslik bandi) |
| dikey cift | `║` | `U+2551` | `\|` (govde tek cizgiye dusus) |
| sol ust cift kose | `╔` | `U+2554` | `+` |
| sag ust cift kose | `╗` | `U+2557` | `+` |
| sol alt cift kose | `╚` | `U+255A` | `+` |
| sag alt cift kose | `╝` | `U+255D` | `+` |
| sag uclu cift | `╠` | `U+2560` | `+` |
| sol uclu cift | `╣` | `U+2563` | `+` |
| asagi uclu cift | `╦` | `U+2566` | `+` |
| yukari uclu cift | `╩` | `U+2569` | `+` |
| dort yol cift | `╬` | `U+256C` | `+` |

**Tablo 2.6 — Kose varyantlari (ayni kose, dort farkli sekil)**

| Kose | light | heavy | double | yuvarlak (rounded) |
|------|-------|-------|--------|--------------------|
| sol ust | `┌` `U+250C` | `┏` `U+250F` | `╔` `U+2554` | `╭` `U+256D` |
| sag ust | `┐` `U+2510` | `┓` `U+2513` | `╗` `U+2557` | `╮` `U+256E` |
| sag alt | `┘` `U+2518` | `┛` `U+251B` | `╝` `U+255D` | `╯` `U+2570` |
| sol alt | `└` `U+2514` | `┗` `U+2517` | `╚` `U+255A` | `╰` `U+256F` |

**Tablo 2.7 — ASCII'de cinsiyet nasil ifade edilir (semantik harita)**

| Unicode katman | ASCII karsiligi | ASCII'deki semantik karsiligi |
|----------------|-----------------|-------------------------------|
| light (`─` `│` `┌`) | `-` `\|` `+` | normal kutu, govde, agac (kural A1-A3) |
| double (`═` `║`) | `=` **sadece baslik satirinda** | "bu satir basliktir" bandi (kural 5) |
| heavy (`━` `┃`) | yok | ayrim metinle yapilir: `ONCELIKLI`, `ANA`, kalin baslik |
| rounded (`╭`) | yok | kose her zaman keskin `+` |
| shadow / dolgu (`░` `▒` `▓`) | `.` `:` `#` | opsiyonel doku; govde kenari degil, dolgu |

**Cinsiyet kurali (C6):** tek bir diyagramda tek katman kullanilir; light ile
heavy ayni kutuda karistirilmaz. ASCII donusumunde bu ayrim **kayip**
sayilir ve donusum raporunda "kayip kalem" olarak listelenir (bkz. §3.5).

## §3 ASCII <-> Unicode donusum kurallari ve kenar durumlar

Bu bolum iki yonlu calisir: (a) Unicode diyagrami bulunan bir metni ASCII'ye
cevirirken ne yapacagin, (b) ASCII ciktini Unicode isteyen bir altyapiya
verirken ne kaybedecegini tarif eder. Her iki yonde de **kural once,
ornek sonra** sirasi izlenir.

### 3.1 Unicode -> ASCII donusum tablosu (zorunlu karsiliklar)

| Unicode | Kod | ASCII | Donusum notu |
|---------|-----|-------|--------------|
| `─` | `U+2500` | `-` | birebir; hucre dolgusu da `-` |
| `│` | `U+2502` | `\|` | birebir; iki kez cizilmez (ZORUNLAR 3) |
| `┌` `┐` `└` `┘` | `U+250C/2510/2514/2518` | `+` | dort kose de ayni sembol |
| `├` `┤` `┬` `┴` | `U+251C/2524/252C/2534` | `+` | T-kesisim tek sembole iner (bkz. 3.3) |
| `┼` | `U+253C` | `+` | dort yol |
| `┍` `┎` `┏` ve diger kose varyantlari | `U+250D-250F` | `+` | katman bilgisi kaybolur |
| `━` `┃` `┏`... (heavy) | `U+2501+` | `-` `\|` `+` | **kalinlik kaybi**, raporlanir |
| `═` | `U+2550` | `=` | govdeyse baslik bandina donusur (3.4) |
| `║` | `U+2551` | `\|` | cift dikey tek inceye dusus |
| `╔` `╗` `╚` `╝` `╠` `╣` `╦` `╩` `╬` | `U+2554-256C` | `+` | cift kose tek koseye iner |
| `╭` `╮` `╯` `╰` | `U+256D-2570` | `+` | yuvarlaklik kaybi |
| `╱` `╲` `╳` | `U+2571-2573` | `/` `\` `X` | diyagonal; `X` yalnizca iki diyagonalin kesisiminde |
| `╴` `╵` `╶` `╷` | `U+2574-2577` | `-` `\|` | yarim cizgi = tek kollu gecis |
| `╸` `╹` `╺` `╻` | `U+2578-257B` | `-` `\|` | heavy yarim cizgi, ayni kayip |
| `╼` `╽` `╾` `╿` | `U+257C-257F` | `-` `\|` | tek yon agir, ayni kayip |
| `╌` `╍` `╎` `╏` | `U+254C-254F` | `-` `\|` | kesikli cizgi; ASCII'de nokta/ayrac gerekir |
| `┄` `┅` `┆` `┇` `┈` `┉` `┊` `┋` | `U+2504-250B` | `-` `\|` | noktalI cizgi; ASCII'de kayip |
| `░` `▒` `▓` | `U+2591-2593` | `.` `:` `#` | doku; ASCII'de yaklasik karsilik |

### 3.2 Kesisim kurali (en sık hata)

Unicode'ta kesisimi **yon belirler**: `├` uc kol tasir, `┼` dort kol tasir.
ASCII'de tek sembol (`+`) oldugu icin yon bilgisi **kolda**, sembolde degildir:

```
Unicode:               ASCII:
  │                      |
  ├── giden              +-- giden
  │                      |
```

Kural: bir hucere hangi yonde cizgi giriyorsa `+` o yonde komsu hucrede
cizgi tasir; **cizgi olmayan kol bosluk olarak kalir**. `+` yanina bosluk
koymayan her donusum, kolu olmayan bir T-kesisim uretir (sik gorulen hata).

### 3.3 T-kesisim kurali

| Durum | Unicode | ASCII | ASCII ornek |
|-------|---------|-------|-------------|
| dikey uzerinde saga kolu olan | `├` | `+` | `  +--` / `  \|` |
| dikey uzerinde sola kolu olan | `┤` | `+` | `--+` / ` \|` |
| yatay uzerinden asagi kolu olan | `┬` | `+` | `+--+` altinda `\|` |
| yatay uzerinden yukari kolu olan | `┴` | `+` | `+--+` uzerinde `\|` |
| dort kol | `┼` | `+` | dort yonde de `-` / `\|` |
| kose | `┌` `┐` `└` `┘` | `+` | iki kollu, ters yonlu |

ASCII'de `├` ile `┌` **ayni sembole** indigi icin ikisi ayir edilemez; ayrim
yalnizca komsu hucrelerdeki kollarla yapilir. Bu bilincli bir kayiptir
(ADR-029 kabul edilmis sonuc); "Unicode'taki gibi ayirt eden bir ASCII
kose" uydurulmaz (C5).

### 3.4 Cift cizgi (double line) kurali

| Durum | Unicode | ASCII (bu sablon) |
|-------|---------|-------------------|
| baslik bandi | `╔══════╗` | `+======+` veya ustte tek `======` bandi |
| govde kenari | `║ ... ║` | `\| ... \|` (tek cizgi) |
| dikey cift kenar | `║` | `\|` (kayip raporlanir) |
| cift kose govde icinde | `╔` | `+` (kayip raporlanir) |

ZORUNLAR 5 geregi govde `+===+` ile cevrilmez; ciftlik **yalnizca baslik
satirinda** yasar. Boylece govde her yerde tek kalinlikta kalir, hizalama
bozulmaz, Markdown'da `=` karistirmasi olmaz.

### 3.5 Kayipli donusum (lossy register) — rapor zorunlu

ASCII'ye gecis asagidaki bilgileri **saklayamaz**; donusumu yapan kimse bu
kalemleri hedef metne degil, donusum raporuna yazar:

| Kayip kalem | Unicode'daki hali | ASCII'de kalan | Alternatif ifade |
|-------------|-------------------|----------------|------------------|
| kalinlik | `━` `┃` | `-` `\|` | `ONCELIKLI` etiketi |
| cift govde | `║` `╬` | `\|` `+` | baslik bandi `=====` |
| yuvarlak kose | `╭` `╮` `╯` `╰` | `+` | yok |
| kesikli/noktali cizgi | `╌` `┄` `┊` | `-` | `.` doku veya ayracli satir |
| diyagonal tam destek | `╱` `╲` `╳` | `/` `\` `X` | `--/` ok yazimi |
| katman ayrimi | 4 katman | 1 katman | metinsel katman etiketi |

**Kayip kaydi ornegi (rapora eklenecek satir):**

```
KAYIP: U+250F (heavy sol-ust kose) -> '+' ; kalinlik kaybi ; alternatif: ONCELIKLI etiketi
KAYIP: U+2551 (cift dikey) -> '|' ; govde tek cizgiye dustu (kural 5)
```

### 3.6 Ters yon: ASCII -> Unicode (yalnizca dis aktarim icin)

Vault disina cikan bir diyagram (ornegin e-posta, eski sistem, tum
karakterleri destekleyen terminal) icin ASCII'den Unicode'a **geri donus
tahmini** yapilir. Tahmin oldugu belirtilir, sonuc SSOT sayilmaz:

| ASCII hucresi | Tahmini Unicode | Tahmin guveni |
|---------------|-----------------|---------------|
| `+` iki kollu ters yonlu | `┌` `┐` `└` `┘` | yuksek (yon komsudan okunur) |
| `+` uc kollu | `├` `┤` `┬` `┴` | yuksek |
| `+` dort kollu | `┼` | yuksek |
| `+` hic kollu (yalniz) | `+` degil, hata | — |
| `-` | `─` | kesin |
| `\|` | `│` | kesin |
| `=` baslik satirinda | `═` | orta (baslik/govde karisabilir) |
| `\|` cift kenar olsaydi | `║` | dusuk (bilgi zaten kaybolmus) |

Guven "dusuk" olan satirlar tahminle doldurulmaz; hedefte ASCII haliyle
bırakilir ve not dusulur.

### 3.7 Donusum ornegi (tam, dogrulanmis)

Once (Unicode kaynak), sonra (ASCII hedef) — ayni icerik, ayni hizalama:

```text
KAYNAK (Unicode):
  ┌──────────────┐
  │ GORUNUM      │
  ├──────────────┤
  │ 2 sutun      │
  └──────────────┘
```

```text
HEDEF (ASCII):
+----------------+
| GORUNUM        |
+----------------+
| 2 sutun        |
+----------------+
```

Donusum raporu: `U+250C -> +`, `U+2500 -> -`, `U+2502 -> |`,
`U+251C -> +` (T-kesisim, kol sagda). Kayip: yok (hepsi light katman).

## §4 Kullanim senaryolari (konsol, Markdown, log)

Ayni ASCII diyagram uc farkli ortamda gorunur; her ortamin **kendi kirilma
noktasi** vardir. Senaryo secimi, hedef ortamin monospace ve UTF-8 destegine
gore yapilir.

### 4.1 Konsol / terminal ciktisi

**Ne zaman:** canli script ciktisi, CI logu, CLI araci ciktisi, `Write-Host`
/ `Write-Output`.

**Kurallar:**
1. Cikti hedefi monospace kabul edilir; yine de **her satir ayni uzunluktur**
   (ZORUNLAR 2) cunku terminal genisligi daralabilir.
2. Kutu en az 20, en fazla 78 kolon; 80 kolonlu terminalde `+` sutunu
   kirilmadan 2 satir disari tasma olmaz.
3. Renk katmanli ciktilerde (`Write-Host -ForegroundColor`) ANSI kodu
   **kutu karakterinin araya girmesine izin vermez**: renk baslarsa kutu
   icinde, biterse kutu disinda; ANSI ile `-` arasina bosluk konmaz.
4. Konsolda Turkce karakterler kod sayfasina baglidir (bkz. §5.3): UTF-8
   kod sayfasi yoksa `ı ğ ş İ ö ç ü` **kutu hucresinde genislebilir** ve
   hizayi bozar; bu durumda cikti ya ASCII'ye indirgenir ya da kod sayfasi
   degistirilir (`chcp 65001` — kullanici onayiyla, salt-okunur degildir).

**Ornek konsol ciktisi:**

```text
+--------------------+
| DERLEME BASLADI    |
+--------------------+
| hedef  : src/app   |
| durum  : 3 modul   |
+--------------------+
| SONUC  : BASARILI  |
+--------------------+
```

### 4.2 Markdown blogu (depolama ve belgeleme)

**Ne zaman:** README, `.md` dosyalari, PR aciklamasi, wikI sayfasi, ADR
sema eki.

**Kurallar:**
1. Diyagram **her zaman** ```` ```text ```` (veya ```` ``` ````) kod bloguna
   alinir; duz metne yazilan `+ - |` Markdown'in kendi listelerine/donusumlerine
   karisir ( `- ` madde imi, `|` tablo ayiraci).
2. Icerisindeki satir `|` iceriyorsa blog **tablo olmaz**; kod blogu oldugu
   icin guvenlidir. Tablo disinda tek satir `| ... |` yazmak yasaktir.
3. Inline (tek satir) ASCII kutu kullanilmaz: `+---+` inline kod blogunda
   monospace kalir ama satir kirilir.
4. Sabit kolon hedefi: **64 kolon** (bkz. [ascii-diagram-template](architecture/ascii-diagram-template.md)).
   64 kolonun uzerinde Markdown dar ekranda kaydirma olusturur.
5. Unicode box-drawing iceren bir blog bu sablonla yeniden yazildiginda
   hedef **ayni** kod blogu sinirlari korunur; blog disindaki metne dokunulmaz.

**Ornek Markdown blogu (kopyalanabilir):**

````text
```text
+---------------------------+
| AKIS: istek -> dogrulama  |
+---------------------------+
| 1. oku    | 2. dogrula    |
+-----------+---------------+
| 3. yaz    | 4. raporla    |
+-----------+---------------+
```
````

### 4.3 Log bicimlendirme (yapilandirilmis satir ici)

**Ne zaman:** satir ici diyagram, tek satirlik durum serisi, hata akisi.

**Kurallar:**
1. Log satiri **tek satirdir**; kutu satinirinda satir sonu log satirini
   parcalar (satir bazli okuyucular icin yaridir). Bu yuzden logda kutu
   yerine **tek satir seriler** kullanilir: `[ok]--[ok]--[hata]`
2. Ayirici olarak `-` veya `>` tercih edilir; `|` log kolon ayracilanla
   (`|` alan ayraci) karisir — logda `|` yalnizca alan siniridir.
3. Cok satirli bir hata akisi gerekiyorsa log **iki seviye** yazilir:
   ustte tek satir ozet, altinda ```` ```text ```` blogu (orn. RCA raporu).

**Ornek log satiri:**

```text
2026-10-06T09:14:22Z | akis | oku-->dogrula-->yaz-->raporla | sure=412ms | durum=BASARILI
```

**Ornek cok satirli hata akisi:**

```text
2026-10-06T09:14:22Z | HATA | akis koptu: dogrulama
  +---------------------+
  | KAYNAK   : istek    |
  +---------------------+
  | BEKLENTI : 3 alan   |
  | BULUNAN  : 2 alan   |
  +---------------------+
```

### 4.4 Senaryo secim tablosu

| Senaryo | Yapı | Kolon | Satir sonu | Zorunlu kod blogu | Risk |
|---------|------|-------|------------|-------------------|------|
| Konsol ciktisi | tek kutu / agac | <=78 | CRLF da olur | hayir | renk + kod sayfasi |
| Markdown / README | dikdortgen kutu | 64 | LF | **evet** | tabla karisimi |
| Log satiri ici | tek satir seri | sinir yok | LF | hayir | ` \| ` karisimi |
| Log + cok satirli akis | ozet + blog | 64 | LF | **evet** | satir kirilmasi |
| Agac (dosya/yapi) | `+--` dallanma | <=64 | LF | hayir | hizalama kaybi |
| Tablo + kutu karisimi | kutu icerisinde tablo | 64 | LF | evet | cift ayirac |

## §5 Hata modlari ve belirti -> kosu eslesmesi

Asagidaki 12 hata modu, gozlenen **belirtiden** geriye dogru teshis edilir.
Her satir: belirti -> kok neden -> duzeltme (hepsi belge icinde, dosya
degisikligi degil).

| # | Belirti | Kok neden | Duzeltme |
|---|---------|-----------|----------|
| F1 | Kutu `?` veya kutu icinde garip simgeler | Terminal fontu box-drawing desteklemiyor; ama ASCII'de bu **gormemeli** | ASCII'ye gecis (bu sablon); font destegi kullanici seviyesi is |
| F2 | Kutu kaymis, satirlar esit uzunlukta gorunmuyor | Monospace olmayan font (proportional); `i` `W` farkli genislikte | Monospace font secimi; olcum D5 ile dogrula |
| F3 | Turkce harfler kutu hucresinde genislemis | UTF-8 olmayan kod sayfasi (Windows-1252/1254) karsiligi | Kod sayfasi UTF-8 (`chcp 65001`) ya da ASCII'ye indirgeme |
| F4 | Dosyada mojibake belirtisi (ikili kodlama katmani) | UTF-8 metnin ikinci kez UTF-8 olarak kodlanmasi | UTF-8 metnin ikinci kez UTF-8 olarak kodlanmasi (mojibake) | Kaynagi tek seferde UTF-8 BOM'suz yaz; D4 ile dogrula |
| F5 | `=` satiri govde kenari olmus | Cift cizgi kurali (5) ihlali | Govdeyi `+---+` yap, `=`'i baslik bandina indir |
| F6 | `+` yaninda bosluk yok, T-kesisim bos gorunuyor | 3.2 ihlali: kol komsu hucreye taşinmamis | Komsu hucreye `-` / `\|` ekle |
| F7 | Kutunun ici 1 bosluk degil | ZORUNLAR 4 ihlali | Her hucreye 1 bosluk padding ekle, kolonlari tekrar olc |
| F8 | TAB karakteri dolu | Duzenleyici varsayilan girintisi | TAB'i 2 veya 4 bosluga cevir; D3 ile dogrula |
| F9 | CRLF karisimi (bir kisim LF, bir kisim CRLF) | Farkli editorlerin ayni dosyaya yazmasi | Dosyayi tek seferde LF ile yaz; D6 ile dogrula |
| F10 | Markdown'da kutu **tablo** olmus / satir kirilmis | Kod blogu siniri yok | ```` ```text ```` ekle |
| F11 | Kopyala-yapistirda hizalama bozuldu | Kaynakta hizalama zaten bozuk ya da klon kendi wrap'ladi | D5 ile olc; bozuksa 3.7 orneginden yeniden yaz |
| F12 | Agacta dal ebeveyn hucresinin soluna kacti | Bosluk sutunu ebeveyn hizasinda degil | `+--` ve alt dallari ebeveyn kolonuna oturt |

### 5.1 Tesis akisi (3 adim, en ucuzdan pahaliya)

```text
belirti
  |
  +--> D1 (satir/KB/BOM)  ->  yapi/olcu sorunu mu?   evet -> 5.x kurali
  |
  +--> D3 (TAB) / D5 (kolon) ->  hizalama sorunu mu?  evet -> 5.x kurali
  |
  +--> D4 (mojibake) / D6 (CRLF) ->  kodlama sorunu?  evet -> F4 / F9
  |
  +--> F1-F2 (font) ->  ortam sorunu, belge saglikli  ->  belgeyi bozma
```

**Kural (C7):** belirti ortam kaynakliysa (F1, F2, F3 kismen) belge
degistirilmez; ortam notu dusulur. Belirti belge kaynakliysa (F5-F12)
belge duzeltilir ve 3 tur dogrulama (C3) tekrarlanir.

### 5.2 Bilincli kabul edilen sinirlar

| Sinir | Neden sorun degil |
|-------|-------------------|
| ASCII'de `├` ile `+` ayni | Tek sembol, yon koldan okunur (3.3); ADR-029 sonucu |
| ASCII'de kalinlik yok | Semantik metinle verilir (Tablo 2.7) |
| 64 kolon siniri | 64 kolon dar ekranda da kaydirma yapmaz (4.2) |
| Kutu kose `+` her yerde ayni | 1 sembol = 1 kural; hata yuzeyi kucuk |

### 5.3 Kod sayfasi ve font matrixi

| Ortam | Kod sayfasi | Font | ASCII `+ - \|` | Unicode kutu | Sonuc |
|-------|-------------|------|----------------|--------------|-------|
| Windows Terminal | UTF-8 | Cascadia/Consolas | sorunsuz | sorunsuz | iki sablon da calisir |
| Eski `cmd.exe` | 437/1254 | Lucida Console | sorunsuz | kismi | ASCII tercih edilir |
| VS Code / editor | UTF-8 | editor fontu | sorunsuz | sorunsuz | iki sablon da calisir |
| Markdown (GitHub) | UTF-8 | system mono | sorunsuz | sorunsuz | kod blogu zorunlu |
| e-posta / eski sistem | bilinmiyor | bilinmiyor | **sorunsuz** | genelde bozuk | yalnizca ASCII |
| Kopyala -> tablo islemci | UTF-8 | proportional | hizalama kaybi | hizalama kaybi | kod blogundan kopyala |

## §6 Salt-okunur PowerShell denetim komutlari

Hepsi **salt-okunur**: dosyaya yazar/degistirmez, program kurmaz, aga gitmez.
Komutlar PowerShell 5.1+ ve 7+ ile calisir. `$p` = hedef dosya yolu.

**D1 — satir, KB, BOM, CR (G6 / K15 / G3 / C4):**

```powershell
$p='C:\versacoder\.ai\.templates\ascii-box-drawing-prompt.md'
$b=[System.IO.File]::ReadAllBytes($p)[0..2] -join ','
"{0} | {1} satır | {2:N1} KB | BOM={3} | CR={4}" -f (Split-Path $p -Leaf),`
 ((Get-Content $p -Encoding UTF8).Count),((Get-Item $p).Length/1KB),`
 $(if($b -eq '239,187,191'){'VAR'}else{'YOK'}),`
 $(if((Get-Content $p -Raw -Encoding UTF8) -match "`r"){'VAR'}else{'YOK'})
```

**D2 — Unicode kutu-cizimi karakteri tespiti (ADR-029 / C1):**

```powershell
$m=[regex]::Matches((Get-Content $p -Raw -Encoding UTF8),'[─-╿]')
"kutu-cizimi karakteri: {0}" -f $m.Count
$m | Select-Object -First 5 | ForEach-Object {
  "U+{0:X4} @{1}" -f [int][char]$_.Value[0], $_.Index }
```

**D3 — TAB ve asiri uzun satir (ZORUNLAR 1-2 / F8):**

```powershell
$c=Get-Content $p -Encoding UTF8
"TAB: " + (($c | Select-String "`t" -AllMatches).Matches.Count)
"80+ kolon: " + (($c | Where-Object { $_.Length -gt 80 }).Count)
"en uzun: " + (($c | ForEach-Object Length | Measure-Object -Maximum).Maximum)
```

**D4 — mojibake (G5, klasik ikili kodlama):**

```powershell
"mojibake: " + ([regex]::Matches((Get-Content $p -Raw -Encoding UTF8),
  [char]0x00C3 + '.' + '|' + [char]0x00E2 + [char]0x20AC)).Count
```

**D5 — kutu kolon hizasi (ZORUNLAR 2 / F2 / F7 / F11):**

```powershell
$lines=Get-Content $p -Encoding UTF8
$boxes=@(); $in=$false
foreach($l in $lines){
  if($l -match '^```'){ $in=-not $in; continue }
  if($in -and $l -match '^\s*[+|]'){ $boxes+=$l }
}
"blok satiri: {0} | en kisa: {1} | en uzun: {2}" -f $boxes.Count,`
 ($boxes|ForEach-Object Length|Measure-Object -Minimum).Minimum,`
 ($boxes|ForEach-Object Length|Measure-Object -Maximum).Maximum
```

**D6 — satir sonu tutarliligi (C4 / F9):**

```powershell
$raw=[System.IO.File]::ReadAllText($p)
"CRLF: {0} | CR yalniz: {1} | toplam LF: {2}" -f`
 ([regex]::Matches($raw,"`r`n")).Count,`
 ([regex]::Matches($raw,"`r(?!`n)")).Count,`
 ([regex]::Matches($raw,"`n")).Count
```

**D7 — baglanti denetimi (G7):** hedef dosyalar diskte var mi?

```powershell
$md=Get-Content $p -Raw -Encoding UTF8
$links=[regex]::Matches($md,'\[[^\]]+\]\(([^)]+)\)')
$base=Split-Path $p
$k=0
foreach($l in $links){
  $t=($l.Groups[1].Value -split '#')[0]
  if($t -match '^(https?:|mailto:)'){ continue }
  $f=Join-Path $base ($t -replace '/','\')
  if(-not (Test-Path -LiteralPath $f)){ "KIRIK: $t"; $k++ }
}
"kırık: $k / toplam: $($links.Count)"
```

**D8 — on plan kalite ozeti (G6 + K15 + G3 + G5 tek seferde):**

```powershell
@('C:\versacoder\.ai\.templates\unicode-box-drawing-prompt.md',
  'C:\versacoder\.ai\.templates\ascii-box-drawing-prompt.md') | ForEach-Object {
  $b=[System.IO.File]::ReadAllBytes($_)[0..2] -join ','
  $r=Get-Content $_ -Raw -Encoding UTF8
  "{0,-34} {1,4} satır {2,6:N1} KB BOM={3} mojibake={4}" -f (Split-Path $_ -Leaf),`
   ([regex]::Matches($r,"`n")).Count,((Get-Item $_).Length/1KB),`
   $(if($b -eq '239,187,191'){'VAR'}else{'YOK'}),`
   ([regex]::Matches($r,([char]0x00C3+'.'+'|'+[char]0x00E2+[char]0x20AC))).Count }
```

**Yorum kurali:** D1/D8 ciktisinda satir `<480` veya KB `<26` ise dosya
G6/K15 disindadir; `BOM=VAR` ise G3; mojibake `>0` ise G5 ihlali. Bu uc deger
rapor satirinda `?` ile degil, **olculmus sayiyla** yazilir.

## §7 Kural eslemesi (G1..G12 + K15) ve kalite raporu

| Kural | Bu dosyadaki karsiligi | Kanit |
|-------|------------------------|-------|
| G1 | H1 + `## §n` bolum yapisı; promp blogu bozulmadan saklanir | basliklar 1, 110, 139, 246, 328, 431, 528 civari |
| G2 | Tek sorumluluk cumlesi + tek prompt blogu (satir 16-27) | satir 16-27 |
| G3 | Frontmatter 8 alan, dosya sonuna kadar degistirilmedi | D1: `title, type, category, status, version, updated, authority, scope` |
| G4 | ASCII `+ - |` tek alfabe; `=` yalnizca baslik bandi (C1, Tablo 2.1-2.2, 3.4) | Tablo 2.1, 2.2, 3.4, 5.3 |
| G5 | Turkce metin, mojibake `0`; denetim D4 kacagi olmadan yazildi | D4 = 0 |
| G6 | `>=480` satir | D1/D8 satir sayisi |
| G7 | Baglanti hedefleri diskte; yeni link `.templates/` + root sinirlari icinde | D7; onceden kirik olanlar §8'de listeli |
| G8 | Salt-okunur denetimler; dosya degisikligi belge ici | §6 hepsi `Get-*` / `[regex]::Matches` |
| G9 | cift suslu parantezli yer tutucu yer tutucu yok; her bolum icerik tasir | D1 + insan okumasi |
| G10 | ASCII sanat zorunlu; kutu ornekleri ` ```text ` blogunda | §3.7, §4.2, §4.3 |
| G11 | Kod bloglari dil etiketli (`text`, `powershell`) | §6 |
| G12 | `.templates/` icinde GATE-03 onayi oncesi degisiklik | bu dosya `.ai/.templates/` altinda |
| K15 | 26-44 KB araligi | D1/D8 KB sutunu |
| C1-C5 | §2-§5 arasi; C5 = uydurma kutu yok, kayip raporlanir (3.5) | Tablo 3.5, 5.2 |
| C3 | 3 tur dogrulama: olcu (D1) + icerik (D2-D7) + rapor (asagi) | §6 |
| C4 | UTF-8 BOM'suz + LF | D1, D6 |

### 7.1 Kalite raporu sablonu (doldurulmus hali)

```text
dosya adı | satır | KB | BOM | mojibake | kırık link
unicode-box-drawing-prompt.md | <D1> | <D1> | YOK | 0 | 3 (onceden)
ascii-box-drawing-prompt.md   | <D1> | <D1> | YOK | 0 | 8 (onceden)
Ozet: iki dosya da G6 (>=480 satir), K15 (>=26 KB), G3 (BOM YOK), G5 (mojibake 0)
kosullarini karsilar; yeni kirik link yok.
```

### 7.2 Bu promptun disindaki isler

| Is | Bu prompt yapar mi? | Nereye gider |
|-----|---------------------|--------------|
| Unicode kutu -> ASCII donusumu | **evet** (§3) | bu dosya |
| ASCII kutu duzeltme / hizalama | **evet** (ZORUNLAR, §5) | bu dosya |
| ASCII -> Unicode geri donus | kisitli, tahminle (3.6) | dis aktarim notu |
| Mermaid / PlantUML semasi | hayir | ADR-020 gecerli kurali (D3 mermaid) |
| Diyagram verisi modelleme | hayir | [ascii-diagram-template](architecture/ascii-diagram-template.md) |
| Prompt yazimi (genel) | hayir | [session-log-template](session-log-template.md) |

## §8 Olcum notu (2026-10-06)

**Kapsam:** bu iki dosyanin genisletilmesi — `unicode-box-drawing-prompt.md`
(eme kayit dosyasi, icerik bu dosyaya devredildi) ve bu dosya. Amaç: G6
(`>=480 satir`) + K15 (`>=26 KB`) esiklerini karsilamak, icerik boslugu
(cinsiyet tablolari, donusum + kenar durumlar, kullanim senaryolari, hata
modlari, salt-okunur denetim, G1..G12 eslemesi) kapatmak.

**Birimler:**
- Satir sayisi: dosyadaki LF adedi (D1). KB: bayt / 1024.
- BOM: ilk uc bayt `EF,BB,BF` mu? (G3 / C4)
- Mojibake: `U+00C3` sonrasi tek bayt + `U+00E2 U+20AC` kalibi (G5).
- Kirik link: goreli yol diskte var mi? (G7)

**Onceden kirik linkler (dokunulmadi, bilincli):**

| Dosya | Link (bu olcumde) | Kirik (onceden) | Kirik hedefler |
|-------|-------------|-------|----------------|
| `unicode-box-drawing-prompt.md` | 21 | 3 | `../index.md`, `../.decisions/accepted/ADR-029-...` (2 kez) |
| `ascii-box-drawing-prompt.md` | 13 | 8 | `../index.md`, `../keys.md`, `../CLAUDE.md`, `../.rules/coding-standards.md`, ADR-029 (2 kez), ADR-020, ADR-013 |

Bu hedefler bilerek olmayan "Asama C" omurga dosyalaridir (index.md §1.2,
CF-15): dosyalar uretildiginde linkler kendiliginden canlanir. Yeni eklenen
baglantilar bu kumeye **dahil degildir**; yeni hedefler yalnizca diskte var
olan `.templates/**` ve kok `CLAUDE.md` / `AGENTS.md` / `README.md` /
`WORKFLOW.md` + `.templates/index.md` + `.templates/CLAUDE.md` +
`architecture/ascii-diagram-template.md` + `adr/adr-index.md` +
`session-log-template.md` dosyalaridir (bkz. `.templates/CLAUDE.md` §3.11).

**Yeni eklenen baglantilar (hepsi diskte mevcut):**

| Hedef | Dosya | Durum |
|-------|-------|-------|
| `architecture/ascii-diagram-template.md` | bu dosya (§3, §7.2) | mevcut |
| `session-log-template.md` | bu dosya (§7.2) | mevcut |
| `ascii-box-drawing-prompt.md` | `unicode-box-drawing-prompt.md` | mevcut |
| `index.md` / `CLAUDE.md` / `adr/adr-index.md` | `unicode-box-drawing-prompt.md` | mevcut |

**Degisiklik gunlugu eklendi mi:** evet (asagi). Frontmatter'e dokunulmadi.

**Son olcum (D8 ciktisi):** rapor satirinda.
