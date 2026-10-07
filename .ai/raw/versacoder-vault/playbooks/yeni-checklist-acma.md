---
title: "yeni-checklist-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "checklists/ altinda yeni denetim listesi acma - kanit bekleyen sorular, gate bagi, ureten ile dolduranin ayriligi"
---

# yeni-checklist-acma — Yeni Checklist Acma

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [yeni-spec-acma](yeni-spec-acma.md)

> **Tek cumlelik sorumluluk:** *Hangi kapidan once hangi sorular sorulur, kaniti nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Checklist = **evet/hayir + kanit sutunu**; aciklama `spec/`'te yasar (SRP). Her soru bir cikti bekler (dosya, komut, sayi); kanitsiz soru **bos evet** uretir ve liste gecersizdir. Listeyi **ureten degil** baska bir rol doldurur (`reviewer`). En fazla 30 soru, sure ≈15 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Hangi gate bagli belli | `.ai/WORKFLOW.md` §3 (GATE-001..006) |
| 2 | Mevcut checklistler okunmus (cakisma yok) | `.ai/checklists/index.md` |
| 3 | Her sorunun kaniti yazilabilir | komut/dosya/sayi hazirligi |
| 4 | Dolduracak rol belirlendi | ureten rol disi (`reviewer`) |
| 5 | Ilgili spec/ADR mevcut | `.ai/spec/*.md` ya da `.decisions/accepted/` |

---

## §2 Adimlar

**Toplam ≈15 dk** · yapan: `architect` (dolduran: `reviewer`)

1. **Gate + soru setini sec** — hangi kapi, hangi sorular; soru sayisi 30'u gecmez.
   - Referans: `.ai\WORKFLOW.md` §3 cikis kontrolleri
   - Cikti: gate adi + soru taslagi (madde basina 1 satir)

2. **Her soruya kanit sutunu yaz** — `[ ]` + beklenen cikti; "temiz mi?" gibi olcusuz soru yasak.
   - Sablon: `.ai\checklists\index.md` §1 tablosu bicimi
   - Cikti: her satirda `Soru | Kanit` ikilisi

3. **Dosyayi olustur** — frontmatter 8 alan, `type: checklist`, §0 Ozet + §1 soru tablosu.
   - Ornek: `.ai\checklists\gate-00X-<konu>.md`
   - Cikti: ilk satir `---`, en az 1 tablo

4. **Deftere kaydet** — `checklists/index.md`'ye dosya + gate + kanit satiri.
   - Dosya: `.ai\checklists\index.md` §1
   - Cikti: `[<dosya>.md](<dosya>.md) | GATE-00X | <kanit>`

5. **Link taramasi** — bozuk link, gate oncesi reddedilir.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `BOZUK link : 0`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Dosya + defter | `checklists/` dosyasi + `index.md` satiri ayni adla |
| K2 | Kanit sutunu | her soruda dosya/komut/sayi; olcusuz soru = 0 |
| K3 | Soru siniri | `<= 30` soru |
| K4 | Rol ayrimi | dosyada "dolduran: reviewer" notu var |
| K5 | Link | `check-links.ps1` -> `BOZUK link : 0` |

**Ornek cikti (K2):**

```markdown
- [ ] dotnet build iki bacakta 0 hata   -> KANIT: build ciktisi (net48 + net10.0-windows)
- [ ] EF Core referansi yok            -> KANIT: Select-String EntityFramework -> 0 satir
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni checklist dosyasi | dosyayi sil |
| 2 | `index.md` satiri | ayni satiri geri al |
| 3 | Doldurulmus liste (`[x]` dolu) | kutulari `[ ]`'a cevir ya da dosyayi `status: archived` yap |
| 4 | Gate cikisina eklendi | `WORKFLOW.md` §3 satirini eski haline dondur (onay: kullanici, G7) |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri ekle |

