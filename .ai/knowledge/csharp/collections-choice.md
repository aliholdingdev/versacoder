---
title: "collections-choice - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "topluluk secimi erisim desenine gore (sira/anahtar/tekrarsiz/kuyruk), kapasite ve disari gorunurluk kurallari, net48 ile net10 arasindaki topluluk farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# collections-choice — Koleksiyon seçimi

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [coding-standards](../../.rules/coding-standards.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [threading-locking](threading-locking.md) · [span-interop-net10](span-interop-net10.md)
**Küme + Hub:** [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (csharp · 5/28):** [assertion-quality](assertion-quality.md) · [async-await-basics](async-await-basics.md) · [async-ui-state-flows](async-ui-state-flows.md) · [cancellation-token-policy](cancellation-token-policy.md) · [channel-queue-patterns](channel-queue-patterns.md) · [constants-and-magic-values](constants-and-magic-values.md) · [exception-handling-patterns](exception-handling-patterns.md) · [generics-constraints](generics-constraints.md) · [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [layer-dependency-direction](layer-dependency-direction.md) · [multi-targeting-props](multi-targeting-props.md) · [naming-and-comments](naming-and-comments.md) · [nullable-reference-types](nullable-reference-types.md) · [pattern-matching](pattern-matching.md) · [resource-disposal-patterns](resource-disposal-patterns.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md) · [serialization-choices](serialization-choices.md) · [solid-principles-applications](solid-principles-applications.md) · [task-vs-value-task](task-vs-value-task.md) · [testing-determinism](testing-determinism.md)

> **Tek cumlelik sorumluluk:** *Bir veri grubu hangi toplulukta tutulur, disariya hangi arayuzle acilir ve hangi topluluk hangi bacakta var* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Secim **erişim desenine** gore yapilir: sirali erisim mi, anahtarli arama mi, tekrarsiz kume mi, FIFO/LIFO mu, tek yazici-cok okuyucu mu. Varsayilan `List<T>`; anahtarli hizli arama `Dictionary<TKey,TValue>`; tekrarsiz `HashSet<T>`; kuyruk `Queue<T>`/`Stack<T>`; eszamanli `ConcurrentDictionary`. `net48` bacakta `PriorityQueue`, `FrozenDictionary`, `SearchValues`, `CollectionsMarshal.AsSpan` **yoktur** — ortak kod C# 7.3'te de calisan `List`/`Dictionary`/`HashSet` cekirdeginde kalir. Eski non-generic topluluklar (`ArrayList`, `Hashtable`) bu vault'ta **kullanilmaz**: boxing + tip guvensizlik (`CS-16`, `CS-09`).

## §1 Secim tablosu

| Ihtiyac | Secim | Neden | net48 | net10 |
|---|---|---|---|---|
| Sirali eleman, indeksli erisim | `List<T>` | genel amacli, ekonomik | Var | Var |
| Anahtar -> deger, O(1) | `Dictionary<TKey,TValue>` | hizli arama/guncelleme | Var | Var |
| Tekrarsiz kume, `Contains` hizi | `HashSet<T>` | O(1) uyelik | Var | Var |
| FIFO / LIFO | `Queue<T>` / `Stack<T>` | is kuyrugu | Var | Var |
| Iki yonlu, ortadan silme | `LinkedList<T>` | nadir, pahali alloc | Var | Var |
| Tek yazici + cok okuyucu | `ConcurrentDictionary` | kilit icine gizlenmis atomik | Var | Var |
| Oncelikli kuyruk | `PriorityQueue<T,TPriority>` | .NET 6+ | **Yok** | Var |
| Salt okunur, dondurulmus harita | `FrozenDictionary` / `FrozenSet` | .NET 8+ | **Yok** | Var |
| Degismez dizi | `ImmutableArray<T>` | paylasim guvenligi | Paketle | Yerlesik |
| `List<T>`'ye ham span yazma | `CollectionsMarshal.AsSpan` | sifir alloc guncelleme | **Yok** | Var |
| Ilkel tip icinde arama | `SearchValues` | hizli `Contains(char)` | **Yok** | Var |

## §2 Kapasite ve disaridan gorunurluk

| Kural | Uygulama | Ihlalinin bedeli |
|---|---|---|
| Eleman sayisi biliniyorsa `new List<T>(kapasite)` | tek buyuk toplama | yeniden buyume + alloc |
| Disariya `IReadOnlyList<T>` / `IReadOnlyDictionary` ver | ic tip `List<T>` gizli kalir | disaridan `Add` ile bozulma |
| Topluluk `private set` + `IReadOnlyX` don | degismez sozlesme | beklenmedik degisiklik |
| Yazma > okuma ve cok thread | `ConcurrentDictionary` ya da tek kilitleme | yaris durumu |
| Sik silme + sirali | `List.Remove` O(n) | sik silmede yeniden kur |

```csharp
internal sealed class KayitIndeksi
{
    private readonly Dictionary<int, Kayit> _id = new Dictionary<int, Kayit>(64);
    private readonly HashSet<int> _aktif = new HashSet<int>();

    internal IReadOnlyCollection<Kayit> Aktifler
    {
        get
        {
            var liste = new List<Kayit>(_aktif.Count);
            foreach (var id in _aktif)
                if (_id.TryGetValue(id, out var k)) liste.Add(k);
            return liste;                    // ic List<T> disari cikmaz
        }
    }

    internal void Ekle(Kayit kayit)
    {
        _id[kayit.Id] = kayit;
        _aktif.Add(kayit.Id);
    }
}
```

| Secim | Mantik |
|---|---|
| `Dictionary` + `HashSet` | anahtarli arama ile kume uyeligi birlikte |
| onbelirli kapasite (64) | buyume alloc'ini azaltir |
| `IReadOnlyCollection<T>` | disaridan yazma kapali |

## §3 Iki bacak notu

| Konu | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| Koleksiyon ifadesi `[1, 2]` (C# 12) | Yok | Var |
| `List<T>.AddRange` + `Span` uzerinden eleman | kisli (paketle) | Var |
| `foreach` uzerinde `ref` okuma | Yok (C# 7.2 kisli) | Var |
| Kutuphane paketi | `System.Collections.Immutable` gerekir | runtime |

> Koleksiyon ifadesi ve `Frozen*` tipleri **ortak dosyaya yazilmaz**; lazimsa dosya bacaga ayrilir ve `#if` yalniz API farkinda kalir (G6).

## §4 Sik hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `Hashtable` ile value tipi anahtar | boxing + tip hatasi | `Dictionary<TKey,TValue>` |
| 2 | `net10`'daki `PriorityQueue`'yu ortak koda yazmak | net48 derleme hatasi | §1 tablosu veya sirali `List` |
| 3 | `List<T>`'yi disarida `Add` ile genisletmek | kapsam disi degisiklik | `IReadOnlyList<T>` |
| 4 | toplulugu `lock` disinda paylasmak | yaris | [threading-locking](threading-locking.md) |
| 5 | kapasite tahminsiz dev toplama | GC baskisi | bilinen sayiyla basla |

## §5 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| Erisim deseni | §1 satirinda desen -> topluluk eslesmesi |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |
| Eszamanlilik | ayni topluluk cok thread ise kilitleme testi |
| Disaridan yazma | public imzalarda `IReadOnlyX` |

## §6 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| LINQ tuzaklari | [linq-performance-pitfalls](linq-performance-pitfalls.md) |
| kilit secimi | [threading-locking](threading-locking.md) |
| span ile sifir alloc | [span-interop-net10](span-interop-net10.md) |
| degismez tip | [records-and-immutability](records-and-immutability.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | non-generic topluluk yasagi, ordinal karsilastirma | `.rules/coding-standards.md` CS-16 |
| 2 | `magic` kapasite sayisi sabit/`Constants` olmali | `.rules/coding-standards.md` CS-14 |
| 3 | eszamanli erisimde kilit kurali | `.rules/coding-standards.md` CS-09 + `async-rules` A6 |
| 4 | net48'de `PriorityQueue`/`FrozenDictionary` yok | ADR-001 ikili bacak + CS-12 |
| 5 | kaynak/olcum disiplini | `.rules/testing-rules.md` TS-04 |
