---
title: "N29 FS Tools"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N29 - dosya sistemi araclari (atomik yazma, kopyalama, yedekleme, yol guvenligi - V100/V101/V102/V105 temizkanal)
---

# N29 FS Tools

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N30-git-adapter.md](N30-git-adapter.md)
- [N31-llm-http.md](N31-llm-http.md)
- [N32-tool-runner.md](N32-tool-runner.md)
- [../../index.md](../../index.md)
- [../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N29, diske yazan tum katmanlar icin atomik, yol-guvenli ve yedekli dosya araclari saglar; temiz kanal (V100/V101/V102/V105) davranisinin dosya boyutundaki tek uygulayicisidir.

## §0 Ozet (Answer First)

- N29 iki davranisi tek yerde toplar: **atomik yazma** (once `.tmp`, sonra rename) ve **yol denetimi** (mutlak yol, traversal, izin).
- Duz metin ureten herkesin (N30 git, N41 edit, N25 rapor yazimi) gecmek zorunda oldugu ortak gecittir; `System.IO.File.*` dogrudan cagrisi baska katmanlarda **tavsiye edilmez**.
- **Sahip oldugu nuans:** V100/V101/V102/V105 "temiz kanal" davranisi dosya formati bazlidir ve N29'un girdi dogrulama sinirinda durur.
- Ilerleme kosulu: atomik yazma + traversal reddi testleri (§9 P29-2, P29-3).

## §1 Sorumluluk

| Tablo A - N29 yapar | Tablo B - N29 yapmaz |
|---|---|
| Atomik yazma (tmp + rename) ve yedek alma | Dosya iceriginin anlamini dogrulamak (uretici katman) |
| Yol denetimi: mutlak yol, `..` traversal, izin | Git semantigi (N30) veya HTTP (N31) |
| Kopyalama, tasma, klasor temizleme | Icerik uretimi (LLM/duz metin) |
| Cok buyuk dosya icin akis (stream) yazma | l2 politikasi kararlari (dosya kisitlari) |
| Sonucu `Result` ile vermek (fail-closed) | l6 dosya yolu kaynagi (config ornegi) |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Hedef dosya yolu | Cagiran katman (l4 ic) | Once denetlenir, sonra yazilir |
| Icerik (byte/metin) | Uretici (N30, N41, N25) | Atomik olarak diske konur |
| Izin ve mod ayarlari | Config / calisma ortami | Salt-okunur ve paylasim modlari |
| Yedek politikasi | N29 yapilandirmasi | Uzerine yazmadan once kopya |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Guvenli yazilmis dosya | Cagiran katman | Yarim dosya birakilmaz |
| Yedek kopya (`.bak`) | Geri alma islemi | Politikaya gore |
| Hata sonucu (Result) | Cagiran | Red / izin / yol hatasi ayristirilir |
| Olay kaydi | l0 (bayt seviyesi) | Yazma basarisi/hatasi gozlemi |

## §4 Yasak Yonler

| Yon | ADR-028 | N29 notu |
|---|---|---|
| l4 -> l2 | X | Politika metni okunmaz; N29 kendi kisitini uygular |
| l4 -> l5 | YOK | Protokol paketi dosyaya dokunamaz |
| l4 -> l7 | YOK | UI dosya yazamaz; `#if` sahibi `ITfmBridge` |
| l4 -> l0 | YOK / DOĞRULAMA GEREKLİ | Cagri bayt seviyesinde (NOT: olay yazilari l0'a gecer) |
| l3 -> l4 | YOK | `IMigrationHost` gecilmez |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N29 | Dosya araclari | Atomik + yol denetimi |
| N30 | Git adaptoru | Cagiran (commit oncesi yazma) |
| N31 | LLM HTTP | Cevap dosyaya bu uzerinden iner |
| N32 | Tool runner | Girdi/cikti dosyalarini bu uzerinden isler |

## §6 Projeler

| Proje | N29 katkisi |
|---|---|
| VersaCoder.Core | Ortak dosya yardimcisi (bagimlilik gider) |
| VersaCoder.Ado | Migration ve rapor yazimlari |
| VersaCoder.Sdk | Export/backup islemleri |
| VersaCoder.Web | Icerik ve rapor uretimi |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | BCL `System.IO` (standart kutuphane) | Kod seviyesi |
| l4 -> l0 | Olay/bayt yazimi | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l4 -> l3 | YOK | `IMigrationHost` gecilmez |
| l4 -> l7 | YOK | Geri donus yok |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Atomik yazma (kesinti simülasyonu) | `dotnet test` | Yarim dosya kalmaz |
| `..` traversal reddi | `dotnet test` | Yol denetimi |
| Mutlak yol ve izin hatasi | `dotnet test` | Result kodlari |
| Uzerine yazma + yedek | `dotnet test` | `.bak` uretimi |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N29 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P29-1 | `dotnet build` (ilgili proje) | 0 hata |
| P29-2 | `dotnet test` (atomik yazma testi) | GECTI - **DOĞRULAMA GEREKLİ** |
| P29-3 | Traversal testi (`..\..\x`) | Red - **DOĞRULAMA GEREKLİ** |
| P29-4 | `Get-Content ...\*.cs \| Select-String "File.WriteAllText"` (N29 disi yazanlar) | Listelenir ve gerekcelenir - **DOĞRULAMA GEREKLİ** |
| P29-5 | Yedekleme testi | `.bak` uretilir - **DOĞRULAMA GEREKLİ** |
