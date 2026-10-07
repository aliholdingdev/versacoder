---
title: "controls-lifecycle-events - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "Control baz sinifinin olaylarini, parent-child olusum ve dispose sirasini, odak/vurgu/desen bozulmalarini ve iki bacak farklarini tanimlar."
---

# Control Olay Hayati

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [coding-standards](../../.rules/coding-standards.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms 4/32):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Bir `Control` hangi olaylari ne sira ile yayar, odak/vurgu sirasi nasil bozulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`Form` bir `Control`'dur; olaylar **cocuk once, parent sonra** (yukari dogru kabarcik) ya da **parent once, cocuk sonra** (asagi dogru) olarak yayilir. En cok karistirilan uc kural: `Enter`/`Leave` odak icindir (`GotFocus` degil), `Validating` **Leave'den once** ve iptal edilebilir, `Paint` her yeniden cizimde tekrar gelir (veri yukleme icin kullanilmaz). Handle yokken `Invoke`/`BeginInvoke` `InvalidOperationException` atar; olay handler'i `async void` olabilir **yalnizca** EventHandler imzasiyla.

```text
+------------------------------------------------------------------+
| odak A -> B : A.Validating -> A.Validated -> A.Leave -> B.Enter  |
| kabarcik    : child.Click -> parent.Click (e.Handled ile durdur)  |
| handle yok  : Invoke / BeginInvoke -> InvalidOperationException    |
+------------------------------------------------------------------+
```

---

## §1 Control olay sirasi

| # | Olay | Ne zaman | Iptal | Not |
|---|---|---|---|---|
| 1 | `HandleCreated` | pencere handle'i olustugunda | - | kaynak / timer bagla |
| 2 | `LocationChanged` / `SizeChanged` | konum olcu degisince | - | `DpiChanged` da tetiklenir |
| 3 | `Enter` / `Leave` | odak giris / cikis | hayir | **odak**, klavye gezinmesi |
| 4 | `Validating` / `Validated` | `Leave` ONCESINDE | **`e.Cancel = true`** | dogrulama |
| 5 | `GotFocus` / `LostFocus` | gercek odak | - | `Enter`/`Leave` kadar sik degil |
| 6 | `Paint` | yeniden cizim | - | **hizli, I/O'suz** olmali |
| 7 | `Disposed` | serbest birakildiktan sonra | - | olaylari unsubscribe |

```text
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
| Olay unsubscribe | `Dispose` / `FormClosed` icinde | aksi halde kapatilmis nesne ayakta |
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

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Enter` / `Leave` / `Validating` | ayni | ayni |
| `Control.Invoke` / `BeginInvoke` | var | var + `Control.InvokeAsync` (`Task` dondurur) |
| `AsyncOperationManager` / `BackgroundWorker` | `System.dll` | `System.ComponentModel.EventBasedAsync` |
| `IDisposable` zinciri | `Dispose(bool)` | ayni |
| Nullable anotasyonu | `EventArgs e` | `EventArgs? e` (NRT) |
| `Task.WaitAsync` / `PeriodicTimer` | **YOK** | var |

`InvokeAsync` **yalniz `net10.0-windows`** bacaginda mevcuttur (`net48` icin eslestirme `BeginInvoke` + `TaskCompletionSource` ile yazilir); ortak kaynaktaki tek guvenli katman `SynchronizationContext` + `IProgress<T>` (her iki bacakta da var).

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

private void OkButton_Validating(object sender, CancelEventArgs e)
{
    if (nameBox.Text.Length == 0) { e.Cancel = true; }
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

private void OkButton_Validating(object? sender, CancelEventArgs e)
{
    e.Cancel = nameBox.Text.Length == 0;
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `sender` tipi | `object` | `object?` | 7.3'te `object` yazmak derlenir |
| Interpolasyon | `$"..."` | ayni | **her iki bacakta da var** |
| `+=` olay baglama | `this.Enter += ...` | `Enter += ...` | ayni anlama |
| `async void` handler | yalniz `EventHandler` | ayni | EventHandler imzasi zorunlu |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `GotFocus` ile odak yonetimi | `Enter`/`Leave` disinda kod, gezinmede kopukluk | `Enter` + `Validating` kullan |
| `Paint` icinde veri / SQL | ekran donmasi, GDI handle sizintisi | is mantagini uygulama katmanina tasi |
| Handle olmadan `Invoke` | `InvalidOperationException` | `IsHandleCreated` kontrolu ya da `SynchronizationContext` |
| Unsubscribe edilmeyen olay | kapatilmis formun handler'i hala cagiriliyor | `FormClosed` / `Dispose` icinde `-=` |
| Olay disinda `async void` | yakalanmayan istisna -> uygulama duser | `async Task` + tam kafes |

---

## §6 Tarama tablosu (kim neyi degistirir)

| Aranacak | `.Form.cs` | Kim sahip |
|---|---|---|
| `this.Enter +=` / `.Validating +=` | baglama `.Designer.cs`'te | `ui-designer` |
| `Validating` govdesi (`e.Cancel`) | `.Form.cs` icinde | `developer` |
| `OnHandleCreated` / `OnHandleDestroyed` | eslesmeli | `developer` |
| `Paint` icinde `Sql` / `await` | 0 eslesme | `reviewer` |
| `-=` unsubscribe | `Dispose` / `FormClosed` icinde | `developer` |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\controls-lifecycle-events.md` (kaynak vault) | olay sirasi, odak ve kabarcik diyagrami, olusum/dispose sirasi, TFM ve dil tablolari, ihlal belirtileri |
| `.rules\coding-standards.md` | olay isleyicisi yazim kurali (EventHandler imzasi) |
| `checklists\winforms-designer-check.md` | olay baglama yerinin denetimi (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
