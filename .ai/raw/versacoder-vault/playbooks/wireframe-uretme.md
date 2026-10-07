---
title: "wireframe-uretme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "7 ekran icin ASCII wireframe uretimi - kutu-cizim, kontrol listesi v26.1, GATE-003 onayi ve geri alma"
---

# wireframe-uretme — ASCII Wireframe Uretimi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [yeni-form-tasarlama](yeni-form-tasarlama.md)

> **Tek cumlelik sorumluluk:** *Bir ekranin tel kafesi nasil cizilir, ne zaman onaya gider* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Wireframe **koddan once** gelir ve **GATE-003 = kullanici onayi (HARD STOP)** ister; onaysiz kodlama G12 ihlalidir, part basa doner. Dosya `.ai/ui-design/NN-<ekran>.md`, 7 ekran sabittir (`01-start-page` .. `07-settings`), kutu-cizim saf ASCII, kontrol listesi yalniz DevExpress **v26.1**. Tek ekran ≈25 dk (onay suresi haric).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | 7 ekran listesi kilitli (master prompt SSOT) | `.ai/VISION.md` + `.ai/PROJECTS.md` |
| 2 | Wireframe sablonu okunmus | `.ai/.templates/ui-design/wireframe-template.md` |
| 3 | Kontrol listesi v26.1 ile sinirli | `.ai/.rules/devexpress-rules.md` |
| 4 | Ribbon stili biliniyor | `RibbonControlStyle.Office2010` (ADR-004) |
| 5 | Layout kisitlari (olcu/erisilebilirlik) okunmus | `.ai/knowledge/winforms/` ilgili dosya |

---

## §2 Adimlar

**Toplam ≈25 dk / ekran** · yapan: `ui-designer`

1. **Ekran + rol sec** — 7 listeden biri; ad `NN-<ekran>.md`, master prompt disina cikilmaz.
   - Ornek: `.ai\ui-design\03-solution-explorer.md`
   - Cikti: dosya adi + ekranin tek sorusu (1 satir)

2. **Kutu-cizim ciz** — saf ASCII (`┌ ─ │ └ ┘`), ustte ribbon, altta durum cubugu; olcu yazilir.
   - Sablon: `.ai/.templates/ui-design/wireframe-template.md`
   - Cikti: gorunur bolge + panel sinirlari, Unicode kutu degil

3. **Kontrol listesini yaz** — her bolge icin kontrol + v26.1 bagi; dis paket/surum yasak.
   - Icerik: `RibbonControl`, `GridControl`, `TreeList`, `DxAIChat` ... (yalniz v26.1)
   - Cikti: tablo: `Bolge | Kontrol | Surum`

4. **Onaya gonder (HARD STOP)** — kullanici onaylamadan `.Form.cs` yazilmaz.
   - Komut: onay istemi + `.ai\log.md`'ye "GATE-003 <ekran> onayli" satiri
   - Cikti: yazili onay kaydi

5. **Onaydan sonra devret** — 4 maddelik devre paketiyle `developer`'a; kontrol listesi ekli.
   - Dosya: `.ai\ui-design\NN-<ekran>.md` + devre paketi
   - Cikti: `NE URETILDI / KANIT / BILINEN EKSIK / SONRAKI ADIM`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Dosya | `.ai/ui-design/NN-<ekran>.md`, frontmatter 8 alan |
| K2 | Saf ASCII | kutu-cizim satirlari `┌─│└┘`, Unicode kutu yok |
| K3 | 7 ekran | `Get-ChildItem .ai\ui-design\NN-*.md` -> 7 dosya |
| K4 | v26.1 disi yok | kontrol listesinde baska surum/paket gecmiyor |
| K5 | Onay | `log.md`'de GATE-003 onay satiri |

**Ornek cikti (K3):**

```text
01-start-page.md  02-main-ide.md  03-solution-explorer.md
04-code-editor.md 05-ai-chat.md    06-agent-panel.md  07-settings.md
Toplam: 7
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Wireframe dosyasi | `status: draft`'a cevir ya da sil; adim 2'den basla |
| 2 | GATE-003 onayi | onay satirini `REVERT` olarak isaretle, kodlamayi durdur |
| 3 | Onaydan sonra kod yazildi | `.Form.cs` + `.Designer.cs` + `.resx` silinir (uclem birlikte) |
| 4 | Kontrol listesi degisti | eski listeye don + kullaniciya yeniden onay iste (GATE-003 kapanmaz) |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri ekle |

