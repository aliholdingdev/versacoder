---
title: "pattern-matching - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "kalip esleme surumleri (C# 7-14) ve hangi kalip turunun hangi bacakta derlendigi"
---

# pattern-matching — Kalıp eşleme

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [records-and-immutability](records-and-immutability.md)

> **Tek cumlelik sorumluluk:** *Hangi kalip turu hangi bacakta derlenir ve `switch` neden expression olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kalip esleme **surum surum** gelisti: C# 7.3 yalniz `is`-kalip + sabit + bildirim (declaration) kalibi + tuple `switch` tanir; **`switch` expression ve property/positional kalip C# 8**, **`and`/`or`/`not` + iliskisel kalip C# 9**, **list kalibi C# 11** gerektirir — yani hepsi **yalniz `net10.0-windows` bacakta**. Ortak kodda kalip stratejisi basit olandir: **7.3'te calisan `is` + `switch` statement**, karmasik dallanma gerekirse dosya net10'a ozel ayrilir. Kalip esleme, `if`/`else` yığını yerine **tip ve deger bilgisini tek satirda** ifade eder ve `Result`/`ErrorCode` yonlendirmeleri icin en okunur yoldur.

---

## §1 Surum Haritasi (iki bacak)

| Kalip / yazim | Surum | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|---|
| `x is int i` (bildirim kalibi) | 7.0 | Var | Var |
| Sabit kalip: `case 1:` / `is 1` | 7.0 | Var | Var |
| Tuple kalip + `switch (a, b)` | 7.0 | Var | Var |
| Kalip `when` filtresi | 7.0 | Var | Var |
| **`switch` expression** | 8.0 | **Yok** | Var |
| **Property kalipi** (`is { Ad.Length: > 0 }`) | 8.0 | **Yok** | Var |
| **Positional kalip** (`is (var a, var b)`) | 8.0 | **Yok** | Var |
| **`and` / `or` / `not`** bilesik kalip | 9.0 | **Yok** | Var |
| Iliskisel kalip (`is > 0 and < 100`) | 9.0 | **Yok** | Var |
| Type kalibi `is int` (statement'da) | 9.0 | Kisli | Var |
| **List kalibi** `[1, 2, ..]` | 11.0 | **Yok** | Var |
| `field` anahtari ile kalip destegi | 14.0 | **Yok** | Var |

> **UYARI:** "Kalip guzeldir" diye ortak dosyaya C# 8+ yazmak, `net48` derlemesini kirmaktir (CS8xxx). Kalip secimi **surum haritasina gore** yapilir, tada gore degil.

## §2 Ne Zaman Kalip, Ne Zaman `if`

| Durum | Secim | Ornek |
|---|---|---|
| Tip + deger tek karar | `is` kalipi | `if (girdi is string bos && bos.Length == 0)` |
| 3+ dal, deger uzerinden | `switch` statement (7.3 uyumlu) | `ErrorCode` -> mesaj |
| Tek ifade, donus degeri | `switch` expression (**net10**) | `ErrorCode` -> ekran |
| Boyle de, su de | `and`/`or` (**net10**) | `is > 0 and < 100` |
| Boyle degil | `not` (**net10**) | `is not null` |
| Sadece null kontrol | `is null` / `?? ` | her iki bacakta |

## §3 Ornek

Iki bacakta derlenen yonlendirme (statement):

```csharp
internal string EkranMesaji(ErrorCode kod)
{
    switch (kod)
    {
        case ErrorCode.NotFound:
            return "kayit bulunamadi";
        case ErrorCode.Invalid:
            return "form hatali";
        case ErrorCode.Timeout:
            return "islem zaman asimina ugradi";
        case ErrorCode.None:
            return string.Empty;
        default:
            return "beklenmeyen hata";
    }
}
```

net10 bacakta ayni sey (expression + bilesik kalip):

```csharp
internal static string EkranMesaji(ErrorCode kod) => kod switch
{
    ErrorCode.NotFound => "kayit bulunamadi",
    ErrorCode.Invalid  => "form hatali",
    ErrorCode.Timeout  => "islem zaman asimina ugradi",
    ErrorCode.None     => string.Empty,
    _                  => "beklenmeyen hata",
};

internal static bool Gecerli(int x) => x is > 0 and < 100;   // C# 9
```

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `switch` expression'i ortak dosyaya yazmak | net48 derleme hatasi | §1 haritasi |
| 2 | `default`/`_` kolu unutulmak | Yeni `ErrorCode`'da kaba mesaj | Her zaman default |
| 3 | Kalip yerine `if/else` zinciriyle tip ayristirmak | Okunmaz dallanma | `is` kalipi |
| 4 | Kalip sirasini ters (genel once) | Ozel kol hic calismaz | Ozelden genise |
| 5 | `is null` diye NRT'yi cozmek sanmak | Annotation yok | [nullable-reference-types](nullable-reference-types.md) |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Kalip surumu | §1 satiri "Var" mu iki bacakta? |
| Kapsam testi | `dotnet test`: her `ErrorCode` kolu kapsandi |
| Build | `net48` + `net10.0-windows` 0 hata |
| Kardesler | [records-and-immutability](records-and-immutability.md) · [result-type-design](result-type-design.md) |
