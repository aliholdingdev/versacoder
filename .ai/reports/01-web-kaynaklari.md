---
title: "VersaCoder — Web Kaynak Raporu (WEB GATE)"
type: report
category: research
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Zorunlu web araştırması: her senior karar ≥1 birincil kaynak URL + erişim tarihi, çelişki/NEEDS_VALIDATION kaydı"
---

# VersaCoder — Web Kaynak Raporu (WEB GATE)

**Zorunlu bağlantılar:** [[00-kapsam-analizi.md]] · [[../index.md]] · [[../WORKFLOW.md]] · [[../CLAUDE.md]] · [[../keys.md]]

---

## 0. Özet (Answer First)

**Doğrulandı: `net48` + `net10.0-windows` ikili bacak DevExpress v26.1 ile resmen destekleniyor.** WinForms prerequisites tablosu v24.2–v26.1 için `.NET Framework 4.8 = EVET`, v25.2–v26.1 için `.NET 10 = EVET`. Tek gerçek risk: **Microsoft.Data.SqlClient 6.0 .NET Standard desteğini düşürdü** → net48 bacağında 5.x satırına sabitlenmeli (ADR konusu). Doğrulanamayan 1 madde: `Microsoft.Data.Sqlite` .NET 10 + net48 ikili desteği (yalnız forum sinyali var → `NEEDS_VALIDATION`).

---

## 1. WEB GATE kuralı

| # | Kural | Durum |
|---|---|---|
| 1 | Her senior karar (§2) + teknoloji yığını (§3) için **≥1 birincil kaynak URL + erişim tarihi** | ✅ bu dosyada |
| 2 | Kaynaksız karar = geçersiz; uydurma API/ürün = `G9` ihlali | ✅ |
| 3 | Çelişkide **kaynak kazanır**, prompt düzeltmesi kullanıcı onayıyla yapılır | ✅ §3 |
| 4 | PART 6/7'de emin olunmayan API'de web araştırması tekrar zorunlu | ⏳ kodlama part'i |
| 5 | "Bitti" iddiası = kaynak + build/test kanıtı birlikte | ⏳ PART 8 |

**Erişim tarihi (tüm satırlar): 2026-09-25.**

---

## 2. Karar → Kanıt tablosu

| Karar | Web bulgusu | Kaynak (URL) | Etki | Durum |
|---|---|---|---|---|
| DevExpress **v26.1** + WinForms | v24.2–v26.1: `.NET Framework 4.6.2+` → **4.8 destekli**; v25.2–v26.1: `.NET 8/9/10` destekli | `docs.devexpress.com/WindowsForms/8092/prerequisites` | **İki bacak da uyumlu** — karar 1 geçerli | ✅ VERIFIED |
| DevExpress sürüm ömrü | v25.2–v26.1.5 = *Supported*, minor + security update açık | `devexpress.com/support/versions.xml` | v26.1 kilidi doğru | ✅ VERIFIED |
| v26.2 riski | v26.2'de minimum desteklenen .NET sürümü **.NET 10** olacak; min IDE VS2022; .NET Framework 4.6.2 kalıyor | `community.devexpress.com/Blogs/winforms/archive/2026/08/20/...roadmap-v26-2.aspx` | v26.2'ye geçişte net48 etkilenmez, modern bacak .NET 10'da kalır | ✅ VERIFIED |
| **.NET 10 LTS** | 11.11.2025 çıkışı, destek **14.11.2028**, son yama 10.0.12 (08.09.2026), **C# 14** | `learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview` · `dotnet.microsoft.com/platform/support/policy/dotnet-core` | Modern bacak dili: C# 14 (LangVersion default) | ✅ VERIFIED |
| **Dapper** (EF Core yok) | Dapper **2.1.89** — ADO.NET üzerinde micro-ORM, SQL Server/SQLite/MySQL | `nuget.org/packages/dapper` | Tek sorgu katmanı = Dapper + parameterized | ✅ VERIFIED |
| `Microsoft.Data.SqlClient` | **6.0 .NET Standard desteğini düşürdü** (discussion #3115) | `github.com/dotnet/SqlClient/discussions/3115` | **KARAR (25.09): her iki bacakta 5.x** → CPM tek sürüm, TFM koşulu yok → **ADR-012** | ✅ KAPANDI |
| `Microsoft.Data.Sqlite` | Yalnız forum sinyali: ".NET 10 ve gelecek sürümlerde netframework desteği" tartışması | `reddit.com/r/dotnet/comments/1rq4vx4` (ikincil) | net48'de Sqlite desteği **teyit edilmedi** | ❓ NEEDS_VALIDATION |
| DevExpress v26.1 **AI özellikleri** | WinForms **AI Chat** kontrolü + **AI Agent Skills** + Monaco tabanlı editör (demo) | `devexpress.com/subscriptions/whats-new/` | `05-ai-chat` ekranı için aday kontrol → PART 4'te `ui-designer` değerlendirir | ✅ VERIFIED |
| opencode (yerel envanter) | `%TEMP%\opencode-fresh` = **v1.18.32**, 33 paket; `core/src/tool` = tool.ts/tools.ts, `filesystem`, `ripgrep`, `pty`, `session`, `skill`, `plugin`, `protocol`, `llm` | yerel clone (git) | PART 7 tool sözleşmesi girdisi | ✅ YEREL KANIT |

---

## 3. Çelişki kayıtları (prompt ↔ web)

| # | Çelişki | Karar | Onay |
|---|---|---|---|
| C1 | Prompt §4 PART 5 `.slnx` vs ai.01 `.sln` | `.slnx` (prompt = SSOT) | kapalı |
| C2 | ai.01/ai.02 "C# .NET 8 / C# 12" vs prompt "net48 + net10.0-windows" | prompt kazanır; web: her ikisi de v26.1 ile destekli | kapalı |
| C3 | "50+ katman / 5000+ layer" (ai.01) vs 7 katmanlı Clean mimari | **7 makro × 45 alt katman = 52 düğüm, ≥5000 bileşen kaydı** (kullanıcı kararı 25.09.2026) | kapalı |
| C4 | Gate sayısı: 9 (ai.03) vs 6 (ai.01/ai.02) | **6 gate = GATE-001..006** — `WORKFLOW.md` (vault SSOT) | kapalı |
| C5 | `Microsoft.Data.SqlClient` 6.0 net48'i desteklemiyor | **KAPANDI (kullanıcı 25.09):** iki bacakta da **5.x** — CPM'te tek sürüm (`Directory.Packages.props`), ADR-012 | ✅ kapalı |
| C6 | `Microsoft.Data.Sqlite` net48 + net10 ikili | `NEEDS_VALIDATION` — PART 5'te NuGet sayfasından teyit edilecek | AÇIK |

---

## 4. Kaynak listesi

| # | Kaynak | Erişim |
|---|---|---|
| 1 | DevExpress WinForms System Requirements (v26.1) — `docs.devexpress.com/WindowsForms/8092/prerequisites` | 2026-09-25 |
| 2 | DevExpress Supported Versions — `devexpress.com/support/versions.xml` | 2026-09-25 |
| 3 | DevExpress WinForms Year-End Roadmap v26.2 — `community.devexpress.com/.../2026/08/20/...` | 2026-09-25 |
| 4 | What's new in .NET 10 — `learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview` | 2026-09-25 |
| 5 | .NET support policy — `dotnet.microsoft.com/platform/support/policy/dotnet-core` | 2026-09-25 |
| 6 | Dapper 2.1.89 — `nuget.org/packages/dapper` | 2026-09-25 |
| 7 | SqlClient #3115 (.NET Standard dropped in 6.0) — `github.com/dotnet/SqlClient/discussions/3115` | 2026-09-25 |
| 8 | DevExpress What's New v26.1 (AI Chat / AI Agent Skills) — `devexpress.com/subscriptions/whats-new/` | 2026-09-25 |

---

## 5. Doğrulama

- [x] Frontmatter 8 alan, `authority: SSOT`
- [x] Her satırda URL + erişim tarihi
- [x] `VERIFIED` / `NEEDS_VALIDATION` / `AÇIK` etiketleri (G9)
- [x] Çelişki listesi 6 madde, her maddeyle karar
- [ ] C5 → `architect` ADR-004 açacak (PART 3)
- [ ] C6 → PART 5'te NuGet teyidi
