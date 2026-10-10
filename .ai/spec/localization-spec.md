---
title: "Yerelleştirme Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Dil/çeviri yaklaşımı, kaynak yönetimi (.resx + ResourceManager), anahtar kuralları ve kapsam notu"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Yerelleştirme Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/settings-spec|settings]] · [[spec/error-handling-spec|error-handling]]

> **Tek cümlelik sorumluluk:** *UI metinleri nerede yaşar, nasıl okunur ve dil değiştirme ne zaman (kapsam dışı) sayılır* sorusunun tek cevabıdır.
> **Kapsam notu:** çok dilli arayüz **açık kapsam dışıdır** (`versacoder-spec.md` §6); bu şartname yalnız kaynak **düzenini** şart koşar.

---

## §0 Özet (Answer First)

Tüm kullanıcıya görünen sabit metin `.Form.resx` içinde yaşar ve `ResourceManager` ile okunur (designer üçlüsünün parçası). Anahtarlar **İngilizce**, varsayılan değerler **Türkçe**'dir (NC-18). Dynamik mesajlar (hata/görev) şablon üzerinden üretilir; metin **kod içinde birleştirilmez**. Dil çevirisi (TR/EN/…) **kapsam dışıdır**; ek dil eklemek yalnız `.resx` satellite dosyası ile gelecekte yapılabilir.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | `.resx` + `ResourceManager` kullanımı ve adlandırma | Gerçek çeviri metinleri / dil paketleri | kapsam dışı (versacoder-spec §6) |
| 2 | Anahtar/adlandırma kuralları (NC-18) | Ayar ekranındaki dil seçimi davranışı | [[spec/settings-spec|settings]] |
| 3 | Dinamik mesaj şablonları (hata, görev) | Hata kodu üretimi/seviyesi | [[spec/error-handling-spec|error-handling]] |
| 4 | Designer üçlüsü ile senkron (`Form.cs/.Designer.cs/.resx`) | Form yerleşimi | [[spec/ui-shell-spec|ui-shell]] |
| 5 | Eksik anahtar denetimi (build/test) | Ölçüm/monitoring | [[spec/observability-spec|observability]] |

---

## §2 Davranış

### §2.1 Kaynak yerleşimi

```
 VersaCoder.Desktop/
 +-- Forms/
 |   +-- MainForm.cs · MainForm.Designer.cs · MainForm.resx
 |   +-- SettingsForm.cs · SettingsForm.Designer.cs · SettingsForm.resx
 +-- Resources/   Strings.resx   (varsayilan: Turkce degerler)
 +-- (gelecek)    Strings.tr.resx · Strings.en.resx   <- satellite (KAPSAM DISI)
```

### §2.2 Kurallar

| # | Kural | Kabul |
|---|---|---|
| R1 | Form metni `.resx`'ten gelir; `.cs` içinde sabit kullanıcı metni **yok** | kod taraması: ekranda görünür sabit dize = 0 |
| R2 | Anahtar adı `İngilizce`, noktalı: `settings.save`, `task.running` | tutarsız anahtar = 0 |
| R3 | Dinamik metin **şablondur** (`{0}`/`{1}`); birleştirme (`+`) yasak | string concat = 0 (G2 ruhu, okunabilirlik) |
| R4 | `ResourceManager` anahtarı **elle değiştirilmez** (designer üretir) | `ResourceManager` bozuk başvuru = 0 (G3/G4) |
| R5 | Eksik anahtar testte yakalanır | test: her anahtar varsayılan dilde dolu |

### §2.3 Kapsam notu (dil değiştirme)

| # | Durum | Karar |
|---|---|---|
| N1 | UI dili değişimi | **KAPSAM DIŞI** — açık karar, kullanıcı onayı ister (kapsam yetkisi kullanıcıda) |
| N2 | Yeni `.resx` dil ekleme | gelecekte tek satırlık satellite ile mümkündür; bugün üretilmez |
| N3 | Araç/LLM çıktısı dili | sağlayıcıya bağlıdır; sabitlenmez |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| `ResourceManager` | BCL `System.Resources` | aynı | **fark yok** |
| Satellite derleme | `NeutralResourcesLanguage` + derleyici | aynı | fark yok |
| Designer/resx | DevExpress v26.1 designer | aynı | fark yok |

> `#if` yok; kaynak okuma her iki bacakta aynıdır (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Eksik anahtar taraması | birim test (anahtar kümesi) | 0 eksik |
| 2 | Şablon yer tutucu sayısı | birim test | `{0}`=1 ↔ argüman=1 |
| 3 | `ResourceManager` bütünlüğü | derleme + smoke | eksik kaynak = 0 hata |
| 4 | Kod içinde sabit metin | statik tarama | ihlal = 0 (NC-18) |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
