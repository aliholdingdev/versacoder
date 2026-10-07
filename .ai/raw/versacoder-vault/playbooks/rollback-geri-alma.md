---
title: "Geri Alma (Rollback) Playbook - rollback-geri-alma"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: developer
scope: "Hatali bir degisikligi guvenle geri alma adimlari - SQL migration, kod ve konfigurasyon icin sirali prosedur"
---

# Geri Alma (Rollback) Playbook

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bozuk bir degisikligi (kod / SQL / konfigurasyon) hangi sirayla, kanitla geri alirim* sorusunun tek cevabidir.
> Bu bir **is akisidir**; her adimda kanit uretilir, kanitsiz "geri aldik" iddiasi yasaktir (G5).

---

## §0 Ozet (Answer First)

Rollback 3 katmanda yapilir ve sirasi **degistirilemez**: (1) **calisan kodu durdur/yayindan kaldir**, (2) **veri semasini geri al** (saf SQL + version tablosu, EF Core migration DEGIL), (3) **kodu git ile geri al**. Once etkiyi kisitla, sonra geri al; her adim `log.md`'ye tarihle yazilir.

---

## §1 Karar Agaci

```
HATA TESPIT EDILDI
  |
  +-- canli veri kaybi riski var mi? -- EVET -> ONCE veri yedegi al (adim 0)
  |
  +-- hizli geri alinabilir mi? ------ EVET -> git revert (adim 3) + yayindan kaldirma (adim 1)
  |
  +-- sema degisti mi? --------------- EVET -> SQL rollback script'i calistir (adim 2)
  |
  +-- hepsi HAYIR -------------------------> yalnizca kodu geri al (adim 3), log'a yaz
```

---

## §2 Adimlar

| # | Adim | Yapilan | Kanit |
|---|---|---|---|
| 0 | **Yedek al** | Veritabanı yedegi / dosya kopyasi | yedek dosya + boyut |
| 1 | **Yayindan kaldir** | Hatali surum devre disi (service stop / eski surume don) | surum satiri |
| 2 | **Sema geri alma** | Surum tablosundaki onceki surume karsi **saf SQL** rollback script | `SELECT version` ciktisi |
| 3 | **Kodu geri al** | `git revert <sha>` (yikici `git reset` degil; varsa **onay** - G7) | `git log -1` |
| 4 | **Dogrula** | `dotnet build` + `dotnet test` + uygulama smoke kontrolu | komut ciktilari |
| 5 | **Kayit** | `log.md` + ilgili rapor: ne, neden, ne zaman, kanit | tarihli kayit |

---

## §3 SQL Rollback Kurallari (ADONET + Dapper)

| # | Kural | Neden |
|---|---|---|
| 1 | Her migration dosyasi **esleslesine** `*_down.sql` ile gelir | Geri alma dosyasi hep hazir olsun |
| 2 | Sema degisikligi **onceki surum** icin geri alinamazsa veri kopyalanir (expand-and-contract) | Veri kaybi yasak |
| 3 | Tum sorgular **parameterized** (`@p1`), string-concat **yasak** | G2 guvenlik |
| 4 | `CommandTimeout` acikca set edilir | Sonsuz bekleme yok |
| 5 | Rollback script'i **version tablosu** guncelleyerek biter | Yarim kalmis surum yakalanir |

---

## §4 Yapilmayanlar

| # | Yasak | Dogrusu |
|---|---|---|
| 1 | Kanitsiz "geri aldik" demek (G5) | §2'deki kanit sutunu dolu olmali |
| 2 | `git reset --hard` ile kaybi silmek | `git revert` + onay (G7) |
| 3 | EF Core migration ile geri alma | Saf SQL + version tablosu (ADR-005) |
| 4 | Rollback'i yapip testi atlamak | Adim 4 zorunlu |
| 5 | Log'a yazmamak | Adim 5 zorunlu |

---

## §5 Cikti BiCimi

| Alan | Deger |
|---|---|
| Sonuc | `ROLLBACK TAMAM` / `PARTIAL` / `BLOCKED` |
| Kanit | build + test + version sorgusu ciktisi |
| Etkilenen dosyalar | git diff adresi |
| Sonraki adim | [guvenlik-denetimi.md](guvenlik-denetimi.md) ya da reviewer devresi |

**Ilk adim (<2 dk):** Adim 0'u calistir: geri alinacak nesnenin yedeğini al ve dosya boyutunu loga yaz.
