---
title: "ADR-032 — .ai Enterprise AI Engineering Control Plane"
type: adr
category: ".decisions"
status: proposed
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: ".ai katmanının hedef mimarisi: L0-L4, authority zinciri, VAULT-GATE, 56 karar"
tags: [adr, ai-layer, control-plane, architecture, governance, master]
source: "Onaylı mimari planı (2026-10-07) + D1..D56 karar defteri — yeni mimari kaydı"
supersedes: ""
---

## §0 Özet

`.ai` vault'u "documentation klasörü" değil, **AI Engineering Control Plane**'dir.
Bu ADR, `.ai` katmanının hedef mimarisinin **tek kaydıdır** (D54). 56 mimari
karar (D1..D56) bu ADR'de toplanır; çelişen her vault kaynağı bu ADR'ye döner.

## §1 Bağlam

2026-10-07 ölçümü: 281 MD · 763 kırık link · 6 eksik dizin · ADR-001..030 diskte
yok · 281 dosyanın hepsi `authority: SSOT` (281 SSOT = hiç SSOT) · G-ID'ler 3
yerde farklı anlamda · gate numaralandırması ikiye bölünmüş · Unicode çelişkisi.
Vault control plane olarak işlevsizdi.

## §2 Karar — Mimari Gövde

### 2.1 SSOT Modeli (D1)

`.ai` = AI bağlamı + mühendislik yönetişimi + **tam Control Plane**.
`src/**` kodu repo'da kalır; `.ai` onun **governed/expected state**'ini tanımlar.
Repository = actual state; drift = karşılaştırma (D25).

### 2.2 Authority Zinciri (D5)

```text
ADR  >  Kural (.rules/)  >  Gate (WORKFLOW/VAULT-GATE)  >  Türev (brain/memory/knowledge)
```

Çelişki = zincir otomatik + **BLOCKED raporu** + kullanıcı onayı (D29).
Çözüm yeni ADR/kural kaydı olarak audit'e düşer (D12).

`authority` frontmatter alanı değer seti (D51): `primary | derived | reference | shadow`.
Aynı kavramda iki `primary` = VAULT-GATE RED.

### 2.3 Katman Modeli — L0..L4 (D6)

| Katman | Ad | İçerik | Authority |
|---|---|---|---|
| **L0** | Governance & Authority | `.decisions/` (ADR) · `.rules/` (G-seti) · `governance/` (VAULT-GATE, WORKFLOW, audit, keys, versioning) · kök `CLAUDE.md` | primary |
| **L1** | Definitions & Identity | `definitions/`: ROLE · AGENTS · engine · glossary · VISION · PROJECTS · ULTRA-THINKING | primary/derived |
| **L2** | Knowledge & Requirements | `knowledge/` (8 domain) · `spec/` · `memory/` · `patterns/` · `architecture/` (product dokümanı, belgesel ilişki D36) | primary (olgu) / derived (memory) |
| **L3** | Artifacts | `.templates/` · `checklists/` · `ui-design/` | reference |
| **L4** | Operations | `scripts/` · `ops/` (log, TODOS, raporlar) · `archive/` | reference/shadow |

**Bağımlılık yönü:** L4→L3→L2→L1→L0. Alt katman yukarıdan içerik **kopyalayamaz**,
yalnız link/atanır (G1/G12). Circular = RED (D10).
**Authority yönü ters:** karar gücü L0'da.
`.ai` katmanları product `l0..l7`'den **bağımsızdır** (D36); ilişki belgeseldir.

### 2.4 Hedef Dizin Ağacı (D8/D9/D34/D38/D49)

```text
.ai/
├── index.md          # kategori haritası (kök)
├── CLAUDE.md         # anayasa girişi (kök)
├── .rules/           # [L0] G-seti tam metin (D20/D41/D46)
├── .decisions/       # [L0] draft/ accepted/ superseded/ + adr-index + ADR-001..032 + brain.md
├── governance/       # [L0] WORKFLOW · VAULT-GATE · audit · keys · versioning
├── definitions/      # [L1] ROLE · AGENTS · engine · glossary · VISION · PROJECTS · ULTRA
├── spec/             # [L2] requirement SSOT (D33)
├── architecture/     # [L2] product katman dokümanı (index + 00-overview + catalog + layer-5000 + l0..l7)
├── knowledge/        # [L2] 8 domain: csharp data devexpress winforms engine security testing ops
├── memory/           # [L2] MEMORY.md (derived)
├── patterns/         # [L2] öğrenilen kalıplar
├── .templates/       # [L3] envanter SSOT (D15)
├── checklists/       # [L3] + index.md
├── ui-design/        # [L3] spec eşleşmeli (D37)
├── ops/              # [L4] log · TODOS · VAULT-GATE raporları
├── scripts/          # [L4] 24 betik
├── archive/          # supersedes + eski ağaç (D26/D27/D28)
└── .obsidian/        # commit (D50b)
```

Kök depo dosyaları (CLAUDE/AGENTS/WORKFLOW/README) **gölge kalır**; `authority:
SSOT` etiketi kökten **kaldırılır** (D35).

### 2.5 Link Standardı (D50)

Vault içi: `[[wikilink]]` (Obsidian rename/backlink). Dış/vault-üstü atıf:
göreli MD `[t](../x.md)`. check-links.ps1 **iki biçimi** tarar.

### 2.6 Tüketici + Agent (D2/D11/D16/D30)

Tüketici = insan + Claude Code + external agent + VersaCoder runtime.
Context = `global → rol-paketi → task`; child parent'ı override edemez, yalnız
daraltır. Her `.agents` profilinde `allowed/forbidden context` açık listelenir;
**global > rol** üstünlüğü. External agent yazabilir; her yazma **VAULT-GATE**
filtresinden geçer (D13/D30).

### 2.7 Kapılar + Oturum (D13/D21/D24)

```text
VAULT-GATE → GATE-004a → GATE-003 → GATE-004 → GATE-005 → GATE-006
```

Oturum protokolü (5 adım): VAULT-GATE ön kontrol → rol-paketi yükle → iş →
onay (D18: 3 kademe) → audit+log. Commit öncesi VAULT-GATE yeşil.

### 2.8 Validation (D10/D40/D45)

10 ayrı betik: frontmatter · authority · claims · circular · orphan · duplicate
· stale · gate-bypass · architecture · wikilink + mevcut check-links/mojibake
(genleştirilmiş). VAULT-GATE bu betikleri çağırır; faz sonu rapor `.ai/ops/`'e.

### 2.9 Versioning (D14) + Frontmatter (D23)

3 seviye: vault-wide + dosya `version/updated` + ADR `status/date`.
Frontmatter **11 alan**: 8 mevcut + `tags` + `source` + `supersedes`.
ADR lifecycle: `draft → proposed → accepted → implemented → superseded → deprecated`.
`implemented` = D25 reconcile tetikleyicisi.

### 2.10 Knowledge/Memory Yaşam Döngüsü (D9/D28)

Bilgi eskidiğinde silinmez: `supersedes` ile işaretlenir → `archive/`'a →
stale taraması (D10) raporlar. Domain'ler: mevcut 5 + `security/testing/ops`
(D31); yeni domain açmak onaylı.

### 2.11 Güvenlik (D17)

Vault **salt referans**: API anahtarı/token/parola/connection string vault'ta
**yaşamaz**. `keys.md` yalnız tür + saklama yeri referansı. `secret-leak-check`
VAULT-GATE parçası.

### 2.12 Onay + Audit (D4/D12/D18)

Tüm `.ai` yazmaları onaylı. 3 kademe: düşük (paket) / orta (dosya) / yüksek
(etki + tek tek). Audit = `log.md` (append-only) + tek onay-kayıt dosyası
(`governance/audit.md`): kim/tarih/ne/öncesi-sonrası/onay ID.

### 2.13 Kurallar (D20/D41/D46)

Tek küresel G-seti; tam metin `.rules/`'te; kök/CLAUDE yalnız özet+link.
Şablon-içi yerel G numaralandırması kalkar. UI/UX kuralları ayrı
`.rules/ui-rules.md` (D46). `.rules/` kaynağı: vault içi kaynaklardan birleştir
(CLAUDE §1 + AGENTS 12 kural + brain 6 kural + kök özetler); her kural:
ID + metin + provenance + kapsam + ihlalde ne.

### 2.14 Şablon + İndeks (D15/D26/D52)

Şablon envanteri SSOT; tek yönlü üretim (şablon→doküman).
Keşfedilebilirlik: frontmatter tags + klasör index.md (elle) + supersedes +
archive. Betik denetler: orphan + kırık indeks linki = RED.

### 2.15 Ölçek (D19)

5000+ dosya için baştan kurulur: tags + indeks + supersedes + arşiv burada
tasarlandı; ek tagging/arama altyapısı LATER değil, şimdi bağlandı.

### 2.16 Unicode Politikası (D22 — ADR-029 revizyonu)

**Unicode kazanır**: U+2500..257F izinli/tercih. GATE-004a'nın ASCII kriteri
**revize edilmiştir**. İstisna: CLI/headless çıktısı ASCII kalır.

### 2.17 Boyut (D48)

Sayısal belge boyutu sınırı **yoktur**; içerik doğrusu. GATE-004a'nın 10-16 KB
kriteri revize edilmiştir.

## §3 Alternatifler

- **Statükoda kalma:** 763 kırık link + 281 SSOT + tanımsız authority. ❌
- **Aşırı enterprise (microservices/k8s vb.):** gerçek requirement yok. ❌
- **L0-L4 control plane (seçilen):** denetlenebilir + ölçeklenebilir + kanıtlı. ✅

## §4 Sonuçlar

- 6 fazlı geçiş (D42): arşiv → ADR-001..032 → .rules/+architecture/ → iskelet+taşıma
  → 10 betik+VAULT-GATE → onarım+final.
- Kabul = VAULT-GATE yeşil + 7 madde (D44): 10 betik · 0 kırık link · ADR'ler
  diskte · authority tek kaynaktan · G-çakışma 0 · FM 11 alan · audit+log.
- Taşıma güvenliği (D27/D55): git tag → 10-20 dosya batch → kontrol → RED'de rollback.

## §5 Kaynak

- Bu ADR'nin gövdesi = onaylı plan dosyası (2026-10-07) + 11 tur Q&A (D1..D56)
- Keşif kanıtı: envanter 295 dosya/281 MD; 763 kırık link (check-links)
- brain.md · .ai/CLAUDE.md · WORKFLOW.md · index.md (mevcut durum analizi)

## §6 İlgili

- [adr-index](../adr-index.md) · [brain](../../brain.md)
- [ADR-028](ADR-028-layered-baseline-9-projects.md) (product katman baz)
- [ADR-029](ADR-029-ascii-box-drawing-only.md) (D22 revizyonu bu ADR'de)
