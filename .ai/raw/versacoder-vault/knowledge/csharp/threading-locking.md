---
title: "threading-locking - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Paralel erişimde kilit seçimi (lock/Monitor/Interlocked/ReaderWriterLockSlim), ölü kilit nedenleri ve net48 + net10.0-windows bacaklarında API farkı"
---

# threading-locking — Eşzamanlı Erişimde Kilit

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [collections-choice](collections-choice.md) · [async-await-basics](async-await-basics.md)

> **Tek cümlelik sorumluluk:** *Birden fazla iş parçacığı aynı veriye yazarken hangi kilit mekanizması seçilir, kilit ne zaman ölü sebep olur* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Paylaşılan **yazma** durumunda çözüm **tek bir `lock` nesnesi**dir; kilit her zaman **özel (private) `object` alanına** alınır, `this`'e ya da herkese açık bir nesneye asla. Sayısal sayaçlarda kilit yerine `Interlocked` (atomic CAS) kullanılır; çok oku / az yazma durumunda `ReaderWriterLockSlim` kazanır. `async` metot içinde `lock` **yazılmaz** — `await` sırasında kilidi tutmak eşzamanlılık hatası değil, kilit unutulması demektir; onun yerine tek iş parçacıklı kuyruk kullanılır (`channel-queue-patterns`).

| # | Durum | Araç | Neden |
|---|---|---|---|
| 1 | Basit paylaşılan durum (okuma + yazma) | `lock (_sync)` | En düşük maliyet, en düşük risk |
| 2 | Sayaç / bayrak atomik artırma | `Interlocked.Increment/Exchange` | Kilit açma-kapama yok |
| 3 | Çok oku, az yazma koleksiyonu | `ReaderWriterLockSlim` | Okuyucular birbirini beklemez |
| 4 | `async` metot içinde paylaşılan durum | **kilit yok** → tek iş parçacıklı kuyruk | `await` kilidi tutamaz |
| 5 | Koleksiyon eşzamanlı ekleme-okuma | `ConcurrentDictionary` | Kilit zaten içeride |

---

## §1 Kilit nesnesi kuralı

```csharp
public sealed class SessionCache
{
    private readonly object _sync = new object();      // PRIVATE, tip disina acilmaz
    private readonly Dictionary<string, Session> _map = new Dictionary<string, Session>();

    public Session GetOrCreate(string key)
    {
        lock (_sync)                                    // tek giris noktasi
        {
            Session s;
            if (!_map.TryGetValue(key, out s))
            {
                s = LoadFromStore(key);                 // yavas is KILIT DISINDA yapilmali
                _map[key] = s;
            }
            return s;
        }
    }
}
```

| Yanlış kilit nesnesi | Sonuç | Doğrusu |
|---|---|---|
| `lock (this)` | Dış kod `this`'e kilitlenebilir → beklenmeyen blok | `private object _sync` |
| `lock (typeof(SessionCache))` | Tüm tipte tek kilit, kimse paylaşmaz ama herkes etkilenir | tip başına private alan |
| `lock (_map)` (koleksiyonun kendisi) | Koleksiyonun kendisi de değiştirilebilir | sabit, private, hiç değişmeyen nesne |
| Kilidi tutarak I/O (`SqlClient`, dosya) | İş parçacığı havuzu tıkanır | I/O'yu kilidin **dışında** yap |
| Kilit içinde UI thread'ini beklemek | Donma (deadlock riski) | UI ile veri paylaşımı `Invoke`/`BeginInvoke` |

> **KURAL:** `lock` bloğunun içinde **yalnızca bellek içi kısa iş** vardır; SQL sorgusu, dosya yazımı ve `await` kilidin dışına çıkar. Kilidin içi 1 ms'yi aşarsa tasarım yeniden düşünülür.

---

## §2 Sayaçlar: `Interlocked` mı, `lock` mı

```csharp
private int _callCount;

// DOGRU — kilit acmadan atomik artis
Interlocked.Increment(ref _callCount);
int n = Interlocked.Increment(ref _callCount) - 1;      // eski degeri de al

// DOGRU — tek atamali degisim (CAS)
Interlocked.CompareExchange(ref _current, yeni, eski);

// GEREKSIZ — ayni sonuc, kilitleme maliyeti uzerine
lock (_sync) { _callCount++; }
```

| İhtiyaç | API | `lock` gereksinimi |
|---|---|---|
| sayaç artır/azalt | `Interlocked.Increment/Decrement` | hayır |
| tek yazıcılı bayrak-değiştir | `Interlocked.Exchange` | hayır |
| oku-yazma + koşul | `lock` + `Monitor.Wait/Pulse` | evet |
| karmaşık tutarlılık (2+ alan) | `lock` | evet (çoklu `Interlocked` yeterli değil) |
| sıra güvencesi | tek iş parçacıklı kuyruk | hayır (sıra zaten korunur) |

---

## §3 Olu Kilit (deadlock) ve çözümü

| # | Nedeni | Belirti | Çözüm |
|---|---|---|---|
| 1 | İki iş parçacığı farklı iki kilidi **ters sırada** alıyor | Program asla ilerlemez, CPU boştayken takılı | **Kilit sırası**nı global tek sıraya indir (önce A, sonra B — her yerde aynı) |
| 2 | Kilit içinde başka kilit (iç içe `lock`) | Aynı tip iki kez | Kilidi kısa tut, iç kilidi kaldır |
| 3 | Kilit içinde UI (`Control.Invoke`) beklemek | Pencere donar, mesaj pompuju kilitli | `BeginInvoke` + veri kopyalama |
| 4 | `lock` + `await` aynı blokta | Derleme hatası (C# `await` `lock` içinde yasak) | kuyruk (`channel-queue-patterns`) |
| 5 | `lock` içinde bloklayan `Task.Result`/`Wait()` | iş parçacığı havuzu tıkanır | `async/await` + `ConfigureAwait(false)` |

**Kilit sırası tablosu (tüm projede tek sırada tutulur):**

| Sıra | Kilit sahibi | Kilit nesnesi |
|---|---|---|
| 1 | `SessionCache` | `_sync` |
| 2 | `SettingsStore` | `_settingsSync` |
| 3 | (buraya eklenen her yeni kilit) | — |

> **KURAL:** Yeni bir `lock` alanı ekleyen geliştirici, §3.5 sırasına **sırasını** yazar. Sıra yazılmayan kilit, sıradaki RED bulgusudur (`reviewer`).

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `lock` / `Monitor` / `Interlocked` | **aynı** | aynı |
| `System.Threading.Lock` tipi (kilit niyeti taşıyan özel tip) | **yok** — NuGet yok, kodda kullanılamaz | runtime'da var; paylaşılan dosyada yine `object` kullanılır |
| `ReaderWriterLockSlim` | var | var (aynı davranış) |
| Dil | **C# 7.3**: `out` değişkenini satırda bildirir, `using var` yok | C# 14: `out var`, `using var` |
| `#if` | yalnız platform/API farkında | — |

> Paylaşılan `.cs` dosyasında `lock` her iki bacakta **birebir aynı** kalır; farklılık gerekiyorsa `#if NETFRAMEWORK` yalnızca API varlığı içindir (G6).

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| Koleksiyon seçimi (eşzamanlı sözlük dâhil) | [collections-choice](collections-choice.md) |
| `async/await` temelleri | [async-await-basics](async-await-basics.md) |
| `Task` vs `ValueTask` | [task-vs-value-task](task-vs-value-task.md) |
| Kuyruk ile kilitsiz sıra | [channel-queue-patterns](channel-queue-patterns.md) |

---

## §6 Yasaklar

1. `lock (this)` · `lock (typeof(T))` · herkese açık nesne üzerinde kilit (mimari RED).
2. `async` metot içinde `lock`, `Task.Wait()`, `Task.Result` (donma + kilit kaybı).
3. Kilit bloğu içinde dosya/SQL/ağ I/O (iş parçacığı havuzu tıkanması).
4. EF Core ile veri erişimi; bu vault'ta veri **ADO.NET + Dapper**, sorgu **parameterized** (`@p1`) — bkz. [coding-standards](../../.rules/coding-standards.md).
5. Kanıtsız "kilit yeterli" iddiası (G5): ölçü `dotnet build` çıktısı + kilit sırası tablosu.
