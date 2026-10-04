---
title: "VersaCoder — UI Design Raporu (PART 4 / GATE-003)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "PART 4 wireframe durumu: ui-design/ 8 dosya envanteri, ekran adi sozlesmesi, format filtresi ve GATE-003 kullanici onay kutusu"
---

# VersaCoder — UI Design Raporu (PART 4)

**Zorunlu Baglantilar:** [index](index.md) · [09-teslim-raporu](09-teslim-raporu.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [ui-design/README](../ui-design/README.md) · [gate-003-ui-onay](../checklists/gate-003-ui-onay.md)

> **Tek cumlelik sorumluluk:** *7 wireframe diskte tam mi, ekran adlari sozlesmeye uyuyor mu ve GATE-003 onayi nerede verilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**`ui-design/` klasoru 8/8 tamam: 1 indeks (README) + 7 wireframe; hepsi diskte ve olcumu yapildi (2026-09-30).** Ekran adlari sozlesme ile birebir ayni: `01-start-page` .. `07-settings`. Kaynak vault'ta raporlanan "README farkli 5 ekran adi yaziyor" celiskisi bu vault'ta kapatilmis durumda: README §0'da **"dosya adlari esastir (ADR-002)"** karari var, eski liste `DOĞRULAMA GEREKLI` notuyla tasiniyor. **GATE-003 BEKLIYOR** — [gate-003-ui-onay](../checklists/gate-003-ui-onay.md) icindeki 13 sorunun hici isaretlenmemis, kullanici onayi yok; onaya kadar `src/` altina tek satir `.cs` yazilmaz (CLAUDE.md G12; kapi tanimi `checklists/gate-003-ui-onay.md`, karar kaydi ADR-025). Iki yardimci dosya (`flows.md`, `component-inventory.md`) diskte yok → checklist sorulari 4, 8, 9 `DOĞRULAMA GEREKLI`.

---

## §1 Dosya Envanteri (`ui-design/`, 2026-09-30 olcumu)

| # | Dosya | Ekran / rol | Boyut (bayt) | Durum |
|---|---|---|---|---|
| 0 | `README.md` | klasor indeksi + ekran adi sozlesmesi | 6598 | tamam (status: draft) |
| 1 | `01-start-page.md` | karsilama + son projeler | 5828 | tamam |
| 2 | `02-main-ide.md` | ana pencere (kabuk) | 5954 | tamam |
| 3 | `03-solution-explorer.md` | proje agaci | 5187 | tamam |
| 4 | `04-code-editor.md` | kod editoru + sekmeler | 5430 | tamam |
| 5 | `05-ai-chat.md` | ajan sohbeti | 5675 | tamam |
| 6 | `06-agent-panel.md` | gorev paneli | 5997 | tamam |
| 7 | `07-settings.md` | ayarlar | 5607 | tamam |

**Toplam 8 dosya = 8/8.** Boyut araligi **5.187-6.598 bayt (5,1-6,4 KB)**; hicbir dosya 3 KB altinda degil, hicbir 8 KB ustune cikmiyor (sozlesme 3-8 KB).

---

## §2 Ekran Adi Sozlesmesi ve Eski Celiski (R1)

| Kaynak | 7 ekran adi | Bu vault'taki durum |
|---|---|---|
| Usta prompt (SSOT) | `01-start-page` `02-main-ide` `03-solution-explorer` `04-code-editor` `05-ai-chat` `06-agent-panel` `07-settings` | **esas** — diskteki dosya adlariyla birebir |
| `ui-design/README.md` §0 | ayni 7 ad | celiski kapatildi: "dosya adlari esastir", eski liste `DOĞRULAMA GEREKLI` |
| Kaynak vault README (stale) | `02-project-new` `03-editor` `04-agent-chat` `05-tasks` `06-terminal` | **bu vault'ta yok**, uretilmeyecek |

**Karar:** ad uyusmazligi cikarsa diskteki dosya adi kazanir (ADR-002: dogrulanmis yerel kanit > tekrarlanan metin); fark `CONFLICT DETECTED` etiketiyle kullaniciya sorulur, ajan karari kendi basina uygulamaz.

---

## §3 Wireframe Format Filtresi (GATE-003 girişi)

| # | Kural | Dogrulama |
|---|---|---|
| 1 | UTF-8 + ilk satir `---` + 8 alanli frontmatter | `scripts/measure-vault.ps1` → NoFrontmatter 0 |
| 2 | Kutu cizimi **saf ASCII** (`+ - \|`); `U+2500-U+257F` yasak | `scripts/find-mojibake.ps1` + gozden gecirme |
| 3 | Kontrol seti **yalniz DevExpress v26.1**, ribbon **yalniz Office2010** | `.rules/devexpress-rules.md` (ADR-004) |
| 4 | Her dosya: kabuk + bolgeler + durumlar (bos/yukleniyor/hata) + etkilesim + TFM notu (`net48` + `net10.0-windows`) | `.templates/ui-design/` sablonu |
| 5 | `.Form.cs` + `.Designer.cs` + `.resx` **koddan sonra**; onaysiz kod G12 ihlali | [WORKFLOW](../WORKFLOW.md) §2.4 |

---

## §4 GATE-003 — Kullanici Onay Kutusu (HARD STOP)

| # | Onay maddesi | Kanit | Isaret |
|---|---|---|---|
| 1 | 7 wireframe dosyasi `ui-design/` altinda mevcut | §1 envanter (7/7) | `[ ]` |
| 2 | `README.md` ekran listesi dosya adlariyla birebir | §2 tablosu (0 fark) | `[ ]` |
| 3 | Her dosya ASCII + 8 alanli frontmatter + v26.1 kontrol seti | `scripts/find-mojibake.ps1` = 0 | `[ ]` |
| 4 | 16 yedek WF ekran olarak uretilmedi | sayim = 7 | `[ ]` |
| 5 | **Kullanici onayi (yazili)** | `log.md` tarihli `GATE-003 ONAY` satiri | `[ ]` |

> **HARD STOP:** 5 kutu da isaretlenmeden `.cs` / `.Designer.cs` / `.resx` yazilamaz. Onay yalniz **kullaniciya** aittir; ajan onay veremez. Checklist sorulari 4-8-9 (`component-inventory.md`, `flows.md`) su an `DOĞRULAMA GEREKLI`.

---

## §5 Kalan Isler

| # | Is | Sahip | Durum |
|---|---|---|---|
| 1 | `component-inventory.md` (kontrol envanteri) uretilmesi | ui-designer | bekliyor (diskte yok) |
| 2 | `flows.md` (ekranlar arasi gecis oklari) uretilmesi | ui-designer | bekliyor (diskte yok) |
| 3 | 13 soruluk checklist'in doldurulmasi | ui-designer | bekliyor (0/13) |
| 4 | Yazili kullanici onayi | kullanici | **GATE-003 kapali** |
| 5 | Onay sonrasi 4 maddelik devir paketi (`developer`) | ui-designer | onay sonrasi |

---

## §6 Risk

| # | Risk | Etki | Kapanis kosulu |
|---|---|---|---|
| R1 | README `status: draft` + yardimci 2 dosya yok | gate kutusu 4, 8, 9 doldurulamaz | dosyalar uretilir, README `active` yapilir |
| R2 | Onay beklemeden kod yazilmasi | G12 ihlali → tum kod silinir, part basa doner | `src/` = 0 `.cs` (su an dogrulandi) |

---

## §7 Sure

| Is | Sahip | Tahmin |
|---|---|---|
| `component-inventory.md` + `flows.md` | ui-designer | ≈45 dk |
| Checklist 13 soru + gozden gecirme | ui-designer + reviewer | ≈20 dk |
| **GATE-003 kullanici onayi** | kullanici | ≈10 dk |
| **Toplam** | — | **≈75 dk** |

---

## §8 Kapsam Disi

| Bu rapor NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Wireframe iceriginin kendisi (bolgeler, kontrol listeleri) | `ui-design/01-start-page.md` .. `07-settings.md` |
| Ekran akisinin detayi | `ui-design/README.md` §3 (kabuk ortak bolgeleri) |
| Form kodu, olay baglayicilari | [WORKFLOW](../WORKFLOW.md) §2.5 (GATE-003 sonrasi) |
| `.Designer.cs` bicim dogrugu | `checklists/winforms-designer-check.md` |
| Proje iskeleti ve build kaniti | [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) |

---

## §9 Kanit

| # | Kanit | Kaynak | Olcum |
|---|---|---|---|
| 1 | `ui-design/` 8 dosya, 5.187-6.598 bayt | dosya bilgisi (get_file_info), 2026-09-30 | 8/8 |
| 2 | Ekran adlari sozlesme ile ayni | `ui-design/README.md` §0 + dosya listesi | 0 fark |
| 3 | Gate 13 soru hicbiri isaretli degil | `checklists/gate-003-ui-onay.md` satir 36-61 | 0/13 |
| 4 | Onay öncesi kod yok | `C:\versacoder` icinde yalniz `.ai/` + `.git/` | `.cs` = 0 |
| 5 | Yardimci dosyalar yok | `ui-design/` dizin listesi | `flows.md`, `component-inventory.md` = 0 |

Kaynak dosya adlari: `03-ui-design-raporu.md` (bu dosya), `ui-design/README.md`, `checklists/gate-003-ui-onay.md`, `09-teslim-raporu.md`.
