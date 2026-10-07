---
title: "ADR-012 — Microsoft.Data.SqlClient — iki bacakta aynı major"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "SqlClient/Sqlite paketleri net48 ve net10.0-windows bacaklarında aynı major sürümde"
tags: [adr, data, sqlclient, dependency]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Bacaklar farklı major sürüm kullansaydı NU1201 benzeri uyumsuzluk ve sessiz
davranış farkı çıkardı.

## §2 Karar

`Microsoft.Data.SqlClient` (ve ilgili veri sağlayıcı paketleri) **iki bacakta
aynı major** sürümde bulunur. Karar anındaki pin `5.x` idi; **güncel stable
7.1.1 (2026-09-29, WEB_VERIFIED)** — pin'in yenilenmesi gerekir (açık madde:
dependency-update-check ile izlenir).

## §3 Alternatifler

- **Bacak başına farklı major:** NU1201 + sessiz sapma. ❌
- **Tek major (seçilen):** uyum. ✅

## §4 Sonuçlar

**Ters-senaryo:** Bacaklar farklı major kullansaydı NU1201 benzeri uyumsuzluk ve
sessiz davranış farkı çıkardı.

**Yakalama:** `dependency-update-check` + iki bacak matrisi (`PROJECTS.md` §2.1).

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `Microsoft.Data.SqlClient` / `Sqlite` paket satırları | iki bacakta aynı major |

## §6 Kaynak

- brain.md §1 satır 012 · §1.1 · §1.2
- Kök CLAUDE.md Quality Report notu 3 (SqlClient 7.1.1 web doğrulaması, 2026-10-07)

## §7 İlgili

- [ADR-005-ado-net-dapper-no-ef-core.md](adr-005-ado-net-dapper-no-ef-core.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
