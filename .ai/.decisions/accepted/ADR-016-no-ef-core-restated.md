---
title: "ADR-016 — Veri Erişimi Sınırları (ADR-005'i Tamamlayan Kapsam Notu)"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "hangi katmanın veriye nasıl erişebileceği, hangi yolların kapsam dışı olduğuna dair kapsam genişletme notu — ADR-005 supersede edilmez"
---

# ADR-016 — Veri Erişimi Sınırları (ADR-005'i Tamamlayan Kapsam Notu)

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-017](ADR-017-parameterized-sql-mandatory.md)

> **Tek cümlelik karar:** [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) **geçerliliğini korur ve superseded edilmez**; bu ADR yalnız **kapsam sınırını** ekler: veriye erişim **yalnız `l4 ado-dapper`** üzerinden, başka hiçbir katman `DbConnection` tutamaz — karar verildi.

---

## §0 Özet (Answer First)

ADR-005 **hangi yığını** kullanacağımızı söyler (ADO.NET + Dapper, EF Core yok, parametreli SQL, saf SQL migration). Ama "hangi katman erişebilir" sorusunu bırakır; bu ADR o boşluğu kapatır. Kural basit: `DbConnection`, `DbCommand`, connection string ve migration **yalnız `l4 Infrastructure / ado-dapper`** altında yaşar; `l1`/`l2`/`l5` veriye **yalnız `l0` portları** (interface) üzerinden bakar. Bu bir **supersede değil, genişletmedir**: ADR-005 dosyası ve durumu değişmez.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Yığın kararı verildi: ORM yok, Dapper, parametreli, saf SQL migration | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 1–4 |
| C2 | Sağlayıcı ve sürümler ayrıca sabitlendi (SqlClient 5.x / Sqlite test) | [ADR-012](ADR-012-dual-tfm-data-provider.md) D1–D4 |
| C3 | `l4 Infrastructure` altında `ado-dapper` · `sqlite-provider` · `sqlserver-provider` · `migrations` düğümleri ayrılmış | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 (N25–N28) |
| C4 | Bağımlılık yönü tek yönlü; geri dönüş yalnız `interface` (l0) | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2.3 |
| C5 | String-concat SQL kod incelemesinde RED'dir | [ADR-017](ADR-017-parameterized-sql-mandatory.md) |
| C6 | Aynı bilgi iki dosyada olursa biri yanlıştır (SSUT) | `CLAUDE.md` (SSOT) · `adr-index.md` §1.3 |

---

## §2 Karar

**Karar:** Veri erişimine **katman sınırı** konur: `DbConnection`/`DbCommand`/connection string/migration **yalnız `l4`**, üst katmanlar **yalnız `l0` portları** ile okur-yazar; ADR-005'in hiçbir kararı **değişmez** — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Sahip katman | `VersaCoder.Infrastructure` (l4) — bağlantı, komut, migration | C3 |
| D2 | Üst katman erişimi | `l1`/`l2`/`l5` → yalnız `l0` interface'i (`IOrderRepository` benzeri port) | C4 |
| D3 | Yasak | `l2`/`l5` içinde `DbConnection`, `SqlCommand`, `SELECT` dizesi = **0 satır** | C1 · C4 |
| D4 | SQL | her sorgu `@p1..@pN` (bkz. [ADR-017](ADR-017-parameterized-sql-mandatory.md)) | C1 |
| D5 | Migration | saf SQL + `schema_version`; migration da yalnız `l4` | C1 |
| D6 | Test | entegrasyon testi gerçek şema üzerinde (SQLite, yalnız `net10.0-windows`) | [ADR-012](ADR-012-dual-tfm-data-provider.md) D3 |
| D7 | İlişki biçimi | bu ADR **`extends` ADR-005**; `supersedes` alanı **boş** | C6 |

**Neden:** Yığın doğru olsa bile **kapsam sızarsa** her katman kendi sorgusunu yazar, N+1 ve tekrarlayan SQL kaçınılmaz olur; ayrıca NetArchTest'in katman kuralı veri tipine tutunamaz, portlara tutunur. Kapsamı ayrı yazmak, ADR-005'in kanıt satırlarına dokunmadan denetlenebilir bir kontrol noktası ekler. Supersede **kullanılmaması** bilinçlidir: iki karar çelişmiyor, biri diğerini **tamamlıyor**.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Erişim sınırı statik olarak denetlenir | NetArchTest: `l2/l5 → l4` bağımlılığı = 0 ihlal |
| ADR-005 değişmedi → kanıt zinciri bozulmadı | `supersedes` alanı boş, dosya `accepted` |
| N+1 / tekrarlı SQL riski azalır | SQL yalnız `l4`'te; kod taraması tek klasör |
| Yeni repository eklemek tek şablon | `l4/ado-dapper` kalıbı + port `l0` |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Basit bir okuma için bile port + implementasyon = 2 dosya | yalnız **gerçekten** paylaşılan sorgular için port açılır |
| Portlar şişebilir (god interface) | port **tesisat başına** (session · tool · config) |
| Sınır ihlali kod review'da kolay kaçabilir | NetArchTest zorunlu + [ADR-019](ADR-019-dual-tfm-test-matrix.md) matrisi |
| Kapsam notu, ADR-005'in kabul kriterleriyle karıştırılabilir | D7 açıkça `extends` der |

**NEEDS_VALIDATION:** `l4` içindeki portların (`l0`) nihai adları ve granülaritesi PART 6'da ilk repository ile belirlenir; adlandırma `spec/test-strategy.md` ile birlikte teyit edilir. Kapsam **sınırı** (bu ADR) değişmez, yalnız port adları netleşir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | extends | yığın kararı buradan **ayrılmaz**; bu ADR kapsam ekler |
| [ADR-017](ADR-017-parameterized-sql-mandatory.md) | related | SQL yazım kuralı + inceleme RED'i |
| [ADR-012](ADR-012-dual-tfm-data-provider.md) | related | sağlayıcı/sürüm ve SQLite test kapsamı |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | `l4`/`l0` düğüm ve bağımlılık yönü |
| [ADR-019](ADR-019-dual-tfm-test-matrix.md) | related | sınırın test edildiği matris |

---

## §5 Kabul Kriterleri

- [ ] ADR-005 dosyası **değişmedi** (`git status` içinde `ADR-005-*.md` yok)
- [ ] `l1`/`l2`/`l5` içinde `DbConnection|DbCommand|SqlCommand` = **0 satır**
- [ ] NetArchTest: üst katman → `l4` bağımlılığı **0** ihlal
- [ ] Migration dosyaları yalnız `l4` projesine gömülü
- [ ] Bu ADR `adr-index.md`'de **extends** olarak işlendi (supersede değil)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-005 kapsam boşluğu + ADR-013 katman kuralı.
