---
title: "messenger-eventaggregator - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "El yazimi Messenger (event aggregator): abonelik/iptal, zayif referans ve UI thread'e marshal, Bellek sizintisi kurallari ve net48 + net10.0-windows farklari"
---

# messenger-eventaggregator — El yazimi Messenger (event aggregator)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [viewmodel-inotify](viewmodel-inotify.md) · [command-pattern-mvvm](command-pattern-mvvm.md) · [form-navigation-patterns](form-navigation-patterns.md)

> **Tek cumlelik sorumluluk:** *Bir ekrandan/ViewModel'den digerine olay nasil gonderilir, abonelik nasil temizlenir, olay hangi thread'de islenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Messenger **tek global ornektir** (`static` ya da DI'da singleton) ve yalnizca **tip guvenli mesaj siniflari** tasinir (string degil). **Abone ol = iptal et** kurali: her form/ViewModel `Subscribe` yaptiysa `FormClosed`/`Dispose` icinde `Unsubscribe` yapar - yapmazsa kapanmis nesne hala yasar (sizin). Olay **aldigi thread'de degil, UI thread'inde** islenir: abone olurken `SynchronizationContext` (ya da ilgili `Control.BeginInvoke`) kaydedilir. DevExpress Mvvm `Messenger`'i yerine el yazimi kullanilir (G8'siz); zayif referansli varyant bilincli basitlestirmedir: abone acikca iptal eder.

---

## §1 Mesaj ve abonelik

```csharp
public sealed class ProjeAcildi
{
    public string Yol { get; private set; }
    public ProjeAcildi(string yol) { Yol = yol; }
}

public interface IMessenger
{
    void Subscribe<T>(object subscriber, Action<T> handler);
    void Unsubscribe(object subscriber);
    void Publish<T>(T message);
}
```

| Kural | Neden |
|---|---|
| Mesaj = kucuk, degismez veri sinifi | gonderen/alanyi birbirine baglamaz |
| Abone = nesne kimligi (ref) | ayni nesnenin cok handler'i olabilsin |
| `Unsubscribe(subscriber)` | sizin: `FormClosed`'da |
| `Publish` UI thread | handler kontrol'e deger yazar |
| Tip disindaki anahtar yok | string mesaj adi yazim hatasi uretir |

```csharp
public MainForm()
{
    InitializeComponent();
    _messenger.Subscribe<ProjeAcildi>(this, OnProjeAcildi);
    this.FormClosed += delegate { _messenger.Unsubscribe(this); };
}

void OnProjeAcildi(ProjeAcildi m)
{
    if (InvokeRequired) { BeginInvoke(new Action(() => OnProjeAcildi(m))); return; }
    lblYol.Text = m.Yol;
}
```

> **KURAL:** Handler **mutlaka** `InvokeRequired` (ya da abonelikte kaydedilen `SynchronizationContext`) kontrol eder; `Publish` herhangi bir thread'den gelebilir.

## §2 Abuse haritasi

| Kotu kullanim | Sonuc | Dogrusu |
|---|---|---|
| Her seyi messenger ile gondermek | akis okunmaz (SRP) | ana baba -> cocuk `EventCallback`/olay |
| `Unsubscribe` unutma | sizi nesne + handle sizintisi | `FormClosed`'da iptal |
| Mesaj icinde yol/nesne tasima | gizli bagimlilik | ID tasi, sorgu ile al |
| Handler'da agir is | UI donmasi | is -> `Application` katmani, sonra `Publish` |
| Ozel string "komut" mesaji | yazim hatasi, IDE bulamaz | tip guvenli sinif |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `SynchronizationContext.Current` | kutuda | ayni - kutuda |
| `Control.BeginInvoke` | ayni API | ayni (artik `Action` kabul eder) |
| `WeakReference` | ayni | ayni |
| Bellek modeli | AppDomain + statik durum | **domain reload yok** - statik messenger proces boyunca yasar: iptal kurali daha onemli |
| C# dili | **7.3** - `?.` ve `?.Invoke` **var** | C# 14 |

`#if` gerekmez. Dikkat: net48'te statik alani AppDomain yenilemesi sifirlayabilir, net10'da sifirlanmaz - bu yuzden statik messenger **abone listesini disarı yazmaz**, yalniz bellekte tutar.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Abone iptalsiz form | sizinti + olu handler | §1 `FormClosed` |
| 2 | UI disi thread'de handler | `InvalidOperationException` | `InvokeRequired` |
| 3 | Global messenger'a is mantigi | gorunmez akis | mesaj = veri, is baska yerde |
| 4 | String mesaj anahtari | tip hatasi | tip guvenli sinif |
| 5 | Messenger'i her formda `new` | olaylar birbirini gormez | tek ornek |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Durum bildirimi | [viewmodel-inotify](viewmodel-inotify.md) |
| Komut tetiklemesi | [command-pattern-mvvm](command-pattern-mvvm.md) |
| Form kapanisinda temizlik | [form-navigation-patterns](form-navigation-patterns.md) |
| Tray/pencere geri acma olayi | [tray-icon-lifecycle](tray-icon-lifecycle.md) |
