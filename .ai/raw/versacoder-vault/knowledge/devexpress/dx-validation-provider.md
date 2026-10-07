---
title: "dx-validation-provider - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 DXErrorProvider ile dogrulama: hata ikonu, Validating akisi, MVVM siniri ve iki bacak farki"
---

# dx-validation-provider — DXErrorProvider Doğrulama (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md)

> **Tek cumlelik sorumluluk:** *Alan doğrulaması hangi olayda, hangi sağlayıcıyla gösterilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Alan doğrulaması `DXErrorProvider` ile yapılır: **`ContainerControl` forma bağlanır**, `Validating` olayında hata metni yazılır, boş metin **temizleme** anlamına gelir. Doğrulama **iki katmandır**: View'da anlık geri bildirim (`SetError`), iş kuralı servis/ViewModel'de (`mvvm-pattern-devexpress`). UI'da `e.Cancel = true` ile geçersiz odak **engellenmez**, yalnızca görsel uyarı verilir — asıl kilidi kaydetme butonunun `Enabled` durumu sağlar.

---

## §1 Dogrulama Akisi

```
Kullanici alani birakir (Validating)
        |
   kural saglanir mi? --EVET--> SetError(control, "")  (hata temizlenir)
        |
      HAYIR
        |
   SetError(control, "mesaj")  +  ikon (ErrorIconAlignment)
        |
   Kaydet butonu: tum alanlar temizse Enabled = true
```

| Olay | Ne yapılıyor | Kaçınılır |
|---|---|---|
| `Validating` | kural + `SetError` | iş kuralını burada **çözme** (servis katmanı) |
| `Validated` | temiz bilgi | kaydetme işlemi |
| `EditValueChanging` | canlı maskeleme | kural ile karıştırma |

---

## §2 Anahtar Ozellikler

| API / Ozellik | Ne yapar | Not |
|---|---|---|
| `dxErrorProvider1.ContainerControl` | hangi forma ait | **ilk bağlanacak** |
| `SetError(control, msg)` | hata yaz/temizle (`""` = temiz) | tek çağrı noktası |
| `SetIconAlignment` | ikon konumu | `ErrorIconAlignment.MiddleRight` |
| `IconAlignment` / `ErrorStyle` | ikon stili | skin'e bırak |
| `ClearErrors()` | tümünü temizle | form resetinde |

---

## §3 Kod (olay + durum)

```csharp
using DevExpress.XtraEditors.DXErrorProvider;

dxErrorProvider1.ContainerControl = this;                       // forma bagla
dxErrorProvider1.SetIconAlignment(txtName, ErrorIconAlignment.MiddleRight);

private void txtName_Validating(object sender, CancelEventArgs e)
{
    bool hatali = string.IsNullOrWhiteSpace(txtName.Text);      // kural
    dxErrorProvider1.SetError(txtName, hatali ? "Oturum adi zorunlu" : "");
    btnSave.Enabled = !hatali;                                  // kilitleme
}
```

| Kural türü | Nerede | Not |
|---|---|---|
| Zorunlu / uzunluk / regex | View (`Validating`) | anlık geri bildirim |
| Tekrar eden ad (DB) | servis (Dapper `@p1`) | parametreli sorgu — **G2** |
| Çapraz alan (bitiş > başlangıç) | ViewModel | `DelegateCommand.CanExecute` |

---

## §4 Kurallar (en fazla 5 madde)

1. `ContainerControl` **tek sefer** bağlanır; her olayda tekrar bağlanmaz.
2. Hata metni **`.resx`/kaynaktan** gelir, literal yazılmaz (`localizing-ui-text`).
3. İş kuralı veritabanı kontrolü **ViewModel/serviste** kalır (SOL-02).
4. Yalnız kırmızı ikon ile yetinme: metin + `Tab` sırası erişilebilir (`accessibility-keyboard-nav`).
5. Kaydetmeden önce tüm hatalar `ClearErrors()` ile sıfırlanır (yanlış kilit kalmasın).

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `DXErrorProvider` sınıfı | `DevExpress.XtraEditors` (v26.1 net48) | aynı sınıf (v26.1 net10) |
| `Validating` / `CancelEventArgs` | WinForms standart | aynı |
| İkon hizası | `ErrorIconAlignment` | aynı (DPI'da ikon ölçeği farklı) |
| Dil | **C# 7.3** — `using var`, target-typed `new` **yasak** | C# 14; ortak dosya 7.3 kalır |

> **YASAK:** EF Core ile doğrulama sorgusu (G2), `v26.1` dışı sürüm (DX-02), `Office2010` dışı ribbon (DX-03).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| ViewModel / komut doğrulaması | [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) |
| Klavye/erişilebilirlik | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| Form Designer sınırı | [designer-runtime-customization](designer-runtime-customization.md) |
| Editör gridde | [grid-editing-modes](grid-editing-modes.md) |
