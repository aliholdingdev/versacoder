---
title: "form-close-cancel-pattern - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "FormClosing iptali, kaydet-sorma örüntüsü, CloseReason ayrımı, FormClosed ve Dispose sırası"
---

# form-close-cancel-pattern — Kapanışı İptal Etme ve Kaydet-Sorma

**Zorunlu Baglantilar:** [vault index](../../index.md) · [security-rules](../../.rules/security-rules.md) · [winforms index](../index.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-showdialog-result](form-showdialog-result.md)

> **Tek cümlelik sorumluluk:** *Form kapatılırken veri kaybını önleyen iptal örüntüsü nerede kurulur, `FormClosing` ve `FormClosed` arasındaki fark nedir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Kapanışta veri kaybı riski varsa tek adres **`FormClosing`**'dir: `e.Cancel = true` ile kapanış durdurulabilir ve kullanıcıya "Kaydet / Kaydetme / Vazgeç" sorulabilir. `FormClosed` **geri dönüşü olmayan** aşamadır; sadece kalan temizlik (`-=` unsubscribe, `Dispose`) içindir. `CloseReason` okunarak iptal davranışı **yalnız kullanıcı kapatmasında** uygulanır (uygulama kapanışı `Application.Exit` ile sessizce geçilir; kullanıcıyı her kapanışta sıkmak yasaktır). İptal edilen kapanışta arka planda çalışan iş **iptal edilmeye devam eder** — iptal döngüsü `CancellationTokenSource` ile kapatma olayına bağlanır.

---

## §1 Kapanis asamalari

```
Close() / X / Application.Exit
        |
        v
  FormClosing (Cancel = true  ->  kapanis DURUR, form gorunur kalir)
        |  e.CloseReason = UserClosing | ApplicationExitCall | ...
        v
  FormClosed  (geri donus yok; artik olay dinlenemez)
        |
        v
  Dispose()   (designer components + handle; using bunu yapar)
```

| Olay | `Cancel` var mı | Tipik iş | Risk |
|---|---|---|---|
| `FormClosing` | **Evet** | Kaydet-sorma, doğrulanmamış iş uyarısı | İptal ederse temizlik yapılmamış sayılır |
| `FormClosed` | Hayır | `-=` abonelik temizliği, son log | Geç kaldın — iptal yok |
| `Dispose` | Hayır | Kaynak bırakma (image, timer) | Elle çağırmak yerine `using`/`Close()` |

---

## §2 Kaydet-sorma ornegi

```csharp
// DocumentForm.cs — kullanici mantigi; kontrol/SuspendLayout YOK (.Designer.cs)
private bool _dirty;   // degisiklik takibi

private void DocumentForm_FormClosing(object sender, FormClosingEventArgs e)
{
    if (e.CloseReason != CloseReason.UserClosing) return;  // uygulama kapanisi: sorusuz gec
    if (!_dirty) return;

    var cevap = MessageBox.Show(this, "Kaydedilmemiş değişiklik var. Kaydedilsin mi?",
                                "VersaCoder", MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
    if (cevap == DialogResult.Cancel) { e.Cancel = true; return; }   // vazgec -> form acik kalir
    if (cevap == DialogResult.No)     return;                        // kaydetme -> kapan
    e.Cancel = !Kaydet();          // Kaydet() false ise kapanisi IPTAL ET (veri kaybi yok)
}
```

---

## §3 CloseReason tablosu

| `CloseReason` | Kaynak | Kaydet-sorma uygulanır mı |
|---|---|---|
| `UserClosing` | X, ESC, `Close()` kullanıcı eliyle | **Evet** |
| `ApplicationExitCall` | `Application.Exit()` | Hayır (kullanıcı zaten uygulamayı kapatıyor) |
| `MdiFormClosing` | Ana form kapanıyor | Hayır |
| `TaskManagerClosing` | Windows kapatıyor | Hayır |
| `FormOwnerClosing` | Sahip form kapandı | Hayır |
| `WindowsShutDown` | Sistem kapanışı | Hayır (işlemi kısa tut) |

> **UYARI:** İptal dalında `MessageBox` göstermek modal zinciri uzatır; toplu iş (binlerce satır kaydet) `async` ise kapanış olayında senkron blok **yapma** — kapanışı iptal et, işi arka planda bitir ([cancellation-token-in-ui](cancellation-token-in-ui.md)).

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `FormClosingEventArgs.CloseReason` değerleri | **aynı** | aynı |
| `MessageBox` davranışı | **aynı** | aynı |
| Kapanışta kaynak temizliği | `Dispose(bool)` + `components` | aynı |
| Kullanılan dil | C# 7.3 (fonksiyonel `if/return` zinciri) | aynı kod her iki bacakta derlenir |

---

## §5 Kardesler

[form-showdialog-result](form-showdialog-result.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [form-constructor-vs-load](form-constructor-vs-load.md)
