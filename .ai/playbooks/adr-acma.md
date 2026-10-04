---
title: "adr-acma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Yeni ADR acma - numara, Nygard sablonu, 2 alternatif, rules karsiligi, adr-index ve log kaydi"
---

# adr-acma — Yeni ADR Acma

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [adr-index](../.decisions/adr-index.md) · [rules index](../.rules/index.md) · [yeni-rule-acma](../playbooks/yeni-rule-acma.md)

> **Tek cumlelik sorumluluk:** *Bir mimari karar nereye, hangi sirayla ve hangi alanlarla yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

ADR = **geri alinabilir kararin kaydidir**; tek basina karari uygulamaz, `.rules/` maddesi karari zorunlu kilir. Yalniz `architect` acar; alternatif **en az 2**, sonuc hem `(+)` hem `(-)` yazilmazsa ADR eksiktir. Dosya `.decisions/accepted/ADR-0NN-<kebab-konu>.md` altina `status: accepted` ile girer, ayni gunde [adr-index](../.decisions/adr-index.md)'e satir duser. Toplam ≈20 dk; olcumsuz "kapatildi" beyani gecersizdir (G5).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Sonraki bos numara alinmis | adr-index dosyasindaki son satir |
| 2 | Karar kapsam disinda degil | `reports/00-kapsam-analizi.md` |
| 3 | Teknoloji kilitleri okunmus | [keys](../keys.md) §2 (net48 + net10.0-windows, v26.1, Dapper) |
| 4 | En az 2 alternatif hazir | kaba taslak: fikir basina 2 satir |
| 5 | Cakisma suphesi giderilmis | `.decisions/accepted/` altindaki dosya adlari |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `architect` · inceleme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Numara + dosya adi sec | `Get-ChildItem .ai\.decisions\accepted\*.md` -> bos numara | 3 dk |
| 2 | Nygard sablonunu doldur (Baglam / Karar / Alternatif >=2 / Sonuc) | `hedef_bacak: "net48 + net10.0-windows"` alani dolu | 8 dk |
| 3 | Karari zorunlu kilan kural maddesini yaz | `.rules/<ilgili>-rules.md` -> kural + ihlal + tespit komutu | 5 dk |
| 4 | Deftere kaydet | adr-index satiri + `log.md` tarihli not | 2 dk |
| 5 | Link ve format dogrula | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` -> `BOZUK link : 0` | 2 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Alternatif 1 adet | Karar zaten verilmis, arastirma yapilmamis | 2. alternatif yazilmadan dosya kapatilmaz |
| 2 | Sonuc yalniz `(+)` | Sonuc yazimi atlanmis | `(-)` maddeleri eklenir, sure yazar |
| 3 | Kuralsiz ADR | `.rules/` adimi gecmis | kural maddesi + tespit komutu eklenir |
| 4 | `BOZUK link > 0` | adr-index ya da kardes adi yanlis | hedef duzeltilir, tarama tekrar kosulur |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Karar henuz uygulanmadi | `status: accepted` -> `status: superseded` + gerekce + yerine gecen ADR |
| 2 | adr-index satiri | satir silinmez, `superseded` olarak isaretlenir |
| 3 | `.rules/` maddesi | maddedeki ADR referansi `[eski]` yapilir (iz takibi) |
| 4 | `log.md` kaydi | ustune `REVERT <tarih>` satiri eklenir |
| 5 | Koda yansimasi varsa | `git revert <sha>` + iki bacak build yeniden dogrulanir |

---

## §5 Kabul Kriteri

- [ ] Dosya ilk satiri `---`, 8 alanli frontmatter, `authority: SSOT`
- [ ] Alternatif basligi **en az 2**; her birinde `(+)` ve `(-)` maddeleri var
- [ ] `hedef_bacak: "net48 + net10.0-windows"` alani dolu
- [ ] Karari zorunlu kilan `.rules/` maddesi bu ADR'ye referans veriyor
- [ ] `check-links.ps1` -> `BOZUK link : 0`

---

## §6 Ornek Iskelet

```text
+--------------------------------------------------+
| ADR-0NN  <konu>                 status: accepted  |
| BAGLAM  : <neden bu karar, 1-2 satir>             |
| KARAR   : <secim>                                 |
| ALT. A  : <fikir>  (+) ...  (-) ...               |
| ALT. B  : <fikir>  (+) ...  (-) ...               |
| SONUC   : (+) ...  (-) ...                        |
| BACAK   : net48 + net10.0-windows                 |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Link sagligi | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 2 | Frontmatter | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1` | `NoFrontmatter = 0` |
| 3 | Kodlama | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` | `Mojibake file count: 0` |
| 4 | Sure | adim tablosu toplami | ≈20 dk (20 dk asiminda dur + rapor) |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Gate sirasi, kanit kutusu bicimi | [WORKFLOW](../WORKFLOW.md) §2 |
| 2 | Kural metni tam hali | `.rules/` dosyalari (link: [rules index](../.rules/index.md)) |
| 3 | Ajan dagilimi, cikti kutusu | [AGENTS](../AGENTS.md) §1 ve §2 |
| 4 | Yeni dokuman (spec/checklist) acma | ilgili kardes playbook'lar (§9) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `.decisions/adr-index.md` | numara sirasi ve kayit satiri bicimi |
| 2 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | dolu ADR ornegi (karar + alternatif + sonuc) |
| 3 | `.templates/new-doc-contract.md` | frontmatter 8 alan + link kurallari |
| 4 | `playbooks/index.md` | playbook kabul kriteri (adim tablosu, sure, geri alma) |
