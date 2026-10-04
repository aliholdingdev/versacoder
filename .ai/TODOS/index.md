---
title: "TODOS — Adım Adım Görev Fazları (Vault Görev Kaydı)"
type: index
category: todos
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Tum vault task/todo/checklist kayitlarinin tek yerde toplandigi, siraya gore faz dosyalarina bolundugu ana indeks; kullanim kurali ('su dosyayi yap') ve siralama gerekcesi"
---

# TODOS — Görev Fazları İndeksi

**Zorunlu Bağlantılar:** [index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [VISION](../VISION.md) · [gate-004a](../checklists/gate-004a-vault-kapanis.md) · [gate-003](../checklists/gate-003-ui-onay.md) · [bilinen-eksikler](../reports/10-bilinen-eksikler.md) · [adr-index](../.decisions/adr-index.md)

> **Tek cümlelik sorumluluk:** Vault'taki hangi işin hangi faz dosyasında yaşadığı ve
> "şu dosyayı yap" denince ne yapılacağının tek cevabıdır.

---

## §0 Özet (Answer First)

Bu klasörde **6 dosya** var: 1 indeks + 5 faz. Tüm fazlar **en temizden (bloke
olmayandan) en pahalıya** sıralıdır. Görevler vault'taki gerçek kaynaklardan
toplandı: `checklists/gate-*` (4 kapı), `reports/10-bilinen-eksikler.md` (açık
maddeler), `WORKFLOW` §2 gate sırası, ADR-028 §2.3. Uydurma görev yok.

### Kullanım kuralı (bu klasörün sözleşmesi)

| Kullanıcı der ki | Yapılan |
|---|---|
| `01-vault-kapanis dosyasını yap` | **Yalnız** o dosyanın adımları çalıştırılır; diğer dosyalara geçilmez |
| `devam et` | Sıradaki (en üstteki tamamlanmamış) faz dosyasına geçilir |
| `kaç kaldısın` | Bu §1 tablosundan `[ ]`/`[x]` sayılır |
| Faz biter | İlgili satır `[x]` olur, sonraki faz açılır (sıra **atlanmaz**) |

- **Tek dosya kuralı:** tek seferde **yalnız bir** faz dosyası işlenir; çok istek
  = sıra bozulur, kapılar (gate) atlanır.
- **Sıra neden sabit:** [WORKFLOW](../WORKFLOW.md) §2'deki gate sırası (vault →
  onay → iskelet → kod → test) aşılmaz; ters sıra = iş iki kez yapılır.

---

## §1 Faz Tablosu (sıra = öncelik)

| # | Dosya | Ne içerir | Kapı | Durum |
|---|---|---|---|---|
| 1 | [01-vault-kapanis.md](01-vault-kapanis.md) | 500 MD + bozuk link 0 + mojibake 0 + kök bant | GATE-004a | **aktif** |
| 2 | [02-5000-plani-sorulari.md](02-5000-plani-sorulari.md) | 5000+ katmanlı plan için 4 soru (kullanıcı cevabı bekler) | — | bekliyor |
| 3 | [03-faz-d-iskelet.md](03-faz-d-iskelet.md) | `C:\versacoder` kod iskeleti: 9+2 proje, CPM, ilk commit | GATE-004 | kapalı |
| 4 | [04-gate-003-wireframe-onay.md](04-gate-003-wireframe-onay.md) | 7 wireframe kullanıcı onayı (HARD STOP) | GATE-003 | kapalı |
| 5 | [05-kod-ve-kalite.md](05-kod-ve-kalite.md) | Uygulama kodu, NetArchTest 8×8, test kapsamı | GATE-005 | kapalı |
| 6 | [06-5000-katman-kurulumu.md](06-5000-katman-kurulumu.md) | ADR-030: 5000+ tip katmani dogrula -> katalog -> kod -> gate | GATE-004a -> 005 | **aktif** |

**Aktif faz = 01.** Faz 2, 1 bitmeden **açılmaz** (bilinçli kural: kullanıcı
cevabı bekleyen iş, vault işini bloklamaz — 01 tamamlanıp 02'ye geçilir).

---

## §2 Kaynak Eşlemesi (her görev nereden geldi)

| Kaynak dosya | Nereye aktarıldı | Ne aktarıldı |
|---|---|---|
| [gate-004a-vault-kapanis.md](../checklists/gate-004a-vault-kapanis.md) | faz 01 | 14 soruluk kapanış listesi |
| [bilinen-eksikler](../reports/10-bilinen-eksikler.md) §5 | faz 01 | kapanış sırası + süre tahminleri |
| `measure-vault.ps1` / `check-links.ps1` / `find-mojibake.ps1` | faz 01 | sert kapı çıktıları |
| Kullanıcı talimatı (bu oturum) | faz 02 | 4 soru taslağı |
| ADR-028 §2.3 | faz 03 | 6 zorunlu düzeltme + 9+2 proje |
| [gate-004-ilk-commit.md](../checklists/gate-004-ilk-commit.md) | faz 03 | commit kapısı |
| [gate-003-ui-onay.md](../checklists/gate-003-ui-onay.md) | faz 04 | 7 ekran onay paketi |
| [gate-005-test-kapsami.md](../checklists/gate-005-test-kapsami.md) | faz 05 | test kapsamı kapısı |
| [layer-boundary-check](../checklists/layer-boundary-check.md) | faz 05 | 13 soru (NetArchTest) |

- Kaynakta olmayan görev **bu klasöre yazılmaz** (uydurma yasak, G5).
- Her faz dosyasında §Kanıt, görevin hangi betik/çıktı ile doğrulanacağını yazar.

---

## §3 Durum Sayacı (2026-09-29 ölçümü)

| Ölçüm | Değer | Hedef |
|---|---|---|
| Toplam MD | **383** | ≥ 500 |
| Frontmatter'siz | 0 | 0 |
| Mojibake | **0** | 0 |
| Bozuk link | 0 | 0 |
| Kök 30-88 KB | 14/14 | 14/14 |
| Yazan ajanlar | 3 paralel (knowledge, catalog, playbooks+.agents) | ≤ 3 |

Ajanlar bitince ≈ 450-460; kalan ≈ 40-50 dosya faz 01'in alt maddeleridir.

---

## §4 Doğrulama

- [ ] Faz dosyası sayısı 5 + 1 indeks
- [ ] Her faz dosyasında `## §Kanıt` var (kanıtsız "bitti" yok)
- [ ] Sıra atlanmamış (1 → 2 → 3 → 4 → 5)
- [ ] Bozuk link 0 (linkler yalnız diskte var olan dosyalara)
- [ ] `updated: 2026-09-29` + 8 alan frontmatter

---

## §5 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Gate sırası | [WORKFLOW](../WORKFLOW.md) §2 | sıra kuralı |
| 2 | Vault kapısı | [gate-004a](../checklists/gate-004a-vault-kapanis.md) | faz 01 kabul kriteri |
| 3 | Bilinen eksikler | [bilinen-eksikler](../reports/10-bilinen-eksikler.md) | açık maddeler |
| 4 | Mimari baz | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | faz 03 kapsamı |
| 5 | Ajan politikası | [AGENTS](../AGENTS.md) | 3 paralel sınırı |
