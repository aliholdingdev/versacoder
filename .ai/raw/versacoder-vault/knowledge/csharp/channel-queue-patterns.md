---
title: "channel-queue-patterns - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "System.Threading.Channels ile tek is parcacigi kuyruk, producer/consumer akisi, Complete/CompletionAsync ve net48 icin paket farki"
---

# channel-queue-patterns — Kuyruk ile Sıra ve İzolasyon

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [async-await-basics](async-await-basics.md) · [cancellation-token-policy](cancellation-token-policy.md)

> **Tek cümlelik sorumluluk:** *Birden fazla kaynaktan gelen iş tek bir iş parçacığında sıra ile nasıl yürütülür, kuyruk nasıl kapatılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`Channel<T>` = **tek yazan / tek okuyan** (veya çoklu) iş parçacığı arasında tip güvenli kuyruk. UI'da `lock` yerine **kuyruk** kullanılır: işleri tek bir tüketici sırayla işler, sıra garantisi kilit olmadan sağlanır. Kuyruk üç durumda biter: `Complete()` yazıldı ve boşaltıldı, tüketici hata aldı, iptal edildi (`CancellationToken`). `Bounded` (kapasiteli) kuyruk üreticiyi yavaşlatır — **bellek taşması**nın tek engeli budur.

```text
  uretici A (UI event)  --> \
  uretici B (log)       -->  +--> [ Channel<T> ] --> tek tuketici (is parcacigi)
  uretici C (webhook)   --> /        |                      |
                                     | Complete()           | CompletionAsync
                                     v                      v
                                  kanal kapandi         is bitti / hata raporu
```

---

## §1 Kuyruk tipi seçimi

| Seçenek | Ne zaman | Kod | Riski |
|---|---|---|---|
| `Channel.CreateUnbounded<T>()` | Az iş, bellek ucuz | varsayılan | üretici hızlanırsa RAM taşar |
| `Channel.CreateBounded<T>(500)` | Üretici > tüketici | kapasite **zorunlu** | dolunca yazan bekler ya da `DropNewest` |
| `SingleReader = true` | Tek tüketici (tipik) | okuma kilidi kalkar | yanlışsa yarış |
| `SingleWriter = true` | Tek üretici | yazma kilidi kalkar | yanlışsa yarış |
| `DropOldest` / `DropNewest` | Sayaç/telemetri gibi kaybedilebilir iş | kapasite dolunca düşürme | sessiz veri kaybı — dikkatli |

> **KURAL:** Bu vault'ta UI arka plan işleri **`Bounded(500)` + `SingleReader`** ile kurulur; kapasite dolunca uygulama **sessizce veri kaybetmez**, üretici `await WriteAsync` ile bekler.

---

## §2 Üretici / tüketici kalıbı

```csharp
public sealed class WorkQueue : IAsyncDisposable
{
    private readonly Channel<WorkItem> _ch =
        Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(500)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait      // dolunca yazan bekler
        });

    public ValueTask PostAsync(WorkItem item, CancellationToken ct)
        => _ch.Writer.WriteAsync(item, ct);             // uretici: tek satir

    public async Task RunAsync(CancellationToken ct)
    {
        await foreach (WorkItem job in _ch.Reader.ReadAllAsync(ct))   // tek tuketici
        {
            try   { await job.ExecuteAsync(ct); }
            catch (OperationCanceledException) { break; }             // iptal hata degil
            catch (Exception ex)              { _errors.Report(ex); } // hata ayri yolda
        }
    }

    public void Complete() => _ch.Writer.Complete();    // yeni is kabulunu kes

    public async ValueTask DisposeAsync()
    {
        Complete();
        await _ch.Reader.Completion;                    // tuketici bitene kadar bekle
    }
}
```

| Üye | Görev | Sık hata |
|---|---|---|
| `Writer.WriteAsync(item, ct)` | kuyruğa yaz (bekleyebilir) | `TryWrite` ile dolu kanal sessizce reddedilir |
| `Writer.Complete()` | "yeni iş yok" işareti | unutulursa tüketici sonsuza kadar bekler |
| `Reader.ReadAllAsync(ct)` | bitene kadar sırayla oku | `ReadAsync` döngüsünde tek tek yazmak daha karmaşık |
| `Reader.Completion` | kapanışın bitmesini bekleme | `Dispose`'da beklenmezse işler yarım kalır |
| `ChannelClosedException` | `Complete` sonrası yazma denemesi | üreticide kapanış kontrolü |

---

## §3 UI ile kullanım sınırı

| Kural | Neden |
|---|---|
| Kuyruktan **UI kontrollerine dokunan sonuç** `SynchronizationContext` üzerinden geri gelir (`await` UI thread'inde devam eder) | Kontrol thread'leri karışmaz |
| Uzun iş `ConfigureAwait(false)` ile sürdürülür | UI thread'i kilitlenmez |
| Tüketici **tek** olur (`SingleReader`) | sıra garantisi kilit olmadan doğar |
| Form kapanırken `Complete()` + `Cancel()` | yarım iş kalmaz (`cancellation-token-policy`) |
| `lock` içinde `Post` **yapılmaz** | kuyruğun kendisi kilitli değildir, gerek de yoktur |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `System.Threading.Channels` | **NuGet paketi** gerekir (`System.Threading.Channels`) | runtime ile birlikte gelir, paket eklenmez |
| `await foreach` | dil C# 7.3'te **yok** → `while (await WaitToReadAsync())` döngüsü | C# 14: `await foreach` doğrudan |
| `IAsyncDisposable` | `Microsoft.Bcl.AsyncInterfaces` paketi | runtime'da |
| `ValueTask` | paket üzerinden | runtime'da |
| `#if` | yalnız API varlığı farkı (G6) | — |

> **KURAL:** Ortak dosyada `await foreach` **kullanılmaz**; her iki bacakta derlenen kod `WaitToReadAsync()` döngüsüyle yazılır. `#if` yalnız paket/API farkını taşır.

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| `async/await` temelleri | [async-await-basics](async-await-basics.md) |
| İptal politikası | [cancellation-token-policy](cancellation-token-policy.md) |
| `Task` vs `ValueTask` | [task-vs-value-task](task-vs-value-task.md) |
| Kilit yerine kuyruk kararında | [threading-locking](threading-locking.md) |

---

## §6 Yasaklar

1. Kilit + `await` aynı blokta; UI işini kuyruksuz `Task.Run` ile savmak (G5 kanıtsızlık).
2. Sınırsız (unbounded) kanal ile bilinmeyen hızda üretici (bellek taşması).
3. `Writer.Complete()` çağrısı olmadan `Dispose` (işler yarım kalır).
4. EF Core; veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli.
5. Kuyruktan gelen veriyi kanıtsız "doğru işlendi" diye kapatmak — kanıt: tüketici sayacı + `dotnet build` çıktısı.
