---
title: "Ayarlar Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Ayarlar ekranı: profil/tercih grupları, sağlayıcı-model seçimi, secret girişi (şifreli), doğrulama ve kaydetme akışı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Ayarlar Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/persistence-spec|persistence]] · [[spec/error-handling-spec|error-handling]] · [[spec/localization-spec|localization]]

> **Tek cümlelik sorumluluk:** *Hangi ayar var, nasıl doğrulanır, secret nasıl girilir ve değişiklik ne zaman kalıcı olur* sorusunun tek cevabıdır.
> Ayarların **nerede saklandığı** [[spec/persistence-spec|persistence]]’in; şifrelemenin anahtarı mimarinin konusudur.

---

## §0 Özet (Answer First)

`SettingsForm` (`LayoutControl` + `DataLayoutControl` alt gruplar) 4 sekmeden oluşur: **Provider**, **Tools**, **Appearance**, **Profile**. Secret alanları `TextEdit` (şifreli görünüm) ile girilir, **asla düz metin ekran/geri dönüşte görünmez** (SEC-08); kaydet tıklanınca `IStringProtector` ile korunup yazılır. Kaydet/Kapat ayrımı vardır: kapat = değişiklikleri geri al. Alan bazlı doğrulama geçmezse kaydetme butonu **pasif** kalır (VC-PRD-062).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Ayar alanları, grupları, varsayılanları | Kalıcılık yolu/formatı | [[spec/persistence-spec|persistence]] |
| 2 | Secret girişi UI'ı (maskeli, kopyala/sıfırla) | Şifreleme algoritması/anahtarı | [[spec/error-handling-spec|error-handling]] notu + mimari |
| 3 | Doğrulama mesajları ve kaydet engeli | Hata kodu üretimi | [[spec/error-handling-spec|error-handling]] |
| 4 | Tema/dil tercihi **gösterimi** | Tema uygulaması / çeviri kaynakları | [[spec/theming-spec|theming]] · [[spec/localization-spec|localization]] |
| 5 | Ayar değişikliğinin etkili olma zamanı | Sağlayıcı çağrısı (l4) | [[spec/ai-chat-spec|ai-chat]] |

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
