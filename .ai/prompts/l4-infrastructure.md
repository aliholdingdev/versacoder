---
title: "prompt - l4 Infrastructure (ADO.NET + Dapper, Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l4 Infrastructure katmanini (Repository, Dapper sorgu, migration, dosya, HTTP, Process) TAM KOD olarak ureten prompt - 3 kanit zorunlu, EF Core YASAK (G2), DA-01..16 data-access-check"
---

# prompt — l4 Infrastructure (ADO.NET + Dapper, Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [data-access-rules](../.rules/data-access-rules.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [data-access-check](../checklists/data-access-check.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-005](../.decisions/accepted/)

> **Tek cumlelik sorumluluk:** l4 `Infrastructure` katmanindaki somut dis dunya
> erisimini (ADO.NET + Dapper, dosya, HTTP, Process) **tam kod** olarak ureten,
> **3 kanitla** kapanan is emridir — **EF Core YASAK** (G2, CRITICAL).

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l4` — `VersaCoder.Infrastructure` (şemsiye → 29 paket, Faz A-D) |
| K bandı | `K0200-K0249` + 29 mikro (`K0200-K0228`) |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `DA-01..16` (data-access-check) + `VC-CLI-002` (build) |
| Ajan | `developer` → `security-auditor` (SQL injection) → `tester` |
| Kapı | GATE-005 + data-access-check |
| Kanıt | 3 tane: build + test + DA-01..16 |

**YASAK:** `Microsoft.EntityFrameworkCore*` · `DbContext` · LINQ-to-Entities ·
`$"SELECT …{x}"` · `+` ile SQL birleştirme · `CommandTimeout 0` · parametresiz `Execute`.

---

## §1 Baglam

1. `TODOS/05 §0` satır 5: **l4 = ADO.NET+Dapper + migration**, kapı `DA checklist`.
2. `ADR-028` §5.1: `l4` EF Core · string-concat SQL · parametresiz `Execute` **yasak**.
3. `data-access-rules` + `data-access-check` DA-01..16.
4. `l0` portlar (`IClock`, `IFileSystem`, `IVaultReader`) → l4 uygular.

---

## §2 Görev — TAM KOD

### §2.1 `IUnitOfWork` + Dapper repository (`K0200-K0209`)

```csharp
// src/VersaCoder.Infrastructure/Data/UnitOfWork.cs
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace VersaCoder.Infrastructure.Data;

/// <summary>ADO.NET + Dapper — EF Core YASAK (G2). Parametreli sorgu zorunlu.</summary>
public interface IUnitOfWork : IDisposable
{
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default);
    Task BeginAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public sealed class SqliteUnitOfWork : IUnitOfWork
{
    private readonly DbConnection _conn;
    private DbTransaction? _tx;
    public SqliteUnitOfWork(DbConnection conn) => _conn = conn;

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        // DA-03: parametreli sorgu; DA-04: CommandTimeout > 0
        if (_conn.State != ConnectionState.Open) await _conn.OpenAsync(ct);
        return await _conn.QuerySingleAsync<T>(sql, param, _tx, commandTimeout: 30);
    }
    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default)
    {
        if (_conn.State != ConnectionState.Open) await _conn.OpenAsync(ct);
        var r = await _conn.QueryAsync<T>(sql, param, _tx, commandTimeout: 30);
        return r.ToList();
    }
    public async Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default)
    {
        if (_conn.State != ConnectionState.Open) await _conn.OpenAsync(ct);
        return await _conn.ExecuteAsync(sql, param, _tx, commandTimeout: 30);
    }
    public async Task BeginAsync(CancellationToken ct = default)
    {
        if (_conn.State != ConnectionState.Open) await _conn.OpenAsync(ct);
        _tx = await _conn.BeginTransactionAsync(ct);
    }
    public async Task CommitAsync(CancellationToken ct = default) { if (_tx is not null) await _tx.CommitAsync(ct); }
    public async Task RollbackAsync(CancellationToken ct = default) { if (_tx is not null) await _tx.RollbackAsync(ct); }
    public void Dispose() { _tx?.Dispose(); _conn.Dispose(); }
}
```

### §2.2 Migration (ham SQL + `schema_version`) (`K0210-K0219`)

```csharp
// src/VersaCoder.Infrastructure/Data/MigrationRunner.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Infrastructure.Data;

/// <summary>EF Core migration YASAK (G2); ham SQL + schema_version.</summary>
public sealed class MigrationRunner
{
    private readonly IUnitOfWork _uow;
    public MigrationRunner(IUnitOfWork uow) => _uow = uow;

    private const string CreateVersionTable =
        "CREATE TABLE IF NOT EXISTS schema_version (version INTEGER PRIMARY KEY, applied_at TEXT NOT NULL)";

    public async Task<int> GetVersionAsync(CancellationToken ct = default)
    {
        await _uow.ExecuteAsync(CreateVersionTable, null, ct);
        var v = await _uow.QueryAsync<int>("SELECT COALESCE(MAX(version),0) FROM schema_version", null, ct);
        return v.Count > 0 ? v[0] : 0;
    }

    public async Task ApplyAsync(IReadOnlyList<(int Version, string Sql)> migrations, CancellationToken ct = default)
    {
        var current = await GetVersionAsync(ct);
        foreach (var (ver, sql) in migrations)
        {
            if (ver <= current) continue;
            await _uow.ExecuteAsync(sql, null, ct);
            await _uow.ExecuteAsync(
                "INSERT INTO schema_version (version, applied_at) VALUES (@v, @t)",
                new { v = ver, t = System.DateTimeOffset.UtcNow.ToString("O") }, ct);
        }
    }
}
```

### §2.3 `FileSystemAdapter` + `HttpClientAdapter` (`K0220-K0234`)

```csharp
// src/VersaCoder.Infrastructure/FileSystem/FileSystemAdapter.cs
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Infrastructure.FileSystem;

public sealed class FileSystemAdapter : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);
    public string ReadAllText(string path) => File.ReadAllText(path);
    public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
    public Task<string> ReadAllTextAsync(string path, CancellationToken ct) => File.ReadAllTextAsync(path, ct);
}

// src/VersaCoder.Infrastructure/Http/HttpClientAdapter.cs
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Infrastructure.Http;

/// <summary>LLM endpoint'leri icin — llm-provider-spec §3 (OpenRouter + NIM).</summary>
public sealed class HttpClientAdapter
{
    private readonly HttpClient _http;
    public HttpClientAdapter(HttpClient http) => _http = http;
    public async Task<string> PostJsonAsync(string url, string json, string? bearer, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        if (!string.IsNullOrEmpty(bearer)) req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
        using var resp = await _http.SendAsync(req, ct);
        return await resp.Content.ReadAsStringAsync(ct);
    }
}
```

---

## §3 Kabul Kriterleri

- [ ] `IUnitOfWork`, `SqliteUnitOfWork`, `MigrationRunner`, `FileSystemAdapter`, `HttpClientAdapter` — 5 tip
- [ ] **EF Core = 0 eşleşme** (G2 CRITICAL)
- [ ] **String-concat SQL = 0 eşleşme** (G2)
- [ ] `CommandTimeout > 0` her `Execute`/`Query`
- [ ] `data-access-check` DA-01..16 PASS
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> repository + migration testi yeşil
KANIT 3: data-access-check DA-01..16 -> PASS (EF=0, concat=0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l4 Infrastructure yazildi               |
| KAPSAM : src/VersaCoder.Infrastructure/** (5 tip)|
| KARAR  : ADO.NET + Dapper + ham SQL migration     |
| RISK   : 29 mikro paket Faz A-D'de bolunecek      |
| KANIT  : build x2 = 0 + test yeşil + DA PASS     |
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 5 tip kodu |
| 2 | `security-auditor` | SQL injection + secret |
| 3 | `tester` | repository + migration test |
| 4 | `reviewer` | G2 + DA-01..16 |

---

**Olculen kanit:** `.ai/prompts/l4-infrastructure.md` kaydedildi; `prompts/` = 36 dosya.
