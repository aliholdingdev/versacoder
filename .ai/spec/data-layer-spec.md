---
title: "VersaCoder — Veri Katmanı Şartnamesi (ADO.NET + Dapper)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l4 veri alt kümesi: repository sınırı, parameterized SQL zorunluluğu, bağlantı yaşam döngüsü, provider soyutlaması (ADR-005 / ADR-012)"
---

# VersaCoder — Veri Katmanı Şartnamesi (`spec/data-layer-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [architecture/index](../architecture/index.md) · [infra-layer-spec](infra-layer-spec.md) · [migration-spec](migration-spec.md) · [security-spec](security-spec.md)

> **Tek cümlelik sorumluluk:** *Veritabanına kim, hangi teknolojiyle, hangi SQL biçimiyle ve hangi timeout ile ulaşır* sorusunun tek cevabıdır.
> Kural listesi `.rules/data-access-rules.md` (DA-01..DA-16), şema değişikliği [migration-spec](migration-spec.md)'tedir — bu belge **sınır ve davranış**tır.

---

## §0 Özet (Answer First)

Veri erişimi **yalnız l4 repository'lerinde**, **ADO.NET + Dapper** ile ve **her sorgu `@p1..@pN` parametreli** yapılır; EF Core ve string-concat SQL **yoktur** (ADR-005, G2). Bağlantı kısa ömürlü (`using`), her komutta `CommandTimeout` setli, provider somut tipi **hiçbir zaman** l4 dışına çıkmaz — şema ise **yalnız** saf SQL migration ile değişir.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Repository arayüzü ve yerleşimi (yalnız `VersaCoder.Infrastructure`) | Katman sınırları: [infra-layer-spec](infra-layer-spec.md) |
| 2 | Parameterized SQL, `const` sorgu gövdesi, `DynamicParameters` | Injection savunması detayı: [security-spec](security-spec.md) |
| 3 | Bağlantı yaşam döngüsü, timeout sabitleri, transaction | Şema + versiyon tablosu: [migration-spec](migration-spec.md) |
| 4 | Provider soyutlaması (`IDbConnectionFactory`, `ISqlDialect`) | Paket sürüm kilidi: [packaging-spec](packaging-spec.md) |
| 5 | N+1 / sayfalama / streaming davranışı | Bütçe ve ölçüm yöntemi: [performance-spec](performance-spec.md) |

> **YASAK:** EF Core (G2) · `string` + SQL (DA-03) · `SELECT *` · `AddWithValue` ile belirsiz uzunluk · `CommandTimeout = 0` · paylaşılan statik `IDbConnection` · `Application`/`Domain` içinde SQL metni.

---

## §2 Davranış

### §2.1 Sorumluluk dağılımı

| # | Katman | SQL görür mü? | Not |
|---|---|---|---|
| 1 | l1 Domain | Hayır | Invariant yalnız |
| 2 | l2 Application | Hayır | Repository **arayüzünü** (l0) çağırır |
| 3 | l4 Infrastructure | **Evet** | Tek SQL sahibi; `const string` gövde |
| 4 | l7 UI / CLI | Hayır | ViewModel/DTO alır |
| 5 | Migration dosyaları | **Evet** | DDL'nin tek sahibi (DA-07) |

### §2.2 Sorgu davranışı

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | Parametre | `WHERE Id = @p1` + `DynamicParameters` | `"... WHERE Id = " + id` |
| 2 | Gövde | `const string sql = "..."` | Interpolasyonla kurulan sorgu |
| 3 | Kolon adı | Allowlist enum → sabit metin | Kullanıcıdan gelen kolon adı |
| 4 | Timeout | `cmd.CommandTimeout = DbTimeouts.Query` (15 sn) | Yok ya da `0` |
| 5 | Sonuç | Tip `record` satır (`SessionRow`) | `Query<dynamic>` ile dağınık erişim |

### §2.3 Bağlantı ve transaction akışı

```
  handler (l2)
      |  ISessionRepository (l0 arayuzu)
      v
  SessionRepository (l4)
      |  using var conn = _factory.Create();   <-- kisa omurlu, her cagrida yeni
      |  conn.Open();                          
      |  cmd = Query(sql, DynamicParameters)   <-- @p1..@pN, CommandTimeout=15
      |
      +-- yazma ise --> using tx = conn.BeginTransaction();
                        try  { ...; tx.Commit(); }
                        catch{ tx.Rollback(); throw; }   <-- tek yerde (DA-13)
```

| # | Kural | Sonuç |
|---|---|---|
| 1 | Kisa ömür: metot içi `using` (DA-10) | Yarış durumu yok |
| 2 | Tek birim iş = tek `DbTransaction`, iç içe yok | Yarım kayıt yok |
| 3 | SQLite: WAL + `foreign_keys=ON` + `busy_timeout` tek yerde | Testlerde kilit hatası yok (DA-14) |
| 4 | Büyük sonuçlar akışla okunur (`SequentialAccess`) | Bellek patlaması yok |
| 5 | Listeler `LIMIT/OFFSET` (100'lük sayfa) | Sonsuz `ReadAll` yok |

---

## §3 TFM Notu

| Bacak | Provider | Not |
|---|---|---|
| `net48` | `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` **5.2.x** (kilitli) | Dapper sürümü CPM'de tek yerde |
| `net10.0-windows` | Aynı paketler, aynı sürümler | Sürüm farkı = G8 ihlali |
| Kod | İki bacakta **aynı** repository kodu | `#if` yalnız provider API farkındaysa (ADR-012) |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| DL-01 | Gerçek SQLite şema üzerinde repository testi | `Infrastructure/Integration/` yeşil (mock yok) |
| DL-02 | Parameterized zorunluluğu | Concat SQL taraması = **0 satır** (G2) |
| DL-03 | EF Core yok | `.csproj`/`.props` taraması = 0 satır |
| DL-04 | Timeout her komutta | Kod denetimi + sahte gecikmeli sorgu → `TimeoutException` |
| DL-05 | İki bacakta aynı testler | `dotnet test -f net48` / `-f net10.0-windows` = 0 failed |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
