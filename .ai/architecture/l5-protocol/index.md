---
title: "l5 Protocol — Katman Şartnamesi (VersaCoder)"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l5 Protocol katmani: sorumluluk siniri, girenler/cikanlar, ADR-028 yasak yonleri, N33-N37 + N50, VersaCoder.Protocol, bagimlilik listesi ve test stratejisi"
---

# l5 Protocol — Katman Şartnamesi

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test stratejisi](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *VersaCoder ile ajan/LLM arasinda gecen **mesajin, aracin ve oturumun sekli** nedir; sekmaya aykiri cagri nasil reddedilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l5 **hicbir sey calistirmaz**: arac, mesaj, oturum, dongu ve surum/hata **semasini** tanimlar ve dogrular (fail-closed); eylem l4'tedir, gosterim l7'dedir, baglama l6'dadir. `protocolVersion` her zorunlu mesajda bulunur, eksikse mesaj gecerli degildir; hata kodlari tek `IErrorCatalog` icerinde yasar ve iki bacakta da ayni string'dir. Yon kurali: l5 **l6 ve l7'ye bakamaz** (matris X); **l3'e de bakamaz** (matris X); **l4 ile iliskisi `?`** — ADR-028 §2.1 zinciri `->` derken §2.3/2 `l5 -> l4 X` yazar → `DOĞRULAMA GEREKLİ`, `dependency-map.md` kazanir. Klasordeki 7 dosya: bu sartname (`index.md`), N33..N37 alt dugumleri ve N50 makro dugumu.

---

## §1 Sorumluluk

**Tablo A — Bu katman NE YAPAR**

| # | Sorumluluk | Kanıt | Çıktı |
|---|---|---|---|
| A1 | Arac semasi: `read/write/edit/bash/grep/glob/list/webfetch/task/todowrite` tanim kayitlari | `ToolDefinition` tipi + `Validate()` | tool katalogu |
| A2 | Mesaj tipleri: user/assistant/tool/tool_result/stream tek karede | round-trip testi | `MessageEnvelope` |
| A3 | Oturum/transkript semasi + baglam butcesi kurali | butce 101% iken kompakt | `SessionState` |
| A4 | Dongu politikasi: adim butcesi, derinlik, eszamanlilik, iptal | 4. adim asla baslamaz | `LoopDecision` |
| A5 | Surumleme + hata katalogu: breaking change = major | `(1.x <-> 2.0)` uyumsuz | `Result.Fail` |
| A6 | Sema disi cagriyi **reddetmek** (yorumlama yok) | `ProtocolViolation` vaka tablosu | fail-closed |

**Tablo B — Bu katman NE YAPMAZ**

| # | Yapmaz | Nerede | Neden |
|---|---|---|---|
| B1 | Dosya okuma/yazma, kabuk calistirma | l4 (N29/N32) | sema != eylem |
| B2 | HTTP/LLM cagrisi | l4 (N31) | ag altyapi |
| B3 | SQLite/SQL yazimi | l4 (N25..N28) | kalıcilik altyapi |
| B4 | Ribbon/Grid/WinForms | l7 | gosterim |
| B5 | DI kaydi, config okuma | l6 | composition root |

---

## §2 GIRENLER

| # | Giren | Kaynak | Hangi kapıdan geçer |
|---|---|---|---|
| 1 | `ToolCall` JSON | `l4` N31/N32 ya da `l7` N43 | `IToolCallSchema.FromJson` |
| 2 | `MessageEnvelope` | `l7` istemci | `protocolVersion` + `role` zorunlu |
| 3 | Uzak surum bilgisi | `l4`/`l7` | `IVersionNegotiation.Compatible` |
| 4 | Butce/limit ayari | `l6` N40 config | `ILoopPolicy`, `IContextWindow` |
| 5 | `CancellationToken` | `l6` agent dongusu | iptal -> `CANCELLED` |

---

## §3 CIKANLAR

| # | Çıkan | Alıcı | Biçim |
|---|---|---|---|
| 1 | Dogrulanmis `Result<ToolCall>` | `l4` N32 | gecerliyse eyleme gecer |
| 2 | `ProtocolViolation` / `ErrorCode` | `l4`, `l7` | tek hata katalogu (N37) |
| 3 | `SessionState` + `SummaryFrame` | `l6` dongu, `l7` gosterim | seq korunur |
| 4 | `LoopDecision` (Continue/Stop/Pause) | `l6` | `Result` ile |
| 5 | JSON cikti (round-trip) | iki bacak ayni | metin karsilastirma testi |

---

## §4 Yasak Yonler (ADR-028 matrisi)

| Yön hücrei | Değer | Anlam |
|---|---|---|
| `l5 -> l6`, `l5 -> l7` | **X** | ust katmana bakis yok |
| `l5 -> l3` | **X** | matris `l3 yalniz l0` kurali disinda tasiyamaz |
| `l5 -> l4` | **?** | §2.1 zinciri `->` vs §2.3/2 `X` → `DOĞRULAMA GEREKLİ` |
| `l5 -> l0`, `l1`, `l2` | `.` | gecisli; port ve tipler `l0` uzerinden |
| `l6 -> l5`, `l7 -> l5` | OK / `.` | tuketim serbest — sema **okunur**, l5 geri bakmaz |

```text
   l7 (gosterim)      l6 (baglama)
        \                /
         v              v   semayi uygular (tek yon)
              l5 PROTOCOL  --semayi uygular-->  l4 (calistirir)
         \              /
          yasak: l5 -> l6 / l7 / l3 (X) · l5 -> l4 (?) DOĞRULAMA GEREKLİ
```

---

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N33 | tool-schema | N36 | loop-policy |
| N34 | messages | N37 | versioning-errors |
| N35 | session-transcript | N50 | makro l5 (5 kit) |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

---

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Protocol` | `net48;net10.0-windows` | katmanin tek projesi (9 uretimden biri) |
| `VersaCoder.Tests` | çift bacak | sema/round-trip/surum testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | çift bacak | NetArchTest: `l5 -> l3/l6/l7` = 0 |

`net48` C# 7.3 alt kumesi (`record` yalniz `#if NET`), `net10.0-windows` C# 14. Iki bacakta **ayni serializer ve ayni JSON ciktisi** zorunlu (parite testi).

---

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tür |
|---|---|---|
| l5 | `VersaCoder.Abstractions` (l0): `Result`, `Error`, events-abi | izinli (`ProjectReference`) |
| l5 | `VersaCoder.Domain` (l1) / `VersaCoder.Application` (l2) tipleri | matriste `.` → gecisli |
| l5 | `VersaCoder.Crosscutting` (l3) | matris **X** → `dependency-map.md` |
| l5 | `VersaCoder.Infrastructure` (l4) | **?** → `DOĞRULAMA GEREKLİ` |
| l5 | `System.Text.Json` (NuGet) | surum `Directory.Packages.props` (G8) |

---

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | fail-closed vaka tablosu, `LoopPolicy` sinirlari | **%90** satır |
| Parite | JSON ↔ tip round-trip, iki bacak ayni cikti | 0 fark |
| Arch (NetArchTest) | `l5 -> l3/l4/l6/l7` kenari, dosya/HTTP/SQL cagrisi | **0 ihlal** |
| Contract | surum matrisi `(1<->1 ok, 1<->2 yok)` | 100% vaka |

Kosum: `dotnet test --filter Category=Protocol` · 13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

---

## §9 Kanit

| # | İddia | Kanıt komutu | Beklenen |
|---|---|---|---|
| 1 | Dosya boyutu 2.600–8.500 bayt | `Get-Item` ile `.Length` | aralık içinde |
| 2 | Bozuk link yok | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `0` |
| 3 | Frontmatter 9 alan, ilk satır `---` | gözden geçirme | 9/9 |
| 4 | N33..N37 + N50 + index = 7 dosya | klasör listesi | 7 |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | `0` |

`src/` henüz diskte olmadigindan kod bazli ihlal sayilari `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
