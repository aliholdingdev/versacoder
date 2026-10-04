---
title: "N31 LLM HTTP"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N31 - LLM HTTP istemcisi (streaming cevap, zaman asimi, iptal, yanit kaydi)
---

# N31 LLM HTTP

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N29-fs-tools.md](N29-fs-tools.md)
- [N32-tool-runner.md](N32-tool-runner.md)
- [N49-macro-l4.md](N49-macro-l4.md)
- [../../index.md](../../index.md)
- [../../spec/index.md](../../spec/index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N31, LLM saglayicisina HTTP istemcisi olur; streaming cevabi iptal edilebilir bicimde tuketir, zamani asimini ve hatayi `Result` ile dondurur, yaniti N29 ile diska guvenli yazar.

## §0 Ozet (Answer First)

- N31 **saglayici tarafindan bilinmez**: endpoint, model ve anahtar l6 config'ten gelir; kod icine **API anahtari gomulmez**.
- Streaming (`text/event-stream`) ve tek seferlik cevap iki modudur; ikisinde de **iptal (CancellationToken)** birinci siniftir.
- **Sahip oldugu nuans:** `l1 -> l0` bagimliligi geregi yanit boyutu ve sure metrikleri bayt seviyesinde l0 kanalina yazilir.
- Ilerleme kosulu: zaman asimi/iptal testleri + anahtar sızıntısı taramasi (§9 P31-2, P31-3).

## §1 Sorumluluk

| Tablo A - N31 yapar | Tablo B - N31 yapmaz |
|---|---|
| HTTP/1.1 SSE streaming ve chunk birlestirme | Prompt tasarimi veya model secimi (isteminci) |
| Zaman asimi, tekrar (retry) ve iptal | API anahtarini uretmek/saklamak (l6 sifre) |
| Yaniti N29 ile atomik diska yazmak | Dosya format dogrulamasi (N29/V100-V105) |
| Saglayici hatalarini `Result` koduna cevirmek | Tool calistirma (N32) |
| Yanit metriklerini l0 kanalina vermek | l2 politika metni okumak |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Endpoint + model + anahtar | l6 config | Istek basligi (`Authorization`) |
| Prompt / mesaj listesi | Istemci (N41/l7) | Govde olarak gonderilir |
| `CancellationToken` | Cagiran | Iptal ve zaman asimi |
| Cevap dosya yolu | Cagiran | N29 atomik yazimi |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Streaming token akisi | l6 / istemci | `IAsyncEnumerable` benzeri akis |
| Tam cevap metni | N41 / rapor | N29 ile dosyaya |
| Hata/iptal sonucu (Result) | Cagiran | Zaman asimi, 4xx/5xx, iptal |
| Metrik (sure, bayt) | l0 | Bayt seviyesi akis |

## §4 Yasak Yonler

| Yon | ADR-028 | N31 notu |
|---|---|---|
| l4 -> l2 | X | Kimlik/politika kodu okunmaz |
| l4 -> l5 | YOK | N35 istemcisiyle iliski yok |
| l4 -> l7 | YOK | UI token akisini dogrudan cekemez |
| l4 -> l0 | YOK / DOĞRULAMA GEREKLİ | Metrikler bayt seviyesinde |
| l3 -> l4 | YOK | `IMigrationHost` gecilmez |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N31 | LLM HTTP istemcisi | Streaming + iptal |
| N29 | Dosya araclari | Yanit yazimi |
| N32 | Tool runner | Arac cagri akisi (LLM tool-call hedefi) |
| N49 | L4 makro | LLM makrolarinin alti |

## §6 Projeler

| Proje | N31 katkisi |
|---|---|
| VersaCoder.Core | Istemci ve `Result` cevrimicisi |
| VersaCoder.Config | Endpoint, model, anahtar (sifre disarida) |
| VersaCoder.Web | Sunucu tarafinda cevap akisi |
| VersaCoder.Sdk | Akisli cevap kaydi ve export |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | `HttpClient` / SSE (BCL + paket) | Kod seviyesi |
| l4 -> l0 | Metrik ve olay yazimi | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l4 -> l3 | YOK | `IMigrationHost` gecilmez |
| l4 -> l7 | YOK | Geri donus UI'a dogrudan degil |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Streaming birlestirme | `dotnet test` | Mock SSE akisi |
| Zaman asimi | `dotnet test` | Simule gecikme |
| Iptal (CancellationToken) | `dotnet test` | Erken vazgecme |
| Anahtar sızıntısı taramasi | Statik arama | Kodda anahtar yok |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N31 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P31-1 | `dotnet build` (ilgili proje) | 0 hata |
| P31-2 | `dotnet test` (zaman asimi + iptal) | GECTI - **DOĞRULAMA GEREKLİ** |
| P31-3 | `Get-Content ...\*.cs \| Select-String "sk-\|api[_-]key\s*="` | 0 eslesme |
| P31-4 | Streaming testi (parca birlestirme) | Tam metin eslesir - **DOĞRULAMA GEREKLİ** |
| P31-5 | Yanit yazim testi (N29 atomik) | Yarim dosya yok - **DOĞRULAMA GEREKLİ** |
