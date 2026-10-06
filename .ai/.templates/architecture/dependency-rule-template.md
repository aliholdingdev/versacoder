---
title: "VersaCoder — Bağımlılık Kuralı Şablonu (l0..l7 Matrisi)"
type: template
category: templates/architecture
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "izinli/yasak bağımlılık matrisi, yönelim kuralı, ihlal tespiti ve ihlal durumunda ne olduğu — hedef .ai/architecture/dependency-rule.md"
---

# VersaCoder — Bağımlılık Kuralı Şablonu (l0..l7 Matrisi)

**Zorunlu Bağlantılar:** [index](../index.md) · [architecture-template](architecture-template.md) · [layer-spec-template](layer-spec-template.md) · [ascii-diagram-template](ascii-diagram-template.md) · [repository-template](../backend/repository-template.md) · [form-template](../frontend/form-template.md) · [CLAUDE](../../../CLAUDE.md)

---

## §1 Amaç

**Bağımlılık kuralı**, hangi katmanın hangi katmana bakabileceğini tek sayfada kesinleştiren, **test ile kanıtlanan** ve ihlalinde işi **durduran** tek gerçek kaynaktır. Bu şablon, `.ai/architecture/dependency-rule.md` dosyasının tek iskeletidir: matris, yön oku, Domain'in bilmediği şeyler, DevExpress sınırı, ihlal tespit komutları, ihlal cezası ve anti-pattern listesi aynı dosyada yaşar.

| Alan | Değer |
|------|-------|
| Şablon / Yol | `dependency-rule-template.md` · `.ai/.templates/architecture/dependency-rule-template.md` |
| Hedef Dosya | `.ai/architecture/dependency-rule.md` |
| Katman Dili | `l0` … `l7` + test (kök `CLAUDE.md` B5) |
| Kanıt Aracı | `ArchitectureTests` (NetArchTest) + §5.3 PowerShell |
| Kapı | `GATE-002` mimari onayı — matris onaysız koda uygulanmaz |
| Sorumlu | `architect` · denetim `reviewer` · kabul `master-orchestrator` |
| Kodlama | **UTF-8 (BOM'suz)** — ç ğ ı İ ö ş ü doğru; mojibake YASAK |
| Dil | Türkçe metin · katman/proje/namespace adı İngilizce |
| Versiyon · Authority | `1.0.0` · `SSOT` (bu dosya) |
| Bootstrap | 2026-10-05 |

### §1.1 Bu Şablon Ne Üretir

Tek dosya üretir: `.ai/architecture/dependency-rule.md`. Bu dosya **belge değil, denetim sözleşmesidir** — her satırı ya bir tablo hücresinde ya bir testte ya da §5.3 komutunda karşılığı olan bir kuraldır.

| Çıktı Parçası | Nerede Yaşar | Tekrar Edilir mi |
|---------------|--------------|------------------|
| İzinli/yasak matrisi (§3.4) | hedef dosyanın §3'ü — **tek yer** | Hayır; `README` yalnız bağlantı verir |
| Yön oku `l7→l6→l5→l4→l2→l1→l0` (§3.3) | hedef dosyanın §3'ü | Hayır; kök `CLAUDE.md` B5 özeti verir |
| Architecture testleri (§3.8) | `tests/**/ArchitectureTests` | Kural metni koda **kopyalanmaz**, test onu okur |
| İhlal tespit komutları (§5.3) | bu şablonda kalır, hedefe **yalnız sonuç** yazılır | Komut bloğu ikiye bölünmez |
| Anti-pattern listesi (§3.11) | hedef dosyanın §3'ü | Hayır |

**SRP gerekçesi:** Katman şartnamesi `layer-spec-template`'in, genel yığın `architecture-template`'in, çizim `ascii-diagram-template`'in, port kodu `repository-template`'in işidir. Bu dosya yalnız **okun hangi tarafa bakabileceği** ve **ihlalin bedeli** konusundadır; dördü de buraya taşınırsa dört SSOT doğar (envanter G12).

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz; satır `⚠️ VERIFICATION REQUIRED` olarak kalır ve hiçbir yorum yazılmaz. Ölçüm: `C:\versacoder`, 2026-10-05, `get_file_info` + dizin taraması.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| E1 | `.ai/.templates/architecture/` = 3 şablon | ✅ DOĞRULANDI | `architecture-template` · `ascii-diagram-template` · `layer-spec-template` |
| E2 | `.ai/.templates/` = **38 MD** | ✅ DOĞRULANDI | `search_files **/*.md` = 38 (bu dosya ile 39) |
| E3 | Envanter satırı #15 `dependency-rule-template` | ⚠️ GEÇERSİZ | `index.md` §3.3: `Durum = ÜRETİMDE` — `ÜRETİMDE` geçerli değer değil (`YAZILDI/PLANLI/MEVCUT`) |
| E4 | `.ai/.templates/architecture/dependency-rule-template.md` | ❌ YOK → ✅ bu dosya | yazım öncesi `get_file_info` hata verdi |
| E5 | Hedef `.ai/architecture/dependency-rule.md` | ❌ YOK | `Test-Path = False` |
| E6 | `.ai/architecture/` (hedef dizin) | ❌ YOK | `Test-Path = False` |
| E7 | `src/` · `tests/` · `VersaCoder.sln` | ❌ YOK | üçü de `Test-Path = False` → §5.3 dürüst `DIZIN YOK` yazar |
| E8 | Kök `CLAUDE.md` · `AGENTS.md` · `README.md` · `WORKFLOW.md` | ✅ VAR | 4 MD, kökte |
| E9 | `.ai/index.md` · `.ai/CLAUDE.md` · `.ai/WORKFLOW.md` · `.ai/.rules/` · `.ai/adr/` | ❌ YOK | **bu dosyadan asla bağlantı verilmez** (G7) |
| E10 | `global.json` · `Directory.Packages.props` · `.ai/scripts/` | ❌ YOK | §5.3 komutları harici betiğe bağımlı değildir |
| E11 | NetArchTest paket sürümü | ⚠️ VERIFICATION REQUIRED | `.csproj` yok; sürüm **uydurulmaz** |
| E12 | DevExpress `26.1` · TFM `net48` + `net10.0-windows` | ⚠️ VERIFICATION REQUIRED | kod/sln kanıtı yok; ADR-001'e sorulur |

**Yorum yasağı:** `❌ YOK` ve `⚠️` satırları için "şurada olmalıydı", "muhtemelen vardır" denmez. Boşluk ya §5.1 Adım 2'de kapatılır ya da satır olduğu gibi kalır. E3, envanter satırının **neden düzeltilmesi gerektiğini** kanıtlar: dosya yokken `ÜRETİMDE` yazılmış (§6.5 K1).

### §1.3 Bağımlılık Kuralı Ne Değildir?

| Kuraldır (BU ŞABLON) | Değildir (hangi şablon) |
|----------------------|-------------------------|
| Hangi katmanın hangi katmana **bakabileceği** | Katmanın **ne yaptığı** → `layer-spec-template` |
| İhlalin **nasıl yakalanacağı** ve **bedeli** | Mimari kararın **gerekçesi** → `adr-architecture-template` |
| Tek sayfalık matris + ok + test | Katman yığını, TFM, proje ağacı → `architecture-template` |
| Port ile uygulamanın **ayrımı** (§3.5) | Portun kendi kodu → `backend/repository-template` |
| DevExpress'in **nerede duracağı** | Formun kendisi → `frontend/form-template` |
| `{{…}}` iskelet (§3.9) | Dolu mimari belge → `.ai/architecture/dependency-rule.md` |

### §1.4 Katman Yaşam Döngüsü ve Ok Yönü

```text
+----------------------------------------------------------------+
| [l7] UI + CLI     RibbonForm / arguman ayristirma   (kaynak)   |
|    v  ── izinli tek yön ────────────────────────────────────── |
| [l6] Host         composition root, DI, appsettings            |
|    v                                                           |
| [l5] Protocol     mesaj semasi, tool sozlesmesi                |
|    v                                                           |
| [l4] Infrastructure  ADO.NET + Dapper, dosya, saglayici        |
|    v  ╳  dogrudan ok YOK (port l0'da, §3.5)                    |
| [l2] Application  use case, CQRS handler                       |
|    v                                                           |
| [l1] Domain       entity, invariant, domain event              |
|    v                                                           |
| [l0] Abstractions port arayuzu, Result<T>, event sozlesmesi    |
+----------------------------------------------------------------+
| [l3] Crosscutting  logging, IClock, yardimcilar → YALNIZ [l0]  |
| [test] ArchitectureTests + unit — katman DEGIL, kanit uretir   |
+----------------------------------------------------------------+
```

**Ne anlatır:** Üstten alta tek ok; `l3` zincirin dışındadır ve yalnız `l0`'a bakar; `l4` ile `l2` arasında **ok yoktur** — ikisi aynı portta (`l0`) buluşur. Yukarı (alta → üste) bakan her referans ihlaldir. **Test katman değildir**, ayrı projedir (üretim 9 + test 1–2, en fazla 11 `.csproj`).

---

## §2 Kapsam

**İçinde:** bağımlılık matrisi, yön kuralı, Domain'in yasak bilgileri, DevExpress sınırı, ihlal tespiti (test + komut), ihlal cezası, anti-pattern listesi.
**Dışında:** katman içi sınıf tasarımı, port imzaları, SQL/şema, paket sürümü, kapı takvimi, ekran onayı.

### §2.1 Dosya Tipi → Şablon Eşlemesi

| # | Dosya / Karar | Şablon | Bu Şablon mu |
|---|---------------|--------|--------------|
| 1 | `.ai/architecture/dependency-rule.md` | **bu şablon** | ✅ |
| 2 | `.ai/architecture/layer-<ad>.md` | `architecture/layer-spec-template` | ❌ |
| 3 | `.ai/architecture/<konu>.md` (yığın/ağaç/akış) | `architecture/architecture-template` | ❌ |
| 4 | `.ai/architecture/diagrams/*.md` | `architecture/ascii-diagram-template` | ❌ |
| 5 | `.ai/adr/ADR-NNN-arch-*.md` (gerekçeli karar) | `adr/adr-architecture-template` | ❌ |
| 6 | `src/**/XRepository.cs` (port + uygulama kodu) | `backend/repository-template` | ❌ |
| 7 | `tests/**/*ArchitectureTests.cs` (kanıt) | `testing/unit-test-template` | ❌ — §3.8 yalnız **örnek verir** |
| 8 | `.ai/architecture/dependency-rule.md` **tartışması** | — şablon yok | envantere §5.1 Adım 1'den satır aç |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. **Bağımlılık yönü / `using` yasağı mı** tanımlıyor? → **bu şablon**.
2. Katmanın **iç işi mi** (sorumluluk, TFM, paket)? → `layer-spec-template`.
3. Bir karar mı (geri alınabilir, gerekçeli)? → `adr-architecture-template`.
4. Diyagram / çizim mi? → `ascii-diagram-template`.
5. Port **kodu** mı (`IPesRepository` + `PesRepository`)? → `backend/repository-template`.
6. Portun **sorgusu / SQL'i** mi? → `query/Query-Template`.
7. Testin **kendisi** mi yazılıyor? → `testing/unit-test-template`.
8. **Hiçbiri değilse** → şablon yoktur; talep `index.md` §5.1 Adım 1'den açılır.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriğinden cümle mi geldi (G1)? · ORM bağımlılığı mı geçiyor (G2)? · `.ai/.rules/` kural metni mi kopyalanıyor (G12)? · `GATE-002` mimari onayı var mı (§4.2)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek `dependency-rule.md` dosyasının yapısını** tanımlar.

### §3.1 Hedef Dosya Sözleşmesi

Her üretimde şu alanlar **tek satırda** doldurulur; boş hücre kabul edilmez:

| Alan | Kural | Zorunlu Kalıp |
|------|-------|---------------|
| Dosya | kökten yazılır | `.ai/architecture/dependency-rule.md` |
| Amaç | tek cümle | `{{KATMAN_ADI}}` katmanı {{YON}} yönünde bakabilir/emez. |
| Matris | 8×8, dört değerli | `✓` · `✗` · `—` · `†` (§3.4) |
| Port satırı | her port için | `{{PORT_ADI}}` → `l0` · uygulama `l4` · doğrudan ok `✗` |
| Test | kanıt yoksa kural yok | `ArchitectureTests/{{TEST_ADI}}` |
| Onay | kapı kaydı | `GATE-002` onay tarihi + `{{TARIH}}` |
| Kodlama | UTF-8 (BOM'suz) | Latin-1/CP1254 otomatik RED |

### §3.2 Katman Kayıt Defteri (`l0`..`l7` + test)

Bu tablo kök `CLAUDE.md` B5'teki listeyi **kaynak alır**; çelişkide kök dosya geçerlidir, bu dosya yalnız matrisi uygular.

| # | Katman | `.csproj` | Tek satır sorumluluk | Bakabilir |
|---|--------|-----------|----------------------|-----------|
| 0 | `l0` Abstractions | `VersaCoder.Abstractions` | port arayüzleri, `Result<T>`, event sözleşmesi | **hiçbirine** |
| 1 | `l1` Domain | `VersaCoder.Domain` | entity, invariant, domain event | `l0` |
| 2 | `l2` Application | `VersaCoder.Application` | use case, CQRS handler, orkestrasyon | `l1`, `l0` |
| 3 | `l3` Crosscutting | `VersaCoder.Crosscutting` | logging, `IClock`, dosya/yardımcılar | **yalnız `l0`** |
| 4 | `l4` Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, dosya, git, LLM sağlayıcı | `l1`, `l0` |
| 5 | `l5` Protocol | `VersaCoder.Protocol` | mesaj şeması, tool sözleşmeleri | `l4`, `l2`, `l1`, `l0` |
| 6 | `l6` Host | `VersaCoder.Host` | composition root, DI, `appsettings*.json` | `l5`, `l4`, `l2`, `l1`, `l0` |
| 7 | `l7` UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | RibbonForm sunumu / CLI ayrıştırma | `l6` … `l0` |
| — | test | `VersaCoder.Tests` (+ `ArchitectureTests`) | kanıt üretir; hedef **%90** coverage | katman değildir |

**Sayaç:** 8 makro katman + 1 test = en fazla **11 `.csproj`** (9 üretim + 1–2 test). 12. proje çıkarsa matris yeniden yazılır, sayı **uydurulmaz** (G9).

### §3.3 Yön Kuralı — Tek Ok

```text
+----------------------------------------------------------------+
| YON:  l7 → l6 → l5 → l4 → l2 → l1 → l0                         |
| IZIN: kaynak, bu zincirde KENDISINDEN SONRA gelen her katmana  |
|       bakabilir (yukari bakan referans = IHLAL)                |
| ISTISNA 1:  l3  →  yalniz l0      (diger her seye kapali)      |
| ISTISNA 2:  l2 ↔ l4  dogrudan ok YOK (port l0'da, §3.5)        |
+----------------------------------------------------------------+
```

| # | Madde | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Ok **tek yönlüdür** | `VersaCoder.Desktop` → `VersaCoder.Host` | `VersaCoder.Host` → `VersaCoder.Desktop` |
| 2 | Zincir **sıralama**tır | komşu olmayana da bakılabilir (`l6` → `l2`) | yalnız komşuya bakılır diye okunmak |
| 3 | Matris **kod gerçeğidir** | §3.4'te `✗` olan her yerde `using` **yoktur** | "zincirde yazıyor" diye `using` savunmak |
| 4 | `l3` zincirin **dışında**dır | yalnız `l0` (§3.2/3) | `l3` → `l1` (invariant sızması) |
| 5 | `l2`–`l4` **tek noktada** buluşur | `l0` portu | `l2` → `l4` doğrudan `using` |
| 6 | Yukarı bakan = ihlal | `l0` hiçbirine bakmaz | `l0` → `l4` "yardımcı sınıf için" |

**Çelişki kuralı:** §3.3 zinciri **tariftir**, §3.4 matrisi **kodun kendisidir**. İkisi çelişirse **matris geçerlidir**; özellikle `l2 ↔ l4` her iki yönde de doğrudan `using` yasağıdır (§3.5). Tarifle kod çatışıyorsa kod düzeltilir — matris esnetilmez.

### §3.4 İzinli / Yasak Bağımlılık Matrisi

Satır = **kayan** (bağımlı olan), sütun = **bakılan** (referans verilen). Dört değer: `✓` izinli · `✗` yasak · `—` kendisi · `†` §3.4.1 notu.

| Kaynak ↓ / Hedef → | `l0` | `l1` | `l2` | `l3` | `l4` | `l5` | `l6` | `l7` |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| `l7` UI + CLI | ✓ | ✓ | ✓ | † | ✓ | ✓ | ✓ | — |
| `l6` Host | ✓ | ✓ | ✓ | † | ✓ | ✓ | — | ✗ |
| `l5` Protocol | ✓ | ✓ | ✓ | † | ✓ | — | ✗ | ✗ |
| `l4` Infrastructure | ✓ | ✓ | **✗** | † | — | ✗ | ✗ | ✗ |
| `l2` Application | ✓ | ✓ | — | † | **✗** | ✗ | ✗ | ✗ |
| `l1` Domain | ✓ | — | ✗ | † | ✗ | ✗ | ✗ | ✗ |
| `l0` Abstractions | — | ✗ | ✗ | ✗ | ✗ | ✗ | ✗ | ✗ |
| `l3` Crosscutting | ✓ | ✗ | ✗ | — | ✗ | ✗ | ✗ | ✗ |

**§3.4.1 `†` notu — `l3` sütunu.** Verilen kural yalnız `l3 → l0` okunu yazar; `l3`'e **kimlerin** bakacağı kaynakta açık değildir. Bu yüzden hücre `†` ile işaretlidir: `l3`'e referans **yalnız `logging`, `IClock` ve yardımcı tür tüketimiyle** sınırlıdır; kapsam genişletilecekse `ADR-028 §2.1` onayı gerekir. Onay yoksa hücre **`✗` sayılır** ve genişletme RED'tir (G9).

**§3.4.2 Yoğunluk okuma.** 8×8 = 64 hücre: `—` 8 (köşegen), `†` 7, `✗` 27, `✓` 22. İhlal Adayı = `l1` satırı (6 `✗` — Domain en savunmasız katmandır) ve `l4 × l2` / `l2 × l4` çifti (§3.5).

**§3.4.3 Test kapsamı.** Her `✗` hücre için **en az bir** Architecture testi yazılır; `✓` hücreler için test yazılmaz (izin zaten `using`'den görünür). Toplam zorunlu test: **27** (§6.1 K14).

### §3.5 `l2` ↔ `l4` Ayrımı — Repository Portu `l0`

```text
+----------------------------------------------------------------+
| [l2] Application   →  IPesRepository      (port, l0)        ✓  |
| [l4] Infrastructure : IPesRepository      (uygulayici, l0)   ✓ |
| [l2] Application   →  PesRepository       (dogrudan)         ✗ |
| [l4] Application   →  use case tipi       (geriye dogru)     ✗ |
| bulusma noktasi:  l0  — DI kaydi l6'da, nesne l7'den dogar     |
+----------------------------------------------------------------+
```

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Port **`l0`'da** yaşar | `VersaCoder.Abstractions/IPesRepository.cs` | portu `l2` içine koymak |
| 2 | Uygulama **`l4`'te** yaşar | `VersaCoder.Infrastructure/PesRepository.cs` | uygulamayı `l2`'ye koymak |
| 3 | `l2` portu **bilir** | `IPesRepository` (kendi altındaki `l0`) | `l2` → `l4` `using` |
| 4 | `l4` uygulamayı **bilir** | `: IPesRepository` | `l4` → `l2` `using` |
| 5 | DI kaydı **`l6`'da** | `services.AddScoped<IPesRepository, PesRepository>()` | `l2` içinde `new PesRepository()` |
| 6 | Tipik ihlal belirtisi | `l2` dosyasında `using VersaCoder.Infrastructure` | "geçici çözüm" satırı |

**Neden yasak:** `l2 → l4` yazılırsa use case bir anda bağlantı dizesi, dosya yolu ve sağlayıcı seçimi bilir; test çifti (`l2` + sahte port) çöker. Port `l0`'da olduğu için `l2` yalnız **soyutlamayı** görür, `l4` yalnız **uygulamayı** — aralarındaki ok **hiçbir zaman çizilmez**.

### §3.6 Domain'in Yasak Bilgileri

`l1` (Domain) **yalnız iş gerçeğini** bilir. Şu altı bilgi Domain'e girerse katman çöker; hepsi §3.4'te `l1` satırındaki `✗` ile aynı kökten gelir.

| # | Yasak Bilgi | Belirtisi | Doğru Katman |
|---|-------------|-----------|--------------|
| 1 | **UI** | `System.Windows.Forms`, kontrol adı, ekran metni | `l7` |
| 2 | **DB** | bağlantı dizesi, tablo adı, SQL metni, dosya yolu | `l4` (`@p1..@pN`) |
| 3 | **ORM** | `DbSet`, entity map, lazy-load nesnesi | `l4` — veri erişimi ADO.NET + Dapper |
| 4 | **DevExpress** | `XtraForm`, `GridControl`, `LookAndFeel` | `l7` (§3.7) |
| 5 | **CLI** | argüman ayrıştırma, `args[]`, konsol çıktısı | `l7` (`VersaCoder.Cli`) |
| 6 | **MCP** | tool çağrısı, protokol mesajı, istemci nesnesi | `l5` |

| Domain'in **bildiği** | Domain'in **bilmediği** |
|------------------------|--------------------------|
| entity · value object · invariant | nerede saklandığı |
| domain event · kural ifadesi | kimin dinlediği |
| başarisizlık kodu (`Result<T>` hata adı) | nasıl loglandığı (`l3`), nasıl gösterildiği (`l7`) |
| zamanın **kavramı** (ne zaman başlangıç) | saatin kendisi (`IClock` → `l3`) |

**Kısa test:** Domain dosyasında `using` satırlarını oku. `VersaCoder.Domain` dışında **tek** kabul edilen `using` → `VersaCoder.Abstractions`. İkinci bir VersaCoder `using`'i = otomatik RED.

### §3.7 DevExpress = Presentation Sınırı

| # | Kural | İzinli | Yasak |
|---|-------|--------|-------|
| 1 | DevExpress **yalnız `VersaCoder.Desktop`** içinde | `l7` (Desktop) | `l0`–`l6`, `VersaCoder.Cli`, test |
| 2 | DevExpress tipi **Domain'e sızmaz** | domain nesnesi POCO | `XtraForm` türevi domain sınıfı |
| 3 | DI kaydı **`l7`'de** yapılır | `l7` kendi servislerini kaydeder | `l6`'nın `VersaCoder.Desktop`'a `using`'i (matris `l6 × l7 = ✗`) |
| 4 | Görünüm metni **`l7`'de** | `"Kaydedildi"` → form/lokalizasyon | sabit metin `l1` içinde |
| 5 | Sürüm **ADR-001'e** bağlı | `26.1` (kanıt: §1.2 E12 `⚠️`) | kanıtsız "güncel sürüm" iddiası |
| 6 | Çekirdek katman **DevExpress'i hiç görmez** | `l0`–`l6` derlemesi DevExpress referanssız | `csproj` reference satırı |

**Sınır cümlesi (hedef dosyaya birebir yazılır):** *DevExpress bir **sunum teknolojisidir**; `VersaCoder.Desktop` dışına çıktığı anda katman sırası değil, ürünün derlemesi kırılır.*

### §3.8 Architecture Tests — NetArchTest Örnekleri

Kural test olmadan **iddiadır**. `tests/VersaCoder.ArchitectureTests/DependencyRuleTests.cs` içinde her `✗` hücre için bir test yaşar. Aşağıdaki blok **örnektir**; sınıf/tip adları üretilen dosyada `{{…}}` yerine gerçek adlarla yazılır.

```csharp
using NetArchTest.Rules;
using Xunit;

public sealed class DependencyRuleTests
{
    // §3.4 — l1 satiri: Domain hicbir katmana bakamaz, yalniz l0
    [Fact]
    public void Domain_Yalniz_Abstractions_A_Bakar()
    {
        var result = Types.InAssembly(typeof(VersaCoder.Domain.AssemblyMarker).Assembly)
            .Should().HaveDependencyOn("VersaCoder.Abstractions")
            .And().ShouldNot().HaveDependencyOn("VersaCoder.Infrastructure")
            .And().ShouldNot().HaveDependencyOn("VersaCoder.Application")
            .And().ShouldNot().HaveDependencyOn("VersaCoder.Desktop")
            .And().ShouldNot().HaveDependencyOn("DevExpress")
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join(Environment.NewLine, result.FailingTypes));
    }

    // §3.5 — l2 x l4 : dogrudan ok YOK (port l0'da)
    [Fact]
    public void Application_Infrastructure_E_Dogrudan_Baglanti_Yok()
    {
        var result = Types.InAssembly(typeof(VersaCoder.Application.AssemblyMarker).Assembly)
            .ShouldNot().HaveDependencyOn("VersaCoder.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join(Environment.NewLine, result.FailingTypes));
    }

    // §3.7 — DevExpress yalniz l7 (Desktop) icinde yasar
    [Fact]
    public void DevExpress_Only_In_Desktop()
    {
        var result = Types.InCurrentDomain()
            .That().DoNotResideInNamespaceStartingWith("VersaCoder.Desktop")
            .ShouldNot().HaveDependencyOn("DevExpress")
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join(Environment.NewLine, result.FailingTypes));
    }

    // §3.4 — l6 asla UI'ya bakamaz (composition root UI gormez)
    [Fact]
    public void Host_Desktop_A_Bakamaz()
    {
        var result = Types.InAssembly(typeof(VersaCoder.Host.AssemblyMarker).Assembly)
            .ShouldNot().HaveDependencyOn("VersaCoder.Desktop")
            .GetResult();

        Assert.True(result.IsSuccessful,
            string.Join(Environment.NewLine, result.FailingTypes));
    }
}
```

| Kural | Test | Karşılık (matris) |
|-------|------|-------------------|
| `l1` yalnız `l0` | `Domain_Yalniz_Abstractions_A_Bakar` | `l1` satırı: 6 `✗` |
| `l2` ↔ `l4` yok | `Application_Infrastructure_E_Dogrudan_Baglanti_Yok` | `l2 × l4` = `✗` |
| DevExpress sınırı | `DevExpress_Only_In_Desktop` | §3.7 kural 1 |
| `l6` UI görmez | `Host_Desktop_A_Bakamaz` | `l6 × l7` = `✗` |
| **27 hücre** | §6.1 K14 | 27 `✗` × 1 test |

**Uyarı:** `dotnet test` bu depoda **şu an çalışmaz** — `tests/` yok (§1.2 E7). Kanıt, `src/` ve `tests/` üretilince oluşur; o zamana kadar §5.3 komutları **dosya tabanlı** denetimdir.

### §3.9 Tam Kalıp

Bu blok `{{…}}`'nın ana evidir: üretilen dosyada `{{…}}` adedi **0** olmalıdır (G4). Blok dışı kullanım yalnız §3.1 ile sınırlıdır ve üretim dosyasına **taşınmaz**.

````markdown
---
title: "VersaCoder — Bağımlılık Kuralı"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: {{TARIH}}
authority: SSOT
scope: "l0..l7 izinli/yasak bağımlılık matrisi, yon oku, ihlal tespiti ve cezasi"
---

# VersaCoder — Bağımlılık Kuralı

**Zorunlu Bağlantılar:** `.ai/.templates/index.md` · kök `CLAUDE.md` · `.ai/.templates/architecture/dependency-rule-template.md` (yalnız bağlantı, metin kopyalanmaz)

§1 Amaç        → §1.1 ne üretir · §1.2 disk kanıtları · §1.3 ne değildir · §1.4 ok diyagramı
                 {{AMAC_CUMLESI}}
§2 Kapsam      → §2.1 dosya tipi eşlemesi · §2.2 sınır testi
                 {{ICINDE}} / {{DISINDA}}
§3 Mimari      → §3.1 katman kayıt defteri (l0..l7 + test) · §3.2 yön oku · §3.3 matris 8×8 ·
                 §3.4 port ayrımı ({{PORT_ADI}}) · §3.5 Domain yasakları · §3.6 DevExpress sınırı ·
                 §3.7 architecture tests ({{TEST_ADI}}) · §3.8 ihlal cezası · §3.9 anti-pattern ·
                 §3.10 salt-okunur doğrulama · §3.11 değişiklik kontrolü
§4 Kurallar    → §4.1 yazım sırası · §4.2 onay · §4.3 ölçüt · §4.4 değişiklik · §4.5 G1..G12
§5 Workflow    → §5.1 adımlar · §5.2 hata modları · §5.3 salt-okunur komut · §5.4 Quality Report
§6 Doğrulama   → §6.1 checklist (21) · §6.2 red sinyalleri · §6.3 geçiş · §6.4 ilgili · §6.5 envanter
§7 Referanslar → §7.1 bağlantılar · §7.2 red team
````

### §3.10 İhlal Durumunda Ne Olur

| # | Aşama | Bulan | Ne Olur | Süre |
|---|-------|-------|---------|------|
| 1 | Commit öncesi | `§5.3` komutu | satır listelenir; düzeltmeden commit **yapılmaz** | 2 dk |
| 2 | PR / inceleme | `reviewer` | §6.1 K12–K15'ten biri kırmızı → **RED** | 5 dk |
| 3 | Architecture test | `dotnet test` | `Assert.True(result.IsSuccessful)` düşer, test **kırmızı** | 1 dk |
| 4 | Mimari onay | `architect` | matris değişikliği isterse `GATE-002` yeniden açılır | 8 dk |
| 5 | Kapı | `master-orchestrator` | 3 RED → iş durur, gerekçe yükseltilir | — |
| 6 | Düzeltme | `developer` | ya kod taşınır ya da **ADR** ile matris değiştirilir (asla ikisi birden) | 20 dk |

**Tek doğru davranış:** kodu matrise uydurmak. Matrisi koda uydurmak isteyen kişi `ADR-NNN-arch-*` açmak zorundadır (G9); ADR onaylanmadan `✗` hücresi `✓`'ye çevrilemez.

### §3.11 Anti-Pattern Listesi

| # | Anti-Pattern | Belirti | Sonuç | Düzeltme |
|---|--------------|---------|-------|----------|
| 1 | **God Class** | tek sınıf 5+ katmandan tip taşıyor | her katman ona bağımlı olur, matris anlamsızlaşır | sınıfı katmanına böle |
| 2 | **Circular dependency** | `A → B → A` (proje düzeyinde döngü) | derleme imkânsızlaşır, paket şişer | ortak tip `l0`'a iner |
| 3 | **DI leakage** | `new` ile somut repo/`DbContext`-benzeri nesne | test edilemez, gizli `l4` bağımlılığı | port + DI (`l6`) |
| 4 | **Yukarı bakan referans** | `l0`/`l1` içinde `using VersaCoder.Infrastructure` | çekirdek teknolojiye bağlanır | bağımlılık port'a taşınır |
| 5 | **Utils/Helpers batağı** | her katmandan `Common`/`Helpers` kullanımı | gizli 9. katman doğar | tipi ait katmanına taşı |
| 6 | **Arayüz yanlış yerde** | port `l2` içinde tanımlı | `l4` → `l2` mecburiyeti doğar | port `l0`'a (§3.5) |
| 7 | **Testin katman olması** | test projesi üretim `using`'i alır | production kod test paketine bağlanır | test ayrı projedir |
| 8 | **Şişirilmiş `l3`** | `l3` içine iş kuralı/DTO girer | `l3`→`l1` ihlali | yalnız logging/`IClock`/yardımcı |
| 9 | **UI mantığı** | `l7` handler içinde iş kuralı | kural test edilemez | `l2` use case'e |
| 10 | **Koşullu katman** | `#if` ile katman değişimi | iki gerçek kaynağı doğurur | ayrı adaptör |

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | `GATE-002` mimari onayı yoksa | **DUR** — matris yayına girmez |
| 2 | Kök `CLAUDE.md` B5'te katman değişmişse | §3.2 tablosu önce ona eşitlenir |
| 3 | Katman kayıt defteri hazır | §3.3 oku yaz |
| 4 | Matris | §3.4 — 64 hücre, dört değer |
| 5 | Portlar | §3.5 — her port tek satır |
| 6 | Domain yasakları + DevExpress | §3.6 · §3.7 |
| 7 | Testler | §3.8 — her `✗` için bir test |
| 8 | Ceza + anti-pattern | §3.10 · §3.11 |
| 9 | Doğrulama | §5.3 komutları → 0 satır → §6.1 21/21 |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | `architect` | §3.2–§3.5 matrisi + port ayrımı | taslak | 15 dk |
| 2 | `reviewer` | §6.1 (21 madde) | ✅ / RED | 8 dk |
| 3 | `developer` | §3.8 testleri derleniyor mu | `DependencyRuleTests.cs` | 20 dk |
| 4 | `security` | §3.6 + §3.7 (sınır sızması) | ✅ / RED | 5 dk |
| 5 | `master-orchestrator` | §5.3 çıktısı + §5.4 | Quality Report | 5 dk |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşamaya 1'den dön.

### §4.3 Ölçüt / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Matris boyutu | 8×8 (64 hücre) | eksik hücre = RED |
| `✗` hücre | 27 | 26/28 = RED (§3.4.2) |
| Zorunlu test | 27 | < 27 = RED (§6.1 K14) |
| Şablon satırı | ≥ 480 | < 480 = RED (G6) |
| Şablon boyutu | ≥ 27,0 KB | < 27,0 = RED |
| Kodlama | UTF-8 BOM'suz | BOM / mojibake = RED (G5) |
| `{{…}}` (üretimde) | 0 | §3.9 dışı sızıntı = RED (G4) |
| Kırık bağlantı | 0 | ≥ 1 = RED (G7) |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Sürüm | Kayıt |
|------|---------|-------|-------|
| Matris hücresi `✗` → `✓` | **zorunlu ADR** + test silinmez, güncellenir | `major` | `GATE-002` + log |
| Yeni katman / proje | §3.2 sayacı + §3.4 boyutu yeniden yazılır | `major` | log + Quality Report |
| Yeni port | §3.5'e tek satır + 1 test | `minor` | Quality Report |
| Komut/checkbox düzeltmesi | ilgili hücre | `patch` | tek satır log |
| Kanıt satırı `⚠️` → ✅ | §1.2 kanıt hücresi | `patch` | ölçüm tarihi |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | CoreMusic / versacoder.ai içeriğinden cümle | **DUR** — sil + özgün yaz |
| G2 | ORM bağımlılığı; veri erişimi değilse ADO.NET + Dapper yerine başka katman | **DUR** — `l4`'e taşınır |
| G3 | Frontmatter'sız Markdown (8 alan) | **DUR** — §5.3/6 |
| G4 | Üretilen dosyada `{{…}}` (§3.9 dışı) | **DUR** — temizle, yeniden ölç |
| G5 | Mojibake / BOM / karışık dil | **DUR** — UTF-8 BOM'suz yeniden yaz |
| G6 | < 480 satır (tek muaf: `.templates/CLAUDE.md`) | **DUR** — §6.5 K2 |
| G7 | Bozuk bağlantı | **DUR** — hedefi üret ya da sil |
| G8 | `GATE-003` onaysız ekran/kod | **DUR** — bu dosya kod üretmez |
| G9 | ADR'siz matris değişikliği / kanıtsız sayı | **DUR** — §3.10 satır 6 |
| G10 | SQL'de string concat; `@p1..@pN` yok | **DUR** — `Query-Template` |
| G11 | `TreatWarningsAsErrors=true` + **%90** coverage ihlali | **DUR** — test kırmızı |
| G12 | Kural metninin şablona/envantere kopyalanması | **DUR** — yalnız bağlantı |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ön koşul | `GATE-002` kaydı | onay satırı | architect | 3 dk |
| 2 | Kanıt | §1.2 tablosu | `Test-Path` çıktısı | architect | 5 dk |
| 3 | Katman defteri | kök `CLAUDE.md` B5 | §3.2 dolu | architect | 5 dk |
| 4 | Ok + istisna | §3.3 | tek ok + 2 istisna | architect | 4 dk |
| 5 | Matris | §3.4 | 64 hücre | architect | 12 dk |
| 6 | Port + sınır | §3.5 · §3.6 · §3.7 | 3 tablo | architect | 10 dk |
| 7 | Test | §3.8 | 27 test | developer | 25 dk |
| 8 | Doğrulama | §5.3/1–6 · §6.1 | 0 satır · 21/21 | reviewer → orchestrator | 12 dk |
| | **Toplam** | | | | **≈ 76 dk** |

**Zaman bütçesi:** 1–4 ≈ 17 dk · 5–6 ≈ 22 dk · 7 ≈ 25 dk · 8 ≈ 12 dk. **90 dakikadan uzun** sürüyorsa §2.2'ye dön: katman şartnamesi ya da port kodu bu şablona sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Eksik hücre | matris 60 hücre | satır/sütun atlanmış | 8×8 tamamla |
| 2 | `✓` ile `✗` karışık | aynı hücre iki yerde farklı | tarif matrisi ezmiş | §3.3/§3.4 çelişki kuralı |
| 3 | Port yanlış katmanda | `l2` içinde `IPesRepository` | §3.5 atlanmış | porta `l0`'a taşı |
| 4 | Domain'de UI/DB `using` | `l1` dosyasında ikinci `using` | §3.6 denetlenmemiş | bağımlılığı `l0`'a indir |
| 5 | DevExpress sızması | `VersaCoder.Cli` `csproj`'inde DevExpress | §3.7 kural 1–6 | kaydı `l7`'ye taşı |
| 6 | Test eksik | 27 `✗` hücre, 12 test | §3.8 parçalanmış | kalan 15 testi yaz |
| 7 | Kanıtsız sayı | "%90 coverage" ama `tests/` yok | §1.2 atlanmış | `⚠️` yaz, uydurma |
| 8 | Bozuk bağlantı | olmayan hedefe verilmiş bağlantı | `.ai/architecture/` dizini yok (§1.2 E6) | bağlantıyı sil (G7) |
| 9 | Envanter erken güncelleme | `Durum = ÜRETİMDE` | §4.1/4 atlanmış | tek geçişte `YAZILDI` |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; `src/` yoksa dürüstçe `DIZIN YOK` yazar, **uydurma satır üretmez**.

```powershell
# 1) Baglanti denetimi — bu dosyadaki tum hedefler diskte var mi (kirik = 0)
$doc = 'C:\versacoder\.ai\.templates\architecture\dependency-rule-template.md'
$base = Split-Path $doc
$targets = Select-String -Path $doc -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$broken = $targets | Where-Object { -not (Test-Path (Join-Path $base $_)) }
'Toplam baglanti {0} · kirik {1} (hedef 0)' -f @($targets).Count, @($broken).Count
$broken

# 2) Olcum — satir / KB (hedef >= 480 satir, >= 27,0 KB)
$f = Get-Item $doc
'{0} satir · {1:N1} KB  (esik 480 · 27,0)' -f (Get-Content $doc).Count, ($f.Length / 1KB)

# 3) Ters referans — Domain/Abstractions icinde yukari bakan using (hedef 0 satir)
$src = 'C:\versacoder\src'
if (-not (Test-Path $src)) { 'DIZIN YOK -> src yok, tarama 0 satir' } else {
  Get-ChildItem "$src\VersaCoder.Domain","$src\VersaCoder.Abstractions" -Recurse -Filter *.cs -EA SilentlyContinue |
    Select-String -Pattern 'using\s+VersaCoder\.(Infrastructure|Protocol|Host|Desktop|Cli|Application)' |
    ForEach-Object { '{0}:{1}  {2}' -f $_.Path, $_.LineNumber, $_.Line.Trim() } }

# 4) l2 x l4 dogrudan ok + DevExpress siniri (hedef 0 satir)
if (-not (Test-Path $src)) { 'DIZIN YOK -> src yok, tarama 0 satir' } else {
  Get-ChildItem "$src\VersaCoder.Application" -Recurse -Filter *.cs -EA SilentlyContinue |
    Select-String -Pattern 'using\s+VersaCoder\.Infrastructure' |
    ForEach-Object { 'L2->L4 IHLAL -> {0}:{1}' -f $_.Path, $_.LineNumber }
  Get-ChildItem $src -Recurse -Filter *.cs -EA SilentlyContinue |
    Where-Object { $_.FullName -notlike '*VersaCoder.Desktop*' } |
    Select-String -Pattern 'using\s+DevExpress' |
    ForEach-Object { 'DEVEXPRESS SINIR IHLALI -> {0}:{1}' -f $_.Path, $_.LineNumber } }

# 5) Architecture testi kaniti — proje/dis dosya var mi
$at = @()
if (Test-Path $src) { $at = Get-ChildItem $src -Recurse -Filter '*.csproj' -EA SilentlyContinue |
    Where-Object { $_.Name -match 'Architecture|Tests' } }
if ($at.Count) { $at | ForEach-Object { $_.FullName } }
else { 'ARCHITECTURE TEST YOK -> kanit uretilmedi (src de yok)' }

# 6) Kodlama + yer tutucu — BOM / mojibake / {{...}} sayaci
$b = [System.IO.File]::ReadAllBytes($doc); $raw = Get-Content $doc -Raw
$bom = if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB) { 'VAR (RED)' } else { 'YOK' }
$moji = if ($raw -match '\xC3[\x80-\xBF]\xC3[\x80-\xBF]|\uFFFD') { 'VAR (RED)' } else { 'YOK' }
$ph = (Select-String -Path $doc -Pattern '\{\{[A-Za-z0-9_]+\}\}' -AllMatches |
       ForEach-Object { $_.Matches }).Count
'BOM: {0} · Mojibake: {1} · Yer tutucu: {2}' -f $bom, $moji, $ph
```

**Geçme kriteri:** #1 kırık **0** · #2 `≥ 480` satır ve `≥ 27,0 KB` · #3–#5 **0 satır** (ya da dürüst `DIZIN YOK`) · #6 `BOM: YOK` · `Mojibake: YOK`.

### §5.4 Quality Report

```markdown
## Quality Report — dependency-rule-template (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (şablon) | <satır> satır / <KB> KB | ≥ 480 · ≥ 27,0 | 🟢/🔴 |
| Checklist §6.1 | n/21 | 21/21 | 🟢/🔴 |
| Komut §5.3 (#1–#6) | n satır çıktı | 0 (+ dürüst DIZIN YOK) | 🟢/🔴 |
| Matris | n/64 hücre · `✗` n | 64 · 27 | 🟢/🔴 |
| Architecture test | n/27 | 27 | 🟢/🔴 |
| Yer tutucu / mojibake / BOM | n/n/n | 0/0/0 | 🟢/🔴 |
| Kırık bağlantı | n | 0 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (21 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan + `authority: SSOT` | 8/8 · `SSOT` | §5.3/6 |
| K2 | `category: templates/architecture` | birebir | frontmatter |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (7 bağlantı) | 1 satır | üst kısım |
| K4 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K5 | §1.2 disk kanıtları + `⚠️` + yorum yasağı | var | §1.2 |
| K6 | `§2.1–§2.2` dolu | 2/2 | §2 |
| K7 | `§3.1–§3.11` — 11 alt bölüm | 11/11 | §3 |
| K8 | Katman defteri `l0..l7` + test = 9 satır | 9 | §3.2 |
| K9 | Yön oku `l7→l6→l5→l4→l2→l1→l0` yazılı | tek satır | §3.3 |
| K10 | `l3` yalnız `l0` + `l2↔l4` ok yok | 2 istisna | §3.3, §3.5 |
| K11 | Matris 8×8 = 64 hücre, dört değerli | 64 | §3.4 |
| K12 | `✗` hücre sayısı | 27 | §3.4.2 |
| K13 | Domain'in 6 yasağı (UI/DB/ORM/DevExpress/CLI/MCP) | 6 | §3.6 |
| K14 | Architecture test = 27 (`✗` başına 1) | 27 | §3.8 |
| K15 | DevExpress = Presentation sınırı (6 kural) | 6 | §3.7 |
| K16 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K17 | `§5.1` 8 adım + zaman bütçesi | 8 + ≈76 dk | §5.1 |
| K18 | `§5.2` ≥ 8 hata modu | 9 | §5.2 |
| K19 | `§5.3` 4–6 salt-okunur komut | 6 | §5.3 |
| K20 | `§6.1` ≥ 17 madde | 21 | bu tablo |
| K21 | `§7.2` ≥ 10 satır · kırık link · mojibake | ≥ 10 · 0 · 0 | §7 · G5 · G7 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Neden Red | Aksiyon |
|---|--------|-----------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | gerçek ihlal | gösterilen dosya düzeltilir |
| R2 | Matris ≠ 8×8 ya da `✗` ≠ 27 | sözleşme kırıldı | §3.4'ten yeniden yazılır |
| R3 | `l1` dosyasında ikinci VersaCoder `using` | §3.6 ihlali | bağımlılık `l0`'a iner |
| R4 | `using VersaCoder.Infrastructure` (Application'da) | `l2 × l4` ihlali | port `l0` (§3.5) |
| R5 | `l0`–`l6` içinde `using DevExpress` | §3.7 ihlali | `l7`'ye taşınır |
| R6 | Matris değişmiş ama ADR yok | G9 | `ADR-NNN-arch-*` açılır |
| R7 | `{{…}}` §3.9 dışında | G4 | temizlenir, yeniden ölçülür |
| R8 | BOM / mojibake / kırık bağlantı | G5 / G7 | UTF-8 BOM'suz yeniden yazılır |
| R9 | Envanter `Durum = ÜRETİMDE` | §1.1 M3 | `YAZILDI`/`PLANLI`/`MEVCUT` |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Yalnız ifade/çeviri düzeltmesi | `version` patch, `updated` değişir |
| Matris hücresi değişimi (`✗`↔`✓`) | **zorunlu ADR** + `version` major + 1 test |
| Yeni port satırı (§3.5) | `version` minor + yeni test |
| Yeni katman (§3.2 sayacı) | `version` major + matris yeniden yazılır |
| Architecture testi silinmesi | **RED** — kanıt kaybı, geri alınır |
| DevExpress sürümü talebi | **RED** — ADR-001 kanıtı §1.2 E12'de `⚠️` |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/architecture/dependency-rule-template.md` | **bu dosya** — bağımlılık kuralının tek kaynağı |
| `.ai/.templates/architecture/layer-spec-template.md` | katmanın şartnamesi (ne yaptığı) |
| `.ai/.templates/architecture/architecture-template.md` | katman yığını + proje ağacı |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi ve ölçüm standardı |
| `.ai/.templates/backend/repository-template.md` | port + uygulama kodu (§3.5) |
| `.ai/.templates/index.md` | envanter satır #15 + §3.7 kalite sözleşmesi |
| `../../../CLAUDE.md` | B5 katman listesi — çelişkide geçerli |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Envanter satır #15 `Durum` değeri | geçersiz `ÜRETİMDE` tespit edilip `YAZILDI`'ye çekilecek | `index.md` §3.3 |
| K2 | Bu şablonun satır aralığı | ≥ 480 | §5.3/2 |
| K3 | Bu şablonun KB aralığı | ≥ 27,0 | §5.3/2 |
| K4 | `.templates/architecture/` klasöründe 4 şablon | 4 | `search_files *.md` |
| K5 | `.templates/` toplamı | 38 + 1 = **39 MD** | §1.2 E2 |
| K6 | Yer tutucu yalnız §3.1/§3.9 (+ §5.4 kalıp) | §3.9 dışı sızıntı 0 | §5.3/6 |
| K7 | Kırık bağlantı | 0 | §5.3/1 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-10-05) |
|--------|-----|---------|---------------------|
| Şablon envanteri | [index](../index.md) | satır #15 · §3.7 sözleşmesi · §4.5 G1..G12 | ✅ 38 MD içinde |
| Katman yığını | [architecture-template](architecture-template.md) | §3.2 ile çelişki testi | ✅ |
| Katman şartnamesi | [layer-spec-template](layer-spec-template.md) | §1.3 sınırı | ✅ |
| Diyagram standardı | [ascii-diagram-template](ascii-diagram-template.md) | §1.4 / §3.3 kutuları | ✅ |
| Port kodu | [repository-template](../backend/repository-template.md) | §3.5 örneği | ✅ |
| Form sınırı | [form-template](../frontend/form-template.md) | §3.7 (form repository görmez) | ✅ |
| Kök kural | [CLAUDE](../../../CLAUDE.md) | B5 katman listesi | ✅ |
| Hedef dosya | `.ai/architecture/dependency-rule.md` | üretilcek çıktı | ❌ YOK → §5.1 |
| Architecture test | `tests/**/ArchitectureTests` | 27 test | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Matrisi "yaklaşık" çizmek (6 hücre eksik) | denetlenebilir değil | 8×8 = 64 hücre, dört değer (§3.4) |
| 2 | `l2 → l4` yazmak "şimdilik" | port `l0`'da unutulur | §3.5 — ikisi de yalnız `l0`'a bakar |
| 3 | `l3`'ü herkese açmak | gizli 9. katman doğar | yalnız `l0` + `†` notu, ADR şart (§3.4.1) |
| 4 | Domain'e `using VersaCoder.Infrastructure` | çekirdek teknolojiye bağlanır | §3.6 — tek kabul `VersaCoder.Abstractions` |
| 5 | DevExpress'i `csproj` genellemek | derleme tüm katmanlara bulaşır | §3.7 — yalnız `VersaCoder.Desktop` |
| 6 | Test yazmadan "matris doğru" demek | kural iddiada kalır | 27 test = 27 `✗` hücre (§3.8) |
| 7 | Kuralı `.ai/.rules/`'tan kopyalamak | iki gerçek kaynağı doğar (G12) | yalnız bağlantı, metin bu şablonda |
| 8 | ADR'siz `✗` → `✓` yapmak | matris koda göre esnetilir | §3.10 satır 6 — önce ADR (G9) |
| 9 | `src/` yokken "temiz" demek | kanıtsız beyan | §5.3 dürüst `DIZIN YOK` yazar |
| 10 | Envantere erken `YAZILDI` yazmak | çift kayıt (§4.1 M4) | tek geçiş, şablon bittikten sonra |
| 11 | String concat SQL / parametresiz sorgu | G10 ihlali | `@p1..@pN` + indeks notu (`Query-Template`) |
| 12 | Mojibake / BOM ile kaydetmek | UTF-8 bozulur (G5) | §5.3/6 `BOM: YOK` |

**SSUT beyanı:** Bu dosya bağımlılık kuralının **tek** üretim kaynağıdır; `.ai/architecture/dependency-rule.md` yalnız bu iskeletten ve `GATE-002` onayından doğar. Çoğaltılan her satır ikinci gerçek kaynağı doğurur (envanter G12).
