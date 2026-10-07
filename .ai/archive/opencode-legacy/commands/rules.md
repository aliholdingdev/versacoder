---
description: Degisen dosyalari .ai/.rules icindeki 16 kural karsisinda denetler
authority: derived
tags: [archive]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

Kural denetimi: $ARGUMENTS

1. `rules-check` skill'ini yukle (skill tool, id: `rules-check`).
2. §1 akisini uygula: degisim listesi -> `.ai/.rules/index.md` §1/§2 uzerinden
   **eslesen** kural dosyalarini sec (16 dosyanin tamami okunmaz) -> kodu
   maddeleriyle karsilastir.
3. Sonucu §3 tablosuyla ver: `DOSYA | KURAL | DURUM`; ihlal satirinda kanit
   `dosya yolu : satir no` + kural maddesi (kanitsiz ihlal iddiasi yazma, G5).
4. Ihlal varsa duzelt ve ayni denetimi tekrar calistir; ihlal kalmayinca kisa
   ajan kutusu don (`+---+` ASCII, 5 alan).
