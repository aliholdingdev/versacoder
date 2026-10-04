---
title: "async-void-event-handlers - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "async void metotlarinin yalniz nerede kullanildigini, kacinan istisnalarin nereye gittigini ve her olay isleyicisinde kurulmasi gereken hata kafesini tanimlar."
---

# `async void` Olay Isleyicileri ve Hata Yuzeyi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [logging-integration](logging-integration.md)

> **Tek cumlelik sorumluluk:** *`async void` metot nerede serbesttir, nerede kafes zorunludur ve yakalanmayan istisna kime gider* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`async void` **yalniz** WinForms olay isleyicisinde ve **yalniz** tam kapsamli `try/catch/finally` ile kullanilir. Kapsanmayan bir `async void` istisnasi `SynchronizationContext.Post` uzerinden **uygulamayi dusurebilir**; bu yuzden her `async void` metot kendi kafesine sahiptir ve ayrica global yakalayiciya haber verir.

```text
+-----------------------------------------------------------------+
| async void  : yalniz olay isleyicisi + TAM try/catch/finally     |
| async Task  : servis ve uc metot (await edilebilir, test edilir) |
| kafesiz kacis -> SynchronizationContext.Post -> uygulama duser   |
+-----------------------------------------------------------------+
```

---

## §1 Metot turune gore karar

| # | Metot turu | Donus | Kural |
|---|---|---|---|
| 1 | Olay isleyici (`Click`, `Load`, `Shown`) | `async void` | zorunlu `try/catch/finally` |
| 2 | Servis / uc metot | `async Task` | `await` edilebilir, test edilebilir |
| 3 | `override` metot | `async Task` (mumkunse) | `async void` yalniz `OnXxx` istisnasi |
| 4 | Yapici (constructor) | **asla** `async` | `async` kurucu yoktur |
| 5 | `IDisposable.Dispose` | **asla** `async` | `FormClosed` icinde `await` ayri metotta |

---

## §2 Zorunlu kafes kalibi

```csharp
private async void btnRun_Click(object sender, EventArgs e)
{
    try
    {
        await RunAsync();
    }
    catch (OperationCanceledException)
    {
        // iptal hata degildir - yalniz durum satiri guncellenir
        status.Caption = "Iptal edildi.";
    }
    catch (Exception ex)
    {
        _errors.Report(ex);                     // hata yuzeyi tek yerde
        MessageBox.Show(this, ex.Message, "Hata",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnRun.Enabled = true;                  // UI her kosulda toparlanir
    }
}
```

### §2.1 Hata yuzeyi karari

| # | Hata tipi | Ne yapilir | Nerede gosterilir |
|---|---|---|---|
| 1 | Kullanici girdisi hatali | dogrula, tekrar dene | `ErrorProvider` (alan ustunde) |
| 2 | Beklenen is kurali ihlali | kullaniciya aciklanir | `MessageBox` / durum cubugu |
| 3 | Altyapi (DB, ag, dosya) | raporlanir + gunluge yazilir | hata diyalogu + `ILogger` |
| 4 | `OperationCanceledException` | **hata sayilma** | durum cubugu |
| 5 | Bilinmeyen / `NullReference` | `UNKNOWN` etiketi ile rapor | global yakalayici |

---

## §3 Kacis yollari ve tarama tablosu

| # | Yol | Sonuc | Kapatma |
|---|---|---|---|
| 1 | `async void` icinde `catch` yok | uygulama dusabilir | her isleyicide kafes |
| 2 | `Task.Run` icinde firlatilip `await` edilmeyen task | sessiz kayip | her `Task` `await` edilir |
| 3 | `ContinueWith` icinde istisna | sessiz | `await` tercih et |
| 4 | Timer / olay icinde async | kafes yok | isleyicide kafes |
| 5 | `FormClosed` sonrasi UI'a yazma | `ObjectDisposedException` | iptal + durum kontrolu |

```powershell
# Kafesiz async void taramasi
Select-String -Path "src\**\*.cs" -Pattern "private async void" -Context 0,4
# Beklenen: her eslesmenin ardindan try { satiri gelir
```

---

## §4 Istisna: yutmak mi, yeniden firlatmak mi

| Durum | Davranis |
|---|---|
| Kullaniciya gosterilebildi | `catch` + rapor + **yut** (uygulama yasar) |
| Gosterilemedi ama loga yazildi | `catch` + rapor + **yut** |
| Programci hatasi, yakalanmali | `catch` + rapor + **yeniden firlat** (global yakalayici gorsun) |
| Asla | `catch { }` - bos kafes (nedensiz sessizlik) |

---

## §5 Kim neyi degistirir

| Kim | Ne yapar | Ne yapmaz |
|---|---|---|
| `developer` | olay isleyicisi govdesine kafes ve rapor satiri yazar | servis metodunu `async void` yapmak |
| `ui-designer` | olay `+=` bagini `.Designer.cs` icine koyar | `async`/`await` kodu yazmak |
| `reviewer` | bos `catch { }` ve kafesiz `async void` tarar | hata gosterim politikasini tek basina degistirmek |
| `tester` | hata yolunu (ag yok, iptal) sinar | uretim kodunda log seviyesini oynatmak |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Her `async void` icinde `try/catch/finally` var | §3 tarama komutu ciktisi |
| 2 | Bos `catch { }` yok | grep hedefi: 0 eslesme |
| 3 | Hata gosterimi tek yuzeyde toplanmis | kod incelemesi |
| 4 | Iptal `OperationCanceledException` ayri ele aliniyor | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\async-void-event-handlers.md` (kaynak vault) | metot turu karari, kafes kalibi, hata yuzeyi, kacis yollari, yutma/firlatma tablosu |
| `.rules\error-handling-rules.md` | yakalama ve hata gosterim kurali |
| `.rules\coding-standards.md` | bos catch yasagi, hata siniflandirmasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
