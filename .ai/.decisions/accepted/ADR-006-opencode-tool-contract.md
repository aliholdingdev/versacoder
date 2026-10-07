---
title: "ADR-006 — opencode araç kümesi = VersaCoder.Protocol sözleşmesi"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "read/write/edit/bash/grep/glob/list — 7 araç, tek C# sözleşmesi"
tags: [adr, protocol, tools, contract]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Araçlar ayrı ayrı sarmalanırsa tek bir araç adı değişimi tüm çağrı noktalarını
kırar; hata noktası belli olmaz.

## §2 Karar

opencode araç kümesi (`read/write/edit/bash/grep/glob/list` — **7 araç**) tek bir
C# sözleşmesinde tanımlanır: `VersaCoder.Protocol` (l5). Araç çağrısı = sözleşme
üzerinden; doğrudan adaptör çağırmak yasak.

## §3 Alternatifler

- **Her araç için ayrı arayüz + doğrudan çağrı:** esnek ama kırılgan. ❌
- **Tek sözleşme (seçilen):** ince arayüz, tek değişiklik noktası. ✅

## §4 Sonuçlar

**Ters-senaryo:** Araçlar ayrı ayrı sarmalansaydı tek bir araç adı değişimi tüm
çağrı noktalarını kırar, hata noktası belli olmazdı.

**Yakalama:** `spec/tool-contract-spec.md` ile sözleşmenin birebir eşleşmesi.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| Protocol `tool-schema` + `spec/tool-contract-spec.md` | sözleşmedeki araç adı = 7 |

## §6 Kaynak

- brain.md §1 satır 006 · §1.1 · §1.2

## §7 İlgili

- [ADR-009-tool-execution-sandbox.md](ADR-009-tool-execution-sandbox.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
