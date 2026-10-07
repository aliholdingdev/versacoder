---
title: "adr-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Yeni ADR acma - numara, Nygard sablonu, 2 alternatif, .rules karsiligi, adr-index ve log kaydi"
---

# adr-acma — Yeni ADR Acma

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [yeni-rule-acma](yeni-rule-acma.md)

> **Tek cumlelik sorumluluk:** *Bir mimari karar nereye, hangi sira ve hangi alanlarla yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

ADR = **geri alinabilir kararin kaydi**. Yalniz `architect` acar; alternatif **en az 2**, sonuc hem `(+)` hem `(-)` yazilmazsa ADR eksiktir. Dosya `.decisions/accepted/ADR-0NN-<kebab-konu>.md` altina, `status: accepted` ile girer; karari zorunlu kilan `.rules/` maddesi olmadan ADR **kapanmaz**. Toplam ≈20 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Sonraki bos numara alinmis | `.ai/.decisions/adr-index.md` son satir |
| 2 | Karar kapsam disi degil (kapsam = kullanici) | `.ai/reports/00-kapsam-analizi.md` |
| 3 | Ilgili teknoloji kilidi okunmus | [keys.md](../keys.md) §2 (net48 + net10.0-windows, v26.1, Dapper) |
| 4 | En az 2 alternatif hazir | kaba taslak (2 satir/fikir) |
| 5 | Cakisma suphesi | `.ai/.decisions/accepted/` dosya adlarina hizli bakis |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `architect`

1. **Numara + dosya adi** — `ADR-0NN-<kebab-konu>.md`; mevcut ADR'lerle ayni klasore (`accepted/`).
   - Komut: `Get-ChildItem C:\versacoder\.ai\.decisions\accepted\*.md`
   - Cikti: bos numara (006..027 bandi planli)

2. **Nygard sablonunu doldur** — Baglam / Karar / Alternatif (>=2) / Sonuc (+ ve −).
   - Sablon: `.ai/.templates/adr/adr-template.md`
   - Cikti: `hedef_bacak: "net48 + net10.0-windows"` alani **dolu** (G6)

3. **`.rules/` karsiligini yaz** — karari "yapilir" degil "zorunlu" kilan madde; yoksa ADR kagit uzerinde kalir.
   - Dosya: `.ai\.rules\<ilgili>-rules.md` -> yeni kural satiri
   - Cikti: kural + ihlal sonucu + tespit komutu (3 sutun)

4. **Defter + kayit** — adr-index'e satir, `log.md`'ye tarihli not.
   - Dosya: `.ai\.decisions\adr-index.md` + `.ai\log.md`
   - Cikti: `| ADR-0NN | <konu> | accepted | <tarih> |` satiri

5. **Link + format dogrulama** — bozuk link ADR'yi GATE-004a'da dusurur.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `BOZUK link : 0`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | ADR dosyasi | ilk satir `---`, 8 alanli frontmatter, `authority: SSOT` |
| K2 | Alternatif sayisi | `>= 2` baslik, her birinde `(+)` ve `(-)` |
| K3 | bacak alani | `hedef_bacak: "net48 + net10.0-windows"` |
| K4 | `.rules/` bagi | ilgili rules dosyasinda bu ADR'ye referans var |
| K5 | Link | `check-links.ps1` -> `BOZUK link : 0` |

**Ornek cikti (K1-K3):**

```text
---
title: "ADR-014 - oturum saklama"
type: adr
...
hedef_bacak: "net48 + net10.0-windows"
---
## Alternatifler
### A) SQLite (secilen)        (+) sifir kurulum ...
### B) SQL Server Express      (+) kurumsal ...
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Karar henuz uygulanmadi | `status: accepted` -> `status: superseded`, yerine gerekce + yerine gecen ADR |
| 2 | adr-index satiri | satiri silme, `superseded` olarak isaretle |
| 3 | `.rules/` maddesi | maddede `ADR-0NN` referansini `[eski]` yap, kaldirma (iz takibi) |
| 4 | `log.md` kaydi | ustune `REVERT <tarih>` satiri ekle |
| 5 | Kod jenerasyonu vardi | `git revert <sha>` + `dotnet build` ile iki bacak yeniden dogrulanir |

