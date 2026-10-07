---
title: "cancellation-token-policy - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "CancellationToken politikasi: imza kurali, iptal zinciri, eslesme hukmu ve iki bacakta iptal API'leri"
---

# cancellation-token-policy — İptal politikası

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-await-basics](async-await-basics.md) · [security-rules](../../.rules/security-rules.md)

> **Tek cumlelik sorumluluk:** *Bir `CancellationToken` nerede dogar, nereye kadar tasinir ve iptal neden hata sayilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her asenkron public metot **son parametrede `CancellationToken cancellationToken = default`** tasir; token **dogdugu yerde iptal edilir** (UI butonu, timeout, uygulama kapanisi) ve **zincirin her halkasina gecirilir**, ara katmanda `CancellationTokenSource` **yeni dogmaz** (yalnizca `CreateLinkedTokenSource` ile timeout eklenir). İptal bir hata degildir: `OperationCanceledException` **ayni token** ile gelmissa normal sonuc sayilir (yakalanip "iptal" olarak islenir), baskasinin token'indan gelmisse **hata**dir ve yukari taşinir. `net48` bacakta iptal API'leri genis de olsa `.NET 6+` eklemeleri (`Task.WaitAsync`, `PeriodicTimer`) yoktur; iki bacakli kod yalnizca ortak sette yazilir.

---

## §1 Imza ve Tasima Kurallari

| # | Kural | Ornek | Ihlalinin sonucu |
|---|---|---|---|
| 1 | Son parametre `CancellationToken ct = default` | `Task<Kayit> GetAsync(int id, CancellationToken ct = default)` | Cagiran iptal edemez |
| 2 | Token **zincir boyunca** gecer | `A` -> `B` -> `C(ct)` hepsi `ct` | Iptal yarida kalir |
| 3 | Yeni `CancellationTokenSource` sadece sahibinde | UI'da `using var cts = new()` | Cift kaynak, sızıntı |
| 4 | Timeout `CreateLinkedTokenSource` ile | Kullanici iptali + 15 sn | Ikisi de calisir |
| 5 | Dondurmeden once kontrol | `ct.ThrowIfCancellationRequested()` | Gereksiz is yapma |

## §2 Iki Bacakta IPTAL API'leri

| API | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `CancellationToken.ThrowIfCancellationRequested()` | Var | Var |
| `CancellationTokenSource` + `Cancel()` / `CancelAfter(ms)` | Var | Var |
| `CreateLinkedTokenSource(...)` | Var | Var |
| `token.Register(Action)` | Var | Var |
| `token.WaitHandle` | Var | Var |
| `Task.Delay(ms, ct)` / `Task.Run(..., ct)` | Var | Var |
| `Task.WaitAsync(ct)` | **Yok** (.NET 6+) | Var |
| `PeriodicTimer` + `WaitForNextTickAsync(ct)` | **Yok** (.NET 6+) | Var |
| `CancellationTokenSource.CancelAsync` | **Yok** (.NET 8+) | Var |

> **UYARI:** `net48`'de `Task.WaitAsync` ihtiyaci `Task.WhenAny(gorev, gecikme)` ile kapatilir; bu desen ortak katmanda yazilir ki iki bacak ayni testten gecsin.

## §3 Ornek: UI dogus + linkli timeout

```csharp
private CancellationTokenSource? _iptal;

private async void btnIptal_Click(object? sender, EventArgs e)
{
    _iptal?.Cancel();          // mevcut isi durdur
    _iptal?.Dispose();
    _iptal = null;
    lblDurum.Text = "iptal edildi";
}

private async Task TaraAsync()
{
    _iptal?.Dispose();
    _iptal = new CancellationTokenSource();
    var kullanici = _iptal.Token;
    using var bagli = CancellationTokenSource.CreateLinkedTokenSource(kullanici);
    bagli.CancelAfter(TimeSpan.FromSeconds(30));      // timeout

    try
    {
        var sonuc = await Servis.TaraAsync(bagli.Token).ConfigureAwait(false);
        lblDurum.Text = $"{sonuc.Adet} kayit";
    }
    catch (OperationCanceledException) when (kullanici.IsCancellationRequested)
    {
        lblDurum.Text = "kullanici iptal etti";       // normal sonuc
    }
    catch (OperationCanceledException)
    {
        lblDurum.Text = "30 sn asildi";               // timeout
    }
}
```

`when` filtresi, "kim iptal etti" sorusunu **istisnayi bastirmeden** ayirt eder; bu, iptal hukmunun temelidir.

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Token'i atlayip sadece `CommandTimeout` kullanmak | Kullanici iptali calismaz | Iki birlikte: timeout + ct |
| 2 | Metot icinde yeni `CancellationTokenSource` dogurmak | Sahiplik karisi, iptal kaybı | Sadece bagli kaynak |
| 3 | Iptal hatalarini genis `catch (Exception)` ile yutmak | Gec / yanlis raporlama | `catch (OperationCanceledException) when (...)` |
| 4 | UI'da token'i kapatmayip isi yarida birakmak | Kullanilmayan is akisi | `Dispose` + `Cancel` |
| 5 | Token'i `static` alanda tutmak | Testlerde kesisen iptal | Token'sahiplik instance'da |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Public async imzalar | Hepsinde son parametre `CancellationToken` |
| Iptal testi | `dotnet test`: iptal -> `OperationCanceledException` |
| SQL'de iptal | Dapper `cancellationToken:` parametresi, `commandTimeout` setli |
| Iki bacak | `net48` (WaitAsync'siz) + `net10.0-windows` build 0 hata |

Ilgili: [async-await-basics](async-await-basics.md) · [channel-queue-patterns](channel-queue-patterns.md) · [security-rules](../../.rules/security-rules.md).
