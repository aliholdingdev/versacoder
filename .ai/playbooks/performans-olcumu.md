---
title: "performans-olcumu - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Performans olcusu - tek hedef secimi, taban deger kaydi, N+1/sorgu kontrolu ve geri alma"
---

# performans-olcumu — Performans Olcumu

**Zorunlu Baglantilar:** [playbooks index](index.md) · [rules index](../.rules/index.md) · [dual-tfm-test](../playbooks/dual-tfm-test.md) · [guvenlik-denetimi](../playbooks/guvenlik-denetimi.md) · [spec index](../spec/index.md)

> **Tek cumlelik sorumluluk:** *Yavaslik nerede ve kaniti hangi olcum* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Olcum **once** yapilir, optimizasyon **sonra**: taban deger yazilir, tek bir darboğaz secilir, tek degisiklik uygulanir ve olcum tekrarlanir. Kanit = **sayi + komut + tarih**; "daha hizli hissediliyor" cumlesi G5 ihlalidir. Hizli kazanimlar: `CommandTimeout` (15/60/300 sn) politikasi, N+1 (dongude sorgu) taramasi ve sayfalama. Toplam ≈30 dk.

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Build/test yeşil (olcum bosa gitmesin) | dual-tfm-build §5 + dual-tfm-test §5 |
| 2 | Olculcek yol belli | tek hedef: `dosya::metot` ya da ekran adi |
| 3 | Taban deger alinmis | ilk olcum ciktisi (sayi + tarih) |
| 4 | Kural okunmus | [rules index](../.rules/index.md) -> performance kurallari |
| 5 | Veri buyuklugu biliniyor | satir sayisi (N+1 icin: `COUNT(*)`) |

---

## §2 Adimlar

**Toplam ≈30 dk** · yapan: `developer` · degerlendirme: `reviewer`

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Tek hedef sec + taban olc | `dotnet test <sln> -f net10.0-windows --filter "Category=Performance" -v n` | 5 dk |
| 2 | Darbozag tara | `Select-String -Path 'src\**\*' -Pattern 'CommandTimeout\s*=\s*0'` -> 0 satir | 6 dk |
| 3 | N+1 tara | dongu-icinde-sorgu kalibi -> bulgu listesi `dosya:satir` | 6 dk |
| 4 | Tek degisiklik + yeniden olc | adim 1'in aynisi; fark sayiyla yazilir | 8 dk |
| 5 | Kaniti kaydet | taban/son deger + komut + tarih rapora | 5 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Sonuc tutarsiz (oynuyor) | Veri/seed farkli, olcum disi is var | Ayni veri + ayni filtre ile 3 kosum ortalamasi |
| 2 | N+1 bulunamadi | Tarama kalibi dar | Sorgu cagrisi dongu icinde mi, repository uzerinden mi genislet |
| 3 | Kazanc yok ama kod degisti | Yanlis darbozag dokunulmus | Taban olcume don, tek farkla tekrar |
| 4 | Timeout taramasi kirmizi | Varsayilan 0 verilmis | Kilit degerlere donulur (15/60/300) |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Optimize edilmis kod | `git revert <sha>` + ayni olcum (geri donus dogrulamasi) |
| 2 | Indeks eklenmis (migration) | `*.down.sql` (`DROP INDEX IF EXISTS`) |
| 3 | Sayfa/limit eklenmis | limit kaldirilir + eski davranis test edilir |
| 4 | Timeout degeri | sabit timeout degerlerine donulur |
| 5 | Olcum kaydi | rapor satirina `REVERT <tarih>` eklenir; taban deger silinmez |

---

## §5 Kabul Kriteri

- [ ] Taban deger raporda: sayi + komut + tarih
- [ ] Son deger ayni komutla alinmis, **fark** yazilmis
- [ ] `CommandTimeout = 0` taramasi 0 satir
- [ ] N+1 taramasi 0 satir ya da bilinen bulgu listesi (`dosya:satir`)
- [ ] Sonuc esigi gecti/kaldi olarak karara baglanmis

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| BASLIK : Oturum listesi 840 ms -> 120 ms          |
| KAPSAM : src/.../SessionStore : 42 -> 51           |
| KARAR  : toplu okuma (IN @p1..@p100) tek noktada   |
| RİSK   : eski tekil okuma yollari incelenmeli      |
| KANIT  : once 840 ms -> sonra 120 ms, fark -720 ms |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Timeout | `Select-String -Path 'src\**\*' -Pattern 'CommandTimeout\s*=\s*0'` | 0 satir |
| 2 | Olcum | `dotnet test <sln> -f net10.0-windows --filter "Category=Performance"` | sayili sure ciktisi |
| 3 | Vault | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `BOZUK link : 0` |
| 4 | Butce | adim tablosu | ≈30 dk |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Hata kok nedeni (kiriliyor) | [debug-hatayi-bulma](../playbooks/debug-hatayi-bulma.md) |
| 2 | Mimari/katman performansi karari | [yeni-katman-ekleme](../playbooks/yeni-katman-ekleme.md) |
| 3 | Kapsam/coverage hedefi | [spec index](../spec/index.md) |
| 4 | Guvenlik acigi | [guvenlik-denetimi](../playbooks/guvenlik-denetimi.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `checklists/performance-budget-check.md` | butce esikleri ve kontrol sorulari |
| 2 | `spec/performance-spec.md` | kabul edilen esikler ve olcum yollari |
| 3 | `checklists/dual-tfm-build-check.md` | olcum oncesi build yeşilligi |
| 4 | `playbooks/index.md` | adim + sure + geri alma kabul kriteri |
