---
title: "TODOS — Adım Adım Görev Fazları (Vault Görev Kaydı)"
type: index
category: todos
status: active
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "Tum vault task/todo/checklist kayitlarinin tek yerde toplandigi, siraya gore faz dosyalarina bolundugu ana indeks; kullanim kurali ('su dosyayi yap'), siralama gerekcesi ve guncel proje durumu"
tags: [ops]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# TODOS — Görev Fazları İndeksi

**Zorunlu Bağlantılar:** [index](../../index.md) · [WORKFLOW](../../governance/workflow.md) · [AGENTS](../../definitions/agents.md) · [VISION](../../definitions/vision.md) · [checklist index](../../checklists/index.md) · [gate-004a](../../checklists/gate-004a-vault-kapanis.md) · [gate-003](../../checklists/gate-003-ui-onay.md) · [adr-index](../../.decisions/adr-index.md)

> **Tek cümlelik sorumluluk:** Vault'taki hangi işin hangi faz dosyasında yaşadığı ve
> "şu dosyayı yap" denince ne yapılacağının tek cevabıdır.

---

## §0 Özet (Answer First)

Bu klasörde **7 dosya** var: **1 indeks + 6 faz**. Tüm fazlar **en temizden (bloke
olmayandan) en pahalıya** sıralıdır. Görevler vault'taki gerçek kaynaklardan
toplandı: `checklists/` (**24 liste — 2026-10-07'de dolduruldu**), `WORKFLOW` §2
gate sırası, ADR-028 §2.3, ADR-030. Uydurma görev yok.

### Güncel proje durumu (ölçüm 2026-10-07)

| Gösterge | Değer | Not |
|---|---|---|
| `.ai/` toplam MD | **282** | GATE-004a eşiği 500 → **şu an RED (282 < 500)** |
| `checklists/` | **24 MD** ✅ | önce **YOK**'tu → 2026-10-07'de üretildi |
| `knowledge/` | 134 MD | kümelenmiş hub bağlantısı tamam |
| `ui-design/` | 14 MD | wireframe dosyaları mevcut |
| `spec/` · `.templates/` | 45 · 36 MD | — |
| `src/` · `tests/` · `.sln` | **YOK** | GATE-004 başlamadı |
| `global.json` · `Directory.Packages.props` | **YOK** | `DOĞRULAMA GEREKLİ` |
| `.decisions/accepted/` (ADR-001..030) | **YOK** | yalnız `draft/ADR-031` var |
| git | 418 tracked · **0 src** | 3 commit |

### Kullanım kuralı (bu klasörün sözleşmesi)

| Kullanıcı der ki | Yapılan |
|---|---|
| `01-vault-kapanis dosyasını yap` | **Yalnız** o dosyanın adımları çalıştırılır; diğer dosyalara geçilmez |
| `devam et` | Sıradaki (en üstteki tamamlanmamış) faz dosyasına geçilir |
| `kaç kaldısın` | Bu §1 tablosundan `[ ]`/`[x]` sayılır |
| Faz biter | İlgili satır `[x]` olur, sonraki faz açılır (sıra **atlanmaz**) |

- **Tek dosya kuralı:** tek seferde **yalnız bir** faz dosyası işlenir; çok istek
  = sıra bozulur, kapılar (gate) atlanır.
- **Sıra neden sabit:** [WORKFLOW](../../governance/workflow.md) §2'deki gate sırası (vault →
  onay → iskelet → kod → test) aşılmaz; ters sıra = iş iki kez yapılır.

---

## §1 Faz Tablosu (sıra = öncelik)

| # | Dosya | Ne içerir | Kapı | Durum (2026-10-07) |
|---|---|---|---|---|
| 1 | [01-vault-kapanis.md](01-vault-kapanis.md) | 500 MD + bozuk link 0 + mojibake 0 + kök bant | GATE-004a | 🔴 **RED** — 282/500 MD |
| 2 | [02-5000-plani-sorulari.md](02-5000-plani-sorulari.md) | 5000+ katmanlı plan için 4 soru (kullanıcı cevabı bekler) | — | ⏸ bekliyor (kullanıcı cevabı) |
| 3 | [03-faz-d-iskelet.md](03-faz-d-iskelet.md) | `C:\versacoder` kod iskeleti: 9+2 proje, CPM, ilk commit | GATE-004 | ⛔ **BLOKE** — `src/` `tests/` `.sln` YOK |
| 4 | [04-gate-003-wireframe-onay.md](04-gate-003-wireframe-onay.md) | 7 wireframe kullanıcı onayı (HARD STOP) | GATE-003 | 🟡 **teyit gerekli** — `ui-design/` 14 MD var, log'da onay kaydı mevcut |
| 5 | [05-kod-ve-kalite.md](05-kod-ve-kalite.md) | Uygulama kodu, NetArchTest 8×8, test kapsamı | GATE-005 | ⛔ **BLOKE** — `src/` YOK |
| 6 | [06-5000-katman-kurulumu.md](06-5000-katman-kurulumu.md) | ADR-030: 5000+ tip katmanı doğrula → katalog → kod → gate | GATE-004a → 005 | 🔴 **RED** — ADR-030 dosyası diskte YOK |

**Sıra kuralı değişmedi:** faz 02, 1 bitmeden **açılmaz** (kullanıcı cevabı bekleyen
iş, vault işini bloklamaz). **Ama 01 şu an RED** — 282/500 MD; eşiğe ulaşmak için ya
eksik dizinlerin (`.rules` · `checklists` ✅ · `architecture` · `reports` · `playbooks` ·
`prompts`) üretilmesi ya da eşiğin (ADRsiz) yeniden gözden geçirilmesi gerekir →
**kullanıcı kararı**.

---

## §2 Kaynak Eşlemesi (her görev nereden geldi)

| Kaynak dosya | Nereye aktarıldı | Ne aktarıldı |
|---|---|---|
| [gate-004a-vault-kapanis.md](../../checklists/gate-004a-vault-kapanis.md) | faz 01 | 14 soruluk kapanış listesi |
| [bilinen-eksikler](../log.md) §5 | faz 01 | kapanış sırası + süre tahminleri |
| `measure-vault.ps1` / `check-links.ps1` / `find-mojibake.ps1` | faz 01 | sert kapı çıktıları |
| Kullanıcı talimatı (bu oturum) | faz 02 | 4 soru taslağı |
| ADR-028 §2.3 | faz 03 | 6 zorunlu düzeltme + 9+2 proje |
| [gate-004-ilk-commit.md](../../checklists/gate-004-ilk-commit.md) | faz 03 | commit kapısı |
| [gate-003-ui-onay.md](../../checklists/gate-003-ui-onay.md) | faz 04 | 7 ekran onay paketi |
| [gate-005-test-kapsami.md](../../checklists/gate-005-test-kapsami.md) | faz 05 | test kapsamı kapısı |
| [layer-boundary-check](../../checklists/layer-boundary-check.md) | faz 05 | 13 soru (NetArchTest) |
| [checklist index](../../checklists/index.md) | tüm fazlar | 24 listenin envanteri + referans sayacı |

- Kaynakta olmayan görev **bu klasöre yazılmaz** (uydurma yasak, G5).
- Her faz dosyasında §Kanıt, görevin hangi betik/çıktı ile doğrulanacağını yazar.

---

## §3 Durum Sayacı (2026-10-07 ölçümü)

| Ölçüm | Değer | Hedef | Durum |
|---|---|---|---|
| `.ai/` toplam MD | **282** | ≥ 500 (GATE-004a) | 🔴 **218 eksik** |
| Frontmatter'siz | **0** | 0 | ✅ |
| Mojibake (gerçek) | **0** | 0 | ✅ |
| `checklists/` | **24 MD** | — (önce YOK'tu) | ✅ **2026-10-07 doldu** |
| `knowledge/` | 134 MD | — | ✅ hub bağlantısı tam |
| Bozuk link | 752 | 0 | 🔴 — `.rules` · `architecture` · `reports` · `playbooks` · `prompts` + `accepted/` YOK |
| Kod (`src/` · `tests/` · `.sln`) | **0** | 9+2 proje | ⛔ başlamadı |
| Yazan ajanlar | ≤ 3 | ≤ 3 | ✅ |

**500 MD'ye nasıl ulaşılır (faz 01 RED'inin iki yolu):**

```text
YOL A (kullanici karari) : eksik 6 dizin uretilir
   .rules/  architecture/  reports/  playbooks/  prompts/  (+ accepted/ ADR'ler)
   -> tahmini +200-300 MD -> 500 esigi tutturulur
YOL B (kullanici karari) : GATE-004a esigi yeniden gozden gecirilir
   (vault daraldi; esik ADR ile mi guncellenir?) -> ADR acilir
IKISI DE : kullanici onayi (G7/G8) olmadan yapilmaz
```

---

## §4 Doğrulama

- [ ] Faz dosyası **6 + 1 indeks = 7**
- [ ] Her faz dosyasında `## §Kanıt` var (kanıtsız "bitti" yok)
- [ ] Sıra atlanmamış (1 → 2 → 3 → 4 → 5 → 6)
- [ ] `checklists/` diskte **24 MD** ve linkleri çözüyor (checklists kaynaklı kırık = 0)
- [ ] Her faz dosyası **2026-10-07** tarihli güncel durum taşıyor
- [ ] 8 alan frontmatter · `updated: 2026-10-07` · mojibake 0
- [ ] Silinmiş dizinlere giden linkler **korunmuş** (silinmedi)

---

## §5 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Gate sırası | [WORKFLOW](../../governance/workflow.md) §2 | sıra kuralı |
| 2 | **Checklist envanteri (24)** | [checklists index](../../checklists/index.md) | hangi liste hangi kapıda |
| 3 | Vault kapısı | [gate-004a](../../checklists/gate-004a-vault-kapanis.md) | faz 01 kabul kriteri |
| 4 | Bilinen eksikler | [bilinen-eksikler](../log.md) | açık maddeler |
| 5 | Mimari baz | [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md) | faz 03 kapsamı |
| 6 | Ajan politikası | [AGENTS](../../definitions/agents.md) | 3 paralel sınırı |
| 7 | Bilgi kümesi (134) | [knowledge index](../../knowledge/index.md) | teknik notlar |
