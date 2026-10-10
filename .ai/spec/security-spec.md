---
title: "VersaCoder — Güvenlik Şartnamesi (Secret / İzin / SQL)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Secret yönetimi, izin modeli (allow/ask/deny) ve SQL enjeksiyon savunması — kural seti `.rules/security-rules.md` ile birlikte tek davranış kaynağı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Güvenlik Şartnamesi (`spec/security-spec.md`)

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[architecture/index|architecture/index]] · [[spec/tool-contract-spec|tool-contract-spec]] · [[spec/data-layer-spec|data-layer-spec]] · [[spec/host-layer-spec|host-layer-spec]]

> **Tek cümlelik sorumluluk:** *Secret nerede ve nasıl saklanır, kim neye izinlidir ve SQL enjeksiyonu hangi üç katmanda durdurulur* sorusunun tek cevabıdır.
> Kural numaraları (SEC-01..SEC-16) `.rules/security-rules.md`'dedir; bu belge **uygulama davranışı**nı anlatır (tekrar yok).

---

## §0 Özet (Answer First)

Secret **repo'da ve düz metinde durmaz** (DPAPI / AES-256-GCM ile şifreli, anahtar dosyası repo dışında), izin modeli **`allow / ask / deny` + varsayılan `ask`** ve yükseltememe kuralıyla tool kapısında uygulanır, SQL savunması **üç katmanlıdır**: `@p1..@pN` parametreleme + allowlist kolon adı + sorgu metninin `const` olması (G2, DA-03).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Secret saklama/çözme akışı (açılışta) | Açılmış config'in DI'a gidişi: [[spec/host-layer-spec|host-layer-spec]] |
| 2 | İzin modeli davranışı (`allow/ask/deny`, timeout'ta `deny`) | İzin matrisi + sandbox şeması: [[spec/tool-contract-spec|tool-contract-spec]] |
| 3 | SQL injection savunmasının üç katmanı | Repository davranışı: [[spec/data-layer-spec|data-layer-spec]] |
| 4 | Log/transcript kırmızıaltma (maskelenen değer tipleri) | Hata kodu listesi: [[spec/protocol-spec|protocol-spec]] |
| 5 | Yıkıcı işlem onayı (G7/G11) ve audit izi | Onay mercii/gate: [[governance/workflow|WORKFLOW]] §3 |

> **YASAK:** Repo'ya ham anahtar/`Password=`/`sk-` · `ECB`/sabit IV · `BinaryFormatter` · maskelenmemiş log · onaysız migration/silme (SEC-15).

---

## §2 Davranış

### §2.1 Secret akışı

| # | Aşama | Davranış | İhlal belirtisi |
|---|---|---|---|
| 1 | İlk çalıştırma | API anahtarı kullanıcıdan alınır, **prompt ile** | Kodda literal anahtar |
| 2 | Saklama | `net48`: DPAPI (`ProtectedData`, `CurrentUser`); `net10`: AES-256-GCM, her seferinde yeni nonce | Sabit nonce / elle şifreleme |
| 3 | Anahtar dosyası | Repo **dışında**, `.gitignore` kapsamı içinde | `.db`, `secrets.*` stage'de |
| 4 | Çözme | Yalnız açılışta, tek yerde (`IConnectionStringProtector`) | Çözülmüş string loga/DB'ye yazıldı |
| 5 | Kullanım | Bağlantı string'i yalnız l4 bağlantı fabrikasında | Yukarı taşınan ham string |

### §2.2 İzin modeli (özet — tam matris tool-contract-spec §5)

```
  tool.call -▶ [ deny? ]--evet--▶ hata doner (calismaz)
                    |
                  hayir
                    v
               [ allow? ]-evet-▶ calisir -▶ sonuc
                    |
                  hayir (ask)
                    v
              kullaniciya onay -+- allow  -> oturum icin hafiza
                                +- always -> proje ayari (shell icin YOK)
                                +- 60 sn cevap yok -> VARSAYILAN DENY
```

| # | Kural | Sonuç |
|---|---|---|
| 1 | Varsayılan `ask`; kullanıcı `ask → allow` düşürebilir, **`deny`'yi yükseltilemez** | Yetki aşımı imkânsız |
| 2 | Zaman aşımı = **`deny`** (gizli onay yok) | Sessiz yetkilendirme yok |
| 3 | Workspace dışı / `high` tehlike = `deny` ya da her seferinde onay | Sistem dosyasına erişim yok |
| 4 | Reddedilen çağrı transcript'e yazılır | Denetlenebilirlik (audit) |

### §2.3 SQL enjeksiyonu — üç katman

| Katman | Savunma | Kırılırsa |
|---|---|---|
| 1 — Parametre | Değer **hiç** sorgu metnine girmez; `IN` listesi bile parametre üretilir | `reviewer` RED + G2 |
| 2 — Kolon/tablo adı | **Allowlist** (enum) ile sabit metin; kullanıcı girdisi ad olamaz | RED (DA-03) |
| 3 — Gövde sabitliği | `const string sql` — interp/concat ile kurulan gövde **yok** | RED (DA-06) |
| Ek | Girdi sınırı doğrulama (tip/uzunluk/aralık) + sınırda `MaxLength` | Protokol/tool şeması reddeder (SEC-04) |
| Ek | Hata mesajında SQL metni/stack **yok** | `VC-E` + maskelenmiş log (SEC-13) |

---

## §3 TFM Notu

| Bacak | Fark | Kural |
|---|---|---|
| `net48` | Secret = **DPAPI** (`ProtectedData`) | Modern kripto API'si (`AES-GCM`) burada kullanılmaz |
| `net10.0-windows` | Secret = **AES-256-GCM** (rastgele nonce) | DPAPI hâlâ geçerli; ikisi de CPM sürümüyle kilitli |
| Ortak | İzin modeli ve SQL savunması **aynı** kod | `#if` ile güvenlik davranışı **bölünmez** (G6) |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| SE-01 | Repo'da secret yok | Tarama: `sk-`, `Password=`, `ApiKey=` = **0 satır** (SEC-01) |
| SE-02 | Concat SQL yok | Tarama: `"..."+id` = 0 satır (G2) |
| SE-03 | Onaysız tool çalışmaz + 60 sn sonra `deny` | Tool izin testi yeşil (tool-spec §5) |
| SE-04 | Log maskelenmesi | Sahte anahtar logda `sk-***` görünür (SEC-08) |
| SE-05 | Sıfır EF Core + sıfır `BinaryFormatter` | Proje taraması = 0 satır |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
