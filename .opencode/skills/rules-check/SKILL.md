---
name: rules-check
description: Kod yazimindan sonra degisen dosyalari .ai/.rules icindeki 16 kural karsisinda denetler, ihlali dosya:satir raporlar. Tetikler: "kurallari calistir", "rules check", "kural denetimi", "rule run".
---

# Rules Check — `.ai/.rules/**`

> Tek cumlelik sorumluluk: yazilan kodun hangi kural dosyasina girdigini
> `.rules/index.md` uzerinden bulup ihlali kanitla (dosya : satir) raporlamak.

## §1 Akis

1. `git diff` (ya da elle degisen dosya listesi) ile degisimi al.
2. `.ai/.rules/index.md` §1 (kayit defteri) ve §2 (kural eslesmesi) uzerinden
   her degisen dosya icin **ilgili kural dosyalarini** sec — 16 dosyanin
   tamami okunmaz, yalniz eslesenler.
3. Secilen kural dosyasini oku, kodu maddeleriyle karsilastir.
4. Ihlal bulunan her madde icin kanit yaz: `dosya yolu : satir no` + kuralin
   maddesi. Kanitsiz ihlal iddiasi islenmez (G5).
5. Sonucu kisa tabloyla ver; ihlal varsa **duzelt -> tekrar denetle**
   dongusunu ihlal kalmayana kadar cevir.

## §2 Hizli esleme (tam metin `.ai/.rules/index.md` §2'dedir)

| Icerik | Kural dosyasi |
|---|---|
| EF Core / sorgu / timeout | `data-access-rules.md` (CRITICAL) |
| DevExpress / ribbon / surum | `devexpress-rules.md` (CRITICAL) |
| secret / sifre / DPAPI | `security-rules.md` (CRITICAL) |
| async / ConfigureAwait / iptal | `async-rules.md` |
| catch siniri / Result sozlesmesi | `error-handling-rules.md` |
| test / determinizm / kapsam | `testing-rules.md` |
| RibbonForm / ekran / yerlesim | `ui-rules.md` |
| katman yonu / ilke | `solid-rules.md` · `configuration-rules.md` |
| log seviyesi / tek satir / PII | `logging-rules.md` |
| performans / N+1 / olcum | `performance-guidelines.md` |
| isimlendirme / namespace | `naming-conventions.md` |
| C# dili / nullable / analyzer | `coding-standards.md` |
| klavye / odak / kontrast | `accessibility-rules.md` |
| cache / TTL / invalidation | `caching-rules.md` |
| paketleme / CI / rollback | `deployment-guide.md` |

## §3 Cikti bicimi

```text
DOSYA                          KURAL                     DURUM
src/.../Store.cs                data-access-rules.md      RED : 14 (EF Core)
src/.../SessionLoader.cs        async-rules.md            GECTI
```

- Cakisan kuralda oncelik: guvenlik > anayasa (G1..G12) > ADR > veri erisimi >
  kod > performans > stil (`.ai/.rules/index.md` §3).
- Kurali **bilerek** cigneyen is RED olur; kural degisikligi yalniz
  `architect` + gerekirse kullanici onayiyla girer.
