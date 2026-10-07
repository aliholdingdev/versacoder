---
title: "ADR-011 — LLM Sağlayıcı Sırası ve Zorunlu Streaming"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "LLM erişim sırası (OpenRouter /free → Ollama → OpenAI-uyumlu), ortak sözleşme ve streaming zorunluluğu"
hedef_bacak: "net48 + net10.0-windows"
evidence: "adr-index.md §2 (ADR-011 satırı) · reports/01-web-kaynaklari.md · ADR-018 (secret store)"
---

# ADR-011 — LLM Sağlayıcı Sırası ve Zorunlu Streaming

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-018](ADR-018-secret-store.md) · [ADR-008](ADR-008-transcript-compaction.md)

> **Tek cümlelik karar:** LLM erişimi **OpenRouter `/free` (ilk doğrulama) → Ollama (yerel) → OpenAI-uyumlu herhangi bir uç nokta** sırasıyla denenir, **streaming (SSE) her sağlayıcıda zorunludur** ve API anahtarı yalnız secret store'dan okunur — karar verildi.

---

## §0 Özet (Answer First)

Tek sağlayıcıya kilitlenmek, o sağlayıcı düştüğünde ürünü durdurur; bu yüzden sözleşme **tek** (`ILLMProvider`), **uygulama üç**tür. Doğrulama sırası maliyet sırasıdır: önce ücretsiz OpenRouter modelleriyle iskelet test edilir, sonra yerel Ollama ile çevrimdışı/Low maliyetli devam, son olarak OpenAI-uyumlu herhangi bir uç nokta (kendi sunucunuz dahil). **Streaming olmadan sağlayıcı kabul edilmez** — arayüzde tek parça yanıt bekleyen tasarım, ikinci sağlayıcı eklerken yeniden yazım ister. Anahtarlar [ADR-018](ADR-018-secret-store.md) gereği kodda/vault'ta **düz metin durmaz**.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Master prompt PART 7: `ILLMProvider` tek contract + sıralı doğrulama istiyor | `adr-index.md` §2 · ADR-011 |
| C2 | İlk doğrulama ücretsiz katmanla yapılır (OpenRouter `/free`), maliyet sıfır | `adr-index.md` §2 (ADR-011 beklentisi) |
| C3 | Ajan döngüsü yanıtı kullanıcıya **akış halinde** göstermek zorunda (tek parça yanıt UI'ı kilitleşir) | `AGENTS.md` §6.2/5 (uzun çıktı parçalanır) |
| C4 | API anahtarları vault'ta düz metin duramaz | [ADR-018](ADR-018-secret-store.md) · `CLAUDE.md` G9 |
| C5 | Transcript boyutu bayt ile ölçülür; token hesabı sağlayıcıya göre değişir | [ADR-008](ADR-008-transcript-compaction.md) D1 |
| C6 | Sağlayıcı hataları döngüyü kilitlememeli; araç hata = değer ilkesi | [ADR-006](ADR-006-opencode-tool-contract.md) D4 |

---

## §2 Karar

**Karar:** Tek `ILLMProvider` sözleşmesi tanımlanır, **öncelik sırası OpenRouter `/free` → Ollama → OpenAI-uyumlu uç nokta** olarak sabitlenir ve **streaming zorunlu** kılınır; sıradaki sağlayıcıya geçiş yapılandırmayla olur, kod değişikliğiyle değil — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Sözleşme | `ILLMProvider { StreamAsync(request, ct) → IAsyncEnumerable<Chunk> }` | C3 · C6 |
| D2 | Öncelik 1 | **OpenRouter `/free`** — iskelet + kontrakt testleri, maliyet 0 | C2 |
| D3 | Öncelik 2 | **Ollama** (yerel) — çevrimdışı geliştirme, veri dışarı çıkmaz | C2 |
| D4 | Öncelik 3 | **OpenAI-uyumlu** herhangi bir uç nokta (kendi sunucumuz dahil) | C1 |
| D5 | Streaming | **zorunlu**; tek parça yanıt (`non-streaming`) implementasyonu **RED** | C3 |
| D6 | Anahtar | yalnız secret store üzerinden okunur; istek log'a yazılırken maskelenir | C4 |
| D7 | Sıra başarısızsa | sıradaki sağlayıcıya geç; üçü de yoksa `ToolError`-benzeri **değer** hatası | C6 |
| D8 | Zaman aşımı | sağlayıcı çağrısı `CancellationToken` + timeout tablosu (bkz. [ADR-009](ADR-009-tool-execution-sandbox.md)) | C6 |

**Neden:** Sıra, **maliyet → izolasyon → genişleme** mantığıyla kurulur: önce bedava doğrula, sonra veriyi yerelde tut, en son üreticiye bağlan. Streaming'in zorunlu olması, UI ile protokolü ayırır: ajan döngüsü her koşulda parça parça token alır, sağlayıcı farkı arayüzden görünmez. Anahtarın secret store'a hapsi, [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 5 ile aynı ilkeyi sürdürür.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Tek sözleşme → yeni sağlayıcı eklemek kod değil yapılandırma işi | 1 implementasyon × 3 yapılandırma |
| Bedava ilk doğrulama → iskelet maliyeti 0 | OpenRouter `/free` smoke testi |
| Çevrimdışı geliştirme mümkün | Ollama ayağa kalktığında ağ kesilse de döngü çalışır |
| UI asla "tek parça yanıt" beklemez | streaming kontrakt testi (ilk chunk ≤ 5 sn) |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| 3 sağlayıcı = 3 entegrasyon yüzeyi | hepsi tek sözleşmeye uyar; kontrakt testi ortak |
| `/free` modeller değişebilir/kalkabilir | D7 sıralı geçiş; model listesi yapılandırmadadır |
| Streaming + SSE ayrıştırması hata üretir | ayrıştırıcı birim testi; chunk kaybı testi |
| Sağlayıcı davranışı (context penceresi) farklı | **NEEDS_VALIDATION** (aşağıda) |

**NEEDS_VALIDATION:** Üç sağlayıcının **context penceresi ve token sayımı** farklıdır → PART 7'de OpenRouter `/free` ve Ollama üzerinde ölçülür, değerler `reports/` raporuna yazılır. Ölçüm gelene kadar [ADR-008](ADR-008-transcript-compaction.md) bayt eşiği (8 MB) geçerlidir; token'a çevrilmez.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-018](ADR-018-secret-store.md) | related | API anahtarının saklandığı yer |
| [ADR-008](ADR-008-transcript-compaction.md) | related | token/bayt ölçümünün bu ADR'de kapanması |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | related | ağ/timeout/iptal kuralları (D8) |
| [ADR-006](ADR-006-opencode-tool-contract.md) | related | hata = değer ilkesinin protokol tarafı |
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | related | secret tutma ilkesinin devamı |

---

## §5 Kabul Kriterleri

- [ ] `ILLMProvider` tek sözleşme; 3 implementasyon kontrakt testinden geçiyor
- [ ] Varsayılan öncelik sırası yapılandırmada yazılı: `openrouter-free → ollama → openai-compatible`
- [ ] Streaming olmayan implementasyon kod incelemesinde **RED**
- [ ] Log satırında anahtar **maskeli** (`sk-***` biçiminde)
- [ ] Üç sağlayıcı da `CancellationToken` iptalinde temiz kapanıyor (test)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + `adr-index.md` §2.
