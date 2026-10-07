---
title: "terminal-cli-surface - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "VersaCoder.Cli konsol yuzeyi: l0..l6 ortek cekirdek (ADR-010), ekran karsiliklari, CI/otomasyon senaryolari, ekrana gore islem tablosu"
---

# Terminal / CLI Yuzeyi

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [VISION](../../VISION.md) · [glossary](../../glossary.md) · [ADR-010](../../.decisions/accepted/ADR-010-cli-desktop-shared-core.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [keys](../../keys.md)
**Kardeşler (engine · 4/8):** [diff-approval-gate](diff-approval-gate.md) · [free-model-router](free-model-router.md) · [protocol-tool-schema](protocol-tool-schema.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Bir is ekrandan mi CLIden mi yapilir, cekirdek nerede ortaktir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`VersaCoder.Cli`, Desktop ile **l0..l6 cekirdegini birebir paylasir**; ayrismak yalniz `l7`'dedir ve cekirdege `DevExpress.*` / `System.Windows.Forms` referansi girmez (ADR-010). Glossary'de **Terminal** = "tek pencere icindeki komut satiridir" (glossary madde 90). Kullanim senaryosu: **CI, otomasyon, betik**. Ekran karsiliklari: `05-ai-chat` -> stdout/JSON akisi, `06-agent-panel` -> durum satiri + exit code, `07-settings` -> `--flag` + yerel ayar dosyasi.

```text
+---------------------------------------------------------------+
|        VersaCoder.Desktop            VersaCoder.Cli            |
|               |                            |                  |
|               +-------- l7 ayrismasi -------+                 |
|               |     (Desktop: WinForms)  (CLI: konsol)        |
|               +-------- ORTAK CEKIRDEK l0..l6 ----------------+|
|                      DevExpress.* / SWF referansi YASAK        |
+---------------------------------------------------------------+
```

---

## §1 Ekran karsiliklari

| Ekran | CLI karsiligi | Cikti bicimi |
|---|---|---|
| `05-ai-chat` | yanit akisini izleme | stdout / JSON akisi |
| `06-agent-panel` | headless calistirma | durum satiri + **exit code** |
| `07-settings` | ayar degistirme | `--flag` + yerel ayar dosyasi |
| `01-start-page` | oturum listesi ayni kaynaktan okunur | liste |
| `03` + `04` (dosya/diff) | **karsiligi yok** (CLI dosyayi gostermez) | `DOĞRULAMA GEREKLI` |

---

## §2 Islem tablosu (hangi is nerede)

| # | Islem | Ekran (GUI) | CLI |
|---|---|---|---|
| 1 | Oturumu listele / ac | `01-start-page` | ayni oturum listesi (`VC-SES-015`) |
| 2 | Gorev calistir / durdur | `06-agent-panel` | headless + exit code (ADR-010) |
| 3 | Yanit akisini izleme | `05-ai-chat` | stdout / JSON |
| 4 | Model / endpoint degistirme | `07-settings` | `--flag` (bayrak adlari `DOĞRULAMA GEREKLI`) |
| 5 | UI kodu / `.Designer.cs` | `05` -> onay -> `06` -> `04` (F3) | **CLIden yapilmaz** (U5) |
| 6 | Vault belgesi duzenleme | ekran degil — ajan isi | yok |

**Ortak kural:** ayni veri **iki yerde ayri ayri guncellenmez**; ekran ne yazar CLI onu okur, tersi de gecerlidir. Farkli deger gosterirse cakisma protokolu (G10) calisir, **son deger yazilmaz**.

---

## §3 Kural tablosu

| # | Kural | Kaynak |
|---|---|---|
| 1 | Cekirdege `DevExpress.*` / `System.Windows.Forms` girmez | ADR-010 |
| 2 | Ayrismak yalniz `l7`'dedir | VISION §2.2 |
| 3 | CLI sonucu exit code ile olculur (CI icin) | VISION §2.10 madde 2 |
| 4 | CLI ciktisi da kanit kutusuna girer; kanitsiz "bitti" yasak | G5 |
| 5 | Bayrak adlari (`--flag`) spec'te yoksa uydurulmaz | `DOĞRULAMA GEREKLI` |

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `VersaCoder.Cli` projesinde WinForms/DevExpress referansi = 0 | proje dosyasi |
| 2 | CLI ve Desktop ayni oturum listesini okuyor | test / `VC-SES-015` |
| 3 | Headless gorev exit code donduruyor | CI ciktisi |
| 4 | Bayrak adlari spec'te tanimli ya da etiketli | dosya kontrolu |
| 5 | Her iki host'ta `dotnet build` -> 0 hata | build ciktisi |

---

## §5 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Streaming ciktisi (05) | [agent-streaming-sse](agent-streaming-sse.md) |
| Oturum / kanit kapanisi | [session-checkpoint-persistence](session-checkpoint-persistence.md) |
| Paralel sinir (CI'da da gecerli) | [parallel-batch-limit](parallel-batch-limit.md) |
| Terimler | [glossary](../../glossary.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `VISION.md` §2.2 | CLI cekirdek paylasimi, ekran karsiliklari, kullanim senaryosu |
| `VISION.md` §2.10 | 8 satirlik islem tablosu, ortak kural (G10) |
| `glossary.md` madde 90 | Terminal tanimi |
| `.decisions\accepted\ADR-010-cli-desktop-shared-core.md` | l0..l6 ortek cekirdek |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; `--flag` bayrak adlari ve CLI-diff karsiligi bu etiketle kalir.
