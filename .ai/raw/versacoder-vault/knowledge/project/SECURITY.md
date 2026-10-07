---
title: 'Versa Coder - SECURITY: Security by Design, Vault, OWASP, Onay ve Audit'
source: 'USER-BOOTSTRAP §9 (Root Documentation); versacoder proje planaması ai.md'
date: '2026-09-24'
status: 'ACTIVE'
confidence: 'SOURCE_VERIFIED'
related_decisions: ['ADR-001', 'ADR-003']
related_requirements: ['REQ-SEC-001', 'REQ-SEC-002', 'REQ-SEC-003']
---

# 1. Amaç ve Kapsam

Bu dosya Versa Coder'ın **güvenlik politikasını** tanımlar: security by design, sır saklama (credential vault + AES-256-GCM), insan onayı kapıları, denetim izi (audit trail), parametreli SQL zorunluluğu, OWASP kontrol listesi.

Kapsam: üretim kodu, veri katmanı, yapılandırma, paket tedariki, depo hijyeni, olay müdahalesi. Kapsam dışı: ağ/dağıtım güvenliği (dağıtım yok), fiziksel güvenlik; kimlik doğrulama **modeli** henüz gereksinimde yok (`UNKNOWN`, P0 — Bölüm 10).

Neden bu dosya var: güvenlik kod yazıldıktan sonra **eklenemez**; ayrıca `.gitignore` 2026-09-24'te **yok** (`REPOSITORY_VERIFIED`) — depo ilk commit'e sırla girebilir. Kurallar ilk commit'ten önce sabittir.

İçindekiler: 1 Amaç / 2 İlkeler / 3 Parametreli SQL / 4 Vault / 5 Sır Saklama / 6 Onay Kapıları / 7 Audit Trail / 8 OWASP / 9 Girdi / 10 Tarama / 11 Olay / 12 Doğrulama.

# 2. İlkeler

| # | İlke | Ne demek | Nerede |
|---|---|---|---|
| 1 | En az yetki | gerektiği kadar erişim, fazlası değil | `Data`, dosya yolu |
| 2 | Varsayılan güvenli | açık/serbest seçenek **yazılmaz** | yapılandırma |
| 3 | Savunma derinliği | parametre + doğrulama + yetki birlikte | Bölüm 9 |
| 4 | Açıklanmama | sır **hiçbir yere** yazılmaz (kod, `.resx`, log, mesaj) | Bölüm 5 |

**Güvenlik gereksinimdir, tercih değil** (P0 sınıfı); cevabı bilinmeyen güvenlik sorusu açıkken ilgili kod **yazılmaz**: (1) bu ekran/akış kimin verisini gösteriyor? (2) sır/parola nerede saklanacak? (Bölüm 4) (3) dışarıya (dosya/pano/ağ) hangi veri çıkıyor?

Sahiplik: politikayı `security-engineer` yazar; `developer` kuralı koda uygular; `reviewer` güvenlik açığını **kapatmadan** geçer not veremez; sub-agent yalnız öneri üretir (`.ai/.decisions/` altına **yazamaz**).

# 3. Parametreli SQL Zorunluluğu

## 3.1 Kural ve yasaklar

Tüm SQL **parametreli** yazılır: `@p1..@pN` yer tutucuları, değerler komutun **parametre collection'ı** üzerinden gider ("PDO gibi"). Değerin SQL metnine birleştirilmesi **kesin yasaktır**.

| Yasak | Karşılığı |
|---|---|
| Değeri SQL metnine birleştirmek (`$` / `string.Format` / `+`) | `@p1..@pN` |
| Dinamik sorguda kullanıcı belirlediği ad (tablo/sütun) | beyaz liste + sabit ad |
| Ekrandan doğrudan SQL (`UI` → `Data`) | `IRepository<T>` üzerinden |
| Ham veriyi yalnız arayüzde doğrulamak | `Core`/`Data` tarafında da doğrulama |
| "Geçici" olarak doğrulamayı boş bırakmak | yok; P0 açıkken kod yazım |

## 3.2 Neden

`VersaCoder.Cli` ve `Desktop` aynı veri katmanını paylaşır; parametreleme tek noktada (Dapper sorgusu) uygulanır ve **her iki arayüz** için geçerlidir. EF Core **yasak** olduğundan (net48 desteklemiyor) otomatik parametreleme katmanı yoktur: parametreleme **yazarın** sorumluluğudur ve kod incelemesinde denetlenir.

# 4. Credential Vault ve AES-256-GCM

## 4.1 Kural

Sır verisi (API anahtarı, token, parola, bağlantı dizesi anahtarı) **credential vault** içinde tutulur; vault dışı düz metin saklama **yasaktır**.

| Alan | Değer |
|---|---|
| Şifreleme | **AES-256-GCM** (şifreleme + doğrulama; GCM etiketi bozulma tespiti verir) |
| Kontrat | `ICredentialVault` (`ARCHITECTURE.md` Bölüm 5) |
| Anahtar | veri anahtarı, anahtarla (key encryption key) korunur; anahtar **kodda tutulmaz** |
| Kullanım | `Services` akışında okunur, kullanılır, bellekte bekletilmez |
| Audit | her okuma/yazma denetim izine yazılır (Bölüm 7) |

## 4.2 DOĞRULAMA NOTU (açık)

AES-256-GCM + vault kararı **bu dosyanın kaynağıyla** verilmiştir (`SOURCE_VERIFIED`, `USER-BOOTSTRAP §9`). Anahtar yönetimi (KEK yerleşimi, yenileme, saklama ortamı) henüz **`UNKNOWN`** (P0) ve ADR gerektirir — `.ai/.rules/security.md` Bölüm 4'teki `UNKNOWN` kaydı bu konuyu kapsar. Kimlik doğrulama/parola modeli `UNKNOWN` (P0): gereksinim gelmeden parola karma kodu **yazılmaz**; gereksinim gelirse asgari küme tuzlu karma + sabit süreli karşılaştırma + deneme sınırıdır (algoritma adı ADR ile seçilir).

# 5. Sır Saklama Kuralları

## 5.1 Beş konum

| Konum | Sır bulunabilir mi? |
|---|---|
| Kaynak kod | **Hayır** |
| `.resx` / yapılandırma dosyası | **Hayır** |
| Log dosyası | **Hayır** |
| Hata mesajı / ekran | **Hayır** |
| Ortam değişkeni / vault (erişim sınırlı) | **Uygun** — tek yol |

## 5.2 Tarama

Her commit öncesi (en az oturum sonunda) sır taraması yapılır: kod (bilinen anahtar desenleri; araç seçimi `UNKNOWN` → ADR), diff (yeni eklenen sır benzeri dize), yapılandırma (bağlantı dizesi, token). Araç yokken **manuel tarama zorunludur** ve kanıt oturuma yazılır: `TARAMA: el ile, dosya listesi, 0 bulgu`.

## 5.3 `.gitignore` (ilk commit ön koşulu)

`.gitignore` **yok** (`REPOSITORY_VERIFIED`). Sonuç: ilk commit'te `bin/`, `obj/`, yerel ayar ve **sır dosyaları** repoya girebilir.

```text
[ ] .gitignore ilk commit ONCESI yazildi            (zorunlu)
[ ] sir dosyalari (yapilandirma, yerel ayar) haric
[ ] yedek/snapshot dizini haric (.ai/archives/)
[ ] degisimi yalniz git-engineer + reviewer yapar
```

`.gitignore` yokken **commit atılmaz**. Sır içeren dosya commit edildiyse: commit geri çekilir → **sıra değiştirilir** (geçmişte görülmüş parola artık sır değildir) → olay kayda geçer → `.gitignore` güncellenir. Yedek snapshot'ları da aynı hassasiyeti taşır: yedek sızıntısı = kaynak sızıntısıdır.

# 6. Human-in-the-Loop Onay Kapıları

## 6.1 Ana akıştaki yer

```text
... -> Policy Evaluation -> Workflow -> [ APPROVAL ] -> Agent Execution ...
```

`Approval` **insan onayıdır**; onay olmadan yürütüm başlamaz/tamamlanmaz.

## 6.2 Onay gerektiren işlemler

| # | İşlem | Onay |
|---|---|---|
| 1 | Kapı geçişi (`PASSED` yazma) | **kullanıcı onayı zorunlu** |
| 2 | Kural **gevşetme** / kural dosyası değişikliği | ADR + onay |
| 3 | Yeni paket bağımlılığı (8 adımlı kapı) | onay + ADR |
| 4 | Sır üretimi / vault yazımı / anahtar değişimi | onay + audit |
| 5 | Silme / geri alma (dosya, kayıt) | onay + snapshot |
| 6 | GATE-006'ya giriş | Bölüm 3 ön koşulları + onay |

Onay **kayıtla** verilir (sözlü onay sayılmaz):

```text
ONAY     : <gate ya da islem>
ISTEYEN  : <rol>
ONAYLAYAN: <kullanici ya da master-orchestrator>
TARIH    : <YYYY-MM-DD>
KANIT    : <dosya yolu + komut ciktisi>
```

## 6.3 Policy Evaluation

Yürütüm öncesi `IPolicyEngine` + `ISecurityService` şunları değerlendirir: kim, hangi oturumda, hangi araçla, hangi veriye erişiyor. Sonuç `DENY` ise yürütüm başlamaz; gerekçe denetim izine yazılır.

# 7. Audit Trail

Her güvenlikle ilgili olay şu alanlarla yazılır (`IAuditTrail`, `Data`): `OLAY` (sınıf + tek cümle), `BULGU` (dosya yolu + satır ya da komut + çıktı), `ETKI` (kim etkilendi, ne veri), `MUDAHALE` (ne yapıldı), `DURUM` (`KAPALI`/`ACIK`), `KANIT` (doğrulama komutu + çıktı).

| Kural | Değer |
|---|---|
| Kayıt yeri | `.ai/` altı + oturum kaydı (tek doğruluk = dosya) |
| `DURUM: KAPALI` | **yalnız** `KANIT` ile yazılır |
| Kapsam | kapı onayları, vault erişimi, sır bulguları, yetki ihlalleri |
| Süre saklama | `UNKNOWN` (gereksinim P0) |

# 8. OWASP Kontrol Listesi

2021 sürümüne göre denetim (üretim oturumu başına):

| # | Kategori | Bu projedeki karşılığı | Durum |
|---|---|---|---|
| A01 | Broken Access Control | policy + en az yetki (Bölüm 2, 6.3) | tasarım hazır, kod **yok** |
| A02 | Cryptographic Failures | AES-256-GCM vault (Bölüm 4) | karar var, anahtar yönetimi `UNKNOWN` |
| A03 | Injection | parametreli SQL (Bölüm 3) | kural var, kod **yok** |
| A04 | Insecure Design | mimari + policy (`ARCHITECTURE.md`) | tasarım |
| A05 | Security Misconfiguration | `.gitignore`, feed/CPM eksikleri | **açık iş** |
| A06 | Vulnerable Components | 8 adımlı paket kapısı (Bölüm 10) | tarama aracı `UNKNOWN` |
| A07 | Identification & Auth Failures | kimlik modeli `UNKNOWN` (P0) | **bekliyor** |
| A08 | Software & Data Integrity Failures | kapı kanıtı + onay kaydı (Bölüm 6.2) | süreç hazır |
| A09 | Logging & Monitoring Failures | audit trail (Bölüm 7) | tasarım hazır |
| A10 | SSRF | dış servis yok henüz | kapsam `UNKNOWN` |

# 9. Girdi Doğrulama — Üç Katman

Her girdi **güvenilmezdir** (kullanıcı, dosya, kayıt, dış servis yanıtı; dış servis yanıtı da girdidir ve şeması doğrulanır).

| Katman | Ne yapar | Nerede |
|---|---|---|
| 1. Şekil | biçim / uzunluk / menzil | giriş noktası |
| 2. Anlam | iş kuralı (izin, tutarlılık) | `Core` |
| 3. Parametreleme | SQL injection'ı keser | `Data` |

Sıra değişmez (anlam, biçimden **önce** konamaz); tek katman **yetmez**, üçü birlikte zorunludur. Doğrulamayı yalnız `UI`'da yapmak **yasaktır**: `Cli` aynı `Services` yolunu kullanır. Hata mesajı üç şeyi **içermez**: ham girdi, bağlantı dizesi, sistem/istek ayrıntısı — kullanıcıya iç kuralı mesajı gider, ayrıntı yalnız kayda düşer.

# 10. Bağımlılık ve Tarama

Yeni paket 8 adımlı kapıdan geçer (`.ai/.rules/dependencies.md`); güvenlik kutusu:

| Kapı | Soru | Red nedeni |
|---|---|---|
| Alternatif | çekirdek kütüphane yeterli mi? | gereksiz bağımlılık |
| Sürüm | iki hedefte de erişilebilir mi? | derleme hatası |
| Feed | yerel feed'de var mı? | sürüm uyuşmazlığı (C-ARCH-002) |
| Güvenlik | bilinen açık taraması geçti mi? | açık |
| Lisans | dağıtım uygun mu? | hukuki risk |

Tarama kanıtı **yoksa paket eklenemez** (`RED`). Yerel DevExpress feed'i (985 nupkg) için "yerel olduğu için güvenli" varsayımı **yapılmaz**; muafiyet yoktur. Sürüm sabitlenir (ADR-001 emsal); **floating sürüm yasaktır**.

# 11. Olay Müdahalesi

| Sınıf | Örnek | Müdahale |
|---|---|---|
| S1 | depoda açık sır/parola | hemen: döndür + değiştir + olay kaydı |
| S2 | injection deliği | kod düzeltme + test + inceleme |
| S3 | bağımlılık açığı | sürüm/kaldırma + ADR |
| S4 | şüpheli davranış (bilgi yok) | araştırma → sınıf belirle |

Sınıf belirsizse **en yüksek sınıftan** başlanır; sınıf düşürülebilir, yükseltilir. Aynı açık 3 kez düzeltilemezse **durulur** (`.ai/.rules/workflow.md` Bölüm 7): şüpheli varsayım adlandırılır, `security-engineer` + `master-orchestrator`'a eskale edilir; 4. kez aynı düzeltme **yasaktır**.

# 12. Doğrulama Kriterleri

```text
[ ] Metadata ilk 12 satirda 7 alan; Bolum 1-12 sirasi
[ ] 4 ilke + 3 P0 soru (Bolum 2)
[ ] Parametreli SQL 5 yasak + EF Core yasagi (Bolum 3)
[ ] AES-256-GCM + vault + acik DOĞRULAMA notu (Bolum 4)
[ ] 5 konum + tarama + .gitignore on kosulu (Bolum 5)
[ ] 6 onay maddesi + ONAY kayit formati + policy (Bolum 6)
[ ] Audit 6 alan + KAPALI sarti (Bolum 7)
[ ] OWASP A01-A10 tablosu (Bolum 8)
[ ] 3 katman + hata mesaji 3 yasak (Bolum 9)
[ ] 5 kapi + "muafiyet yok" + floating yasak (Bolum 10)
[ ] S1-S4 + 3 deneme (Bolum 11)
[ ] Uydurma arac/paket/surum adi yok; bilmeyen UNKNOWN
[ ] 160-220 satir araliginda
```

| Tarih | Değişiklik |
|---|---|
| 2026-09-24 | İlk yazım - 4 ilke, parametreli SQL, AES-256-GCM vault (SOURCE_VERIFIED + anahtar yönetimi UNKNOWN), 6 onay maddesi, audit, OWASP A01-A10, 5 kapı, S1-S4; `.gitignore` eksikliği açık iş |
