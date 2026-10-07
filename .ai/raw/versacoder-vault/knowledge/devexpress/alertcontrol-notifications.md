---
title: "AlertControl Bildirimleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "AlertControl ile kosan uyari pencereleri: gosterme, tiklama verisi, ozel butonlar ve HTML sablonu"
---

# AlertControl Bildirimleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [splashscreen-waitform](splashscreen-waitform.md) · [ribbon-form-basics](ribbon-form-basics.md)

> **Tek cumlelik sorumluluk:** *Bilgi/uyari penceresi MessageBox yerine nasil gosterilir ve tiklanan veri nasil geri alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`AlertControl` (`DevExpress.XtraBars.Alerter`) ekranda kayan, kendi kendine kaybolan bildirim penceresi gosterir; `Show(...)` ile bir kere cagrilir, kapatma isini kontrol kendisi yapar. Her uyarinin `Tag` alani oldugu icin **tiklanan bildirimin verisi** `AlertClick` olayindan geri alinir - MessageBox'a dusmeden ayni is akisi korunur. Uyari icerigi HTML/CSS sablonuyla bicimlenebilir, `Buttons` koleksiyonu ile icerisine ozel buton konabilir. `MessageBox.Show` yerine kullanmak DX-11 ile uyumludur; is mantigi yine handler'da kalir.

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Alerter;

alertControl1.Show(this, "Kaydedildi", "Oturum kaydedildi.", "", null, sessionId);

private void alertControl1_AlertClick(object sender, AlertClickEventArgs e)
{
    string id = e.Info.Tag as string;      // tiklanan uyarinin verisi
    OpenSession(id);
}
```

---

## §2 Ozellik / olay tablosu

| Üye | Tur | Ne is yapar |
|---|---|---|
| `AlertControl.Show(owner, caption, text, html, image, tag)` | method | Uyariyi gosterir; `tag` geri donus verisidir |
| `AlertClick` | olay | Uyariye tiklanir; `e.Info.Tag` veriyi tasir |
| `BeforeFormShow` | olay | Gosterilmeden once ince ayar (`e.AlertForm.OpacityLevel`) |
| `AlertControl.Buttons` | koleksiyon | Uyari icine ozel butonlar |
| `ButtonClick` | olay | Ozel butona basilinca |
| HTML/CSS sablonu | icerik | Uyari govdesini HTML ile bicimleme |
| `AlertContainerControl` | kontrol | Uyariyi form **icinde** sabit alanda tutmak → `NEEDS_VALIDATION` |
| Form gecikme/konum ozellikleri | property | Ne sure gosterilecegi, hangi kose → `NEEDS_VALIDATION` |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Referans | `DevExpress.XtraBars.v26.1` (paket `DevExpress.Win.Navigation`) | Ayni paket |
| `Show(...)` ilk parametre | `this` (Form) | Ayni |
| `Tag` veri tipi | `object`; `as` ile indirger | Ayni; `Nullable=enable` ise `as` sonrasi null kontrolu |
| HTML render | Gomulu HTML motoru (framework'e bagli degil) | Ayni motor |
| `#if` | Yalniz platform farkinda | Ayni; bildirim kodu #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Is kuralini `AlertClick` icine gommek | Test edilemez, god handler | `OpenSession(id)` → `Application` katmani |
| 2 | `Tag`'i null birakip sonra `as` ile okumak | Anlam verisiz tiklama | Her `Show`'a anlamli `tag` ver |
| 3 | `MessageBox.Show` ile ayni anda kullanmak | Cifte bildirim, kafa karisikligi | Tek kanal: AlertControl |
| 4 | Uyariyi ana is parcacigindan cagirmak | UI thread disi cagri | UI thread'de `Show` |
| 5 | Gorsel/metni kodda sabitlemek | Yerellestirme kaybi | Metin resx/kaynak |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `Show(...)` imzasi, `AlertClick` / `BeforeFormShow` / `Buttons` | v26.1 WinForms dokumantasyonu ile **dogrulandi** |
| `AlertContainerControl` ve gecikme/konum property adlari | `NEEDS_VALIDATION` |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-11 (gosterge is mantigi disinda), DX-13 (ince handler) |

**Kardesler:** [splashscreen-waitform](splashscreen-waitform.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-application-menu](ribbon-application-menu.md) · [tile-control-startpage](tile-control-startpage.md)
