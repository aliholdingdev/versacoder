---
title: "dual-tfm-test - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 + net10.0-windows ikili bacak test - komutlar, sayac kaniti, NetArchTest ve geri alma"
---

# dual-tfm-test — İki Bacak Test

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [test-strategy](../spec/test-strategy.md) · [dual-tfm-build](dual-tfm-build.md)

> **Tek cumlelik sorumluluk:** *Testler iki bacakta da yeşil mi, kaniti hangi sayaclardir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Test **tek bacakta gecti diye GATE-005 kapanmaz**: `net48` ve `net10.0-windows` icin ayri `dotnet test` kosulur, ikisinde de sayac (gecen/basarisiz/sure) raporlanir. Mimari test (NetArchTest: katman yonu + circular) dahil; coverage kaniti ya da kapsam-disi gerekcesi zorunlu (G5). Build yeşil degilse test kosma - once `dual-tfm-build`. ≈20 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Iki bacak build yeşil | `dual-tfm-build.md` §3 K1+K2 ciktisi |
| 2 | Test projesi mevcut | `Get-ChildItem C:\versacoder -Recurse -Filter *Tests*.csproj` |
| 3 | Strateji + hedef okunmus | `.ai/spec/test-strategy.md` |
| 4 | Test yazilimi kilitli | xUnit + NSubstitute + NetArchTest |
| 5 | Veri deterministik | seed'li veri, `TimeProvider` (UTC), I/O'suz birim test |

---

## §2 Adimlar

**Toplam ≈20 dk** · yapan: `tester`

1. **Bacak 1 testleri calistir** — once `net48`; hata varsa log'u kaydet, adim 3'e gecme.
   - Komut: `dotnet test C:\versacoder\<sln> -f net48 -v m`
   - Cikti: `Gecti: N · Basarisiz: 0 · Gecen sure: ...`

2. **Bacak 2 testleri calistir** — ayni testler `net10.0-windows` icin de kosar; ikisi de yeşil olmali.
   - Komut: `dotnet test C:\versacoder\<sln> -f net10.0-windows -v m`
   - Cikti: `Gecti: N · Basarisiz: 0` (bacak 1 ile ayni test sayisi)

3. **Mimari testi ayri dogrula** — katman yonu + circular dependency 0 ihlal (NetArchTest).
   - Komut: `dotnet test C:\versacoder\<sln> --filter "Category=Architecture"`
   - Cikti: `Basarisiz: 0` (ihlal listesi bos)

4. **Coverage / kapsam kaydi** — coverage yoksa "kapsam disi + gerekce" yazilir; kanitsiz "bitti" yasak.
   - Komut: `dotnet test C:\versacoder\<sln> -f net10.0-windows --collect:"XPlat Code Coverage"`
   - Cikti: `coverage.cobertura.xml` yolu + yuzde ya da gerekce

5. **Kaniti raporla** — iki bacak sayaci + sure, devre paketinin `KANIT` maddesine.
   - Dosya: `.ai/reports/` + devre paketi (4 madde)
   - Cikti: 2 test ciktisi + coverage satiri; "gecti" yazimi degil

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Bacak 1 | `dotnet test -f net48` -> `Basarisiz: 0` |
| K2 | Bacak 2 | `dotnet test -f net10.0-windows` -> `Basarisiz: 0` |
| K3 | Test sayisi esit | iki bacakta ayni `Gecti: N` |
| K4 | Mimari | `Category=Architecture` -> 0 ihlal |
| K5 | Coverage | `.xml` dosyasi + yuzde ya da yazili gerekce |

**Ornek cikti (K1+K2 - zorunlu):**

```text
PS> dotnet test C:\versacoder\VersaCoder.sln -f net48 -v m
  Gecen: 42   Basarisiz: 0   Atlanan: 0   Gecen sure: 00:00:03.120

PS> dotnet test C:\versacoder\VersaCoder.sln -f net10.0-windows -v m
  Gecen: 42   Basarisiz: 0   Atlanan: 0   Gecen sure: 00:00:02.480
```

> **NOT (2026-09-25):** test projesi henuz yok (`*.csproj` bulunamadi); iskeletle birlikte bu cikti uretilir - `NEEDS_VALIDATION`.

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Test kodu yanlis (uretim kodu degil) | `tester` kendi dosyasini duzeltir, `.cs`'e dokunmaz |
| 2 | Uretim kodu test icin bozuldu | `git revert <sha>`; davranis degisikligi geri alinir |
| 3 | Test skip'lendi (`[Skip]`) | skip'i kaldir + gerekceyi sil; gerekce yoksa kapsam kapaniyor |
| 4 | Coverage hedefi dusuruldu | hedefi eski degerine dondur (`test-strategy.md`) |
| 5 | Filtre/`--filter` unutuldu | filtreli kosu tam kosuya cevirilir, kanit yeniden uretilir |

