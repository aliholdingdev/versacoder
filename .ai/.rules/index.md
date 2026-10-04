---
title: ".rules — Kural Defteri (17 kural dosyasi)"
type: index
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: ".rules/ altindaki 17 kural dosyasinin tek kayit defteri: kapsam, kimi RED ederdigi, kural eslesmesi ve catisma onceligi"
---

# `.rules/` — Kural Defteri

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md)

> **Tek cümlelik sorumluluk:** *Vault'a yeni bir kural gerektiğinde hangi dosyanın açılacağı, mevcut 17 kuralın kimi RED ettiği ve ikisi çatışırsa hangisinin kazandığı* sorusunun tek cevabıdır.
> Kural **metni** ilgili dosyadadır; bu dosya yalnız **kayıt + yönlendirme + öncelik** taşır (SRP).

---

## §0 Özet (Answer First)

`.rules/` altında **17 kural dosyası** var ve hepsi `status: active`, `authority: SSOT`. Aranan kural §1'de dosya adıyla bulunur; iki kural çatışırsa **§3 öncelik sırası** uygulanır: güvenlik > anayasa (G1..G12) > ADR > veri erişimi > kod > performans > stil. Kuralı **bilerek** çiğneyen iş RED olur; denetleyen değil, uygulayan da kuralın sahibidir. Kural değişikliği yalnız `architect` + (gate gerekiyorsa) kullanıcı onayıyla girer.

---

## §1 Kayıt Defteri (17 dosya)

| # | Dosya | Kapsam (tek satır) | RED eden |
|---|---|---|---|
| 1 | [accessibility-rules.md](accessibility-rules.md) | klavye, tab sırası/mnemonic, odak, kontrast, DPI | `reviewer` |
| 2 | [async-rules.md](async-rules.md) | `async/await` derinliği, `ConfigureAwait`, timeout + iptal | `reviewer` |
| 3 | [caching-rules.md](caching-rules.md) | ne cache'lenir, TTL, invalidation, anahtar, ölçüm | `reviewer` |
| 4 | [coding-standards.md](coding-standards.md) | C# dil kuralları, nullable, analyzer, iki bacak, yorum | `reviewer` |
| 5 | [configuration-rules.md](configuration-rules.md) | yapılandırma kaynağı, öncelik, secret dışı değer, doğrulama | `architect` |
| 6 | [data-access-rules.md](data-access-rules.md) | ADO.NET + Dapper, **EF Core yasak**, parameterized, timeout | `reviewer` (**CRITICAL**) |
| 7 | [deployment-guide.md](deployment-guide.md) | iki bacak paketleme, DevExpress runtime, CI, rollback | `architect` |
| 8 | [devexpress-rules.md](devexpress-rules.md) | **v26.1 tek sürüm**, ribbon `Office2010`, form üçlüsü | `reviewer` (**CRITICAL**) |
| 9 | [error-handling-rules.md](error-handling-rules.md) | `catch` sınırı, `Result` sözleşmesi, yeniden deneme, UI mesajı | `reviewer` |
| 10 | [layer-rules.md](layer-rules.md) | **4 kademe + 3 eksen**, yon grubu **8·12·16·20** + 29 paket eslemesi, baslangic 8x8 matris, 9+N+2 topoloji, katalog ID/8 sutun, esik 5000 + dugum 44/100/500/1000/5000 | `architect` (**CRITICAL**) |
| 11 | [logging-rules.md](logging-rules.md) | seviye, tek satır format, secret-PII yasağı, dönen dosya | `reviewer` |
| 12 | [naming-conventions.md](naming-conventions.md) | namespace, sınıf/olay, dosya adı, slug, Form eki | `reviewer` |
| 13 | [performance-guidelines.md](performance-guidelines.md) | N+1 yasağı, render bütçesi, IO/arama eşikleri, ölçüm | `reviewer` |
| 14 | [security-rules.md](security-rules.md) | secret, şifreli bağlantı, sınır doğrulama, process, DPAPI | `reviewer` (**CRITICAL**) |
| 15 | [solid-rules.md](solid-rules.md) | ilke başına do/don't, katman yönü, NetArchTest denetimi | `architect` |
| 16 | [testing-rules.md](testing-rules.md) | unit/integration, determinizm, coverage, gate öncesi test | `tester` |
| 17 | [ui-rules.md](ui-rules.md) | `RibbonForm` + `Office2010`, wireframe → Form zinciri, yerleşim | `ui-designer` |

---

## §2 Kural Eşleşmesi (neyi arıyorsan)

| Soru | Dosya |
|---|---|
| Bu kod EF Core kullanıyor mu? | [data-access-rules.md](data-access-rules.md) |
| Sürüm / ribbon stili doğru mu? | [devexpress-rules.md](devexpress-rules.md) |
| `async void` yazabilir miyim? | [async-rules.md](async-rules.md) · [coding-standards.md](coding-standards.md) |
| Log'a parola düştü mü? | [security-rules.md](security-rules.md) · [logging-rules.md](logging-rules.md) |
| Ekran fareyle mi geziliyor? | [accessibility-rules.md](accessibility-rules.md) |
| Test yeterli mi? | [testing-rules.md](testing-rules.md) · [../spec/test-strategy.md](../spec/test-strategy.md) |

---

## §3 Çatışma Önceliği

| Sıra | Üst gelen | Örnek |
|---|---|---|
| 1 | **Güvenlik** | secret log'a sızdı → performans mazereti geçersiz |
| 2 | **Anayasa** `CLAUDE.md` G1..G12 | tek bacaklı tasarım her şeyi ezer |
| 3 | **ADR** (`.decisions/accepted/`) | v26.1 kilidi, EF Core yasağı |
| 4 | **Veri erişimi** | parameterized her sorguda bağlayıcı |
| 5 | **Kod / async / hata** | `coding-standards` → `async` → `error-handling` |
| 6 | **Performans** | eşikler, ölçüm |
| 7 | **Stil / isim** | `naming-conventions` — işi kilitlemez (LOW) |

**Yol:** Çatışma bulunca etiketle → `CONFLICT DETECTED` → yukarıdaki sıra → çözülmediyse `DECISION BLOCKED` → kullanıcıya 1 soru (protokol §3.3).

---

## §4 Kapsam Dışı ve Değişiklik

| # | Bu dosyada değil | Nerede |
|---|---|---|
| 1 | Kural metni | ilgili `.rules/*.md` (§1) |
| 2 | Karar gerekçesi (neden bu kural) | `.decisions/accepted/ADR-NNN-*.md` |
| 3 | Gate sırası / onay | `../WORKFLOW.md` |
| 4 | Yetki (kim neyi yazar) | `.agents/AGENTS.md` |
| 5 | Yeni kural ekleme | önce ADR → sonra §1'e satır → sonra dosya |
| 6 | Kademe/eksen/yon grubu kararlari (8·12·16·20, 44/100/500/1000/5000) | [layer-rules.md](layer-rules.md) §0-§1.3 · gerekce ADR-030 §2 |

**Yeni kural sırası (zorunlu):** `ADR aç` → `.rules/<ad>.md` yaz → **bu tabloya satır ekle** → `index.md`'ye kayıt → `log.md`. Tersi kayıt defteri koparır.

---

## §5 Doğrulama

- [ ] §1'de **17** satır, her satırda gerçek dosya (link = 0 bozuk)
- [ ] §1'de listelenen her dosya disekte var; diske yeni dosya var mı diye §1 taranır
- [ ] Çatışma önceliği §3'te, en üstte güvenlik var
- [ ] `check-links.ps1` → **0** bozuk link

---

## §6 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 2 | ADR defteri | `.ai/.decisions/index.md` |
| 3 | Test stratejisi | `.ai/spec/test-strategy.md` |
| 4 | Yetki matrisi | `.ai/.agents/AGENTS.md` |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `.rules/*.md` frontmatter `scope:` alanları.
