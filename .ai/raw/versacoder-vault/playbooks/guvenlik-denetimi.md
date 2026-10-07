---
title: "Guvenlik Denetimi Playbook - guvenlik-denetimi"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: developer
scope: "Kod / konfigurasyon / veri katmani guvenlik denetimi - OWASP odakli adim sirasi ve kanit bicimi"
---

# Guvenlik Denetimi Playbook

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *VersaCoder kod tabaninda guvenlik denetimini hangi sirayla, kanitla yaparim* sorusunun tek cevabidir.
> Her bulgu `CRITICAL / HIGH / MEDIUM / LOW` seviyesi + `dosya:satir` + duzeltme onerisi biciminde yazilir; kanitsiz bulgu gecersizdir.

---

## §0 Ozet (Answer First)

Denetim **4 yuzeyde** yapilir: **girdi isleme** (SQL injection, path traversal), **sir** (secret/sakli anahtar), **erisim** (izin matrisi), **bagimlilik** (NuGet CVE). Bu projede **EF Core yok**, tum sorgular **parameterized** oldugundan SQL injection denetimi klasik `string concat` taramasina indirgenir - grep ile dakikalar icinde sonuclanir.

---

## §1 Denetim Sirasi

| # | Yuzey | Kontrol | Arac / Kanit |
|---|---|---|---|
| 1 | **SQL** | String-concat SQL = **0**; her `SqlCommand` parameterized + `CommandTimeout` setli | grep + kod inceleme |
| 2 | **Secret** | API key / connection string dosya kodunda **yok**; secret store'da | grep `.env|apikey|password` |
| 3 | **Dosya sistemi** | Kullanici girdisi dogrudan yol olmaz; `Path.Combine` + kok kisitlama | kod inceleme |
| 4 | **Bagimlilik** | NuGet paket surumleri bilinen CVE icin taranir | denetim raporu |
| 5 | **Erisim** | Yikici islemler (silme/overwrite) onayli (G7), audit trail var | kod + log |

---

## §2 Sorgu Denetimi (bu projenin kirmizi cizgisi)

| # | Kirmizi | Yesil |
|---|---|---|
| 1 | `"SELECT ... " + kadi` | `"SELECT ... WHERE id = @id"` |
| 2 | Interpolasyonlu SQL (`$"..."`) | `command.Parameters.AddWithValue("@p1", deger)` |
| 3 | `CommandTimeout` = varsayilan | `command.CommandTimeout = 30` acikca |
| 4 | Genel yetkili baglanti paylasimi | Baglanti yazilimi katmanda (ADR-005 / ADR-012) |
| 5 | Mesajda sorgu metni loglama | Parametre **degerleri** maskelenir |

---

## §3 Sonuc Kaydi

| Alan | Deger |
|---|---|
| Bulgu sayisi | CRITICAL `n` / HIGH `n` / MEDIUM `n` / LOW `n` |
| Kanit | grep komutu + sayi ya da `UNKNOWN` |
| Gecis | CRITICAL > 0 ise **is durur** (`developer`'a devir) |
| Devir | Bulgu listesi [documentation-check.md](../checklists/documentation-check.md) uzerinden `reviewer`a |

| Seviye | Tepki | SLA |
|---|---|---|
| CRITICAL | Is durur, duzelt + yeniden denet | ≈10 dk |
| HIGH | Ayni oturumda duzelt | ≈30 dk |
| MEDIUM | Kuyruga al | ayni gun |
| LOW | Sonraki part | — |

---

## §4 Yapilmayanlar

| # | Yasak | Neden |
|---|---|---|
| 1 | "Guvenceli" demek kanit yokken | G5 ihlali |
| 2 | Bulguyu bulan kisinin kapatmasi | 2. inceleme `architect`e gider |
| 3 | Secret'i koda yazip "sonra temizleriz" | Sizinti kalici olur |
| 4 | String-concat SQL'i yamayla gizlemek | Kaynak duzeltilir (ADR-017) |
| 5 | Denetimi test olmadan "bitti" saymak | Test + grep kaniti zorunlu |

---

## §5 Cikti BiCimi

| Alan | Deger |
|---|---|
| Sonuc | `PASS` / `RED` (belirsiz yok) |
| Kanit | grep ciktisi + sayilar |
| Sonraki adim | RED ise `developer`'a 4 maddele devre; PASS ise `tester`a |

**Ilk adim (<2 dk):** `Select-String -Path C:\versacoder\src\*.cs -Pattern '"\s*\+\s*\w*(sql|query|select)' -SimpleMatch:$false` calistir; sonucu §1 tablosuna isaretle.
