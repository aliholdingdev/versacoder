---
title: "spec/cli-console-spec.md - CLI Konsolu (09-cli-console) Ekran Sozlesmesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-02
authority: SSOT
scope: "09-cli-console tam ekran CLI yuzeyinin bolge/olcu/klavye sozlesmesi (VC-U-0901..0912), OpenCode-benzeri gorunum, 09-terminal dock siniri ve GATE-003 onay alani"
---

# 09 — CLI KONSOLU (OpenCode-Benzeri Tam Ekran CLI Yuzeyi)

**Zorunlu Baglantilar:** [ui-design/README](../ui-design/readme.md) · [cli-spec](cli-spec.md) · [terminal-spec](terminal-spec.md) · [ai-chat-console-spec](ai-chat-console-spec.md) · [09-terminal](../ui-design/09-terminal.md) · [01-start-page](../ui-design/01-start-page.md) · [02-main-ide](../ui-design/02-main-ide.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md)

> **Tek cumlelik sorumluluk:** *CLI Konsolu ekrani OpenCode CLI gibi mi gorunur, hangi bolgelerden olusur ve klavye/komut davranisi nasildir* sorusunun tek cevabidir.
> Gereksinim kimligi: **VC-U-0901..0912** · Ekran adi: `09-cli-console` (ui-design/README §1) · Karsilikli yuzey: [10-ai-chat-console](ai-chat-console-spec.md).

---

## §0 Ozet (Answer First)

`09-cli-console` = **tam ekran CLI yuzeyi** (OpenCode TUI benzeri): ustte **sekme seridi** (oturum sekmeleri + yeni sekme), ortada **ASCII logo + prompt kutusu** (metin + `Build / model / tools` satiri + yol:branch), altta **ipucu satiri** ve en altta **durum cubugu**. Komut girisi `TextEdit` benzeri tek satir; cikti `MemoEdit` (ReadOnly) akisi. Bu ekran `02-main-ide` dock paneli **degildir**; dock paneli [09-terminal](../ui-design/09-terminal.md) olarak ayrica yasar.

**Durum: GATE-003 ONAY BEKLIYOR** - bu sozlesme kod izni icin **girdidir** (G12).

| Bolge | Kontrol (teklif) | Yerlesim |
|---|---|---|
| Ust | `XtraTabControl` sekme seridi: oturum sekmeleri (`[i] baslik [x]`) + `[+ New session]` | Pencere ustu, ≈32 px |
| Merkez-ust | ASCII logo (blok `versacoder`) - `LabelControl` monospace | ekran dikey merkezi ustü |
| Merkez | Prompt kutusu: metin satiri + `Build / model / tools` satiri + alt satir `yol:branch` + klavye ipuclari | ≈660 x ≈110 px |
| Alt | `MemoEdit` (ReadOnly) ciktisi - komut calisirken logo yerini cikti akisina birakir | prompt alti, kalan yukseklik |
| En alt | `StatusBarControl`: `cd yol:branch` · komut/sayac · `local` | ≈24 px |

> **Kontrol notu (ui-designer):** ASCII logo icin `LabelControl` + monospace font mu, yoksa `PictureEdit` mi - **DOGRULAMA GEREKLI** (ilk derlemede karar).

---

## §1 Wireframe (ASCII)

**Olculendirme (yaklasik, `≈` zorunlu):** pencere `1280x800 px` · sekme seridi `≈32 px` ·
prompt kutusu `≈660 x 110 px` · durum cubugu `≈24 px` · sadakat L2 (bilesen adayi, piksel birebir degil).
Kutu `|` `-` `+` ile cizildi; Unicode kutu cizim (U+2500-U+257F) ve emoji YOK (G4).

```text
+----------------------------------------------------------------------+ <- 0
| [i] VersaCoder 5000+ Layer M [x] | [+] New session                    | <- ~32 px
+----------------------------------------------------------------------+ <- 32
|                                                                      |
|                                                                      |
|                       v e r s a c o d e r                            | <- ASCII logo
|                                                                      |    (monospace)
|                                                                      |
|      +----------------------------------------------------------+    |
|      | komut veya istek yazin...                                |    |
|      | Build  auto  ·  MiMo-V2.6-Flash OpenRouter  ·  none      |    | <- prompt
|      | C:\versacoder:main                                       |    |    ~660x110
|      +----------------------------------------------------------+    |
|      shift+tab agents     ctrl+p commands     ctrl+l temizle         |
|                                                                      |
|  (komut calisirken: logo kaybolur, cikti akisi buraya duser)         |
|   $ versacoder build                                                 |
|   Build succeeded. 0 Warning(s) 0 Error(s)                           |
|                                                                      |
+----------------------------------------------------------------------+ <- 776
| cd C:\versacoder:main          3 MCP /mcps   2 plugins failed         | <- StatusBar ~24
| local                                                                     |
+----------------------------------------------------------------------+ <- 800
```

**Komut calisirken (gecis) durumu:**

```text
+----------------------------------------------------------------------+
| [i] VersaCoder 5000+ Layer M [x] | [+] New session                    |
+----------------------------------------------------------------------+
| > versacoder build                                                    |
|   Determining projects to restore...                                  |
|   Build succeeded. 0 Warning(s) 0 Error(s)                            |
|   exit: 0   sure: 3.4s   satir: 10.000/10.000   UTF-8                |
+----------------------------------------------------------------------+
```

**Klavye ipuclari satiri (prompt alti):** `shift+tab` ajan secici · `ctrl+p` komut paleti ·
`ctrl+l` temizle · `ctrl+c` calisan komutu iptal (exit 5) · `up/down` gecmis (1000 satir, VC-CLI-014).

---

## §2 Davranis (VC-U-0901..0912)

| ID | Olay | Ne olur | Kanit |
|---|---|---|---|
| `VC-U-0901` | Ekran acilir | logo + prompt kutusu + durum cubugu gorunur; cikti akisi bos | §1 wireframe |
| `VC-U-0902` | `Enter` | satir `cli-spec` komut agacina gonderilir (6 ana komut) | cli-spec §3 |
| `VC-U-0903` | komut calisirken | logo gizlenir, `MemoEdit` satir bazli async akar (VC-CLI-015) | terminal-spec §2.2 |
| `VC-U-0904` | bitti | ozet satiri (`exit / sure / satir`) + StatusBar guncellenir | cli-spec §4.1 |
| `VC-U-0905` | `Ctrl+C` / Stop | `CancellationToken` + kill -> "cancelled" + exit 5 | terminal-spec §2.3 C3 |
| `VC-U-0906` | `--timeout` asilir | kill + "killed (timeout)" + exit 5 | cli-spec VC-E-0504 |
| `VC-U-0907` | `deny` (izin reddi) | "permission denied" + exit 4 | cli-spec VC-CLI-016 |
| `VC-U-0908` | `up/down` | gecmis (history.txt, 1000 satir FIFO) | cli-spec VC-CLI-014 |
| `VC-U-0909` | `shift+tab` | ajan secici acilir (3 paralel ajan siniri) | AGENTS §5 |
| `VC-U-0910` | `ctrl+p` | komut paleti (6 ana komut + ayarlar) | cli-spec §3 |
| `VC-U-0911` | sekme `[x]` | son sekme kapaninca `02-main-ide`'ye donulur | flows §2/10 |
| `VC-U-0912` | tampon asimi | eski satir dus (FIFO, 10.000 satir) | 09-terminal §2.1/3 |

**Tek satis kurali:** bu ekranda **iki komut ayni anda calismaz** (terminal-spec C5); ikinci `Enter` pasiftir.

---

## §3 DevExpress Kontrolleri (devexpress-rules DX-09)

| # | Kontrol | Rol | Neden |
|---|---|---|---|
| 1 | `XtraTabControl` | oturum sekmeleri | sekme yonetimi + kapatma |
| 2 | `TextEdit` | komut girdisi | tek satir, Enter tetikler |
| 3 | `MemoEdit` (ReadOnly) | cikti akisi | satir bazli ekleme (terminal-spec) |
| 4 | `LabelControl` | ASCII logo + ipucu satiri | monospace font, dosya I/O yok |
| 5 | `StatusBarControl` | yol:branch / sayac / local | alt cubuk |
| 6 | `DefaultLookAndFeel` | tema | theming-spec |

**YASAK:** `TextBox`, `RichTextBox` (WinForms), elle `Cursor`/`Thread` (ui-rules), kod ici sabit string (`.resx` sahipligi U5).

---

## §4 Iliskiler (karsi ekranlar)

| Iliski | Yon | Ne tasir |
|---|---|---|
| `09-cli-console -> 09-terminal` | dock | terminal dock paneli ayri yuzeydir; bu ekran tam ekran CLI'dir |
| `09-cli-console -> 10-ai-chat-console` | /chat | `/chat` komutu AI Chat konsolunu acar |
| `09-cli-console -> 01-start-page` | tile | "CLI Konsolu" tile'i bu ekrani acar (01 §3/2b) |
| `09-cli-console -> 02-main-ide` | sekme | Ctrl+W / son sekme kapanisi kabuga doner |
| `09-cli-console -> cli-spec` | contract | komut agaci + exit code (VC-CLI-001..016) |
| `09-cli-console -> 06-agent-panel` | task | `agent run` ciktisi gorev kuyruguna duser |

---

## §5 GATE-003 Onay Alani

- [ ] Wireframe `09-cli-console` 100% (`ui-rules` U1)
- [ ] ASCII logo kontrolu (`LabelControl` vs `PictureEdit`) **onaylandi** (§0 kontrol notu)
- [ ] `cli-spec VC-CLI-008..016` ile uyumlu (§2 davranis tablosu)
- [ ] `09-terminal` ile cakisma yok (dock vs tam ekran siniri §4/1)
- [ ] `ui-design/README.md` §0 + §7.1'e satir eklendi (bu dosya)
- [ ] GATE-003 checklist 13/13 - **onay = kod izni**

**Onay bekleniyorsa:** `GATE-OPEN` yazilir, `src/**`'ye tek satir `.cs` YAZILMAZ (G12).

---

**Olculen kanit (2026-10-02):** bu dosya YENI yazildi; `ui-design/` = 10 -> 11 wireframe (+ README).
