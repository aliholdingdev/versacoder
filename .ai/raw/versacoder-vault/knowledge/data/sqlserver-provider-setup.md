---
title: "sqlserver-provider-setup - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Microsoft.Data.SqlClient 5.2.x kurulumu, CPM paket satiri, baglanti metni yazimi ve factory kaydi (ADR-012 D1)"
---

# sqlserver-provider-setup — SQL Server Sağlayıcı Kurulumu

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md) · [data index](../index.md) · [connection-string-secrets](connection-string-secrets.md)

> **Tek cümlelik sorumluluk:** *`Microsoft.Data.SqlClient` hangi sürümle, hangi satırla eklenir, bağlantı metni nasıl yazılır, factory'ye nasıl bağlanır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Tek sürüm: **`Microsoft.Data.SqlClient` 5.2.x** ve **her iki bacakta aynı** (ADR-012 D1). 6.x **kullanılmaz** — .NET Standard desteğini düşürdü, `net48` restore/derleme kırılır. Paket sürümü **CPM** ile tek noktada (`Directory.Packages.props`), `.csproj` sürüm yazmaz. Sağlayıcı somut sınıfı yalnızca `SqlServerConnectionFactory` içindedir; uygulama `DbConnection` görür (DA-04).

---

## §1 Kurulum (CPM)

```xml
<!-- Directory.Packages.props (tek nokta) -->
<PackageVersion Include="Microsoft.Data.SqlClient" Version="5.2.2" />
<PackageVersion Include="Dapper" Version="2.1.66" />

<!-- VersaCoder.Data.csproj (surum YOK) -->
<PackageReference Include="Microsoft.Data.SqlClient" />
<PackageReference Include="Dapper" />
```

---

## §2 Baglanti metni tablosu

| Anahtar | Önerilen değer | Not |
|---|---|---|
| `Server` / `Data Source` | `localhost` / `.` | yerel geliştirme |
| `Database` | `VersaCoder` | üretim adı ortam ayarı |
| `Integrated Security` | `true` | parola metni **yok** (bkz. kardeş dosya) |
| `Encrypt` | `true` | SQL Client 5.x varsayılanı güvenli |
| `TrustServerCertificate` | yalnız yerel geliştirme | üretimde `false` |
| `Max Pool Size` | `100` (varsayılan) | havuz davranışı kardeş dosyada |
| `Connect Timeout` | `15` | bağlantı kendisi için ayrı süre |

---

## §3 Kural tablosu (ADR-012 · DA-04)

| # | Kural | Yasak |
|---|---|---|
| 1 | Sürüm **5.2.x**, CPM'te tek satır | `6.x` · `.csproj`'de `Version=` |
| 2 | Somut sınıf yalnız factory/DI | repository'de `new SqlConnection(...)` |
| 3 | Bağlantı metni sır ise DPAPI blob | düz metin parola |
| 4 | `Encrypt=true` + sertifika bilinci | kör `TrustServerCertificate=true` |
| 5 | Sağlık kontrolü `SELECT 1` + `DbTimeouts.Query` | timeout'suz ping |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Paket | `Microsoft.Data.SqlClient` **5.2.x** (NS 2.0) | **aynı 5.2.x** |
| `6.x` | restore kırılır (NS düşürüldü) | teknik olarak gelir → **yine yasak** (sürüm kilidi) |
| `System.Data.SqlClient` | yerleşik eski sürümden | **kullanılmaz** — iki kod yolu olur (ADR-012 A2) |
| `SqlBulkCopy` | 5.2.x ile var | aynı |
| Async API | `OpenAsync`/`ExecuteReaderAsync` var | aynı |

---

## §5 Kardeşler

[dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [connection-string-secrets](connection-string-secrets.md) · [connection-pooling-notes](connection-pooling-notes.md) · [ado-net-connection](ado-net-connection.md)
