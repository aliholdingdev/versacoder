---
title: "Proje Modeli Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Çözüm/proje/dosya modeli ve sarmalayıcı (wrapper) katmanı - varlıklar, ilişkiler, workspace sınırı, depolama"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Proje Modeli Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[.rules/testing-rules|testing-rules]] · [[architecture/index|architecture]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/persistence-spec|persistence]] · [[spec/solution-explorer-spec|solution-explorer]] · [[spec/search-grep-spec|search-grep]]

> **Tek cümlelik sorumluluk:** *Çözüm/proje/dosya varlıkları nedir, birbirine nasıl bağlıdır ve diske nasıl sarmalanıp yazılır* sorusunun tek cevabıdır.
> Model **saf C#**tır (l1) ve **EF Core kullanmaz** (G2): kalıcılık ADO.NET + Dapper ile [[spec/persistence-spec|persistence]] katmanındadır.

---

## §0 Özet (Answer First)

Model üç seviyelidir: `Solution` → `ProjectNode` → `FileNode`. Kök (workspace) bir **sınırdır**: tüm yollar bu köke göre çözülür, kaçış reddedilir (SEC-11). Sarmalayıcı (`ProjectModel`), ham dosya sistemi/DB satırlarını **tek bir salt-okunur görünümde** birleştirir ve UI'a yalnız bu görünümü verir; dolayısıyla `TreeList`/arama aynı kaynağı paylaşır. Kayıtlar SQLite'tadır (ADO.NET + Dapper, parametreli sorgu), dosya içerikleri **değil** yalnız metadata.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Varlık tanımları ve ilişkileri (id, ad, yol, durum) | DB şeması/migration SQL'i | [[spec/persistence-spec|persistence]] |
| 2 | Workspace sınırı ve yol çözümleme | Ağaç render'ı/düğüm olayları | [[spec/solution-explorer-spec|solution-explorer]] |
| 3 | Sarmalayıcı (`ProjectModel`) API'si | Arama tarama motoru | [[spec/search-grep-spec|search-grep]] |
| 4 | Model olayları (`ProjectOpened`, `NodeChanged`) | Görev tetiklemeleri | [[spec/task-queue-spec|task-queue]] |
| 5 | Model seviyesi doğrulama (ad, çakışma) | Kullanıcıya hata metni | [[spec/error-handling-spec|error-handling]] |

---

## §2 Davranış

### §2.1 Varlık diyagramı

```
+--------------------------------------------------------------+
| Solution (tek)                                               |
|   id · ad · kokDizin (workspace) · olusturulma · sonAcilis   |
|      |                                                       |
|      +-- ProjectNode* (salt görünüm: klasör/dosya grubu)     |
|      |      id · ad · ebeveynId · tur(klasor|dosya)          |
|      |      |                                              |
|      |      +-- FileNode*  uzantı · boyut · degistiMi        |
|      |      |                                              |
|      +-- TaskRef* -- gorev kimligi (agent gorevleri)         |
+--------------------------------------------------------------+
```

### §2.2 Varlık ve kurallar

| # | Varlık | Alanlar | Kural |
|---|---|---|---|
| M1 | `Solution` | `Id`, `Name`, `RootPath`, `LastOpenedUtc` | kök dizin **var olmalı** ve yazılabilir |
| M2 | `ProjectNode` | `Id`, `Name`, `ParentId`, `Kind` | kökün dışında düğüm **oluşamaz** |
| M3 | `FileNode` | `Id`, `Path`, `SizeBytes`, `IsDirty` | yol daima köke göre çözülür |
| M4 | `TaskRef` | `TaskId`, `NodeId?` | görev kimliği [[spec/task-queue-spec|task-queue]] ile aynıdır |
| M5 | Sarmalayıcı | `IReadOnlyList<Node> Snapshot()` | anlık görüntü **değiştirilemez**; UI kopyasıdır |

### §2.3 Sarmalayıcı (wrapper) sorumlulukları

| # | Yapar | Yapmaz |
|---|---|---|
| W1 | Tarama + DB satırlarını **tek** ağaçta birleştirir | dosya **içeriği** okumaz (2 MB üstü dahil) |
| W2 | Yol çözümlemeyi ve kaçış denetimini **tek yerde** tutar | iş kuralı/LLM çağırmaz |
| W3 | Değişiklik olaylarını (`NodeChanged`) yayınlar | doğrudan UI kontrolüne dokunmaz |
| W4 | Aynı düğümü **tekrarsız** döner (kimlik = yol) | dosyayı silmez/taşımaz (komut l2'dir) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Model tipleri (l1) | C# 7.3 ile yazılır | aynı kaynak derlenir | dil sürümü farkı **dilbilgisine** bağlıdır (`readonly struct` vb.) |
| Yol API'si | `Path.GetFullPath` | aynı | **fark yok** |
| Kalıcılık | SQLite + Dapper (l4) | aynı | fark yok |

> `#if` yalnız platform farkında (ör. uzun yol davranışı); modelde hedef 0 (G6).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Yol kaçışı (`..`, mutlak, sembolik) | birim test | 0 kabul (SEC-11) |
| 2 | Sarmalayıcı tekilliği (aynı yol 2 kez) | birim test | tekrar yok |
| 3 | Anlık görüntü değiştirilemezliği | birim test | `NotSupportedException` |
| 4 | DB tur (Dapper parametreli) | SQLite entegrasyon | EF Core = 0, concat SQL = 0 |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
