---
title: "K3 618 DLL Ham Envanteri — Kapsam Dışı Ham Veri"
type: report
category: reports
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "K3 versacoder.mimari-02.md 9 Tier / ~618 DLL listesinin ozeti, bilinen hatalari, kataloga neden girmedigi ve 3 adimlik dogrulama - kopya liste YOK (ADR-028 2.2)"
---

# K3 618 DLL Ham Envanteri

**Zorunlu Bağlantılar:** [index](index.md) · [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [keys](../keys.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](../architecture/index.md)

> **Tek cümlelik sorumluluk:** K3'ün 9 Tier / 618 DLL listesi nedir, neden vault kataloguna girmez ve nasıl doğrulanır sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**Bu dosya, K3 prompt dosyasındaki (`versacoder.mimari-02.md`) 9 Tier başlıklı ~618 DLL listesinin ÖZETİdir; listenin kendisi buraya kopyalanmaz.** Liste, ADR-028 §2.2'ye göre vault kataloguna, `N##` düğüm kimliklerine ve `count-components.ps1` sayımına **girmez** — tek izi bu dosyadır: `status: needs-validation`, `authority: derived`. Kayıtlı hatalar: **618 giriş / 616 benzersiz / 2 birebir tekrar / 9 başlıktan 2'si yanlış / 4 DLL dosyada yok / §15 "nihai değil"** (kaynak: ADR-028 §1 C2). Bu sayılar kaynak dosya bu vault dışında olduğu için **yeniden sayılmadı → DOĞRULAMA GEREKLİ.**

---

## §1 Bu Nedir, Nereden Gelir

| # | Alan | Değer |
|---|---|---|
| 1 | Kaynak dosya | `G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md` |
| 2 | İçerik | 9 Tier başlığı altında elle listelenmiş DLL envanteri (~618 giriş) |
| 3 | Kime ait | Başkasının not dosyası → bu vault'ta **yalnızca türetilmiş özet** (`authority: derived`) |
| 4 | Vault'a giriş biçimi | rapor satırı; katalog sütunu veya N-ID **değil** (ADR-028 §2.2) |
| 5 | Okuma kuralı | Salt okunur; G1 (kopyalama yasağı) kapsamındadır — tek satır kopyalanmaz |

**Bu dosyanın yapıp yapmadıkları:** yapar — özeti, hataları, doğrulama adımlarını; yapmaz — 618 satırlık listeyi, Tier başına DLL dökümünü, katalog kaydı üretmeyi.

---

## §2 Neden Vault Kataloguna Girmez (ADR-028 §2.2)

| # | Gerekçe | Sonuç |
|---|---|---|
| 1 | K3 envanteri **ikinci sayım birimi** doğurur (N kaydı vs DLL kaydı) | `count-components.ps1` iki birimde RED olurdu |
| 2 | İçinde kapsama dışı DLL'ler var (ör. `EntityFramework`, `Host.Linux`, `Protocol.Kafka`) | G2 (EF Core yasak) ve kapsam kirletilir |
| 3 | Katalog `Durum` sütunu ile değil, **kodla eşleşme** denetlenir | türetilmiş veri sütun olursa staleness iki katına çıkar |
| 4 | Sayım birimi tektir: `| N##-### |` ile başlayan katalog tablo satırı | tier verisi **etiketli rapor satırıdır, sütun değil** |

**Kabul edilen yer:** yalnız bu dosya + [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §3 tartışma kaydı.

### §2.1 Neden Listeye Dahil Edilmedi — Alternatifler (ADR-028 §3)

| # | Alternatif | Sonuç | Gerekçe (kayıtlı) |
|---|---|---|---|
| A1 | ADR-013 birebir + 6 şart (kabul edilen karar) | **KABUL** (3. tur 6/6) | tek sayım birimi (`N##`), `count-components` PASS |
| A2 | 8 katman + tier envanteri katalogda `Tier-DLL` türetilmiş sütunu olarak | **RED** | ikinci sayım birimi; araç iki birimde RED olurdu; staleness 2 katına çıkardı |
| A3 | K3'ün 9 Tier / 500+ DLL listesini ayrı proje/layer yapmak | **EVRENSEL RED** | 575 csproj × 2 TFM ≈ **1150 hedef**; CPM/VS/restore çatlar; listenin içinde kapsam dışı DLL'ler var |

**Tur 3 itirazları** (bu dosyanın varlık nedeni): junior #2 "yeni tip nereye yazılır" + **sahte PASS** (`src/**/Domain/**` taraması) + test/Desktop TFM uyuşmazlığı (NU1201); ajans #3 **5440 eşiği 0 marj**. Bu üç itiraz, ham listenin kataloga girmemesini değil, `DOĞRULAMA GEREKLİ` olarak tek raporda yaşamasını sağlamıştır.

---

## §3 Bilinen Hatalar (kayıtlı, yeniden sayılmadı)

| # | Hata | Kayıtlı değer | Durum |
|---|---|---|---|
| 1 | Toplam giriş | 618 | DOĞRULAMA GEREKLİ |
| 2 | Benzersiz DLL | 616 (2 fazla) | DOĞRULAMA GEREKLİ |
| 3 | Birebir tekrar | 2 satır: Performance 642/662 · Security 643/663 | DOĞRULAMA GEREKLİ |
| 4 | Yanlış başlık | 9 başlıktan 2'si | DOĞRULAMA GEREKLİ |
| 5 | Dosyada olmayan DLL | 4 adet | DOĞRULAMA GEREKLİ |
| 6 | Kaydın kendisi | §15: "nihai değil" | kayıtta — listede belirtilmiş |

> **Kaynak disiplini:** Bu satırlar ADR-028 §1 C2'nin özetidir; ham dosya bu vault dışında olduğu için sayım tekrarlanmadı. Sayım yapılmadan hiçbir satır `DOĞRULANDI` etiketi almaz.

---

## §4 Kapsam ve Kabul Kriteri

**Kapsam dışı:** listenin kendisi (618 satır), Tier bazlı DLL dökümü, katalog/N-ID eşlemesi, proje adaylığı.

| # | Kabul kriteri (hepsi tutmalı) | Ölçüm |
|---|---|---|
| 1 | Kaynak dosyada DLL satırı sayısı = **618** (farklıysa gerçek sayı yazılır) | elle/komut sayımı |
| 2 | Benzersiz DLL = **616**; 2 tekrar giderilir | tekil listelem |
| 3 | 9 başlıktan **2 yanlış** başlık düzeltilir | başlık–içerik karşılaştırması |
| 4 | Dosyada olmayan **4 DLL** işaretlenir (kaynakta var, listede karşılığı yok ya da tersi) | `Test-Path` benzeri teyit |
| 5 | "nihai değil" ifadesi ya kalkar ya da bu dosyada gerekçesiyle kalır | §3/6 satırı |
| 6 | Hiçbiri tutmazsa sayılar **bulunan gerçek değerle** güncellenir | §3 tablosu |

**Geçiş:** 6/6 tutarsa `status: needs-validation` → `status: active`; `authority: derived` **değişmez** (kaynak bu vault'un değil).

---

## §5 Doğrulama Adımları (3 satır)

```text
1) Kaynagi ac: G:\...\Versacoder\new\versacoder.mimari-02.md  -> DLL satirlarini say (618 / 616 benzersiz)
2) 2 birebir tekrari + 2 yanlis basligi + 4 dosyada olmayani isaretle
3) Sonucu bu dosyanin §3 tablosuna yaz; 6/6 tutarsa status: active yap, tutmazsa sayiyi gercek degerle duzelt
```

**Sorumlu:** architect · **Tahmin:** ≈30 dk (tek dosya okuma + sayım) · **Kanıt:** §3 tablosundaki hücrelerin `DOĞRULANDI` olması.

---

## §6 Doğrulama

- [ ] Frontmatter 8 alan; `status: needs-validation`, `authority: derived` birebir
- [ ] 618 satırlık liste kopyalanmadı (yalnız özet + kabul kriteri)
- [ ] Her sayı `DOĞRULAMA GEREKLİ` etiketli ya da ölçümle gelmiş
- [ ] Bu dosyadan kataloga (`architecture/catalog/`) link yok
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` → bozuk link 0

| # | Red sinyali | Aksiyon |
|---|---|---|
| R1 | Dosyada 618 satırlık DLL listesi göründü | sil — kapsam dışı (ADR-028 §2.2) |
| R2 | `status: active` yazdı ama sayım yapılmadı | geri al → `needs-validation` |
| R3 | Bu listedeki DLL'ler katalog tablosuna eklendi | sil — ikinci sayım birimi |

---

## §7 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Katman baz kararı | `.ai/.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | kapsam dışı gerekçesi (§2.2) + hata kaydı (§1 C2) |
| 2 | Mimari indeks | `.ai/architecture/index.md` | 52 düğüm / N sayım birimi |
| 3 | Kapsam analizi | `.ai/reports/00-kapsam-analizi.md` | K3 dosyasının vault'a giriş kaydı |
| 4 | Bilinen eksikler | `.ai/reports/10-bilinen-eksikler.md` | NEEDS_VALIDATION maddeleri |
| 5 | Rapor indeksi | `.ai/reports/index.md` | navigasyon |
