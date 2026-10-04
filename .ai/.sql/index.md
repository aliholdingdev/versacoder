---
title: "SQL / Veritabanı Notları İndeksi (`.sql/`)"
type: index
category: sql
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: ".sql/ dizininin navigasyonu: var olan SQL/migration notları, hangi kural setinden türüdüğü, nereye bakılır — şema uydurmak yok, kanıt yoksa DOĞRULAMA GEREKLI"
---

# `.sql/` — SQL ve Veritabanı Notları İndeksi

**Zorunlu Bağlantılar:** [index](../index.md) · [keys](../keys.md) · [data-access-rules](../.rules/data-access-rules.md) · [rules index](../.rules/index.md) · [ADR-005](../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [spec index](../spec/index.md)

> **Tek cümlelik sorumluluk:** Bu dizinde SQL ile ilgili hangi notun yaşadığı ve hangi kural setine bağlı olduğu sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`.sql/` **yeni ve boştur**; ilk not bu dosyadır. VersaCoder'ın veritabanı kararları bu dizinde
**yeniden tanımlanmaz** — hepsi [data-access-rules](../.rules/data-access-rules.md) (DA-01..DA-16) ve
[ADR-005](../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) üstünde yaşar. Buradaki iş,
onların **saf SQL karşılıklarını** (migration şablonu, PRAGMA seti, sürüm tablosu) tutmaktır.

| Kural | Karar | Kaynak |
|---|---|---|
| ORM | **EF Core yasak**; yol = ADO.NET + Dapper | ADR-005 / DA-02 |
| SQL yazımı | yalnız **parametreli**; concat/interpolasyon yasak | DA-03 |
| Şema değişikliği | yalnız migration dosyasında (`V0001__ad.sql`) | DA-07 / DA-11 |
| Sürüm tablosu | `__Migrations` + SHA-256 checksum (fail fast) | DA-11 |
| Provider | `IDbConnectionFactory` + `ISqlDialect`; somutu yalnız factory | DA-04 |

> **CONFLICT DETECTED (devam ediyor):** `__Migrations` (kanonik, bu dosya dahil her yerde) ile
> `CLAUDE.md` G2 / `keys.md` §3/28'deki `schema_version` ifadesi arasındaki fark kapatılmadı —
> [data-access-rules](../.rules/data-access-rules.md) §2 uyarısında işaretlendiği gibi hizalama
> ayrı iş kalemidir. Bu dosya tek başına karar **vermez** (G10).

---

## §1 Dizin Envanteri

| # | Dosya | Ne içerir | Durum |
|---|---|---|---|
| 1 | `index.md` (bu dosya) | navigasyon + karar özeti + ekleniş kuralı | active |
| 2 | `migration-V0001-init.sql` | `__Migrations` sürüm tablosu + PRAGMA seti (yalnız `DOĞRULAMA GEREKLI` taslak) | **henüz yok** |
| 3 | `migration-V0002__*.sql` | ilk uygulama şeması (`Sessions`, `Messages`) | **henüz yok** |

- **Eklenen her dosya = bu tabloya 1 satır** ve ad kuralına uyar: `V<4 hane>__<english-kebab>.sql` (DA-11).
- Henüz yazılmamış satırlar **plan**tır; varlıklar olarak sunulmaz.

---

## §2 Buraya ne konur, ne konmaz

| Konur (`.sql/`) | Konmaz (nerede) |
|---|---|
| Saf SQL migration dosyaları (`V00xx__*.sql`) | Sorgu *tasarımı* tartışması → [spec](../spec/index.md) |
| PRAGMA / diyalekt karşılaştırma notları | Kurallar (DA-01..16) → [data-access-rules](../.rules/data-access-rules.md) |
| Ortam-özel SQL (Sqlite vs SqlServer farkı) | ADO.NET/EF kararı → [ADR-005](../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) |
| Şema dökümü (üretimden alındıysa, tarihli) | Repository kodu → `src/VersaCoder.Infrastructure/` (ADR-028 l4) |
| — | Secret/bağlantı dizesi → [security-rules](../.rules/security-rules.md), açık metin **yasak** |

**Bağlantı dizesi kuralı:** hiçbir `.sql`/`.md` dosyasında açık `Server=...;Password=...` yok;
yalnız ortam değişkeni adı (`VC_DB_PATH` benzeri) — ayrıntı `DOĞRULAMA GEREKLI`.

---

## §3 Ekleniş Akışı (3 adım)

| # | Adım | Çıktı | Kapı |
|---|---|---|---|
| 1 | İhtiyaç doğar (yeni tablo/alan) | yeni `V00xx__ad.sql` (up + `-- down` ya da `-- irreversible`) | DA-11 / DA-12 |
| 2 | Kural denetimi | DA-03 (parametre), DA-07 (şema yerinde), DA-09 (timeout), DA-14 (PRAGMA) | review |
| 3 | Envanter güncelleme | §1 tablosuna 1 satır + `reports/index.md`'ye gerekiyorsa 1 satır | GATE-004a |

- Adım 2 geçmezse dosya vault'a girmez (kanıtsız teslim, G5).
- Checksum uyumsuzluğu = uygulama **başlamaz**; bu bilgi [data-access-rules](../.rules/data-access-rules.md) §3.3 içindir, burada tekrarlanmaz.

---

## §4 Doğrulama

| # | Kontrol | Beklenen |
|---|---|---|
| 1 | Karar tekrarı var mı? | kural metni yalnız `.rules` + ADR-005'te |
| 2 | Açık secret | 0 |
| 3 | EF Core *öneri* olarak geçmiş mi | 0 (yalnız yasak, DA-02) |
| 4 | Bozuk link | 0 (`check-links.ps1`) |
| 5 | Frontmatter 8 alan + `updated: 2026-09-29` | 8/8 |

- [ ] §1'de her gerçek dosya için 1 satır var
- [ ] Uydurma şema/sütun adı yok (kaynaksız = `DOĞRULAMA GEREKLI`)
- [ ] Yeni `.sql` dosyası ad kuralına uyuyor (`V00xx__kebab.sql`)

---

## §5 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Veri erişim kuralları (DA-01..16) | [.rules/data-access-rules.md](../.rules/data-access-rules.md) | şema, parametre, transaction |
| 2 | EF Core yasak kararı | [.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md](../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) | neden Dapper |
| 3 | Kurallar indeksi | [.rules/index.md](../.rules/index.md) | kardeş kural setleri |
| 4 | Testte geçici SQLite | [spec/test-strategy.md](../spec/test-strategy.md) | şema testi |
| 5 | Şablon: migration | [.templates/infrastructure/migration-template.md](../.templates/infrastructure/migration-template.md) | `V00xx` dosya biçimi |
