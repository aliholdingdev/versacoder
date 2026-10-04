---
title: Assertion Kalitesi
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# Assertion Kalitesi

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Test Kurallari](../../.rules/testing-rules.md) | [Test Determinizmi](testing-determinism.md) | [C Sharp Index](index.md)

> **Tek cumlelik sorumluluk:** "Var mi" degil "neyin ne oldugunu" assert et; her zaman dogru gorunen assertion = test yok.

## §0 Ozet (Answer First)

Dort zayif assertion: **`Assert.NotNull` tek basina**, **`Assert.True(true)`**, **`Assert.Throws<Exception>` (genel tip)**, **`Count > 0`**. Guclu karsiligi: somut durum (`Assert.Equal(SessionState.Running, s.State)`), beklenen eleman sayisi, tip+mesaj (`Assert.Equal("VC-E-0003", ex.ErrorCode)`), kaynak durumu (DB satiri).

## §1 Karar Tablosu

| Iyi | Kötü |
| --- | --- |
| `Assert.Equal(SessionState.Running, s.State)` | `Assert.NotNull(s)` |
| `Assert.Equal("VC-E-0003", ex.ErrorCode)` | `Assert.Throws<Exception>(...)` |
| `Assert.Equal(3, rows.Count)` + elemanlar | `Assert.True(rows.Count > 0)` |
| `Assert.Contains(r => r.Id == id, rows)` | `Assert.True(rows.Any())` |
| DB'de kaynak durumu dogrulamasi | Yalniz donus degerine bakmak |
| `await Assert.ThrowsAsync<ToolTimeoutException>(...)` | `try/catch` + `Assert.True(false)` |

## §2 Iki Bacak

**Bacak 1 - Iddia cesitliligi:** Tek tip assertion yiginisi kapsam yanlisi gosterir. Deger, durum, sira, eleman sayisi, hata kodu ve kaynak durumu birlikte kullanildiginda test gercek davranisi kilitler.

**Bacak 2 - Kaynak durumu:** Donus degeri basarili olsa bile DB'de/sistemde durum degismemis olabilir. Nihai dogrulama kaynak uzerindedir (satir, dosya, olay).

## §3 Ornek

```csharp
// KOTU: her zaman gecer
var sonuc = await _service.BulAsync(7, ct);
Assert.True(sonuc.IsSuccess);

// IYI: ne oldugu belli
var sonuc = await _service.BulAsync(7, ct);
Assert.True(sonuc.IsSuccess);
Assert.Equal("ali", sonuc.Value!.Ad);
Assert.Equal(3, satirlar.Count);                  // beklenen sayi, "> 0" degil

// IYI: hata tipi + kod, genel tip degil
var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
    () => _service.BulAsync(999, ct));
Assert.Equal("VC-E-0003", ex.Message);
```

## §4 Hata Modlari

1. `Assert.NotNull` yalniz: kod null dondurmeyi biraksa test yine gecer; davranis degisir, test kirmizi olmaz.
2. `Assert.Throws<Exception>`: yanlis tip de yakalar; hata kodu kontrolsuz kalir.
3. `Count > 0`: 1 ve 500 ayni; beklenen kapsam kaybolur.
4. Yalniz donus degeri: yan etki (DB yazimi) test disinda kalir, uretimde kirilir.

## §5 Kurallar

- Her assertion'a "bunu kirarsak test niye kirmizi olur" diye sor.
- Beklenen degeri sabitle (sayi, kod, durum); esik yazma (`> 0`).
- Exception testi tip + kod/mesaj ile; genel tip degil.
- Yan etki gerektiginde kaynak durumunu da dogrula.
- Assertion mesajini okunur yaz; kisaltma ve sihirli sayi yok.

## §6 Yasaklar

- `Assert.True(true)` ve assertion'siz test **YASAK**.
- Yalniz `Assert.NotNull` / `IsTrue(IsSuccess)` **YASAK**; somut deger ekle.
- `Assert.Throws<Exception>` gibi genel tip **YASAK**; ozel tip.
- `try/catch` + `Assert.True(false)` **YASAK**; `ThrowsAsync` kullan.
- Kaynak durumu yoksayilan tam yan etki testi **YASAK**.

## §7 Test Baglantisi

- Iddia cesitliligi kapsam raporuna yansir: esit assertion tipi = tek boyutlu kapsam.
- Assertion kalitesi, test adinin (`Yontem_Senaryo_Beklenen`) vaadiyle ayni olmali.
- Flaky oldugundan suphelenilen test once determinizme bakilarak incelenir. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Test Kurallari](../../.rules/testing-rules.md) · [Test Determinizmi](testing-determinism.md) · [Isimlendirme ve Yorum](naming-and-comments.md) · [C Sharp Index](index.md)

## §9 Kanit

- Iyi/kotu karsilastirma tablosu test kural dosyasinin iddia cesitliligi bolumunden derlendi. Kaynak: [Test Kurallari](../../.rules/testing-rules.md), 2026-09-25.
- `VC-E-0003` kodu ornek niteligindedir; gercek kod listesi proje enum'unda DOĞRULAMA GEREKLİ.
