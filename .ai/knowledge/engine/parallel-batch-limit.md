---
title: "parallel-batch-limit - VersaCoder"
type: knowledge
category: engine
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Paralel ajan siniri (<= 3), blok/sira karar matrisi, ezme (write conflict) cozumu ve 2026-09-29 rate limit olayi"
---

# Paralel Batch ve Ajan Siniri

**Zorunlu Baglantilar:** [vault indeks](../../index.md) · [knowledge indeks](../index.md) · [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [ADR-007](../../.decisions/accepted/adr-007-agent-hierarchy-session.md) · [performance-guidelines](../../.rules/performance-guidelines.md)
**Küme + Hub:** [engine index](index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (engine · 1/8):** [agent-streaming-sse](agent-streaming-sse.md) · [diff-approval-gate](diff-approval-gate.md) · [free-model-router](free-model-router.md) · [protocol-tool-schema](protocol-tool-schema.md) · [session-checkpoint-persistence](session-checkpoint-persistence.md) · [terminal-cli-surface](terminal-cli-surface.md) · [wizard-approval-flow](wizard-approval-flow.md)

> **Tek cumlelik sorumluluk:** *Ayni anda kac ajan calisir, ayni dosyaya iki yazici gelirse ne olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sinir **en fazla 3 paralel ajan**dir. Bu sayi kestirim degil, **olculmus sonuctur**: 2026-09-29'da 8 paralel ajan ayni anda cagrildi, `Rate limit exceeded` ile hepsi dustu. Ayni dosyaya **tek yazici**; ezme (write conflict) varsa once seriley alinir. Paralel blokta bagimlilik 0 olmalidir; bagimli is siralanir.

```text
+---------------------------------------------------------------+
| 8 paralel ajan (2026-09-29) -> Rate limit exceeded -> 8/8 DUSTU |
| kok neden: fan-out > saglayici limiti + model erisilebilir degil|
| cozum: paralellik = 3 + model sabitlendi                       |
| sonuc: 3'erli batch'lerin TAMAMI GECTI                         |
+---------------------------------------------------------------+
```

---

## §1 Sinir tablosu

| # | Kural | Limit | Ihlalde |
|---|---|---|---|
| 1 | Ayni anda calisan ajan | **<= 3** | Rate limit -> is durur |
| 2 | Ayni dosyaya es zamanli yazan | **1** (tek yazici) | Ezilme varsa seriley al |
| 3 | Ayni oturumda dokunulan dosya | **<= 3** | Yeni oturuma bol |
| 4 | Blok icinde dokunulan dosya | **<= 8** | Bolumlere ayir |
| 5 | Paralel bloktaki bagimlilik | **0** | Bagimliysa sirala |

---

## §2 Paralel mi, seri mi (karar matrisi)

| Durum | Mod | Neden |
|---|---|---|
| Farkli katmanlarda bagimsiz isler (ADR-028 baglari dahil) | **PARALEL** | bagimlilik yok, kanit bagimsiz |
| Ayni `.cs` dosyasina iki yazici | **SERI** | cakisma kesindir |
| Ayni modul / ayni test dosyasi | **SERI** | ayni kanit satiri iki ajan uretir |
| Bagimli sir: `architect -> developer` | **SERI** | sozlesme yoksa kod tahminle yazilir |
| `l0 -> l1 -> l2` (BLOK A) | **SERI** | her katman oncekinin sozlesmesine bagli |
| `l3 \|\| l4 \|\| l5` (BLOK B) | **PARALEL (<=3)** | ucu birbirine dogrudan baglanmaz |
| `l6 -> (l7 Desktop \|\| l7 Cli)` (BLOK C) | l6 tek, sonra **PARALEL (<=3)** | DI kaydi iki kez degismesin |

---

## §3 Ezme (write conflict) cozumu

| Adim | Kural |
|---|---|
| 1 | **Tespit** — es zamanli yazim kaybi `KAPSAM` satirindan gorunur |
| 2 | **Kanit yarisi** — isinin kaniti olan (build/test ciktisi) kazanir |
| 3 | **Seriye alma** — kanit yoksa tek yazici kurali devreye girer |
| 4 | **Devir** — ikinci ajan 4 maddelik devre paketine gecer |
| 5 | **Sahiplik** — tek dosya = tek sahip; sahip disindaki okur, yazar degil |

---

## §4 Olay kaydi (2026-09-29)

| Alan | Deger |
|---|---|
| Deneme | ayni anda **8 paralel ajan** baslatildi |
| Belirti | `Rate limit exceeded`; 8/8 ajan `Model unavailable` ile dustu |
| Neden | saglayici hiz siniri + model erisilebilir degil |
| Mudahale | paralel sayisi **3'e** dusuruldu; kalan is seriye alindi |
| Kurala donus | 3 siniri kurala yazildi (AGENTS §5 + engine §2.1) |
| Kalan belirsizlik | hangi saglayicinin limiti doktugu ve ajan ID listesi `DOĞRULAMA GEREKLI` |

> 3 sayisi **olculmus sonuctur**; yeni eszamanlilik denemesi ancak `log.md`'ye tarihli kayitla ve kullanici onayiyla yapilir.

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Paralel cagri <= 3 | cagri kaydi |
| 2 | Ayni dosyaya ikinci yazici yok | KAPSAM satiri |
| 3 | Bagimlilik 0 ise paralel, degilse seri | karar matrisi |
| 4 | Blok sonunda her ajan 5 alanli kutusunu doner | kutu |
| 5 | Dussen is dissal kesinti mi, ajan yontemi mi? (strike sayaci) | AGENTS §4.3 |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `AGENTS.md` §5 / §5.1 | 3 siniri, 8 ajan olay kaydi, cozum maddeleri |
| `engine.md` §2.1 - §2.3 | sinir tablosu, karar matrisi, ezme akisi, blok semasi |
| `.decisions\accepted\ADR-028-layered-baseline-9-projects.md` | BLOK A/B/C paralellik duzeni |
| `.rules\performance-guidelines.md` | olcum/limit yaklasimi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; saglayici limit sayisi ve ajan ID listesi bu etiketle kalir.
