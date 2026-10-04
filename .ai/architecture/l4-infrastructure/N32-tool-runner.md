---
title: "N32 — Arac Calistirici (l4 Infrastructure)"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "N32 dugumu: read/write/edit/bash/grep araclari - izin haritasi, sandbox, timeout, cikti siniri; sorumluluk, giren/cikan, ADR-028 yasak yonleri ve test stratejisi (kayit tablosu DEGIL)"
---

# N32 — Arac Calistirici

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [kardes N31](N31-llm-http.md)

> **Tek cumlelik sorumluluk:** *`read/write/edit/bash/grep` araclarinin **izin, calisma dizini, sure siniri ve cikti boyutu** altinda nasil calistirildigi ve arac hatasinin **istisna degil veri** olarak dondugu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

N32, l5'teki `ToolDefinition` semasini **eyleme** cevirir: her cagri once `IToolPermissionMap`'ten izin alir, sonra sabit bir `cwd` sandbox'inda ve zorunlu timeout ile calisir, cikti **boyut siniriyla** kirpilir ve sonuc her zaman `ToolResult{ content, isError }` biciminde doner. Arac hatasi istisna **degildir** — veridir; hata akisi l5 kamarasina (N34) ayni bicimde akar. `IBashRunner` kabuk **calistirmez**, arguman dizisi alir; `sandbox disi yol`, `izin yok` ve `sure asimi` durumlarinda surec **baslamaz ya da `Kill()` ile biter**. Yon kurali: N32 l5'in semasini okur, l5 asla N32'ye bakmaz (ADR-028: `l5 -> l4` = X).

---

## §1 Sorumluluk

| # | Yapar | Kanıt | Çıktı |
|---|---|---|---|
| 1 | Izin haritasi kontrolu (arac + eylem bazli) | `write` kapaliyken surec sayisi 0 | `ToolResult(isError)` |
| 2 | Sandbox disiplini: sabit `cwd`, `..`/mutlak yol reddi | PATH_ESCAPE testi gecti | dosya degisikligi yok |
| 3 | Zorunlu timeout + `CancellationToken` ile `Kill()` | 5 sn komut + 1 sn timeout → surec olu | `PROCESS_TIMEOUT` |
| 4 | Cikti boyut siniri (ilk 8 KB stderr, 1 MB ust limit) | tasma testinde kirpma | `FILE_TOO_LARGE` |
| 5 | Atomik yazma / birebir metin degistirme (N29 ile) | yanlis offset'te dosya degismez | `isError=true` |

**Yapmaz:** mesaj/ sema dogrulamaz (l5), DI kaydetmez (l6), ekran acmaz (l7), prompt/model secmez, SQL calistiraz.

---

## §2 GIRENLER

| # | Giren | Kaynak | Koşul |
|---|---|---|---|
| 1 | `ToolCall` (ad + argumanlar) | l5 `IToolCallSchema` | gecersiz sema → cagrilmaz |
| 2 | Izin haritasi | `l6` config kopusu (N40) | varsayilan kapali, aciklama acik |
| 3 | `cwd` + `timeout` | cagiran / varsayilan 30 sn | sandbox koki disina cikilmaz |
| 4 | `CancellationToken` | l6 agent dongusu | iptal → `CANCELLED`, surec biter |
| 5 | Cevap dosya yolu (write) | cagiran | N29 atomik yazimi kullanilir |

---

## §3 CIKANLAR

| # | Çıkan | Alıcı | Hata yolu |
|---|---|---|---|
| 1 | `ToolResult{ content, isError }` | l5 N34 kamarasi | hata da ayni bicimde |
| 2 | `PERMISSION_DENIED` / `PATH_ESCAPE` | istemci (l7) | surec baslamaz |
| 3 | `PROCESS_TIMEOUT` + kill | l6 dongu politikasi (N36) | kismi cikti atilir |
| 4 | Dosya degisikligi (write/edit) | N29 atomik yazma | yarim dosya yok |
| 5 | Surec cikis kodu (0 disi) | `isError=true` | istisna **yok** |

---

## §4 Yasak Yonler (ADR-028 matrisi)

| Yön | Değer | Not |
|---|---|---|
| `l4 -> l5` | **X** | sema okunmaz, `l5 -> l4` de X'tir |
| `l4 -> l2` | **X** | use case/politika metni bilinmez |
| `l4 -> l7` | **X** | UI arac cagrisini dogrudan cekemez |
| `l2 -> l4` | **YOK** | port `l0`'da, uygulama `l4`'te |
| `l4 -> l0` | `?` | portlar `l0`'a yaziliyor → `DOĞRULAMA GEREKLİ` |

```text
  LLM (l5 semasi) --ToolCall--> ToolExecutor (N32) --izin yok?--> RED, surec 0
                                          |
                                          +--> cwd sandbox + timeout + Kill()
                                          +--> ToolResult{content,isError} -> N34
```

---

## §5 N Kodlari

| N | Alt katman | İlişki |
|---|---|---|
| N32 | tool-runner (bu dosya) | eylem katmani |
| N31 | llm-http | tool-call akisinin kaynagi |
| N29 | fs-tools | atomik yazma ortagi |
| N33 | tool-schema (l5) | sema kaynagi — yon `l4 -> l5` **X** |
| N49 | makro l4 | `ILlmKit` kit'i |

---

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Infrastructure` | `net48;net10.0-windows` | `ToolExecutor` + araclar |
| `VersaCoder.Tests` | çift bacak | izin/sandbox/timeout testleri |

`net48` C# 7.3 alt kumesi; `net10.0-windows` C# 14. Surec argumani (`Arguments` kacisi vs `ArgumentList`) ve `Kill(entireProcessTree)` yalnizca platform farkidir — izin ve sandbox kurallari **ortaktir** (`#if` G9).

---

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tür |
|---|---|---|
| N32 | `l0` portlari (`IToolPermissionStore`, `Result`) | izinli |
| N32 | N29 atomik yazma | katman ici |
| N32 | N31 HTTP istemcisi (webfetch) | katman ici |
| N32 | `l2`, `l5`, `l6`, `l7` | **yasak** |
| N32 | `Microsoft.Extensions.*` disi ek paket | paket **YOK** — yalniz BCL |

---

## §8 Test Stratejisi

| # | Test | Tip | Beklenen |
|---|---|---|---|
| 1 | `write` izni kapaliyken surec baslamaz | birim | surec sayisi 0 |
| 2 | `"a & del /q C:\"` argumani tek eleman kalir | birim | enjeksiyon yok |
| 3 | timeout asiminda surec `Kill()` ile biter | entegrasyon | `PROCESS_TIMEOUT` |
| 4 | edit yanlis offset → dosya **degismez** | birim | atomiklik |
| 5 | Layer siniri (13 soru) | mimari | [layer-boundary-check](../../checklists/layer-boundary-check.md) |

Kosum: `dotnet test --filter Category=Infrastructure`. `src/` diskte olmadigindan sayisal kanitlar `DOĞRULAMA GEREKLİ` kalir.

---

## §9 Kanit

| # | İddia | Kanıt | Beklenen |
|---|---|---|---|
| 1 | Dosya 2.600–8.500 bayt | `Get-Item .Length` | aralık içinde |
| 2 | Kardes link N31 + N49 gecerli | `check-links.ps1` | bozuk 0 |
| 3 | Frontmatter 9 alan | gözden geçirme | 9/9 |
| 4 | Izin/sandbox/timeout testleri | `dotnet test` | `src/` yokken `DOĞRULAMA GEREKLİ` |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | 0 |
