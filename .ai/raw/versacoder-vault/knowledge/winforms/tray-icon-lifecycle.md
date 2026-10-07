---
title: "tray-icon-lifecycle - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "NotifyIcon yasam dongusu: tray'e gizleme vs cikis, sag tik menusu, hayalet ikon temizligi ve net48 + net10.0-windows farklari"
---

# tray-icon-lifecycle — Tray ikonu yasam dongusu (NotifyIcon)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [form-navigation-patterns](form-navigation-patterns.md) · [single-instance-app](single-instance-app.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Tray ikonu ne zaman olusturulur, pencere gizlenince ne olur, uygulama cikarken nasil temizlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`NotifyIcon` **tek ornektir** (pencere basina degil, uygulama basina) ve **sahibi form dispose edildiginde o da dispose edilir**. Pencere X'e basilinca iki davranis tanimlanir: **cikis** (gercekten kapat) veya **tray'e gizle** (`Hide()` + `ShowInTaskbar = false`); ikisi karistirilirsa kullanici "uygulama acik mi kapali mi" diye sorar. Uygulama cikisinda **`Visible = false` yapilmadan dispose edilmemeli** - aksi halde isletim sistemi **hayalet ikon** birakir (kapanana kadar duran gri ikon). Tray menüsü `ContextMenuStrip` ile kurulur; "Cikis" komutu `Application.Exit()` gonderir.

---

## §1 Yasam dongusu

| Asama | Ne yapilir | Ne olmazsa |
|---|---|---|
| Olusturma | `new NotifyIcon { Icon = ..., Visible = true }` | ikon gorunmez |
| Menu | `ContextMenuStrip` bagla (Goster / Cikis) | sag tik boss |
| Gizleme | form `Hide()` + `ShowInTaskbar = false` | pencer kapanmis gibi gorunur |
| Geri acma | tray ikon `MouseDoubleClick` -> `Show()` + `BringToFront()` | kullanici kapali sanir |
| Cikis | `Visible = false` -> `Dispose()` | **hayalet ikon** |

```csharp
void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
{
    this.Show();
    this.ShowInTaskbar = true;
    this.WindowState = FormWindowState.Normal;
    this.BringToFront();
}

protected override void OnFormClosing(FormClosingEventArgs e)
{
    if (e.CloseReason == CloseReason.UserClosing && _minimizeToTray)
    {
        e.Cancel = true;                    // kapatma -> gizleme
        this.Hide();
        this.ShowInTaskbar = false;
        return;
    }
    trayIcon.Visible = false;               // gercek cikis: once gorunurluk
    trayIcon.Dispose();
    base.OnFormClosing(e);
}
```

> **KURAL:** `FormClosing` icinde `CloseReason` kontrol edilmeden `Cancel = true` yapmak, `Application.Exit()`'i de iptal eder ve uygulama **kapatilmaz**.

## §2 Tray menu komutlari

| Komut | Davranis | Not |
|---|---|---|
| Goster | `Show()` + `Normal` | varsayilan cift tik |
| Yeni pencere | `form-navigation-patterns` havuzu | tek ornek korunur |
| Otomatik baslat | ayar yazimi (`settings-user-config`) | kullanici onayi |
| Cikis | `Application.Exit()` | `FormClosing` gecer |
| Hakkinda | modal dialog | `ShowDialog(this)` |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `NotifyIcon` / `ContextMenuStrip` | ayni API | ayni API |
| Ikon kaynagi | `.resx` icindeki `Icon` | ayni |
| Cikis temizligi | `Visible=false` + `Dispose` | ayni - kural degismez |
| DPI / ikon boyu | manifest olcekmeleri | high-DPI varsayilan acik |
| C# dili | **7.3** - `using` deyimi ile dispose | C# 14 |

`#if` gerekmez; fark yok. Tek dikkat: ikon `.resx`'te **`Icon` turunde** ve adi form sinifi ile eslesmeli (DX-05 ruhu).

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Dispose oncesi `Visible=false` unutmak | hayalet ikon | §1 cikis satiri |
| 2 | Her `Show()`'da yeni `NotifyIcon` | yigilan ikon + handle sizintisi | tek ornek |
| 3 | `CloseReason` kontrolsuz `Cancel` | uygulama kapanmaz | §1 ornegi |
| 4 | Tray ikonunu is mantigi ile doldurmak | code-behind sismesi (SOL-02) | menu -> komut -> servis |
| 5 | Tek ornekle cakisan ikinci ikon | kullanici karisir | uygulama basina tek ikon |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Gizlenen pencereyi geri acmak | [form-navigation-patterns](form-navigation-patterns.md) |
| Ikinci ornekten uyarma | [single-instance-app](single-instance-app.md) |
| Menu -> komut bagi | [command-pattern-mvvm](command-pattern-mvvm.md) |
| Ayar (otomatik baslat) | [settings-user-config](settings-user-config.md) |
