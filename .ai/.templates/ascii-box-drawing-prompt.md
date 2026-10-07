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

**Zorunlu Bağlantılar:** [index](../index.md) · [keys](../governance/keys.md) · [CLAUDE](../claude.md) · [templates index](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cümlelik sorumluluk:** Bir belgedeki tüm diyagramları/şemaları, izinli
> **ASCII `+ - |` alfabesiyle** hizalı, kesişimi doğru, doğrulanmış biçimde
> yeniden yazdırmak için kopyala-yapıştır tek komut.

> **Kullanım:** `<HEDEF DOSYA>` yerine dosya yolunu koy, koda yapıştır, çalıştır.
> MODEL DEĞİŞTİRME: bu prompt yeni oturumda/tahminci ajan başında kullanılır.

> **ADR-029 (2026-10-01):** Unicode box-drawing (`U+2500-U+257F`) yasaktır;
> bu dosyanın eski sürümü (v1.0.0, `unicode-box-drawing-prompt.md`) emekliye
> ayrılmıştır. Kural kaynağı: [ADR-029](../.decisions/accepted/adr-029-ascii-box-drawing-only.md) · `CLAUDE.md` G4.

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

**İlgili:** [ADR-029](../.decisions/accepted/adr-029-ascii-box-drawing-only.md) (ASCII-only kararı) · [ADR-020](../.decisions/accepted/adr-020-no-mermaid-only-box-drawing.md) (`superseded`; D3 mermaid kuralı yürürlükte) · [ADR-013](../.decisions/accepted/adr-013-layered-architecture-52-nodes.md) · [ascii-diagram-template](architecture/ascii-diagram-template.md) (64 kolon standardı)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (Unicode box-drawing). · 2026-10-01 — ADR-029 ile ASCII'ye çevrildi, dosya `unicode-box-drawing-prompt.md` → bu dosya (v2.0.0).
