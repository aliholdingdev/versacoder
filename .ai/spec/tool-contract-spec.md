---
title: "VersaCoder — Tool Çalışma Sözleşmesi (Girdi/Çıktı/Hata/Timeout)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Tool çağrısının çalışma sözleşmesi: girdi/çıktı zarfı davranışı, hata sınıfları, timeout/iptal/idempotentlik — ADR-006 (opencode tool seti → C#) ile tutarlı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Tool Çalışma Sözleşmesi (`spec/tool-contract-spec.md`)

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[architecture/index|architecture/index]] · [[spec/tool-spec|tool-spec]] · [[spec/protocol-layer-spec|protocol-layer-spec]] · [[spec/security-spec|security-spec]]

> **Tek cümlelik sorumluluk:** *Bir tool çağrısı hangi girdiyle başlar, hangi çıktı/zaman/asgari garantiyle biter, hata ve timeout'ta ne olur* sorusunun tek cevabıdır.
> **Tanım/şema/izin/sandbox** [[spec/tool-spec|tool-spec]]'tedir; **zarf tipi** [[spec/protocol-layer-spec|protocol-layer-spec]] §2'dedir — bu belge yalnız **çalışma (runtime) sözleşmesidir**.

---

## §0 Özet (Answer First)

Her tool çağrısı **tek çağrı kimliği (`callId`) + sıra no (`Seq`)** ile çalışır, **zorunlu `timeoutMs` + `CancellationToken`** taşır ve **tek sonuç** üretir (`ok`, `durationMs`, `content` ya da `error`); süre dolarsa süreç **tamamen öldürülür**, `idempotent: true` olanlar **tekrar denenebilir**, olmayanlar **yeniden denenmez** — hata kodu burada üretilmez, `protocol-spec` §5'teki `VC-E-02xx` kullanılır.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Girdi/çıktı zarfının **çalışma** davranışı (`callId`, `Seq`, `ok`, `durationMs`) | Şema/zorunlu alanlar: [[spec/tool-spec|tool-spec]] §2 |
| 2 | Timeout, iptal, idempotentlik, tekrar deneme | İzin matrisi + sandbox: [[spec/tool-spec|tool-spec]] §5–§6 |
| 3 | Hata **sınıfları** ve sahibi | Hata kodu listesi (`VC-E-02xx`): [[spec/protocol-spec|protocol-spec]] §5 |
| 4 | Sıralı çalışma (eşzamanlı = 1) ve `Seq` | Oturum/transcript yazımı: [[spec/session-spec|session-spec]] |
| 5 | Tool setinin C# karşılığı (ADR-006) | Sağlayıcı seçimi: [[spec/llm-provider-spec|llm-provider-spec]] |

> **YASAK:** Sonsuz bekleme (timeout'suz tool) · aynı `callId` ile ikinci sonuç · iptalsiz `Process` · hata kodu uydurmak (G9) · çıktıyı talimat olarak uygulamak (SEC-14).

---

## §2 Davranış

### §2.1 Çevrim içi girdi-çıktı

| Aşama | Alanlar | Garanti |
|---|---|---|
| Girdi | `callId`, `tool`, `arguments`, `Seq`, `timeoutMs` | `callId` oturumda **tekildir**; tekrarlı `callId` → ikinci sonuç **üretilmez** |
| Doğrulama | Şema + `path` kanonik kontrol | Başarısızsa tool gövdesi **çalışmaz** |
| Çalıştırma | `CancellationToken` + sandbox | Süreç/kaynak her durumda serbest bırakılır (`finally`) |
| Çıktı | `ok`, `content`/`contentRef`, `error`, `durationMs` | **Tek** sonuç; boş içerik `ok:false` değil `ok:true` + `content:""` olabilir |
| Kayıt | transcript girdisi (`Seq` artar) | UI'da çağrısız tool çalışmaz (şeffaflık) |

### §2.2 Timeout / iptal / tekrar

| Tool grubu | `timeoutMs` | Süre dolarsa | `idempotent` | Tekrar deneme |
|---|---|---|---|---|
| `read`, `search` | 15.000–30.000 | Sonuç `VC-E-0205` | `true` | Evet |
| `write`, `edit` | 15.000 | Kısmi yazım **geri alınır** | `write: true` / `edit: false` | Yalnız `write` |
| `shell` | **120.000** | `Kill(entireProcessTree)` + sonuç yok | `false` | **Hayır** |
| Varsayılan (tanımsız) | 30.000 | `VC-E-0205` | Tanıma göre | Tanıma göre |

```
  tool.call --▶ dogrulama --▶ timeout sayaci baslar
                    |                |
                  hata             sure doldu
                    v                v
        tek sonuc { ok:false }   surec oldur -> tek sonuc { ok:false, VC-E-0205 }
                    |                |
                    +-------+--------+
                            v
              transcript'e 1 kayit (Seq++)  -> model'e doner
```

### §2.3 Hata sınıfları (kodun sahibi protocol-spec §5)

| Sınıf | Ne kapsar | Davranış |
|---|---|---|
| Şema/parametre | Geçersiz girdi | Gövde çalışmaz, `ok:false` |
| İzin | `deny` / onay yok | Çalıştırılmaz, transcript'e `permission` girdisi |
| Sınır | Sandbox dışı yol/ağ | `ok:false` + audit |
| Süre | Timeout / iptal | Süreç tamamen öldürülür |
| Alt sistem | Dosya yok, komut yok | `ok:false` + kısa, maskelenmiş mesaj (SEC-13) |
| Bilinmeyen kod | — | Uydurma kod **yok** → `UNKNOWN` + `NEEDS_VALIDATION` |

---

## §3 TFM Notu

| Bacak | Fark | Kural |
|---|---|---|
| `net48` | `Process.Kill(entireProcessTree)`/async API farklılıkları olabilir | `#if` yalnız API farkında (CS-12) |
| `net10.0-windows` | Tam async I/O | Aynı sözleşme (timeout değeri dahil) |
| Ortak | Süre/iptal/idempotent davranış | İki bacakta **aynı** test beklentisi |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| TC-01 | Tek sonuç kuralı | Aynı `callId` ikinci kez → ikinci sonuç üretilmiyor |
| TC-02 | Shell timeout → süreç tamamen ölür | Uzun komut testi: `VC-E-0205` + artefakt yok |
| TC-03 | `idempotent:false` yeniden denenmez | Retry testi: ikinci çağrı yok |
| TC-04 | Sıralı çalışma `Seq` artar | Paralel iki çağrı → sıraya girer |
| TC-05 | İki bacakta aynı sözleşme | `dotnet test -f net48` / `-f net10.0-windows` = 0 failed |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
