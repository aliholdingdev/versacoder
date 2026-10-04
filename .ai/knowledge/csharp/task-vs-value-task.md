---
title: Task ve ValueTask Secimi
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# Task ve ValueTask Secimi

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Async Kurallari](../../.rules/async-rules.md) | [Kodlama Standartlari](../../.rules/coding-standards.md) | [C Sharp Index](index.md)

> **Tek cumlelik sorumluluk:** `ValueTask` tahsissiz hiz icindir ve **bir kez** beklenebilir; emniyetli varsayilan hala `Task`.

## §0 Ozet (Answer First)

Secim tablosu: **Senkron hazir donus + sik cagri → ValueTask**. **Yeniden bekleme, iki yerde kullanma, senaryo belirsiz → Task**. Kural: `ValueTask`'i bir kez `await` et, sonrasi icin `AsTask()` ile `Task`'a gec. Yanlis kullanim tahsis avantajini siler, hatta yarisi bozuk durum birakir.

## §1 Karar Tablosu

| Senaryo | Secim | Sebep |
| --- | --- | --- |
| Cache/hafizada hazir deger | `ValueTask<T>` | Tahsis yok |
| Dogada hep bekleyen IO | `Task<T>` | Zaten tahsis var |
| Ayni donusu iki kez bekleme | `Task<T>` | Cift await yasak |
| Cagiran `Task.WhenAll` yapacak | `Task<T>` | Koleksiyon bekler |
| Senkron dal sik calisiyor | `ValueTask<T>` | Aninda donus |

## §2 Iki Bacak

**Bacak 1 - Maliyet:** `ValueTask<T>` struct; hazir degerse ek tahsis yapmaz. Cache'li katmanlarda, fallback'li okuma yollarinda gorunur kazanc saglar.

**Bacak 2 - Kullanim kisiti:** Tek kullanim hakki var. Await ettikten sonra ayni donusu tekrar beklemek tanimsiz davranis; gorunen hata degil, sessiz bozulmadir. Emin degilsen `AsTask()` ile `Task`'a cevir ve rahatla.

## §3 Ornek

```csharp
// IYI: sik cagirilan, cogunlukla hazir okuma
public async ValueTask<Kayit?> BulAsync(int id)
{
    if (_onbellek.TryGetValue(id, out var v))
        return v;                       // tahsis yok, aninda donus

    var kayit = await YukleAsync(id);
    _onbellek[id] = kayit;
    return kayit;
}

// IYI: iki kez gerekecekse gecis yap
var vt = BulAsync(7);
var liste = await vt.AsTask();          // simdi Task olarak rahat

// YASAK: ayni donus iki kez
// var vt = BulAsync(7);
// var a = await vt;
// var b = await vt;                     // tanimsiz
```

## §4 Hata Modlari

1. Cift await: bellek/cagri durumu yeniden kullanilir; yanlis deger veya istisna tekrari.
2. `ValueTask`'i `Task.WhenAll` ile beklemek icin once `AsTask()` gerekir; dogrudan koleksiyon yapma.
3. `ValueTask`'i field'da tutup bekletmek riskli; gecici ol.
4. Async metot icinde senkron donus yapip `ValueTask` sarmalamak avantaji siler; yarim async'e dikkat.

## §5 Kurallar

- Varsayilan imza `Task`/`Task<T>` olsun; `ValueTask` bilincli kazanim olsun.
- `ValueTask``i tek sefer beklemeyi imza yorumuyla soyle.
- Cagri tarafinda iki kez beklenecekse `AsTask()`.
- Async cagrilarda `ConfigureAwait(false)` kurallari gecerli - bekleyen kimse nezdinde. Detay: [Async Kurallari](../../.rules/async-rules.md).
- Yarim async kod yazma: ya tam async, ya tam senkron.

## §6 Yasaklar

- Ayni `ValueTask` iki kez await **YASAK**.
- `ValueTask`'i disari verip "istedigin gibi kullan" demek **YASAK**; kisiti imzada belli olsun.
- Senkron donus metnini `ValueTask.FromResult` ile uretip yarim async birakmak **YASAK**.
- Emin olunmayan senaryoda varsayilan `ValueTask` **YASAK**; `Task` sec.

## §7 Test Baglantisi

- Cache hit ve miss'i ayri test et: ikisi de ayni degeri dondurur, `Assert.AreEqual`.
- Async testlerde `await` zorunlu; senkron testte kalan async yol kacagidir.
- Cift-bekleme testi yazma (tanimsiz davranis); bunun yerine `AsTask()` kullanimini test et. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Async Kurallari](../../.rules/async-rules.md) · [Kodlama Standartlari](../../.rules/coding-standards.md) · [Iptal Tokeni](cancellation-token-policy.md) · [C Sharp Index](index.md)

## §9 Kanit

- Tek-bekleme kisiti dil seviyesinde (struct hafiza modeli); kaynak vault C# seti, 2026-09-25.
- `ConfigureAwait` ve iptal kurallari [Async Kurallari](../../.rules/async-rules.md) ile baglantili; proje geneli varsayilan icin DOĞRULAMA GEREKLİ.
