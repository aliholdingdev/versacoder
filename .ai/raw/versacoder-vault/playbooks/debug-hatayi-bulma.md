---
title: "debug-hatayi-bulma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Derleme/test/runtime hatasini kok-neden bulma - tekrar uretim, daraltma, 3-Strike kurali ve kanit"
---

# debug-hatayi-bulma — Hata Kök Neden Bulma

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [performans-olcumu](performans-olcumu.md)

> **Tek cumlelik sorumluluk:** *Bir hata nereden geldigi bilinmeden nasil kapatilir, ne zaman durulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sira: **hata metnini aynen al -> tekrar uret -> tek farkla daralt -> kok nedeni adlandir -> tek duzeltme -> kanit**. Tahmini yama yasak; **3 basarisiz denemeden sonra kod yazmayi birak** ve `3-STRIKE` raporuyla escalite et (AGENTS §8.1). Kanit = build/test ciktisi; "gecti" yazimi degil. Basari ≈15 dk, kronik hata ≈45 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Hata metni aynen alinmis | kopyalanmis 1-3 satir cikti |
| 2 | Son degisiklik listesi | `git status --porcelain` + `git diff` |
| 3 | Reprodüksiyon yolu | ayni hatayi veren komut |
| 4 | Build/test ortami yeşil basliyor | onceki `0 Hata` / `Basarisiz: 0` kaydi |
| 5 | Ilgili kural/ADR | hata turune gore `.rules/` (G2, G3, G6...) |

---

## §2 Adimlar

**Toplam ≈15-45 dk** · yapan: `developer` (3 denemeden sonra: kullaniciya escalite)

1. **Hati tekrar uret + kaydet** — adi belirsiz hata yok; komut + tam cikti + ilk satir.
   - Komut: hatayi veren komut (`dotnet build -v n`, `dotnet test ...`)
   - Cikti: birebir ayni hata metni (kopyalanmis, tahmin degil)

2. **Daralt** — hata dosyasina indir: hangi proje, hangi TFM, hangi test, hangi satir.
   - Komut: `dotnet build C:\versacoder\<sln> -f net48 -v n` (ayrica `-f net10.0-windows`)
   - Cikti: `dosya:satir` + bacak bilgisi (net48 mi net10 mu)

3. **Kok nedeni adlandir** — degistirilen tek dosya/varsayim; `UNKNOWN`/`NEEDS_VALIDATION` etiketini kullan.
   - Komut: `git diff --stat` + ilgili `.cs` dosyasini oku
   - Cikti: tek supheli varsayim, 1 cumlede adlandirilmis

4. **Tek duzeltme + kanit** — ayni tur 2. degisiklik ayni deneme sayilmaz ama **koda dokunmadan once** durum notu yazilir; duzeltme sonra iki bacak build+test.
   - Komut: `dotnet build` (iki bacak) + `dotnet test` (iki bacak)
   - Cikti: 2 x `0 Hata` + 2 x `Basarisiz: 0`

5. **Dur veya devret** — ayni hata 3. kez duzeltme denemesi basarisizsa **DUR**: `3-STRIKE` sablonu + supheli varsayim adiyla rapor et.
   - Dosya: `.ai\log.md` + rapor (`3-STRIKE` sablonu)
   - Cikti: `HATA / DENEMELER 1-3 / SUPHLI VARSAYIM / KANIT / SECENEKLER / SORU`

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Reprodüksiyon | hatayi veren komut + aynen cikti |
| K2 | Konum | `dosya:satir` + bacak (net48 / net10.0-windows) |
| K3 | Duzeltme kaniti | 2 x `0 Hata` + 2 x `Basarisiz: 0` |
| K4 | Davranis | ilgili test su an yeşil (once kirmiziydi) |
| K5 | 3-Strike | 3 denemeden sonra rapor var, kod yazimi durdu |

**Ornek cikti (K3 - zorunlu):**

```text
PS> dotnet build C:\versacoder\VersaCoder.sln -f net48 -v m
Derleme tamamlandı. 0 Hata.
PS> dotnet test C:\versacoder\VersaCoder.sln -f net48
Gecen: 42  Basarisiz: 0
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Yanlis yama | `git revert <sha>` (commit edilmemisse `git checkout -- <dosya>`) |
| 2 | Gecici kapatma (`try/catch` bos) | blokalari kaldir + hatayi yeniden uret |
| 3 | Test kisaltildi (skip) | skip'i kaldir, orijinal testi geri getir |
| 4 | Log/olcum eklendi | seviye/kanal eski haline dondur (log seviyesi `.rules/` isi) |
| 5 | `log.md` / rapor | `REVERT <tarih>` satiri ekle; 3-Strike raporunu silme (iz) |

