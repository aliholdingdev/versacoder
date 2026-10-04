---
title: "ADR-022 — İlk Commit GATE-004'te; Önce Vault ve Wireframe Onayı"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "git'e ilk ne yazılacağı, sırası ve kilidi: vault + wireframe onayı olmadan commit yok"
---

# ADR-022 — İlk Commit GATE-004'te; Önce Vault ve Wireframe Onayı

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-025](ADR-025-gate003-ui-hard-stop.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md)

> **Tek cümlelik karar:** Depoda **ilk commit yalnız GATE-004'te** atılır; ondan önce **vault kalitesi (0 bozuk link) + GATE-003 wireframe onayı** alınmış olmalıdır — karar verildi.

---

## §0 Özet (Answer First)

Erken commit, **henüz onaylanmamış işi** geri alınamaz geçmişe yazar: onaysız wireframe değişince kod çöp olur, vault'ta bozuk link kalırsa o da tarihe yazılır. Bu ADR sırayı kilitler: `GATE-003 (wireframe onayı) → GATE-004a (vault: 0 bozuk link · frontmatter) → GATE-004 (ilk commit)`. İlk commit **tek seferdedir** ve `git status --porcelain` çıktısı ile birlikte rapora yapıştırılır. Yıkıcı git işlemleri (`reset --hard`, `push -f`) ayrıca G7/G11 onayı ister.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Gate sırası bozulmadan iş yapılmaz; GATE-003 açıkken kod dosyası ihlaldir | `CLAUDE.md` G12 |
| C2 | Vault kalitesi gate'i: 0 bozuk link, frontmatter 0 eksik | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) §karar 3 · `scripts/check-links.ps1` |
| C3 | Yıkıcı işlem öncesi onay zorunlu | `CLAUDE.md` G7 · G11 |
| C4 | 7 wireframe onaylanmadan `.cs` yazılmaz | [ADR-025](ADR-025-gate003-ui-hard-stop.md) |
| C5 | `git status --porcelain` ile yazım kapsamı denetlenir | `CLAUDE.md` §denetim G1 |
| C6 | Vault'ta kopyalanan içerik varsa reddedilir (yasaklı vault yazımı) | [ADR-024](ADR-024-coremusic-vault-immutable.md) |

---

## §2 Karar

**Karar:** İlk commit **yalnız GATE-004'te** atılır ve ön koşulları **GATE-003 wireframe onayı + GATE-004a vault kalitesidir**; bu ikisiz commit atılamaz — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Sıra | `GATE-003` → `GATE-004a` → **`GATE-004` (ilk commit)** | C1 · C2 |
| D2 | Vault ön koşulu | `check-links.ps1` → **0** bozuk link · frontmatter eksik **0** | C2 |
| D3 | UI ön koşulu | 7 wireframe onayı (`ui-design/`) yazılı olarak alınmış | C4 |
| D4 | Commit kapsamı | `.ai/` + `src/` + `tests/` + yapı dosyaları; **secret yok** | C5 · [ADR-018](ADR-018-secret-store.md) |
| D5 | Kanıt | `git status --porcelain` + commit hash rapora yazılır | C5 |
| D6 | Yıkıcı git | `reset --hard` · `push -f` · `clean -fd` → **G7/G11 onayı** | C3 |
| D7 | Commit mesajı | `<gate>: <kapsam>` (örn. `GATE-004: ilk commit - vault + src iskeleti`) | C1 |

**Neden:** Commit, **onayın kaydıdır**; onaydan önce atılan commit onayı taklit eder ve geri alma maliyetini büyütür (yeni dosyalar silinmez/ızgara tarihi kirlenir). Sıranın yazılı olması, ekip/hizmetli ajanlar için tartışmayı kapatır: `GATE-004a` kırmızıysa commit **hiç düşünülmez**. D4'teki secret yasağı, [ADR-018](ADR-018-secret-store.md) ile aynı kapıdan çıkar — depoya giren secret silinemez.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Onaysız iş geçmişe yazmıyor | D1 sırası + gate kontrolü |
| İlk commit temiz ve denetlenebilir | `git status --porcelain` çıktısı raporda |
| Vault kalitesi depoya girmeden kapanmış | `check-links.ps1` 0 bozuk link |
| Yıkıcı git işlemi onaysız yapılamıyor | D6 + `CLAUDE.md` G7 |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Commit gecikir → iş kaybı riski (diske yazılan ama commit edilmeyen) | vault + kod **GATE-004a'ya kadar yerel** tutulur; günlük `log.md` kaydı |
| Tek büyük commit = inceleme zorluğu | ilk commit **iskele** (vault + proje iskeleti); sonraki işler ayrı commit |
| Gate'ler kâğıt üzerinde kalabilir | D5: gate raporu + komut çıktısı eklenir |
| Yerel dosya kaybı (disk hatası) | günlük zip/yerel yedek; **commit değil** |

**NEEDS_VALIDATION:** `git`'in ilk kez kullanılacağı makinede `user.name`/`user.email` + repo başlatma adımları henüz yazılmadı → GATE-004 öncesi `checklist` maddesi olarak eklenir ve **ilk commit anında** teyit edilir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-025](ADR-025-gate003-ui-hard-stop.md) | related | commit'in ön koşulu olan wireframe onayı |
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | related | vault kalitesi gate'inin ölçüleri |
| [ADR-018](ADR-018-secret-store.md) | related | commit kapsamından secret'ın çıkarılması |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | related | `git` komutlarının timeout/onay eşiği |
| [ADR-024](ADR-024-coremusic-vault-immutable.md) | related | depoya giren içeriğin kaynağının yasağı |

---

## §5 Kabul Kriterleri

- [ ] `GATE-004a` raporu: `check-links.ps1` 0 bozuk link + frontmatter 0 eksik
- [ ] `GATE-003` onayı: 7 wireframe için yazılı onay kaydı
- [ ] `git status --porcelain` çıktısı raporda (beklenen dosyalar listesiyle)
- [ ] İlk commit hash'i + `log.md` kaydı
- [ ] Depoda secret = 0 (tarama)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G7/G12 + gate sırası.
