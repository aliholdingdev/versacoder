---
title: "migration-ekleme - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Saf SQL migration ekleme - V0xx dosya adi, __Migrations surum tablosu, checksum, up/down cifti; EF Core YASAK"
---

# migration-ekleme — Saf SQL Migration Ekleme

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [rules index](../.rules/index.md) · [rollback-geri-alma](../playbooks/rollback-geri-alma.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *Yeni sema degisikligi hangi dosyada, hangi sirada ve geri nasil alinir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Migration = **saf `.sql` dosyasi + `__Migrations` surum tablosu**; EF Core migration **YASAK** (ADR-005 / G2). Her sorgu **parameterized** (`@p1`), string-concat SQL **yok** (ADR-017). Sira degistirilemez: **numara al -> up yaz -> down cifti -> checksum uret -> yedek -> uygula -> dogrula**. Toplam ≈35 dk. Geri alinamayacak `DROP` iceren islemde **onay + yedek zorunludur** (G7).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Sira numarasi bos (`V<4 hane>`) | `SELECT MAX(Id) FROM __Migrations` |
| 2 | Hedef DB + yedek dosyasi | `db/versacoder.db.bak` boyutu > 0 |
| 3 | Kural okunmus (parameterized, idempotent) | [rules index](../.rules/index.md) -> data-access kurallari |
| 4 | EF Core referansi yok | `Select-String -Path '*.csproj' -Pattern EntityFramework` -> 0 satir |
| 5 | Down (geri alma) yazilabilir | her up icin `*.down.sql` ya da `-- irreversible` gerekcesi |

---

## §2 Adimlar

**Toplam ≈35 dk** · yapan: `developer` · denetim: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Numara + dosya adi al | `V0001__<kebab-ad>.sql` + `V0001__<kebab-ad>.down.sql` | 3 dk |
| 2 | Up SQL'i yaz | her CREATE'te `IF NOT EXISTS`; deger metne hic girmez | 12 dk |
| 3 | Down ciftini yaz | ileri adimin tersi; degilse `-- irreversible` + gerekce | 7 dk |
| 4 | Checksum uret + kaydet | SHA-256 dosya iceriginden; `__Migrations` satiri (UTC) | 4 dk |
| 5 | Yedek al + uygula + dogrula | tek transaction, `CommandTimeout` 300 sn; sonra sema sorgusu | 9 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Checksum uyusmazligi | Uygulanmis dosya sonradan degistirilmis | Dosya `git checkout --` ile geri alinir, yeni migration acilir |
| 2 | App acilis hatasi | Yarim kalmis islem | Down tersten calistirilir ya da yedekten donulur |
| 3 | Concat SQL bulgusu | Interpolasyonlu sorgu | Kaynak parameterized hale getirilir (ADR-017) |
| 4 | `DROP` onaysiz calisti | G7 atlanmis | Islem durur, onay + yedek ile yeniden |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Uygulanmis up | `*.down.sql` **tersten** calistirilir (onceki numaralardan basa dogru) |
| 2 | Down'suz (irreversible) | yedekten donus: `db\versacoder.db.bak` (G7 onayi ile) |
| 3 | `__Migrations` satiri | ayni transaction'da `DELETE FROM __Migrations WHERE Id = @p1` |
| 4 | Dosya henuz uygulanmadi | up + down dosyalari silinir, numara bosaltilir |
| 5 | Vault tarafinda kirilan link | [vault-link-taramasi](../playbooks/vault-link-taramasi.md) ile 0'a indirilir |

---

## §5 Kabul Kriteri

- [ ] `db\migrations\` altinda up + `*.down.sql` **ayni numarayla** ikili var
- [ ] `SELECT MAX(Id) FROM __Migrations` yeni numarayi veriyor
- [ ] Kayitli `Checksum` dosyanin SHA-256'si ile esit
- [ ] Sema sorgusu yeni kolonu/tabloyu gosteriyor
- [ ] EF Core ve concat taramalari **0 satir**

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : __Migrations -> Id=7, Checksum eslesiyor |
|          PRAGMA table_info -> yeni kolon gorunur   |
|          EntityFramework taramasi -> 0 satir       |
| DOSYA  : db/migrations/V0007__<ad>.sql (+ down)   |
| EKSIK  : YOK                                       |
| SONRAKI : iki bacak build + test kaniti (~15 dk)   |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | EF Core | `Select-String -Path '*.csproj' -Pattern EntityFramework` | 0 satir |
| 2 | Concat | `Select-String -Path 'src\**\*.cs' -Pattern '"\s*\+\s*\w*(Id\|Name)'` | 0 satir |
| 3 | Vault sagligi | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈35 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Tam rollback proseduru | [rollback-geri-alma](../playbooks/rollback-geri-alma.md) |
| 2 | Veri erisim katmani kodu | ADR-028 katmanlari + [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |
| 3 | Kural metni (neden parameterized) | [rules index](../.rules/index.md) |
| 4 | Yeni paket / ORM ekleme | [nuget-surum-yukseltme](../playbooks/nuget-surum-yukseltme.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `spec/migration-spec.md` | surum tablosu, checksum ve dosya adi sozlesmesi |
| 2 | `checklists/migration-check.md` | migration kontrol sorulari |
| 3 | `.rules/index.md` | parameterized-only ve EF Core yasagi kural dosyalari |
| 4 | `playbooks/index.md` | adim + sure + geri alma kabul kriteri |
