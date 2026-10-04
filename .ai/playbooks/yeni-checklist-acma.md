---
title: "yeni-checklist-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "checklists/ altinda yeni denetim listesi acma - kanit bekleyen sorular, gate bagi, ureten ile dolduranin ayriligi"
---

# yeni-checklist-acma — Yeni Checklist Acma

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [gate-004a](../checklists/gate-004a-vault-kapanis.md) · [yeni-spec-acma](../playbooks/yeni-spec-acma.md)

> **Tek cumlelik sorumluluk:** *Hangi kapidan once hangi sorular sorulur, kaniti nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Checklist = **evet/hayir + kanit sutunu**; aciklama `spec/`'te yasar (SRP). Her soru bir cikti bekler (dosya, komut, sayi); kanitsiz soru **bos evet** uretir ve liste gecersizdir. Listeyi **ureten rol degil**, baska bir rol doldurur (`reviewer`). En fazla 30 soru, sure ≈15 dk. Gate kapanisinda adim adim soru listesi istenen kapidan gelir (ornek: [gate-004a](../checklists/gate-004a-vault-kapanis.md)).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Hangi gate bagli belli | [WORKFLOW](../WORKFLOW.md) §2 (GATE-004a..006) |
| 2 | Mevcut checklistler okunmus (cakisma yok) | `checklists/index.md` |
| 3 | Her sorunun kaniti yazilabilir | komut / dosya / sayi hazirligi |
| 4 | Dolduracak rol belirlendi | ureten rol disi (`reviewer`) |
| 5 | Ilgili spec/ADR mevcut | [spec index](../spec/index.md) ya da [adr-index](../.decisions/adr-index.md) |

---

## §2 Adimlar

**Toplam ≈15 dk** · yapan: `architect` · dolduran: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Gate + soru setini sec | gate adi + soru taslagi (madde basina 1 satir), <= 30 soru | 3 dk |
| 2 | Her soruya kanit sutunu yaz | `Soru / Kanit` ikilisi; olcusuz soru yasak | 5 dk |
| 3 | Dosyayi olustur | frontmatter 8 alan, `type: checklist`, §0 + soru tablosu | 3 dk |
| 4 | Deftere kaydet | `checklists/index.md`'ye dosya + gate + kanit satiri | 2 dk |
| 5 | Link taramasi | `check-links.ps1` -> `BOZUK link : 0` | 2 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Soru "temiz mi?" seklinde | Kanit sutunu yazilmamis | Her soruya komut/dosya/sayi eklenir |
| 2 | Liste uretenin elinde doldurulmus | Rol ayrimi kurulmamis | "dolduran: reviewer" notu + baska rol |
| 3 | Ayni soru 2 listede | Kardes kontrol okunmamis | Tek liste kazanir, digeri linklenir |
| 4 | 30 soru asimi | Aciklama da listeye girmis | Aciklama spec'e tasinir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni checklist dosyasi | dosya silinir |
| 2 | `index.md` satiri | ayni satir geri alinir |
| 3 | Doldurulmus liste (`[x]` dolu) | kutular `[ ]`'a cevrilir ya da `status: archived` |
| 4 | Gate cikisina eklendi | [WORKFLOW](../WORKFLOW.md) eski haline doner (onay: kullanici) |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri eklenir |

---

## §5 Kabul Kriteri

- [ ] Her soruda beklenen kanit yazili (dosya / komut / sayi)
- [ ] Soru sayisi **en fazla 30**
- [ ] Dosyada "dolduran: <rol>" notu ureten rolden farkli
- [ ] `checklists/index.md`'de ayni adla 1 satir kayit var
- [ ] `check-links.ps1` -> `BOZUK link : 0`

---

## §6 Ornek Soru Satiri

```text
+--------------------------------------------------+
| [ ] Iki bacak build 0 hata                        |
| KANIT : dotnet build -f net48 ve -f net10.0-windows|
| [ ] EF Core referansi 0 satir                     |
| KANIT : Select-String EntityFramework -> 0 satir  |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Soru sayisi | dosyadaki `[ ]` sayaci | <= 30 |
| 2 | Kanit sutunu | dosya gozlemi: her satirda KANIT | bos soru 0 |
| 3 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈15 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Gate tanimi ve sirasi | [WORKFLOW](../WORKFLOW.md) |
| 2 | Kural metni | [rules index](../.rules/index.md) |
| 3 | Spec yazimi | [yeni-spec-acma](../playbooks/yeni-spec-acma.md) |
| 4 | Ajan/secimi ve cikti kutusu | [AGENTS](../AGENTS.md) §1-§2 |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `checklists/index.md` | mevcut checklist envanteri ve kayit bicimi |
| 2 | `checklists/gate-004a-vault-kapanis.md` | kanit sutunu dolu 14 soru ornegi |
| 3 | `checklists/layer-boundary-check.md` | 13 soruluk katman siniri ornegi |
| 4 | `WORKFLOW.md` | hangi kapidan once hangi listenin istendigi |
