---
title: "UI Wireframe 07 — Settings (Ayarlar)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-09-29
authority: reference
scope: "07-settings ekranının wireframe'i: kategori ağacı, ayar ızgarası, kaydet/sıfırla, dosya üçlüsü, etkileşim kuralları — GATE-003 onayı bekliyor"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 07 — SETTINGS

**Zorunlu Bağlantılar:** [[ui-design/readme|ui-design/index]] · [[ui-design/01-start-page|01-start-page]] · [[ui-design/02-main-ide|02-main-ide]] · [[.rules/configuration-rules|.rules/configuration-rules.md]] · [[.rules/security-rules|.rules/security-rules.md]] · [[governance/keys|keys]]

> **Tek cümlelik sorumluluk:** Ayar penceresinin hangi kategorilerden oluştuğu, değerlerin
> nereye yazıldığı ve gizli alanların nasıl korunduğu sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`07-settings`, `02-main-ide` içinden açılan **ayrı modal diyaloğ**tür (dock bölgesi değildir):
solda kategori ağacı (`TreeList`), sağda ayar ızgarası (`GridControl`, satır = ayar),
altta Kaydet / Sıfırla / Vazgeç. Değerler **servis katmanında** saklanır; formda ham dosya
yazımı yoktur. **Durum: GATE-003 onayı alındı (2026-10-01).**

| Kategori | Örnek ayarlar | Kayıt yeri |
|---|---|---|
| Genel | başlangıç ekranı, dil, görünüm teması | servis (`ISettingsService`) |
| LLM | model, endpoint, timeout | servis; **anahtar gizli** |
| Agent | paralellik, varsayılan görev şablonu | servis |
| Terminal | varsayılan kabuk, satır sarmalama | servis |
| Veritabanı | bağlantı yolu (`VC_DB_PATH`), yedek dizini | servis; açık metin secret **yok** |
| Hakkında | sürüm, bacaklar, lisans | derleme bilgisi (salt okunur) |

---

## §1 Wireframe

```text
+----------------------------------------------------------------------+
| AYARLAR                                              [-] [o] [x]     |
+----------------+-----------------------------------------------------+
| KATEGORI       | AYAR (GridControl)                                  |
| (TreeList)     | +-------------+---------------------+-----------+   |
| v Genel        | | Ayar        | Değer              | Tür       |    |
|   o Ekran      | +-------------+---------------------+-----------+   |
|   o Dil        | | Varsayılan  | [Start Page    v]  | seçim     |    |
| v LLM          | | Model       | [mimo-free     v]  | seçim     |    |
|   o Model      | | Endpoint    | [https://...     ] | metin     |    |
|   o Anahtar    | | Anahtar     | [**************]   | gizli     |    |
|   o Timeout    | | Timeout (s) | [ 30           ]   | sayi      |    |
| v Agent        | | Paralellik  | [ 3            ]   | sayi      |    |
| v Terminal     | | ...(kaydir) |                     |           |   |
| v Veritabani   | +-------------+---------------------+-----------+   |
| v Hakkında     | UYARI: Anahtar yalnız gizli gösterilir (SEC-02)     |
+----------------+-----------------------------------------------------+
| [Kaydet]  [Varsayınlara Dön]  [Vazgeç]        değişiklik: 2 *        |
+----------------------------------------------------------------------+
```

**Durumlar:** değer doğrulama hatası → satırda `!` + tooltip (kod + metin, yalnız renk yok);
gizli alan → gözcü maskeleme + göster/gizle ikonu; kaydetmeden kapatış → **onay** (değişiklik varsa).

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `SettingsDialog.cs` | yükle/kaydet/sıfırla handler'ları, doğrulama, `ISettingsService` çağrıları, gizli alan göster-gizle | `developer` |
| `SettingsDialog.Designer.cs` | `TreeList` kategori + `GridControl` ayar ızgarası + butonlar | **`ui-designer`** |
| `SettingsDialog.resx` | kategori adları, ayar açıklamaları, uyarı metinleri | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Kategori tıklama | ızgara o kategoriye göre **filtrelenir** (yeniden kurulum yok) |
| 2 | Kaydet | doğrulama → servis → yalnız değişen satırlar yazılır |
| 3 | `Varsayınlara Dön` | **onay** → kategori bazında sıfırlama, tek tek değil toplu |
| 4 | Gizli alan (anahtar) | varsayılan maskeli; göster ikonu kalıcı loga **yazmaz** |
| 5 | Esc / Vazgeç | değişiklik yoksa doğrudan kapat; varsa onay |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| Kontroller | `GridControl` + `TreeList` + `ToggleSwitch` her iki bacakta aynı (DevExpress.Win 26.1) |
| Kayıt | formda ham I/O **yok**; `.rules/configuration-rules.md` (ayarlar servisten, ortam değişkeni öncelikli) |
| Güvenlik | anahtar/metin gizli alanlar `.rules/security-rules.md` SEC-02; `.resx`'e secret **konmaz** |
| Erişilebilirlik | kategori↔ızgara `Tab` ile geçiş; hata yalnız renkle **değil** (`!` + açıklama); sayısal alanlarda ok tuşları |

---

## §5 GATE-003 Durumu

| # | Öğe | Durum |
|---|---|---|
| 1 | Wireframe yazıldı | evet (bu dosya) |
| 2 | Kullanıcı onayı | **bekliyor** |
| 3 | Kod izni | kapalı |

---

## §6 Kanıt

| # | Kaynak | Ne kanıtlar |
|---|---|---|
| 1 | Kaynak wireframe `ui-design/07-settings.md` (eski vault) | kategori/ızgara düzeni |
| 2 | [[.rules/configuration-rules|.rules/configuration-rules.md]] | ayar saklama hiyerarşisi |
| 3 | [[.rules/security-rules|.rules/security-rules.md]] SEC-02 | gizli alan kuralı |
| 4 | [[governance/keys|keys.md]] | anahtar meta haritası (değer değil) |

> `ISettingsService` adı taslaktır — `l0` içinde kesinleşmesi `DOĞRULAMA GEREKLİ`.
> `VC_DB_PATH` yalnız **ad** örneğidir; gerçek değer/secret hiçbir `.md`/`.resx` dosyasında tutulmaz.
