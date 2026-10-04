---
title: "dual-tfm-test - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "net48 + net10.0-windows ikili bacak test - komutlar, sayac kaniti, mimari test ve geri alma"
---

# dual-tfm-test — Iki Bacak Test

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [spec index](../spec/index.md) · [dual-tfm-build](../playbooks/dual-tfm-build.md)

> **Tek cumlelik sorumluluk:** *Testler iki bacakta da yeşil mi, kaniti hangi sayaclardir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Test **tek bacakta gecti diye GATE-005 kapanmaz**: `net48` ve `net10.0-windows` icin ayri `dotnet test` kosulur, ikisinde de sayac (gecen/basarisiz/sure) raporlanir. Mimari test (katman yonu + circular) dahildir; coverage kaniti ya da kapsam-disi gerekcesi zorunludur (G5). Build yeşil degilse test kosulmaz - once [dual-tfm-build](../playbooks/dual-tfm-build.md). Testi `tester` yazar, PASS/RED karari `reviewer`'dadir. ≈20 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Iki bacak build yeşil | dual-tfm-build §5 maddeleri |
| 2 | Test projesi mevcut | `Get-ChildItem C:\versacoder -Recurse -Filter *Tests*.csproj` |
| 3 | Strateji + hedef okunmus | [spec index](../spec/index.md) uzerinden test stratejisi |
| 4 | Test yazilimi kilitli | xUnit + NSubstitute + NetArchTest (kilit dosyada) |
| 5 | Veri deterministik | seed'li veri, UTC zaman, I/O'suz birim test |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `tester` · degerlendirme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Bacak 1 testleri calistir | `dotnet test <sln> -f net48 -v m` -> `Gecti: N, Basarisiz: 0` | 5 dk |
| 2 | Bacak 2 testleri calistir | `dotnet test <sln> -f net10.0-windows -v m` -> ayni `N` | 5 dk |
| 3 | Mimari testi ayri dogrula | `dotnet test <sln> --filter "Category=Architecture"` -> 0 ihlal | 3 dk |
| 4 | Coverage / kapsam kaydi | `--collect:"XPlat Code Coverage"` -> xml + yuzde ya da gerekce | 4 dk |
| 5 | Kaniti raporla | iki bacak sayaci + sure devre paketinin `KANIT` maddesine | 3 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Iki bacakta farkli test sayisi | Kosum filtresi ya da TFM farki | Filtre kaldirilir, iki kosum ayni kapsamla tekrar |
| 2 | Mimari testte ihlal | Katman oku ADR-028 disi | Kod katman sozlesmesine cekilir, matris elle degistirilmez |
| 3 | `Skip` / `Assert.Ignore` var | Test kapatilmis | Skip kaldirilir ya da gerekce `EKSIK` satirina yazilir |
| 4 | Coverage yok, gerekce de yok | Olcum atlanmis | Ya xml uretilir ya da yazili gerekce eklenir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Test kodu yanlis (uretim kodu degil) | `tester` kendi dosyasini duzeltir, uretim `.cs`'ine dokunmaz |
| 2 | Uretim kodu test icin bozuldu | `git revert <sha>`; davranis degisikligi geri alinir |
| 3 | Test skip'lendi | skip + gerekce silinir, test geri acilir |
| 4 | Coverage hedefi dusuruldu | hedef eski degerine doner (test stratejisi dosyasi) |
| 5 | Filtre unutuldu | filtreli kosu tam kosuya cevirilir, kanit yeniden uretilir |

---

## §5 Kabul Kriteri

- [ ] `dotnet test -f net48` -> `Basarisiz: 0`
- [ ] `dotnet test -f net10.0-windows` -> `Basarisiz: 0` ve ayni gecen test sayisi
- [ ] `Category=Architecture` -> 0 ihlal (yon + circular)
- [ ] Coverage xml dosyasi + yuzde var ya da kapsam-disi gerekce yazilmis
- [ ] `Skip` / `Assert.Ignore` 0 (varsa gerekcesi `EKSIK` satirinda)

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : test -f net48        -> Gecti=42, 0 kaldi|
|          test -f net10.0-windows -> Gecti=42, 0   |
|          Category=Architecture -> 0 ihlal          |
| DOSYA  : tests/**                                  |
| EKSIK  : YOK                                       |
| SONRAKI : teslim raporuna gec (GATE-006, ~40 dk)   |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Bacak 1 | `dotnet test <sln> -f net48 -v m` | `Basarisiz: 0` |
| 2 | Bacak 2 | `dotnet test <sln> -f net10.0-windows -v m` | `Basarisiz: 0` |
| 3 | Mimari | `dotnet test <sln> --filter "Category=Architecture"` | 0 ihlal |
| 4 | Butce | adim tablosu | ≈20 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Build hatalari | [dual-tfm-build](../playbooks/dual-tfm-build.md) |
| 2 | Test stratejisi hedefleri | [spec index](../spec/index.md) |
| 3 | Kapsam/coverage raporlamasi | [WORKFLOW](../WORKFLOW.md) §2.4 (GATE-005) |
| 4 | Hatayi kokunden bulma | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `WORKFLOW.md` | GATE-005 kabul kriteri (5 madde) ve kanit kutusu |
| 2 | `checklists/gate-005-test-kapsami.md` | 14 adimlik test kontrol listesi |
| 3 | `checklists/layer-boundary-check.md` | 13 soru ile katman siniri denetimi |
| 4 | `AGENTS.md` | `tester` gorevi, kaniti ve gecis kapi (§1.3) |
