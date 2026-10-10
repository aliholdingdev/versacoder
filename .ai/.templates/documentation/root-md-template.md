---
title: "Şablon — Kök MD Belgesi (root-md)"
type: template
category: templates/documentation
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "kok MD sablonu: tek ekran ozet, ust/alt baglantilar ve durum satiri"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Şablon — Kök MD Belgesi (`root-md-template.md`)

**Zorunlu Bağlantılar:** [[.templates/index|index.md]] · [[claude|CLAUDE.md]] · [[.templates/documentation/docs-md-template|docs-md-template.md]]

> **Tek cümlelik sorumluluk:** `.ai/` kökündeki 14 zorunlu belgenin **30–88 KB** bandını, ortak iskeletini ve birbirinden farkını tanımlar.
> Şablon **kendi başına** bir kök belge değildir; kök belgeler bunu temel alır.

---

## §1 Neden Ayrı Şablon

| # | Gerekçe | Bozulursa |
|---|---|---|
| 1 | Kök MD'ler vault'un omurgası; ortalama 30–88 KB | Her biri kendi başına yazılır → kalite tutarsız |
| 2 | 14 dosyanın her biri *tek sorumluluk* taşır (SRP) | `index.md` + `brain.md` aynı bilgiyi barındırır → SSUT ihlali |
| 3 | `keys.md` zorunlu anahtar kelime haritasıdır | Arama çalışmaz, vault ölü olur |
| 4 | Hepsi aynı frontmatter + bağ satırı kullanır | Navigasyon kırılır |
| 5 | Ölçüm tek yerden yapılır | 30–88 KB bandı tutmaz |

---

## §2 14 Kök Belge Envanteri

| # | Dosya | Tek sorumluluk | Hedef KB | `type` | Doğrulama |
|---|---|---|---|---|---|
| 1 | `index.md` | Vault navigasyonu — tüm dosyaların indeksi | 30–45 | `index` | Her link hedefi mevcut |
| 2 | `AGENTS.md` | Ajan kaydı, yetkiler, handover protokolü | 30–40 | `registry` | Ajan sayısı `.agents/` ile eşit |
| 3 | `CLAUDE.md` | AI anayasası — hard guardrail'lar | 35–60 | `constitution` | G1..G12 tek tek tanımlı |
| 4 | `WORKFLOW.md` | Süreçler, fazlar, gate'ler | 35–65 | `workflow` | GATE-000..005 tanımlı |
| 5 | `ROLE.md` | Mutlak rol tanımı + roller arası öncelik | 30–50 | `role` | Rol çatışması kuralı var |
| 6 | `keys.md` | **Anahtar kelime → hedef dosya haritası** | 35–70 | `navigation` | Zorlu anahtarlar §3'te |
| 7 | `glossary.md` | Terimler sözlüğü (TR/EN) | 30–45 | `reference` | Her terim tek satır |
| 8 | `brain.md` | ADR özetleri + mühendislik kısıtları | 40–88 | `decisions` | Her ADR linkli |
| 9 | `MEMORY.md` | Kalıcı bellek — oturumlar arası durum | 30–50 | `memory` | Silme/kalıcılık kuralı |
| 10 | `VISION.md` | Ürün vizyonu, problem–çözüm matrisi | 30–50 | `vision` | 7 ekranla hizalı |
| 11 | `PROJECTS.md` | Proje kapsamı, hedefler, bileşenler | 30–50 | `project` | Kapsam sınırı açık |
| 12 | `log.md` | Kural değişikliği + kalite raporu günlüğü | 30–60 | `log` | Tarihli kayıt zorunlu |
| 13 | `engine.md` | Orkestrasyon yürütme matrisi | 30–50 | `engine` | Gate ↔ part eşleşmesi |
| 14 | `ULTRA-THINKING.md` | Derin düşünme protokolü + şablonlar | 30–45 | `protocol` | Adım sayısı somut |

**Toplam hedef:** 14 dosya · **~500–800 KB** · hepsi YAML frontmatter'lı.

---

## §3 Zorunlu Anahtar Kelimeler (`keys.md` hariç hepsinde geçmeli)

```
VersaCoder · VERSA CODER · agentic IDE · AI coding platform
RibbonControl · RibbonForm · RibbonControlStyle.Office2010 · DevExpress v26.1
Designer.cs · .resx · ResourceManager · WinForms
net48 · net10.0-windows · #if · iki bacak
ADO.NET · Dapper · parameterized · migration · version tablosu
Clean Architecture · SOLID · CQRS · DDD · DI
Protocol · Host · Abstractions · Domain · Application · Infrastructure
opencode · tool · agent · session · MCP · skill · streaming
Desktop · Cli · GATE · ADR · SSOT · frontmatter
```

---

## §4 İskelet (Kök MD'ler İçin)

```markdown
---
title: "<Belge Adı> — VersaCoder Vault"
type: <index|registry|constitution|workflow|role|navigation|reference|decisions|memory|vision|project|log|engine|protocol>
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "<tek cümle>"
---

# <Belge Adı>

**Zorunlu Bağlantılar:** [[index.md]] · [[AGENTS.md]] · [[CLAUDE.md]] · [[WORKFLOW.md]] · [[keys.md]] · [[brain.md]] · [[MEMORY.md]] · [[log.md]] · [[.templates/index]] · [[.agents/AGENTS.md]]

> **Tek cümlelik sorumluluk:** <bu dosyanın tek cevabı olduğu soru>

## İçindekiler
| Bölüm | Alt Konular |
|---|---|

## §1 Amaç
## §2 Kapsam
## §3 İçerik (ana gövde — 30 KB'lık kısım burada)
## §4 Kurallar
## §5 Doğrulama
## §6 Referanslar
```

---

## §5 KB Bandını Doldurma Yöntemi (Kopyalamadan)

30–88 KB'ı **kopyala-yapıştır olmadan** doldurmak için:

| # | Teknik | Örnek | Ek KB |
|---|---|---|---|
| 1 | **Karar tablosu** — her satır gerekçe + alternatif + sonuç | ADR özeti 25 satır | +3–6 |
| 2 | **Kapsam dışı listesi** — ne yapmadığını yaz | 10–15 madde | +2–4 |
| 3 | **Kontrol checklist'i** — ölçülebilir `[ ]` maddeleri | 20–40 madde | +3–7 |
| 4 | **Hata modları tablosu** | 8–12 satır | +2–5 |
| 5 | **ASCII diyagram** (teknik belgelerde) | 30–60 satır | +2–5 |
| 6 | **Örnek/girişim** — gerçek VersaCoder senaryosu | 5–10 blok | +4–10 |
| 7 | **Red team** — sık yapılan hatalar | 8–12 satır | +2–4 |

**Yasak dolgu:** CoreMusic içeriği kopyası · tekrar eden tablo · uydurma istatistik · "detaylıdır" lafı.

---

## §6 Doğrulama Checklist'i

- [ ] Frontmatter 11 alan + `authority: primary`
- [ ] "Zorunlu Bağlantılar" satırı 10 hedefi de içeriyor
- [ ] Boyut **30–88 KB** bandında (±%10)
- [ ] Tek H1, § numaralı bölümler
- [ ] Bozuk iç link yok
- [ ] EF Core geçmiyor; DevExpress yalnız **v26.1**
- [ ] `keys.md` §3'teki zorlu anahtarların **tamamını** içeriyor
- [ ] Kanıtsız "tamamlandı" yok

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Genel doküman şablonu | `.ai/.templates/documentation/docs-md-template.md` |
| 2 | Şablon envanteri | `.ai/.templates/index.md` |
| 3 | Ölçüm kanıtı | `.ai/reports/00-kapsam-analizi.md` |
| 4 | Bootstrap kaynağı | `G:\…\Versacoder\versacoder proje planaması ai.03.md` |
