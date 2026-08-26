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

## 4. CI/CD Pipeline

### 4.1 GitHub Actions

```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Test
      run: dotnet test -c Release --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Upload artifact
      uses: actions/upload-artifact@v4
      with:
        name: versacoder
        path: ./publish
```

### 4.2 Release Pipeline

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  release:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: dotnet build -c Release
    
    - name: Test
      run: dotnet test -c Release
    
    - name: Publish Self-Contained
      run: |
        dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win-x64
        dotnet publish -c Release -r linux-x64 --self-contained -o ./publish/linux-x64
        dotnet publish -c Release -r osx-x64 --self-contained -o ./publish/osx-x64
    
    - name: Create ZIP
      run: |
        Compress-Archive -Path ./publish/win-x64/* -DestinationPath ./VersaCoder-win-x64.zip
        Compress-Archive -Path ./publish/linux-x64/* -DestinationPath ./VersaCoder-linux-x64.zip
        Compress-Archive -Path ./publish/osx-x64/* -DestinationPath ./VersaCoder-osx-x64.zip
    
    - name: Create Release
      uses: softprops/action-gh-release@v2
      with:
        files: |
          VersaCoder-win-x64.zip
          VersaCoder-linux-x64.zip
          VersaCoder-osx-x64.zip
```

---

## 5. Dağıtım Stratejisi

### 5.1 Versioning

```csharp
// Assembly version
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0+build.20260825")]

// Semantic versioning
public static class VersionInfo
{
    public const string Major = "1";
    public const string Minor = "0";
    public const string Patch = "0";
    public const string Build = "20260825";
    
    public static string FullVersion => $"{Major}.{Minor}.{Patch}.{Build}";
    public static string DisplayVersion => $"{Major}.{Minor}.{Patch}";
}
```

### 5.2 Auto-Update Mechanism

```csharp
public class UpdateService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UpdateService> _logger;
    
    public UpdateService(HttpClient httpClient, ILogger<UpdateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<UpdateInfo?> CheckForUpdateAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.versacoder.com/version");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var latestVersion = JsonSerializer.Deserialize<VersionInfo>(json);
            
            var currentVersion = Version.Parse(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0");
            
            if (latestVersion != null && Version.Parse(latestVersion.DisplayVersion) > currentVersion)
            {
                return new UpdateInfo
                {
                    Version = latestVersion.DisplayVersion,
                    DownloadUrl = latestVersion.DownloadUrl,
                    ReleaseNotes = latestVersion.ReleaseNotes
                };
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check for updates");
            return null;
        }
    }
    
    public async Task<bool> DownloadAndInstallUpdateAsync(UpdateInfo updateInfo)
    {
        try
        {
            // Download update
            var tempPath = Path.Combine(Path.GetTempPath(), $"versacoder-update-{updateInfo.Version}.zip");
            var response = await _httpClient.GetAsync(updateInfo.DownloadUrl);
            response.EnsureSuccessStatusCode();
            
            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(tempPath);
            await stream.CopyToAsync(fileStream);
            
            // Extract and install
            var installPath = AppDomain.CurrentDomain.BaseDirectory;
            ZipFile.ExtractToDirectory(tempPath, installPath, overwriteFiles: true);
            
            // Cleanup
            File.Delete(tempPath);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download update");
            return false;
        }
    }
}
```

---

## 6. Environment Yapılandırması

### 6.1 Development Environment

```json
{
  "Environment": "Development",
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder-dev.db"
  }
}
```

### 6.2 Production Environment

```json
{
  "Environment": "Production",
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=versacoder-prod.db"
  }
}
```

---

## 7. Monitoring ve Alerting

### 7.1 Application Insights

```csharp
// Program.cs'de monitoring yapılandırması
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Custom telemetry
public class CustomTelemetry : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Component.Version = VersionInfo.DisplayVersion;
        telemetry.Context.Session.Id = Guid.NewGuid().ToString();
    }
}
```

### 7.2 Health Monitoring

```csharp
// Health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        
        await context.Response.WriteAsJsonAsync(result);
    }
});
```

---

## 8. Deployment Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Build başarılı | ☐ |
| 2 | Testler başarılı | ☐ |
| 3 | Code coverage > 80% | ☐ |
| 4 | Security scan başarılı | ☐ |
| 5 | Performance testleri başarılı | ☐ |
| 6 | Documentation güncellendi | ☐ |
| 7 | Changelog güncellendi | ☐ |
| 8 | Version bump yapıldı | ☐ |
| 9 | Release notes hazırlandı | ☐ |
| 10 | Deploy planı hazırlandı | ☐ |

---

## 9. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Deployment Channels | 4 |
| CI/CD Pipelines | 2 |
| Environments | 2 (Dev, Prod) |
| Monitoring Tools | 2 (App Insights, Health Checks) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
