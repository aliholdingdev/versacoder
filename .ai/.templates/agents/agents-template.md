---
title: Agent Profil Şablonu
type: template
category: agents
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "ajan profil sablonu: rol, arac seti, yetki ve cikti sozlesmesi"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Agent Profil Şablonu — `.ai/.agents/<rol>.md`

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[governance/WORKFLOW]] · [[architecture/index]] · [[glossary]] · [[.rules/coding-standards]] · [[.rules/solid-rules]]

> Bu dosya **şablondur, içerik değil**. `## §1`..`## §7` başlıkları **silinemez, adlandırılamaz, sıralanamaz**.

---

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

Agent kataloğu 14 roldür: `master-orchestrator, architect, developer, reviewer, testing,
security, ui-designer, devops-engineer, docs-writer, researcher, debugger, refactorer,
database-optimizer, business-analyst`. Her rol `.ai/.agents/<rol>.md` dosyasında yaşar.

Serbest yazılmış profil üç hata üretir:

1. **Yetki belirsizliği** — scope yazılmazsa `developer` bir ADR'yi düzenleyebilir.
2. **Kayıp bağlam** — oturum kapanınca rolün bildiği unutulur; yeni oturum aynı keşfi 50.000 token'la tekrarlar.
3. **Denetimsizlik** — `⚠️ VERIFICATION REQUIRED` yoksa her cümle kanıtsız kabul edilir.

`§3` iskeleti bağlamı, `§3.5` yetkiyi, `§4.5` ihlal sonucunu, `§6.1` denetimi zorunlu kılar.

### §1.2 Disk Kanıtları ve Boşluk tablosu (YAGNI)

Her satır **iddiadır**. Kanıt üretilene kadar satır silinmez, `⚠️ VERIFICATION REQUIRED` ile bekletilir.
Doğrulanmayan yapı eklenmez (YAGNI).

| # | İddia | Beklenen disk kanıtı | Durum | Boşluk / Aksiyon |
|---|---|---|---|---|
| 1 | Vault indeksi var | `.ai/index.md` | ⚠️ VERIFICATION REQUIRED | Yoksa `GATE-000` içinde üretilir |
| 2 | Katalog 14 rol | `.ai/AGENTS.md` | ⚠️ VERIFICATION REQUIRED | Tutmuyorsa `ADR-007` açılır |
| 3 | Rol dosyaları mevcut | `.ai/.agents/*.md` | ⚠️ VERIFICATION REQUIRED | Eksik rol bu şablonla yazılır |
| 4 | Kural seti 7 dosya | `.ai/.rules/*.md` | ⚠️ VERIFICATION REQUIRED | 7'den azsa `.rules/` tamamlanır |
| 5 | ADR planı 8 karar | `.ai/adr/ADR-00*.md` | ⚠️ VERIFICATION REQUIRED | `ADR-001`..`008` dışına çıkılmaz |
| 6 | Alt klasörleşme | `.ai/.agents/<rol>/<alt>.md` | **KAPSAM DIŞI** | Üretilmez; rol başına tek dosya |

**Okuma kuralı:** işaretli satır kanıt sunulana kadar **geçersizdir**; `§6.1` işaretlenemez,
profil `active` alamaz.

### §1.3 Kullanım Türleri

| Kullanım türü | Ne zaman | Kullanılan bölüm | Yapan | Çıktı |
|---|---|---|---|---|
| Yeni rol profili | Kataloğa rol eklendiğinde | `§3.11` kalıbı | `docs-writer` + `architect` | `.ai/.agents/<rol>.md` |
| Profil revizyonu | Yetki/scope değişince | `§4.4` | Rol sahibi + `reviewer` | Sürüm + `log.md` satırı |
| Katalog mutabakatı | `ADR-007` tetiklenince | `§1.2` + `§7.2` | `master-orchestrator` | Eksik/fazla rol raporu |
| Devir teslim | Rol sahibi değişince | `§3.9.1` | Giden → alan rol | İmzalı devir bloğu |

### §1.4 Neden zorunlu

1. **Tek SSOT** — profil, o rolün tek geçerli tanımıdır.
2. **Yetki dosyada kalır** — `Allowed`/`Forbidden Scope` yoksa rol sonsuz yetki varsayar.
3. **Denetlenebilirlik** — `§4.5` ihlal sonucunu önceden yazar; pazarlık yoktur.
4. **Maliyet** — bağlamı dosyadan okuyan alt-ajan `§5.1` bütçesinde kalır.
5. **Süreklilik** — `§3.3..§3.9` 11 bölümü tek biçimde dayatır.

---

## §2 Kapsam

| Kapsam (bu şablon kapsar) | Kapsam Dışı (KAPMAZ) |
|---|---|
| `.ai/.agents/<rol>.md` profil yazımı | `.templates/adr/adr-template.md` (ADR formatı) |
| 14 rolün yetki, scope, devir, hata kuralları | `.ai/log.md`, `.ai/sessions/*.md` kayıt formatı |
| Rol → dosya tipi eşlemesi (`§2.1`) | Alt-ajan görev promptu dağıtımı |
| `Allowed` / `Forbidden` Scope ayrımı | `GATE-000`..`GATE-005` kapı metinleri |
| Guardrail G1..G12 ve ihlal sonucu | `ADR-001`..`ADR-008` karar içerikleri |

### §2.1 Dosya Tipi → Sorumlu eşlemesi

| Dosya tipi | Yazan | Onaylayan | Arşivleyen | Not |
|---|---|---|---|---|
| `.ai/.agents/<rol>.md` | `docs-writer` | `architect` | `master-orchestrator` | Yetki satırı için `security` onayı |
| `.ai/.agents/architect.md` | `architect` | `master-orchestrator` | `master-orchestrator` | Tek imza geçersiz |
| `.ai/.agents/developer.md` | `developer` | `reviewer` | `master-orchestrator` | Scope dışı kod yasağı (`§3.5`) |
| `.ai/.rules/*.md` | İlgili rol | `architect` | `docs-writer` | 7 dosya sabit |
| `.ai/log.md`, `.ai/sessions/*.md` | Her rol | `reviewer` | `master-orchestrator` | Oturum şablonu geçerli |
| `.ai/index.md`, `brain.md` | `master-orchestrator` | `architect` | — | Diğer roller **yazamaz** |

### §2.2 Kapsam Sınırı Testi

Bu şablon **zorunludur**; 5 şartın **hepsi** true olmalı:

1. Dosya yolu `.ai/.agents/` altındır ve uzantı `.md`'dir.
2. İlk satır YAML frontmatter `---` ile başlar.
3. Dosya bir **rolün** kimliğini, yetkisini ve iş akışını tanımlar.
4. Dosya bir **görev promptu** ya da **oturum kaydı** değildir.
5. Dosya `AGENTS.md` kataloğunun bir satırına karşılık gelir.

**Ret testi (herhangi biri true ise bu şablon kullanılmaz):**

| Soru | Cevap = EVET ise | Doğru şablon |
|---|---|---|
| Dosya bir ADR kararı mı? | ADR şablonu | `.templates/adr/adr-template.md` |
| Dosya parent→child görev çağrısı mı? | Alt-ajan şablonu | `.templates/agents/subagent-template.md` |
| Dosya `.ai/log.md` satırı mı? | Oturum kaydı | `.templates/session-log-template.md` |
| Dosya `.ai/.rules/` içinde mi? | Kural dosyası | Şablon yok; 7 dosya korunur |
| Dosya `.csproj` / `.cs` mi? | Kod dosyası | Şablon yok; `.rules/coding-standards` |

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

Profilin **ilk 8 satırı** alanları eksiksiz taşır; eksiklik `§6.1` madde 1'i kapatamaz.

```yaml
---
title: <Rol Adı> — VersaCoder Agent Profili
type: agent-profile
category: agents
status: draft | review | active | archived
version: <major.minor.patch>
updated: <YYYY-MM-DD>
authority: primary
---
```

Alan adları: `title, type, category, status, version, updated, authority`. Alan adı ya da
tipi bozuksa **G1**, `authority` değeri değişmişse **G2** ihlali sayılır.

### §3.2 H1 iskeleti

```markdown
# <Rol Adı> — VersaCoder Agent Profili

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[governance/WORKFLOW]] · [[architecture/index]]
· [[glossary]] · [[.rules/<ilgili-3-kural>]] |

> <Tek cümlelik rol özeti — 25 kelimeyi geçmez.>
```

### §3.3-§3.9 Zorunlu 11 bölümün markdown iskeleti

Tek çit içinde, sırayla. Başlıklar aynen korunur; satır sayısı bilinçli minimal tutulur.

```markdown
### §3.3.1 Agent Identity
| Alan | Değer |
|---|---|
| Rol adı (slug) | {{ROLE_SLUG}} |
| Katalog sırası | {{CATALOG_INDEX}} / 14 |
| Üst rapor | `master-orchestrator` |
| Alt-ajan disiplinleri | {{SUBAGENT_DISCIPLINES}} |

### §3.3.2 Mission
- **Bir cümle:** {{MISSION_ONE_SENTENCE}}
- **Başarı ölçütü:** {{SUCCESS_METRIC}} — ölçülmediyse görev başlamaz.

### §3.4 Responsibilities
| # | Sorumluluk | Girdi | Çıktı | Kanıt |
|---|---|---|---|---|
| R1..R5 | {{RESP_N}} | {{IN_N}} | {{OUT_N}} | {{EV_N}} |

### §3.5.1 Allowed Scope
| Alan | Yol | Koşul |
|---|---|---|
| {{ALLOWED_N}} | {{PATH_N}} | {{COND_N}} |
| Alt-ajan çağrısı | {{SUB_CALL}} | Alt-ajan şablonu ile |

### §3.5.2 Forbidden Scope
| # | Yasak | Kural kaynağı | İhlal sonucu |
|---|---|---|---|
| F1 | `EntityFramework` / `DbContext` | `ADR-004` | G5 — build durur |
| F2 | `.ai/index.md`, `brain.md`, `keys.md` | `§2.1` | G12 — geri alma |
| F3 | `Designer.cs` / `.resx` otomatik üretim | `ADR-005` | G8 — revert |
| F4 | TFM dışı hedefleme | `ADR-003` | G6 — PR reddi |
| F5 | {{FORBIDDEN_5}} | {{SRC_5}} | G12 |

### §3.6 Tech Stack
| Katman | Teknoloji | Sürüm / TFM | Not |
|---|---|---|---|
| UI | WinForms `RibbonForm` | DevExpress 25.2.8 | `RibbonControlStyle.Office2010` |
| Host / Cli | `VersaCoder.Desktop`, `VersaCoder.Cli` | `net48` + `net10.0-windows` | İkili hedef |
| Veri | ADO.NET + Dapper | `@p1..@pN` | Sqlite + SqlClient |
| Migration | SQL + `schema_version` | elle | **EF Core YASAK** |
| Test | xunit | `%90` coverage | `TreatWarningsAsErrors=true` |

### §3.7 Architecture Rules
1. `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`
   (+ `CrossCutting`, `Cli`). Ters bağımlılık **YASAK**.
2. `Domain` hiçbir projeye referans vermez.
3. `Designer.cs` + `.resx` + `ResourceManager` **elle** yönetilir.
4. Public API değişikliği ADR gerektirir (`ADR-001`..`ADR-008`).
5. {{ARCH_RULE_5}}

### §3.8 Workflow
| # | Adım | Girdi | Çıktı | Yapan |
|---|---|---|---|---|
| W1 | {{WF_N}} | {{WIN_N}} | {{WOUT_N}} | bu rol / `reviewer` / `testing` |

### §3.9.1 Handover
| Tetikleyici | Devredilen bağlam | Devir alan rol | Zorunlu çıktı |
|---|---|---|---|
| {{TRIGGER}} | `.ai/log.md` + `.ai/sessions/{{S}}.md` | {{SUCCESSOR}} | İmzalı devir bloğu (tarih, dosya, risk) |

### §3.9.2 Failure Handling
| Hata sınıfı | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| {{FAIL_1}} | {{SYM_1}} | Dur + kanıt topla | `master-orchestrator` |
| Build hatası | `TreatWarningsAsErrors` ihlali | Hata satırını aynen aktar | `reviewer` |
| Yetki ihlali | G12 tetiklendi | İşlemi geri al | `security` |
| Bütçe aşımı | > 50.000 token | Oturumu kapat | `master-orchestrator` |

### §3.9.3 Version
| Sürüm | Tarih | Değişiklik | Onay |
|---|---|---|---|
| 1.0.0 | {{DATE}} | İlk yayın | {{APPROVER}} |
| 1.1.0 | {{DATE}} | {{CHANGE_1}} | {{APPROVER}} |
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği — `.ai/.agents/developer.md`

```markdown
# Developer — VersaCoder Agent Profili

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[governance/WORKFLOW]] · [[architecture/index]] · [[.rules/coding-standards]]

> C# üretim kodunu yazar, derler, testini geçirir; tasarım ve ADR'ye dokunmaz.

### §3.3.1 Agent Identity
| Alan | Değer |
|---|---|
| Rol adı (slug) | `developer` |
| Katalog sırası | 3 / 14 |
| Üst rapor | `master-orchestrator` |
| Alt-ajan disiplinleri | `csharp-developer`, `explore`, `test-writer` |

### §3.3.2 Mission
- **Bir cümle:** Verilen ADR kararını katman sırasına uygun, `%90` coverage geçiren kod olarak teslim etmek.
- **Başarı ölçütü:** `dotnet build` 0 hata + xunit `%90` + `TreatWarningsAsErrors` temiz; üçü birlikte.

### §3.4 Responsibilities
| # | Sorumluluk | Girdi | Çıktı | Kanıt |
|---|---|---|---|---|
| R1 | Katman kodunu yazmak | `ADR-004`, `coding-standards` | `.cs` | Derleme çıktısı |
| R2 | Dapper sorgusu kurmak | Tablo şeması | `@p1..@pN` | Test çıktısı |
| R3 | `Designer.cs` + `.resx` elle güncellemek | Ekran 01..07 | Elle dosya | `git diff` |
| R4 | xunit testi yazmak | Metot imzası | `*Tests.cs` | Coverage raporu |
### §3.5.1 Allowed Scope
| Alan | Yol / Örnek | Koşul |
|---|---|---|
| Uygulama kodu | `src/VersaCoder.*/**/*.cs` | Katman sırası korunur |
| Test kodu | `tests/**/*.cs` (xunit) | `%90` hedefi |
| Elle tasarım | `**/*.Designer.cs`, `**/*.resx` | Otomasyon **yok** |
| SQL migration | `db/migrations/*.sql` | `schema_version` yazılır |
### §3.5.2 Forbidden Scope
| # | Yasak | Kural kaynağı | İhlal sonucu |
|---|---|---|---|
| F1 | `EntityFramework` / `DbContext` | `ADR-004` | G5 — build durur |
| F2 | `.ai/index.md`, `brain.md`, `keys.md` | `§2.1` | G12 — geri alma |
| F3 | `Designer.cs` / `.resx` otomatik üretim | `ADR-005` | G8 — revert |
| F4 | TFM dışı hedefleme | `ADR-003` | G6 — PR reddi |
| F5 | ADR veya `GATE-*` metni düzenleme | `§2` Kapsam Dışı | G12 |

### §3.6 Tech Stack
| Katman | Teknoloji | Sürüm / TFM | Not |
|---|---|---|---|
| UI | WinForms `RibbonForm` | DevExpress 25.2.8 | `RibbonControlStyle.Office2010` |
| Host / Cli | `VersaCoder.Desktop`, `VersaCoder.Cli` | `net48` + `net10.0-windows` | İkili hedef |
| Veri | ADO.NET + Dapper | `@p1..@pN` | Sqlite + SqlClient |
| Migration | SQL + `schema_version` | elle | **EF Core YASAK** |
| Test | xunit | `%90` coverage | `TreatWarningsAsErrors=true` |

### §3.7 Architecture Rules
1. `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`
   (+ `CrossCutting`, `Cli`). Ters bağımlılık **YASAK**.
2. `Domain` hiçbir projeye referans vermez.
3. `Designer.cs` + `.resx` + `ResourceManager` **elle** yönetilir.
4. Public API değişikliği ADR gerektirir.
5. String birleştirilmiş SQL **yok**; her sorgu `@p1..@pN` parametreli.

### §3.8 Workflow
| # | Adım | Girdi | Çıktı | Yapan |
|---|---|---|---|---|
| W1 | Gereksinim + ADR okuma | `REQUIREMENTS.md`, ADR | Uygulama notu | bu rol |
| W2 | Katman kodu yazımı | Uygulama notu | `.cs` + `.resx` | bu rol |
| W3 | Derleme | Kod | `dotnet build` | bu rol |
| W4 | Test + coverage | Testler | xunit raporu | `testing` |

### §3.9.1 Handover
| Tetikleyici | Devredilen bağlam | Devir alan rol | Zorunlu çıktı |
|---|---|---|---|
| `GATE-005` ya da oturum kapanışı | `.ai/log.md` + `.ai/sessions/*.md` | `architect`, `testing` | İmzalı devir: tarih, dosya, risk |

### §3.9.2 Failure Handling
| Hata sınıfı | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| Build hatası | CS kodu, 0 hata değil | Satırı aynen kopyala | `reviewer` |
| Coverage düşüğü | `< %90` | Eksik senaryoyu testle | `testing` |
| Yetki ihlali | G12 tetiklendi | İşlemi geri al | `security` |
| Belirsiz tasarım | 2 geçerli çözüm | Soru üret, **tahmin etme** | `architect` |

### §3.9.3 Version
| Sürüm | Tarih | Değişiklik | Onay |
|---|---|---|---|
| 1.0.0 | 2026-09-25 | İlk yayın | `architect` |
```

### §3.11 Kopyala-yapıştır tam kalıp (tümü `{{PLACEHOLDER}}`)

```markdown
---
title: {{DISPLAY_NAME}} — VersaCoder Agent Profili
type: agent-profile
category: agents
status: draft
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: primary
---

# {{DISPLAY_NAME}} — VersaCoder Agent Profili

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[governance/WORKFLOW]] · [[architecture/index]] · [[.rules/{{RULE_1}}]]

> {{ROLE_TAGLINE_25_WORDS_MAX}}

### §3.3.1 Agent Identity
| Alan | Değer |
|---|---|
| Rol adı (slug) | `{{ROLE_SLUG}}` |
| Katalog sırası | {{CATALOG_INDEX}} / 14 |
| Üst rapor | `master-orchestrator` |
| Alt-ajan disiplinleri | {{SUBAGENT_DISCIPLINES}} |

### §3.3.2 Mission
- **Bir cümle:** {{MISSION_ONE_SENTENCE}}
- **Başarı ölçütü:** {{SUCCESS_METRIC}}

### §3.4 Responsibilities
| # | Sorumluluk | Girdi | Çıktı | Kanıt |
|---|---|---|---|---|
| R1 | {{RESP_1}} | {{IN_1}} | {{OUT_1}} | {{EV_1}} |
| R2 | {{RESP_2}} | {{IN_2}} | {{OUT_2}} | {{EV_2}} |
| R3 | {{RESP_3}} | {{IN_3}} | {{OUT_3}} | {{EV_3}} |
| R4 | {{RESP_4}} | {{IN_4}} | {{OUT_4}} | {{EV_4}} |
| R5 | {{RESP_5}} | {{IN_5}} | {{OUT_5}} | {{EV_5}} |

### §3.5.1 Allowed Scope
| Alan | Yol | Koşul |
|---|---|---|
| {{ALLOWED_1}} | {{PATH_1}} | {{COND_1}} |
| {{ALLOWED_2}} | {{PATH_2}} | {{COND_2}} |
| Alt-ajan çağrısı | {{SUB_CALL}} | Alt-ajan şablonu ile |

### §3.5.2 Forbidden Scope
| # | Yasak | Kural kaynağı | İhlal sonucu |
|---|---|---|---|
| F1 | `EntityFramework` / `DbContext` | `ADR-004` | G5 — build durur |
| F2 | `.ai/index.md`, `brain.md`, `keys.md` | `§2.1` | G12 — geri alma |
| F3 | `Designer.cs` / `.resx` otomatik üretim | `ADR-005` | G8 — revert |
| F4 | TFM dışı hedefleme | `ADR-003` | G6 — PR reddi |
| F5 | {{FORBIDDEN_5}} | {{SRC_5}} | G12 |

### §3.6 Tech Stack
| Katman | Teknoloji | Sürüm / TFM | Not |
|---|---|---|---|
| UI | WinForms `RibbonForm` | DevExpress 25.2.8 | `RibbonControlStyle.Office2010` |
| Host / Cli | `VersaCoder.Desktop`, `VersaCoder.Cli` | `net48` + `net10.0-windows` | İkili hedef |
| Veri | ADO.NET + Dapper | `@p1..@pN` | Sqlite + SqlClient |
| Migration | SQL + `schema_version` | elle | **EF Core YASAK** |
| Test | xunit | `%90` coverage | `TreatWarningsAsErrors=true` |
| LLM | OpenAI-uyumlu + Ollama | `SmartModelRouter` | {{LLM_EXTRA}} |

### §3.7 Architecture Rules
1. `Abstractions → Domain → Application → Infrastructure → Protocol → Host → UI`
   (+ `CrossCutting`, `Cli`). Ters bağımlılık **YASAK**.
2. {{ARCH_RULE_2}}
3. `Designer.cs` + `.resx` + `ResourceManager` **elle** yönetilir.
4. Public API değişikliği ADR gerektirir (`ADR-001`..`ADR-008`).
5. {{ARCH_RULE_5}}

### §3.8 Workflow
| # | Adım | Girdi | Çıktı | Yapan |
|---|---|---|---|---|
| W1 | {{WF_1}} | {{WIN_1}} | {{WOUT_1}} | bu rol |
| W2 | {{WF_2}} | {{WIN_2}} | {{WOUT_2}} | `reviewer` |
| W3 | {{WF_3}} | {{WIN_3}} | {{WOUT_3}} | `testing` |
| W4 | {{WF_4}} | {{WIN_4}} | {{WOUT_4}} | `master-orchestrator` |

### §3.9.1 Handover
| Tetikleyici | Devredilen bağlam | Devir alan rol | Zorunlu çıktı |
|---|---|---|---|
| {{TRIGGER}} | `.ai/log.md` + `.ai/sessions/{{S}}.md` | {{SUCCESSOR}} | İmzalı devir: tarih, dosya, risk |

### §3.9.2 Failure Handling
| Hata sınıfı | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| {{FAIL_1}} | {{SYM_1}} | Dur + kanıt topla | `master-orchestrator` |
| Build hatası | `TreatWarningsAsErrors` ihlali | Hata satırını aynen aktar | `reviewer` |
| Yetki ihlali | G12 tetiklendi | İşlemi geri al | `security` |
| Bütçe aşımı | > 50.000 token | Oturumu kapat | `master-orchestrator` |

### §3.9.3 Version
| Sürüm | Tarih | Değişiklik | Onay |
|---|---|---|---|
| 1.0.0 | {{YYYY-MM-DD}} | İlk yayın | {{APPROVER}} |
| 1.1.0 | {{YYYY-MM-DD}} | {{CHANGE_1}} | {{APPROVER}} |
```

---

## §4 Kurallar

### §4.1 Durum / Aşama döngüsü

`status` yalnızca dört değer alır; her geçiş `§4.4` sürüm artışı + `.ai/log.md` satırı ister.

```
 draft --inceleme--► review --onay--► active (SSOT) --30 gün--► archived
   ▲                    |                    |                       |
   +---- bulgu geri ----+  scope değişti ----+   active'e dönüş yalnız §4.4 ile (major)
 YASAK: active ► draft (sessiz düşüş) · YASAK: draft ► active (doğrudan yayın)
```

| Durum | Anlamı | Kim koyar | Kim kaldırır | `§6.1` zorunlu mu |
|---|---|---|---|---|
| `draft` | Yazıldı, okunmadı | Profil sahibi | — | Hayır |
| `review` | `reviewer` + `quality-check` aşaması | Profil sahibi | `reviewer` (bulgu ile) | Kısmen |
| `active` | Tek geçerli SSOT tanımı | `architect` | `master-orchestrator` | **Evet, 16/16** |
| `archived` | Okunur, düzenlenmez | `master-orchestrator` | Yalnız `§4.4` | Evet (arşiv notu) |

### §4.2 Onay / Rol akışı

| # | Adım | Yapan rol | Onay bekleyen | Redde yetkili | Zorunlu kanıt |
|---|---|---|---|---|---|
| A1 | Profil taslağı | Profil sahibi | `reviewer` | — | `§3.11` kalıbı eksiksiz |
| A2 | Yetki taraması | `security` | `architect` | `architect` | `§3.5` tabloları dolu |
| A3 | Mimari uyum | `architect` | `master-orchestrator` | `master-orchestrator` | Katman ihlali yok |
| A4 | Katalog mutabakatı | `master-orchestrator` | — | `architect` | 14 rol = 14 dosya |
| A5 | Kalite kontrolü | `quality-check` | `reviewer` | `reviewer` | Quality Report (`§6.4`) |
| A6 | Yayına alma | `architect` | `master-orchestrator` | — | `status: active` + `log.md` |

### §4.3 Öncelik / Ölçek tablosu

| Öncelik | Ad | Ne zaman | Onay süresi | Bütçe etkisi |
|---|---|---|---|---|
| **P0** | Kritik | `§4.5` ihlali, secret sızıntısı, build kırık | Anında — **Otomatik DUR** | Bütçe askıya alınır |
| **P1** | Yüksek | Yetki/scope değişikliği, `active` geçişi | ≤ 10 dk | +5.000 token rezerv |
| **P2** | Normal | Yeni rol profili, Responsibility revizyonu | 1 iş günü | 50.000 içinde |
| **S/M/L** | 1-10 / 10-60 / 60+ satır | patch / minor / scope değişikliği | Sahibi / `reviewer` / `architect`+`master-orchestrator` | 1.000 / 5.000 / 15.000 token |

### §4.4 Değişiklik kontrolü

1. Her düzenleme `version` artırır: düzeltme → **patch**, yeni satır/bölüm → **minor**,
   `Allowed`/`Forbidden` Scope değişikliği → **major**.
2. `updated` her düzenlemede `YYYY-MM-DD` olarak yenilenir.
3. `authority: primary` **hiçbir** sürümde değiştirilemez (G2).
4. `active` profilin `major` değişikliği önce `review`'a düşer; sessiz `active → active` **yasaktır**.
5. Her değişiklik `.ai/log.md`'ye tek satır yazar: `tarih | rol | dosya | sürüm | neden`.
6. Diff alınmadan commit **yok**; kanıt `git diff --stat` çıktısıdır.
7. `archived` yalnız madde 1 kapsamında yeniden `active` olabilir.

### §4.5 Hard Guardrails tablosu

| ID | Guardrail | Kural kaynağı | Tespit yöntemi | İhlal Sonucu |
|---|---|---|---|---|
| **G1** | Frontmatter 7 alan eksiksiz | `§3.1` | `Select-String '^authority:'` | Dosya reddedilir; `active` olamaz |
| **G2** | `authority: primary` değişmez | Vault kuralı | Frontmatter diff | Değişiklik geri alınır + `log.md` |
| **G3** | `## §1`..`## §7` silinemez | Bu şablon | Başlık sayısı = 7 | Dosya revert; iş `draft`'a döner |
| **G4** | Forbidden Scope kural dışı yazılamaz | `§3.5` | `security` taraması | İşlem durur, `security` bilinir |
| **G5** | EF Core / `DbContext` yok | `ADR-004` | `Select-String 'DbContext'` | **Build durur**; PR reddi |
| **G6** | Yalnız `net48` + `net10.0-windows` | `ADR-003` | `.csproj` + `§3.6` | PR reddi; hedef çevrilir |
| **G7** | DevExpress `25.2.8` | `ADR-001` / `ADR-008` | Paket listesi + profil | Sürüm sabitlenir |
| **G8** | `Designer.cs`/`.resx` yalnız elle | `ADR-005` | Dosya imzası/geçmişi | Dosyalar revert, iş durur |
| **G9** | Kanıtsız iddia yazılamaz | `§1.2` | İşaret yoksa | İddia **silinir**, satır işaretlenir |
| **G10** | Token ≤ 50.000 / oturum | Vault bütçesi | Oturum sayacı | Oturum kapanır; handover zorunlu |
| **G11** | Mojibake yasak | Dil kuralı | `\xc3\xbc`, `â„¢`, `ðŸ` taraması | Dosya geri alınır, yeniden yazılır |
| **G12** | Allowed Scope dışı her işlem | `§3.5` | Rol denetim kaydı | İşlem **geri alınır** + rapor |

---

## §5 Workflow

### §5.1 Adım tablosu ve zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Talep ayrıştırma | İstek metni | 3-5 madde kapsam | `business-analyst` | 5 dk |
| 2 | Katalog mutabakatı | `AGENTS.md` | Eksik/fazla rol listesi | `master-orchestrator` | 5 dk |
| 3 | Şablon seçimi | Dosya yolu | Doğru `.templates/*` | `docs-writer` | 2 dk |
| 4 | Frontmatter + H1 | `§3.1`, `§3.2` | İlk 15 satır | `docs-writer` | 5 dk |
| 5 | `§3.3..§3.9` doldurma | Rol tanımı, ADR'ler | 11 bölüm | Profil sahibi | 30 dk |
| 6 | Scope taraması | `§3.5` | Allowed/Forbidden tablosu | `security` | 10 dk |
| 7 | Mimari uyum + guardrail | `architecture/index.md`, `§4.5` | G1..G12 sonuçları | `architect` + `quality-check` | 20 dk |
| 8 | `§6.1` onay kutuları | Profil + kanıtlar | 16/16 işaretli | `reviewer` | 10 dk |
| 9 | Quality Report + yayın | `§6.4` kalıbı | `status: active` | `architect` | 10 dk |

**Toplam zaman bütçesi: 97 dakika.** Adım 5 bütçesi aşılırsa görev `GATE`'e taşınmaz, parçalanır.
Adım 8'de 1'den fazla açık madde varsa profil `review`'da kalır (`§6.2`).

**Token bütçesi (toplam 50.000 / oturum):** adım 1-3 → %10 (5.000) · adım 4-5 → %45 (22.500) ·
adım 6-8 → %30 (15.000) · adım 9 + oturum kaydı → %15 (7.500).

### §5.2 Hata Modları tablosu

| # | Hata modu | Belirti | Olası neden | İlk müdahale | Kurtarma |
|---|---|---|---|---|---|
| E1 | Frontmatter eksik | `§6.1` madde 1 kapanmıyor | Kalıp kopyalanmadı | `§3.1` kalıbını yapıştır | Dosya `draft`'a alınır |
| E2 | `## §1..§7` eksik | Başlık sayısı < 7 | Bölüm silindi | `§3.3..§3.9` + bu şablon | Revert + `review` |
| E3 | Yetki aşımı | G12 tetiklendi | Forbidden ihlali | İşlemi geri al, `security`'ye bildir | `architect` onayıyla kapanır |
| E4 | EF Core sızıntısı | G5, build durdu | Eski refaktör kalıntısı | `DbContext` referansını kaldır | Build yeşil olana dek |
| E5 | Katalog tutmazlığı | 14 rol ≠ dosya | Yeni rol eklenmiş | `ADR-007` mutabakatı | Profil eklenir/rol çıkarılır |
| E6 | Mojibake | `\xc3\xbc`, `â„¢` görünüyor | Yanlış kodlama | UTF-8 ile yeniden kaydet | G11: dosya yeniden yazılır |
| E7 | Token taşması | `> 50.000` (G10) | Bağlam şişmesi | Oturumu kapat, handover yaz | Yeni oturumda adım 1 |

### §5.3 Salt-okunur PowerShell komut bloğu

6 komut **yalnızca okur**; dosya değiştirmez. Çıktısı `§6.1` kutusunun kanıtıdır.

```powershell
# 1 — Profil dosyası var olmalı ve 15-32 KB aralığında olmalı
Get-Item 'C:\versacoder\.ai\.agents\developer.md' | Select-Object Name, Length, LastWriteTime

# 2 — Frontmatter 1. satırda başlamalı, 7 alan içermeli
Get-Content 'C:\versacoder\.ai\.agents\developer.md' -TotalCount 8

# 3 — Zorunlu 7 başlık eksiksiz ve sırayla olmalı (7 sonuç)
Select-String -Path 'C:\versacoder\.ai\.agents\developer.md' -Pattern '^## §[1-7] '

# 4 — Dosya 480-660 satır ve 15-32 KB olmalı (değilse G1 ihlali)
(Get-Item 'C:\versacoder\.ai\.agents\developer.md').Length / 1KB

# 5 — EF Core referansı olmamalı — 0 sonuç olmalı (G5)
Select-String -Path 'C:\versacoder\.ai\.agents\developer.md' -Pattern 'EntityFramework|DbContext'

# 6 — Mojibake olmamalı — 0 sonuç olmalı (G11)
Select-String -Path 'C:\versacoder\.ai\.agents\*.md' -Pattern '\xc3\xbc|\xc3\xb6|â„¢|\xe2\u20ac™|ðŸ|ï¿½'
```

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi

Profil yalnızca **16/16** işaretlendiğinde `status: active` alır.

- [ ] 1. Frontmatter 7 alan eksiksiz: `title, type, category, status, version, updated, authority`
- [ ] 2. `authority: primary` değiştirilmemiş (G2)
- [ ] 3. `type: agent-profile` ve `category: agents` sabit
- [ ] 4. H1 başlığı dosya adındaki slug'ı (`<rol>.md`) birebir içeriyor
- [ ] 5. `**Zorunlu Bağlantılar:**` satırı var ve `.ai` içine göreli wiki-link kullanıyor
- [ ] 6. `## §1 Amaç` başlığı tam metinle duruyor (G3)
- [ ] 7. `## §2 Kapsam` içinde Kapsam ve Kapsam Dışı tabloları ikisi de var
- [ ] 8. `§2.1` Dosya Tipi → Sorumlu eşlemesi en az 6 satır
- [ ] 9. `§2.2` Kapsam Sınırı Testi: 5 şart + 5 ret sorusu dolu
- [ ] 10. `§3.3..§3.9` arasında 11 zorunlu bölümün iskeleti mevcut
- [ ] 11. `§3.10` gerçek VersaCoder örneği tam ve somut (placeholder yok)
- [ ] 12. `§3.11` kalıbındaki tüm alanlar `{{PLACEHOLDER}}`
- [ ] 13. `§4.5` tablosunda **en az 12** satır ve `İhlal Sonucu` sütunu var
- [ ] 14. `§5.1` adımları **en az 9** satır + zaman bütçesi yazılı
- [ ] 15. `§5.2` hata modları **en az 7** satır
- [ ] 16. `§5.3` içinde salt-okunur **6** PowerShell komutu, her biri "— N olmalı" yorumlu

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| # | Red sinyali | Tetikleyen koşul | Sonuç |
|---|---|---|---|
| R1 | Kanıtsız iddia | İşaret yok + kanıt yok | **Otomatik DUR** — yayınlanmaz |
| R2 | Bölüm silinmiş | `## §` başlık sayısı ≠ 7 | **Otomatik DUR** — revert |
| R3 | Yetki belirsiz | `Forbidden Scope` tablosu boş | **Otomatik DUR** — `security` onayı |
| R4 | Yasaklı teknoloji | G5 ihlali (EF Core) | **Otomatik DUR** — build kapalı |
| R5 | Onay eksik | `§4.2` A2/A3/A6'dan biri atlandı | **Otomatik DUR** — `draft`'a döner |
| R6 | Katalog tutmazlığı | 14 rol ≠ profil dosyası sayısı | **Otomatik DUR** — `ADR-007` açılır |
| R7 | Bütçe taşması | Oturum `> 50.000` token | **Otomatik DUR** — handover zorunlu |

### §6.3 Sürüm / Aşama geçiş kuralları

| Geçiş | Koşul | Sürüm etkisi | Onay | Zorunlu kanıt |
|---|---|---|---|---|
| `draft → review` | `§3.11` eksiksiz | patch | Profil sahibi | `§5.3` komut 1-3 |
| `review → active` | 16/16 onay kutusu | minor | `architect` | Quality Report (`§6.4`) |
| `active → active` (içerik) | `§4.4` maddeleri | patch | `reviewer` | `git diff --stat` |
| `active → review` | `major` scope değişikliği | major | `architect` + `master-orchestrator` | Yeni `§3.5` tabloları |
| `active → archived` | 30 gün dokunulmadı | patch | `master-orchestrator` | Arşiv notu + `log.md` |
| `draft → active` / `active → draft` | — | — | — | **YASAK** — G12 / G3 ihlali |

### §6.4 Quality Report markdown bloğu

`quality-check` bu kalıbı **aynen** doldurur ve `.ai/log.md`'ye iliştirir.

````markdown
### Quality Report — {{ROLE_SLUG}} v{{VERSION}}
- **Tarih:** {{YYYY-MM-DD}} · **Kontrol eden:** `quality-check` + `reviewer` · **Sonuç:** PASS | FAIL

| Kontrol | Beklenen | Gerçek | Durum |
|---|---|---|---|
| Boyut | 480-660 satır / 15-32 KB | {{LINES}} / {{KB}} | OK/FAIL |
| Bölüm + frontmatter | 7 başlık / 7 alan | {{SECTIONS}} / {{FM}} | OK/FAIL |
| Guardrail satırı | ≥ 12 (G1..G12) | {{G}} | OK/FAIL |
| Adım / hata modu / komut | ≥ 9 / ≥ 7 / 6 | {{STEPS}} / {{ERR}} / {{PS}} | OK/FAIL |
| Onay kutusu + Red Team | ≥ 16 / ≥ 10 | {{CHECKS}} / {{RED}} | OK/FAIL |
| Mojibake / EF Core | 0 / 0 | {{MOJI}} / {{EF}} | OK/FAIL |

**Açık bulgular:** {{FINDINGS_OR_NONE}} · **Onay:** {{APPROVER}} · {{YYYY-MM-DD}}
````

### §6.5 Şablona özgü ek kontroller

1. **Kanıt bütünlüğü** — `§1.2`'deki her işaretli satır için `§5.3` komut 1-4 çıktısı dosyaya iliştirilmiş mi? Değilse `R1` çalışır.
2. **Placeholder sızıntısı** — `§3.10` içinde `{{` geçmemeli; `§3.11` içinde yalnız `{{PLACEHOLDER}}` bulunmalı.
3. **Yerel göreli bağlantı** — `Zorunlu Bağlantılar` satırındaki her wiki-link `.ai/` köküne göreli; mutlak yol (`C:\...`) **yasak**.
4. **Türkçe bütünlüğü** — `ç ğ ı İ ö ş ü` doğru; `Ã`/`â` artıkları olmamalı (G11).
5. **İddia-kanıt eşleşmesi** — `§1.4`, `§4.5` G1..G12 ve `§5.2` E1..E7 çelişmemeli; çelişki varsa rapor `FAIL` verir.

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Kaynak | Yol (`.ai/` içine göreli) | Bu şablonla ilişkisi |
|---|---|---|
| Vault indeksi | `index.md` | Rol dosyalarına giriş kapısı |
| Agent kataloğu | `AGENTS.md` | 14 rol = 14 profil (mutabakat zorunlu) |
| İş akışı | `WORKFLOW.md` | `§5.1` adımlarının üst tanımı |
| Mimari | `architecture/index.md` | `§3.7` katman kurallarının kaynağı |
| Oturum kaydı | `log.md` | `§4.4` madde 5 zorunlu satırı |
| Kurallar | `.rules/coding-standards`, `.rules/solid-rules`, `.rules/security-rules` | `§3.6`, `§3.7`, `§3.5` denetimi |
| ADR şablonu | `.templates/adr/adr-template.md` | Yapı referansı (içerik kopyalanmaz) |
| Alt-ajan şablonu | `.templates/agents/subagent-template.md` | `§5.1` adım 5 parçalama |
| Oturum şablonu | `.templates/session-log-template.md` | `§4.4` madde 5 kayıt formatı |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Belirti | Neden yanlış | Doğrusu | Yakalayan kontrol |
|---|---|---|---|---|---|
| 1 | ADR şablonunun içeriğini kopyalamak | Aynı cümleler iki dosyada | Yapı ≠ içerik | Yalnız `§` iskeleti; metin sıfırdan | `§6.5` madde 2 |
| 2 | `authority: draft` yazmak | Frontmatter bozuk | Anlam karışıklığı | `authority` sabit; durum `status`'tedir | G2 |
| 3 | `## §` başlıklarını adlandırmak | `## 1. Amaç` | "Düzeltme" refleksi | Başlıklar **silinemez** | G3 |
| 4 | Kanıtsız iddia | "Build yeşil" log yok | Kolaycılık | İşaret + `§5.3` çıktısı | G9, R1 |
| 5 | `Forbidden Scope` boş bırakmak | Tablo yok | "Sonra eklerim" | En az F1-F5 dolu | R3 |
| 6 | EF Core referansı | `DbContext` geçiyor | Eski alışkanlık | ADO.NET + Dapper + `@p1..@pN` | G5 |
| 7 | Tek TFM hedeflemek | Yalnız `net10.0-windows` | Modernite refleksi | `net48` + `net10.0-windows` | G6 |
| 8 | `Designer.cs`'i otomatik üretmek | İmza değişti | DevExpress tasarımcısı | Elle düzenle; `ADR-005` | G8 |
| 9 | 13 dosyayla teslim | Katalog tutmaz | Rol unutulmuş | `ADR-007`: 14 = 14 | R6 |
| 10 | Token sayacını yazmamak | Oturum kaydı eksik | Hafife alma | `(n/8)` + toplam token her turda | G10 |

> **Şablonun kendi doğrulaması:** her güncellemeden sonra `§5.3` komutları çalıştırılır,
> `§6.4` Quality Report üretilir, `§6.1` 16/16 kapanmadan dosya `active` olmaz.
