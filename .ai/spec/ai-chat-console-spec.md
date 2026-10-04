---
title: "spec/ai-chat-console-spec.md - AI Chat Konsolu (10-ai-chat-console) Ekran Sozlesmesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "10-ai-chat-console tam ekran sohbet yuzeyinin bolge/olcu/komut sozlesmesi (VC-U-1001..1010), oturum sekmeleri, 05-ai-chat dock siniri ve GATE-003 onay alani"
---

# 10 — AI CHAT KONSOLU (Oturum Sekmeli Tam Ekran Sohbet)

**Zorunlu Baglantilar:** [ui-design/README](../ui-design/README.md) · [ai-chat-spec](ai-chat-spec.md) · [cli-console-spec](cli-console-spec.md) · [05-ai-chat](../ui-design/05-ai-chat.md) · [02-main-ide](../ui-design/02-main-ide.md) · [session-spec](session-spec.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md)

> **Tek cumlelik sorumluluk:** *AI Chat Konsolu oturum sekmeleriyle nasil calisir, sohbet nerede akar ve prompt kutusu hangi alanlari tasir* sorusunun tek cevabidir.
> Gereksinim kimligi: **VC-U-1001..1010** · Ekran adi: `10-ai-chat-console` (ui-design/README §1) · Es yüzey: [09-cli-console](cli-console-spec.md).

---

## §0 Ozet (Answer First)

`10-ai-chat-console` = **oturum sekmeli tam ekran sohbet yuzeyi** (OpenCode TUI'nin oturum sekmeleri benzeri): ustte sekmeler (`[i] oturum adi [x]` + `[+ New session]`), ortada `RichEditControl` sohbet akisi (kod bloglari dahil), altta prompt kutusu (`MemoEdit`) + model/agent satiri, en altta durum cubugu. `05-ai-chat` dock panelinin **tam ekran karsiligidir**; ayni servisleri paylasir (l2 + l5).

**Durum: GATE-003 ONAY BEKLIYOR** - bu sozlesme kod izni icin **girdidir** (G12).

| Bolge | Kontrol (teklif) | Yerlesim |
|---|---|---|
| Ust | `XtraTabControl` oturum sekmeleri + `[+ New session]` | Pencere ustu, ≈32 px |
| Merkez | `RichEditControl` (ReadOnly) sohbet akisi + rol etiketleri | kalan yukseklik |
| Alt-1 | `MemoEdit` prompt kutusu + model/agent satiri + `[Gonder] [Durdur]` | ≈120 px |
| Alt-2 | `StatusBarControl`: yol:branch · mesaj/token sayaci · model | ≈24 px |

---

## §1 Wireframe (ASCII)

**Olculendirme (yaklasik, `≈` zorunlu):** pencere `1280x800 px` · sekme seridi `≈32 px` ·
prompt kutusu `≈660 x 120 px` · durum cubugu `≈24 px` · sadakat L2.
Kutu `|` `-` `+` ile cizildi; Unicode kutu cizim (U+2500-U+257F) ve emoji YOK (G4).

```text
+----------------------------------------------------------------------+ <- 0
| [i] Eski kodu denetleyen [x] | [i] VersaCoder 5000+ Layer [x] | [+ ]  | <- ~32 px
+----------------------------------------------------------------------+ <- 32
| KULLANICI                                                             |
|   09-terminal wireframe'ini 09 olarak duzelt                          |
|                                                                       |
| VERSACODER (developer)                                                |
|   09-terminal.md basligi ve title alani guncellendi.                  |
|   +----------------------------------------------------------+       |
|   | diff: -# 08 ...  +# 09 ...                                 |       | <- kod blogu
|   +----------------------------------------------------------+       |
|                                                                       |
+----------------------------------------------------------------------+ <- 660
| +--------------------------------------------------------------+      |
| | devam et...                                                   |      | <- prompt
| | Build  auto  ·  MiMo-V2.6-Flash OpenRouter  ·  none           |      |    MemoEdit
| +--------------------------------------------------------------+      |    ~660x120
| [Gonder]  [Durdur]        shift+tab agents    ctrl+p commands         |
+----------------------------------------------------------------------+ <- 776
| cd C:\versacoder:main        mesaj: 12   token: 4.1k   MiMo-V2.6-F   | <- ~24 px
+----------------------------------------------------------------------+ <- 800
```

**Bos durum (yeni oturum):** sohbet alani bos birakilmaz - "Yeni oturum - ne uzerinde calisalim?" +
ipucu satiri (`/prompt-maker` · `/rules`) yazilir (bos beyaz alan yasak, 01 §1 ruhu).

---

## §2 Davranis (VC-U-1001..1010)

| ID | Olay | Ne olur | Kanit |
|---|---|---|---|
| `VC-U-1001` | Ekran acilir | son aktif oturum sekmesi acik; akis servisten okunur | session-spec |
| `VC-U-1002` | `[+ New session]` | yeni sekme + bos durum metni | §1 bos durum |
| `VC-U-1003` | sekme `[x]` | son sekme kapaninca `02-main-ide`'ye donulur | flows §2/11 |
| `VC-U-1004` | `Enter` (poundlu degil) | mesaj akisa eklenir + servise gider (UI thread marshal) | 05 §3/1 |
| `VC-U-1005` | `Shift+Enter` | satir atlar (poundlu gonderim yok) | ai-chat-spec |
| `VC-U-1006` | `Durdur` | `CancellationToken`; UI iptal durumu gosterir | 05 §3/2 |
| `VC-U-1007` | yanit kod blogu | `RichEditControl` renkli/blok gosterim (monospace) | ai-chat-spec |
| `VC-U-1008` | `/chat` (CLI'dan) | bu ekran acilir ve odaklanir | cli-console §4/2 |
| `VC-U-1009` | model/agent satiri degisir | secim servis katmaninda; `07-settings` ile ayni kaynak | 05 §3/4 |
| `VC-U-1010` | token sayaci asilir | uyari + StatusBar'da metin (yalniz renk yok) | ui-rules erisilebilirlik |

---

## §3 DevExpress Kontrolleri (devexpress-rules DX-09)

| # | Kontrol | Rol | Neden |
|---|---|---|---|
| 1 | `XtraTabControl` | oturum sekmeleri | sekme yonetimi + kapatma |
| 2 | `RichEditControl` | sohbet akisi | kod bloglari + zengin metin |
| 3 | `MemoEdit` | prompt kutusu | cok satirli girdi |
| 4 | `SimpleButton` x2 | `[Gonder]` `[Durdur]` | aksiyonlar |
| 5 | `LabelControl` | model/agent satiri + ipuclari | bilgi etiketi |
| 6 | `StatusBarControl` | mesaj/token/model | alt cubuk |

**YASAK:** `TextBox`, WinForms `RichTextBox`, elle `Thread` (ui-rules), kod ici sabit string (U5).

---

## §4 Iliskiler (karsi ekranlar)

| Iliski | Yon | Ne tasir |
|---|---|---|
| `10 -> 05-ai-chat` | dock | ayni servis, iki yuzey (dock vs tam ekran) |
| `10 -> 09-cli-console` | /chat | CLI'dan bu ekrana gecis (§2/8) |
| `10 -> 04-code-editor` | uygula | "Dosyaya uygula" onayiyla diff -> editor (05 §3/3) |
| `10 -> 06-agent-panel` | task | istek gorev kuyruguna dusser (flows §3/15) |
| `10 -> 07-settings` | model | model/agent secimi ayni kaynaktan okunur |

---

## §5 GATE-003 Onay Alani

- [ ] Wireframe `10-ai-chat-console` 100% (`ui-rules` U1)
- [ ] `05-ai-chat` ile kontrol tutarliligi (§3 vs 05 §0) - iki yuzey ayni mi
- [ ] `session-spec` oturum sekmeleri ile uyumlu (sekme yasam dongusu)
- [ ] `ui-design/README.md` §0 + §7.1'e satir eklendi (bu dosya)
- [ ] GATE-003 checklist 13/13 - **onay = kod izni**

**Onay bekleniyorsa:** `GATE-OPEN` yazilir, `src/**`'ye tek satir `.cs` YAZILMAZ (G12).

---

**Olculen kanit (2026-10-02):** bu dosya YENI yazildi; `ui-design/` = 10 -> 11 wireframe (+ README).
