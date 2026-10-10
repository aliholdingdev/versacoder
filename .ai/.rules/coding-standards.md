---
title: "Kodlama Standartları (G1 G4 G5 G9 G10 G11 G13 G14 G15)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Vault + kod yazım standartları; sıfırdan yazım, kanıt, Türkçe, yer tutucu"
tags: [rules, coding, standards, g1, g4, g5, g9, g13]
source: "Birleştirme: .ai/CLAUDE.md §1 (G1/G4/G5/G9/G10/G11) + kök CLAUDE.md (G5 Türkçe, G4 placeholder, G12 SRP) + brain.md §3"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

# Kodlama Standartları

## G1 — Sıfırdan yazım (mülkiyet)

Eski vault / prompt arşivi / CoreMusic **yalnız bilgi kaynağıdır**; içerik kendi
cümlelerimizle yazılır. `Copy-Item`, blok yapıştırma, birebir cümle yasak.
İhlalde: dosya silinir, yeniden yazılır. Kanıt: `git status` + cümle benzerliği.
(Kaynak: ADR-002 · .ai/CLAUDE.md G1)

## G4 — Vault biçimi

Her `.md`: 11 alanlı YAML frontmatter (D23: 8 eski + `tags/source/supersedes`),
UTF-8, 0 bozuk link, 0 mojibake. Frontmatter'sız dosya vault'a ait değildir.
İhlalde: biçim düzeltilir. Kanıt: FM taraması = 0 eksik.

## G5 — Kanıtsız "bitti" yasak

"Tamam/bitti" **yalnız** çalıştırılmış komut çıktısıyla söylenir (build, test,
tarama). Etiket zorunlu: `REPOSITORY_VERIFIED / SOURCE_VERIFIED / WEB_VERIFIED /
INFERRED / NEEDS_VALIDATION / UNKNOWN`. Etiketsiz teknik iddia uydurmadır,
madde silinir. Kanıt: `KANIT` satırında komut + sayı.

## G9 — Uydurma (zero-hallucination)

Var olmayan dosya/class/API/NuGet/sürüm/ad iddia edilemez. Bilinmeyen:
`DOĞRULAMA GEREKLİ` yazılır, tahmin girmez. Kanıt: etiket + kaynak satırı.

## G10 — Çelişki işleme

Kaynak çelişince `CONFLICT DETECTED`; authority zinciri (ADR > Kural > Gate >
Türev, D5) ile çözülür; çözülmüyorsa `DECISION BLOCKED` + tek soru. Sessiz
çözüm yasak. Kanıt: BLOCKED raporu + log satırı.

## G11 — Gizli işlem yok

Yıkıcı / geri alınamaz / uzun süreli sessiz iş önceden raporlanır. Arka planda
sessiz migration, sessiz paket kurulumu yasak. Kanıt: görev listesi + onay satırı.

## G13 — Türkçe + mojibake

Vault içeriği Türkçe yazılır (teknik terimler İngilizce kalabilir). Mojibake
(`Ã¼`, `�` vb.) yasak. İhlalde: çevrilir/temizlenir. Kanıt: mojibake taraması = 0.

## G14 — Yer tutucu yok

Üretilen dosyada `{{değişken}}`, `TODO`, `TBD`, `WIP`, `~` gibi yer tutucu
**kalmaz** (0 adet); kural maddesi olarak yazılan "placeholder yasak" ifadeleri
kuralıdır, ihlal sayılmaz (klasör içi istisna: şablon `.templates/` — orada
yer tutucu **amaçtır**). İhlalde: doldurulur ya da madde silinir.

## G15 — Vault SRP

Tek dosya tek konu; tek dizinde tek indeks/liste (ADR-021). Aynı liste iki
dosyada tekrarlanmaz. İhlalde: bölünür ya da listeye indirgenir. Kanıt:
duplicate taraması.

## Genel kod stili (vault içi)

- Kod örnekleri BAD → GOOD çiftiyle verilir (brain §3.7 deseni).
- `Result<T>` dönüşleri [[.rules/error-handling-rules|error-handling-rules]] kapsamında.
- `nullable` açık; `!` savsaklama olarak kullanılmaz (brain §3/4).
- Kod içindeki yorumlar gerekiyorsa Türkçe.

## İlgili

- [[.rules/index|rules-index]] · [[.decisions/accepted/adr-002-vault-ownership-no-copy|ADR-002]]
- [[.decisions/accepted/adr-003-vault-structure-500md-frontmatter|ADR-003]]
