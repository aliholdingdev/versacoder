---
title: "C# / .NET Knowledge Index"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "knowledge/csharp altindaki 18 teknik dokumanin kayit defteri: iki bacak (net48 + net10.0-windows) dili, durum ve izinli link hedefleri"
---

# C# / .NET Knowledge Index

**Zorunlu Bağlantılar:** [../index.md](../index.md) · [../../index.md](../../index.md) · [../../CLAUDE.md](../../CLAUDE.md) · [../../keys.md](../../keys.md) · [coding-standards](../../.rules/coding-standards.md) · [ADR-012](../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md)

> **Tek cümlelik sorumluluk:** *Bu vault'ta hangi C#/.NET özelliği iki bacakta nasıl davranır, o cevap hangi dosyada* sorusunun tek cevabıdır.
> Bağlayıcı kural `.rules/coding-standards.md`'dedir; burada yalnız **kayıt + yönlendirme** vardır (SRP).

---

## §0 Özet (Answer First)

Bu dizin `.ai/knowledge/csharp/` altındaki **18 teknik dokümanın** kayıt defteridir. Tamamı **iki bacak** (`net48` + `net10.0-windows`) boyunca cevap verir: hangi dil özelliğinin hangi bacakta derlendiği, hangi API'nin `net48`'de paket istediği ve `#if`'in neden yalnız platform farkıyla sınırlı olduğu (G6). Öncelik sırası: **anayasa (G1..G12) > `.rules/coding-standards.md` > bu dizindeki anlatım** — çelişirse kural kazanır, dosya `NEEDS_VALIDATION` ile işaretlenir.

---

## §1 Dosya Kayıt Defteri (18)

| # | Dosya | Sorumluluk | Durum |
|---|---|---|---|
| 1 | `async-await-basics.md` | async tüm yol kuralı, `ConfigureAwait` seçimi | ✅ |
| 2 | `cancellation-token-policy.md` | `CancellationToken` imzası, iptal zinciri, eşleşme hükmü | ✅ |
| 3 | `channel-queue-patterns.md` | `System.Threading.Channels` kuyruk; `net48` paket farkı | ✅ |
| 4 | `collections-choice.md` | Koleksiyon seçimi, kapasite yönetimi, iki bacak farkı | ✅ |
| 5 | `exception-handling-patterns.md` | `catch` sırası, `throw` vs `throw ex`, `when` filtresi | ✅ |
| 6 | `generics-constraints.md` | `where` kılavuzları; C# 7.3 ve sonraki sürümlerin dağılımı | ✅ |
| 7 | `interfaces-vs-abstract.md` | `interface` vs `abstract`, default interface metodu | ✅ |
| 8 | `language-versions-net48-net10.md` | `net48` → C# 7.3, `net10` → C# 14; özellik bulunabilirliliği | ✅ |
| 9 | `linq-performance-pitfalls.md` | LINQ tuzakları: erteleme, tekrar sayım, kapalı ifade | ✅ |
| 10 | `multi-targeting-props.md` | `Directory.Build.props/.targets`, TFM koşulları, `LangVersion`, CPM | ✅ |
| 11 | `nullable-reference-types.md` | NRT geçiş durumu, `#nullable enable`, iki bacakta hükmü | ✅ |
| 12 | `pattern-matching.md` | Kalıp eşleme sürümleri (C# 7–14) ve bacak dağılımı | ✅ |
| 13 | `records-and-immutability.md` | `record`, value-equality, `with`; `net48` karşılığı | ✅ |
| 14 | `result-type-design.md` | `Result`/`ErrorCode` sözleşmesi (l0-abstractions N02 ile tutarlı) | ✅ |
| 15 | `serialization-choices.md` | JSON/XML seçimi, `BinaryFormatter` yasağı, paket farkı | ✅ |
| 16 | `span-interop-net10.md` | `Span`/`Memory`, P/Invoke marshalling, `System.Memory` farkı | ✅ |
| 17 | `task-vs-value-task.md` | `Task` vs `ValueTask`, tekrar `await` kuralı, alloc maliyeti | ✅ |
| 18 | `threading-locking.md` | `lock`/`Monitor`/`Interlocked`/`ReaderWriterLockSlim`, ölü kilit | ✅ |

---

## §2 Kullanım (neyi arıyorsan)

| İhtiyaç | Gidilecek dosya |
|---|---|
| Bu özellik `net48`'de var mı? | `language-versions-net48-net10.md`, `generics-constraints.md` |
| `async void` / iptal nasıl? | `async-await-basics.md`, `cancellation-token-policy.md` |
| `#if` ne zaman yazabilirim? | `.rules/coding-standards.md` + `multi-targeting-props.md` |
| Hata sözleşmesi `Result` mı? | `result-type-design.md` + `.rules/error-handling-rules.md` |
| Derleme hatası / TFM koşulu | `multi-targeting-props.md` |

---

## §3 İzinli Link Hedefleri

| Hedef | Durum |
|---|---|
| `../../index.md` · `../index.md` · `../../CLAUDE.md` · `../../keys.md` · `../../VISION.md` | Var → link verilir |
| `../../.rules/coding-standards.md` · `testing-rules.md` · `data-access-rules.md` | Var → link verilir |
| `../../.rules/security-rules.md` · `devexpress-rules.md` | Var → link verilir |
| `../../.decisions/accepted/ADR-004…` · `ADR-005…` · `ADR-012…` | Var → link verilir |
| §1'de adı geçen 18 kardeş dosya | Var → adıyla (dizin 3–8 KB sınırı) |
| Bunun dışındaki hedef (ör. `ADR-001…`, `../../.rules/ui-rules.md`) | **Yasak** → link kırılır (GATE-004a) |

---

## §4 Yasaklar

1. §1'de kaydı olmayan dosya eklemek (önce satır, sonra içerik).
2. §3'te yasak yazan hedefe link vermek (izin listesi bilinçli olarak dar).
3. Kardeş dosya adını yanlış yazmak (ad = link).
4. Doğrulamadan ✅ işaretlemek ya da iki bacaktan birini atlayarak anlatmak (G6).

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `knowledge/csharp/*.md` frontmatter `scope:`.
