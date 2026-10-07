---
title: "Control Olay Hayati - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Control baz sinif olaylari: HandleCreated, Enter/Leave, Paint, SizeChanged, Validating; parent-child olusum ve dispose sirasi; iki bacak (net48 / net10.0-windows) farklari"
---

# Controls Lifecycle Events

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [form-lifecycle](form-lifecycle.md) · [validation-form-level](validation-form-level.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bir `Control` hangi olaylari ne sira ile yayar, odak/vurgu/desen sirasi nasil bozulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`Form` bir `Control`'dur; olaylar **cocuk once, parent sonra** (yukari dogru kabarcik) ya da **parent once, cocuk sonra** (asagi dogru) olarak yayilir. En cok karistirilan uc kural: `Enter`/`Leave` odak icindir (`GotFocus` degil), `Validating` **Leave'den once** ve iptal edilebilir, `Paint` her yeniden cizimde tekrar gelir (veri yukleme icin kullanilmaz). Handle yokken `Invoke`/`BeginInvoke` `InvalidOperationException` atar; olay handler'i `async void` olabilir **yalnizca** EventHandler imzasiyla (CS-06).

---

## §1 Control olay sirasi

| # | Olay | Ne zaman | Iptal | Not |
|---|---|---|---|---|
| 1 | `HandleCreated` | pencere handle'i olustugunda | - | kaynak/timer bagla |
| 2 | `LocationChanged` / `SizeChanged` | konum/olcu degisince | - | `DpiChanged` da tetiklenir |
| 3 | `Enter` / `Leave` | odak giris/cikis | hayir | **odak**, klavye gezinmesi |
| 4 | `Validating` / `Validated` | `Leave` ONCESINDE | **`e.Cancel = true`** | dogrulama (bkz. [validation-form-level](validation-form-level.md)) |
| 5 | `GotFocus` / `LostFocus` | gercek odak | - | `Enter`/`Leave` kadar sik degil |
| 6 | `Paint` | yeniden cizim | - | **hizli, I/O'suz** olmali |
| 7 | `Disposed` | serbest birakildiktan sonra | - | olaylari unsubscribe |

```
[odak A'dan B'ye]
   A.Validating --(Cancel?)--> geri don
   A.Validated
   A.Leave
   B.Enter
   B.GotFocus

[cocuk -> parent kabarcigi]
   child.Click -> parent.Click (e.Handled / e.SuppressKeyPress ile durdur)
```

---

## §2 Olusum ve Dispose sirasi

| Asama | Sirasi | Neden onemli |
|---|---|---|
| `Controls.Add` | child **once** olusturulur, **sonra** parent'a eklenir | `Parent` nullken layout olmaz |
| `Dispose` | **child once**, parent sonra (`components.Dispose()` yardimiyla) | ters sira `ObjectDisposedException` |
| Olay unsubscribe | `Dispose`/`FormClosed` icinde | aksi halde kapatilmis nesne ayakta |
| Handle'a bagli kaynak | `HandleCreated` -> `HandleDestroyed` eslesmeli | `WndProc` override'lari |

```csharp
protected override void OnHandleCreated(EventArgs e)
{
    base.OnHandleCreated(e);
    _hook = NativeMethods.InstallHook(Handle);   // handle'a bagli
}

protected override void OnHandleDestroyed(EventArgs e)
{
    if (_hook != IntPtr.Zero) { NativeMethods.RemoveHook(_hook); _hook = IntPtr.Zero; }
    base.OnHandleDestroyed(e);
}
```

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Enter`/`Leave`/`Validating` | ayni | ayni |
| `Control.Invoke` / `BeginInvoke` | var | var + `Control.InvokeAsync` (Task dondurur) |
| `AsyncOperationManager` / `BackgroundWorker` | `System.dll` | `System.ComponentModel.EventBasedAsync` |
| `IDisposable` zinciri | `Dispose(bool)` | ayni |
| Nullable anotasyonu | `EventArgs e` | `EventArgs? e` (NRT) |
| `Task.WaitAsync` / `PeriodicTimer` | **YOK** | var |

> **Not:** `InvokeAsync` **yalniz `net10.0-windows`** bacaginda mevcuttur (NEEDS_VALIDATION: `net48` icin eslestirme `BeginInvoke` + `TaskCompletionSource` ile yazilir); ortak kaynaktaki tek guvenli katman `SynchronizationContext` + `IProgress<T>` (her iki bacakta da var).

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private Button okButton;

public MainForm()
{
    InitializeComponent();
    this.Enter += MainForm_Enter;
    okButton.Validating += OkButton_Validating;
}

private void MainForm_Enter(object sender, EventArgs e)
{
    EventArgs args = e;
    this.Text = string.Format("{0} - {1}", ProductName, args.GetType().Name);
}

private void OkButton_Validating(object sender, CancelEventArgs e)
{
    if (nameBox.Text.Length == 0)
    {
        e.Cancel = true;
    }
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
private readonly Button okButton = null!;

public MainForm()
{
    InitializeComponent();
    Enter += MainForm_Enter;
    okButton.Validating += OkButton_Validating;
}

private void MainForm_Enter(object? sender, EventArgs e)
{
    Text = $"{ProductName} - {e.GetType().Name}";
}

private void OkButton_Validating(object? sender, CancelEventArgs e)
{
    e.Cancel = nameBox.Text.Length == 0;
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `sender` tipi | `object` | `object?` | 7.3'te `object` yazmak derlenir |
| Interpolasyon | `$"..."` 6.0+ | ayni | **her iki bacakta da var** |
| `+=` olay baglama | `this.Enter += ...` | `Enter += ...` | ayni anlama |
| `async void` handler | yalniz `EventHandler` | ayni | `coding-standards.md` CS-06 |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `GotFocus` ile odak yonetimi | `Enter`/`Leave` disinda kod, gezinmede kopukluk | `Enter` + `Validating` kullan |
| `Paint` icinde veri/SQL | ekran donmasi, GDI handle sizintisi | is mantigini `Application` katmanina tasi |
| Handle olmadan `Invoke` | `InvalidOperationException` | `IsHandleCreated` kontrolu ya da `SynchronizationContext` |
| Unsubscribe edilmeyen olay | kapatilmis formun handler'i hala cagiriliyor | `FormClosed` / `Dispose` icinde `-=` |
| `async void` disinda async void | yakalanmayan istisna -> uygulama patlar | `async Task` + CS-06 |

> **Iliskili:** form seviyesi siralar icin [form-lifecycle](form-lifecycle.md) · odak + dogrulama zinciri icin [validation-form-level](validation-form-level.md) · UI thread'e donus icin [async-ui-thread-marshalling](async-ui-thread-marshalling.md).
