---
title: "gate-003-ui-onay - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GATE-003 (UI Approval - HARD STOP) oncesi 13 kanitli soru: 7 wireframe, saf ASCII, yalniz v26.1 kontrol seti, kullanici onayi"
---

# gate-003-ui-onay — GATE-003 UI Onayi (HARD STOP) Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *7 ekran wireframe'i tam, kontrol seti yalniz DevExpress v26.1 ve kullanici onayi yazili mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

GATE-003, vault'un tek **dururucu** gate'idir: kapali degilse `src/**` altina **hicbir `.cs` yazilamaz** (CLAUDE.md G12). Listeyi `ui-designer` hazirlar, doldurur ve **kullanici** onay vermeden kapanmaz; onaysiz kodlamaya baslanan her dosya part basa alinir. Kanit = 7 dosya + `component-inventory.md` + `log.md`'ye tarihli onay satiri.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (mimari kisit `architect`) |
| Kim kapatir | **yalniz kullanici** — ajan kapatamaz |
| Hard stop | Onay yoksa `.cs` yazimi **0** olmali |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [x] 1. `ui-design/01-start-page.md` … `07-settings.md` dosyalarinin **7/7** si diskte mi?
  **Kanit:** `Get-ChildItem .ai\ui-design -Filter *.md` ciktisi (7 ekran adi tek tek gorunur).
  **Cikti (2026-10-01):** 11 `.md` — 01-start-page, 02-main-ide, 03-solution-explorer,
  04-code-editor, 05-ai-chat, 06-agent-panel, 07-settings, 08-ascii-art-viewer,
  component-inventory, flows, README → ekran **7/7 + 1 onayli ekstra (08)**
  (duzeltme: ilk yazimda 03/05/06 dosya adlari yanlis listelenmisti, komutla yeniden alindi).
- [x] 2. Her dosyanin ilk satiri `---` ve 8 alanli frontmatter var mi?
  **Kanit:** 7 dosyanin ilk 10 satiri yan yana alinti.
  **Cikti (2026-10-01):** 11/11 dosyada `alan=8` (title, type, category, status, version,
  updated, authority, scope); 8 ekran dosyasinin ilk 10 satiri okundu, hepsi `---` ile basliyor.
- [x] 3. Wireframe'ler **saf ASCII** mi (Unicode kutu cizim `-|++` yok)?
  **Kanit:** `Select-String -Path .ai\ui-design\*.md -Pattern '[\u2500-\u257F]'` ciktisi = **0 satir**.
  **Cikti (2026-10-01):** desen = **0 satir**; ayrica emoji/non-BMP taramasi
  (`[\uD800-\uDBFF][\uDC00-\uDFFF]`) = **0 satir**.
- [x] 4. `component-inventory.md`'deki tum kontroller **yalniz DevExpress v26.1** mi?
  **Kanit:** envanter tablosu + `26.1` disi herhangi bir surum satiri aramasinin 0 sonucu.
  **Cikti (2026-10-01):** envanterde tek surum tokeni `{26.1}`; eski surum taramasi
  (`24.|23.|25.`) = **0 satir**.
- [x] 5. Ana pencere `RibbonForm` ve stil `RibbonControlStyle.Office2010` mi?
  **Kanit:** `component-inventory.md` + `ui-design/02-main-ide.md` icinde bu iki adin gectigi satirlar.
  **Cikti (2026-10-01):** `02-main-ide.md:29` → **`MainForm : RibbonForm`** +
  `RibbonControlStyle.Office2010` (kayit satiri bu oturumda eklendi);
  `component-inventory.md:25,53` ayni kayit; literal `RibbonForm` sayisi = **1**.
- [x] 6. Ekran adlari usta prompt'taki 7 ekran SSOT'u ile birebir ayni mi?
  **Kanit:** ad listesi karsilastirmasi (0 fark); fark varsa `CONFLICT DETECTED` etiketi.
  **Cikti (2026-10-01):** VISION §2.1 (7 ad) <-> disk (01..07) = **0 fark**; ekstra
  `08-ascii-art-viewer` onayli kapsam degisikligi (README §7.1 satir 8, log.md:178 "7+1 ONAYLI").
- [x] 7. 16 vs 7 ekran celiskisi (R1) kullaniciya soruldu mu?
  **Kanit:** `log.md` icinde R1/`CONFLICT DETECTED` kaydi ya da "YOK" yazilmis satir.
  **Cikti (2026-10-01):** `log.md:179` (satir 32) → `R1 (16 vs 7 ekran celiskisi) KAPANDI`
  + kaynak `reports/00-kapsam-analizi.md` karar 8; eski `Select-String` = 0 satirdi,
  bu oturumda tarihli kayit eklendi (kullanici onayi README §7.1 satir 8 = 7+1).
- [x] 8. `flows.md` ekranlar arasi gecisleri (baslangic → IDE → chat → ayarlar) kapsiyor mu?
  **Kanit:** `flows.md` icinde her ekranin en az 1 giris + 1 cikis oku.
  **Cikti (2026-10-01):** ekran basina gecis sayisi (giris+cikis satiri) — 01:12, 02:21,
  03:7, 04:9, 05:6, 06:6, 07:11, 08:7 → hepsi **>= 2** (min 1 giris + 1 cikis).
- [x] 9. Wireframe'deki her kontrol `component-inventory.md`'de kayitli mi?
  **Kanit:** iki dosya arasinda ad eslesme tablosu; eslesmeyen ad = 0.
  **Cikti (2026-10-01):** `component-inventory.md` §5 ad eslesme tablosu: **25 ad, eslesmeyen 0**
  (iki yonlu kontrol); §1 tablo 72 satir / 8 ekran.
- [x] 10. Mimari diyagramlar da **saf ASCII** mi (Unicode kutu yok)?
  **Kanit:** `Select-String` ayni desenle `architecture/` uzerinde = 0 satir.
  **Cikti (2026-10-01):** `architecture/` icindeki **117** `.md` uzerinde
  `[\u2500-\u257F]` = **0 satir**.
- [x] 11. **Onay oncesi** kod yazilmis mi (0 olmali)?
  **Kanit:** `git status --porcelain` ya da `Get-ChildItem C:\versacoder\src -Recurse -Filter *.cs` = **0 dosya**.
  **Cikti (2026-10-01):** `Test-Path src` = **False** → 0 `.cs`;
  `git status --porcelain` = 10 satir, tamami `.ai/**` → `.cs` = **0**.
- [x] 12. Kullanicinin **acik, yazili** onayi var mi?
  **Kanit:** `log.md` icinde tarihli `GATE-003 ONAY` satiri (onay metni + tarih).
  **Cikti (2026-10-01):** `log.md:178` → `GATE-003 ONAY` (2026-10-01, 7+1 ONAYLI);
  `ui-design/README.md:189` (§7.4) → ayni onay metni.
- [x] 13. Onay sonrasi devre paketi `developer`'a 4 maddeyle gonderildi mi?

- [x] 14. **09-terminal** (yeni, 2026-10-02) wireframe'i onayli mi? (CLI terminal dock paneli)
  **Kanit:** `ui-design/09-terminal.md` + `cli-spec VC-CLI-015` + `llm-provider-spec VC-LLM-014`.
  **Durum:** **ONAYLI (2026-10-02)** — kullanici "CLI wireframe onayi = EVET, basla" dedi;
  kayit `log.md` [gate-003-madde14-onay] satiri. 9. ekran + CLI kod izni ACIK (G12 kaldirildi).
  **Kapali ise:** `src/**`'ye 9. ekran (`09-terminal`) icin tek satir `.cs` YAZILMAZ (G12);
  mevcut 8 ekran icin izin SURER (checklist 13/13, 2026-10-01).
  **Kanit:** devre metninde `NE URETILDI / KANIT / BILINEN EKSIK / SONRAKI ADIM` basliklari (AGENTS.md §4.2).
  **Cikti (2026-10-01):** `log.md` satir 33 (bu dosya) → 4 baslik tek satirda kayitli;
  ayni paket `developer` ajanina aktarildi (oturum ici devir).

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | Eksik ekran (7'den az) → gate acik, kod yasak |
| 2 | Soru 11 | Oncesinde `.cs` varsa **G12 ihlali** → tum kod silinir, part basa doner |
| 3 | Soru 4+5 | v26.1 disi kontrol ya da Office2010 disi ribbon stili → **G3 CRITICAL** |
| 4 | Soru 12 | Yazili onay yoksa gate **kapali sayilir**, surum raporu gecersiz |
| 5 | Soru 3+10 | Unicode kutu cizim → wireframe ASCII'ye cevrilir, liste yeniden doldurulur |

### §2.1 Isaretlenmeyen sorular (2026-10-01)

```text
KAPANDI : Soru 5 - 02-main-ide.md'e 1 satir 'MainForm : RibbonForm' kaydi eklendi
          (satir 29); kanit simdi iki dosyada da tam → [x].

KAPANDI : Soru 7 - log.md satir 32'ye tarihli 'R1 ... KAPANDI' kaydi eklendi
          (kaynak: reports/00-kapsam-analizi.md karar 8) → [x].

KALIYOR : Soru 13 - devir paketi ust ajanin (parent) gorevi; paket log.md'ye
          kaydedilip isaretlenecek → o ana kadar [ ].
```

Gecen: **10/13** (1,2,3,4,6,8,9,10,11,12) · Isaretsiz: **5, 7, 13**

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Formun kod uretimi (`.Form.cs`, olay baglayicilari) | [../WORKFLOW.md](../WORKFLOW.md) §2.5 P5 |
| `.Designer.cs` / `.resx` bicim ve `ResourceManager` dogrulugu | [winforms-designer-check.md](winforms-designer-check.md) |
| DevExpress kontrol API dogrulugu (v26.1 icerigi) | [devexpress-control-check.md](devexpress-control-check.md) · [ribbon-style-check.md](ribbon-style-check.md) |
| Erisilebilirlik (klavye, kontrast, ScreenReader) | [accessibility-check.md](accessibility-check.md) |
| Test kapsamı ve coverage | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
