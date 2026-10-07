---
title: "connection-string-secrets - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Baglanti metni sirlarinin DPAPI ile sifreli saklanmasi, duz metin yasagi, rotasyon ve inceleme kontrolu (ADR-005 #5)"
---

# connection-string-secrets — Bağlantı Metni ve Sırlar

**Zorunlu Baglantilar:** [vault index](../../index.md) · [security-rules](../../.rules/security-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [data index](../index.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md)

> **Tek cümlelik sorumluluk:** *Bağlantı metni (özellikle parola) nerede, nasıl, kim için şifrelenerek saklanır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Bağlantı metni **düz metin durmaz**: `ProtectedData` (DPAPI) ile `DataProtectionScope.CurrentUser` altında şifrelenir, base64 olarak ayar dosyasında ya da kullanıcı profilinde saklanır, çözümleme yalnız çalıştırma anında yapılır. Parola **koda, `.config`'e, `.sql`'e, repo'ya** yazılmaz; `keys.md` yalnız anahtar kelime haritasıdır, sır deposu değildir. Şifreleme **parola değil metni** korur; erişim ayrıcalığı ayrı bir katmandır (en az yetki).

---

## §1 Doğru kullanım

```csharp
var clear = "Server=.;Database=VersaCoder;Integrated Security=true";
var entropy = Encoding.UTF8.GetBytes(Vault.Entropy);              // uygulamaya ozel, sirket ici

var protectedBytes = ProtectedData.Protect(
    Encoding.UTF8.GetBytes(clear), entropy, DataProtectionScope.CurrentUser);

File.WriteAllText(secretPath, Convert.ToBase64String(protectedBytes));  // duz metin yok
```

---

## §2 Saklama kuralı tablosu

| Yer | Durum | Açıklama |
|---|---|---|
| Kaynak kod (`.cs`) | **yasak** | string literal = repo sızıntısı |
| `.sql` / migration | **yasak** | şema dosyaları paylaşılır |
| `keys.md` | yalnız anahtar kelime haritası | sır içermez (ADR-005 #5) |
| Ayar dosyası (şifreli blob) | **izinli** | base64 + DPAPI |
| Kullanıcı ortam değişkeni | **izinli** | CI/ortam stratejisi |
| Git | `.gitignore` + tarama | sır = CRITICAL (SEC-02) |

---

## §3 Denetim listesi

| # | Kontrol | Kanıt |
|---|---|---|
| 1 | Düz metin parola araması (`Password=`, `Pwd=`) repoda = **0** | kod taraması |
| 2 | DPAPI kapsamı bilinçli: `CurrentUser` (tek kullanıcı) / `Machine` (servis) | kod incelemesi |
| 3 | Entropy sabiti sır sayılmaz ama **değiştirilemez** | sürüm kontrol |
| 4 | Rotasyon: parola değişince yalnız şifreli blob yenilenir | runbook |
| 5 | `Encrypt=true` (SQL Client varsayılanı) + güven sertifikası bilinçli | bağlantı testi |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `ProtectedData` | `System.Security.dll` yerleşik | NuGet `System.Security.Cryptography.ProtectedData` (**yalnız Windows**) |
| Kapsam | `CurrentUser` / `Machine` | aynı (Windows DPAPI) |
| Parola kaynağı | yerel ayar / profil | aynı — ortam değişkeni tercih |
| Erişim | `#if` **değil**, ayrı `ISecretStore` çözümü | aynı arayüz |

> **NOT:** Kasa (secret store) soyutlaması `IDbConnectionFactory` gibi **tek arayüz** ile iki bacağa ortaktır; `#if` yalnız platform API farkı için kalır (ADR-001).

---

## §5 Kardeşler

[sqlserver-provider-setup](sqlserver-provider-setup.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [ado-net-connection](ado-net-connection.md) · [sql-injection-defense](sql-injection-defense.md) · [connection-pooling-notes](connection-pooling-notes.md)
