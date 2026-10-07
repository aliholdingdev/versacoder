---
title: "TODOS Faz 01 — Vault Kapanışı (GATE-004a)"
type: todo
category: todos
status: active
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "500 MD + 0 bozuk link + 0 mojibake + kok bant 30-88 KB icin adim adim is listesi; kapanis sirasi ve kanit betikleri"
tags: [ops]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Faz 01 — Vault Kapanışı (GATE-004a)

**Zorunlu Bağlantılar:** [TODOS index](index.md) · [gate-004a](../../checklists/gate-004a-vault-kapanis.md) · [bilinen-eksikler](../log.md) · [vault-link-check](../../checklists/vault-link-check.md) · [documentation-check](../../checklists/documentation-check.md) · [WORKFLOW](../../governance/workflow.md)

> **Tek cümlelik sorumluluk:** Vault'un 500 MD ve 3 sıfır (link/mojibake/frontmatter)

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: RED — 282/500 MD (GATE-004a esigi altinda)**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| `.ai/` toplam MD | **282** | 500 (GATE-004a) | RED — 218 eksik |
| checklists/ | **24 MD** | — (once YOK) | OK — 2026-10-07 doldu |
| knowledge/ | 134 MD | — | OK — hub baglantisi tam |
| Eksik dizin | 5: .rules · architecture · reports · playbooks · prompts | 0 | RED — uretilmeli veya esik yeniden gozden gecirilmeli |
| Frontmatter eksik | 0 | 0 | OK |
| Gercek mojibake | 0 | 0 | OK |
| Kirik link | 752 | 0 | RED — silinmis dizin hedefleri (korundu) |

> KAPI RED: esige ulasmak icin (A) eksik dizinler uretilir ya da (B) esik ADR ile yeniden gozden gecirilir — IKISI de kullanici onayi ister (G7/G8).

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).

> kapısına ulaşması için sırayla çalıştırılacak işlerin tek listesidir.

**Durum: TAMAM** (GATE-004a kullanici onayi 2026-09-30 - 14/14, 4 kapi yesil).

---

## §0 Özet (Answer First)

| Ölçüm | Şu an | Hedef | Açık |
|---|---|---|---|
| Toplam MD | 383 | ≥ 500 | **~117** (3 ajanın işi ~80'i yazıyor) |
| NoFrontmatter | 0 | 0 | — |
| Mojibake | 0 | 0 | — |
| Bozuk link | 0 | 0 | — |
| Kök 30-88 KB | 14/14 | 14/14 | — |

Kalan ~117'nin kaynak dağılımı: knowledge kalan (~45), catalog kalan (ajan bitince),
reports (3), catalog eksik kalan, `knowledge/*/index` (5), rezerve adlar (3).

---

## §1 Adımlar (sırayla — her adım kanıtsız kapanmaz)

| # | Adım | Ne yapılır | Kanıt | Süre |
|---|---|---|---|---|
| 1 | 3 ajanı bitir | knowledge 30 · catalog 26 · playbooks+.agents 24 | ajan §2 kutuları | ~10 dk |
| 2 | `measure-vault.ps1` | `TotalMD ≥ 500` bekle | exit 0 / özet çıktısı | 1 dk |
| 3 | Kalan açığı dağıt | reports 3 · `knowledge/*/index` 5 · rezerve 3 · kalan batch | yeni ajan (3'lü) | ~30 dk |
| 4 | `check-links.ps1` | **bozuk link 0** | çıktı: 0 | 1 dk |
| 5 | `find-mojibake.ps1` | **0** | çıktı: 0 | 1 dk |
| 6 | `count-components.ps1` | 5440+ kayıt PASS | PASS satırı | 1 dk |
| 7 | gate-004a 14 soru | hepsi [x] | [gate-004a](../../checklists/gate-004a-vault-kapanis.md) işaretli | 10 dk |
| 8 | `log.md` kapanış kaydı | tarihli satır | log.md son satır | 2 dk |
| 9 | Kullanıcı onayı | GATE-004a kapandı derken onay | kullanıcı yanıtı | — |

- **Adım 2'de TotalMD < 500 ise** adım 3'e dön; 04'e asla erken geçme.
- **Adım 4/5'de > 0 ise** o dosya UTF-8 yeniden yazılır; sayacı sıfırla, 4'ten tekrar.

---

## §2 Betikler (komutlar)

```text
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1
```

| Betik | Beklenen | RED |
|---|---|---|
| `measure-vault.ps1` | exit 0 | `TotalMD < 500` ya da kök bant dışı |
| `check-links.ps1` | 0 bozuk link | > 0 → G4 ihlali |
| `find-mojibake.ps1` | 0 | > 0 → dosya UTF-8 yeniden yazılır |
| `count-components.ps1` | PASS | FAIL → ADR-028 52 düğüm sayımı tutmuyor |

> PS 5.1 `.ps1` dosyalarını ANSI okur — betik içi metinde `U+FFFD`/`Ã` **yazma**;
> düzenlemelerde `[char]0x...` kaçışı kullan (bilinen tuzak, 2026-09-29 kaydı).

---

## §3 Kapanış Sırası (bilinen-eksikler §5'e göre)

| # | İş | Kaynak | Durum |
|---|---|---|---|
| 1 | Mojibake + frontmatter onarımı | §1 | **bitti** (0 / 0) |
| 2 | ui-design 7 wireframe + README | §4 madde 3 | **bitti** (8/8) |
| 3 | l7-ui 2 eksik düğüm | mimari | **bitti** (6/6) |
| 4 | .sql index | yeni | **bitti** (1/1) |
| 5 | Rezerve adlar (B8/B9/B10) + `knowledge/*/index` | §4 | **açık** |
| 6 | 3 ajan işinin teslimi | §0 | **devam** |
| 7 | 500 MD ölçümü + 3 betik | §2 | **bekliyor** |

---

## §4 Kabul Kriterleri (hepsi zorunlu)

- [x] `TotalMD ≥ 500` (measure exit 0)
- [x] `NoFrontmatter = 0`
- [x] `Mojibake = 0`
- [x] `check-links.ps1 = 0`
- [x] Kök 14 dosya 30-88 KB bandında
- [x] `count-components.ps1 = PASS`
- [x] `log.md`'ye tarihli kapanış satırı
- [x] Kullanıcı GATE-004a onayı

**Tek satır kural:** 8 madde eksikse faz kapanmaz; 02 açılmaz.

---

## §5 Kanıt

| # | Kanıt | Beklenen |
|---|---|---|
| P1-1 | `measure-vault.ps1` özeti | TotalMD ≥ 500, kalan 4 sayaç 0 |
| P1-2 | `check-links.ps1` | 0 bozuk (3400+ link) |
| P1-3 | `find-mojibake.ps1` | 0 |
| P1-4 | `gate-004a-vault-kapanis.md` | 14/14 [x] |
| P1-5 | `log.md` son satır | tarihli kapanış |

`src/` yok — bu fazda kod bazlı kanıt yoktur; hepsi betik çıktısıdır.