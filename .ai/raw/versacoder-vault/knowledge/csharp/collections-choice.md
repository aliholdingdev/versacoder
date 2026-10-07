---
title: "collections-choice - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "topululuk secimi (List/Dictionary/HashSet/Queue...), kapasite yonetimi ve net48 ile net10 arasindaki topluluk farklari"
---

# collections-choice — Koleksiyon seçimi

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) · [threading-locking](threading-locking.md)

> **Tek cumlelik sorumluluk:** *Bir veri grubu hangi koleksiyonda tutulur ve hangi topluluk hangi bacakta var* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Secim **erişim desenine** gore yapilir: sira mi, anahtar mi, tekrarsiz mi, LIFO/FIFO mi, tek yazici/cok okuyucu mu. Varsayilan **`List<T>`** (sirali, indeksli), anahtarli hizli arama **`Dictionary<TKey,TValue>`**, tekrarsiz kume **`HashSet<T>`**, kuyruk **`Queue<T>`/`Stack<T>`**, eszamanli dagitilmis **`ConcurrentDictionary`** ve `net10`'da **`FrozenDictionary`** (salt okunur, hizli). `net48` bacakta yeni topluluklar (`PriorityQueue`, `FrozenDictionary`, `CollectionsMarshal`) **yoktur**; ortak kod 7.3'te de calisan `List`/`Dictionary`/`HashSet` cekirdeginde kalir.

---

## §1 Secim Tablosu

| Ihtiyac | Secim | Neden | net48 | net10 |
|---|---|---|---|---|
| Sirali eleman, indeksli erisim | `List<T>` | Genel amacli, ekonomik | Var | Var |
| Anahtar -> deger, O(1) | `Dictionary<TKey,TValue>` | Hizli arama/guncelleme | Var | Var |
| Tekrarsiz kume, `Contains` hizi | `HashSet<T>` | O(1) uyelik | Var | Var |
| FIFO / LIFO | `Queue<T>` / `Stack<T>` | Iş kuyrugu | Var | Var |
| Iki yonlu, ortadan silme | `LinkedList<T>` | Nadir, pahali alloc | Var | Var |
| Tek yazici + cok okuyucu | `ConcurrentDictionary` | Kilit icine gizlenmis atomik | Var | Var |
| Oncelikli kuyruk | `PriorityQueue<T,TPriority>` | **.NET 6+** | **Yok** | Var |
| Salt okunur, dondurulmus harita | `FrozenDictionary` / `FrozenSet` | **.NET 8+** | **Yok** | Var |
| Degismez dizi | `ImmutableArray<T>` (paket) | Paylasim guvenligi | Paketle | Yerlesik |
| `List<T>`'ye ham span yazma | `CollectionsMarshal.AsSpan` | Sifir alloc guncelleme | **Yok** (.NET 6+) | Var |
| Ilkel tip icinde arama | `SearchValues` | Hizli `Contains(char)` | **Yok** (.NET 8+) | Var |

> **KURAL:** Eski `ArrayList` / `Hashtable` (non-generic, **boxing**) bu vault'ta **kullanilmaz**; nesil tipler tercih edilir — hem tip guvenligi hem alloc.

## §2 Kapasite ve Disaridan Gorunurluk

| Kural | Uygulama | Ihlalinin bedeli |
|---|---|---|
| Eleman sayisi biliniyorsa `new List<T>(kapasite)` | Tek buyuk toplama | Yeniden buyume + alloc |
| Disariya `IReadOnlyList<T>` / `IReadOnlyDictionary` ver | Ic tip `List<T>` gizli kalir | Disaridan `Add` ile bozulma |
| Toplulugu `private set` + `IReadOnlyX` don | Degismez sozlesme | Beklenmedik degisiklik |
| Yazma > okuma ve cok thread | `ConcurrentDictionary` ya da tek kilitleme | Yarış durumu (bkz. [threading-locking](threading-locking.md)) |
| Eleman silme sik + sirali | `List.Remove` O(n) | Sık silmede `LinkedList`/yeniden kur |

## §3 Ornek

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
                if (_id.TryGetValue(id, out var k))
                    liste.Add(k);
            return liste;                       // ic List<T> disari cikmaz
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
| `Dictionary` + `HashSet` | Anahtarli arama ile kume uyeligi birlikte |
| Onbelirli kapasite (64) | Buyume alloc'ini azaltir |
| `IReadOnlyCollection<T>` | Disaridan yazma kapali |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Value tipi anahtarla eski `Hashtable` | Boxing + tip hatasi | `Dictionary<TKey,TValue>` |
| 2 | `net10`'daki `PriorityQueue`'yu ortak koda yazmak | net48 derleme hatasi | §1 tablosu veya sirali `List` |
| 3 | `List<T>`'yi disarida `Add` ile genisletmek | Kapsam disi degisiklik | `IReadOnlyList<T>` |
| 4 | Toplulugu `lock` disinda paylasmak | Yarış (race) | [threading-locking](threading-locking.md) |
| 5 | Kapasiteyi tahminlemeden dev toplama | GC baskisi | Bilinen sayiyla basla |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Erişim deseni | §1 satiri: desen -> topluluk eslesmesi |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |
| Eszamanlılik | Ayni topluluk cok thread ise kilitleme testi |
| Kardesler | [linq-performance-pitfalls](linq-performance-pitfalls.md) · [span-interop-net10](span-interop-net10.md) |
