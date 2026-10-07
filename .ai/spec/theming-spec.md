---
title: "Tema Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Tema + ribbon stili (yalnız Office2010), koyu/acik tercihi ve özel renk kapsam notu"
---

# Tema Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [settings](settings-spec.md) · [ui-shell](ui-shell-spec.md) · [localization](localization-spec.md)

> **Tek cümlelik sorumluluk:** *Hangi görünüm (skin) ve ribbon stili kullanılır, koyu/açık tercihi nasıl uygulanır, neler bilinçli kapsam dışıdır* sorusunun tek cevabıdır.
> **Değiştirilemez:** `RibbonControlStyle.Office2010` (ADR-004 / G3). Bu satırı kıran kod RED'dir.

---

## §0 Özet (Answer First)

Ribbon stili **her koşulda** `RibbonControlStyle.Office2010`'dur; kullanıcıya sunulan tek değişken DevExpress **skin**'idir (`UserLookAndFeel.Default`). Varsayılan skin **Office 2010 benzeri (açık)**; koyu tercih ayrı bir skin ile karşılanır. **Kapsam notu:** kendi paletini üretmek (özel renk/skin derlemek) ve kullanıcı tanımlı CSS benzeri tema **açık kapsam dışıdır**; önce mevcut skinlerle yetinilir, talep gelirse ADR açılır (G8).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Ribbon stili kilidi (`Office2010`) | Panel yerleşimi/kabuk | [ui-shell](ui-shell-spec.md) |
| 2 | Skin seçimi (açık/koyu) ve uygulama anı | Ayarın kaydedilmesi | [settings](settings-spec.md) · [persistence](persistence-spec.md) |
| 3 | Yeni form/bağlantının mevcut görünümü alması | Her ekranın özel renk kodu | kardeş şartnameler |
| 4 | Erişilebilirlik kontrast gerekliliği (DX-14) | Kontrast ölçümü/metriği | [observability](observability-spec.md) |
| 5 | Kapsam dışı: özel skin/derleme, palet editörü | — | karar = kullanıcı + ADR (G8) |

---

## §2 Davranış

### §2.1 Değişmezler ve değişkenler

| # | Öğe | Değer | Değişebilir mi? |
|---|---|---|---|
| 1 | Ribbon stili | `RibbonControlStyle.Office2010` | **HAYIR** (G3/ADR-004) |
| 2 | Kontrol kütüphanesi | DevExpress Universal **v26.1** | **HAYIR** (G8) |
| 3 | Skin (açık/koyu) | kullanıcı tercihi | EVET |
| 4 | Yazı boyutu/DPI | sistem ölçeği + DevExpress otomatik | EVET (yalnız büyütme) |
| 5 | Özel palet | — | **KAPSAM DIŞI** (ADR ister) |

### §2.2 Uygulama akışı

```
 ayar (Appearance.skin)
        |
        ▼
 UserLookAndFeel.Default.SetSkinStyle(...)
        |
        +-- RibbonControl      -> Office2010 kalır (stil ≠ skin)
        +-- DockManager / Grid / TreeList / MemoEdit  -> skin'i miras alır
        +-- Yeni açılan Form   -> Default'tan miras (tek kayıt yeri)
```

| # | Kural | Kabul |
|---|---|---|
| T1 | Skin değiştirme **anında** uygulanır; yeniden başlatma gerekmez | form yeniden çizimi 1 geçiş |
| T2 | Skin tercihi kalıcıdır ([persistence](persistence-spec.md)) | yeniden açılışta aynı görünüm |
| T3 | Bilinmeyen skin adı → varsayana düşer + uyarı | kilitli açılış yok |
| T4 | Tüm formlar `UserLookAndFeel.Default`'tan miras alır | forma özel skin = 0 |
| T5 | Ribbon stili hiçbir yerde **kodla değiştirilemez** | `RibbonControlStyle.` yalnız `Office2010` |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Skin motoru | DevExpress **v26.1** | aynı sürüm | **fark yok** |
| Ribbon stili | `Office2010` | `Office2010` | fark yok |
| Koyu skin kullanılabilirliği | **`NEEDS_VALIDATION`** (OQ-4 ile ilgili) | `NEEDS_VALIDATION` | ilk build'de doğrulanır |

> Doğrulanmamış madde işaretlidir (G9); "koyu skin her iki bacakta da var" iddiası **kanıtsız yazılmaz**.

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Ribbon stili kilidi | kod taraması + smoke | `Office2010` dışı = 0 |
| 2 | Skin geçişi anlık | smoke (açık ↔ koyu) | yeniden başlatmasız |
| 3 | Kalıcılık | yeniden açılış testi | tercih korunur |
| 4 | Bilinmeyen skin | birim test | varsayana + uyarı |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
