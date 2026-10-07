---
title: "performans-olcumu - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Performans olcusu - nereden baslanir, hangi komut, taban deger kaydi, N+1/sorgu kontrolu ve geri alma"
---

# performans-olcumu — Performans Ölçümü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [coding-standards](../.rules/coding-standards.md) · [guvenlik-denetimi](guvenlik-denetimi.md)

> **Tek cumlelik sorumluluk:** *Yavaşlık nerede, kanıtı hangi ölçüm* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ölçüm **önce** yapılır, optimizasyon **sonra**: taban değer yazılır, tek bir darboğaz seçilir, tek değişiklik uygulanır ve ölçüm tekrarlanır. Kanıt = sayı + komut + tarih; "daha hızlı hissediliyor" G5 ihlalidir. UI bütçesi, `CommandTimeout` (15/60/300 sn) ve N+1 taraması bu işin hızlı kazanımlarıdır. ≈30 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Build/test yeşil (ölçüm boşa gitmesin) | `dual-tfm-build.md` + `dual-tfm-test.md` kaniti |
| 2 | Ölçülecek yol belli (ekran/sorgu) | tek hedef: `dosya::metot` ya da ekran adı |
| 3 | Taban değeri | ilk ölçüm çıktısı (sayı + tarih) |
| 4 | Kural okunmuş | `.ai/.rules/performance-guidelines.md` |
| 5 | Veri büyüklüğü biliniyor | satır sayısı (N+1 için: `COUNT(*)`) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `developer` (degerlendirme: `reviewer`)

1. **Tek hedef sec + taban olc** — her turda tek yol ölçülür; ilk sayı taban değerdir.
   - Komut: `dotnet test C:\versacoder\<sln> -f net10.0-windows --filter "Category=Performance" -v n`
   - Cikti: süre sayıları + tarih (rapora yazılır)

2. **Darboğaz tara** — N+1 (döngüde sorgu), timeout `0`, sınırsız `await`, gereksiz `ToList`/kayıt.
   - Komut: `Select-String -Path 'src\**\*.cs' -Pattern 'CommandTimeout\s*=\s*0'` -> 0 satır
   - Komut: `Select-String -Path 'src\**\*.cs' -Pattern 'foreach\s*\([^)]*\)\s*[\r\n]*[^;]*Query\('`
   - Cikti: bulgu listesi `dosya:satır`

3. **Sorgu/veri yolu dogrula** — parameterized + indeks + sayfa (100'lük) var mı; plan okunur.
   - Komut: `EXPLAIN QUERY PLAN <sorgu>` (SQLite) ya da `SET STATISTICS IO ON` (SQL Server)
   - Cikti: `SCAN TABLE` ise indeks adayı, `SEARCH TABLE ... USING INDEX` hedeftir

4. **Tek degisiklik + yeniden olc** — ayni komutla tekrar; fark sayıyla yazılır.
   - Komut: adim 1'in aynisi
   - Cikti: `önce 840 ms -> sonra 120 ms` (olçüm ciktisi dosyada)

5. **Kaniti kaydet** — taban/son değer, komut, tarih, kabul edilen eşik.
   - Dosya: `.ai/reports/` ilgili rapor + devre paketi `KANIT` maddesi
   - Cikti: 2 ölçüm satırı + eşik kararı (geçti/kaldı)

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | Taban değer | ilk ölçüm ciktisi (sayi + tarih) raporda |
| K2 | Son deger | 2. olcum ayni komutla, fark yazilmis |
| K3 | Timeout | `CommandTimeout = 0` taramasi -> 0 satir |
| K4 | N+1 | döngü-icinde-sorgu taramasi -> 0 satir ya da bilinen liste |
| K5 | Plan | `EXPLAIN` ciktisi indeks kullanir (veya gerekce) |

**Ornek cikti (K1+K2):**

```text
2026-09-25  Baslangic : 840 ms  (filter=Category=Performance)
2026-09-25  Sonrasi   : 120 ms  (IN @p1..@p100 toplu okuma)
Fark : -720 ms  ->  K2 PASS
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Optimize edilmis kod | `git revert <sha>` + ayni olcum (geri donus dogrulamasi) |
| 2 | Indeks eklenmis (migration) | `V0xx__<ad>.down.sql` (`DROP INDEX IF EXISTS`) |
| 3 | Sayfa/limit eklenmis | limiti kaldir + eski davranisi test et |
| 4 | Timeout degeri | `DbTimeouts` sabitlerine don (15/60/300) |
| 5 | Olcum kaydi | rapor satirina `REVERT <tarih>` ekle; taban degeri silme |

