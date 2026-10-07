---
title: "yeni-spec-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "spec/ altinda yeni katman veya konu sartnamesi acma - sablon, kabul kriterleri, test-strategy bagi, defter kaydi"
---

# yeni-spec-acma — Yeni Sartname Acma

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [test-strategy](../spec/test-strategy.md) · [yeni-checklist-acma](yeni-checklist-acma.md)

> **Tek cumlelik sorumluluk:** *Yeni bir sartname nereye, hangi bolumlerle ve hangi kabul kriterleriyle acilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Spec = **"neyin dogru oldugunu"** yazar; nasil yapilacagi `playbooks/`, kural `.rules/` (SRP). Yeni konu ya da katman icin `.ai/spec/<konu>-spec.md` acilir, her sart **olculur kabul kriterine** baglanir, `test-strategy.md`'ye karsiligi not edilir. Kapsam degisikligi ise kullanici onayi ister (G10 / §3.2 #11). Toplam ≈30 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Kapsam dondurulmus | `.ai/reports/00-kapsam-analizi.md` (GATE-002) |
| 2 | Mevcut spec listesi okunmus | `.ai/spec/index.md` satirlari |
| 3 | Ilgili ADR var ya da acildi | `.ai/.decisions/accepted/ADR-0NN-*.md` |
| 4 | Test stratejisi erisilebilir | [test-strategy](../spec/test-strategy.md) |
| 5 | Kabul kriteri olculer hazir | komut/sayi/dosya (tahmin degil) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `architect`

1. **Konu + kategori sec** — katman mi (l0..l7) yoksa konu mu (oturum, tool, guvenlik...) karar ver; dosya adi `kebab-case`.
   - Ornek: `.ai\spec\session-spec.md`
   - Cikti: bos numara/ad, kardes dosyalarla cakismiyor

2. **Sablonu doldur** — frontmatter 8 alan + §0 Ozet + zorunlu baglantilar.
   - Sablon: `.ai/.templates/documentation/docs-md-template.md`
   - Cikti: ilk satir `---`, `type: spec`, `category: spec`

3. **Sartlari kabul kriterine bagla** — her sart ya bir komut ciktisi ya bir dosya ya da bir sayi ile kanitlanir; "dogru calisir" yasak.
   - Icerik: K1..Kn tablosu (sart · kanit · durum)
   - Cikti: olcusu olmayan sart = 0

4. **Test stratejisi + ADR bagi** — sartin hangi test katmaninda kanitlanacagini yaz (birim/entegrasyon/mimari).
   - Dosya: ilgili spec icinde `Test: [test-strategy](../spec/test-strategy.md) §N`
   - Cikti: her sartin bir test karsiligi ya da gerekcesi

5. **Defter + link taramasi** — `spec/index.md`'ye satir, sonra 0 bozuk link.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1`
   - Cikti: `BOZUK link : 0` + `spec/index.md`'de yeni satir

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Dosya acildi | `.ai/spec/<konu>-spec.md` var, ilk satir `---` |
| K2 | 8 alanli frontmatter | `title,type,category,status,version,updated,authority,owner,scope` |
| K3 | Kabul kriterleri | en az 1 tablo, her satirda kanit sutunu dolu |
| K4 | Defter satiri | `spec/index.md` -> `[<konu>-spec.md](<konu>-spec.md)` |
| K5 | Link | `check-links.ps1` -> `BOZUK link : 0` |

**Ornek cikti (K3):**

```text
| # | Sart | Kanit | Durum |
| K1 | Oturum 30 sn idle'da kaydedilir | dotnet test -f net10.0-windows -> 1 gecti | planli |
| K2 | Kayit iki bacakta ayni | dotnet test -f net48 -> 1 gecti | planli |
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yeni spec dosyasi | dosyayi sil + `spec/index.md` satirini geri al |
| 2 | Kapsam satiri eklendiyse | eski kapsam metnine don, farki kullaniciya raporla (kapsam yetkisi kullanici) |
| 3 | ADR bagi eklendiyse | ADR'de `superseded` isareti; spec'teki baglanti kalir (iz) |
| 4 | Kabul kriteri test urettiyse | test dosyalarini sil + `dotnet build` |
| 5 | `log.md` kaydi | ustune `REVERT <tarih>` satiri ekle |

