---
title: "Spec Dizini - VersaCoder"
type: index
category: spec
status: active
version: 1.2.0
updated: 2026-10-02
authority: reference
scope: "spec/ altindaki tum teknik şartnamelerin kayit ve navigasyon noktasi - hangi spec nerede, kimin, hangi gate'e bagli"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Spec Dizini

**Zorunlu Baglantilar:** [[index|vault index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys.md]] · [[governance/workflow|WORKFLOW.md]] · [[spec/test-strategy|test-strategy]]

> **Tek cumlelik sorumluluk:** *Hangi şartname nerede ve hangi soruyu cevaplar* sorusunun tek cevabidir.
> Icerik tekrari yasak (SSP): bu dosya yalnizca kayit tutar.

---

## §0 Ozet (Answer First)

`spec/` = **sistem nasil davranir** sorusunun kaynagi. Kod yazilirken bakilan tek yer burasi. Mevcut kayitlar + GATE-004a batch'i ile genisler.

---

## §1 Kayit Tablosu

| Dosya | Ne cevaplar | Sahip | Gate |
|---|---|---|---|
| [[spec/versacoder-spec|versacoder-spec.md]] | Urunun tam kapsamı, hedef kullanici, 8 PART haritasi | architect | GATE-003 |
| [[spec/session-spec|session-spec.md]] | Oturum/transcript yasam dongusu, compaction | architect | GATE-004 |
| [[spec/protocol-spec|protocol-spec.md]] | Araclar ve mesaj sozlesmeleri (l5) | architect | GATE-004 |
| [[spec/tool-spec|tool-spec.md]] | Tool seti, schema, timeout, hata sozlesmesi | architect | GATE-004 |
| [[spec/terminal-spec|terminal-spec.md]] | CLI ciktisi yakalama, komut calistirma, timeout/iptal + CLI entegrasyonu | architect | GATE-004 |
| [[spec/cli-spec|cli-spec.md]] | VersaCoder CLI komut agaci, arguman, cikti, exit code, terminal baglantisi (VC-CLI-001..016) | architect | GATE-004 |
| [[spec/cli-console-spec|cli-console-spec.md]] | CLI Konsolu (09-cli-console) ekran sozlesmesi: OpenCode-benzeri tam ekran CLI yuzeyi, sekme seridi, prompt kutusu, durum cubugu (VC-U-0901..0912) | ui-designer | GATE-003 |
| [[spec/ai-chat-console-spec|ai-chat-console-spec.md]] | AI Chat Konsolu (10-ai-chat-console) ekran sozlesmesi: oturum sekmeleri, sohbet akisi, prompt kutusu (VC-U-1001..1010) | ui-designer | GATE-003 |
| [[spec/llm-provider-spec|llm-provider-spec.md]] | OpenRouter + NVIDIA NIM saglayici portu (IAgentProvider), endpoint, anahtar, hata VC-E-0601..0615 (VC-LLM-001..014) | architect | GATE-004 |
| [[spec/ui-shell-spec|ui-shell-spec.md]] | RibbonForm kabugu, dock panel, durum cubugu | ui-designer | GATE-003 |
| [[spec/test-strategy|test-strategy.md]] | Test piramidi, %90 kapsam, iki bacak test matrisi | tester | GATE-005 |
| [[spec/prompt-maker-spec|prompt-maker-spec.md]] | Prompt Maker giris/kabul sozlesmesi + oturum girdi hatti (Exploration Gate -> Instruction) + sonradan eklenecekler kaydi | general | GATE-004a |
| [[spec/layering-5000-spec|layering-5000-spec.md]] | 5000+ tip (katman) kaydi: uretim, adlandirma, sayim esigi, yon ve gate baglantisi (VC-LAY-NNN) | architect | GATE-005 |

---

## §2 Kural

Yeni şartname acilirken: dosya `spec/` altina, bu tabloya satir, `keys.md`'ye anahtar kelime. Siralama alfabetik degil **onecelik** sirasidir.

---

## §3 Bkz.

- [[index|vault index]] §12 Spec Pusulasi
- [[.rules/testing-rules|.rules/test-strategy baglantisi]]
