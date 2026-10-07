---
title: "single-instance-app - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Tek ornek (single instance) WinForms uygulamasi: named Mutex kilidi, ikinci ornegi birinciyi uyarma kanali ve net48 + net10.0-windows farklari"
---

# single-instance-app — Tek ornek kilidi + uyarma kanali

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [security-rules](../../.rules/security-rules.md) · [tray-icon-lifecycle](tray-icon-lifecycle.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Ikinci kez acilan kopya nasil algilanir ve birinci kopya nasil uyandirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kilif **named `Mutex`**'tir: `createdNew` false cikarsa ikinci ornek varsadir ve o **form acmadan** cikar. Uyarma kanali **named pipe**'tir (iki bacakta da kutudadir): ikinci ornek birincinin pipe sunucusuna kisa bir mesaj gonderir, birinci kuyruktan mesaji alip pencereyi one cikarir (`Show()` + `Activate()`). Mutex **isimlendirilmis** olmalidir (`Global\` ya da yerel onek) - isimsiz mutex yalniz proces ici korur. Kilif her iki bacakta aynidir; fark `#if` degil, **state modelindedir** (net10'da domain reload yoktur, statik durum sıfirlanmaz).

---

## §1 Kilif + uyarma akisi

```
 ikinci kopya                                  birinci kopya
      |                                             |
      +-- new Mutex(false, "Global\\VersaCoder", out created)
      |        created == false  ->  VAR            |
      +-- pipe uzerinden "SHOW" gonder  ----------->  |  (dinleyici thread)
      |        bekle / cik                         +-- mesaji al
      |                                            +-- BeginInvoke: pencereyi one cikar
```

| Parca | Secim | Neden |
|---|---|---|
| Mutex adi | `Global\VersaCoder.SingleInstance` (ilce giris `Local\` ile) | isimsiz = korumasiz |
| Olusturma | `new Mutex(false, name, out createdNew)` | sahiplenme basinda |
| Birakma | `ReleaseMutex()` + `Dispose()` (`finally`) | kilidi kirik birakma |
| Uyarma | `NamedPipeServerStream` / `NamedPipeClientStream` | kutuda, framework farki yok |
| Mesaj | 2-4 byte sabit (orn. `SHOW`) | veri degil, komut |

```csharp
bool createdNew;
var mutex = new Mutex(false, @"Local\VersaCoder.SingleInstance", out createdNew);
if (!createdNew)
{
    NotifyFirstInstance();      // pipe'a "SHOW" yaz -> cik
    return;
}
try { Application.Run(new MainForm()); }
finally { mutex.ReleaseMutex(); mutex.Dispose(); }
```

> **YASAK:** Kilif icine `Application.Run`'dan uzun is koymak (ikinci ornek kilitlenir) · kilidi `finally` disinda birakmak · uyarma mesajina veri/tam yol eklemek (guvenlik).

## §2 Birincinin dinleyicisi

| Adim | Cagri | Not |
|---|---|---|
| Dinleyiciyi ac | `NamedPipeServerStream("VersaCoder.activate")` | `Main`'de, `Run`'dan once |
| Mesaji bekle | `BeginWaitForConnection` + callback | UI thread'i bloklamaz |
| Mesaji al | `Read` -> tam `SHOW` ise eylem | dogrulanmis sabit string |
| Pencereyi one cikar | `BeginInvoke(delegate { ... })` | UI thread'ine gecis |
| Kapat | `Application.ApplicationExit`'te dispose | siza karsi |

> **KURAL:** Pipe mesaji **ham veridir**: uzunluk sinirlanir, sabit degerle karsilastirilir, yorumlanmaz (security-rules).

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `Mutex` (named) | `System.Threading` - kutuda | ayni - kutuda |
| `System.IO.Pipes` | kutuda | ayni |
| Domain reload / statik durum | AppDomain yeniden yuklenir, statik sifirlanabilir | **domain reload yok**; statik durum proces boyunca kalir |
| Binding redirect / resolver | `App.config` redirect gerekir (26.1) | gerekmez |
| C# dili | **7.3** - `out` parametresi `out var` **yasak** | C# 14 (`out var` kullanilabilir - ama ayni dosya 7.3'te) |

`#if` yalnizca **ilk ornek cagrisindaki** dil/paket farkinda; kilif mantigi ortaktir (G6).

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Isimsiz mutex | her kopya "ilk benim" der | adli mutex |
| 2 | Mutex'i `ReleaseMutex` sahibi olmayan thread'de birakmak | `ApplicationException` | ayni thread'de ac + `finally` |
| 3 | Kilifsiz ikinci ornek | ayni anda 2 kabuk, veri catismasi | §1 |
| 4 | Uyarmasiz cikis | kullanici hicbir sey gormez | pipe mesaji + one cikarma |
| 5 | Pipe'a yol/tam veri gondermek | guvenlik | sabit komut |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Kilif nereye konur | [startup-pipeline-ui](startup-pipeline-ui.md) |
| Uyandirilan pencerenin gizlenmesi | [tray-icon-lifecycle](tray-icon-lifecycle.md) |
| Pencereyi one cikarma | [form-navigation-patterns](form-navigation-patterns.md) |
| Log (kilif olayi) | [logging-integration](logging-integration.md) |
