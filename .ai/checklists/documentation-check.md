---
title: "Dokümantasyon Kontrol Listesi - Documentation Check"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Bir MD dosyasi ya da dokuman seti teslim edilmeden once cevaplanmasi zorunlu sorular - eksik madde = RED"
---

# Dokümantasyon Kontrol Listesi (documentation-check)

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu dokuman GATE-004a / reviewer tesliminden geciyor mu* sorusunun tek cevabidir.
> Her madde **kanit** (satir no, olcum, komut ciktisi) ile isaretlenir; kanitsiz "evet" gecersizdir.

---

## §0 Ozet (Answer First)

Kontrol 4 katmandan olusur: **bicim** (frontmatter + UTF-8), **baglanti** (0 bozuk), **icerik** (SSOT + kanit), **yasak** (EF Core / string-concat / surum sapmasi). Tamami gecmedikce dosya teslim edilmez; kirmizi maddeler `reviewer` tarafindan RED olarak bildirilir.

---

## §1 Bicim Kontrolu

| # | Soru | Kanit | Gecis |
|---|---|---|---|
| 1 | Ilk satir `---` mu? | Dosyanin 1. satiri | [ ] |
| 2 | Frontmatter 8 alan tam mi (`title,type,category,status,version,updated,authority,scope`)? | 1-12 satir | [ ] |
| 3 | `authority: SSOT` yaziyor mu? | frontmatter | [ ] |
| 4 | Dosya adi kebab-case ve ASCII mi (Turkce karakter yok)? | dosya adi | [ ] |
| 5 | Icerik UTF-8, mojibake yok mu? | `scripts/measure-vault.ps1` ciktisi | [ ] |
| 6 | Bolum basliklari `## §N <ad>` biciminde ve `§0` var mi? | basliklar | [ ] |

---

## §2 Baglanti Kontrolu

| # | Soru | Kanit | Gecis |
|---|---|---|---|
| 1 | `check-links.ps1` **0 bozuk** veriyor mu? | komut ciktisi | [ ] |
| 2 | Her goreli link var olan dosyaya mi gidiyor? | tarama satiri | [ ] |
| 3 | Ayni bilgi 2. dosyada tekrar ediyor mu (SSOT)? | grep sonucu | [ ] |
| 4 | Index dosyasi (varsa) yeni kaydi iceriyor mu? | index tablosu | [ ] |
| 5 | Dis URL `http(s)://` varsa az ve gercek mi? | link listesi | [ ] |

> **KURAL:** Bozuk link = `G4 RED`. Duzeltme linki silmek degil, **hedefi uretmek** ya da **yolu dogrulamaktir** (dosya adi yazim hatasi ise kaynak duzeltilir).

---

## §3 Icerik Kontrolu

| # | Soru | Kanit | Gecis |
|---|---|---|---|
| 1 | `## §0 Ozet (Answer First)` cevap once veriyor mu? | §0 metni | [ ] |
| 2 | En az 1 tablo ya da 1 kutu-cizim diyagrami var mi? | sayim | [ ] |
| 3 | Madde listeleri en fazla 5 madde mi? | sayim | [ ] |
| 4 | Bilinmeyen bilgi `NEEDS_VALIDATION` / `UNKNOWN` mi isaretli? | etiket | [ ] |
| 5 | "Bitti/tamam" iddiasi build/test/olcum ciktisi ile mi? | kanit satiri | [ ] |

---

## §4 Yasak Kontrolu (Kirmizi)

| # | Yasak | Kontrol | Gecis |
|---|---|---|---|
| 1 | **EF Core** referansi | grep `Microsoft.EntityFrameworkCore` = 0 | [ ] |
| 2 | **String-concat SQL** (`"..."+degisken`) | grep | [ ] |
| 3 | DevExpress **v26.1** disi surum | grep `v2[0-9]` | [ ] |
| 4 | `RibbonControlStyle.Office2010` disi ribbon stili | grep `RibbonControlStyle` | [ ] |
| 5 | CoreMusic / versacoder.ai'dan kopya metin (ADR-002) | karsilastirma | [ ] |

---

## §5 Cikti BiCimi

| Alan | Deger |
|---|---|
| Sonuc | `PASS` ya da `RED` (belirsiz sonuc yok) |
| RED gerekcesi | `seviye · dosya:satir · neden · düzeltme` |
| Kanit | komut ciktisi ya da `UNKNOWN` |
| Devir | PASS -> [session-handoff-check.md](session-handoff-check.md) |

**Ilk adim (<2 dk):** `powershell -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` calistir, sonucu bu dosyanin §2 tablosuna isaretle.
