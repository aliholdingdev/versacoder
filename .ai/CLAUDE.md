---
title: "VersaCoder — AI Anayasası (Constitution)"
type: constitution
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "Vault genelinde bağlayıcı 12 hard guardrail (G1..G12), yasak listesi, kanıt formatı, dur/bekle kuralları ve mimari sınır yasakları"
tags: [claude]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — AI Anayasası (`CLAUDE.md`)

**Zorunlu Bağlantılar:** [index](index.md) · [WORKFLOW](governance/workflow.md) · [ROLE](definitions/role.md) · [AGENTS](definitions/agents.md) · [ADR-028](.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [architecture/index](architecture/index.md) · [layer-boundary-check](checklists/layer-boundary-check.md) · [MEMORY](memory/memory.md) · [log](ops/log.md) · [keys](governance/keys.md)

> **Tek cümlelik sorumluluk:** *Hangi davranış kesinlikle yasak, bilgi yoksa ne yazılır,
> kanıt nasıl sunulur, ne zaman durulur* sorusunun tek cevabıdır.
> Bu dosya **bağlayıcıdır**; WORKFLOW.md, ROLE.md ve AGENTS.md ile çelişirse bu dosya kazanır.

---

## §0 Özet (Answer First)

```text
╔═══════════════════════════════════════════════════════════════╗
║  12 GUARDRAIL (G1..G12)  ·  İHLAL = İŞ RED                    ║
║  Rapor biçimi:  konum + neden + düzeltme                      ║
║  KANIT = çalıştırılmış komutun çıktısı (çıktısız iddia uydurma)║
║  GATE-003 kapalıyken src/** altına tek satır .cs bile yazılmaz ║
╚═══════════════════════════════════════════════════════════════╝
```

- **12 guardrail ihlal edilemez.** En çok tekrarlanan 7 yasak: EF Core ·
  string-concat SQL · DevExpress `v26.1` dışı sürüm / `Office2010` dışı ribbon ·
  kanıtsız "bitti" · uydurma bilgi · yetki dışı kod yazımı.
- **Mimari bağ ADR-028 ile kilitli:** 8 makro katman (l0..l7) · 52 düğüm ·
  9 üretim + 1-2 test projesi · çift TFM `net48;net10.0-windows`.
- **Dur/Bekle çizgisi nettir:** onay yalnız kullanıcıdan gelir.

---

## §1 Hard Guardrails (G1..G12)

> Her madde `kural / yasak / ihlalde / kanıt` dörtlüsündedir.

| G | Kural | Yasak | İhlalde | Kanıt |
|---|---|---|---|---|
| **G1 Sıfırdan yazım** | Eski vault yalnız **bilgi kaynağıdır**; içerik sıfırdan yazılır | `Copy-Item` ile `.ai` kopyalamak · blok yapıştırma | RED — dosya silinir, yeniden yazılır | `git status --porcelain` + cümle benzerliği |
| **G2 EF Core + string-concat SQL** | Veri erişimi **ADO.NET + Dapper**; her sorgu **parametreli**; migration = ham SQL + `schema_version` | `EntityFrameworkCore` · `DbContext` · LINQ-to-Entities · `$"SELECT …{x}"` · `+` ile birleştirme | CRITICAL — build RED | EF taraması = 0 satır; concat taraması = 0 satır |
| **G3 DevExpress sürüm + ribbon** | DevExpress **v26.1** tek sürüm; ribbon yalnız **`RibbonControlStyle.Office2010`**; ana pencere `RibbonForm` | `24.2` · `23.x` · `25.x` · Office2007 / Office2010Silver / Office2010Blue · `XtraForm` | CRITICAL — RED | `Directory.Packages.props` tek DevExpress satırı |
| **G4 Vault biçimi, UTF-8, ASCII** | Her MD **8 alanlı YAML frontmatter**, UTF-8, 0 bozuk link, 0 mojibake | frontmatter'sız dosya · bozuk link · CP1254 · `WIP`/`~`/`TBD` | RED — biçim düzeltilir | Frontmatter taraması = 0 eksik · link = 0 bozuk |
| **G5 Kanıtsız "bitti"** | "Bitti/tamam" **yalnız** build + test + tarama çıktısıyla söylenir | "Muhtemelen derlenir" · "testler geçer" (çıktı yok) | RED — iş bitmedi sayılır | Konsol çıktısı: proje + hata sayacı + süre |
| **G6 Tek bacaklı tasarım** | Her karar **iki bacakta**: `net48` + `net10.0-windows`; `#if` **yalnız** platform/API farkında | "net48'yi sonra bakarız" · platform kodunu çoğaltmak | HIGH — mimari RED | İki ayrı derleme: `-f net48` ve `-f net10.0-windows`, 0 hata |
| **G7 Yıkıcı işlem onayı** | Silme / overwrite / `reset --hard` / `push --force` **öncesi** kullanıcı onayı | Sessiz `Remove-Item -Recurse` · sormadan overwrite | CRITICAL — durdurulur | `log.md` tarihli onay satırı |
| **G8 Sürüm / teknoloji sapması** | Yığın: C# · WinForms · DevExpress v26.1 · Dapper · xUnit + NSubstitute + NetArchTest · CPM | Alternatif ORM · JS framework · ribbon dışı stil · "geçici" paket | RED → `DECISION BLOCKED` | `Directory.Packages.props` okuma raporu |
| **G9 Uydurma (zero-hallucination)** | Her teknik iddianın etiketi ve kaynağı vardır | Var olmayan dosya/class/API/NuGet/sürüm iddiası | RED — madde silinir | Etiket + kaynak satırı (§3) |
| **G10 Çelişki işleme** | Kaynak çelişirse `CONFLICT DETECTED`; çözülmediyse `DECISION BLOCKED` + tek soru | Çelişkiyi sessizce çözmek · iki sürümü tutmak | RED — karar geçersiz | `log.md` etiket satırı |
| **G11 Gizli işlem yok** | Yıkıcı, geri alınamaz ya da uzun süreli sessiz iş **önceden** raporlanır | Arka planda paket kurulumu · sessiz migration | CRITICAL | Görev listesi + onay satırı |
| **G12 Yetki ve gate disiplini** | Kod yalnız **rolün X alanında**; gate'leri **kullanıcı** kapatır | Kapalı gate'de `.cs` yazmak · `architect`/`reviewer` kod yazması | RED — dosyalar geri alınır | Gate kapanış kaydı + dosya sahipliği |

### §1.1 Temel yasak örnekleri (KOTU / İYİ)

```text
G2  KOTU : cmd.CommandText = "SELECT * FROM T WHERE Id = " + id;
    İYI  : cmd.CommandText = "SELECT * FROM T WHERE Id = @p1";
           cmd.Parameters.AddWithValue("@p1", id);

G3  KOTU : <PackageVersion Include="DevExpress.Win" Version="24.2" />
    İYI  : <PackageVersion Include="DevExpress.Win" Version="26.1" />
           ribbon.RibbonStyle = RibbonControlStyle.Office2010;

G5  KOTU : "kod temiz, derleniyor gibi"   (çıktı yok)
    İYI  : dotnet build -> 0 hata (net48, net10.0-windows) / 42 sn
           dotnet test  -> 18 geçti / 0 başarısız / 3,1 sn

G7  KOTU : Remove-Item -Recurse -Force src\legacy   (sorgusuz)
    İYI  : G7 ONAYI: src/legacy silinecek — geri alınabilir: HAYIR
           A) uygula  B) iptal  C) alternatif öner → onay BEKLENIYOR

G12 KOTU : GATE-003 kapalıyken Program.cs'e satır eklemek
    İYI  : gate hazır → kabul kriteri kanıtı → kullanıcı onayı → gate kapanır
```

---

## §2 Yasaklar Özeti ve Tespit Komutları

| # | Yasak | Tespit komutu (PowerShell) | Beklenen |
|---|---|---|---|
| 1 | EF Core | `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` | **0 satır** |
| 2 | String-concat SQL | `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id\|Name)'` | **0 satır** |
| 3 | DevExpress sürümü | `Select-String -Path Directory.Packages.props -Pattern 'DevExpress\.\w+ \d+\.\d+'` | tek sürüm `26.1` |
| 4 | Ribbon stili | `Select-String -Path src\**\*.cs -Pattern 'RibbonControlStyle\.\w+'` | yalnız `Office2010` |
| 5 | Frontmatter'sız MD | ilk satırı `---` olmayan `.ai/**/*.md` sayımı | **0 dosya** |
| 6 | Bozuk link | vault link taraması | `BROKEN = 0` |
| 7 | Mojibake | `.ai/**/*.md` ilk 200 bayt kontrolü | `0 dosya` |

> Komutlar **salt okunurdur**; `C:\www\` ve `*.old.*` üzerinde yazma G7 + G1 çift ihlalidir.

---

## §3 Kanıt Formatı

### §3.1 Doğrulama etiketleri (7 adet)

| Durum | Doğru yazım | Yanlış yazım |
|---|---|---|
| Diskte okudum | `REPOSITORY_VERIFIED — .ai/index.md:12 (2026-10-07)` | "index'de yazıyordu" |
| Belgede okudum | `SOURCE_VERIFIED — ADR-028 §2.1` | "planda öyle" |
| Kaynak kodunda | `REFERENCE_VERIFIED — dosya + satır` | "kaynak böyle yapıyor" |
| Web'de doğruladım | `WEB_VERIFIED — <URL> (tarih)` | "internette öyle" |
| Kendi çıkarımım | `INFERRED — doğrulanmadı` | doğrulanmış gibi sunmak |
| Doğrulanmalı | `NEEDS_VALIDATION — komut: <komut>` ya da `DOĞRULAMA GEREKLI` | "sorun olmaz" |
| Bilmiyorum | `UNKNOWN` + işi durdur | tahmin yazmak |

> **KURAL:** Etiketsiz teknik iddia — hangi alanda olursa olsun — **uydurmadır** ve madde silinir.

### §3.2 Gate kanıt kutusu (4 satır)

```text
┌──────────────────────────────────────────────────────────┐
│ KANIT   : <komut> -> <sayılar>                          │
│ DOSYA   : .ai/CLAUDE.md, .ai/WORKFLOW.md                │
│ EKSIK   : YOK  (varsa: DOĞRULAMA GEREKLI)               │
│ SONRAKI : <adım, ~ süre>                                │
└──────────────────────────────────────────────────────────┘
```

**Karıştırma belirtisi:** kutuda `BASLIK` ya da `RİSK` satırı görürsen bu ajan
kutusudur ([AGENTS.md](definitions/agents.md) §3); gate kanıtı geçersiz sayılır.

### §3.3 Hata raporu biçimi

```text
┌──────────────────────────────────────────────────────────┐
│ KONUM     : src/.../SessionRepository.cs:64              │
│ NEDEN     : $"SELECT ... {id}" — string-concat (G2)      │
│ DUZELTME  : CommandText + Parameters.Add (~6 dk)         │
│ KANIT     : dotnet build -> 0 hata; tarama -> 0 eşleşme   │
└──────────────────────────────────────────────────────────┘
```

---

## §4 Dur / Bekle Kuralları

### §4.1 Durduran 5 durum

| # | Durum | Etiket | Yapılacak |
|---|---|---|---|
| 1 | **GATE-003 kapalı** | `GATE-OPEN` | `.cs` yazımı **yasak**; okuma/planlama serbest |
| 2 | Yıkıcı işlem gerekiyor | `G7 ONAYI` | Tek satır onay + A/B/C; onay gelmeden komut çalışmaz |
| 3 | Yeni paket / sürüm / ORM | `G8` | Kullanıcı onayı; "geçici çözüm" yok |
| 4 | Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | Kod yazmayı bırak, varsayımı **adlandır**, A/B/C ile sor |
| 5 | Bilgi yok / çelişki çözülmedi | `UNKNOWN` / `DECISION BLOCKED` | Madde yazma, işi durdur |

### §4.2 Onay istemi şablonu

```text
┌──────────────────────────────────────────────────────────┐
│ G7 ONAYI GEREKIYOR                                      │
│   ISLEM  : <tam yol> - <ne yapılacak>                   │
│   ETKI   : <geri alınabilir mi: EVET/HAYIR + kurtarma>   │
│   SECENEK: A) uygula  B) iptal  C) alternatif öner      │
│   BEKLEME: onay gelmeden ISLEM YAPILMAZ                 │
└──────────────────────────────────────────────────────────┘
```

### §4.3 Gate'ler kimin

| Gate | Ne kapatır | Kapatan |
|---|---|---|
| GATE-004a | Vault yeşil (0 bozuk link · 0 mojibake) | kullanıcı |
| **GATE-003** | **UI onayı — HARD STOP** | **yalnız kullanıcı** |
| GATE-004 | İskelet + iki bacak build + ilk commit | kullanıcı |
| GATE-005 | Test + NetArchTest 0 ihlal | kullanıcı |
| GATE-006 | Teslim | kullanıcı |

> **KURAL:** Ajan gate'i **hazırlar**, asla **kapatmaz**.

---

## §5 Katman Yasakları (ADR-028 bağlayıcı)

**Bağlayıcı:** [ADR-028](.decisions/accepted/adr-028-layered-baseline-9-projects.md) · [architecture/index](architecture/index.md) · [layer-boundary-check](checklists/layer-boundary-check.md)

### §5.1 İzinli yön (saf ASCII)

```text
 l7-ui  ──────►  l6-host  ──────►  l5-protocol
                     │                  │
                     ▼                  ▼
             l4-infrastructure ──► l2-application ──► l1-domain ──► l0-abstractions
                     │                     │                 ▲
                     └──► l3-crosscutting ──┘                 │
                            (l3 yalnız l0'a bağlanır)         │
                                                              │
                  (her katman içeriye yalnız interface ile bağlanır)
```

**İki doğrudan ok yasağı:** `l4 → l2` **X** · `l5 → l4` **X** · `l3` yalnız `l0`.

### §5.2 Mimari sınır yasakları (ai.08 §49)

```text
┌──────────────────────────────────────────────────────┐
│ Domain ──► UI                 ❌                      │
│ Domain ──► DevExpress         ❌                      │
│ Domain ──► Infrastructure    ❌                      │
│ Domain ──► Database           ❌                      │
│ Domain ──► AI Provider        ❌                      │
│                                                      │
│ Application ──► DevExpress    ❌                      │
│ Application ──► CLI           ❌                      │
│ Application ──► Database API ❌                      │
│ Application ──► Provider SDK ❌                      │
│                                                      │
│ Agent Runtime ──► UI          ❌                      │
│ Agent Runtime ──► DevExpress  ❌                      │
│                                                      │
│ UI ──► Infrastructure         ❌                      │
│ UI ──► Database               ❌                      │
│ UI ──► CLI                    ❌                      │
└──────────────────────────────────────────────────────┘
```

> UI, Application üzerinden işlem başlatır. Domain, Infrastructure'ı **bilmemelidir**.

### §5.3 Katman × proje × yasak özeti

| Makro katman | Proje (.csproj) | Kesinlikle YASAK |
|---|---|---|
| `l0` Abstractions | `VersaCoder.Abstractions` | DevExpress · ADO.NET · somut sınıf |
| `l1` Domain | `VersaCoder.Domain` | Infrastructure/Host/UI `using` · SQL · `DateTime.Now` |
| `l2` Application | `VersaCoder.Application` | Infrastructure'i tanıma · UI tipini iş mantığında taşımak |
| `l3` Crosscutting | `VersaCoder.Crosscutting` | Kural iş mantığı · `l0` dışına bağımlılık |
| `l4` Infrastructure | `VersaCoder.Infrastructure` | EF Core · string-concat SQL · parametresiz `Execute` |
| `l5` Protocol | `VersaCoder.Protocol` | UI tipine referans · runtime üretim SQL'i |
| `l6` Host | `VersaCoder.Host` | İş mantığı (yalnız kompozisyon + startup) |
| `l7` UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | İş mantığı · yetkisiz `.Designer.cs`/`.resx` |
| test | `VersaCoder.Tests` (+ `ArchitectureTests`) | Üretim kodunda "küçük düzeltme" |

**Okuma yönü:** `l7 → l6 → l5 → l4 → l2 → l1 → l0`; `l3` yalnız `l0`'a bakar;
`l4` ile `l2` arasında doğrudan ok **yok** (port `l0`'da, uygulaması `l4`'te).

**Yeni tip nereye:** arayüz→`l0`, iş kuralı→`l1`, senaryo→`l2`, kesit→`l3`,
somut dış dünya→`l4`, araç sözleşmesi→`l5`, kurulum→`l6`, ekran/komut→`l7`.
Kararsız kaldığında tahmin yazma: `DOĞRULAMA GEREKLI` + [architecture/index](architecture/index.md).

---

## §6 Sayısal Gerçekler (uydurma yasak)

| # | Sayı | Anlam | Kaynak |
|---|---|---|---|
| 1 | **8 makro katman / 52 düğüm** | l0..l7 + N01-N44 | ADR-028 §2.1 |
| 2 | **9 üretim + 1-2 test projesi** | en fazla 11 `.csproj` × 2 TFM | ADR-028 §2.1 |
| 3 | **5440 katalog kaydı** | `count-components` = PASS | ADR-028 §4 |
| 4 | **500 MD** | GATE-004a eşiği | vault gate eşiği |
| 5 | **7 ekran** | `01-start-page` … `07-settings` | [VISION](definitions/vision.md) |

> Kaynaksız hücreye **`DOĞRULAMA GEREKLI`** yazılır; tahmin sayı girmez (G9).

---

## §7 SSS (8 Soru)

**S1 — Başka `.ai` dosyası bu anayasayla çelişirse?** Bu dosya kazanır; çelişki `G10` ile raporlanır.

**S2 — Kanıtım yoksa ne yazarım?** `UNKNOWN` ya da `DOĞRULAMA GEREKLI` — ve işi o adımda durdurursun.

**S3 — Bu anayasayı kim değiştirebilir?** Yalnız kullanıcı; `log.md`'ye kaydedilir.

**S4 — Aynı madde iki dosyada yaşarsa?** SSOT ihlali başlar; birinde tut, diğerine yalnız bağlantı ver.

**S5 — Kanıt kutusu ile ajan kutusunu karıştırırsam?** Gate kanıtı geçersiz sayılır, dört satırla tekrar istenir (§3.2).

**S6 — "Bilmiyorum" demek işi batırır mı?** Hayır; gizlemek batırır. `UNKNOWN` + durdurmak doğrudur.

**S7 — G maddeleri ezberlenmeli mi?** Hayır; §2'deki tespit komutları maddelerin çoğunu otomatik yakalar.

**S8 — Bir madde birden fazla G'ye giriyorsa?** En ağırı (`CRITICAL > HIGH > RED`) raporlanır, tümü `log.md`'ye etiketlenir.

---

## §8 Doğrulama

- [ ] Frontmatter 11 alan, ilk satır `---`, `authority: primary`, `updated: 2026-10-07`
- [ ] G1..G12 tek tek `kural/yasak/ihlalde/kanıt` dörtlüsünde
- [ ] §3'te 7 doğrulama etiketi + 4 satır kanıt kutusu + hata raporu
- [ ] §4'te 5 dur/bekle durumu + G7 onay şablonu
- [ ] §5 ADR-028'e bağlı, diyagram **saf ASCII** (katman yönü)
- [ ] §6 sayıları yalnız kaynaklı; kaynaksız hücre `DOĞRULAMA GEREKLI`
- [ ] Tüm markdown linkleri izinli hedeflere

### §8.1 Kırmızı ekip — anayasanın en sık delinen 8 yeri

| # | Delik | Belirtisi | Yakalama | Düzeltme |
|---|---|---|---|---|
| 1 | "Geçici paket" eklenmiş | `Directory.Packages.props` onaysız satır | §2 satır 3 | Paketi kaldır ya da `G8` onayı al |
| 2 | `KANIT` satırı boş | Kapanışta komut çıktısı yok | §3.2 tablosu | Komutu çalıştır; yoksa `UNKNOWN` |
| 3 | Katman oku ters | `l4 → l2` kullanan kod | NetArchTest `LayerRulesTests` | Portu `l0`'a taşı |
| 4 | Ajanın gate'i kapatması | `log.md` onay satırı yok | §4.3 tablosu | Gate'i yeniden aç |
| 5 | Etiketsiz iddia | "olması gerekir" | §3.1 tablosu | Etiketi ekle ya da sil |
| 6 | Tek bacakta kod | Bir TFM'de derleniyor | `-f net48` + `-f net10.0-windows` | `#if`'i yalnız platform farkına indir |
| 7 | Kopya metin | Cümleler eski vaultla birebir | G1 benzerlik | Maddeleri kendi cümlenle yaz |
| 8 | Kırık link | `Test-Path` = false | link taraması | Hedefi ekle ya da gelecek sürümde bağla |

**Ortak belirti:** sekiz deliğin yedisi **kanıt eksikliği** olarak görünür.

---

## §9 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Katman/proje baz kararı | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 2 | Süreç ve gate'ler | `WORKFLOW.md` |
| 3 | Rol ve karar hiyerarşisi | `ROLE.md` |
| 4 | Ajan dağıtım matrisi | `AGENTS.md` |
| 5 | Katman sınırı kontrol listesi | `checklists/layer-boundary-check.md` |
| 6 | Navigasyon · karar defteri · oturum durumu | `index.md` · `brain.md` · `MEMORY.md` |
| 7 | Prompt kaynağı (mimari) | `G:/…/versacoder proje planlaması ai.08.md` |
| 8 | Prompt kaynağı (proje tanımı) | `G:/…/versacoder proje planlaması ai.04.md` |
| CLAUDE | [CLAUDE](claude.md) | AI anayasa — G1..G12 hard guardrail, kanit formati, dur/bekle |
| engine | [engine](definitions/engine.md) | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| brain | [brain](.decisions/brain.md) | ADR ozetleri (29) + SOLID kisitlari + 6 sert kod kurali |
| PROJECTS | [PROJECTS](definitions/projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [glossary](definitions/glossary.md) | terim sozlugu (136 terim) TR / EN / ELI10 |
| ULTRA | [ULTRA](definitions/ultra-thinking.md) | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| ADR-031 | [ADR-031](.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok CLAUDE | [kok CLAUDE](../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok README | [kok README](../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok AGENTS | [kok AGENTS](../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
