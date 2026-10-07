---
title: "ADR-018 — Secret Store: Anahtarlar Şifreli Depoda, keys.md Yalnız Harita"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "API anahtarı/parola saklama yeri, şifreleme, maskeleme ve keys.md'nin kapsamı"
hedef_bacak: "net48 + net10.0-windows"
evidence: "ADR-005 §karar 5 (DPAPI) · CLAUDE.md G9 · reports/01-web-kaynaklari.md"
---

# ADR-018 — Secret Store: Anahtarlar Şifreli Depoda, keys.md Yalnız Harita

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-011](ADR-011-llm-provider-order.md)

> **Tek cümlelik karar:** Tüm secret'lar (LLM API anahtarı, bağlantı parolası) **DPAPI ile kullanıcı kapsamında şifrelenmiş yerel secret store'da** tutulur; **`keys.md` yalnız anahtar kelime haritasıdır ve değer içermez** — karar verildi.

---

## §0 Özet (Answer First)

Vault (`C:\versacoder\.ai`) metin deposudur ve git'e girer; secret git'e girerse **döndürülemez**. Bu ADR iki şeyi ayırır: **değer** şifreli yerel depoda (`%APPDATA%\VersaCoder\secrets.bin`, DPAPI kullanıcı kapsamı), **harita** (hangi anahtarın hangi sağlayıcıda kullanıldığı, adı/erişim yolu) `keys.md`'de. Kodda düz metin anahtar = kritik RED; loglarda değer **maskelenir**. Bu karar [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 5'in genişletmesidir, onu **değiştirmez**.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | `keys.md` zaten "anahtar kelime haritası" olarak tanımlı; değer oraya girmez | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 5 |
| C2 | Secret/API key bulunması **kritik RED** nedenidir | `CLAUDE.md` G9 |
| C3 | `keys.md` kök belgedir, frontmatter + scope zorunlu | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) |
| C4 | LLM sağlayıcısı anahtar ister (`OpenRouter`), taşınacak yer gerekli | [ADR-011](ADR-011-llm-provider-order.md) D6 |
| C5 | SQL bağlantı parolası düz metin duramaz | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 5 |
| C6 | Kopyalama yasağı: başka vault'tan secret/metin taşınmaz | [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |

---

## §2 Karar

**Karar:** Secret **değeri** DPAPI ile şifrelenerek kullanıcı kapsamındaki yerel depoda saklanır; **`keys.md` yalnız ad + yol + sağlayıcı haritasıdır**, değer hiçbir vault dosyasında düz metin durmaz — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Depo yolu | `%APPDATA%\VersaCoder\secrets.bin` — **repo ve vault dışında** | C1 · C6 |
| D2 | Şifreleme | DPAPI (`ProtectedData`), **kullanıcı kapsamı** | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) |
| D3 | `keys.md` kapsamı | `anahtar adı · sağlayıcı · erişim yolu · durum` — **değer yok** | C1 |
| D4 | Kod kuralı | kodda/`.cs`/`.json` config içinde düz metin secret = **kritik RED** (G9) | C2 |
| D5 | Log kuralı | değer maskeleme (`sk-***`, parola `****`); yalnız **ad** loglanır | C2 · [ADR-026](ADR-026-logging-structured.md) |
| D6 | Erişim | secret yalnız `l4` (bağlantı) ve `l3 crypto-sec` üzerinden okunur | [ADR-013](ADR-013-layered-architecture-52-nodes.md) |
| D7 | Geri alma | depo dosyası silinirse uygulama **değer ister**, asla yedeğini bulmaz | C2 |

**Neden:** Git geçmişinden secret silmek **mümkün değildir** (kopya her yerde kalır); bu yüzden tek geçerli strateji değeri **hiç yazmamaktır**. DPAPI, makine/OTURUM ile bağlı şifreleme sağlar: depo dosyası kopyalansa bile başka kullanıcıda açılmaz. `keys.md`'nin harita olması, ekibe "hangi anahtar hangi sağlayıcıda" cevabını verirken **sıfır** sızıntı riski taşır — harita ile değer aynı dosyada olsa bir süre sonra biri oraya yapışır.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Vault'ta/repoda secret = 0 | tarama: `.ai/**` içinde `sk-`/parola deseni = 0 satır |
| `keys.md` güvenlikli paylaşılabilir | dosyada değer yok, yalnız ad/yol |
| Çapraz makine sızıntısı kapalı | DPAPI kullanıcı kapsamı — kopya açılamaz |
| Sağlayıcı değişince tek dosya değişir | `keys.md` haritası + secret store kaydı |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Yedekleme/senkron zor (DPAPI makineye bağlı) | depo yedeklenmez; makinede yeniden girilir (D7) |
| Ekip paylaşımı yok (herkes kendi girer) | bilinçli: paylaşım git/repoya sızmaktan iyidir |
| Anahtar unutulursa uygulama başlamaz | net hata mesajı + `keys.md` haritası yönlendirir |
| DPAPI yalnız Windows | zaten hedef Windows (`net48`/`net10.0-windows`) |

**NEEDS_VALIDATION:** `ProtectedData` API'sinin `net48` **ve** `net10.0-windows` bacaklarında aynı kabuk ismiyle derlenip derlenmediği PART 5'te doğrulanır; farklılık olursa `l3 crypto-sec` içinde iki bacak köprüsü tanımlanır ve bu ADR **kendisi güncellenir** (yeni numara değil, `version` bump).

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | extends | DPAPI + "keys.md harita" ilkesinin kökeni |
| [ADR-011](ADR-011-llm-provider-order.md) | related | hangi anahtarın hangi sağlayıcıya gittiği |
| [ADR-026](ADR-026-logging-structured.md) | related | maskelemenin log formatındaki karşılığı |
| [ADR-016](ADR-016-no-ef-core-restated.md) | related | secret okuma yetkisinin katman sınırı |
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | related | `keys.md`'nin vault belgesi olarak kuralları |

---

## §5 Kabul Kriterleri

- [ ] `.ai/**` ve `src/**` içinde düz metin secret = **0 satır**
- [ ] `keys.md` içinde `sk-`/`ghp_`/parola değeri = **0 satır** (yalnız ad/yol)
- [ ] `secrets.bin` vault/repo **dışında** ve gitignore'da
- [ ] Log testi: secret değeri log çıktısında **maskeli**
- [ ] Bozuk/eksik secret'ta uygulama **değer ister**, yedek aramaz

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-005 §karar 5 + `CLAUDE.md` G9.
