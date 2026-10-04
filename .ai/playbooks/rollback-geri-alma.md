---
title: "rollback-geri-alma - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Hatali bir degisikligi guvenle geri alma - kod / saf SQL / konfigurasyon icin sirali prosedur ve kanit"
---

# rollback-geri-alma — Geri Alma (Rollback)

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [migration-ekleme](../playbooks/migration-ekleme.md) · [guvenlik-denetimi](../playbooks/guvenlik-denetimi.md)

> **Tek cumlelik sorumluluk:** *Bozuk bir degisikligi hangi sirayla, kanitla geri alirim* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Rollback **3 katmanda** yapilir ve sirasi **degistirilemez**: (1) **calisan kodu durdur / yayindan kaldir**, (2) **veri semasini geri al** (saf SQL + surum tablosu; EF Core migration DEGIL), (3) **kodu git ile geri al**. Once etkiyi kisitla, sonra geri al; her adim `log.md`'ye tarihle yazilir. Kanitsiz "geri aldik" iddiasi yasaktir (G5); yikici `git reset` yerine `git revert` kullanilir (G7).

---

## §1 Karar Agaci

```text
HATA TESPIT EDILDI
  |
  +-- canli veri kaybi riski var mi? -- EVET -> ONCE yedek al (adim 0)
  |
  +-- hizli geri alinabilir mi? ------ EVET -> git revert (adim 3) + yayindan kaldir (adim 1)
  |
  +-- sema degisti mi? --------------- EVET -> SQL rollback (adim 2)
  |
  +-- hepsi HAYIR -------------------------> yalniz kodu geri al (adim 3), log'a yaz
```

---

## §2 Adimlar

**Toplam ≈40 dk** · yapan: `developer` · denetim: `reviewer` · onay: kullanici (yikici adim)

| # | Adim | Komut / Cikti | Kanit |
|---|---|---|---|
| 0 | Yedek al | veritabanı yedegi / dosya kopyasi | yedek dosya + boyut |
| 1 | Yayindan kaldir | hatali surum devre disi | surum satiri |
| 2 | Sema geri alma | surum tablosundaki onceki surume karsi **saf SQL** | `SELECT version` ciktisi |
| 3 | Kodu geri al | `git revert <sha>` (yikici `git reset` degil) | `git log -1` |
| 4 | Dogrula | iki bacak build + test + smoke kontrol | 2 x `0 Hata`, 2 x `0 kaldi` |
| 5 | Kayit | `log.md` + rapor: ne, neden, ne zaman, kanit | tarihli kayit |

---

## §3 SQL Rollback Kurallari

| # | Kural | Neden |
|---|---|---|
| 1 | Her migration dosyasi eslesine `*_down.sql` ile gelir | Geri alma dosyasi hep hazir olsun |
| 2 | Onceki surume geri alinamiyorsa veri kopyalanir (expand-and-contract) | Veri kaybi yasak |
| 3 | Tum sorgular parameterized (`@p1`), string-concat yasak | G2 guvenlik |
| 4 | `CommandTimeout` acikca set edilir | Sonsuz bekleme yok |
| 5 | Rollback script'i surum tablosunu guncelleyerek biter | Yarim kalmis surum yakalanir |

---

## §4 Yapilmayanlar

| # | Yasak | Dogrusu |
|---|---|---|
| 1 | Kanitsiz "geri aldik" demek (G5) | §2'deki kanit sutunu dolu olmali |
| 2 | `git reset --hard` ile kaybi silmek | `git revert` + onay (G7) |
| 3 | EF Core migration ile geri alma | Saf SQL + surum tablosu (ADR-005) |
| 4 | Rollback'i yapip testi atlamak | §2 adim 4 zorunlu |
| 5 | Log'a yazmamak | §2 adim 5 zorunlu |

---

## §5 Kabul Kriteri

- [ ] Karar agacindan hangi dallarinin izlendigi yazilmis
- [ ] Her adimin kanit sutunu dolu (dosya, komut ciktisi ya da sayi)
- [ ] Sema geri alindiysa `SELECT version` ciktisi dosyada
- [ ] Iki bacak build + test yeniden kosulmus
- [ ] Sonuc `ROLLBACK TAMAM` / `PARTIAL` / `BLOCKED` olarak yazilmis

---

## §6 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : V0007 migration geri alindi              |
| KAPSAM : db/migrations/V0007__oturum.sql (+ down) |
| KARAR  : down tersten calistirildi, yedek dokunulmadi |
| RİSK   : V0007 sonrasi yazilan kayitlar silindi    |
| KANIT  : SELECT version -> Id=6 | test -> 0 kaldi  |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Build | `dotnet build <sln> -f net48` ve `-f net10.0-windows` | 2 x `0 Hata` |
| 2 | Test | `dotnet test <sln> -f net48` ve `-f net10.0-windows` | 2 x `Basarisiz: 0` |
| 3 | Vault | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈40 dk (onay beklemesi haric) |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Migration ekleme (ileri yon) | [migration-ekleme](../playbooks/migration-ekleme.md) |
| 2 | Hatanin kok nedeni | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |
| 3 | Guvenlik aciginin kapatilmasi | [guvenlik-denetimi](../playbooks/guvenlik-denetimi.md) |
| 4 | Gate kapanis sirasi | [WORKFLOW](../WORKFLOW.md) §2 |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `spec/migration-spec.md` | surum tablosu ve down dosyasi sozlesmesi |
| 2 | `checklists/deployment-rollback-check.md` | geri alma kontrol sorulari |
| 3 | `AGENTS.md` | devre paketi (4 madde) ve yikici islem onayi |
| 4 | `WORKFLOW.md` | geri yalniz kanitli RED raporu ile olur (§1.2) |
