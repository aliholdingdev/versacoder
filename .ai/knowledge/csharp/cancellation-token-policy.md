---
title: "cancellation-token-policy - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "CancellationToken nerede dogar, nereye kadar tasinir, linkli timeout nasil kurulur ve iptal istisnasinin hukmu nedir"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# cancellation-token-policy — İptal politikası

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-rules](../../.rules/async-rules.md) · [async-await-basics](async-await-basics.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp 5/28):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Bir `CancellationToken` nerede dogar, hangi imzalarla asagi iner ve iptal neden hata sayilmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her asenkron public metot **son parametrede `CancellationToken cancellationToken = default`** tasir (`CS-04`). Token **dogdugu yerde iptal edilir** (UI butonu, uygulama kapanisi, timeout) ve **zincirin her halkasina gecirilir**; ara katmanda yeni `CancellationTokenSource` dogmaz — yalnizca `CreateLinkedTokenSource` ile ustteki token'a timeout eklenir (`async-rules` A4). İptal hata degildir: `OperationCanceledException` **ayni token** ile gelmisse normal sonuc sayilir ve "iptal" olarak islenir, baskasinin token'indan gelmisse yukari tasınir (`error-handling-rules` E12). `net48` bacakta `.NET 6+` iptal eklemeleri (`Task.WaitAsync`, `PeriodicTimer`, `CancelAsync`) **yoktur**; ortak kod yalnizca ortak sette yazilir.

## §1 Imza ve tasima kurallari

| # | Kural | Ornek | Ihlalinin sonucu |
|---|---|---|---|
| 1 | Son parametre `CancellationToken ct = default` | `Task<Kayit> GetAsync(int id, CancellationToken ct = default)` | cagiran iptal edemez |
| 2 | Token zincir boyunca gecer | `A -> B -> C(ct)` | iptal yarida kalir |
| 3 | Yeni `CancellationTokenSource` yalniz sahibinde | UI'da `using var cts = new()` | cift kaynak, sızıntı |
| 4 | Timeout `CreateLinkedTokenSource` ile | kullanici iptali + 30 sn | ikisi birlikte calisir |
| 5 | Donmeden once kontrol | `ct.ThrowIfCancellationRequested()` | gereksiz is yapilir |

## §2 Iki bacakta iptal API'leri

| API | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `ThrowIfCancellationRequested()` | Var | Var |
| `CancellationTokenSource` + `Cancel()` / `CancelAfter(ms)` | Var | Var |
| `CreateLinkedTokenSource(...)` | Var | Var |
| `token.Register(Action)` / `token.WaitHandle` | Var | Var |
| `Task.Delay(ms, ct)` / `Task.Run(..., ct)` | Var | Var |
| `Task.WaitAsync(ct)` | **Yok** (.NET 6+) | Var |
| `PeriodicTimer.WaitForNextTickAsync(ct)` | **Yok** (.NET 6+) | Var |
| `CancellationTokenSource.CancelAsync` | **Yok** (.NET 8+) | Var |

`net48`'de `WaitAsync` ihtiyaci `Task.WhenAny(gorev, gecikme)` ile kapatilir; bu sarmalayici **ortak katmanda** yazilir ki iki bacak ayni testten gecsin.

## §3 Akis: dogus + linkli timeout

```text
+----------------------------------------------------------+
| UI butonu -> _iptal.Cancel()     (token dogdugu yer)      |
| servis    -> CreateLinkedTokenSource(kullanici)           |
|             + CancelAfter(30_000)                         |
| yakalama  -> when (kullanici.IsCancellationRequested)     |
|              -> "kullanici iptal etti" (normal sonuc)     |
|              else -> "30 sn asildi" (timeout)             |
+----------------------------------------------------------+
```

```csharp
private async Task TaraAsync()
{
    _iptal?.Dispose();
    _iptal = new CancellationTokenSource();
    var kullanici = _iptal.Token;
    using var bagli = CancellationTokenSource.CreateLinkedTokenSource(kullanici);
    bagli.CancelAfter(TimeSpan.FromSeconds(30));

    try
    {
        var sonuc = await Servis.TaraAsync(bagli.Token).ConfigureAwait(false);
        lblDurum.Text = $"{sonuc.Adet} kayit";
    }
    catch (OperationCanceledException) when (kullanici.IsCancellationRequested)
    { lblDurum.Text = "kullanici iptal etti"; }
    catch (OperationCanceledException)
    { lblDurum.Text = "30 sn asildi"; }
}
```

`when` filtresi "kim iptal etti" sorusunu istisnayi bastirmeden ayirt eder — iptal hukmunun temelidir.

## §4 Sik hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | yalniz `CommandTimeout`, token yok | kullanici iptali calismaz | timeout + ct birlikte |
| 2 | metot icinde yeni `CancellationTokenSource` | sahiplik karisi | yalniz bagli kaynak |
| 3 | iptali genis `catch (Exception)` ile yutmak | gec/yanlis raporlama | `catch (OperationCanceledException) when (...)` |
| 4 | token'i `static` alanda tutmak | testlerde kesisen iptal | sahiplik instance'da |
| 5 | iptali `ERROR` saymak | log yiginmasi | `error-handling-rules` E12 |

## §5 Kurallar

1. Token yalniz **sahibinde** dogar; ara katman yalniz **tasiyici**dir.
2. Timeout her zaman **linkli** kaynakla eklenir, ayri token olarak degil.
3. İptal `Result.Cancelled`/`ErrorCode.Cancelled` olarak doner; exception hata degildir.
4. SQL sorgusunda Dapper `cancellationToken:` + `commandTimeout` birlikte setlenir.
5. `net48` ortak kod `WaitAsync` kullanmaz; sarmalayici ortak katmanda yasar.

## §6 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Public async imzalar | hepsinde son parametre `CancellationToken` |
| Iptal testi | `dotnet test` -> iptal yolunda `OperationCanceledException` |
| SQL iptali | Dapper `cancellationToken:` + `commandTimeout` setli |
| Iki bacak | `net48` (WaitAsync'siz) + `net10.0-windows` build 0 hata |

## §7 Hata modlari

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| İptal butonu hiçbir sey yapmiyor | token hiyerarsi kirilmis | zinciri tara, her imzaya `ct` ekle |
| İptal loglari hata olarak yagiyor | E12 ihlali | `Result.Cancelled` + seviye INFO |
| 30 sn timeout calisiyor, kullanici iptali calismiyor | linkli kaynak ters kurulmus | kullanici token'i **ustte**, timeout bagli |
| Testte beklenmedik iptal | token static/ortak paylasilmis | fixture basina ayri `CancellationTokenSource` |

## §8 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| async temeli | [async-await-basics](async-await-basics.md) |
| timeout + yeniden deneme | [retry-and-timeout-policy](retry-and-timeout-policy.md) |
| kuyruk uzerinden iptal | [channel-queue-patterns](channel-queue-patterns.md) |
| hata bicimi | [result-type-design](result-type-design.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | token'li imza zorunlu | `.rules/coding-standards.md` CS-04 |
| 2 | linkli timeout kurali | `.rules/async-rules.md` A4 |
| 3 | iptal hata degildir | `.rules/error-handling-rules.md` E12 |
| 4 | timeout her sorguda acik | `.rules/error-handling-rules.md` E7 |
| 5 | net48'de `WaitAsync` yok | ADR-001 ikili bacak + `.rules/coding-standards.md` CS-12 |
