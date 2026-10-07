---
title: "channel-queue-patterns - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "System.Threading.Channels ile kilitsiz siralama: kuyruk tipi secimi, Complete/Completion akisi, UI siniri ve net48 paket farki"
---

# channel-queue-patterns — Kuyruk ile sıra ve izolasyon

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-rules](../../.rules/async-rules.md) · [threading-locking](threading-locking.md) · [cancellation-token-policy](cancellation-token-policy.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp 5/28):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Birden fazla kaynaktan gelen is tek tuketicide sirayla nasil yurutulur ve kuyruk nasil saglam kapatilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`Channel<T>`, tip guvenli kuyruktur ve **kilit olmadan** sirayi garanti eder: cok uretici, **tek tuketici** (`SingleReader = true`). Bu vault'ta UI arka plan isleri `Bounded(500) + SingleReader` ile kurulur; kapasite dolunca uretici `await WriteAsync` ile bekler, uygulama **sessizce veri kaybetmez**. Kuyruk uctur biter: `Complete()` + bosaltma, tuketicide hata, iptal ([cancellation-token-policy](cancellation-token-policy.md)). `lock` + `await` ayni blokta yasak oldugu icin (`async-rules` A6) kilitsiz sira arayan her yerin dogal cevabi budur.

```text
  uretici A (UI event) --> \
  uretici B (log)      -->  +--> [ Channel<T> ] --> tek tuketici
  uretici C (webhook)  --> /        |                      |
                                    | Complete()           | Completion
                                    v                      v
                                 kanal kapandi        is bitti / hata
```

---

## §1 Kuyruk tipi secimi

| Secenek | Ne zaman | Kod | Riski |
|---|---|---|---|
| `Channel.CreateUnbounded<T>()` | az is, bellek ucuz | varsayilan | uretici hizlanirsa RAM tasanir |
| `Channel.CreateBounded<T>(500)` | uretici > tuketici | kapasite **zorunlu** | dolunca bekler ya da `DropNewest` |
| `SingleReader = true` | tek tuketici (tipik) | okuma kilidi kalkar | yanlissa yaris |
| `SingleWriter = true` | tek uretici | yazma kilidi kalkar | yanlissa yaris |
| `DropOldest` / `DropNewest` | sayac/telemetri (kaybedilebilir) | kapasite dolunca dusme | sessiz veri kaybi |

## §2 Uretici / tuketici kalibi

```csharp
public sealed class WorkQueue : IAsyncDisposable
{
    private readonly Channel<WorkItem> _ch =
        Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(500)
        {
            SingleReader = true, SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });

    public ValueTask PostAsync(WorkItem item, CancellationToken ct)
        => _ch.Writer.WriteAsync(item, ct);

    public async Task RunAsync(CancellationToken ct)
    {
        while (await _ch.Reader.WaitToReadAsync(ct).ConfigureAwait(false))
        {
            while (_ch.Reader.TryRead(out var job))
            {
                try { await job.ExecuteAsync(ct).ConfigureAwait(false); }
                catch (OperationCanceledException) { return; }
                catch (Exception ex) { _errors.Report(ex); }
            }
        }
    }

    public void Complete() => _ch.Writer.Complete();

    public async ValueTask DisposeAsync()
    {
        Complete();
        await _ch.Reader.Completion.ConfigureAwait(false);
    }
}
```

> `while (await WaitToReadAsync())` dongusu **iki bacakta da derlenir**; `await foreach` yalniz net10 bacakta oldugu icin ortak dosyada kullanilmaz (`CS-12`, G6).

| Uye | Gorev | Sik hata |
|---|---|---|
| `Writer.WriteAsync(item, ct)` | kuyruga yaz (bekleyebilir) | `TryWrite` ile dolu kanal sessiz reddedilir |
| `Writer.Complete()` | "yeni is yok" isareti | unutulursa tuketici sonsuz beklemede kalir |
| `Reader.WaitToReadAsync` / `TryRead` | bitene kadar sirayla oku | tek tek `ReadAsync` yazimi karmaşık |
| `Reader.Completion` | kapanisin bitmesini bekle | `Dispose`'da beklemezse isler yarim kalir |
| `ChannelClosedException` | `Complete` sonrasi yazma | ureticide kapanis kontrolu |

## §3 UI ile kullanim siniri

| Kural | Neden |
|---|---|
| Sonuc UI kontrollerine `await` zinciriyle doner | SynchronizationContext korunur, `Invoke` gerekmez (`async-rules` A1) |
| Uzun is `ConfigureAwait(false)` ile surdurulur | UI thread'i kilitlenmez |
| Tuketici **tek** olur | sira garantisi kilitsiz dogar |
| Form kapanirken `Complete()` + `Cancel()` | yarim is kalmaz |
| `lock` icinde `Post` yapilmaz | kuyrugun kendisi kilitli degil, gerek de yok |

## §4 net48 / net10.0-windows farki

| Konu | net48 | net10.0-windows |
|---|---|---|
| `System.Threading.Channels` | **NuGet paketi** gerekir | runtime ile gelir |
| `await foreach` | dil 7.3'te **yok** -> `WaitToReadAsync` dongusu | C# 14: dogrudan |
| `IAsyncDisposable` | `Microsoft.Bcl.AsyncInterfaces` | runtime'da |
| `ValueTask` | `System.Threading.Tasks.Extensions` | runtime'da |
| `#if` | yalniz API varligi farki (G6) | — |

## §5 Kurallar

1. UI arka plan isi bilinmeyen hizda uretiliyorsa **Bounded** kullan; unbounded yalniz kucuk ve tahmin edilebilir is icin.
2. Kapanis her zaman `Complete()` + `Reader.Completion` beklemesiyle yapilir.
3. Tuketici hatasi **ayri yolda** raporlanir; kuyruk durmaz, iptal durdurur.
4. Kilit + `await` gerekmiyor gorunuyorsa once bu dosyaya bakilir (`threading-locking` §4).
5. Kuyruk kapasitesi ve tuketici sayisi magic deger degil, sabittir (`CS-14`).

## §6 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Kapanis | `DisposeAsync` -> `Reader.Completion` bekleniyor |
| Iptal | form kapanisinda is listesi bos |
| Iki bacak | `net48` (paketli) + `net10.0-windows` build 0 hata |
| Kilit taramasi | `lock` icinde `await` = 0 satir |

## §7 Yasaklar

| # | Yasak | Karshiligi |
|---|---|---|
| 1 | bilinmeyen hizda unbounded kanal | `Bounded(500)` |
| 2 | `Complete()`siz kapanis | `Complete()` + `Completion` |
| 3 | kuyruksuz `Task.Run` ile UI isini savmak | kuyruk ya da `async-rules` A3 gerekcesi |
| 4 | tuketici sayisini artirip sirayi bozmak | tek tuketici + disari sarmalama |
| 5 | kanitsiz "isler sirayla bitti" | tuketici sayaci + `dotnet test` |

## §8 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| kilit secimi | [threading-locking](threading-locking.md) |
| iptal | [cancellation-token-policy](cancellation-token-policy.md) |
| async temeli | [async-await-basics](async-await-basics.md) |
| eszamanli koleksiyon | [collections-choice](collections-choice.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | `lock` icinde `await` yasak | `.rules/async-rules.md` A6 |
| 2 | UI'da context korunur | `.rules/async-rules.md` A1 |
| 3 | `#if` yalniz platform farkinda | `.rules/coding-standards.md` CS-12 (G6) |
| 4 | `net48`'de `await foreach` yok | `.rules/coding-standards.md` CS-12 + ADR-001 |
| 5 | kilit/kuyruk iliskisi | `.rules/coding-standards.md` CS-03 |
