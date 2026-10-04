---
title: "VersaCoder — opencode Uyarlama Raporu (PART 7)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "PART 7: opencode tool setinden C# sozlesmesine, ajan politikasi (AGENTS §5.1 kaza kaydi) ve LLM provider kararlari - kod durumu DOĞRULAMA GEREKLI"
---

# VersaCoder — opencode Uyarlama Raporu (PART 7)

**Zorunlu Baglantilar:** [index](index.md) · [00-kapsam-analizi](00-kapsam-analizi.md) · [01-web-kaynaklari](01-web-kaynaklari.md) · [05-katman-kodlama-raporu](05-katman-kodlama-raporu.md) · [AGENTS](../AGENTS.md) · [spec/index](../spec/index.md)

> **Tek cumlelik sorumluluk:** *opencode'un hangi parcalari C#'a uyarlanacak, ajan politikasinin bugunku gercekleri nedir ve LLM provider sirasi nasildir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Uyarlama kopyalama degil, sozlesme cevirisi: TypeScript kodu kopyalanmaz (ADR-002 / ADR-023 "opencode referans, salt okunur").** Bu vault'ta **spec dosyalari mevcut** (`spec/tool-spec.md`, `spec/tool-contract-spec.md`, `spec/protocol-spec.md`, `spec/session-spec.md`, `spec/llm-provider-spec.md`, `spec/context-budget-spec.md` — hepsi [spec/index](../spec/index.md) kayitli) ve **ADR-006..011 kabul edilmis durumda** (`.decisions/accepted/` icinde ADR-001..028, toplam 28 dosya). Buna karsilik **L5 (`VersaCoder.Protocol`) kodu 0 dosya** → uyarlamanin uygulama adimi **`DOĞRULAMA GEREKLI` / bekliyor**, `[KANIT_BEKLIYOR]` (G5). Ajan politikasi kaza kaydina gore yazildi: **en fazla 3 paralel ajan**, model **`opencode/mimo-v2.6-flash-free`**, rate limit dususu strike sayilmaz.

---

## §1 Ajan Politikasi Gercekleri (AGENTS.md)

| # | Kural | Deger | Kaynak |
|---|---|---|---|
| 1 | Eszamanlilik siniri | ayni anda **en fazla 3** ajan | AGENTS §5 |
| 2 | Kaza kaydi (2026-09-29) | **8 paralel** cagri → ilk yanit `Rate limit exceeded`, **8/8 ajan** `Model unavailable: anthropic/...` ile dustu | AGENTS §5.1 |
| 3 | Cozum 1 | paralellik **3'e** indirildi; is 3'erli batch'lerde calisti | AGENTS §5.1 |
| 4 | Cozum 2 | model **`opencode/mimo-v2.6-flash-free`** olarak sabitlendi | AGENTS §5.1 |
| 5 | Strike kurali | `Rate limit exceeded` / `Model unavailable` **strike sayilmaz** (dissal kesinti, ajan yontemi degil) | AGENTS §4.3 |

**Okuma:** 3 sayisi tahmin degil, olayla belirlendi; ayni hatanin tekrarinda `ETIKET: UNKNOWN` ile bloke raporu yazilir ve sayac isletilmez.

---

## §2 Tool → C# Sozlesme Haritasi

| opencode tool | C# karsiligi | Sozlesme | Durum |
|---|---|---|---|
| `read` | `ITool.ReadAsync(ReadArgs)` | `spec/tool-spec.md` | spec var / **kod yok** |
| `edit` | `ITool.EditAsync(EditArgs)` - parcali duzenleme, eslesme zorunlu | `spec/tool-spec.md` | spec var / **kod yok** |
| `write` | `ITool.WriteAsync(WriteArgs)` | `spec/tool-spec.md` | spec var / **kod yok** |
| `shell` | `ITool.ShellAsync(ShellArgs)` + timeout + sandbox | `spec/tool-contract-spec.md` | spec var / **kod yok** |
| `search` (ripgrep) | `ITool.SearchAsync(SearchArgs)` | `spec/search-grep-spec.md` | spec var / **kod yok** |
| tool registration | `IToolRegistry` - kesif + JSON schema dogrulama | `spec/tool-spec.md` §3-§4 | spec var / **kod yok** |
| permission gating | `IPermissionGate` - karar matrisi | `spec/tool-contract-spec.md` | spec var / **kod yok** |

Her cagri zinciri: **parametre dogrulama > izin kapi > sandbox kisiti > zaman asimi > hata sozlesmesi** (uydurma API yok, CLAUDE.md G9).

---

## §3 Ajan Modeli ve Dagitim

| Katman | Bileşen | Not |
|---|---|---|
| Vault rolleri | `architect` · `developer` · `reviewer` · `tester` · `ui-designer` · `general` | [AGENTS](../AGENTS.md) §1 dagitim matrisi |
| Cikti kutusu | `BASLIK / KAPSAM / KARAR / RISK / KANIT` (5 alan ASCII) | AGENTS §2; kutusuz cikti islenmez |
| Runtime agent | `VersaCoder.Protocol` - mesaj semasi, oturum, streaming | `spec/protocol-spec.md` · `spec/session-spec.md` |
| Dongu | tool cagrisi > sonuc > devam; context assembly + compaction | ADR-007 · ADR-008 |
| Dogrulama | her tool sonucu log'a; ajan karari kanitsiz "bitti" uretemez | CLAUDE.md G5 |

**Zincir kurali:** architect > developer > reviewer > tester; atlanan halkada kabul kriteri kanitsiz kalir (AGENTS §1).

---

## §4 LLM Provider

| # | Karar | Ayrinti | Kanit |
|---|---|---|---|
| 1 | **Tek contract** | `ILLMProvider` : chat + **SSE streaming** | 00-kapsam §2.1 karar 10 |
| 2 | **Ilk dogrulama** | OpenRouter `/free` router modeli | `01-web-kaynaklari.md` (S11) |
| 3 | Ikinci saglayici | Ollama (yerel) > OpenAI-uyumlu uclar | ADR-011 |
| 4 | Saglayici sayisi | 2 impl, tek interface (yalniz implementasyon degisir) | ADR-011 |
| 5 | Vault uzerinde calisan model | **`opencode/mimo-v2.6-flash-free`** | AGENTS §5.1 |

Anahtar yonetimi `keys.md` uzerindedir; uretim kodunda sabit string anahtar yasak (CLAUDE.md G2).

---

## §5 ADR-006..011 Durumu (bu vault)

| ADR | Konu | Beklenen cikti | Durum |
|---|---|---|---|
| ADR-006 | tool seti / tool sozlesmesi | `spec/tool-spec.md` + `ITool` | **kabul** (spec var, kod bekliyor) |
| ADR-007 | session yasam dongusu | `spec/session-spec.md` | **kabul** |
| ADR-008 | context assembly / compaction | `spec/context-budget-spec.md` | **kabul** |
| ADR-009 | tool calistirma sandbox'i | `spec/tool-contract-spec.md` | **kabul** |
| ADR-010 | cli/desktop paylasili cekirdek | iskelet [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) | **kabul** / uygulama bekliyor |
| ADR-011 | cok saglayicili LLM tek contract | §4 bu rapor | **kabul** |
| — | ADR-014 (slnx + CPM), ADR-023 (opencode salt okunur), ADR-025 (GATE-003 hard stop) | `.decisions/accepted/` | **kabul** |

> Kaynak vault'ta bu ADR'ler "yazilmadi" olarak raporlaniyordu; **bu vault'ta 28 ADR dosyasi mevcut** — durum farki §9'da olculdu.

---

## §6 Risk

| # | Risk | Etki | Kapanis kosulu |
|---|---|---|---|
| R1 | spec var, kod yok durumunun "bitti" sanilmasi | G5 ihlali; L5 0 dosya | [05-katman-kodlama-raporu](05-katman-kodlama-raporu.md) §1 kapanir |
| R2 | opencode TypeScript kodunun tasinmasi | ADR-002/ADR-023 ihlali, bakim iki yerde | yalniz sozlesme cevirisi; kod elle yazilir |
| R3 | paralelligin 3 sinirindan asilmasi | ayni kaza tekrar (8/8 dusus) | AGENTS §5 + §4.3 uygulanir |

---

## §7 Sure

| Is | Sahip | Tahmin |
|---|---|---|
| `ITool` + registry + permission gate (L5) | developer | ≈60 dk |
| `ILLMProvider` + OpenRouter `/free` dogrulamasi | developer | ≈30 dk |
| ADR-006..011 ile spec eslesme denetimi | architect | ≈30 dk |
| `reviewer` PASS + rapor | reviewer | ≈15 dk |
| **Toplam PART 7** | — | **≈2 sa 15 dk** (WORKFLOW §2.7) |

---

## §8 Kapsam Disi

| Bu rapor NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| opencode kaynaginin kendisi (kurulum, paketler) | `01-web-kaynaklari.md` · [00-kapsam-analizi](00-kapsam-analizi.md) |
| Ajan dagitim matrisinin tamami | [AGENTS](../AGENTS.md) §1 |
| L5 kodu ve test sonuclari | [05-katman-kodlama-raporu](05-katman-kodlama-raporu.md) |
| Vault olcumleri | [07-dogrulama-raporu](07-dogrulama-raporu.md) |
| Katman/proje sinirlari | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |

---

## §9 Kanit

| # | Kanit | Kaynak | Olcum |
|---|---|---|---|
| 1 | 8 paralel ajan kazasi + 3 sinir + model sabitleme | `AGENTS.md` §5.1 (2026-09-29 kaydi) | 8/8 ajan dustu |
| 2 | Rate limit strike sayilmaz | `AGENTS.md` §4.3 satir 1 (tablo) | 4 madde |
| 3 | Spec dosyalari mevcut | dizin listesi (`spec/`) | 40 `.md` |
| 4 | ADR-006..011 kabul | `.decisions/accepted/` dizin listesi | 28 ADR |
| 5 | L5 kodu yok | `src/` yok (2026-09-30) | 0 dosya |

Kaynak dosya adlari: `06-opencode-uyarlama-raporu.md` (bu dosya), `AGENTS.md`, `spec/tool-spec.md`, `spec/protocol-spec.md`, `spec/session-spec.md`.
