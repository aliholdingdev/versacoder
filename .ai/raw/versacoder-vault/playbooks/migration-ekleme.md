---
title: "migration-ekleme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Saf SQL migration ekleme - V0xx dosya adi, __Migrations surum tablosu, checksum, up/down cifti; EF Core YASAK, parameterized-only"
---

# migration-ekleme — Saf SQL Migration Ekleme

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [rollback-geri-alma](rollback-geri-alma.md)

> **Tek cumlelik sorumluluk:** *Yeni sema degisikligi hangi dosyada, hangi sirada ve geri nasil alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Migration = **saf `.sql` dosyasi + `__Migrations` surum tablosu**; EF Core migration **YASAK** (ADR-005 / G2). Her sorgu **parameterized** (`@p1`), string-concat SQL **yok**. Sira: numara al -> up yaz -> down cifti -> checksum uret -> yedek -> uygula -> dogrula. Toplam ≈35 dk. Geri alinamayacak `DROP` varsa **onay + yedek zorunlu** (G7).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Sira numarasi bos (`V<4 hane>`) | `SELECT MAX(Id) FROM __Migrations` |
| 2 | Hedef DB + yedek dosyasi | `db/versacoder.db.bak` boyutu > 0 |
| 3 | Kural okunmus (parameterized, idempotent) | `.ai/.rules/data-access-rules.md` DA-11/DA-12 |
| 4 | EF Core referansi yok | `Select-String -Path '*.csproj' -Pattern EntityFramework` -> 0 satir |
| 5 | Down (geri alma) yazilabilir | her up icin `*.down.sql` ya da `-- irreversible` gerekcesi |

---

## §2 Adimlar

**Toplam ≈35 dk** · yapan: `developer` (denetim: `reviewer`)

1. **Numara + dosya adi al** — `V0001__<kebab-ad>.sql` ve `V0001__<kebab-ad>.down.sql`; ayni numarayi iki dosya paylasamaz.
   - Klasor: `C:\versacoder\db\migrations\`
   - Cikti: bosluklu/Turkce karakter icsiz iki dosya

2. **Up SQL'i yaz** — `IF NOT EXISTS` her CREATE'te, deger **hic** metne girmez; kolon/tablo adi allowlist.
   - Ornek: `SELECT Id FROM Sessions WHERE Title = @p1;` (Dapper `DynamicParameters`)
   - Yasak: `$"SELECT ... {deger}"`, `string.Format` + SQL, `DbContext`

3. **Down ciftini yaz** — ileri adimin tersi; `DROP` icermiyorsa `-- irreversible` + gerekce.
   - Dosya: `V0001__<ad>.down.sql`
   - Cikti: tersten calistirilinca sema eski haline doner

4. **Surt + checksum kaydet** — SHA-256 dosya iceriginden; uygulanmis dosya sonradan degistirilirse app **baslamaz** (fail fast).
   - Tablo: `__Migrations (Id, Name, Checksum, AppliedAt)` — `AppliedAt` UTC
   - Cikti: `SELECT Name, Checksum FROM __Migrations` satiri

5. **Yedek al + uygula + dogrula** — tek transaction; migration suresi `CommandTimeout` 300 sn, `0` yasak.
   - Komut: `pwsh -File C:\versacoder\.ai\scripts\check-links.ps1` (vault sagligi) + DB icin `PRAGMA table_info(<tablo>)`
   - Cikti: yeni kolon/liste gorunur, app acilis hatasi vermez

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Dosya cifti | `db\migrations\` altinda up + `*.down.sql` ayni numarayla |
| K2 | Surum tablosu | `SELECT MAX(Id) FROM __Migrations` -> yeni numara |
| K3 | Checksum | dosya SHA-256'si = kayitli `Checksum` |
| K4 | Semada degisiklik | `PRAGMA table_info(<tablo>)` -> yeni kolon |
| K5 | EF / concat taramasi | 0 satir (`EntityFramework` ve `"\s*\+\s*\w*(Id\|Name)"`) |

**Ornek cikti (K4):**

```text
cid  name        type     notnull  dflt_value  pk
0    Id          INTEGER  1        NULL        1
1    Title       TEXT     1        NULL        0
2    CreatedAt   TEXT     1        NULL        0
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Uygulanmis up | `V0001__<ad>.down.sql`'i **tersten** calistir (onceki numaralardan basa dogru) |
| 2 | Down'suz (irreversible) | yedekten donus: `db\versacoder.db.bak` geri yuklenir (G7 onayi alinir) |
| 3 | `__Migrations` satiri | adim 1 ile ayni transaction'da `DELETE FROM __Migrations WHERE Id = @p1` |
| 4 | Dosya henuz uygulanmadi | up + down dosyalarini sil, numarayi bosalt |
| 5 | Checksum uyusmazligi | dosyayi **geri al** (`git checkout -- V0001__*.sql`); App'i eski dosyayla baslat, yeni migration ac |

