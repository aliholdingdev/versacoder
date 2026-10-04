---
title: "N34 Mesaj Tipleri"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N34 - MessageEnvelope karesi, ContentPart/ToolCall/StreamDelta, round-trip serializer (kayit tablosu DEGIL)
---

# N34 Mesaj Tipleri

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N33-tool-schema.md](N33-tool-schema.md)
- [N35-session-transcript.md](N35-session-transcript.md)
- [N50-macro-l5.md](N50-macro-l5.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N34, bir mesajin tek kare (envelope) icinde hangi alanlari tasidigini ve JSON'a nasil kayipsiz gittigini (round-trip) tanimlar.

## §0 Ozet (Answer First)

- Tum iletisim `MessageEnvelope` icinde toplanir: `protocolVersion` + `role` + `content[]` (parcalar `ContentPart` / `IToolCallPart` / `IToolResultPart` / `IStreamDelta`).
- JSON <-> tip donusumu **round-trip ile kanitlanir**; `role` veya `protocolVersion` OLMAYAN mesaj gecerli sayilmaz.
- Sahip oldugu nuans: UTF-8 (Turkce, emoji) metin iki bacakta da bozulmaz - kodlama testi zorunlu; ham metin `MALFORMED` durumunda saklanmaz.
- Ilerleme kosulu: 4 rol x 4 parca tipi round-trip matrisi (§9 P34-1, P34-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `record/interface` | `MessageEnvelope.ProtocolVersion` / `.Role` / `.Content` | tek kareli mesaj |
| `enum` | `MessageRole { User, Assistant, Tool, System }` | rol kumesi (sabit) |
| `interface` | `IContentPart.Kind` / `.Text` / `.Ref` | metin, dosya, gomulu veri |
| `interface` | `IToolCallPart.CallId` / `.ToolName` / `.ArgumentsJson` | cagri kaydi |
| `interface` | `IMessageSerializer.Serialize(envelope)` / `.Deserialize(json)` | round-trip |

```csharp
public interface IMessageSerializer
{
    string Serialize(MessageEnvelope e);
    Result<MessageEnvelope> Deserialize(string json);   // hata = Result, exception DEGIL
}
// Kural: Serialize(Deserialize(x)) birebir ayni gorunumu uretir (round-trip).
// role ve protocolVersion OLMAYAN mesaj gecerli sayilmaz.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| ToolCall / sonuc | N33 / N32 (l4) | `IToolCallPart` / `IToolResultPart` olarak kareye |
| Kullanici metni | N43 AI Chat (l7) | `role=User` ile `ContentPart` |
| Stream parcalari | N31 (l4) | `IStreamDelta` - canli akis kaydi |
| `protocolVersion` | N37 | Her zorunlu mesajda zorunlu alan |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `MessageEnvelope` (JSON) | N35 `SessionState.messages[]` | transkript kaydi |
| `Result<MessageEnvelope>` | cagiran (l6 dongu / l7 gosterim) | hata = Result |
| `ProtocolViolation(INVALID_ROLE/VERSION_MISSING/UNKNOWN_PART/ORPHAN_RESULT/ENVELOPE_TOO_LARGE/MALFORMED)` | N37 | kod tek katalogda |
| Serialize JSON ciktisi | iki bacak ayni | metin karsilastirma testi |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N34 notu |
|---|---|---|
| l5 -> l6, l5 -> l7 | X | dongu/gosterim paketi okunmaz |
| l5 -> l4 | **?** | DOĞRULAMA GEREKLİ - ureten tarafla iliski port uzerinden |
| l5 -> l3 | X | Crosscutting tasiyamaz |
| l5 -> l0, l1, l2 | . | gecisli; `Result`/`Error` l0 uzerunden |
| l4 -> l5 | YOK (N31 notu) | iliski tek yonlu: kare **uretilmez**, yalnizca tuketilir |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N34 | Mesaj tipleri | Bu dosya |
| N33 | Arac semasi | `IToolCallPart` alanlari |
| N35 | Oturum transkripti | `messages[]` ici kareler |
| N37 | Surum/hata | `protocolVersion` denetimi |
| N50 | Makro l5 | `IMessageKit` / `IProtocolCore` |

## §6 Projeler

| Proje | N34 katkisi |
|---|---|
| `VersaCoder.Protocol` | Envelope, part tipleri, serializer (tek uretim projesi) |
| `VersaCoder.Tests` | Round-trip + UTF-8 + parite testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: serializer'da dosya/HTTP/SQL cagrisi = 0 |

TFM: `net48;net10.0-windows` - `record` yalniz `#if NET` (net48'de `sealed class` + manuel `Equals`), serializer ayni `System.Text.Json` surumu (G8).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l5 -> l0 | `Result` / `Error` (`VersaCoder.Abstractions`) | izinli |
| l5 -> l1, l2 | tipler (matriste `.`) | gecisli |
| l5 -> l3 | `VersaCoder.Crosscutting` | matris **X** - yasak |
| l5 -> l4 | `VersaCoder.Infrastructure` | **?** - DOĞRULAMA GEREKLİ |
| harici | `System.Text.Json` (NuGet) | surum sabit (G8) |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Round-trip matrisi | `dotnet test --filter Category=Protocol` | 4 rol x 4 parca |
| Zorunlu alan (protocolVersion sil) | Theory | reddedilir |
| Orphan result (CallId eslesmez) | birim test | ORPHAN_RESULT |
| UTF-8 kodlama | birim test | Turkce + emoji metin |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N34 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P34-1 | `dotnet build VersaCoder.Protocol` | 0 hata |
| P34-2 | Round-trip: Serialize(Deserialize(x)) == x | 16/16 vaka - **DOĞRULAMA GEREKLİ** |
| P34-3 | protocolVersion silinmis mesaj | reddedilir |
| P34-4 | iki bacak JSON ciktisi karsilastirma testi | 0 fark |
| P34-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
