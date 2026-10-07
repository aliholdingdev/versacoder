---
title: WORKFLOW - Kapı zinciri özeti (boot)
type: boot-instruction
category: root
status: active
version: 2.1.0
updated: 2026-10-07
authority: SSOT
scope: "Repo kökü; kapı adları ve sırası - kapı kriterleri .ai/WORKFLOW.md'de, durum kanıtı .ai/log.md'dedir"
---

# WORKFLOW — Kapı zinciri (özet)

**Zorunlu Bağlantılar:** [index](.ai/index.md) · [templates](.ai/.templates/index.md) · [WORKFLOW](.ai/WORKFLOW.md) · [checklists](.ai/checklists/index.md) · [log](.ai/log.md) · [AGENTS](.ai/AGENTS.md)

> **Tek cümlelik sorumluluk:** kapı adlarını ve sırasını tek satırda verir; kapı
> kriterleri, kanıt kutusu biçimi ve durum SSOT dosyalardadır — burada tekrarlanmaz.

## §1 Zincir

```text
GATE-004a (vault) -> GATE-003 (UI onayi, HARD STOP) -> GATE-004 (iskelet + ilk commit) -> GATE-005 (test) -> GATE-006 (teslim)
```

| Kapı | Ne kapatır | Checklist | Kanıt nerede aranır |
|---|---|---|---|
| GATE-004a | vault yeşil: ölçüm sayaçları sıfırlarda (link/mojibake/frontmatter) | `gate-004a-vault-kapanis.md` | `.ai/scripts/*.ps1` çıktısı |
| GATE-003 | ekran onayı — onaysız `src/**` kodu yasak (HARD STOP) | `gate-003-ui-onay.md` | kullanıcı onayı (tarihli, `.ai/log.md`) |
| GATE-004 | iskelet + 2 bacak build + dosya bazlı ilk commit | `gate-004-ilk-commit.md` | `dotnet build` + `dotnet test` çıktısı |
| GATE-005 | test + NetArchTest 0 ihlal | `gate-005-test-kapsami.md` | `dotnet test` çıktısı |
| GATE-006 | teslim | — (bu bandda tanımlı liste yok) | `WORKFLOW.md` §5 raporu |

## §2 Kural (tek satır)

- **Kapıyı yalnız kullanıcı açar**; RED veren kapı işi üreten ajana döndürür,
  başka ajana devredilmez — [`AGENTS`](.ai/AGENTS.md) §1.3.
- **Sıra atlanmaz:** `GATE-003` onayından **önce** `src/**` altına tek satır `.cs` yazılmaz.
- **Kapı RED'i strike sayılmaz** (gate kuralı, ajan yöntemi değil) — [`AGENTS`](.ai/AGENTS.md) §4.3.
- **Tam metin** (kapı kriterleri, kanıt kutusu `KANIT/DOSYA/EKSIK/SONRAKI`, süreler):
  [`WORKFLOW`](.ai/WORKFLOW.md) §2.
- **Günlük durum** (hangi kapı açık/kapalı, son onay ne zaman): [`log`](.ai/log.md)
  son kaydı — burada tahmin yazılmaz.
- **Kapalı soru:** "kapı kriteri tam nedir?" → `.ai/WORKFLOW.md`;
  "şimdi durum ne?" → `.ai/log.md`.

## §3 Şu anki durum (ölçüm 2026-10-05)

| Kapı | Durum | Kanıt |
|---|---|---|
| GATE-004a | **ÖLÇÜLEBİLİR** | 255 MD (`.ai/`, 2026-10-07) · frontmatter 255/255 · BOZUK link 192 (bilinçli — eksik dizinler) · MOJIBAKE 0 |
| GATE-003 | **AÇIK (HARD STOP)** | kullanıcı onayı yok — `src/**`a yazılmadı |
| GATE-004 | **BLOKE** | `src/`, `tests/`, `.sln`, `global.json` **YOK** → `dotnet build` `MSB1003` |
| GATE-005 | **BLOKE** | `tests/` YOK → `dotnet test` hedef bulamaz |
| GATE-006 | **BLOKE** | önceki kapılar kapalı |

## §4 Doğrulama

```powershell
# GATE-004a kaniti (bu makinede calisir)
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\find-mojibake.ps1
# durum kaniti (tahmin degil)
Get-Content .\.ai\log.md -Tail 5
```

---

## §5 Baglanti Agu (tam capraz-link)

Bu dosyanin diger 18 vault dosyasina baglantilari (eksiksiz; hicbir baglanti silinmemistir):

| # | Dosya | Ne tasiyor |
|---|---|---|
| CLAUDE | [CLAUDE](.ai/CLAUDE.md) | AI anayasa — G1..G12 hard guardrail, kanit formati, dur/bekle |
| ROLE | [ROLE](.ai/ROLE.md) | rol — 18->5, karar hiyerarshisi (7 basamak), devir zinciri |
| engine | [engine](.ai/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](.ai/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| VISION | [VISION](.ai/VISION.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](.ai/PROJECTS.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](.ai/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| keys | [keys](.ai/keys.md) | anahtar kelime -> hedef dosya hiz haritasi (101 satir) |
| MEMORY | [MEMORY](.ai/MEMORY.md) | kalici dersler, karar desenleri, kara liste, kayit defteri |
| ULTRA | [ULTRA](.ai/ULTRA-THINKING.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.ai/.decisions/draft/ADR-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok CLAUDE | [kok CLAUDE](./CLAUDE.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok README | [kok README](./README.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok AGENTS | [kok AGENTS](./AGENTS.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
