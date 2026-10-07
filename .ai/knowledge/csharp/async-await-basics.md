---
title: "async-await-basics - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "async tum yol kurali, ConfigureAwait secimi, beklemenin yasak oldugu iki yazim ve iki bacakta (net48 / net10.0-windows) asenkron API farklari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# async-await-basics — Asenkron yazım temeli

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-rules](../../.rules/async-rules.md) · [cancellation-token-policy](cancellation-token-policy.md) · [task-vs-value-task](task-vs-value-task.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp 6/28):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Bir is asenkron nasil yazilir, `ConfigureAwait` nerede kirma yapar ve asenkron zincir hangi uc noktada kirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tek kural **async all the way**: asenkron bir metodu cagiriyorsan cagiran da `async Task` olur; `async void` yalnizca WinForms olay baglayicisinda serbesttir (`CS-06`). Bekleme `await` ile olur; `.Result` / `.Wait()` / `.GetAwaiter().GetResult()` bu vault'ta **yasaktir** (`CS-03`) cunku net48 senkron-baglam deadlock'u uretir. Baglam ikiye ayrilir: **UI katmaninda varsayilan context korunur**, **kutuphane katmaninda `ConfigureAwait(false)` ile kasitli kirilir** (`async-rules` A1/A2). Iki bacak farki kucuktur ama taahhuttur: `await foreach`, `await using`, `Task.WaitAsync` ve `PeriodicTimer` net48'de yoktur — ortak kod en kucuk ortak paydada kalir ([language-versions-net48-net10](language-versions-net48-net10.md)).

```text
+---------------------------------------------------------+
| cagiran   : async Task / Task<T>   (async void YASAK)    |
| beklemek  : await                  (.Result / .Wait YASAK)|
| baglam    : UI -> koru | katman -> ConfigureAwait(false)  |
| iptal     : ct her halkada          (cancellation-token)  |
| donus     : Task mi ValueTask mi -> [task-vs-value-task]  |
+---------------------------------------------------------+
```

---

## §1 Karar matrisi

| Durum | Secim | Sonuc |
|---|---|---|
| Geri donus tipi | `Task` / `Task<T>` | hata yakalanabilir, test edilebilir |
| Geri donus tipi (handler disi) | `async void` **yasak** | istisna islem seviyesine tasınir |
| Bekleme | `await` | baglam ve iptal korunur |
| Bekleme (yasak) | `.Result` / `.Wait()` | net48 deadlock, iptal yok |
| Baglam (kutuphane, l0-l4) | `ConfigureAwait(false)` | gereksiz context yakalama yok |
| Baglam (l7 UI) | varsayilan (`true`) | kontrol yazimlari Invoke gerektirmez |
| Paralel is | `Task.WhenAll` | sira zorunlu degilse sure kazanci |
| Is degil, bekleyen yapilanma | [channel-queue-patterns](channel-queue-patterns.md) | kilit + await yok |

## §2 Iki bacakta asenkron API

| Ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `async`/`await`, `Task`, `Task<T>` | Var | Var |
| `Task.WhenAll` / `WhenAny` | Var | Var |
| `ConfigureAwait(bool)` | Var | Var |
| `await using` + `IAsyncDisposable` | Yok — `Microsoft.Bcl.AsyncInterfaces` + dil 8 | Yerlesik |
| `await foreach` + `IAsyncEnumerable<T>` | Yok — ayni paket + dil 8 | Yerlesik |
| `Task.WaitAsync(CancellationToken)` | Yok — .NET 6+ | Var |
| `PeriodicTimer` | Yok — .NET 6+ | Var |
| CA2007 (ConfigureAwait dogrulama) | analiz eklentisiyle | SDK analizleriyle yerlesik |

> **TAHHUT:** `net48`'de `await foreach` acmak uc parcali karardir: paket referansi + `LangVersion` + test. Tek bacaga yazilan asenkron kod diger bacagi kiratmadan once deney degil, karardir — [ADR-001](../../.decisions/accepted/adr-001-dual-target-net48-net10.md) baglaminda `architect` onayi gerekir.

## §3 Ornek: iki katman, iki yazim

```csharp
// l7 UI (WinForms) - olay baglayicisi: context gerekli, tek serbest async void
private async void btnKaydet_Click(object? sender, EventArgs e)
{
    try
    {
        var sonuc = await _service.KaydetAsync(_model, _cts.Token);
        lblDurum.Text = sonuc.Durum;
    }
    catch (OperationCanceledException) { lblDurum.Text = "iptal"; }
}

// l4 kutuphane - context gerekmez, deadlock guvenligi icin kirilir
internal async Task<SessionDto> YukleAsync(int id, CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();
    await _baglanti.OpenAsync(ct).ConfigureAwait(false);
    return await _baglanti.QuerySingleOrDefaultAsync<SessionDto>(
        sql, new { Id = id }, commandTimeout: 15, cancellationToken: ct
    ).ConfigureAwait(false) ?? throw new KeyNotFoundException($"session {id}");
}
```

Sorgu metni **parameterized** yazilir (`@Id`); string-concat SQL yasaktir (`ADR-005`, G2).

## §4 Zincirin kirildigi noktalar

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| UI donuyor ama CPU calisiyor | `.Result` / `.Wait()` | tum yol `async Task` + `await` |
| Iptal butonu calismiyor | token gecirilmemis | zincir boyunca `ct` ([cancellation-token-policy](cancellation-token-policy.md)) |
| UI'a yazim hatasi | `ConfigureAwait(false)` sonrasi context yok | context gerektiren kodu UI katmaninda ayir |
| Cift tiklaninca iki is | onceki is iptal edilmiyor | `CancellationTokenSource` iptal + yenile |
| `async void` hata patlatiyor | handler disi `async void` | `async Task` + cagiran `await` |

## §5 Kurallar (bu dosyanin kisa cevabi)

1. `async` goruyorsan donus tipi `Task`/`Task<T>` olsun; `async void` yalniz olay baglayicisi.
2. Bekleme her yerde `await`; `CS-03` yasagi kod taramasinda **0** satir ister.
3. Baglam kirma katman goredir: kutuphane `false`, UI varsayilan (`async-rules` A1/A2).
4. Her public async imza son parametrede `CancellationToken` tasir (`CS-04`).
5. Uzun is gerekcesiz `Task.Run` ile degil, kuyrukla disari alinir; CPU-bound ise `Task.Run(..., ct)`.

## §6 Yasaklar

| # | Yasak | Karshiligi | Kaynak |
|---|---|---|---|
| 1 | `.Result` · `.Wait()` · `.GetAwaiter().GetResult()` | tum yol `async` | `CS-03` |
| 2 | handler disi `async void` | `async Task` | `CS-06` |
| 3 | token'siz public async imza | `CancellationToken ct = default` | `CS-04` |
| 4 | `lock` icinde `await` | tek is parcacikli kuyruk | `async-rules` A6 |
| 5 | kanitsiz "UI bloklanmiyor" | stopwatch olcumu | G5 |

## §7 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| `async void` sayisi | yalniz WinForms olay baglayicisi (0 disi) |
| `.Result` / `.Wait()` | `Select-String -Pattern "\.Result|\.Wait\(\)"` = 0 |
| Baglam tutarliligi | kutuphanede `ConfigureAwait` yaygin, UI'da yok |
| Iki bacak | `dotnet build` -> 0 hata (net48 + net10.0-windows) |
| Iptal yolu testi | `dotnet test` gecti (TS-07) |

## §8 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| Iptal imzasi ve zinciri | [cancellation-token-policy](cancellation-token-policy.md) |
| `Task` mi `ValueTask` mi | [task-vs-value-task](task-vs-value-task.md) |
| Kilit yerine kuyruk | [channel-queue-patterns](channel-queue-patterns.md) |
| UI busy durumu ve fire-and-forget | [async-ui-state-flows](async-ui-state-flows.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | `.Result`/`.Wait()` yasagi | `.rules/coding-standards.md` CS-03 |
| 2 | `async void` siniri | `.rules/coding-standards.md` CS-06 |
| 3 | Baglam kirma kurali | `.rules/async-rules.md` A1/A2 |
| 4 | token'li imza zorunlulugu | `.rules/coding-standards.md` CS-04 |
| 5 | net48'de `await foreach`/`WaitAsync` yok | `.rules/coding-standards.md` CS-12 + ADR-001 ikili bacak |
