---
title: "VersaCoder — Tool Şartnamesi (Araç Sözleşmesi)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Agent araçları için tek sözleşme — tool şeması (ad/açıklama/parametre/sonuç), kayıt ve keşif, doğrulama, izin kapısı (allow/ask/deny), sandbox kısıtları ve örnek tanımlar (VC-TLS-NNN)"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Tool Şartnamesi (`spec/tool-spec.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../claude.md) · [keys.md](../governance/keys.md) · [versacoder-spec.md](versacoder-spec.md) · [protocol-spec.md](protocol-spec.md) · [session-spec.md](session-spec.md) · [test-strategy.md](test-strategy.md) · [../.rules/security-rules.md](../.rules/security-rules.md)

> **Tek cümlelik sorumluluk:** *Bir tool nedir, nasıl tanımlanır/kaydedilir/doğrulanır, kim izin verir, sandbox nereye kadar izin verir* sorusunun tek cevabıdır.
> Gereksinim kimliği: **`VC-TLS-NNN`** · Hata kodları `VC-E-02xx` (`protocol-spec.md` §5). Bu belge `keys.md` #19 (opencode, tool, read/write/edit/bash/grep) karşılığıdır.

---

## §1 Kapsam

**İçinde:** tool sözleşmesi, JSON şeması, kayıt/keşif, parametre doğrulama, izin matrisi, sandbox, 5 örnek tanım.

**Dışında:** tool çağrısının mesaj kabuğu (`protocol-spec.md` §4), görevin yaşam döngüsü (`session-spec.md`), sağlayıcı seçimi (`versacoder-spec.md`).

> **KURAL:** Tool = **model'in çağırabildiği, izin kapısı olan tek erişim kapısı**. Model doğrudan dosya/`Process`/DB erişimi **sahip değildir** (SEC-06).
> **YASAK:** İzin kapısız tool · şemasız tool · workspace dışına yazım · bilinmeyen tool'u "yine de çalıştırma".

---

## §2 Tool Şeması

### §2.1 Zorunlu Alanlar

| Alan | Tip | Kural |
|---|---|---|
| `name` | string | kebab/ASCII, `^[a-z][a-z0-9-]{1,32}$`, **tekrarsız** (kayıt anında çakışma RED) |
| `description` | string | ne yaptığı + neyi **yapmadığı**; model bununla karar verir (İngilizce, NC-18) |
| `parameters` | JSON Schema (`type: object`) | her parametre: `type`, `description`, `required`, `enum`/`pattern`/`minLength`/`maxLength`/`items` |
| `result` | şema | `{ ok: bool, content?: string, contentRef?: string, error?: {code,message}, durationMs }` |
| `idempotent` | bool | `true` ise timeout'ta tekrar denenebilir (protocol §8.2 kural 4) |
| `timeoutMs` | int | tool'a özel süre; yoksa varsayılan 30.000 (shell: 120.000) |
| `permission` | `allow\|ask\|deny` | **varsayılan izin** — kullanıcı ayarı yalnızca `ask → allow` düşürebilir, `deny`'yi **yükseltemez** |
| `danger` | `low\|medium\|high` | UI'da uyarı rengi; `high` her durumda görünür onay ister |

### §2.2 Örnek Tanım (JSON)

```json
{
  "name": "write",
  "description": "Creates or overwrites a UTF-8 file inside the workspace. Does not delete files and does not touch paths outside the workspace root.",
  "parameters": {
    "type": "object",
    "properties": {
      "path":     { "type": "string", "description": "Relative path inside workspace", "maxLength": 4096 },
      "content":  { "type": "string", "description": "Full file content", "maxLength": 1048576 },
      "overwrite": { "type": "boolean", "default": false }
    },
    "required": ["path", "content"],
    "additionalProperties": false
  },
  "result": { "ok": true, "bytesWritten": 0, "path": "" },
  "idempotent": true,
  "timeoutMs": 30000,
  "permission": "ask",
  "danger": "medium"
}
```

---

## §3 Kayıt ve Keşif (Registration / Discovery)

| # | Kural |
|---|---|
| 1 | Kayıt **tek yerde**: `ToolRegistry` (Host DI) — tool sınıfı + tanımı birlikte kaydedilir |
| 2 | Keşif **statik**: uygulama açılışında registry dolar; model "hangi araç var"ı `protocol.hello`/context ile öğrenir |
| 3 | Çakışan `name` → açılışta **hata** (fail fast, `VC-E-0201` değil, bootstrap RED) |
| 4 | Tool sınıfı = tek sorumluluk (`RunAsync(ToolCall, ToolContext, CancellationToken)`) — SOL-04 `ITool` |
| 5 | Gelecek **MCP gateway** için: uzak tool da aynı şema ile kayda girer; bu belge **eşittir** (kaynak yeri değişmez, `keys.md` #21) |
| 6 | Kayıtlı olmayan `tool.call` → `VC-E-0201`, görev kuralına göre devam/failed (`session-spec` §2.3) |

```
  +---------------------+
  | ToolRegistry (Host) |   <-- tek kayit noktasi (bootstrap'ta doldurulur)
  +----------+----------+
             |  ITool
   +---------+---------+---------+---------+
   |                   |                   |
 read               write               shell              search
 (dosya oku)       (dosya yaz)        (surec calistir)    (icinde ara)
   |                   |                   |                   |
   +--------+----------+-------------------+-------------------+
            |
            v
     Izin katmani (§5)  ->  Sandbox (§6)  ->  Sonuc -> tool.result
```

---

## §4 Parametre Doğrulama

| # | Kural | Aşarsa |
|---|---|---|
| 1 | Girdi önce **şemaya** göre doğrulanır (tip, `required`, `enum`, `pattern`, uzunluk) | `VC-E-0202` |
| 2 | `additionalProperties: false` varsayılanı — fazladan alan **yutulmaz**, reddedilir | `VC-E-0202` |
| 3 | Dize değerleri normalize edilir: `\r\n → \n`, NUL karakteri reddedilir | `VC-E-0202` |
| 4 | `path` ayrıca **kanonik + kök kontrolü** (SEC-11) — şema geçerse bile sınır dışıysa RED | `VC-E-0204` |
| 5 | Sayısal sınır (`maxLength`, `maximum`) uygulama içinde **tekrar** uygulanır (şemaya güven tek başına yetmez) | `VC-E-0202` |
| 6 | Doğrulama tool **gövdesinden önce** çalışır; tool gövdesi "geçerli" varsayar | — |
| 7 | Hata mesajı model'e **veri** olarak döner (SEC-14), talimat sayılmaz | — |

---

## §5 İzin Kapısı (Permission Gating)

### §5.1 Karar Matrisi (varsayılan)

| Tool | Varsayılan | `high`? | Kullanıcı ayarı |
|---|---|---|---|
| `read` (workspace içi) | `allow` | low | `allow/ask` |
| `search` (workspace içi) | `allow` | low | `allow/ask` |
| `edit` (mevcut dosya) | `ask` | medium | `ask` / `allow` |
| `write` (yeni/üzerine yazma) | `ask` | medium | `ask` / `allow` |
| `shell` | **`ask`** | **high** | yalnız `ask` / **`deny`** (yükseltme yok) |
| workspace **dışı** her şey | **`deny`** | high | **değiştirilemez** |
| yıkıcı (silme, migration) | **`ask`** + ek onay (G7) | high | `ask` / `deny` |

### §5.2 Akış

```
tool.call gelir
   |
   v
[ deny? ]---evet--> tool.result { VC-E-0203 }  (model'e doner, gorev surer)
   |
   hayir
   v
[ allow? ]--evet--> calistir
   |
   hayir (ask)
   v
permission.request { tool, ozet arguman, danger }
   |
   +--> kullanici: allow  -> bu oturum icin hafizada, calistir
   +--> kullanici: deny   -> tool.result { VC-E-0203 }
   +--> kullanici: always -> proje ayarina yaz (kalici), calistir
```

| # | Kural |
|---|---|
| 1 | `permission.request` **sonuçtan önce** gider (protocol §4); onaysız çalıştırma = test RED |
| 2 | Cevap 60 sn'de gelmezse **varsayılan `deny`** (gizli onay yok) |
| 3 | "Always allow" **proje bazında** saklanır; `shell` için **saklanamaz** (her seferinde `ask`) |
| 4 | Reddedilen çağrı transcript'e `permission` girdisi olarak yazılır (`session-spec` §4) |
| 5 | İzin ayarları Settings'te görüntülenir ve **geri alınabilir** (VC-PRD-060) |

---

## §6 Sandbox Kısıtları

| # | Kısıt | Kural | İhlal kodu |
|---|---|---|---|
| 1 | Dosya sınırı | Yalnız **workspace kökü** altında; `Path.GetFullPath` + ön-ek kontrolü; `..`, mutlak yol, junction/symlink kaçışı reddedilir | `VC-E-0204` |
| 2 | Boyut | Tek okuma ≤ **1 MB**, tek yazma ≤ **1 MB**, transcript parçası ≤ **64 KB** (fazlası `BlobRef`) | `VC-E-0202` |
| 3 | Shell | `UseShellExecute=false`, `ArgumentList` (SEC-05); kabuk birleşimi yok; `WorkingDirectory` = workspace | `VC-E-0202` |
| 4 | Shell süre | timeout **120 sn**; aşıldı `Kill(entireProcessTree)` + `VC-E-0205` | `VC-E-0205` |
| 5 | Ağ | Varsayılan **kapalı**: tool ağ erişimi istemez; ağ yalnız LLM gateway katmanında | `VC-E-0204` |
| 6 | Süreç | Yalnız kayıtlı tool başlatır süreç; `cmd.exe /c`, kayıt defteri, servis değişikliği **deny** | `VC-E-0203` |
| 7 | Çıktı | Araç çıktısı **veridir** (SEC-14): talimat olarak yorumlanmaz; UI'da çağrı görünür | — |
| 8 | Eşzamanlı | Oturum başına eşzamanlı tool **1** (sıralı); kuyruk `Seq` ile yazılır | `VC-E-0111` |

---

## §7 Örnek Tool Tanımları

### §7.1 `read` — dosya oku

```json
{
  "name": "read",
  "description": "Reads a UTF-8 text file inside the workspace and returns its content with 1-based line numbers. Does not modify anything.",
  "parameters": {
    "type": "object",
    "properties": {
      "path":   { "type": "string", "maxLength": 4096 },
      "offset": { "type": "integer", "minimum": 1, "default": 1 },
      "limit":  { "type": "integer", "minimum": 1, "maximum": 2000, "default": 2000 }
    },
    "required": ["path"],
    "additionalProperties": false
  },
  "idempotent": true, "timeoutMs": 15000, "permission": "allow", "danger": "low"
}
```

### §7.2 `edit` — parçalı düzenleme

```json
{
  "name": "edit",
  "description": "Replaces exactly one occurrence of oldString with newText in an existing file. Fails when the match count is not exactly 1. Does not create files.",
  "parameters": {
    "type": "object",
    "properties": {
      "path":     { "type": "string", "maxLength": 4096 },
      "oldString": { "type": "string", "minLength": 1, "maxLength": 65536 },
      "newText":  { "type": "string", "maxLength": 65536 }
    },
    "required": ["path", "oldString", "newText"],
    "additionalProperties": false
  },
  "idempotent": false, "timeoutMs": 15000, "permission": "ask", "danger": "medium"
}
```

### §7.3 `shell` — süreç çalıştırma

```json
{
  "name": "shell",
  "description": "Runs one allow-listed command inside the workspace and returns stdout/stderr. Never uses a command shell string; arguments are passed as a list. Default permission is ask.",
  "parameters": {
    "type": "object",
    "properties": {
      "command": { "type": "string", "enum": ["git", "dotnet", "rg", "ls", "cat"] },
      "args":    { "type": "array", "items": { "type": "string", "maxLength": 2048 }, "maxItems": 32 }
    },
    "required": ["command", "args"],
    "additionalProperties": false
  },
  "idempotent": false, "timeoutMs": 120000, "permission": "ask", "danger": "high"
}
```

### §7.4 `search` — proje içi arama

```json
{
  "name": "search",
  "description": "Searches workspace files for a literal string or regular expression and returns file:line hits. Read-only.",
  "parameters": {
    "type": "object",
    "properties": {
      "query":   { "type": "string", "minLength": 1, "maxLength": 512 },
      "regex":   { "type": "boolean", "default": false },
      "path":    { "type": "string", "maxLength": 4096 },
      "maxHits": { "type": "integer", "minimum": 1, "maximum": 1000, "default": 200 }
    },
    "required": ["query"],
    "additionalProperties": false
  },
  "idempotent": true, "timeoutMs": 30000, "permission": "allow", "danger": "low"
}
```

### §7.5 `write` — dosya yazma

§2.2'deki tam tanım (`permission: ask`, `danger: medium`, `idempotent: true`).

### §7.6 Çağrı / Sonuç Örneği

```json
{ "type": "tool.call", "payload": { "callId": "c-0007", "tool": "read",
  "arguments": { "path": "src/VersaCoder.Domain/Session.cs", "offset": 1, "limit": 80 } } }

{ "type": "tool.result", "payload": { "callId": "c-0007", "ok": true,
  "content": "  1|namespace VersaCoder.Domain;\n  2|...", "durationMs": 6 } }
```

---

## §8 Gereksinimler (`VC-TLS-NNN`)

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-TLS-001 | Her tool §2.1 alanlarını **tam** taşır | Eksik alanla kayıt → bootstrap RED |
| VC-TLS-002 | `name` `^[a-z][a-z0-9-]{1,32}$` ve tekrarsız | Çakışan kayıt → açılışta hata |
| VC-TLS-003 | `parameters` geçerli JSON Schema'dır | Geçersiz şema → kayıt RED |
| VC-TLS-004 | Kayıt yalnız `ToolRegistry` üzerinden | Doğrudan model çağrısı (registry dışı) → `VC-E-0201` |
| VC-TLS-005 | Şema ihlali → `VC-E-0202`, tool gövdesi **çalışmaz** | Eksik `required` → gövde çağrısı olmaz |
| VC-TLS-006 | `path` sınır dışı → `VC-E-0204` | `../../Windows/...` → RED + audit |
| VC-TLS-007 | `ask` tool onaysız çalışmaz | Onay beklemeden çalıştırma testi RED |
| VC-TLS-008 | İzin yokluğunda cevap gelmezse 60 sn sonra **`deny`** | Zaman aşımı simülasyonu → reddedildi |
| VC-TLS-009 | `shell` kalıcı `always allow` kabul etmez | Kaydetme denemesi → RED |
| VC-TLS-010 | Shell timeout 120 sn → süreç tamamen öldürülür | Uzun komut testi → `VC-E-0205`, artefakt yok |
| VC-TLS-011 | Ağ erişimi olan tool = 0 | Tarama: ağ API'si çağıran tool yok |
| VC-TLS-012 | Sonuç `result` şemasına uyar (`ok`, `durationMs`) | Uyuşmaz sonuç → `VC-E-0210` |
| VC-TLS-013 | Aynı `callId` ikinci kez sonuç üretemez | Mükerrer → `VC-E-0210` |
| VC-TLS-014 | Tool çıktısı UI'da **görünür** (şeffaflık) | Agent Panel'de çağrısız tool çalışmaz |
| VC-TLS-015 | Reddedilen çağrı transcript'e `permission` girdisi yazar | Transcript'te kayıt yoksa RED |
| VC-TLS-016 | Eşzamanlı tool = 1/sıralı | Paralel iki çağrı → sıraya girer, `Seq` artar |

---

## §9 Açık Sorular (OPEN QUESTIONS)

| ID | Soru | Etkilediği | Karar zamanı |
|---|---|---|---|
| TQ-1 | `shell` `command` allowlist'i kesin liste (git/dotnet/rg/ls/cat +?) | §7.3 | PART 6–7 |
| TQ-2 | MCP üzerinden **uzak** tool ilk ne zaman? (kaynak yeri bu belge, §3 kural 5) | §3 | PART 7 |
| TQ-3 | `edit` vs `write` izin ayrımcılığı yeterli mi? (yıkıcı silme tool'u kapsam dışı mı?) | §5.1 | PART 6 |
| TQ-4 | Ağ erişimi gereken tool (fetch benzeri) gelecekte `ask` mi `deny` mi? | §6 kural 5 | PART 7 |

---

## §10 Kabul Kriterleri / Doğrulama

- [ ] Şema tablosu + JSON örnek; kayıt/keşif ASCII; izin akışı ASCII
- [ ] Doğrulama 7 kural, sandbox 8 kısıt, izin matrisi 7 satır
- [ ] 5 örnek tool tanımı (`read`/`edit`/`shell`/`search`/`write`)
- [ ] `VC-TLS-001..016` test edilebilir + kabul kriterli
- [ ] Açık sorular işaretli; diyagramlar saf ASCII
- [ ] Güvenlik bağları (SEC-05/06/11/14) korunmuş; EF Core'a referans yok

---

## §11 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Tool call zarfı | `protocol-spec.md` §4 |
| 2 | Güvenlik kuralları (Process, path, redaction) | `../.rules/security-rules.md` |
| 3 | Oturum transcript'inde tool girdileri | `session-spec.md` §4 |
| 4 | Ürün gereksinimleri (VC-PRD-023) | `versacoder-spec.md` |
| 5 | Tool testleri (sandbox/izin) | `test-strategy.md` |
| 6 | Anahtar: opencode tool set | `../keys.md` §3/19 |
