---
title: "sqlserver-provider-setup — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Microsoft.Data.SqlClient 5.2.x kurulumu, CPM paket satiri, baglanti metni yazimi ve factory kaydi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# sqlserver-provider-setup — SQL Server Sağlayıcı Kurulumu

**Zorunlu Baglantilar:** [[index|vault index]] · [[knowledge/index|knowledge index]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/ado-net-connection|ado-net-connection]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *`Microsoft.Data.SqlClient` hangi sürümle, hangi satırla eklenir, bağlantı metni nasıl yazılır, factory'ye nasıl bağlanır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Tek sürüm **`Microsoft.Data.SqlClient` 5.2.x** ve **her iki bacakta aynıdır**. 6.x kullanılmaz: .NET Standard desteğini düşürdüğü için `net48` restore/derlemesi kırılır. Paket sürümü Central Package Management ile tek noktada durur (`Directory.Packages.props`), `.csproj` dosyasında `Version=` yazılmaz. Somut sınıf yalnız `SqlServerConnectionFactory` içindedir; uygulamanın gördüğü tip `DbConnection`'dur (DA-04).

## §1 Kurulum (CPM)

```xml
<!-- Directory.Packages.props - tek nokta -->
<PackageVersion Include="Microsoft.Data.SqlClient" Version="5.2.2" />
<PackageVersion Include="Dapper" Version="2.1.66" />

<!-- VersaCoder.Data.csproj - surum YOK -->
<PackageReference Include="Microsoft.Data.SqlClient" />
<PackageReference Include="Dapper" />
```

Sürüm yalnız bir kez yazılır; projeler sürümü miras alır, kopyalamaz.

## §2 Baglanti metni tablosu

| Anahtar | Önerilen değer | Not |
|---|---|---|
| `Server` / `Data Source` | `localhost` / `.` | yerel geliştirme |
| `Database` | `VersaCoder` | üretim adı ortam ayarı |
| `Integrated Security` | `true` | parola metni yok |
| `Encrypt` | `true` | SQL Client 5.x varsayılanı güvenlidir |
| `TrustServerCertificate` | yalnız yerel geliştirme | üretimde `false` |
| `Max Pool Size` | `100` (varsayılan) | havuz davranışı kardeş dosyada |
| `Connect Timeout` | `15` | bağlantı kurulumu için ayrı süre |

## §3 Kural tablosu

| # | Kural | Yasak olan |
|---|---|---|
| 1 | Sürüm 5.2.x, CPM'te tek satır | `6.x`; `.csproj`'de `Version=` |
| 2 | Somut sınıf yalnız factory/DI içinde | repository'de `new SqlConnection(...)` |
| 3 | Bağlantı metni sır ise güvenli depo (DPAPI blob) | düz metin parola |
| 4 | `Encrypt=true` + sertifika bilinci | kör `TrustServerCertificate=true` |
| 5 | Sağlık kontrolü `SELECT 1` + `DbTimeouts.Query` | timeout'suz ping |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Paket | `Microsoft.Data.SqlClient` 5.2.x (NS 2.0) | **aynı** 5.2.x |
| `6.x` | restore kırılır | teknik gelir → yine yasak (sürüm kilidi) |
| `System.Data.SqlClient` | yerleşik eski sürümden | kullanılmaz — iki kod yolu açılır |
| `SqlBulkCopy` | 5.2.x ile var | aynı |
| Async API | `OpenAsync` / `ExecuteReaderAsync` | aynı |

İki bacakta **tek kod yolu** seçilmiştir: sağlayıcı kodu TFM'e göre dallanmaz.

## §5 Kurulum kontrol listesi

| Adım | Doğru davranış | Hata |
|---|---|---|
| Paket ekleme | CPM satırı + sürümsüz `PackageReference` | iki projede farklı sürüm |
| Factory kaydı | DI'da `DbConnection` üreten factory | her yerde `new SqlConnection` |
| Bağlantı metni | ortam ayarı / güvenli depo | koddaki düz metin parola |
| Sertifika | üretimde `Encrypt=true`, `TrustServerCertificate=false` | kör `true` |
| Sağlık | `SELECT 1` + `DbTimeouts.Query` | timeout'suz sorgu |

## §6 Sik hatalar

- 6.x'e "yeni olsun" diye yükseltmek (net48 kırılır)
- `.csproj` içine sürüm yazıp CPM ile çakıştırmak
- Repository içinde doğrudan somut bağlantı sınıfı new'lemek
- Parolayı config dosyasında düz metin tutmak
- `TrustServerCertificate=true`'yı üretime taşımak

## §7 Dogrulama notlari

CPM satırındaki tam sürüm (5.2.2) ile kilitlenmiş patch sürümün vault'ta tek kaynak olup olmadığı ayrıca teyit edilmelidir — **DOĞRULAMA GEREKLI**. Sürüm değişikliği ADR kararı ile yapılır.

## §8 Kardesler

[[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/ado-net-connection|ado-net-connection]]

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `sqlserver-provider-setup.md` (kaynak) | CPM kurulum kodu, bağlantı metni tablosu, sürüm kuralı, TFM tablosu |
| `connection-string-secrets.md` | sır niteliğindeki metnin korunması |
| `connection-pooling-notes.md` | `Max Pool Size` ve havuz davranışı |
| `ado-net-connection.md` | factory sözleşmesi (`Create`/`OpenAsync`) |
| `sqlite-provider-setup.md` | ikinci sağlayıcının kapsamı ve test bacağı |
