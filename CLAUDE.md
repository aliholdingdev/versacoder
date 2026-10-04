---
title: CLAUDE - repo koku kisa kural yonlendirmesi
type: boot-instruction
category: root
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Repo koku; kural metni degil yonlendirme - G1..G12 metni .ai/CLAUDE.md'dedir"
---

# CLAUDE - Kural yonlendirmesi (kisa)

> **Tek cumlelik sorumluluk:** kural metninin tek kaynagini ve asilmayacak
> 5 kirmizi cizgiyi hatirlatir; kurali burada **tekrarlamaz** (SRP).

## §0 Ozet (Answer First)

- **Kural metni:** [.ai/CLAUDE.md](.ai/CLAUDE.md) - G1..G12 orada yazilir; bu dosya yalniz yonlendirir.
- **Okuma sirasi:** bu dosya -> [AGENTS.md](AGENTS.md) -> [README.md](README.md) -> [WORKFLOW.md](WORKFLOW.md) (AGENTS.md §1).
- **Kapi ozeti:** [WORKFLOW.md](WORKFLOW.md); gunluk durum ve kanit kaydi [.ai/log.md](.ai/log.md).
- **Cikti kutusu:** 5 alanli ASCII kutu, [.ai/AGENTS.md](.ai/AGENTS.md) §2 - `KANIT` bos ise is islenmez (G5).
- **Sayi kaynagi:** vault olcumleri [.ai/index.md](.ai/index.md) §4 scriptleriyle alinir; kanitsiz sayi yazilmaz (G9).

## §1 5 kirmizi cizgi (hatirlatma - metin degil)

| # | Cizgi | Tek satir anitim | Metin nerede |
|---|---|---|---|
| 1 | Kanit (G5/G9) | kanitsiz "bitti" ve olculmemis sayi yazilmaz | `.ai/CLAUDE.md` |
| 2 | Onay (G7) | silme, overwrite, `reset --hard`, `push --force` once onay ister | `.ai/CLAUDE.md` |
| 3 | Bicim (G4) | her MD 8 alanli frontmatter; UTF-8; ASCII `+ - \|` kutu; placeholder/WIP/TBD yasak | `.ai/.templates/new-doc-contract.md` |
| 4 | Kapi (G12) | GATE-003 onayindan once `src/**` altina tek satir `.cs` yazilmaz | [WORKFLOW.md](WORKFLOW.md) |
| 5 | Veri (G2) | ADO.NET + Dapper, parametreli sorgu; `EF Core` yasak | `.ai/.rules/data-access-rules.md` |

**En sik yapilan hata:** kural metnini bu dosyaya kopyalamak. Kopya = ikinci SSOT =
celiski uretir (G10). Link ver, kopyalama.

**Kapali soru:** "bu kural tam olarak ne diyor?" -> [.ai/CLAUDE.md](.ai/CLAUDE.md);
"simdi hangi kapi acik?" -> [.ai/log.md](.ai/log.md) son kayit; "vault saglik mi?"
-> [.ai/index.md](.ai/index.md) §4 olcum komutlari.
