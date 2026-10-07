---
title: "async-await-basics - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "async/await temelleri, async tum yol kurali, ConfigureAwaitSecimi ve iki bacakta asenkron API farklari"
---

# async-await-basics — Asenkron temeller

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [task-vs-value-task](task-vs-value-task.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md)

> **Tek cumlelik sorumluluk:** *Bir is nasil asenkron yazilir, `ConfigureAwait` nerede kullanilir ve asenkron zincir nerede kirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kural **async all the way**: asenkron bir metot cagiriyorsan kendi metotun da `async Task` olur; `async void` yalnizca WinForms **olay baglayicisinda** (event handler) serbesttir, geri kalan her yerde hata yakalanamaz ve test edilemez. **Kitaplik/katman kodunda `ConfigureAwait(false)`, UI katmaninda `true` (varsayilan)** kullanilir; WinForms'da `true` olmadan devam eden kod `Control.BeginInvoke` gerektirir. Iptal her asenkron zincirde [cancellation-token-policy](cancellation-token-policy.md) ile taşinir. Iki bacak farki kucuktur: `net48`'de `IAsyncEnumerable`/`await foreach`/`await using` ek paket + dil surumu ister; `net10.0-windows` hepsi yerlesiktir.

---

## §1 Temel Kural Tablosu

| Kural | Iyi | Kotu | Neden |
|---|---|---|---|
| Geri donus tipi | `Task` / `Task<T>` | `async void` (handler disi) | `async void` hatasi process'i atar |
| Bekleme | `await` | `.Result` / `.Wait()` | Deadlock (WinForms SynchronizationContext) |
| Kirisma | `ConfigureAwait(false)` (katman) | Katalogda her yerde `true` | Gereksiz context yakalama |
| Paralel is | `Task.WhenAll` | Art arda `await` (gereksizse) | Sira zorunlu degilse sure kaybi |
| Iptal | `CancellationToken` | Yok | Kullanici iptali calismaz |

## §2 Iki Bacakta Asenkron API

| API / ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `async`/`await`, `Task`, `Task<T>` | Var (4.5+; `Task.CompletedTask` 4.6+) | Var |
| `Task.WhenAll` / `WhenAny` | Var | Var |
| `ConfigureAwait(bool)` | Var | Var |
| `await using` + `IAsyncDisposable` | Yok (C# 8 + `Microsoft.Bcl.AsyncInterfaces` paketi) | Var, yerlesik |
| `await foreach` + `IAsyncEnumerable<T>` | Yok (ayni paket + dil surumu 8) | Var, yerlesik |
| `Task.WaitAsync(CancellationToken)` | Yok (.NET 6+) | Var |
| `PeriodicTimer` | Yok (.NET 6+) | Var |
| `ConfigureAwait` dogrulama (analyzer CA2007) | Analiz eklentisiyle | SDK analizleriyle yerlesik |

> **UYARI:** `net48` bacakta `await foreach` yazmak isterse, bu **iki bacakli bir taahhuttur**: paket referansi + dil surumu + test = uclu birlikte. Tek bacaga yazilan asenkron kod, diger bacagin derlemesini kirmadan once **deney degil, karardir** (ADR acilir).

## §3 Ornek: UI katmani vs katman ici

```csharp
// UI (WinForms) - kontrolleri yazacagimiz icin varsayilan context gerekir
private async void btnKaydet_Click(object? sender, EventArgs e)
{
    try
    {
        var sonuc = await _service.KaydetAsync(_model, _cts.Token);
        lblDurum.Text = sonuc.Durum;
    }
    catch (OperationCanceledException)
    {
        lblDurum.Text = "iptal";
    }
}
```

```csharp
// Katman ici (library) - context'e ihtiyac yok, hiz + deadlock guvenligi
internal async Task<SessionDto> YukleAsync(int id, CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();
    await _baglanti.OpenAsync(ct).ConfigureAwait(false);
    var satir = await _baglanti.QuerySingleOrDefaultAsync<SessionDto>(sql, new { Id = id },
        commandTimeout: 15, cancellationToken: ct).ConfigureAwait(false);
    return satir ?? throw new KeyNotFoundException($"session {id}");
}
```

Sorgu metni **parameterized** yazilir (`@Id`); string-concat SQL yasaktir (G2, bkz. [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md)).

## §4 Zincirin Kirildigi Noktalar

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| UI donuyor ama CPU calisiyor | `.Result` / `.Wait()` | `await` + `async` zinciri |
| Iptal butonu calismiyor | Token gecirilmemis | Zincir boyunca `ct` |
| UI'a yazim hata veriyor | `ConfigureAwait(false)` sonrasi context yok | UI katmaninda context gerekli kodu ayir |
| Cift tiklaninca iki is | Onceki is iptal edilmiyor | `CancellationTokenSource` iptal + yenile |
| `async void` hata patlatiyor | Handler disi `async void` | `async Task` yap, cagirani await et |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| `async void` sayisi | Yalniz WinForms olay baglayicisi (0 disi) |
| `.Result` / `.Wait()` | Kod taramasinda 0 |
| Iptal | Iptal testi: `dotnet test` gecti |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |

Ilgili kardesler: [task-vs-value-task](task-vs-value-task.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md).
