---
title: Async UI Durum Akislari
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: SSOT
scope: csharp
---

# Async UI Durum Akislari

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Async Kurallari](../../.rules/async-rules.md) | [Iptal Tokeni](cancellation-token-policy.md) | [C Sharp Index](index.md)
**Küme + Hub:** [index](../../index.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (csharp · 3/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [channel-queue-patterns](channel-queue-patterns.md) · [collections-choice](collections-choice.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [records-and-immutability](records-and-immutability.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [span-interop-net10](span-interop-net10.md) · [testing-determinism](testing-determinism.md) · [threading-locking](threading-locking.md)

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
- Bloklayici kalip taramasi: `.Result|.Wait()|async void` gecen satirlar incelenir. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Async Kurallari](../../.rules/async-rules.md) · [Iptal Tokeni](cancellation-token-policy.md) · [Task vs ValueTask](task-vs-value-task.md) · [C Sharp Index](index.md)

## §9 Kanit
- Kurallar (A1..A10) ve istisna tablosu async kural dosyasindan derlendi. Kaynak: [Async Kurallari](../../.rules/async-rules.md), 2026-09-25.
- Temel yasaklar kodlama standartlarinda (CS-03/04/06); cakisma yok, derinlik async dosyada. Kaynak: [Kodlama Standartlari](../../.rules/coding-standards.md).
