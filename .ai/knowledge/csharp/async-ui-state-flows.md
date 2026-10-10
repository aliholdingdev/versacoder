---
title: Async UI Durum Akislari
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: primary
scope: csharp
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Async UI Durum Akislari

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/async-rules|Async Kurallari]] | [[knowledge/csharp/cancellation-token-policy|Iptal Tokeni]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 3/28):** [[knowledge/csharp/assertion-quality|assertion-quality]] · [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/guard-clauses-and-method-shape|guard-clauses-and-method-shape]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/naming-and-comments|naming-and-comments]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** UI'da await senkron baglami korur; busy durum `finally`'de geri alinir, iptal hata degildir.

## §0 Ozet (Answer First)

UI akisi uc adim: **busy kur** (ribbon/form pasif), **islemi await et** (baglam korunur, `Invoke` gerekmez), **durumu finally'de geri al**. Kütüphane katmaninda (l1-l3) baglam `ConfigureAwait(false)` ile kasten kirilir; UI handler'inda kirma. CPU-bound is `Task.Run` ile UI'dan ayrilir.

## §1 Karar Tablosu

| Durum | Yap | Yapma |
| --- | --- | --- |
| UI katmaninda await | Dogrudan kontrol yaz | Her devirde `BeginInvoke` |
| Kutuphane katmaninda await | `ConfigureAwait(false)` | UI handler'inda kir |
| CPU-bound is | `Task.Run(..., ct)` | UI thread'de 5 sn dongu |
| Busy durum | `try/finally` ile geri al | Hata sonrasi pasif kalan ribbon |
| Fire-and-forget | Kayitli + hata yakalayici | `IsAsync();` (uyari + kayip) |

## §2 Iki Bacak

**Bacak 1 - Baglam politikasi:** UI'da `await` senkron baglami korudugundan kontrol dogrudan yazilir. Kütüphane kodu kimin bekledigini bilmez; `ConfigureAwait(false)` ile UI'a baglilik kaldirilir - ama yalnizca UI disindaki katmanda.

**Bacak 2 - Durum geri alma + iptal:** Uzun is baslarken pasiflestirilen kontroller, hata veya iptal dahil `finally`'de geri acilir. Iptal, ustten gelen token ile bildirilir; iptal exception'u dar `catch` ile ayristirilir, yutulmaz.

## §3 Ornek

```csharp
// UI handler: baglam korunur, durum finally'de geri alinir
private async void OnKaydetClick(object sender, EventArgs e)
{
    _kaydet.Enabled = false;
    try
    {
        var sonuc = await _handler.ExecuteAsync(_istek, _ct);
        _durum.Text = sonuc.IsSuccess ? "kaydedildi" : sonuc.Error.Code;
    }
    catch (OperationCanceledException) when (_ct.IsCancellationRequested)
    {
        _durum.Text = "iptal edildi";        // iptal hata degil
    }
    finally
    {
        _kaydet.Enabled = true;              // hata olsa da geri acilir
    }
}

// Kutuphane katmani: baglam kirilir
public async Task<Kayit?> BulAsync(int id, CancellationToken ct)
{
    var satir = await _db.QueryAsync(sql, new { Id = id }, cancellationToken: ct)
        .ConfigureAwait(false);
    return satir.SingleOrDefault();
}
```

## §4 Hata Modlari

1. `.Result` / `.Wait()` UI'da: net48'de senkron-baglam deadlock'u, ekran doner.
2. `finally`'siz busy: hata sonrasi buton pasif kalir, kullanici kilitlenir.
3. UI handler'inda `ConfigureAwait(false)`: kontrol disindan yazilmaya calisilir.
4. Kayitsiz fire-and-forget: uyari bastan kayar, hata sessiz kaybolur.
5. Token alinip kullanilmamis: iptal calismaz, is arka planda surer.

## §5 Kurallar

- UI'da await sonrasi kontrol yaz; ekstra `Invoke` sarma.
- Kutuphane katmaninda `ConfigureAwait(false)`; UI'da kullanma.
- Busy/aktif gecisini `try/finally` ile tek yerde yaz.
- Uzun islemde token + timeout link'li olsun; butce 30 sn uzerine gerekce.
- `async void` yalniz WinForms event handler.

## §6 Yasaklar

- `.Result`, `.Wait()`, `GetAwaiter().GetResult()` **YASAK** (her yerde).
- `lock` bloğu icinde `await` **YASAK**; `SemaphoreSlim.WaitAsync` kullan.
- Iptal exception'unu genel `catch` ile yutmak **YASAK**.
- `finally` olmadan busy kurmak **YASAK**.
- Kayitsiz fire-and-forget **YASAK**; `_ =` + hata yakalayici.

## §7 Test Baglantisi

- Iptal yolu testi zorunlu; yalniz mutlu yol bu dosyanin kaniti sayilmaz.
- Busy geri alma, hata senaryosunda ayri test edilir (buton tekrar acik mi).
- Bloklayici kalip taramasi: `.Result|.Wait()|async void` gecen satirlar incelenir. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/async-rules|Async Kurallari]] · [[knowledge/csharp/cancellation-token-policy|Iptal Tokeni]] · [[knowledge/csharp/task-vs-value-task|Task vs ValueTask]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit
- Kurallar (A1..A10) ve istisna tablosu async kural dosyasindan derlendi. Kaynak: [[.rules/async-rules|Async Kurallari]], 2026-09-25.
- Temel yasaklar kodlama standartlarinda (CS-03/04/06); cakisma yok, derinlik async dosyada. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
