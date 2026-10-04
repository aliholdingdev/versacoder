---
title: "debug-hatayi-bulma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Derleme/test/runtime hatasini kok neden bulma - tekrar uretim, daraltma, 3-STRIKE ve kanit"
---

# debug-hatayi-bulma — Hata Kok Neden Bulma

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [dual-tfm-build](../playbooks/dual-tfm-build.md) · [performans-olcumu](../playbooks/performans-olcumu.md)

> **Tek cumlelik sorumluluk:** *Bir hata nereden geldigi bilinmeden nasil kapatilir, ne zaman durulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sira degistirilemez: **hata metnini aynen al -> tekrar uret -> tek farkla daralt -> kok nedeni adlandir -> tek duzeltme -> kanit**. Tahmini yama yasaktir; ayni hata **3 basarisiz denemeden sonra** kod yazmak durur ve `3-STRIKE` raporuyla disari cikilir (AGENTS §4). Kanit = build/test ciktisi, "gecti" yazimi degil (G5). Basari ≈15 dk, kronik hata ≈45 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Hata metni aynen alinmis | kopyalanmis 1-3 satir cikti |
| 2 | Son degisiklik listesi | `git status --porcelain` + `git diff` |
| 3 | Reproduksiyon yolu | ayni hatayi veren tek komut |
| 4 | Build/test yeşil basliyor | onceki `0 Hata` / `Basarisiz: 0` kaydi |
| 5 | Ilgili kural/ADR hazir | [rules index](../.rules/index.md) icindeki ilgili kural |

---

## §2 Adimlar

**Toplam ≈15-45 dk** · yapan: `developer` · 3. denemeden sonra: dur + rapor

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Hati tekrar uret + kaydet | hatayi veren komutun tam ciktisi (tahmin degil) | 3 dk |
| 2 | Daralt: proje, TFM, test, satir | `dotnet build <sln> -f net48 -v n` (ayrica `-f net10.0-windows`) | 5 dk |
| 3 | Kok nedeni adlandir | `git diff --stat` + supheli dosyanin okunmasi -> tek varyant | 5 dk |
| 4 | Tek duzeltme + iki bacak kanit | `dotnet build` + `dotnet test` -> 2 x `0 Hata`, 2 x `Basarisiz: 0` | 8 dk |
| 5 | Dur veya devret | 3. basarisiz denemede `3-STRIKE` raporu yazilir | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Hata tekrar uretilmiyor | Farkli ortam / temizlenmis `bin-obj` | Ayni komut, ayni TFM, ayni veri ile tekrar dene |
| 2 | Yama isliyor ama kok neden farkli | Belirtiyle ugrasilmis | Daraltma adimina don, tek farkla tekrar |
| 3 | 3 deneme de dustu | Yontem degismemis (AGENTS §4) | Yontemi degistir; 3. denemede `DUR` |
| 4 | Duzeltme build'i kirdi | Tek bacakta dogrulanmis | §2 adim 4 iki bacakta tekrarlanir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yanlis yama | commit edilmemis -> `git checkout -- <dosya>`; edilmis -> `git revert <sha>` |
| 2 | Gecici kapatma (bosta yakalama) | blok kaldirilir, hata yeniden uretilir |
| 3 | Test kisaltildi (skip) | skip kaldirilir, orijinal test geri gelir |
| 4 | Log/olcum eklendi | seviye/kanal eski haline doner |
| 5 | Rapor / `log.md` | `REVERT <tarih>` satiri eklenir; 3-STRIKE raporu silinmez (iz) |

---

## §5 Kabul Kriteri

- [ ] Reproduksiyon komutu + birebir hata metni dosyada
- [ ] Konum `dosya:satir` + bacak (net48 / net10.0-windows) olarak yazildi
- [ ] Kok neden tek varyant olarak adlandirilmis (`UNKNOWN`/`NEEDS_VALIDATION` etiketi varsa isaretle)
- [ ] Duzeltme kaniti: 2 x `0 Hata` + 2 x `Basarisiz: 0`
- [ ] 3 basarisiz denemeden sonra kod yazimi durdu ve rapor yazildi

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| BASLIK : Oturum testi UTC kaydirma duzeltildi    |
| KAPSAM : src/.../SessionStore : 42 -> 51          |
| KARAR  : donusturme tek noktada toplandi          |
| RİSK   : eski kayitlar yerel saatte olabilir      |
| KANIT  : dotnet test -> Gecen=42, Basarisiz=0     |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Iki bacak build | `dotnet build <sln> -f net48` ve `-f net10.0-windows` | 2 x `0 Hata` |
| 2 | Iki bacak test | `dotnet test <sln> -f net48` ve `-f net10.0-windows` | 2 x `Basarisiz: 0` |
| 3 | Deneme siniri | AGENTS §4 | 3. strike'ta `DUR` |
| 4 | Butce | adim tablosu | basari ≈15 dk, kronik ≈45 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Performans sikayeti (yavaslik) | [performans-olcumu](../playbooks/performans-olcumu.md) |
| 2 | Testin kendisinin yazimi | [dual-tfm-test](../playbooks/dual-tfm-test.md) |
| 3 | Rollback adimlari | [rollback-geri-alma](../playbooks/rollback-geri-alma.md) |
| 4 | Bloke raporu bicimi | [AGENTS](../AGENTS.md) §4 sablonu (SSOT) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `AGENTS.md` | 3-STRIKE kurali, strike sayilmayanlar, cikti kutusu |
| 2 | `WORKFLOW.md` | hata modlari tablosu (§4.1) ve etiketler |
| 3 | `.rules/index.md` | hataya karsilik gelen kural dosyasi |
| 4 | `playbooks/index.md` | adim tablosu + sure + geri alma zorunlulugu |
