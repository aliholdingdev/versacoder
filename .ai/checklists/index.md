---
title: "checklists — Kapi ve Denetim Listeleri Indeksi"
type: index
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "23 denetim listesinin tam envanteri, hangisinin hangi kapi/ADR ile ilgisi, referans sayilari ve eksiklerin durumu"
---

# checklists — Kapi ve Denetim Listeleri Indeksi

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Bir isin hangi denetim listesinden gectigini, o listeyi
> ne zaman ve kimin calistiracagini ve RED uretirse ne olacagini tek tabloda soyler.

## §0 Ozet (Answer First)

- **23 liste**: 5 sert kapi listesi (WORKFLOW §2 kaynakli) + 18 ADR esmeli denetim (brain §1.1 kaynakli).
- **Kullanim:** ilgili is **baslamadan once** tek tek isaretlenir; **1 madde bile bossa RED**.
- **Kapatan yalniz kullanici** (G12); bu listeler yalniz kanit hazirlar.
- **Turetme (G9):** icerik vault belgelerinden turetilmistir; kaynakta olmayan madde uydurulmaz.
- **Olcum:** 2026-10-07 · 23 MD · frontmatter 23/23 · boyut 3.9-5.6 KB (3-8 bandi)

### §0.1 Sira kurali (hangi liste once)

```text
GATE-004a (vault olcumu)  -->  GATE-003 (wireframe + onay)  -->
GATE-004 (iskelet + build + ilk commit)  -->  GATE-005 (test + kapsam)  -->  GATE-006 (teslim)
```

> Kapilar [WORKFLOW](../governance/workflow.md) §1 ile ayni siradadir; **atlanan kapi geri gelir**.
> Her kapida once bu dizindeki ilgili liste calistirilir, sonra kanit kutusu doldurulur.

## §1 Sert Kapi Listeleri (5 — WORKFLOW §2 kaynakli)

| # | Liste | Kapi | Referans | Ne denetler |
|---|---|---|---|---|
| 1 | [gate-003-ui-onay.md](gate-003-ui-onay.md) | GATE-003 | 17 | 7 wireframe + DevExpress v26.1 + RibbonForm + kullanici onayi (HARD STOP) |
| 2 | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) | GATE-004 | 5 | cift bacak build 0 hata + NoWarn 0 + ilk commit kurallari |
| 3 | [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) | GATE-004a | 9 | vault olcumu: 500 MD · 0 bozuk link · 0 mojibake · frontmatter 8 alan |
| 4 | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) | GATE-005 | 6 | dotnet test 0 basarisiz + NetArchTest 0 ihlal + coverage gerekcesi |
| 5 | [layer-boundary-check.md](layer-boundary-check.md) | GATE-005 (madde 3) | 76 | katman siniri 13 soru — ADR-028 8x8 matris |

## §2 ADR Esmeli Denetimler (18 — brain.md §1.1 kaynakli)

| # | Liste | Bagli ADR | Referans | Ne denetler |
|---|---|---|---|---|
| 1 | [accessibility-check.md](accessibility-check.md) | — | 0 | klavye, odak, AccessibleName, kontrast |
| 2 | [coverage-gate-check.md](coverage-gate-check.md) | 019 (gate-005) | 3 | coverage bani + atlanan test 0 |
| 3 | [data-access-check.md](data-access-check.md) | 005 · 016 | 1 | EF 0 · DbConnection yalniz l4 |
| 4 | [dependency-update-check.md](dependency-update-check.md) | 012 · 014 | 0 | CPM tek yer, .csproj Version=0 |
| 5 | [devexpress-control-check.md](devexpress-control-check.md) | 004 | 0 | yalniz DevExpress v26.1, l7 disi sismiyor |
| 6 | [documentation-check.md](documentation-check.md) | 002 · 024 · 029 | 2 | sifirdan yazim, kopya yok, ASCII kutu 0 |
| 7 | [dual-tfm-build-check.md](dual-tfm-build-check.md) | 001 · 019 | 2 | cift TFM build/test; #if yalniz platform |
| 8 | [layer-5000-check.md](layer-5000-check.md) | 030 | 4 | 5000+ tip katmani, 4 kademe, 3 eksen |
| 9 | [logging-observability-check.md](logging-observability-check.md) | 026 | 0 | structured log, 5 seviye disi 0 |
| 10 | [migration-check.md](migration-check.md) | 005 | 0 | saf SQL + schema_version, ORM migration yok |
| 11 | [performance-budget-check.md](performance-budget-check.md) | — | 1 | olculmus butce; kanitsiz performans iddiasi 0 |
| 12 | [ribbon-style-check.md](ribbon-style-check.md) | 004 | 0 | yalniz RibbonControlStyle.Office2010 |
| 13 | [secret-leak-check.md](secret-leak-check.md) | 011 · 018 | 3 | kod/repoda secret 0, DPAPI |
| 14 | [security-review-checklist.md](security-review-checklist.md) | 018 | 3 | OWASP kalemleri + yetkilendirme |
| 15 | [session-handoff-check.md](session-handoff-check.md) | 007 | 0 | devir paketi 4 madde + derinlik <= 2 |
| 16 | [sql-injection-review.md](sql-injection-review.md) | 017 | 2 | concat SQL 0, parametreli sorgu |
| 17 | [vault-link-check.md](vault-link-check.md) | 003 · 021 | 1 | bozuk link 0, her dizinde tek indeks |
| 18 | [winforms-designer-check.md](winforms-designer-check.md) | 015 | 2 | .Form.cs + .Designer.cs + .resx uclusu |

## §3 Referans Sayaci (hangisi en cok kullaniliyor)

| # | Liste | Link | Anma | Durum |
|---|---|---|---|---|
| 1 | `layer-boundary-check.md` | 76 | 167 | **aktif** |
| 2 | `gate-003-ui-onay.md` | 17 | 74 | **aktif** |
| 3 | `gate-004a-vault-kapanis.md` | 9 | 62 | **aktif** |
| 4 | `gate-005-test-kapsami.md` | 6 | 62 | **aktif** |
| 5 | `gate-004-ilk-commit.md` | 5 | 59 | **aktif** |
| 6 | `winforms-designer-check.md` | 2 | 18 | **aktif** |
| 7 | `secret-leak-check.md` | 3 | 14 | **aktif** |
| 8 | `security-review-checklist.md` | 3 | 8 | **aktif** |
| 9 | `dual-tfm-build-check.md` | 2 | 9 | **aktif** |
| 10 | `documentation-check.md` | 2 | 9 | **aktif** |
| 11 | `data-access-check.md` | 1 | 10 | **aktif** |
| 12 | `layer-5000-check.md` | 4 | 6 | **aktif** |
| 13 | `coverage-gate-check.md` | 3 | 7 | **aktif** |
| 14 | `vault-link-check.md` | 1 | 5 | **aktif** |
| 15 | `performance-budget-check.md` | 1 | 3 | **aktif** |
| 16 | `logging-observability-check.md` | 0 | 4 | aniliyor, link az |
| 17 | `session-handoff-check.md` | 0 | 3 | aniliyor, link az |
| 18 | `ribbon-style-check.md` | 0 | 3 | aniliyor, link az |
| 19 | `devexpress-control-check.md` | 0 | 3 | aniliyor, link az |
| 20 | `dependency-update-check.md` | 0 | 3 | aniliyor, link az |
| 21 | `sql-injection-review.md` | 2 | 0 | **aktif** |
| 22 | `migration-check.md` | 0 | 2 | aniliyor, link az |
| 23 | `accessibility-check.md` | 0 | 1 | aniliyor, link az |

**Toplam:** 143 link referansi (vault geneli, `checklists/` onekli)

## §4 Durum ve Eksikler (durustluk notu)

| # | Konu | Durum | Ne yapilacak |
|---|---|---|
| 1 | Orijinal `layer-boundary-check` 13 soru metni | vaultta **YOK** | 13 madde turetildi; asil metin gelirse birebir guncellenir -> `DOĞRULAMA GEREKLI` |
| 2 | Orijinal `gate-*` "13/14 adim" listeleri | WORKFLOWda **yalniz sayi var** | Adimlar kabul kriterlerinden turetildi -> `DOĞRULAMA GEREKLI` |
| 3 | `layer-5000-check` orijinal 14 maddesi | dosya kayip (log 2026-10-02 kaydi var) | 7 madde turetildi -> `DOĞRULAMA GEREKLI` |
| 4 | `accessibility-rules.md` kural metni | `.rules/` dizini diskte **YOK** | Listeler genel WCAG + VISIONdan turetil |
| 5 | `tool-contract-spec` | `spec/` dosyasi (checklist degil) | Bu dizine alinmadi |
| 6 | Kirik link hedefleri (.rules/ · accepted/ · reports/) | dizinler **YOK** | Linkler **korundu** (silinmedi) — gelecek surumde baglanacak |

> **G9 kurali:** bu dizindeki her madde bir vault kaynakna baglanmistir; kaynagi
> olmayan madde `DOĞRULAMA GEREKLI` ile isaretlenir ve tek basina "gecti" sayilmaz.

---

## §5 Iliskili Dosyalar

[vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [PROJECTS](../definitions/projects.md) · [VISION](../definitions/vision.md) · [keys](../governance/keys.md) · [glossary](../definitions/glossary.md) · [log](../ops/log.md) · [TODOS index](../ops/todos/index.md) · [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md)
