---
title: "Katalog Dugumu N22 - Kripto ve Guvenlik (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N22 crypto-sec dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N22 - Kripto ve Guvenlik

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N22 crypto-sec hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N22 (crypto-sec), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Crypto/` klasorunde yasar.** Simetrik sifreleme, HMAC ve parola ozeti gibi kripto islemlerini sarar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Crypto/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N22 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l3 Crosscutting |
| Proje (.csproj) | VersaCoder.Crosscutting |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Simetrik sifreleme, HMAC ve parola ozeti gibi kripto islemlerini sarar | l0 disinda bir katmana dogrudan bakmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | is kurali veya invariant tasimak, is l1 katmaninda |
| 3 | ortak kesit servisini vermek: log, saat, onbek, kripto, yapilandirma | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 4 | test edilebilir saricilar sunmak (IClock, ILogSink) | SQL veya veri erisimi yazmak, is l4 katmaninda |
| 5 | yalniz l0 katmanina bakmak | UI tipine bakmak (Desktop / Cli) |
| 6 | katmanlari degil alt altyapiyi sarmak | secret veya anahtar barindirmak, keys.md tek kaynaktir |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N22 | crypto-sec | `src/VersaCoder.Crosscutting/Crypto/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Crypto'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SymmetricCipher | 6 | RandomSource |
| 2 | Hasher | 7 | TokenFactory |
| 3 | HmacSigner | 8 | CertificateGuard |
| 4 | SecretProtector | 9 | PasswordHasher |
| 5 | KeyVault | 10 | Nonce |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N22-001 | `ISymmetricCipher` | interface | SymmetricCipher sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-002 | `SymmetricCipher` | class | SymmetricCipher cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-003 | `SymmetricCipherDto` | record | SymmetricCipher veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-004 | `SymmetricCipherMapper` | class | SymmetricCipher <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-005 | `SymmetricCipherValidator` | class | SymmetricCipher dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-006 | `SymmetricCipherFactory` | class | SymmetricCipher uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l3 -> l0 | OK | ADR-028 2.1 |
| 2 | l3 -> l1, l2, l4..l7 | X | ADR-028 2.1 |
| 3 | l5 -> l4 | X | ADR-028 2.3/2 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | SymmetricCipherTests (xUnit) | planli |
| 4 | NetArchTest yon denetimi | izinsiz kenar = 0 | planli (LayerRulesTests) |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu dugume girmez; tek iz `reports\k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- Tam kayit listesi (120 satir) bu dosyaya sasmaz; ornek satirlar aile kalibini gosterir -> **DOĞRULAMA GEREKLI**.
- Kod yazimi `src/` icinde yapilir; bu dosya yalniz katalog kartidir, bir `.csproj` degildir.

---

## §8 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [catalog index](index.md) | 52 dugum envanteri + N <-> klasor eslemesi |
| 2 | [node-inventory](node-inventory.md) | 52 dugumun tek tek kunyesi |
| 3 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi (ADR-028 2.3/2) |
| 4 | [project-map](project-map.md) | 9 uretim + 1-2 test proje, N araliklari |
| 5 | [mimari index](../index.md) | 8 makro katman ozeti |
| 6 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | baz karar: 8 katman / 52 dugum / 9 proje |

---

## §9 Kanit

```powershell
`# 1) kayit sayimi (ADR-013 2.2) - beklenen: 120
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N22-crypto-sec.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N22-crypto-sec.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Crypto')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N22-007 | `SymmetricCipherOptions` | record | SymmetricCipher yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-008 | `SymmetricCipherProvider` | class | SymmetricCipher saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-009 | `SymmetricCipherRegistry` | class | SymmetricCipher kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-010 | `SymmetricCipherConverter` | class | SymmetricCipher bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-011 | `SymmetricCipherTests` | class | SymmetricCipher testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-012 | `SymmetricCipherBuilder` | class | SymmetricCipher uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | SymmetricCipherTests | planli |
| N22-013 | `IHasher` | interface | Hasher sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-014 | `Hasher` | class | Hasher cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-015 | `HasherDto` | record | Hasher veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-016 | `HasherMapper` | class | Hasher <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-017 | `HasherValidator` | class | Hasher dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-018 | `HasherFactory` | class | Hasher uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-019 | `HasherOptions` | record | Hasher yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-020 | `HasherProvider` | class | Hasher saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-021 | `HasherRegistry` | class | Hasher kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-022 | `HasherConverter` | class | Hasher bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-023 | `HasherTests` | class | Hasher testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-024 | `HasherBuilder` | class | Hasher uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | HasherTests | planli |
| N22-025 | `IHmacSigner` | interface | HmacSigner sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-026 | `HmacSigner` | class | HmacSigner cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-027 | `HmacSignerDto` | record | HmacSigner veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-028 | `HmacSignerMapper` | class | HmacSigner <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-029 | `HmacSignerValidator` | class | HmacSigner dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-030 | `HmacSignerFactory` | class | HmacSigner uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-031 | `HmacSignerOptions` | record | HmacSigner yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-032 | `HmacSignerProvider` | class | HmacSigner saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-033 | `HmacSignerRegistry` | class | HmacSigner kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-034 | `HmacSignerConverter` | class | HmacSigner bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-035 | `HmacSignerTests` | class | HmacSigner testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-036 | `HmacSignerBuilder` | class | HmacSigner uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | HmacSignerTests | planli |
| N22-037 | `ISecretProtector` | interface | SecretProtector sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-038 | `SecretProtector` | class | SecretProtector cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-039 | `SecretProtectorDto` | record | SecretProtector veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-040 | `SecretProtectorMapper` | class | SecretProtector <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-041 | `SecretProtectorValidator` | class | SecretProtector dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-042 | `SecretProtectorFactory` | class | SecretProtector uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-043 | `SecretProtectorOptions` | record | SecretProtector yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-044 | `SecretProtectorProvider` | class | SecretProtector saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-045 | `SecretProtectorRegistry` | class | SecretProtector kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-046 | `SecretProtectorConverter` | class | SecretProtector bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-047 | `SecretProtectorTests` | class | SecretProtector testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-048 | `SecretProtectorBuilder` | class | SecretProtector uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | SecretProtectorTests | planli |
| N22-049 | `IKeyVault` | interface | KeyVault sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-050 | `KeyVault` | class | KeyVault cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-051 | `KeyVaultDto` | record | KeyVault veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-052 | `KeyVaultMapper` | class | KeyVault <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-053 | `KeyVaultValidator` | class | KeyVault dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-054 | `KeyVaultFactory` | class | KeyVault uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-055 | `KeyVaultOptions` | record | KeyVault yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-056 | `KeyVaultProvider` | class | KeyVault saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-057 | `KeyVaultRegistry` | class | KeyVault kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-058 | `KeyVaultConverter` | class | KeyVault bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-059 | `KeyVaultTests` | class | KeyVault testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-060 | `KeyVaultBuilder` | class | KeyVault uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | KeyVaultTests | planli |
| N22-061 | `IRandomSource` | interface | RandomSource sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-062 | `RandomSource` | class | RandomSource cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-063 | `RandomSourceDto` | record | RandomSource veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-064 | `RandomSourceMapper` | class | RandomSource <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-065 | `RandomSourceValidator` | class | RandomSource dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-066 | `RandomSourceFactory` | class | RandomSource uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-067 | `RandomSourceOptions` | record | RandomSource yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-068 | `RandomSourceProvider` | class | RandomSource saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-069 | `RandomSourceRegistry` | class | RandomSource kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-070 | `RandomSourceConverter` | class | RandomSource bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-071 | `RandomSourceTests` | class | RandomSource testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-072 | `RandomSourceBuilder` | class | RandomSource uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | RandomSourceTests | planli |
| N22-073 | `ITokenFactory` | interface | TokenFactory sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-074 | `TokenFactory` | class | TokenFactory cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-075 | `TokenFactoryDto` | record | TokenFactory veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-076 | `TokenFactoryMapper` | class | TokenFactory <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-077 | `TokenFactoryValidator` | class | TokenFactory dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-078 | `TokenFactoryFactory` | class | TokenFactory uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-079 | `TokenFactoryOptions` | record | TokenFactory yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-080 | `TokenFactoryProvider` | class | TokenFactory saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-081 | `TokenFactoryRegistry` | class | TokenFactory kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-082 | `TokenFactoryConverter` | class | TokenFactory bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-083 | `TokenFactoryTests` | class | TokenFactory testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-084 | `TokenFactoryBuilder` | class | TokenFactory uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | TokenFactoryTests | planli |
| N22-085 | `ICertificateGuard` | interface | CertificateGuard sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-086 | `CertificateGuard` | class | CertificateGuard cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-087 | `CertificateGuardDto` | record | CertificateGuard veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-088 | `CertificateGuardMapper` | class | CertificateGuard <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-089 | `CertificateGuardValidator` | class | CertificateGuard dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-090 | `CertificateGuardFactory` | class | CertificateGuard uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-091 | `CertificateGuardOptions` | record | CertificateGuard yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-092 | `CertificateGuardProvider` | class | CertificateGuard saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-093 | `CertificateGuardRegistry` | class | CertificateGuard kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-094 | `CertificateGuardConverter` | class | CertificateGuard bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-095 | `CertificateGuardTests` | class | CertificateGuard testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-096 | `CertificateGuardBuilder` | class | CertificateGuard uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CertificateGuardTests | planli |
| N22-097 | `IPasswordHasher` | interface | PasswordHasher sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-098 | `PasswordHasher` | class | PasswordHasher cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-099 | `PasswordHasherDto` | record | PasswordHasher veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-100 | `PasswordHasherMapper` | class | PasswordHasher <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-101 | `PasswordHasherValidator` | class | PasswordHasher dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-102 | `PasswordHasherFactory` | class | PasswordHasher uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-103 | `PasswordHasherOptions` | record | PasswordHasher yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-104 | `PasswordHasherProvider` | class | PasswordHasher saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-105 | `PasswordHasherRegistry` | class | PasswordHasher kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-106 | `PasswordHasherConverter` | class | PasswordHasher bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-107 | `PasswordHasherTests` | class | PasswordHasher testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-108 | `PasswordHasherBuilder` | class | PasswordHasher uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | PasswordHasherTests | planli |
| N22-109 | `INonce` | interface | Nonce sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-110 | `Nonce` | class | Nonce cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-111 | `NonceDto` | record | Nonce veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-112 | `NonceMapper` | class | Nonce <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-113 | `NonceValidator` | class | Nonce dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-114 | `NonceFactory` | class | Nonce uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-115 | `NonceOptions` | record | Nonce yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-116 | `NonceProvider` | class | Nonce saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-117 | `NonceRegistry` | class | Nonce kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-118 | `NonceConverter` | class | Nonce bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
| N22-119 | `NonceTests` | class | Nonce testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N22-120 | `NonceBuilder` | class | Nonce uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | NonceTests | planli |
