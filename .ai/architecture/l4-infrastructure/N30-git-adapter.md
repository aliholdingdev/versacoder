---
title: "N30 Git Adapter"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N30 - Git adaptoru (dalis komutlari, guvenli commit, temp alan yonetimi - V104 ambari)
---

# N30 Git Adapter

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N29-fs-tools.md](N29-fs-tools.md)
- [N32-tool-runner.md](N32-tool-runner.md)
- [N49-macro-l4.md](N49-macro-l4.md)
- [../../index.md](../../index.md)
- [../../spec/index.md](../../spec/index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N30, Git islemlerini (status, add, commit, dal) V104 ambari ve temp alan guvenligi kosuluyla calistirir; komut cagrisini N32 uzerinden yapar, yazmalari N29'a devreder.

## §0 Ozet (Answer First)

- N30 **Git CLI sarmalayicisidir**; Git'i kendisi yeniden uygulamaz, `git` komutlarini guvenli bicimde calistirir.
- Iki mutlak kural: (1) ambar **V104** ile sinirlidir, (2) calisma alani disina yazma ve **temp alan** (V104) kirilamaz.
- **Sahip oldugu nuans:** commit mesaji ve dal adi uretimi N30'un degil, istemcinin (l7 / N41) isidir; N30 yalnizca dogrular ve calistirir.
- Ilerleme kosulu: komutlarin `Result` ile donmesi ve V104/ambar testleri (§9 P30-2, P30-3).

## §1 Sorumluluk

| Tablo A - N30 yapar | Tablo B - N30 yapmaz |
|---|---|
| `git status/add/commit/branch` cagrilari | Commit mesaji / dag politikasi uretimi |
| V104 ambar sinirlamasini zorlamak | Depoyu GitHub'a itmek (ag politikasi disi) |
| Git ciktilarini `Result` olarak yorumlamak | Sema migration (N28) veya dosya politikasi (N29) |
| Temp alanini temizlemek (is bitince) | N24 SPT cagrisi (programlar ic gecittir) |
| Git bulunamazsa fail-closed hata | l2 imza / kimlik dogrulama |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Abar yolu ve dal | l6 config / istemci | Sinirlama ve hedef belirleme |
| Commit mesaji | Istemci (N41/l7) | Dogrulanir, aynen gecilir |
| Dosya degisiklikleri | N29 ile yazilanlar | `add` kapsamina alinir |
| Temp alan | N30 ic yonetim | Is bitince temizlenir |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Commit sonucu (hash, dal) | Istemci / UI | `Result` ile |
| `git status` ozeti | UI raporu | Metin olarak geri doner |
| Git hata sonucu | Cagiran katman | Git bulunamadi / ambar disi / cosma |
| Olay kaydi | l0 (bayt seviyesi) | Komut ve sonuc gozlemi |

## §4 Yasak Yonler

| Yon | ADR-028 | N30 notu |
|---|---|---|
| l4 -> l2 | X | Imza veya politika okumasi yok |
| l4 -> l5 | YOK | Protokol paketi Git'e dokunamaz |
| l4 -> l7 | YOK | UI, Git'i dogrudan cagiramaz |
| l4 -> l0 | YOK / DOĞRULAMA GEREKLİ | Gozlem bayt seviyesinde gecer |
| l3 -> l4 | YOK | `IMigrationHost` gecilmez |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N30 | Git adaptoru | Ambar + temp + komut |
| N32 | Tool runner | Komut calistirma gecidi |
| N29 | Dosya araclari | Yazma ve temizlik |
| N49 | L4 makro | Git makrolarinin altinda calisir |

## §6 Projeler

| Proje | N30 katkisi |
|---|---|
| VersaCoder.Core | Adaptoru barindiran katman |
| VersaCoder.Config | Ambar yolu, dal, Git binari |
| VersaCoder.Sdk | Export/commit akislarinin tuketicisi |
| VersaCoder.Web | Commit durumu raporlamasi |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | Git binari (harici surec) + N32 | Kod seviyesi |
| l4 -> l0 | Komut olaylari | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l4 -> l3 | YOK | `IMigrationHost` gecilmez |
| l4 -> l7 | YOK | Komutu yalnizca alt katman cagirir |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Abar disina commit reddi | `dotnet test` | V104 sinirlamasi |
| Temp alan temizligi | `dotnet test` | Is sonrasi kalinti yok |
| Git bulunamaz senaryosu | `dotnet test` | Fail-closed Result |
| Komut cagri dogrulamasi | `dotnet test` | N32 uzeri cagri |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N30 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P30-1 | `dotnet build` (ilgili proje) | 0 hata |
| P30-2 | `dotnet test` (ambar siniri testi) | GECTI - **DOĞRULAMA GEREKLİ** |
| P30-3 | Temp alan temizlik testi | Kalinti 0 - **DOĞRULAMA GEREKLİ** |
| P30-4 | `Get-Content ...\*.cs \| Select-String "ProcessStartInfo"` | Cagirilar N32'de toplanmis - **DOĞRULAMA GEREKLİ** |
| P30-5 | Git'siz ortamda cagri | Fail-closed Result - **DOĞRULAMA GEREKLİ** |
