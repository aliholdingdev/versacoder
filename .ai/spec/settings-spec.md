---
title: "Ayarlar Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Ayarlar ekranı: profil/tercih grupları, sağlayıcı-model seçimi, secret girişi (şifreli), doğrulama ve kaydetme akışı"
---

# Ayarlar Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [persistence](persistence-spec.md) · [error-handling](error-handling-spec.md) · [localization](localization-spec.md)

> **Tek cümlelik sorumluluk:** *Hangi ayar var, nasıl doğrulanır, secret nasıl girilir ve değişiklik ne zaman kalıcı olur* sorusunun tek cevabıdır.
> Ayarların **nerede saklandığı** [persistence](persistence-spec.md)’in; şifrelemenin anahtarı mimarinin konusudur.

---

## §0 Özet (Answer First)

`SettingsForm` (`LayoutControl` + `DataLayoutControl` alt gruplar) 4 sekmeden oluşur: **Provider**, **Tools**, **Appearance**, **Profile**. Secret alanları `TextEdit` (şifreli görünüm) ile girilir, **asla düz metin ekran/geri dönüşte görünmez** (SEC-08); kaydet tıklanınca `IStringProtector` ile korunup yazılır. Kaydet/Kapat ayrımı vardır: kapat = değişiklikleri geri al. Alan bazlı doğrulama geçmezse kaydetme butonu **pasif** kalır (VC-PRD-062).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Ayar alanları, grupları, varsayılanları | Kalıcılık yolu/formatı | [persistence](persistence-spec.md) |
| 2 | Secret girişi UI'ı (maskeli, kopyala/sıfırla) | Şifreleme algoritması/anahtarı | [error-handling](error-handling-spec.md) notu + mimari |
| 3 | Doğrulama mesajları ve kaydet engeli | Hata kodu üretimi | [error-handling](error-handling-spec.md) |
| 4 | Tema/dil tercihi **gösterimi** | Tema uygulaması / çeviri kaynakları | [theming](theming-spec.md) · [localization](localization-spec.md) |
| 5 | Ayar değişikliğinin etkili olma zamanı | Sağlayıcı çağrısı (l4) | [ai-chat](ai-chat-spec.md) |

---

## §2 Davranış

### §2.1 Sekme ve alan envanteri

| # | Sekme | Alan | Kontrol | Doğrulama |
|---|---|---|---|---|
| 1 | Provider | sağlayıcı tipi | `LookUpEdit` | boş olamaz |
| 2 | Provider | endpoint URL | `TextEdit` | `http(s)://` ön eki |
| 3 | Provider | API key | `TextEdit` (maskeli) | ≥ 8 karakter; **yalnız yazılır, okunmaz** |
| 4 | Provider | model adı | `ComboBoxEdit` | boş olamaz |
| 5 | Tools | tool izin varsayılanı | `ToggleSwitch` + `CheckedComboBoxEdit` | — |
| 6 | Tools | timeout (sn) | `SpinEdit` | 1..600 |
| 7 | Appearance | skin | `LookUpEdit` | listedeki değerlerden |
| 8 | Profile | kullanıcı adı, dil | `TextEdit` / `LookUpEdit` | dil = mevcut `.resx` dilleri |

### §2.2 Akış

```
+--------------+   alan değiştir   +------------------+
|  SettingsForm| ----------------► | "kaydedilmedi"   |
|  (modal)     |                   | işareti + Save●  |
+------+-------+                   +------------------+
       | Save (doğrulama geçerse)        | Close (onay)
       ▼                                 ▼
  secret -> IStringProtector        degisiklikleri geri al
       |                                 |
       ▼                                 ▼
  SQLite (ADO.NET + Dapper)          form kapanar
  + bir sonraki gorevde gecerli
```

| # | Kural | Kabul |
|---|---|---|
| V1 | Doğrulama başarısızsa Kaydet **pasif**; hata ilgili alanın altında | boş URL ile kaydetme = 0 |
| V2 | Secret alanı: yazma anında maskeli; geri okumada yalnız `••••` + "Değiştir/Sıfırla" | UI'da düz metin secret = 0 (SEC-08) |
| V3 | Kapat = iptal; onay dialoğu "kaydedilmemiş değişiklik var" | kayıpsız kapatma |
| V4 | Kaydet → etki **bir sonraki** görevde geçerli; çalışan görev durmaz | çalışma görevi yarıda kesilmez |
| V5 | Ayar bozuksa uygulama **açılır**, varsayılan kullanılır + uyarı | kilitli açılış yok |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Gizleme (password) | `UseSystemPasswordChar` | aynı | **fark yok** |
| Kripto koruma | BCL AES (l3 `IStringProtector`) | aynı | fark yok |
| DPI/ölçek | DevExpress otomatik | otomatik | fark yok |

> Form kodu `#if` içermez (G6 hedefi: 0); `#if` yalnız platform farkında ve bu formda yoktur.

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Doğrulama matrisi (URL, timeout aralığı) | birim test | geçersiz girdi = kaydet engeli |
| 2 | Secret maskeleme | UI smoke + kod taraması | ekranda düz metin = 0 |
| 3 | Kapat = geri alma | birim test (değişiklik sayacı) | form verisi sıfırlanır |
| 4 | Kalıcılık turu | SQLite entegrasyon | yeniden açılışta değerler aynı |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
