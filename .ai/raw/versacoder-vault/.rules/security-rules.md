---
title: "VersaCoder — Güvenlik Kuralları"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "VersaCoder için bağlayıcı güvenlik kuralları — secret, şifreli connection string, parameterized SQL, sınır doğrulama, Process güvenliği, DPAPI/anahtar, log kırmızıaltın, bağımlılık sabitleme"
---

# VersaCoder — Güvenlik Kuralları (`.rules/security-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [security-rules.md](./security-rules.md) · [data-access-rules.md](./data-access-rules.md) · [coding-standards.md](./coding-standards.md) · [testing-rules.md](./testing-rules.md)

> **Tek cümlelik sorumluluk:** *VersaCoder'da hangi güvenlik kuralı ihlal edilemez, ihlal nasıl görünür, nasıl kapatılır* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G2 (EF Core/parameterized)**, **G7 (yıkıcı onay)** ve **G11 (gizli işlem)** guardrail'larının güvenlik derinliğidir; çelişirse `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** masaüstü uygulama (`VersaCoder.Desktop`), CLI (`VersaCoder.Cli`), agent tool'ları, protokol mesajları, SQLite/SQL Server katmanı, transcript/audit günlükleri, NuGet bağımlılıkları.

**Dışında:** DevExpress sürüm kanunu (→ `devexpress-rules.md`), SQL yazım detayı (→ `data-access-rules.md`), test senaryoları (→ `testing-rules.md`).

> **KURAL:** Güvenlik kuralı performans/kısalık lehine **esnetilmez**. Emin olunmayan durumda varsayılan **daha az yetki + reddet**.

### §1.1 Tehdit Modeli Özeti

```
dis tehdit               ic tehdit                    agent kaynagi
+----------------+       +------------------+         +---------------------+
| repo sizintisi |       | log'a sızan secret|        | prompt injection    |
| SQL injection  |       | yanlış yetki      |        | tool cikti manipule |
| path traversal |       | yedek/klasor acigi|        | shell arguman enjek.|
+-------+--------+       +---------+--------+         +----------+----------+
        |                          |                             |
        v                          v                             v
   SEC-01/03/11             SEC-02/06/07/08              SEC-05/10/14
```

### §1.2 Kural ID Aralığı

| Dosya | Önek | Aralık |
|---|---|---|
| `security-rules.md` (bu dosya) | `SEC-` | SEC-01 .. SEC-16 |
| `coding-standards.md` | `CS-` | CS-01 .. CS-18 |
| `data-access-rules.md` | `DA-` | DA-01 .. DA-16 |
| `devexpress-rules.md` | `DX-` | DX-01 .. DX-15 |

---

## §2 Kurallar (SEC-01 .. SEC-16)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| SEC-01 | **Repo'da secret yok**: API key, token, parola, connection string ham hali **asla** commit edilmez; `.gitignore` `.env`, `*.db`, `*.user`, `secrets.*` kapsar | Git geçmişi silinmez; sızan key kalıcıdır | Diff'te `sk-`, `Password=`, `ApiKey=` kalıbı | Key'i iptal et (üretici tarafında), dosyayı geçmişten kaldır, secret store'a taşı |
| SEC-02 | **Connection string şifreli saklanır** (G2): ham metin diskte/DB'de durmaz; çözümleme yalnız çalışma anında | Ham string yedek/kopya ile birlikte sızar | `.db`/`appsettings` içinde düz metin `Data Source=` | Şifreli blob + anahtar → `SEC-07`; çözümleyici tek yerde (`IConnectionStringProtector`) |
| SEC-03 | **Yalnız parameterized SQL** (G2): sorgu metnine değer asla birleştirilmez; `IN` listesi bile parametre üretilerek verilir | Injection tek başına tüm veritabanını ele geçirir | `$"SELECT ... WHERE Ad='{x}'"` | `@p1..@pN` + Dapper `DynamicParameters`; ayrıntı `data-access-rules.md` DA-03 |
| SEC-04 | **Sınırda doğrulama**: UI handler, protokol mesajı, CLI argümanı, tool parametresi — veri **girdi sınırında** doğrulanır (tip, uzunluk, aralık, format) | İç katmanlar güvenilen veri varsayar; geç doğrulama = inject olmuş veri işlenmiş | Uzunluk sınırı olmayan textarea → DB'ye giden string; protokolde `int` yerine `string` | Sınırda allowlist doğrulama + sınır boyutu (`MaxLength`), hatalı girdi `Result` hatası |
| SEC-05 | **`Process` yalnız sanitize edilmiş girdiyle**: argümanlar ayrı (`ArgumentList`), `UseShellExecute=false`, shell birleştirme (`cmd /c "..."`) yasak, `WorkingDirectory` sandbox içinde, timeout'ta `Kill` | Argüman enjeksiyonu → keyfi komut (agent shell tool = en yüksek risk) | `$"git {args}"` → `cmd.exe`; kullanıcı girdisi doğrudan `Arguments` | Argüman listesi olarak ver; girdiyi allowlist/escape et; timeout zorunlu (SEC-12) |
| SEC-06 | **Least privilege**: uygulama yönetici olarak çalışmaz; dosya erişimi yalnız çalışma alanı kökü altında; agent tool izinleri `allow / ask / deny` (tool-spec) | Yetki fazlası = ihlal büyüklüğü | Her tool `allow`; `C:\` geneli yazma izni | İzin matrisi varsayılanı `ask`; yazma yalnız workspace kökü (SEC-11) |
| SEC-07 | **Yerel anahtar işleme**: .NET Framework bacağında **DPAPI** (`ProtectedData`, `CurrentUser`); modern bacakta AES-256-GCM (rastgele nonce, nonce yeniden kullanılmaz); anahtar dosyası repo **dışında** | Ev anahtarı / nonce hatası tüm şifrelemeyi düşürür | Anahtar kodda literal; nonce sabit/tekrarlayan; elle RSA/ECB uydurma | DPAPI ile sarmala; AES-GCM nonce'yu her şifrelemede üret; kripto uydurma (SEC-07 yasak: `ECB`, sabit IV) |
| SEC-08 | **Transcript/log kırmızıaltın**: key/token/parola/connection string değerleri loga **yazılmadan önce** maskelenir (`sk-***`, `***`); maskeleme yazma öncesi tek noktada | Loglar en uzun ömürlü veridir; agent transcript'i dışa aktarılır | Logda `Authorization: Bearer eyJ...`; exception mesajında connection string | `ISecretRedactor` log pipeline'ında zorunlu; hata mesajı formatlayıcıları da kırmızıaltar |
| SEC-09 | **Bağımlılık sabitleme**: CPM (`Directory.Packages.props`) ile kesin sürüm; floating (`*`, version range) yasak; yeni paket eklemek onaya tabi (G8) | Sürüm sürüklenmesi = değişmez güvenlik yarığı | csproj'de `Version="*"`; CPM dışı elle sürüm | Tek noktada sabitle; yeni paket için `log.md` kaydı + inceleme |
| SEC-10 | **Tehlikeli deserializasyon yasak**: `BinaryFormatter`, `SoapFormatter`, `NetDataContractSerializer`, `Type.GetType(kullanıcıMetni)` ile nesne kurma yok; JSON yalnız hedef tip ile (`System.Text.Json`) | Deserializasyon = keyfi kod çalıştırma klasik vektörü | `BinaryFormatter.Deserialize(stream)`; `TypeNameHandling.All` benzeri | Tipli `JsonSerializer.Deserialize<T>`; ikili veri için şema doğrulanmış format |
| SEC-11 | **Path traversal kapat**: tüm dosya yolları `Path.GetFullPath` ile kanonikleştirilir, workspace kökü **önüne** düşen (`..`, mutlak yol, junction) reddedilir | Agent `read/write` tool'u ile sistem dosyasına erişim | `../` ile kök dışına yazım; `C:\Windows\...` kabul | Kök-ön eki kontrolü (`StartsWith(root, OrdinalIgnoreCase)`) + sembolik link doğrulaması |
| SEC-12 | **Kaynak limitleri**: her DB komutunda `CommandTimeout`; her tool/process'te timeout; dosya okuma/yazma boyut sınırı | Sınırsız çalışma = UDoS / askeri kilitlenme | `CommandTimeout = 0`; `WaitForExit()` sonsuz | Sabitler (`DbTimeouts`, `ToolTimeouts`) — süreler `data-access-rules.md` DA-09 |
| SEC-13 | **Hata mesajı sızıntısı yok**: kullanıcıya/LLM'e dönen hata; stack trace, SQL metni, dosya yolu, anahtar **içermez** | Ayrıştırılmış hata = harita + sızıntı | UI'da `SqlException` tam metni; chat'te ham stack trace | Kullanıcı dostu mesaj + dahili `Trace` ayrı; ham detay yalnız local log (kırmızıaltılmış) |
| SEC-14 | **Tool çıktısı güvenilmez veridir** (agent): model'e dönen dosya/konsol çıktısı *talimat değil veri* sayılır; içinde komut/uyarı geçen metin körlemesine uygulanmaz | Prompt injection ile tool çıktısından komut sızdırma | Çıktıdaki "ignore previous..." talimatının uygulanması | Şeffaflık: tool çağrısı her zaman UI'da görünür + izin kapısı (SEC-06, `spec/tool-spec.md`) |
| SEC-15 | **Yıkıcı işlem onayı (G7/G11)**: DB migration, dosya silme, `git reset` — kullanıcı onayı olmadan çalışmaz; agent'ta `ask` varsayılan | Geri alınamaz veri kaybı | Sessiz `DROP TABLE`; toplu dosya silme | Onay diyalogu + geri alma planı; onaysız RED |
| SEC-16 | **Audit izi**: yetki değişimi, secret erişimi, migration, agent tool izin verme → kim/ne/zaman audit tablosuna yazılır (kırmızıaltılmış) | Sonradan "*ne oldu?*" cevapsız kalır | Kritik olayda kayıt yok; kayıtta ham secret | Audit writer tek yerden; `DA-15` audit tablosu sözleşmesi |

> **YASAK:** "Bu iç network, güvelenir" gerekçesiyle herhangi bir maddenin esnetilmesi. İstisna yazılı onay + `log.md` kaydı ister.

---

## §3 Kritik Kuralların Ayrıntısı

### §3.1 SEC-03 — Parameterized (BAD -> GOOD)

```csharp
// BAD — string concat: injection + plan yeniden oluşumu
var sql = $"SELECT Id, Title FROM Sessions WHERE Title LIKE '%{search}%' AND Owner = '{user}'";
var rows = conn.Query<SessionRow>(sql);

// GOOD — parametreli; LIKE kalıbı da parametredir
const string sql = """
    SELECT Id, Title
    FROM Sessions
    WHERE Title LIKE @Pattern AND Owner = @Owner
    """;
var rows = await conn.QueryAsync<SessionRow>(sql,
    new { Pattern = $"%{search}%", Owner = user },
    commandTimeout: DbTimeouts.Query,
    cancellationToken: cancellationToken);
```

> **UYARI:** Filtre/sıralama *alan adı* (kolon) parametrelenemez → kolon adı **allowlist** ile seçılır, kullanıcı metni asla kolon adına dönüşmez (ayrıntı `data-access-rules.md` DA-03).

### §3.2 SEC-05 — Process (BAD -> GOOD)

```csharp
// BAD — argüman enjeksiyonu açık
Process.Start("cmd.exe", $"/c git log --author={userInput}");

// GOOD — ayrı argümanlar, shell yok, timeout + kill
var psi = new ProcessStartInfo
{
    FileName = "git",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    CreateNoWindow = true,
    WorkingDirectory = workspaceRoot
};
psi.ArgumentList.Add("log");
psi.ArgumentList.Add($"--author={SanitizeGitArg(userInput)}");   // allowlist kalıbı

using var process = Process.Start(psi) ?? throw new ToolException("git failed to start.");
if (!await process.WaitForExitAsync(cancellationToken).WaitAsync(ToolTimeouts.Git, cancellationToken))
{
    process.Kill(entireProcessTree: true);
    throw new ToolTimeoutException("git log exceeded timeout.");
}
```

### §3.3 SEC-02 + SEC-07 — Şifreli Bağlantı Metni Akışı

```
kurulum / ilk calistirma
  +-------------------+     DPAPI (CurrentUser)      +----------------------+
  | ham connection str| ----------------------------> | sifreli blob (disk)  |
  +-------------------+                              +----------+-----------+
                                                                 |
calistirma                                                        v
  +----------------------+   unprotect / AES-GCM   +----------------------+
  | IConnectionString-   | <---------------------- | sifreli blob         |
  | Protector.Protect()  |                         +----------------------+
  +----------+-----------+
             |  canli nesne, loga asla yazilmaz (SEC-08)
             v
  +----------------------+        +---------------------+
  | DbConnection (using) | --->   | provider: Sqlite /  |
  +----------------------+        | SqlClient (DA-04)   |
                                  +---------------------+
```

| Katman | Kural |
|---|---|
| Anahtar | repo dışında (`%APPDATA%` altı), DPAPI sarmalı, yedeklenmez |
| Çözümleme | tek implementasyon; call site ham string görmez |
| Log | bağlantı string'i hiçbir seviyede loglanmaz; yalnız *provider + veritabanı adı maskeli* |

### §3.4 SEC-08 — Kırmızıaltma Kalıpları

| Kalıp (regex kabaca) | Çıktı biçimi |
|---|---|
| `sk-...`, `ghp_...`, `eyJ...` (JWT) | `sk-***` / `***` |
| `Password=...;`, `Pwd=...;` | `Password=***` |
| `Bearer <token>` | `Bearer ***` |
| `Data Source=...;` | `Data Source=***` |
| Windows yolu kullanıcı profili | `C:\Users\***\...` |

> **UYARI:** Maskeleme **yazmadan önce** çalışır (pipeline'da ilk adım). Maskeleme sonrası veri tekrar çözülmez.

---

## §4 Review Checklist (incelemede işaretlenir)

- [ ] SEC-01: yeni dosyalarda secret kalıbı yok (`sk-`, `Password=`, `ApiKey=`)
- [ ] SEC-02: connection string diskte ham değil; koruyucu tek yerde
- [ ] SEC-03: sorgu birleştirme (`$"..."` + SQL) geçen satır = **0**
- [ ] SEC-04: yeni uç (UI/protokol/tool) sınır doğrulaması içeriyor
- [ ] SEC-05: `Process` çağrılarında `ArgumentList` + `UseShellExecute=false` + timeout var
- [ ] SEC-06: yeni tool izni varsayılanı `allow` değil
- [ ] SEC-07: sabit IV/nonce/ECB yok; anahtar repo dışında
- [ ] SEC-08: yeni log alanı kırmızıaltma hattından geçiyor
- [ ] SEC-09: sürüm CPM'de sabit; floating yok
- [ ] SEC-10: `BinaryFormatter`/tip metniyle deserializasyon = 0
- [ ] SEC-11: dosya yolu kanonik + kök kontrolü var
- [ ] SEC-12: `CommandTimeout=0` ve sonsuz `WaitForExit()` = 0
- [ ] SEC-13: kullanıcıya dönen hata stack/SQL içermiyor
- [ ] SEC-14: tool çıktısı talimat olarak kullanılmıyor, izin kapısı duruyor
- [ ] SEC-15: yıkıcı işlem onayı bağlı
- [ ] SEC-16: kritik olay audit'e yazılıyor (kırmızıaltılmış)

---

## §5 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | `.env`/config dosyasında ham API key | SEC-01 | key repo geçmişinde | iptal + store + geçmiş temizliği |
| 2 | `$"SELECT ... '{x}'"` | SEC-03 | injection açık, beklenmedik satır | parametreli sorgu |
| 3 | Loga tam exception + connection string | SEC-08 | log sızıntısı | `ISecretRedactor` hattı |
| 4 | `cmd.exe /c` + kullanıcı girdisi | SEC-05 | keyfi komut | `ArgumentList` + timeout |
| 5 | `read` tool'da `..` kontrolsüz kabul | SEC-11 | sistem dosyası okuma | kanonik yol + kök ön eki |
| 6 | `CommandTimeout = 0` | SEC-12 | askeri kilitlenme | `DbTimeouts.Query` |
| 7 | UI'da ham `SqlException` metni | SEC-13 | şema sızıntısı | kullanıcı mesajı + dahili trace |
| 8 | Tool çıktısındaki komutun uygulanması | SEC-14 | prompt injection | izin kapısı + görünür çağrı |
| 9 | CPM'siz `Version="*"` paket | SEC-09 | sürüm sürüklenmesi | `Directory.Packages.props` |
| 10 | Sessiz `DROP`/toplu silme | SEC-15 | veri kaybı | onay akışı (G7) |

---

## §6 Doğrulama

- [ ] 16 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] G2 (parameterized/EF Core) ile çelişen cümle yok
- [ ] Diyagramlar saf ASCII
- [ ] Bağlantılar yalnız mevcut dosyalara
- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G2/G7/G11) | `../CLAUDE.md` |
| 2 | Anahtar haritası (AES-256-GCM, credential vault, audit) | `../keys.md` |
| 3 | SQL/timeout/migration ayrıntısı | `data-access-rules.md` |
| 4 | Tool izin/sandbox | `../spec/tool-spec.md` |
| 5 | Kapsam analizi | `../reports/00-kapsam-analizi.md` |
| 6 | Test senaryoları | `testing-rules.md` |
