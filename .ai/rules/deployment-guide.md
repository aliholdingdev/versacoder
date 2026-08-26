---
title: "Versa Coder — Dağıtım Rehberi"
type: rules
category: deployment
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Dağıtım Rehberi

---

## 1. Dağıtım Kanalları

| Kanal | Format | Durum |
|-------|--------|-------|
| Single Executable | `.exe` (.NET 8 self-contained) | 🔄 Planlanan |
| Windows Installer | `.msi` | 🔄 Planlanan |
| Portable ZIP | `.zip` | 🔄 Planlanan |
| ClickOnce | Otomatik güncelleme | 🔄 Planlanan |

---

## 2. Build Konfigürasyonu

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <PublishSingleFile>true</PublishSingleFile>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
</PropertyGroup>
```

---

## 3. Dağıtım Adımları

| Adım | Komut |
|------|-------|
| 1. Build | `dotnet build -c Release` |
| 2. Test | `dotnet test` |
| 3. Publish | `dotnet publish -c Release` |
| 4. Paketle | MSI veya ZIP oluştur |
| 5. İmza | Kod imzası ekle |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
