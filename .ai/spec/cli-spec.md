---
title: "VersaCoder CLI - Komut Satiri Arayuzu Spec"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-02
authority: primary
scope: "VersaCoder CLI'nin komut agaci, arguman sozdizimi, cikti bicimlendirme, exit code, config dosyasi ve terminal UI entegrasyonu - VC-CLI-001..016 gereksinimleri"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder CLI — Komut Satırı Arayüzü Spec

**Zorunlu Baglantilar:** [[index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[spec/terminal-spec|terminal-spec]] · [[spec/tool-spec|tool-spec]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/ui-shell-spec|ui-shell-spec]] · [[.rules/testing-rules|testing-rules]] · [[.rules/security-rules|security-rules]]

> **Tek cümlelik sorumluluk:** *CLI hangi komutları alır, nasıl çözümlenir, ne döner ve terminal UI'ya nasıl bağlanır* sorusunun tek cevabı.
> Gereksinim kimliği: **`VC-CLI-NNN`** · Hata kodları `VC-E-05xx` · Bağımlılık: [[spec/terminal-spec|terminal-spec]] (görüntüleme) + [[spec/tool-spec|tool-spec]] (izin).

---

## §0 Özet (Answer First)

`VersaCoder.CLI` (K0750-K0799, Exe) = **tek giriş noktası**; 6 ana komut (`new`, `build`, `run`, `layer`, `agent`, `vault`) + `--help`/`--version` global bayraklar. Çıktı **3 biçimde** (human / json / quiet), exit code **0-5**. CLI, **terminal UI**dan bağımsız çalışır ama [[spec/terminal-spec|terminal-spec]] onu dock paneline gömer.

| Alan | Değer | Kaynak |
|---|---|---|
| Proje | `VersaCoder.CLI` · K bandı `K0750-K0799` | [[architecture/layer-5000/index|k5000-dll-list]] §2.4 |
| TFM | `net48;net10.0-windows` (çift bacak, G6) | ADR-001 |
| Gereksinim | `VC-CLI-001..016` (16 madde) | bu dosya §2 |
| Hata kodu | `VC-E-0500..0599` | bu dosya §4 |
| Exit code | 0=OK, 1=usage, 2=runtime, 3=notfound, 4=perm, 5=cancelled | bu dosya §4.1 |
| Terminal bağlantısı | `run` → çıktı satırı → `MemoEdit` (terminal-spec §2.1) | [[spec/terminal-spec|terminal-spec]] |
| Güvenlik | izin kapısı [[spec/tool-spec|tool-spec]] `allow/ask/deny` | SEC-06 |

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Komut ağacı, arguman çözümleme, exit code | Komutların **iş mantığı** (build/run/agent) | ilgili spec (`versacoder-spec`, `session-spec`) |
| 2 | `--format human/json/quiet` çıktı biçimlendirme | Terminal **görüntüleme** (dock, scroll, renk) | [[spec/terminal-spec|terminal-spec]] |
| 3 | `run` komutunun terminal UI'ya bağlanması | Terminal **izin** matrisi (hangi komut çalıştırılabilir) | [[spec/tool-spec|tool-spec]] |
| 4 | `~/.versacoder/config.json` + proje `.versacoder/` okuma | Config **yazma** (ayarlar ekranı) | `07-settings` |
| 5 | `layer` komutu (katalog sorgu) | Layer **oluşturma** (LAY-06, gate) | [[.rules/layer-rules|layer-rules]] |

> **YASAK:** CLI doğrudan dosya/DB/LLM erişimi yapmaz — hep **port** (l0) üzerinden; izin **tool-spec**'ten geçer (SEC-06).

---

## §2 Gereksinimler (`VC-CLI-001..016`)

### §2.1 Komut ağacı (VC-CLI-001..007)

| ID | Komut | Ne yapar | Çıktı | Kanıtı |
|---|---|---|---|---|
| `VC-CLI-001` | `versacoder new <name>` | Yeni proje iskeleti üretir (`.sln`, `.csproj`, `layer.json`) | proje yolu | `new` sonrası dosya ağacı = spec |
| `VC-CLI-002` | `versacoder build [solution]` | `dotnet build` çağırır, çıktıyı satır satır akıtır | exit code + hata özeti | `build` exit code = dotnet exit code |
| `VC-CLI-003` | `versacoder run <target>` | Uygulama/agent çalıştırır, **terminal UI'ya** çıktı akıtır | satır akışı (terminal-spec) | terminal `MemoEdit`'te satır sayısı > 0 |
| `VC-CLI-004` | `versacoder layer list` | `layer.json` katalogunu sorgular (filtre: band, status) | tablo / JSON | `layer list --format json` = 5000 kayıt |
| `VC-CLI-005` | `versacoder layer show <K####` | Tek layer'ın 12 alanını gösterir | 12 alan | `layer show K0000` = 12 satır |
| `VC-CLI-006` | `versacoder agent run <role>` | Agent rolünü başlatır (task-queue'ya düşer) | task ID + akış | agent panelde task görünür |
| `VC-CLI-007` | `versacoder vault <search\|read\|write>` | Vault arama/oku/yaz (SSOT) | sonuç / kayıt | `vault search layer` = ≥1 sonuç |

### §2.2 Global bayraklar + arguman (VC-CLI-008..011)

| ID | Bayrak | Ne yapar | Varsayılan | Kanıtı |
|---|---|---|---|---|
| `VC-CLI-008` | `--help` / `-h` | Komut ağacı + örnek gösterir | — | `versacoder --help` = 6 komut listesi |
| `VC-CLI-009` | `--version` / `-v` | Sürüm bilgisi | — | `versacoder -v` = `1.0.0` |
| `VC-CLI-010` | `--format <human\|json\|quiet>` | Çıktı biçimi | `human` | `--format json` = geçerli JSON |
| `VC-CLI-011` | `--timeout <sn>` | Maksimum süre (sonra kill) | 300 | timeout sonrası exit 5 |

### §2.3 Çıktı + config + terminal (VC-CLI-012..016)

| ID | Konu | Kural | Kanıtı |
|---|---|---|---|
| `VC-CLI-012` | Çıktı biçimi | `human` = renkli tablo; `json` = tek obje; `quiet` = yalnız exit code | 3 biçimde de exit code aynı |
| `VC-CLI-013` | Config | `~/.versacoder/config.json` (global) + `<proje>/.versacoder/config.json` (proje, globalı ezer) | config okunur, eksik alan default |
| `VC-CLI-014` | Geçmiş | `run` komutu geçmişe yazılır (`history.txt`, 1000 satır, FIFO) | `history` = 1000 satır |
| `VC-CLI-015` | Terminal entegrasyonu | `run` çıktısı **satır bazlı** terminal `MemoEdit`'e akar; Ctrl+C = cancel (exit 5) | terminal-spec §2.2 + §2.4 |
| `VC-CLI-016` | İzin kapısı | `run`/`agent` komutları [[spec/tool-spec|tool-spec]] `ask`/`allow`/`deny` geçer; `deny` = exit 4 | izin reddi = exit 4, terminal "permission denied" |

---

## §3 Komut Ağacı (görsel)

```text
versacoder
  +-- new <name>              [VC-CLI-001]
  +-- build [solution]        [VC-CLI-002]
  +-- run <target>            [VC-CLI-003]  ---> terminal UI (MemoEdit satir akisi)
  +-- layer
  |     +-- list [--band] [--status]   [VC-CLI-004]
  |     +-- show <K####>              [VC-CLI-005]
  +-- agent
  |     +-- run <role>                 [VC-CLI-006] ---> task-queue + agent panel
  |     +-- status [task-id]
  +-- vault
  |     +-- search <query>             [VC-CLI-007]
  |     +-- read <layer|adr|spec>
  |     +-- write <file>
  +-- history                         [VC-CLI-014]
  +-- config <get|set>                [VC-CLI-013]
  +-- --help | --version              [VC-CLI-008..009]
  +-- --format | --timeout            [VC-CLI-010..011]
```

**Örnek:**

```bash
versacoder new MyApp                    # proje iskeleti
versacoder build                        # dotnet build, terminal'de akis
versacoder layer list --band Foundation # 45 kayit, tablo
versacoder layer show K0000 --format json
versacoder run dotnet -- test --filter "FullyQualifiedName~Layer"
versacoder agent run developer
versacoder vault search "K2105"
```

---

## §4 Hata Kodları (`VC-E-0500..0599`)

| Kod | Anlam | Örnek | Exit |
|---|---|---|---|
| `VC-E-0501` | Bilinmeyen komut | `versacoder foobar` | 1 |
| `VC-E-0502` | Eksik arguman | `versacoder new` (isim yok) | 1 |
| `VC-E-0503` | Geçersiz bayrak değeri | `--format xml` | 1 |
| `VC-E-0504` | Zaman aşımı (timeout) | `--timeout 5` + uzun build | 5 |
| `VC-E-0505` | Kullanıcı iptali (Ctrl+C) | terminal Stop | 5 |
| `VC-E-0506` | Dosya/bulunamadı | `layer show K9999` | 3 |
| `VC-E-0507` | İzin reddi | `run` + `deny` | 4 |
| `VC-E-0508` | Runtime hata (IO, ağ) | config bozuk | 2 |
| `VC-E-0509` | Config bulunamadı | `~/.versacoder/` yok | 2 (default kullan) |
| `VC-E-0510` | Layer schema ihlali | `layer.json` 12 alan eksik | 2 |

### §4.1 Exit code sözleşmesi

| Exit | Anlam | Ne zaman |
|---|---|---|
| **0** | OK | komut başarılı |
| **1** | Usage hatası | bilinmeyen komut/arguman/bayrak |
| **2** | Runtime hatası | IO, config, schema, beklenmeyen |
| **3** | Bulunamadı | `layer show`, dosya yok |
| **4** | İzin reddi | tool-spec `deny` |
| **5** | Cancelled / timeout | Ctrl+C veya `--timeout` |

---

## §5 Terminal UI Entegrasyonu (VC-CLI-015)

```text
CLI (VersaCoder.CLI.exe)
   |
   | Process.Start (ArgumentList - string bilesimi YOK, SEC-05)
   | stdout satiri -> event
   v
Terminal View (l7, DockManager alt paneli)
   |
   +-- MemoEdit (ReadOnly)  <- satir bazli async ekleme (terminal-spec §2.1)
   +-- TextEdit             <- kullanici girdisi (gecmis: ↑/↓)
   +-- durum alani          <- running/bitti/timeout + exit code
   +-- [Stop] [Temizle]     <- Ctrl+C / kill
   v
Agent Panel (task-queue)   <- "run" gorev ciktisi tasima (terminal-spec §5)
```

**Akış kuralı:** CLI çıktısı **satır bazlı** okunur; her satır `MemoEdit`'e **append** edilir (asenkron, UI thread'e marshal). Buffer 10.000 satır → eski satır düşer (FIFO).

---

## §6 Kabul Kriterleri

- [ ] 16 gereksinim (`VC-CLI-001..016`) tek tek test edilebilir; her birinin "kanıtı ne" satırı dolu
- [ ] 3 çıktı biçimi (`human/json/quiet`) de geçerli, exit code aynı
- [ ] Exit code 0-5 sözleşmesi her komutta tutarlı
- [ ] `layer list --format json` = **5000+** kayıt (K bandı ile birebir)
- [ ] `run` → terminal `MemoEdit` satır akısı ≥ 1 satır
- [ ] `run` + `deny` = exit **4**, terminal "permission denied"
- [ ] `--timeout 5` + uzun komut = exit **5** + terminal "killed (timeout)"
- [ ] Config eksikse default kullanılır, crash yok
- [ ] `check-links.ps1` = 0 bozuk link

---

## §7 Açık Konular (`DOĞRULAMA GEREKLİ`)

| # | Açık | Neden | Kapanış |
|---|---|---|---|
| 1 | `run` hedefi hangi kabuk (`cmd`/`pwsh`/`dotnet`) | netleşmedi | `tool-spec` sandbox ile karar |
| 2 | `agent run` rol listesi (8 rol) CLI'ya nasıl yansır | `agent-spec` henüz yok | `session-spec` + `task-queue-spec` yazılınca |
| 3 | Config şeması (`config.json` alanları) | 12 alanlı `layer.json`'a benzer şema yok | `config-spec` açılınca |
| 4 | `--format json` şeması (her komut için) | tek JSON contract yok | her komutun JSON örneği eklenince |
| 5 | Terminal `run` çıktısında renk (ANSI) | terminal-spec `MemoEdit` renksiz | renkli `RichEditControl` mü, `MemoEdit` mi? |

---

**Sahip:** architect · **Güncelleyen:** architect · **Tarih:** 2026-10-02 · **Ölçüm:** bu spec yazılmadan önce `.ai/spec/index.md`'de yoktu; şimdi 1 satır eklendi.
