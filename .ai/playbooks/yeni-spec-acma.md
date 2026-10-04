---
title: "yeni-spec-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "spec/ altinda yeni katman veya konu sartnamesi acma - sablon, olculur kabul kriterleri, test stratejisi bagi ve defter kaydi"
---

# yeni-spec-acma — Yeni Sartname Acma

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [spec index](../spec/index.md) · [templates](../.templates/new-doc-contract.md) · [yeni-checklist-acma](../playbooks/yeni-checklist-acma.md)

> **Tek cumlelik sorumluluk:** *Yeni bir sartname nereye, hangi bolumlerle ve hangi kabul kriterleriyle acilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Spec = **"neyin dogru oldugunu"** yazar; nasil yapilacagi playbook'lar, kural `.rules/`'tadir (SRP). Yeni konu ya da katman icin `.ai/spec/<konu>-spec.md` acilir; her sart **olculur kabul kriterine** baglanir ve test stratejisindeki karsiligi not edilir. Kapsam degisikligi kullanici onayi ister. Toplam ≈30 dk; olcusu olmayan sart yazilmaz (G5).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Kapsam dondurulmus | `reports/00-kapsam-analizi.md` |
| 2 | Mevcut spec listesi okunmus | [spec index](../spec/index.md) satirlari |
| 3 | Ilgili karar var ya da acildi | [adr-index](../.decisions/adr-index.md) |
| 4 | Test stratejisi erisilebilir | [spec index](../spec/index.md) -> test stratejisi |
| 5 | Kabul kriteri olculer hazir | komut / dosya / sayi (tahmin degil) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `architect`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Konu + kategori sec | katman mi (l0..l7) yoksa konu mu; ad `kebab-case`, cakisma yok | 3 dk |
| 2 | Sablonu doldur | frontmatter 8 alan + §0 Ozet + zorunlu baglantilar | 5 dk |
| 3 | Sartlari kabul kriterine bagla | `Sart / Kanit / Durum` tablosu; olcusuz sart = 0 | 12 dk |
| 4 | Test stratejisi + karar bagi | her sartin bir test karsiligi ya da gerekcesi | 5 dk |
| 5 | Defter + link taramasi | [spec index](../spec/index.md)'e satir + `check-links.ps1` -> 0 | 5 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | "Dogru calisir" gibi sartlar | Olcum yazilmamis | Her satira komut/dosya/sayi eklenir |
| 2 | Spec ile checklist ayni soruyu yaziyor | SRP asilmis | Aciklama spec'te, soru checklist'te ayrilir |
| 3 | Kapsam satiri sessizce degisti | G10 / onay atlanmis | Kullanici onayi alinir, fark yazilir |
| 4 | Test karsiligi yok | Strateji okunmamis | Karsilik ya da yazili gerekce eklenir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni spec dosyasi | dosya silinir + [spec index](../spec/index.md) satiri geri alinir |
| 2 | Kapsam satiri eklendiyse | eski kapsam metnine donulur, fark raporlanir (kapsam yetkisi kullanici) |
| 3 | Karar bagi eklendiyse | karar `superseded`; spec'teki baglanti kalir (iz) |
| 4 | Kabul kriteri test urettiyse | test dosyalari silinir + build |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri eklenir |

---

## §5 Kabul Kriteri

- [ ] Dosya `.ai/spec/<konu>-spec.md`, ilk satir `---`, 8 alanli frontmatter
- [ ] En az 1 tablo; her satirda kanit sutunu dolu
- [ ] Her sartin test karsiligi ya da yazili gerekcesi var
- [ ] [spec index](../spec/index.md)'e 1 satir kayit eklendi
- [ ] `check-links.ps1` -> `BOZUK link : 0`

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : her sartta kanit sutunu dolu (12/12)     |
|          spec/index.md -> 1 satir eklendi          |
|          check-links  -> BOZUK=0                   |
| DOSYA  : spec/<konu>-spec.md                       |
| EKSIK  : YOK                                       |
| SONRAKI : checklist karsiligini yaz (~15 dk)      |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Defter | [spec index](../spec/index.md) dosyada gorunur mu | 1 satir |
| 2 | Olcum sutunu | dosyadaki `Kanal/Kanit` sutunu | bos satir 0 |
| 3 | Link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈30 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Kural metni (neden zorunlu) | [rules index](../.rules/index.md) |
| 2 | Adim adim yapim akisi | [playbooks index](index.md) |
| 3 | Denetim listesi (evet/hayir) | [yeni-checklist-acma](../playbooks/yeni-checklist-acma.md) |
| 4 | Kapsam karari | `reports/00-kapsam-analizi.md` + kullanici onayi |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `spec/index.md` | mevcut spec envanteri ve kayit bicimi |
| 2 | `spec/test-strategy.md` | sart -> test katman eslemesi |
| 3 | `.templates/new-doc-contract.md` | frontmatter ve link sozlesmesi |
| 4 | `AGENTS.md` | `architect` profili ve gecis kapi (§1.1 / §1.3) |
