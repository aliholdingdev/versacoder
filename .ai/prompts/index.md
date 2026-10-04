---
title: "Prompt İndeksi — VersaCoder prompts/"
type: index
category: prompts
status: active
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "Ajan gorev sablonlarinin (prompt kopyalari) kayit yeri: ne tur prompt burada yasar, ornek basliklar ve sablon baglantilari; 2026-10-02'de 15 kod prompt'u (l0..l7 + LLM + terminal + scaffold K0..N4) eklendi"
---

# Prompt İndeksi (prompts/)

**Zorunlu Bağlantılar:** [AGENTS](../AGENTS.md) · [agents-template](../.templates/agents/agents-template.md) · [subagent-template](../.templates/agents/subagent-template.md) · [Şablon envanteri](../.templates/index.md)

> **Tek cümlelik sorumluluk:** "Bir ajana verilecek görev prompt'u nerede durur, hangi şablonla yazılır?" sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`prompts/` yalnız **ajan görev kopyalarını** tutar: bir iş emri, bir ajana verilecek ve tekrar tekrar kullanılabilen görev metnidir. Şablonu `.templates/agents/` altındadır (`agents-template.md` · `subagent-template.md`); rol/yetki ve dağıtım matrisi [AGENTS](../AGENTS.md)'tedir — bu indeks ikisini de **tekrarlamaz, link verir** (SSUT). Klasör durumu (2026-09-29): `prompts/` = **30 dosya** (20 prompt + 9 yeni prompt + bu indeks); §3'teki dört başlık üretilmiş ve diskte mevcuttur (artık link verilir). Kök `.ai/index.md` de 2026-09-29'da diskte yok → bağlantı yazılmadı, düz metindir.

---

## §1 Ne Tur Prompt Buraya Yazılır

| # | Yazılır | Yazılmaz | Nereye gider |
|---|---|---|---|
| 1 | Ajana verilen görev metni (bağlam + görev + kabul kriteri + kanıt zorunluluğu) | ajan tanımı/kimliği | `.ai/AGENTS.md` |
| 2 | Tekrar kullanılan alt-ajan çağrısı (3-5 cümlelik kesin iş emri) | sohbet geçmişi/transkript | `.ai/MEMORY.md` (henüz yok) |
| 3 | Görev başına çıktı kutusu beklentisi (BAŞLIK/KAPSAM/KARAR/RİSK/KANIT) | gate kanıt kutusu tanımı | `.ai/WORKFLOW.md` §2 |
| 4 | Rol şablonuna göre üretilmiş örnek prompt (dolu, gerçek verili) | genel yazılmış tavsiye metni | `.rules/` (kural ise) |
| 5 | Prompt'un beklenen/gerçek çıktısı karşılaştırması | kanıtsız "çalışıyor" iddiası | `.ai/knowledge/index.md` (ders ise) |

**Sınır testi (ilk EVET yeter):** "Bu metin bir ajana **verilecek iş emri** mi?" → EVET = prompts; "Bu ajanın kendisi kim?" → `.ai/AGENTS.md`; "Bu iş tekrarlanan adım dizisi mi?" → `.ai/playbooks/index.md`.

---

## §2 Yazım Kuralları

| # | Kural | Kaynak |
|---|---|---|
| 1 | Her prompt: bağlam → görev → kabul kriteri → kanıt (4 blok) | AGENTS §2 çıktı kutusu |
| 2 | Kanıtsız tamamlandı cümlesi yasak; çıktı kutusuz iş emri işlenmez | AGENTS §2, CLAUDE G5 |
| 3 | Dosya adı `kebab-case.md`, ASCII, tek H1, frontmatter 8 alan | `.templates/new-doc-contract.md` |
| 4 | Şablon metni buraya kopyalanmaz; şablon linki verilir | SSUT / G12 |
| 5 | Ajan/matris bilgisi tekrar edilmez; [AGENTS](../AGENTS.md) §1 tek kaynak | AGENTS §1 |

---

## §3 Örnek Başlıklar (üretilmiş)

| # | Dosya adı | Görev tipi | Durum |
|---|---|---|---|
| 1 | [architect-prompt.md](architect-prompt.md) | mimari/karar incelemesi | üretilmiş |
| 2 | [developer-prompt.md](developer-prompt.md) | kod yazımı / refactor | üretilmiş |
| 3 | [tester-prompt.md](tester-prompt.md) | test yazımı ve kapsam | üretilmiş |
| 4 | [reviewer-prompt.md](reviewer-prompt.md) | inceleme + RİSK satırı | üretilmiş |
| 5 | [panel-devam-prompt.md](panel-devam-prompt.md) | panel devam kararı | üretilmiş |
| 6 | [wip-kontrol-prompt.md](wip-kontrol-prompt.md) | WIP limiti kontrolü | üretilmiş |
| 7 | [test-sonucu-prompt.md](test-sonucu-prompt.md) | test sonucu yorumu | üretilmiş |
| 8 | [seyirci-sef-prompt.md](seyirci-sef-prompt.md) | kod dışı gözlemci modu | üretilmiş |
| 9 | [genel-ozet-prompt.md](genel-ozet-prompt.md) | genel özet raporu | üretilmiş |
| 10 | [sezon-sonu-prompt.md](sezon-sonu-prompt.md) | sezon sonu kapanışı | üretilmiş |
| 11 | [gate-kapanis-prompt.md](gate-kapanis-prompt.md) | gate kapanış kanıt kutusu | üretilmiş |
| 12 | [saf-refactor-prompt.md](saf-refactor-prompt.md) | davranışı bozmayan refactor | üretilmiş |
| 13 | [risk-farkindaligi-prompt.md](risk-farkindaligi-prompt.md) | risk farkındalığı listesi | üretilmiş |
| 14 | [developer-agent-system-prompt.md](developer-agent-system-prompt.md) | developer agent system prompt | üretilmiş (2026-10-02, paralel oturum) |
| 15 | [l0-portlar.md](l0-portlar.md) | **l0 Abstractions portları (tam kod)** — 3 kanıt | üretilmiş (2026-10-02) |
| 16 | [l1-domain.md](l1-domain.md) | **l1 Domain varlıkları (invariant)** — 2 kanıt | üretilmiş (2026-10-02) |
| 17 | [l2-application.md](l2-application.md) | **l2 Application handler (TDD)** — 3 kanıt | üretilmiş (2026-10-02) |
| 18 | [l3-crosscutting.md](l3-crosscutting.md) | **l3 Crosscutting (R2: yalnız l0)** — 3 kanıt | üretilmiş (2026-10-02) |
| 19 | [l4-infrastructure.md](l4-infrastructure.md) | **l4 ADO.NET+Dapper (G2: EF yasak)** — 3 kanıt | üretilmiş (2026-10-02) |
| 20 | [l5-protocol.md](l5-protocol.md) | **l5 Protocol (JSON-RPC + SSE + tool round-trip)** — 3 kanıt | üretilmiş (2026-10-02) |
| 21 | [l6-host.md](l6-host.md) | **l6 Host (DI + bootstrap, R7)** — 3 kanıt | üretilmiş (2026-10-02) |
| 22 | [l7-cli.md](l7-cli.md) | **l7 Desktop + CLI (RibbonForm + Office2010 + U1-U10)** — 3 kanıt | üretilmiş (2026-10-02) |
| 23 | [llm-provider.md](llm-provider.md) | **OpenRouter + NVIDIA NIM adapter (VC-LLM-001..014)** — 3 kanıt | üretilmiş (2026-10-02) |
| 24 | [terminal-ui.md](terminal-ui.md) | **09-terminal dock (VC-PRD-040..042 + VC-CLI-015)** — 3 kanıt | üretilmiş (2026-10-02) |
| 25 | [scaffold-k0.md](scaffold-k0.md) | **K0 iskelet + 45 .csproj + 5000 kural (GATE-004)** — 3 kanıt | üretilmiş (2026-10-02) |
| 26 | [scaffold-n1.md](scaffold-n1.md) | **N1: 5 paket + 455 = 500 .csproj (GATE-005)** — 3 kanıt | üretilmiş (2026-10-02) |
| 27 | [scaffold-n2.md](scaffold-n2.md) | **N2: 7 paket + 1000 = 1500 .csproj** — 3 kanıt | üretilmiş (2026-10-02) |
| 28 | [scaffold-n3.md](scaffold-n3.md) | **N3: 7 paket + 1500 = 3000 .csproj** — 3 kanıt | üretilmiş (2026-10-02) |
| 29 | [scaffold-n4.md](scaffold-n4.md) | **N4: 10 paket + 2000 = 5000 .csproj + 5000² matris (GATE-006)** — 3 kanıt | üretilmiş (2026-10-02) |

> **2026-10-02 eklenti:** satır 15-29 = **15 yeni prompt** (8 katman + LLM provider +
> terminal UI + 5 scaffold fazı). Her prompt: 8 alanlı frontmatter + 3 kanıt +
> 7 ajan ataması (AGENTS §1) + spec gereksinim kimlikleri (VC-CLI/VC-LLM/VC-PRD/VC-LAY).

> İlk dört ad `10-bilinen-eksikler.md` §2'de rezerve ad olarak geçiyordu; 5-13
> numaralı satırlar 2026-09-29'da üretilmiştir. Bu tablo artık **gerçek dosya
> linkleridir** (G7 karşılanıyor: link yalnız üretilen dosyaya yazıldı).

---

## §4 Kabul Kriteri ve Yazım Akışı

| # | Kabul kriteri | Ölçüm |
|---|---|---|
| 1 | 4 blok dolu (bağlam/görev/kabul/kanıt) | blok sayısı 4/4 |
| 2 | İlgili şablon linki var (`agents-template` / `subagent-template`) | link diskte mevcut |
| 3 | Rol ataması [AGENTS](../AGENTS.md) §1 matrisiyle uyumlu | matris kontrolü |
| 4 | Çıktı kutusu beklentisi prompt'un içinde yazılı | §2 kutusu referansı |
| 5 | Bu indekste 1 satır kaydı | §3 tablosu |

**Yazım sırası:** iş tipini AGENTS §1 matrisine yerleştir → `.templates/agents/` şablonunu oku → 4 bloğu doldur → `check-links.ps1` → bu indekse kaydet.

---

## §5 Kapsam Dışı ve Boşluk Notu

| # | Kapsam dışı | Nerede |
|---|---|---|
| 1 | Ajan dağıtım matrisi, çıktı kutusu, 3-STRIKE, eşzamanlılık | `.ai/AGENTS.md` |
| 2 | Gate kanıt kutusu (`KANIT/DOSYA/EKSIK/SONRAKI`) | `.ai/WORKFLOW.md` |
| 3 | Genel belge/iskelet şablonları | `.ai/.templates/index.md` |
| 4 | Kök `.ai/index.md` (navigasyon) | 2026-09-29'da diskte YOK → link yazılmadı |

---

## §6 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Ajan politikası | `.ai/AGENTS.md` | §1 iş tipi → ajan matrisi, §2 çıktı kutusu |
| 2 | Agent şablonu | `.ai/.templates/agents/agents-template.md` | prompt üretim şablonu |
| 3 | Sub-agent şablonu | `.ai/.templates/agents/subagent-template.md` | alt-ajan iş emri şablonu |
| 4 | Şablon envanteri | `.ai/.templates/index.md` | şablon kaydı + §2.1 eşlemesi |
| 5 | Eksikler defteri | `.ai/reports/10-bilinen-eksikler.md` | rezerve adların bozuk-link kaydı |
