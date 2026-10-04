---
title: "guvenlik-denetimi - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Kod / konfigurasyon / veri katmani guvenlik denetimi - OWASP odakli adim sirasi, severity ve kanit bicimi"
---

# guvenlik-denetimi — Guvenlik Denetimi

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [keys](../keys.md) · [rollback-geri-alma](../playbooks/rollback-geri-alma.md)

> **Tek cumlelik sorumluluk:** *Guvenlik denetimi hangi sirayla, hangi seviyeyle ve hangi kanitla yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Denetim **4 yuzeyde** yurur: **girdi isleme** (SQL injection, path traversal), **sir** (anahtar/baglanti dizesi), **erisim** (izin matrisi), **bagimlilik** (paket CVE). Bu projede **EF Core yok**, tum sorgular **parameterized** oldugu icin SQL injection denetimi `string concat` taramasina indirgenir ve dakikalar icinde sonuclanir. Her bulgu `CRITICAL / HIGH / MEDIUM / LOW` + `dosya:satir` + duzeltme onerisi biciminde yazilir; **CRITICAL > 0 ise is durur**. Kanitsiz "guvenceli" demek G5 ihlalidir.

---

## §1 Denetim Sirasi

| # | Yuzey | Kontrol | Arac / Kanit |
|---|---|---|---|
| 1 | SQL | String-concat SQL = **0**; her komut parameterized + `CommandTimeout` setli | `Select-String` taramasi + kod inceleme |
| 2 | Sir | Anahtar / baglanti dizesi dosya kodunda **yok** | `.env`, `apikey`, `password` taramasi |
| 3 | Dosya sistemi | Kullanici girdisi dogrudan yol olmaz; birlestirme + kok kisitlama | kod inceleme |
| 4 | Bagimlilik | Paket surumleri bilinen aciklik acisindan taranir | denetim raporu |
| 5 | Erisim | Yikici islemler onayli (G7), denetim izi var | kod + `log.md` |

---

## §2 Adimlar

**Toplam ≈35 dk** · yapan: `security-auditor` · degerlendirme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Concat taramasi | `Select-String -Path 'src\*.cs' -Pattern '"\s*\+\s*\w*(sql\|query\|select)'` | 3 dk |
| 2 | Sir taramasi | `Select-String -Path 'src\**\*' -Pattern '.env\|apikey\|password'` | 4 dk |
| 3 | Timeout + parametre denetimi | `Select-String -Path 'src\**\*.cs' -Pattern 'CommandTimeout\s*=\s*0'` -> 0 satir | 5 dk |
| 4 | Bagimlilik taramasi | kilit dosyadaki paket surumleri listelenir | 8 dk |
| 5 | Bulgu listesi + seviye | her bulgu: seviye + `dosya:satir` + oneri | 15 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Concat bulgusu var | Kaynak duzeltilmemis, yama atilmis | Kaynak duzeltilir; yama ile gizleme yasak (ADR-017) |
| 2 | "Guvenceli" denmis, kanit yok | Tarama kosulmamis | Komut ciktisi olmadan gecis yazilmaz |
| 3 | Bulgu kapatildi, inceleme ayni elde | Kendi kodunu denetleme | 2. inceleme baska ajana gider (AGENTS §1.2) |
| 4 | Severity listesi bos | Kayit bicimi tutulmamis | Her bulgu icin seviye + konum zorunlu |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Kaynakta duzeltme | `git revert <sha>` + ayni tarama tekrar (sonuc 0 satir olmali) |
| 2 | Secret koda girmis | Anahtar sifirlanir + kasa disina tasinir; koddan silme tek basina yeterli degil |
| 3 | Denetim raporu | `REVERT <tarih>` satiri; bulgu listesi silinmez (iz) |
| 4 | Bagimlilik dusurulmus | Kilit dosya eski surume + iki bacak build |
| 5 | Test kapandiysa | Test geri acilir, kapsam gerekcesi yeniden yazilir |

---

## §5 Kabul Kriteri

- [ ] 4 yuzeyin de tarama ciktisi dosyada (SQL · sir · erisim · bagimlilik)
- [ ] Her bulgu: seviye + `dosya:satir` + duzeltme onerisi
- [ ] String-concat SQL taramasi **0 satir**
- [ ] `CommandTimeout = 0` taramasi **0 satir**
- [ ] Sonuc `PASS`/`RED` olarak yazilmis; belirsizlik `DOĞRULAMA GEREKLI`

---

## §6 Seviye ve Tepki

| Seviye | Tepki | SLA |
|---|---|---|
| CRITICAL | Is durur; duzelt + yeniden denet | ≈10 dk |
| HIGH | Ayni oturumda duzelt | ≈30 dk |
| MEDIUM | Kuyruga al | ayni gun |
| LOW | Sonraki part | — |

```text
+--------------------------------------------------+
| BASLIK : SQL concat taramasi 0 bulgu ile kapandi  |
| KAPSAM : src/** (tarama), reports/guvenlik.md     |
| KARAR  : concat yok; timeout taramasi 0 satir     |
| RİSK   : secret kasasi DOĞRULAMA GEREKLI         |
| KANIT  : Select-String -> 0 satir (2 komut)       |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Concat | `Select-String -Path 'src\*.cs' -Pattern '"\s*\+\s*\w*(sql\|query\|select)'` | 0 satir |
| 2 | Timeout | `Select-String -Path 'src\**\*.cs' -Pattern 'CommandTimeout\s*=\s*0'` | 0 satir |
| 3 | Kodlama/link | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\find-mojibake.ps1` | `Mojibake file count: 0` |
| 4 | Butce | adim tablosu | ≈35 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Inceleme (kod kalitesi, mimari ihlal) | `reviewer` (AGENTS §1) |
| 2 | Hatanin kok nedeni | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |
| 3 | Geri alma adimlari | [rollback-geri-alma](../playbooks/rollback-geri-alma.md) |
| 4 | Anahtar kasasinin tasarimi | [keys](../keys.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `keys.md` | teknoloji kilitleri ve anahtar kasasi konumu |
| 2 | `AGENTS.md` | `security-auditor` gorev, kanit ve gecis kapi (§1 / §1.3) |
| 3 | `WORKFLOW.md` | hata modlari ve etiketler (§4.1) |
| 4 | `playbooks/index.md` | adim + sure + geri alma kabul kriteri |
