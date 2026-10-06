---
title: "Çakışma Kaydı — Master Promptlar, CoreMusic Şablonu ve Vault"
type: report
category: reports
status: active
version: 1.1.0
updated: 2026-10-06
authority: SSOT
scope: "Iki master prompt ile CoreMusic sablonu ve mevcut vault arasindaki karsilamaz iddialar; otorite, cozum ve dogrulama kaniti"
---

# Çakışma Kaydı — Master Promptlar · CoreMusic Şablonu · Vault

**Zorunlu Bağlantılar:** [templates index](../.templates/index.md) · [CLAUDE](../../CLAUDE.md) · [WORKFLOW](../../WORKFLOW.md) · [AGENTS](../../AGENTS.md)

## §1 Amaç

**Prompt A** (Enterprise Engineering v2.0.0) · **Prompt B** (Project Installation v1.0.0) · **CoreMusic şablonu** ile vault'u karşılaştırıp çözülemeyecek çelişkileri kanıtla kaydetmek. Düzeltme değil, **hangi kaynağın kazandığını ilan etmek**.

## §2 Kapsam

- **Kapsamda:** CF-01…CF-16 (durum sütunu 2026-10-06).
- **Kapsam dışı:** kök 4 dosyadaki "553 MD / bozuk link 0 / mojibake 0" iddiaları (§7).
- **Kapsam dışı:** `.ai/` kök omurgası ertelendi — **linkler silinmeyecek**, hedefler üretilmeyecek.

## §3 Çakışma Kaydı

### §3.1 Tablo

Durum: ✅ düzeltildi · ⏳ karar bekliyor · 📌 kapsam dışı, yalnız kayıt.

| ID | Çakışma | A tarafı | B / disk tarafı | Kazanan | Durum |
|---|---|---|---|---|---|
| CF-01 | Katman modeli | CLAUDE §B5 `l0..l7` (8) | README §6/AGENTS `K0..K8` (9) · A §12/§40 (3/14) · B §11 (6) | **CLAUDE §B5** | 📌 kök |
| CF-02 | Gate sayısı | WORKFLOW s.22 = 5 kapı | B §59 = `GATE-000…014` | **WORKFLOW** | 📌 kök |
| CF-03 | Frontmatter 8. alan | index §3.7 = `part` · K1 = "7 alan" | disk **38/38 = `scope`** | **disk** | ✅ §3.7/§3.8/K1 |
| CF-04 | G4 `{{…}}`=0 | index G4 kapsam "tümü" | K5 şablon kalıbında ~370 sahte alarm | **şablonun doğası** | ✅ G4 + K5 |
| CF-05 | G6 muafı | tek muaf `.templates/CLAUDE.md` (139 s) | 10 dosya <480 s | — | **⏳ D4** |
| CF-06 | K15 26–38 KB | index K15/§3.7 | `index.md` = 41,5 KB | — | ✅ 26–44 |
| CF-07 | Part KB | §3.6 + K9 = 582 KB | ara ölçüm 971 KB | **ölçüm** | ✅ → CF-16 |
| CF-08 | `.ai/.rules/` | CLAUDE/README = **18** | K8 = **7** · skill = **16** · disk = **YOK** | **disk** | ✅ K8 `0 — YOK` |
| CF-09 | `.ai/` disk beyanı | index s.80 "kök 14 · reports 2 · prompts 2" | disk: `.templates/` 50 · prompts 2 · reports 1 · kök 0 | **disk** | ✅ s.80 |
| CF-10 | GATE-003 kapsamı | kök CLAUDE G8 "ekran/**kod**" | index G8 "ekran/**dosya**" · §3.4 "şablon yazılmaz" | **kök CLAUDE** | ✅ §3.4 + G8 |
| CF-11 | A vs B iç çelişki | A: 22 phase · 4 etiket · "soru sorma" · v2.0.0 | B: 15 phase · 6 etiket · "STOP sor" · v1.0.0 | — | **⏳ D2** |
| CF-12 | `.ai/` taxonomisi | CoreMusic 14 · A §6 **19 (metin kayıp)** | B §66 15 · vault §3.10 12 | — | ✅ [adim4 raporu](./adim4-vault-skeleton-001.md): **17 belgelenmiş + 2 kayıp** |
| CF-13 | ADR yolu + no | CLAUDE/README `.ai/.decisions/` `ADR-001…030` | index G9 `.ai/adr/` `ADR-001…008` | **kök CLAUDE** | ✅ 11 nokta |
| CF-14 | Etiket seti | A §1 = 4 etiket | B §3 = 6 (`+[INFERENCE]`,`[WEB VERIFIED]`) | **Prompt B** | ✅ 6 etiket |
| CF-15 | Vault bağlantı yolları | kök CLAUDE s.14: `.ai/index.md`·`CLAUDE.md`·`WORKFLOW.md`·`keys.md`·`architecture/index.md` | disk: **5'i YOK**; `WORKFLOW.md`·`README.md` kökte | **disk** | 📌 kök |
| CF-16 | Part toplam KB | §3.6 + K9 = `≈582,0` (175/154/124/129) | ölçüm 2026-10-06 · **44/44 dosya = 1.395,2** | **ölçüm** | ✅ §3.6 + K9 |

### §3.2 Kanıt

- **CF-01** CLAUDE s.46-53 sekiz satır `l0..l7`, yön s.56. README s.51 "kaynak §B5" derken s.21/51/78/92 `K0..K8` yazar — B5'te `K` yok; AGENTS s.14 de öyle. → **Beş ayrı model; vault `l0..l7` kazanır**, A/B haritaları eşleştirilmeden kullanılamaz.
- **CF-02** WORKFLOW s.22 tek zincir `004a→003→004→005→006`, adları s.27-31. B §59'daki 15 kapıdan hiçbiri vault'ta yok; A §54 kapı adı vermez.
- **CF-03** §3.7 sekizinci alanı `part` (`A|B|C|D`) zorunlu kılar; K1 "7 alan" deyip `title…authority` sayar; `new-doc-contract.md` s.23 "8 alan" der. **Ölçüm: `part`=0, `scope`=38.** → `part` bilgisi envanterin sütununda, frontmatter'da değil.
- **CF-04** K5 şablonların kendisinde `\{\{[A-Z0-9_]+\}\}` arar → YAZILDI kalıplarda sahte alarm; denetim değeri yok. G4 asıl olarak *şablondan üretilen gerçek dosyada* anlamlıdır.
- **CF-05** 480 altı 9 dosya: `unicode-box…` 24 · `llm-provider` 81 · `batch-manifest` 90 · `winform-form-designer` 102 · `new-doc-contract` 103 · `readme` 117 · `ascii-box…` 122 · `root-md` 144 · `session-log` 479; `CLAUDE.md` 139 muaf. Kapsam **D4**.
- **CF-06/CF-07** K15 (s.522) + §3.7 (s.288) 26–38 der; `index.md` 41,5 KB → envanter kendini reddeder. §3.6/K9 582 KB'ı hiç ölçülmüştü.
- **CF-08/CF-09** `.ai/` altında klasörler `.templates/` · `prompts/` · `reports/`; kök MD **0**. `.rules/`, `.agents/` yok. index s.80 "14 MD / 2 MD / 2 MD" beyanı ölçülemedi → yanlış.
- **CF-10** kök G8 "ekran/**kod**" (SSOT) · index G8 "ekran/**dosya**" · §3.4 "şablon yazılmaz" · WORKFLOW s.28 "`src/**` yasak". → **GATE-003 ekran dosyasını ve `src/**` kodunu bloklar, şablonu bloklamaz** — döngü aksi halde kapanmaz (şablon→ekran→onay).
- **CF-11** Etiketlerde B kazanır (6). Sorma kuralında taviz: **kritikse dur (B), rutinde sorma (A)**. Phase için kullanıcı kararı.
- **CF-13** CLAUDE s.65 `.ai/.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md`; README "ADR-001…030, 32 MD". index G9 `ADR-001…008`. → İki yol, iki aralık, ikisi de diskte yok.
- **CF-14** CoreMusic'teki `senior-mode.md` etiket seti VersaCoder'da yok.
- **CF-15** Kök CLAUDE s.14 altı bağdan yalnız `.ai/.templates/index.md` diskte; kalanı AŞAMA C hedefi. Kök dosya kapsam dışı → **kayda alındı, düzeltilmedi**; index s.80/§3.10/§7.1 disk gerçeğiyle yazıldı.
- **CF-16** 44/44 dosya; örnek satır `#18` hariç. Nihai **A 336,9 · B 393,9 · C 334,8 · D 329,6 = 1.395,2 KB**.

## §4 Otorite Hiyerarşisi

1. Kullanıcı açık talimatı ("hepsini sırayla uygula" · `.ai/` kök ertelendi · linkler silinmeyecek)
2. Kök [CLAUDE](../../CLAUDE.md) — anayasa (B3/B4/B5)
3. [WORKFLOW](../../WORKFLOW.md) — kapılar
4. [index](../.templates/index.md) §3.7 — şablon kalite sözleşmesi
5. Prompt A / Prompt B — planlama çerçevesi; vault'a uymayan yerde **vault kazanır**
6. CoreMusic şablonu — **yalnız yapı referansı, içerik değil** (G1)

## §5 Doğrulama

```powershell
# CF-05 (<480) · CF-06 (K15 ≤ 44) · CF-08 (.rules YOK) · CF-09/CF-16 (disk + Part KB)
Get-ChildItem 'C:\versacoder\.ai\.templates' -Recurse -Filter *.md |
  %{ "{0,4} s {1,6:N1} KB  {2}" -f (Get-Content $_.FullName).Count,($_.Length/1KB),$_.Name }
'{0:N1} KB' -f ((Get-Item 'C:\versacoder\.ai\.templates\index.md').Length/1KB)
Test-Path 'C:\versacoder\.ai\.rules'   # CF-08 → False
```

## §6 Kararlar (2026-10-06 kullanıcı onaylı — hepsi kapandı)

| # | Karar | Seçenek | Durum |
|---|---|---|---|
| **D1** | CF-06 + CF-05 | K15 26–38 → **26–44** | ✅ uygulandı (index K15 + §3.7) |
| **D2** | CF-11 phase | **Prompt A = 22 phase** (B'nin 15'i reddedildi); kapı = WORKFLOW **5 gate**; etiket = Prompt B'nin **6**'sı; sorma = kritikse dur | ✅ arşivlere yazıldı |
| **D3** | CF-12 taxonomi | Prompt A §6 (19) **metin kayıp** → fallback CoreMusic §4 + index §3.10 → **17 belgelenmiş + 2 bilinmiyor** | ✅ [adim4 raporu](./adim4-vault-skeleton-001.md) |
| **D4** | CF-05 G6 | kapsam = **`.templates/**`** (`prompts/`+`reports/` hariç — orada 3–8 KB) · **9 dosya ≥480 satır**a çıkarılır | 🔄 üretimde |
| **D5** | K14/K15 | **K14 → 480–780** · **K15 → 26–46** | ✅ index K14/K15 + §6.1 |

## §7 Kapsam Dışı (tespit, düzeltme yok)

- Kök 4 dosyadaki "553 MD · bozuk link 0 · mojibake 0" iddiaları.
- `.ai/scripts/*.ps1` yok (`check-links`, `measure-vault`, `find-mojibake`) — kök ertelemesi içinde, 6 referans.
- CoreMusic şablonunun **içeriği** (G1) — yalnız yapı referansı.
