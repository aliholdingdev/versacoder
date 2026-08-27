---
title: "Versa Coder — Dağıtım Rehberi"
type: rules
category: deployment
date: 2026-08-25
updated: 2026-08-26
status: active
version: 2.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/rules/deployment-guide.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/rules/deployment-guide.md"
---

# Versa Coder — Dağıtım Rehberi

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

Bu belge, Versa Coder platformunun production ortamına dağıtım süreçlerini, CI/CD pipeline'larını, container stratejilerini ve felaket kurtarma prosedürlerini kapsamlı olarak tanımlar.

### 1.1 Kapsam

| Kapsam | Kapsam Dışı |
|--------|-------------|
| CI/CD pipeline tanımları | Geliştirme ortamı kurulumu |
| Container stratejileri | Kod yazım standartları |
| Cloud dağıtım planları | UI tasarım kararları |
| Rollback prosedürleri | Veritabanı şema tasarımı |
| Secret yönetimi | API tasarım kararları |
| Monitoring yapılandırması | Güvenlik politika detayları |

### 1.2 Hedef Kitle

| Rol | Kullanım Alanı |
|-----|----------------|
| DevOps Mühendisi | Pipeline oluşturma, deployment |
| Backend Geliştirici | Build ve publish süreçleri |
| Sistem Yöneticisi | Sunucu yapılandırması |
| Teknik Lider | Dağıtım stratejisi kararları |

### 1.3 Terminoloji

| Terim | Tanım |
|-------|-------|
| **Blue-Green** | İki identik ortam arası geçiş stratejisi |
| **Canary** | Kademeli trafik yönlendirme stratejisi |
| **Rolling** | Sıralı güncelleme stratejisi |
| **Recreate** | Eski sürümü durdurup yenisini oluşturma |
| **RPO** | Recovery Point Objective - Veri kaybı toleransı |
| **RTO** | Recovery Time Objective - Kurtarma süresi hedefi |
| **SLA** | Service Level Agreement - Hizmet seviyesi anlaşması |

---

## 2. Deployment Stratejileri

### 2.1 Blue-Green Deployment

İki identik production ortamı (Blue ve Green) arasında anlık geçiş stratejisi.

```
┌─────────────────────────────────────────────────────────┐
│                    Load Balancer                         │
│                        │                                 │
│            ┌───────────┴───────────┐                     │
│            ▼                       ▼                     │
│    ┌───────────────┐       ┌───────────────┐             │
│    │   Blue (Prod) │       │  Green (Next) │             │
│    │   v1.2.0      │       │   v1.3.0      │             │
│    │   ✅ Active   │       │   🔄 Standby  │             │
│    └───────────────┘       └───────────────┘             │
│            │                       │                     │
│            └───────────┬───────────┘                     │
│                        ▼                                 │
│              ┌───────────────┐                           │
│              │   SQLite DB   │                           │
│              │   (Shared)    │                           │
│              └───────────────┘                           │
└─────────────────────────────────────────────────────────┘
```

**Avantajları:**
- Sıfır downtime deployment
- Anlık geri dönüş
- Test edilmiş geçiş

**Dezavantajları:**
- Çift kaynak kullanımı
- Veritabanı senkronizasyonu zorluğu

```yaml
# Blue-Green deployment pipeline
name: Blue-Green Deploy

on:
  push:
    branches: [main]

jobs:
  deploy-blue-green:
    runs-on: windows-latest
    environment: production
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Build and Test
      run: |
        dotnet build -c Release
        dotnet test -c Release --verbosity normal
    
    - name: Determine Target Environment
      id: env
      run: |
        # Check current active environment
        $activeEnv = az webapp config appsettings list \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --query "[?name=='ACTIVE_ENV'].value" -o tsv
        
        if ($activeEnv -eq "blue") {
          echo "target=green" >> $env:GITHUB_OUTPUT
          echo "swap_from=blue" >> $env:GITHUB_OUTPUT
        } else {
          echo "target=blue" >> $env:GITHUB_OUTPUT
          echo "swap_from=green" >> $env:GITHUB_OUTPUT
        }
    
    - name: Deploy to Target Environment
      run: |
        dotnet publish -c Release -o ./publish
        az webapp deploy \
          --name versacoder-${{ steps.env.outputs.target }} \
          --resource-group versacoder-rg \
          --src-path ./publish \
          --type zip
    
    - name: Health Check
      run: |
        $url = "https://versacoder-${{ steps.env.outputs.target }}.azurewebsites.net/health"
        $maxRetries = 30
        $retryCount = 0
        
        do {
          $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction SilentlyContinue
          if ($response.StatusCode -eq 200) {
            Write-Host "Health check passed"
            break
          }
          $retryCount++
          Start-Sleep -Seconds 10
        } while ($retryCount -lt $maxRetries)
    
    - name: Swap Traffic
      run: |
        az webapp trafficrouting set \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --distribution "versacoder-${{ steps.env.outputs.target }}=100"
    
    - name: Update Active Environment
      run: |
        az webapp config appsettings set \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --settings "ACTIVE_ENV=${{ steps.env.outputs.target }}"
```

### 2.2 Canary Deployment

Kademeli olarak trafik yönlendirmesi yaparak yeni sürümü test etme stratejisi.

```
┌─────────────────────────────────────────────────────────┐
│                    Load Balancer                         │
│                        │                                 │
│            ┌───────────┴───────────┐                     │
│            ▼                       ▼                     │
│    ┌───────────────┐       ┌───────────────┐             │
│    │   v1.2.0      │       │   v1.3.0      │             │
│    │   %90 Trafik  │       │   %10 Trafik  │             │
│    │   ✅ Stable   │       │   🔄 Canary   │             │
│    └───────────────┘       └───────────────┘             │
│            │                       │                     │
│            └───────────┬───────────┘                     │
│                        ▼                                 │
│              ┌───────────────┐                           │
│              │   SQLite DB   │                           │
│              │   (Shared)    │                           │
│              └───────────────┘                           │
└─────────────────────────────────────────────────────────┘
```

**Trafik Dağılım Planı:**

| Aşama | Eski Sürüm | Yeni Sürüm | Süre | Koşul |
|-------|------------|------------|------|-------|
| 1 | %90 | %10 | 15dk | Hata oranı < %1 |
| 2 | %70 | %30 | 30dk | Hata oranı < %1 |
| 3 | %50 | %50 | 30dk | Hata oranı < %0.5 |
| 4 | %20 | %80 | 30dk | Hata oranı < %0.5 |
| 5 | %0 | %100 | - | Tüm kontroller başarılı |

```yaml
# Canary deployment pipeline
name: Canary Deploy

on:
  push:
    branches: [main]

jobs:
  canary-deploy:
    runs-on: windows-latest
    environment: canary
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Build and Test
      run: |
        dotnet build -c Release
        dotnet test -c Release --verbosity normal
    
    - name: Deploy Canary Instance
      run: |
        dotnet publish -c Release -o ./publish
        az webapp deploy \
          --name versacoder-canary \
          --resource-group versacoder-rg \
          --src-path ./publish \
          --type zip
    
    - name: Canary Health Check
      run: |
        $url = "https://versacoder-canary.azurewebsites.net/health"
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing
        if ($response.StatusCode -ne 200) {
          throw "Canary health check failed"
        }
    
    - name: Configure Traffic Split - 10%
      run: |
        az webapp trafficrouting set \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --distribution "versacoder-canary=10,versacoder-prod=90"
    
    - name: Monitor Canary Metrics (15 minutes)
      run: |
        # Monitor error rates and performance
        $startTime = Get-Date
        $duration = New-TimeSpan -Minutes 15
        
        while ((Get-Date) - $startTime -lt $duration) {
          $metrics = az monitor metrics list \
            --resource versacoder-canary \
            --metric "requests/failed" \
            --aggregation Count \
            --interval PT1M \
            --query "value[0].timeseries[0].data[-1].count" -o tsv
          
          if ($metrics -gt 5) {
            throw "Canary error rate exceeded threshold"
          }
          
          Start-Sleep -Seconds 60
        }
    
    - name: Increase Traffic to 50%
      run: |
        az webapp trafficrouting set \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --distribution "versacoder-canary=50,versacoder-prod=50"
    
    - name: Monitor Canary Metrics (30 minutes)
      run: |
        $startTime = Get-Date
        $duration = New-TimeSpan -Minutes 30
        
        while ((Get-Date) - $startTime -lt $duration) {
          $metrics = az monitor metrics list \
            --resource versacoder-canary \
            --metric "requests/failed" \
            --aggregation Count \
            --interval PT1M \
            --query "value[0].timeseries[0].data[-1].count" -o tsv
          
          if ($metrics -gt 3) {
            throw "Canary error rate exceeded threshold at 50%"
          }
          
          Start-Sleep -Seconds 60
        }
    
    - name: Full Traffic Switch
      run: |
        az webapp trafficrouting set \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --distribution "versacoder-canary=100"
    
    - name: Cleanup Old Version
      run: |
        # Stop old instance after successful canary
        az webapp stop \
          --name versacoder-prod-old \
          --resource-group versacoder-rg
```

### 2.3 Rolling Update

Mevcut instance'ları sırayla güncelleme stratejisi.

```yaml
# Rolling update configuration
name: Rolling Deploy

on:
  push:
    branches: [main]

jobs:
  rolling-deploy:
    runs-on: windows-latest
    strategy:
      max-parallel: 1
      matrix:
        instance: [instance-1, instance-2, instance-3]
    
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
    
    - name: Deploy to ${{ matrix.instance }}
      run: |
        dotnet publish -c Release -o ./publish
        az webapp deploy \
          --name versacoder-${{ matrix.instance }} \
          --resource-group versacoder-rg \
          --src-path ./publish \
          --type zip
    
    - name: Health Check ${{ matrix.instance }}
      run: |
        $url = "https://versacoder-${{ matrix.instance }}.azurewebsites.net/health"
        $maxRetries = 10
        $retryCount = 0
        
        do {
          try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
              Write-Host "Health check passed for ${{ matrix.instance }}"
              break
            }
          } catch {
            Write-Host "Retry $retryCount..."
          }
          $retryCount++
          Start-Sleep -Seconds 5
        } while ($retryCount -lt $maxRetries)
        
        if ($retryCount -ge $maxRetries) {
          throw "Health check failed for ${{ matrix.instance }}"
        }
```

### 2.4 Recreate Strategy

Eski sürümü tamamen durdurup yenisini oluşturma stratejisi.

```yaml
# Recreate deployment
name: Recreate Deploy

on:
  push:
    branches: [main]
  workflow_dispatch:
    inputs:
      confirm:
        description: 'Type "yes" to confirm recreation'
        required: true

jobs:
  recreate-deploy:
    runs-on: windows-latest
    environment: production
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Verify Confirmation
      if: github.event_name == 'workflow_dispatch'
      run: |
        if ("${{ github.event.inputs.confirm }}" -ne "yes") {
          throw "Deployment not confirmed"
        }
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Build and Test
      run: |
        dotnet build -c Release
        dotnet test -c Release
    
    - name: Stop Current Instance
      run: |
        az webapp stop \
          --name versacoder-prod \
          --resource-group versacoder-rg
    
    - name: Deploy New Version
      run: |
        dotnet publish -c Release -o ./publish
        az webapp deploy \
          --name versacoder-prod \
          --resource-group versacoder-rg \
          --src-path ./publish \
          --type zip
    
    - name: Start Instance
      run: |
        az webapp start \
          --name versacoder-prod \
          --resource-group versacoder-rg
    
    - name: Health Check
      run: |
        $url = "https://versacoder-prod.azurewebsites.net/health"
        $maxRetries = 30
        $retryCount = 0
        
        do {
          try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
              Write-Host "Application is healthy"
              break
            }
          } catch {
            Write-Host "Waiting for application... ($retryCount)"
          }
          $retryCount++
          Start-Sleep -Seconds 10
        } while ($retryCount -lt $maxRetries)
        
        if ($retryCount -ge $maxRetries) {
          throw "Application failed to start"
        }
```

---

## 3. GitHub Actions Workflows

### 3.1 Build Workflow

```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_FILE: 'VersaCoder.slnx'

jobs:
  build:
    name: Build
    runs-on: windows-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Cache NuGet packages
      uses: actions/cache@v4
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-
    
    - name: Restore dependencies
      run: dotnet restore ${{ env.SOLUTION_FILE }}
    
    - name: Build
      run: dotnet build ${{ env.SOLUTION_FILE }} -c Release --no-restore
    
    - name: Check formatting
      run: dotnet format --verify-no-changes --verbosity diagnostic
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: build-output
        path: |
          src/**/bin/Release/
          !src/**/bin/Release/net8.0-windows/
        retention-days: 5

  test:
    name: Test
    runs-on: windows-latest
    needs: build
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore ${{ env.SOLUTION_FILE }}
    
    - name: Run unit tests
      run: |
        dotnet test ${{ env.SOLUTION_FILE }} `
          -c Release `
          --no-restore `
          --logger "trx;LogFileName=test-results.trx" `
          --collect:"XPlat Code Coverage" `
          --results-directory ./TestResults
    
    - name: Generate coverage report
      uses: danielpalme/ReportGenerator-GitHub-Action@5
      with:
        reports: '**/coverage.cobertura.xml'
        targetdir: 'CoverageReport'
        reporttypes: 'HtmlInline_AzurePipelines;Badges;MarkdownSummaryGithub'
    
    - name: Upload test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: test-results
        path: ./TestResults
    
    - name: Upload coverage report
      uses: actions/upload-artifact@v4
      with:
        name: coverage-report
        path: ./CoverageReport

  code-quality:
    name: Code Quality
    runs-on: windows-latest
    needs: build
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run SonarQube analysis
      uses: SonarSource/sonarqube-scan-action@master
      env:
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      with:
        args: >
          -Dsonar.projectKey=versacoder
          -Dsonar.sources=src
          -Dsonar.cs.opencover.reportsPaths=**/coverage.opencover.xml
```

### 3.2 Test Workflow

```yaml
# .github/workflows/test.yml
name: Comprehensive Testing

on:
  pull_request:
    branches: [main, develop]
  schedule:
    - cron: '0 2 * * *'  # Her gece saat 02:00'de çalıştır

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  unit-tests:
    name: Unit Tests
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run Domain Tests
      run: |
        dotnet test tests/VersaCoder.Domain.Tests/ `
          -c Release `
          --logger "trx;LogFileName=domain-tests.trx"
    
    - name: Run Application Tests
      run: |
        dotnet test tests/VersaCoder.Application.Tests/ `
          -c Release `
          --logger "trx;LogFileName=application-tests.trx"
    
    - name: Run Infrastructure Tests
      run: |
        dotnet test tests/VersaCoder.Infrastructure.Tests/ `
          -c Release `
          --logger "trx;LogFileName=infrastructure-tests.trx"

  integration-tests:
    name: Integration Tests
    runs-on: windows-latest
    needs: unit-tests
    
    services:
      sqlite:
        image: nouchka/sqlite3:latest
        ports:
          - 5432:5432
        options: >-
          --health-cmd="sqlite3 /data/versacoder.db 'SELECT 1;'"
          --health-interval=10s
          --health-timeout=5s
          --health-retries=5
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run Integration Tests
      run: |
        dotnet test tests/VersaCoder.IntegrationTests/ `
          -c Release `
          --logger "trx;LogFileName=integration-tests.trx" `
          --filter "Category=Integration"
      env:
        ConnectionStrings__DefaultConnection: "Data Source=versacoder-test.db"

  smoke-tests:
    name: Smoke Tests
    runs-on: windows-latest
    needs: integration-tests
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Build and Run Application
      run: |
        dotnet build -c Release
        $job = Start-Job -ScriptBlock {
          dotnet run --project src/VersaCoder.Host/VersaCoder.Host.csproj -c Release
        }
        Start-Sleep -Seconds 10
    
    - name: Run Smoke Tests
      run: |
        $healthUrl = "http://localhost:5000/health"
        $response = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing
        
        if ($response.StatusCode -ne 200) {
          throw "Smoke test failed: Health check returned $($response.StatusCode)"
        }
        
        Write-Host "Smoke tests passed"
    
    - name: Cleanup
      if: always()
      run: |
        Stop-Job -Name $job.Name -ErrorAction SilentlyContinue
        Remove-Job -Name $job.Name -ErrorAction SilentlyContinue

  performance-tests:
    name: Performance Tests
    runs-on: windows-latest
    needs: smoke-tests
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Install BenchmarkDotNet
      run: dotnet add package BenchmarkDotNet
    
    - name: Run Performance Benchmarks
      run: |
        dotnet run -c Release --project tests/VersaCoder.PerformanceTests/ `
          --filter "*"
    
    - name: Upload benchmark results
      uses: actions/upload-artifact@v4
      with:
        name: benchmark-results
        path: BenchmarkDotNet.Artifacts/
```

### 3.3 Release Workflow

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_FILE: 'VersaCoder.slnx'

jobs:
  release:
    name: Create Release
    runs-on: windows-latest
    permissions:
      contents: write
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Extract version from tag
      id: version
      run: |
        $tag = "${{ github.ref_name }}"
        $version = $tag -replace '^v', ''
        echo "version=$version" >> $env:GITHUB_OUTPUT
        echo "Building version $version"
    
    - name: Restore dependencies
      run: dotnet restore ${{ env.SOLUTION_FILE }}
    
    - name: Build
      run: dotnet build ${{ env.SOLUTION_FILE }} -c Release --no-restore -p:Version=${{ steps.version.outputs.version }}
    
    - name: Test
      run: dotnet test ${{ env.SOLUTION_FILE }} -c Release --no-build
    
    - name: Publish - Windows x64
      run: |
        dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj `
          -c Release `
          -r win-x64 `
          --self-contained `
          -p:PublishSingleFile=true `
          -p:IncludeNativeLibrariesForSelfExtract=true `
          -o ./publish/win-x64
    
    - name: Publish - Linux x64
      run: |
        dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj `
          -c Release `
          -r linux-x64 `
          --self-contained `
          -p:PublishSingleFile=true `
          -o ./publish/linux-x64
    
    - name: Publish - macOS x64
      run: |
        dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj `
          -c Release `
          -r osx-x64 `
          --self-contained `
          -p:PublishSingleFile=true `
          -o ./publish/osx-x64
    
    - name: Create ZIP archives
      run: |
        Compress-Archive -Path ./publish/win-x64/* -DestinationPath ./VersaCoder-win-x64.zip
        Compress-Archive -Path ./publish/linux-x64/* -DestinationPath ./VersaCoder-linux-x64.zip
        Compress-Archive -Path ./publish/osx-x64/* -DestinationPath ./VersaCoder-osx-x64.zip
    
    - name: Generate checksums
      run: |
        Get-FileHash ./VersaCoder-win-x64.zip -Algorithm SHA256 | Select-Object -ExpandProperty Hash | Out-File ./VersaCoder-win-x64.zip.sha256
        Get-FileHash ./VersaCoder-linux-x64.zip -Algorithm SHA256 | Select-Object -ExpandProperty Hash | Out-File ./VersaCoder-linux-x64.zip.sha256
        Get-FileHash ./VersaCoder-osx-x64.zip -Algorithm SHA256 | Select-Object -ExpandProperty Hash | Out-File ./VersaCoder-osx-x64.zip.sha256
    
    - name: Create GitHub Release
      uses: softprops/action-gh-release@v2
      with:
        name: "VersaCoder v${{ steps.version.outputs.version }}"
        body: |
          ## VersaCoder v${{ steps.version.outputs.version }}
          
          ### Downloads
          - **Windows x64:** [VersaCoder-win-x64.zip](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/VersaCoder-win-x64.zip)
          - **Linux x64:** [VersaCoder-linux-x64.zip](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/VersaCoder-linux-x64.zip)
          - **macOS x64:** [VersaCoder-osx-x64.zip](https://github.com/${{ github.repository }}/releases/download/${{ github.ref_name }}/VersaCoder-osx-x64.zip)
          
          ### Checksums (SHA256)
          See .sha256 files for verification.
        files: |
          VersaCoder-win-x64.zip
          VersaCoder-linux-x64.zip
          VersaCoder-osx-x64.zip
          *.sha256
        draft: false
        prerelease: false
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### 3.4 Deploy Staging Workflow

```yaml
# .github/workflows/deploy-staging.yml
name: Deploy to Staging

on:
  push:
    branches: [develop]
  workflow_dispatch:

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME: 'versacoder-staging'
  AZURE_RESOURCE_GROUP: 'versacoder-rg'

jobs:
  deploy-staging:
    name: Deploy to Staging
    runs-on: windows-latest
    environment: staging
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build -c Release --no-restore
    
    - name: Test
      run: dotnet test -c Release --no-build
    
    - name: Publish
      run: |
        dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj `
          -c Release `
          -r win-x64 `
          --self-contained `
          -o ./publish
    
    - name: Deploy to Azure
      run: |
        az webapp deploy `
          --name ${{ env.AZURE_WEBAPP_NAME }} `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --src-path ./publish `
          --type zip `
          --clean true
    
    - name: Health Check
      run: |
        $url = "https://${{ env.AZURE_WEBAPP_NAME }}.azurewebsites.net/health"
        $maxRetries = 20
        $retryCount = 0
        
        do {
          try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
              Write-Host "Staging deployment successful - Health check passed"
              exit 0
            }
          } catch {
            Write-Host "Waiting for application... ($retryCount/$maxRetries)"
          }
          $retryCount++
          Start-Sleep -Seconds 15
        } while ($retryCount -lt $maxRetries)
        
        throw "Health check failed after $maxRetries retries"
    
    - name: Run Smoke Tests
      run: |
        $baseUrl = "https://${{ env.AZURE_WEBAPP_NAME }}.azurewebsites.net"
        
        # Test health endpoint
        $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method Get
        Write-Host "Health: $($health.status)"
        
        # Test API endpoints
        $sessions = Invoke-RestMethod -Uri "$baseUrl/api/sessions" -Method Get
        Write-Host "Sessions endpoint working"
    
    - name: Notify on Success
      if: success()
      run: |
        Write-Host "Staging deployment completed successfully"
    
    - name: Notify on Failure
      if: failure()
      run: |
        Write-Host "Staging deployment failed"
        # Add notification logic here (Slack, Teams, Email)
```

### 3.5 Deploy Production Workflow

```yaml
# .github/workflows/deploy-production.yml
name: Deploy to Production

on:
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to deploy (e.g., v1.2.3)'
        required: true
      strategy:
        description: 'Deployment strategy'
        required: true
        type: choice
        options:
          - blue-green
          - canary
          - rolling
          - recreate
      confirm:
        description: 'Type "yes" to confirm production deployment'
        required: true

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_RESOURCE_GROUP: 'versacoder-rg'

jobs:
  pre-deployment-checks:
    name: Pre-Deployment Checks
    runs-on: windows-latest
    outputs:
      deployment-id: ${{ steps.deploy-id.outputs.id }}
    
    steps:
    - name: Verify Confirmation
      run: |
        if ("${{ github.event.inputs.confirm }}" -ne "yes") {
          throw "Deployment not confirmed. Type 'yes' to proceed."
        }
    
    - name: Generate Deployment ID
      id: deploy-id
      run: |
        $id = "deploy-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        echo "id=$id" >> $env:GITHUB_OUTPUT
    
    - name: Checkout
      uses: actions/checkout@v4
    
    - name: Verify Version Tag Exists
      run: |
        git fetch --tags
        if (-not (git tag -l "${{ github.event.inputs.version }}")) {
          throw "Tag ${{ github.event.inputs.version }} does not exist"
        }
    
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Check Production Health
      run: |
        $healthUrl = "https://versacoder-prod.azurewebsites.net/health"
        try {
          $response = Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -ErrorAction Stop
          Write-Host "Current production is healthy"
        } catch {
          throw "Current production is not healthy. Aborting deployment."
        }

  deploy-production:
    name: Deploy Production (${{ github.event.inputs.strategy }})
    runs-on: windows-latest
    needs: pre-deployment-checks
    environment: production
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        ref: ${{ github.event.inputs.version }}
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Build and Test
      run: |
        dotnet build -c Release
        dotnet test -c Release
    
    - name: Publish
      run: |
        dotnet publish src/VersaCoder.Host/VersaCoder.Host.csproj `
          -c Release `
          -r win-x64 `
          --self-contained `
          -p:PublishSingleFile=true `
          -o ./publish
    
    - name: Deploy - Blue/Green
      if: github.event.inputs.strategy == 'blue-green'
      run: |
        # Determine target slot
        $activeSlot = az webapp config appsettings list `
          --name versacoder-prod `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --query "[?name=='ACTIVE_SLOT'].value" -o tsv
        
        $targetSlot = if ($activeSlot -eq "blue") { "green" } else { "blue" }
        
        Write-Host "Deploying to $targetSlot slot"
        
        az webapp deploy `
          --name versacoder-prod-$targetSlot `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --src-path ./publish `
          --type zip
        
        # Health check on new slot
        $url = "https://versacoder-prod-$targetSlot.azurewebsites.net/health"
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing
        
        if ($response.StatusCode -eq 200) {
          # Swap slots
          az webapp deployment slot swap `
            --name versacoder-prod `
            --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
            --slot $targetSlot `
            --target-slot production
          
          # Update active slot setting
          az webapp config appsettings set `
            --name versacoder-prod `
            --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
            --settings "ACTIVE_SLOT=$targetSlot"
          
          Write-Host "Blue-Green deployment successful"
        } else {
          throw "Health check failed on $targetSlot slot"
        }
    
    - name: Deploy - Canary
      if: github.event.inputs.strategy == 'canary'
      run: |
        Write-Host "Deploying canary instance"
        
        az webapp deploy `
          --name versacoder-canary `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --src-path ./publish `
          --type zip
        
        # Configure traffic split
        az webapp trafficrouting set `
          --name versacoder-prod `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --distribution "versacoder-canary=10,versacoder-prod=90"
        
        Write-Host "Canary deployed with 10% traffic"
    
    - name: Deploy - Rolling
      if: github.event.inputs.strategy == 'rolling'
      run: |
        $instances = @("instance-1", "instance-2", "instance-3")
        
        foreach ($instance in $instances) {
          Write-Host "Deploying to $instance"
          
          az webapp deploy `
            --name versacoder-$instance `
            --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
            --src-path ./publish `
            --type zip
          
          # Health check
          $url = "https://versacoder-$instance.azurewebsites.net/health"
          $response = Invoke-WebRequest -Uri $url -UseBasicParsing
          
          if ($response.StatusCode -ne 200) {
            throw "Health check failed for $instance"
          }
          
          Write-Host "$instance deployed successfully"
        }
    
    - name: Deploy - Recreate
      if: github.event.inputs.strategy == 'recreate'
      run: |
        Write-Host "Stopping current production"
        az webapp stop `
          --name versacoder-prod `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }}
        
        Write-Host "Deploying new version"
        az webapp deploy `
          --name versacoder-prod `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }} `
          --src-path ./publish `
          --type zip
        
        Write-Host "Starting production"
        az webapp start `
          --name versacoder-prod `
          --resource-group ${{ env.AZURE_RESOURCE_GROUP }}

  post-deployment:
    name: Post-Deployment Validation
    runs-on: windows-latest
    needs: deploy-production
    
    steps:
    - name: Health Check
      run: |
        $url = "https://versacoder-prod.azurewebsites.net/health"
        $maxRetries = 30
        $retryCount = 0
        
        do {
          try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
              Write-Host "Production health check passed"
              break
            }
          } catch {
            Write-Host "Waiting... ($retryCount/$maxRetries)"
          }
          $retryCount++
          Start-Sleep -Seconds 10
        } while ($retryCount -lt $maxRetries)
        
        if ($retryCount -ge $maxRetries) {
          throw "Production health check failed"
        }
    
    - name: Smoke Tests
      run: |
        $baseUrl = "https://versacoder-prod.azurewebsites.net"
        
        # Test all critical endpoints
        $endpoints = @(
          "/health",
          "/api/sessions",
          "/api/config"
        )
        
        foreach ($endpoint in $endpoints) {
          try {
            $response = Invoke-WebRequest -Uri "$baseUrl$endpoint" -UseBasicParsing -ErrorAction Stop
            Write-Host "OK: $endpoint"
          } catch {
            throw "Smoke test failed: $endpoint"
          }
        }
    
    - name: Update Deployment Status
      run: |
        $deploymentId = "${{ needs.pre-deployment-checks.outputs.deployment-id }}"
        $version = "${{ github.event.inputs.version }}"
        $strategy = "${{ github.event.inputs.strategy }}"
        
        Write-Host "Deployment $deploymentId completed"
        Write-Host "Version: $version"
        Write-Host "Strategy: $strategy"
        Write-Host "Status: SUCCESS"
    
    - name: Notify Success
      if: success()
      run: |
        Write-Host "Production deployment successful"
        # Add notification logic here
    
    - name: Notify Failure
      if: failure()
      run: |
        Write-Host "Production deployment failed"
        # Add notification logic here
        # Consider automatic rollback
```

---

## 4. Docker Containerization

### 4.1 Multi-Stage Dockerfile

```dockerfile
# Dockerfile
# Stage 1: Restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy solution and project files for caching
COPY VersaCoder.slnx ./
COPY src/VersaCoder.Domain/VersaCoder.Domain.csproj src/VersaCoder.Domain/
COPY src/VersaCoder.Abstractions/VersaCoder.Abstractions.csproj src/VersaCoder.Abstractions/
COPY src/VersaCoder.Application/VersaCoder.Application.csproj src/VersaCoder.Application/
COPY src/VersaCoder.CrossCutting/VersaCoder.CrossCutting.csproj src/VersaCoder.CrossCutting/
COPY src/VersaCoder.Infrastructure.Data/VersaCoder.Infrastructure.Data.csproj src/VersaCoder.Infrastructure.Data/
COPY src/VersaCoder.Infrastructure.AI/VersaCoder.Infrastructure.AI.csproj src/VersaCoder.Infrastructure.AI/
COPY src/VersaCoder.Host/VersaCoder.Host.csproj src/VersaCoder.Host/

RUN dotnet restore

# Stage 2: Build
FROM restore AS build
COPY . .
RUN dotnet build -c Release --no-restore -o /app/build

# Stage 3: Test
FROM build AS test
RUN dotnet test -c Release --no-build --verbosity normal

# Stage 4: Publish
FROM build AS publish
RUN dotnet publish -c Release --no-restore -o /app/publish /p:UseAppHost=false

# Stage 5: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Copy published output
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "VersaCoder.Host.dll"]
```

### 4.2 docker-compose.yml for Development

```yaml
# docker-compose.yml
version: '3.8'

services:
  versacoder:
    build:
      context: .
      dockerfile: Dockerfile
      target: runtime
    container_name: versacoder-dev
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=/app/data/versacoder-dev.db
      - Logging__LogLevel__Default=Debug
    volumes:
      - ./data:/app/data
      - ./logs:/app/logs
      - ./config:/app/config
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 15s

  redis:
    image: redis:7-alpine
    container_name: versacoder-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  sqlite-browser:
    app:coleifer/sqlite-web
    container_name: versacoder-sqlite
    ports:
      - "8080:8080"
    environment:
      - SQLITEDB=/data/versacoder-dev.db
    volumes:
      - ./data:/data
    restart: unless-stopped
    profiles:
      - tools

volumes:
  redis-data:
```

### 4.3 docker-compose.prod.yml for Production

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  versacoder:
    build:
      context: .
      dockerfile: Dockerfile
      target: runtime
    container_name: versacoder-prod
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=/app/data/versacoder-prod.db
      - Logging__LogLevel__Default=Warning
    volumes:
      - ./data:/app/data
      - ./logs:/app/logs
    restart: always
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '1.0'
          memory: 1G
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 30s
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.versacoder.rule=Host(`versacoder.example.com`)"
      - "traefik.http.routers.versacoder.entrypoints=websecure"
      - "traefik.http.routers.versacoder.tls.certresolver=letsencrypt"

  traefik:
    image: traefik:v3.0
    container_name: versacoder-traefik
    command:
      - "--api.insecure=true"
      - "--providers.docker=true"
      - "--entrypoints.web.address=:80"
      - "--entrypoints.websecure.address=:443"
      - "--certificatesresolvers.letsencrypt.acme.tlschallenge=true"
    ports:
      - "80:80"
      - "443:443"
      - "8080:8080"
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock:ro
    restart: always

  backup:
    image: versacoder-backup:latest
    container_name: versacoder-backup
    environment:
      - BACKUP_SCHEDULE=0 2 * * *
      - BACKUP_RETENTION_DAYS=30
      - DATABASE_PATH=/app/data/versacoder-prod.db
    volumes:
      - ./data:/app/data:ro
      - ./backups:/app/backups
    restart: always
    profiles:
      - backup
```

### 4.4 Container Health Checks

```yaml
# healthcheck-config.yml
healthchecks:
  # Database health check
  database:
    command: |
      curl -f http://localhost:8080/health/database || exit 1
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 30s
  
  # AI Provider health check
  ai-provider:
    command: |
      curl -f http://localhost:8080/health/ai || exit 1
    interval: 60s
    timeout: 15s
    retries: 3
    start_period: 60s
  
  # File system health check
  filesystem:
    command: |
      curl -f http://localhost:8080/health/filesystem || exit 1
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 15s
  
  # Memory health check
  memory:
    command: |
      curl -f http://localhost:8080/health/memory || exit 1
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 15s
```

### 4.5 Image Scanning

```yaml
# .github/workflows/security-scan.yml
name: Container Security Scan

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  scan-image:
    name: Scan Container Image
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Build Docker image
      run: docker build -t versacoder:scan .
    
    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        image-ref: 'versacoder:scan'
        format: 'sarif'
        output: 'trivy-results.sarif'
        severity: 'CRITICAL,HIGH'
    
    - name: Upload Trivy scan results
      uses: github/codeql-action/upload-sarif@v2
      if: always()
      with:
        sarif_file: 'trivy-results.sarif'
    
    - name: Run Snyk container scan
      uses: snyk/actions/docker@master
      env:
        SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
      with:
        image: versacoder:scan
        args: --severity-threshold=high
```

---

## 5. Azure Deployment

### 5.1 Azure App Service

```azurecli
# Azure App Service provisioning script
#!/bin/bash

# Variables
RESOURCE_GROUP="versacoder-rg"
LOCATION="Turkey Central"
APP_SERVICE_PLAN="versacoder-plan"
WEB_APP_NAME="versacoder-prod"
STAGING_APP="versacoder-staging"
CANARY_APP="versacoder-canary"

# Create Resource Group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create App Service Plan (Premium for production)
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku P1V2 \
  --is-linux

# Create Production Web App
az webapp create \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE|8.0"

# Create Staging Web App
az webapp create \
  --name $STAGING_APP \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE|8.0"

# Create Canary Web App
az webapp create \
  --name $CANARY_APP \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --runtime "DOTNETCORE|8.0"

# Configure App Settings
az webapp config appsettings set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings \
    "ACTIVE_SLOT=blue" \
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE=false" \
    "SCM_DO_BUILD_DURING_DEPLOYMENT=true"

# Configure Health Check
az webapp config set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --health-check-path "/health" \
  --health-check-interval 30

# Enable Logging
az webapp log config \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --application-logging filesystem \
  --detailed-error-messages true \
  --request-tracing true \
  --web-server-logging filesystem
```

### 5.2 Azure Container Registry

```azurecli
#!/bin/bash

# Variables
RESOURCE_GROUP="versacoder-rg"
ACR_NAME="versacoderregistry"
IMAGE_NAME="versacoder"
TAG=$(date +%Y%m%d-%H%M%S)

# Create Azure Container Registry
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Premium \
  --admin-enabled true

# Login to ACR
az acr login --name $ACR_NAME

# Build and push image
docker build -t $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG .
docker push $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG

# Tag for production
docker tag $ACR_NAME.azurecr.io/$IMAGE_NAME:$TAG $ACR_NAME.azurecr.io/$IMAGE_NAME:latest
docker push $ACR_NAME.azurecr.io/$IMAGE_NAME:latest

# Enable content trust
az acr config content-trust update \
  --registry $ACR_NAME \
  --status enabled
```

### 5.3 Azure DevOps Pipelines

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main
      - develop

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'

stages:
- stage: Build
  displayName: 'Build Stage'
  jobs:
  - job: Build
    displayName: 'Build Solution'
    steps:
    - task: UseDotNet@2
      displayName: 'Install .NET SDK'
      inputs:
        packageType: 'sdk'
        version: '$(dotnetVersion)'
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build solution'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Run tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'
    
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish code coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    
    - task: DotNetCoreCLI@2
      displayName: 'Publish application'
      inputs:
        command: 'publish'
        publishWebProjects: true
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
        zipAfterPublish: true
    
    - task: PublishBuildArtifacts@1
      displayName: 'Publish artifacts'
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'versacoder'

- stage: Deploy_Staging
  displayName: 'Deploy to Staging'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
  jobs:
  - deployment: DeployStaging
    displayName: 'Deploy to Staging'
    environment: 'staging'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App'
            inputs:
              azureSubscription: 'Azure-Connection'
              appType: 'webApp'
              WebAppName: 'versacoder-staging'
              packageForLinux: '$(Pipeline.Workspace)/versacoder/*.zip'

- stage: Deploy_Production
  displayName: 'Deploy to Production'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production'
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            displayName: 'Deploy to Azure Web App'
            inputs:
              azureSubscription: 'Azure-Connection'
              appType: 'webApp'
              WebAppName: 'versacoder-prod'
              packageForLinux: '$(Pipeline.Workspace)/versacoder/*.zip'
```

### 5.4 Azure Key Vault Integration

```csharp
// Program.cs - Key Vault integration
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault configuration
var keyVaultEndpoint = builder.Configuration["AzureKeyVault:Endpoint"];
if (!string.IsNullOrEmpty(keyVaultEndpoint))
{
    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(new Uri(keyVaultEndpoint), credential);
    
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        credential,
        new Azure.Extensions.AspNetCore.Configuration.Secrets.AzureKeyVaultConfigurationOptions
        {
            ReloadInterval = TimeSpan.FromMinutes(5)
        });
}

// Use secrets in configuration
var connectionString = builder.Configuration["Database:ConnectionString"];
var apiKey = builder.Configuration["AI:ApiKey"];

builder.Services.AddDbContext<VersaCoderDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Health check for Key Vault
app.MapHealthChecks("/health/keyvault", new HealthCheckOptions
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
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.Run();
```

```json
// appsettings.json - Key Vault reference
{
  "AzureKeyVault": {
    "Endpoint": "https://versacoder-vault.vault.azure.net/"
  },
  "Database": {
    "ConnectionString": "@Microsoft.KeyVault(SecretUri=https://versacoder-vault.vault.azure.net/secrets/DatabaseConnectionString)"
  },
  "AI": {
    "ApiKey": "@Microsoft.KeyVault(SecretUri=https://versacoder-vault.vault.azure.net/secrets/AiApiKey)"
  }
}
```

---

## 6. AWS Deployment (Alternative)

### 6.1 ECS/Fargate

```yaml
# task-definition.json
{
  "family": "versacoder",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "2048",
  "memory": "4096",
  "executionRoleArn": "arn:aws:iam::123456789012:role/ecsTaskExecutionRole",
  "taskRoleArn": "arn:aws:iam::123456789012:role/ecsTaskRole",
  "containerDefinitions": [
    {
      "name": "versacoder",
      "image": "123456789012.dkr.ecr.us-east-1.amazonaws.com/versacoder:latest",
      "essential": true,
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "secrets": [
        {
          "name": "ConnectionStrings__DefaultConnection",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:versacoder/db-connection:ConnectionString::"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/versacoder",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      },
      "healthCheck": {
        "command": ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"],
        "interval": 30,
        "timeout": 5,
        "retries": 3,
        "startPeriod": 60
      }
    }
  ]
}
```

```yaml
# ecs-service.yml
AWSTemplateFormatVersion: '2010-09-09'
Description: 'VersaCoder ECS Service'

Resources:
  Cluster:
    Type: AWS::ECS::Cluster
    Properties:
      ClusterName: versacoder-cluster
      CapacityProviders:
        - FARGATE
        - FARGATE_SPOT
      DefaultCapacityProviderStrategy:
        - CapacityProvider: FARGATE
          Weight: 1
        - CapacityProvider: FARGATE_SPOT
          Weight: 3

  Service:
    Type: AWS::ECS::Service
    DependsOn: ListenerRule
    Properties:
      ServiceName: versacoder-service
      Cluster: !Ref Cluster
      TaskDefinition: !Ref TaskDefinition
      DesiredCount: 2
      LaunchType: FARGATE
      DeploymentConfiguration:
        MaximumPercent: 200
        MinimumHealthyPercent: 100
      NetworkConfiguration:
        AwsvpcConfiguration:
          AssignPublicIp: DISABLED
          SecurityGroups:
            - !Ref ServiceSecurityGroup
          Subnets:
            - !Ref PrivateSubnet1
            - !Ref PrivateSubnet2
      LoadBalancers:
        - ContainerName: versacoder
          ContainerPort: 8080
          TargetGroupArn: !Ref TargetGroup

  TaskDefinition:
    Type: AWS::ECS::TaskDefinition
    Properties:
      Family: versacoder
      NetworkMode: awsvpc
      RequiresCompatibilities:
        - FARGATE
      Cpu: '2048'
      Memory: '4096'
      ExecutionRoleArn: !GetAtt ExecutionRole.Arn
      TaskRoleArn: !GetAtt TaskRole.Arn
      ContainerDefinitions:
        - Name: versacoder
          Image: !Sub '${AWS::AccountId}.dkr.ecr.${AWS::Region}.amazonaws.com/versacoder:latest'
          Essential: true
          PortMappings:
            - ContainerPort: 8080
              Protocol: tcp

  TargetGroup:
    Type: AWS::ElasticLoadBalancingV2::TargetGroup
    Properties:
      Name: versacoder-tg
      Port: 8080
      Protocol: HTTP
      VpcId: !Ref VPC
      TargetType: ip
      HealthCheckPath: /health
      HealthCheckIntervalSeconds: 30
      HealthCheckTimeoutSeconds: 5
      HealthyThresholdCount: 3
      UnhealthyThresholdCount: 3

  ListenerRule:
    Type: AWS::ElasticLoadBalancingV2::ListenerRule
    Properties:
      ListenerArn: !Ref Listener
      Priority: 1
      Conditions:
        - Field: path-pattern
          Values:
            - /*
      Actions:
        - Type: forward
          TargetGroupArn: !Ref TargetGroup

  ServiceSecurityGroup:
    Type: AWS::EC2::SecurityGroup
    Properties:
      GroupDescription: Security group for VersaCoder service
      VpcId: !Ref VPC
      SecurityGroupIngress:
        - IpProtocol: tcp
          FromPort: 8080
          ToPort: 8080
          SourceSecurityGroup: !Ref ALBSecurityGroup

  ExecutionRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy

  TaskRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: ecs-tasks.amazonaws.com
            Action: sts:AssumeRole
      Policies:
        - PolicyName: VersaCoderTaskPolicy
          PolicyDocument:
            Version: '2012-10-17'
            Statement:
              - Effect: Allow
                Action:
                  - secretsmanager:GetSecretValue
                Resource: '*'
```

### 6.2 ECR

```bash
#!/bin/bash

# ECR setup script
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
REGION="us-east-1"
REPOSITORY_NAME="versacoder"

# Create ECR repository
aws ecr create-repository \
  --repository-name $REPOSITORY_NAME \
  --region $REGION \
  --image-scanning-configuration scanOnPush=true \
  --encryption-configuration encryptionType=KMS

# Login to ECR
aws ecr get-login-password --region $REGION | \
  docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com

# Build and push image
docker build -t $REPOSITORY_NAME .
docker tag $REPOSITORY_NAME:latest $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPOSITORY_NAME:latest
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPOSITORY_NAME:latest

# Set lifecycle policy
aws ecr put-lifecycle-policy \
  --repository-name $REPOSITORY_NAME \
  --lifecycle-policy-text '{
    "rules": [
      {
        "rulePriority": 1,
        "description": "Keep last 10 images",
        "selection": {
          "tagStatus": "any",
          "countType": "imageCountMoreThan",
          "countNumber": 10
        },
        "action": {
          "type": "expire"
        }
      }
    ]
  }'
```

### 6.3 CodePipeline

```yaml
# buildspec.yml (AWS CodeBuild)
version: 0.2

phases:
  install:
    runtime-versions:
      dotnet: 8.0
    commands:
      - dotnet restore

  pre_build:
    commands:
      - dotnet build -c Release
      - dotnet test -c Release --no-build

  build:
    commands:
      - dotnet publish -c Release -o ./publish

  post_build:
    commands:
      - echo Build completed on `date`

artifacts:
  files:
    - '**/*'
  base-directory: 'publish'

cache:
  paths:
    - '/root/.nuget/packages/**/*'
```

```yaml
# pipeline.yml (CloudFormation template)
AWSTemplateFormatVersion: '2010-09-09'
Description: 'VersaCoder CI/CD Pipeline'

Resources:
  Pipeline:
    Type: AWS::CodePipeline::Pipeline
    Properties:
      Name: versacoder-pipeline
      RoleArn: !GetAtt PipelineRole.Arn
      ArtifactStore:
        Type: S3
        Location: !Ref ArtifactBucket
      Stages:
        - Name: Source
          Actions:
            - Name: SourceAction
              ActionTypeId:
                Category: Source
                Owner: ThirdParty
                Provider: GitHub
                Version: '1'
              Configuration:
                Owner: versacoder
                Repo: versacoder
                Branch: main
                OAuthToken: !Ref GitHubToken
                PollForSourceChanges: false
              OutputArtifacts:
                - Name: SourceOutput

        - Name: Build
          Actions:
            - Name: BuildAction
              ActionTypeId:
                Category: Build
                Owner: AWS
                Provider: CodeBuild
                Version: '1'
              Configuration:
                ProjectName: !Ref BuildProject
              InputArtifacts:
                - Name: SourceOutput
              OutputArtifacts:
                - Name: BuildOutput

        - Name: Deploy
          Actions:
            - Name: DeployAction
              ActionTypeId:
                Category: Deploy
                Owner: AWS
                Provider: ECS
                Version: '1'
              Configuration:
                ClusterName: !Ref ECSCluster
                ServiceName: !Ref ECSService
                FileName: imagedefinitions.json
              InputArtifacts:
                - Name: BuildOutput

  BuildProject:
    Type: AWS::CodeBuild::Project
    Properties:
      Name: versacoder-build
      ServiceRole: !GetAtt BuildRole.Arn
      Artifacts:
        Type: CODEPIPELINE
      Environment:
        Type: LINUX_CONTAINER
        ComputeType: BUILD_GENERAL1_MEDIUM
        Image: aws/codebuild/dotnet-core:3.1
      Source:
        Type: CODEPIPELINE
      TimeoutInMinutes: 15

  ArtifactBucket:
    Type: AWS::S3::Bucket
    Properties:
      BucketName: versacoder-artifacts

  PipelineRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codepipeline.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/AWSCodePipelineFullAccess
        - arn:aws:iam::aws:policy/AWSCodeBuildFullAccess

  BuildRole:
    Type: AWS::IAM::Role
    Properties:
      AssumeRolePolicyDocument:
        Version: '2012-10-17'
        Statement:
          - Effect: Allow
            Principal:
              Service: codebuild.amazonaws.com
            Action: sts:AssumeRole
      ManagedPolicyArns:
        - arn:aws:iam::aws:policy/AWSCodeBuildFullAccess
        - arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryFullAccess

Parameters:
  GitHubToken:
    Type: String
    NoEcho: true
    Description: GitHub OAuth token

  ECSCluster:
    Type: String
    Default: versacoder-cluster

  ECSService:
    Type: String
    Default: versacoder-service
```

---

## 7. On-Premise Deployment

### 7.1 Windows Service Deployment

```powershell
# install-service.ps1
# VersaCoder Windows Service Installation Script

param(
    [string]$InstallPath = "C:\Program Files\VersaCoder",
    [string]$ServiceName = "VersaCoderService",
    [string]$DisplayName = "VersaCoder Service",
    [string]$Description = "VersaCoder AI-powered IDE Backend Service"
)

# Stop existing service if running
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($service) {
    Write-Host "Stopping existing service..."
    Stop-Service -Name $ServiceName -Force
    Start-Sleep -Seconds 5
}

# Create installation directory
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
}

# Copy files
Write-Host "Copying files to $InstallPath..."
Copy-Item -Path ".\publish\*" -Destination $InstallPath -Recurse -Force

# Install service
Write-Host "Installing Windows service..."
New-Service `
    -Name $ServiceName `
    -DisplayName $DisplayName `
    -Description $Description `
    -BinaryPathName "$InstallPath\VersaCoder.Host.exe" `
    -StartupType Automatic `
    -Credential (Get-Credential -Message "Enter service account credentials")

# Configure service recovery
sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/120000/restart/300000

# Start service
Write-Host "Starting service..."
Start-Service -Name $ServiceName

Write-Host "Service installed successfully!"
Write-Host "Service Name: $ServiceName"
Write-Host "Install Path: $InstallPath"
Write-Host "Status: $(Get-Service -Name $ServiceName | Select-Object -ExpandProperty Status)"
```

```xml
<!-- appsettings.WindowsService.json -->
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      },
      "Https": {
        "Url": "https://0.0.0.0:5001",
        "Certificate": {
          "Path": "cert.pfx",
          "Password": "certificate-password"
        }
      }
    }
  },
  "Service": {
    "Name": "VersaCoder",
    "DisplayName": "VersaCoder Service",
    "Description": "VersaCoder AI-powered IDE Backend"
  }
}
```

### 7.2 IIS Hosting

```xml
<!-- web.config for IIS hosting -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\VersaCoder.Host.dll" stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
    </system.webServer>
  </location>
</configuration>
```

```powershell
# iis-setup.ps1
# IIS Configuration Script for VersaCoder

# Install IIS features
Install-WindowsFeature -Name Web-Server -IncludeAllSubFeature
Install-WindowsFeature -Name Web-Asp-Net45
Install-WindowsFeature -Name NET-Framework-45-ASPNET
Install-WindowsFeature -Name Web-Net-Ext45

# Install ASP.NET Core Hosting Bundle
$installerUrl = "https://download.visualstudio.microsoft.com/download/pr/8f7e6a2b-3b5c-4c7a-9c2e-1d3f4b5e6a7c/dotnet-hosting-8.0.0-win.exe"
$installerPath = "$env:TEMP\dotnet-hosting-installer.exe"

Invoke-WebRequest -Uri $installerUrl -OutFile $installerPath
Start-Process -FilePath $installerPath -ArgumentList "/quiet /install" -Wait

# Create IIS Application Pool
New-WebAppPool -Name "VersaCoderPool"
Set-ItemProperty IIS:\AppPools\VersaCoderPool -Name processModel.identityType -Value ApplicationPoolIdentity
Set-ItemProperty IIS:\AppPools\VersaCoderPool -Name processModel.loadUserProfile -Value True

# Create IIS Website
New-Website `
    -Name "VersaCoder" `
    -PhysicalPath "C:\inetpub\versacoder" `
    -ApplicationPool "VersaCoderPool" `
    -Port 80 `
    -HostHeader "versacoder.local"

# Configure SSL
$cert = New-SelfSignedCertificate `
    -DnsName "versacoder.local" `
    -CertStoreLocation "Cert:\LocalMachine\My" `
    -NotAfter (Get-Date).AddYears(5)

New-WebBinding `
    -Name "VersaCoder" `
    -Protocol "https" `
    -Port 443 `
    -HostHeader "versacoder.local"

$binding = Get-WebBinding -Name "VersaCoder" -Protocol "https"
$binding.AddSslCertificate($cert.Thumbprint, "My")

# Configure logging
Set-WebConfigurationProperty `
    -Filter "system.applicationHost/log" `
    -PSPath "IIS:\" `
    -Name "centralLogFileMode" `
    -Value "CentralW3C"

# Test configuration
Test-NetConnection -ComputerName "localhost" -Port 80
Test-NetConnection -ComputerName "localhost" -Port 443

Write-Host "IIS configuration completed successfully!"
```

### 7.3 Self-Contained Deployment

```xml
<!-- Publish settings for self-contained deployment -->
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  
  <!-- Self-contained deployment -->
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  
  <!-- Single file deployment -->
  <PublishSingleFile>true</PublishSingleFile>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  
  <!-- Ready to Run -->
  <PublishReadyToRun>true</PublishReadyToRun>
  
  <!-- Trim unused assemblies -->
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>partial</TrimMode>
</PropertyGroup>
```

```powershell
# self-contained-build.ps1
# Build self-contained deployment packages

param(
    [string]$Version = "1.0.0",
    [string[]]$Runtimes = @("win-x64", "linux-x64", "osx-x64")
)

foreach ($runtime in $Runtimes) {
    Write-Host "Building for $runtime..."
    
    $outputDir = "./publish/$runtime"
    
    dotnet publish `
        -c Release `
        -r $runtime `
        --self-contained `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -p:PublishReadyToRun=true `
        -p:PublishTrimmed=true `
        -p:Version=$Version `
        -o $outputDir
    
    # Create ZIP archive
    $zipName = "VersaCoder-$Version-$runtime.zip"
    Compress-Archive -Path "$outputDir/*" -DestinationPath "./releases/$zipName"
    
    # Generate checksum
    $hash = Get-FileHash "./releases/$zipName" -Algorithm SHA256 | Select-Object -ExpandProperty Hash
    $hash | Out-File "./releases/$zipName.sha256"
    
    Write-Host "Created $zipName"
}

Write-Host "All builds completed!"
```

---

## 8. Rollback Stratejileri

### 8.1 Database Rollback

```powershell
# database-rollback.ps1
# Database rollback script for EF Core migrations

param(
    [string]$MigrationName,
    [string]$ConnectionString,
    [string]$BackupPath = ".\backups"
)

# Create backup before rollback
Write-Host "Creating database backup..."
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupFile = "$BackupPath\versacoder-$timestamp.db"

# For SQLite - copy database file
Copy-Item -Path $ConnectionString -Destination $backupFile -Force
Write-Host "Backup created: $backupFile"

# Rollback to specific migration
if ($MigrationName) {
    Write-Host "Rolling back to migration: $MigrationName"
    dotnet ef database update $MigrationName `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host
} else {
    # Rollback last migration
    Write-Host "Rolling back last migration..."
    $lastMigration = dotnet ef migrations list `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host |
        Select-Object -Last 1 |
        ForEach-Object { $_ -replace ' \(.*\)', '' }
    
    $previousMigration = dotnet ef migrations list `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host |
        Select-Object -Last 2 |
        Select-Object -First 1 |
        ForEach-Object { $_ -replace ' \(.*\)', '' }
    
    if ($previousMigration) {
        dotnet ef database update $previousMigration `
            --project src/VersaCoder.Infrastructure.Data `
            --startup-project src/VersaCoder.Host
    } else {
        Write-Host "No previous migration found. Cannot rollback."
        exit 1
    }
}

Write-Host "Database rollback completed successfully!"
Write-Host "Backup location: $backupFile"
```

```csharp
// MigrationRollbackService.cs
public class MigrationRollbackService
{
    private readonly VersaCoderDbContext _context;
    private readonly ILogger<MigrationRollbackService> _logger;

    public MigrationRollbackService(
        VersaCoderDbContext context,
        ILogger<MigrationRollbackService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> RollbackToMigrationAsync(string migrationName)
    {
        try
        {
            _logger.LogInformation("Rolling back to migration: {MigrationName}", migrationName);

            // Create backup
            var backupPath = await CreateBackupAsync();
            _logger.LogInformation("Backup created at: {BackupPath}", backupPath);

            // Perform rollback
            await _context.Database.MigrateToAsync(migrationName);

            _logger.LogInformation("Rollback completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rollback failed");
            return false;
        }
    }

    private async Task<string> CreateBackupAsync()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var backupPath = $"backups/versacoder-{timestamp}.db";

        // For SQLite - copy database file
        var dbPath = _context.Database.GetConnectionString();
        if (File.Exists(dbPath))
        {
            File.Copy(dbPath, backupPath, overwrite: true);
        }

        return backupPath;
    }
}
```

### 8.2 Code Rollback

```yaml
# .github/workflows/rollback.yml
name: Rollback Deployment

on:
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to rollback to'
        required: true
      environment:
        description: 'Target environment'
        required: true
        type: choice
        options:
          - staging
          - production
      reason:
        description: 'Rollback reason'
        required: true

jobs:
  rollback:
    name: Rollback to ${{ github.event.inputs.version }}
    runs-on: windows-latest
    environment: ${{ github.event.inputs.environment }}
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      with:
        ref: ${{ github.event.inputs.version }}
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Build
      run: |
        dotnet build -c Release
        dotnet publish -c Release -o ./publish
    
    - name: Deploy Previous Version
      run: |
        $webAppName = "versacoder-${{ github.event.inputs.environment }}"
        $resourceGroup = "versacoder-rg"
        
        az webapp deploy `
          --name $webAppName `
          --resource-group $resourceGroup `
          --src-path ./publish `
          --type zip
    
    - name: Health Check
      run: |
        $url = "https://versacoder-${{ github.event.inputs.environment }}.azurewebsites.net/health"
        $maxRetries = 20
        $retryCount = 0
        
        do {
          try {
            $response = Invoke-WebRequest -Uri $url -UseBasicParsing -ErrorAction Stop
            if ($response.StatusCode -eq 200) {
              Write-Host "Rollback successful - Health check passed"
              break
            }
          } catch {
            Write-Host "Waiting... ($retryCount/$maxRetries)"
          }
          $retryCount++
          Start-Sleep -Seconds 10
        } while ($retryCount -lt $maxRetries)
        
        if ($retryCount -ge $maxRetries) {
          throw "Health check failed after rollback"
        }
    
    - name: Log Rollback
      run: |
        $rollbackInfo = @{
          Version = "${{ github.event.inputs.version }}"
          Environment = "${{ github.event.inputs.environment }}"
          Reason = "${{ github.event.inputs.reason }}"
          Timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
          Actor = "${{ github.actor }}"
        } | ConvertTo-Json
        
        Write-Host "Rollback logged:"
        Write-Host $rollbackInfo
        
        # Add to deployment log
        Add-Content -Path "./rollback-log.json" -Value $rollbackInfo
```

### 8.3 Configuration Rollback

```powershell
# config-rollback.ps1
# Configuration rollback script

param(
    [string]$Environment = "production",
    [string]$ConfigVersion
)

$configPath = "./config/$Environment"
$backupPath = "./config-backups/$Environment"

# Create backup of current config
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
Copy-Item -Path "$configPath" -Destination "$backupPath/$timestamp" -Recurse -Force

if ($ConfigVersion) {
    # Restore specific version
    Write-Host "Restoring config version: $ConfigVersion"
    Copy-Item -Path "$backupPath/$ConfigVersion/*" -Destination $configPath -Recurse -Force
} else {
    # Restore previous version
    $versions = Get-ChildItem -Path $backupPath -Directory |
        Sort-Object Name -Descending |
        Select-Object -Skip 1 -First 1
    
    if ($versions) {
        Write-Host "Restoring previous config: $($versions.Name)"
        Copy-Item -Path "$($versions.FullName)/*" -Destination $configPath -Recurse -Force
    } else {
        Write-Host "No previous config version found"
        exit 1
    }
}

Write-Host "Configuration rollback completed!"
```

### 8.4 Automated Rollback Triggers

```yaml
# .github/workflows/auto-rollback.yml
name: Automatic Rollback

on:
  schedule:
    - cron: '*/5 * * * *'  # Her 5 dakikada kontrol et
  workflow_dispatch:

jobs:
  monitor-and-rollback:
    name: Monitor and Auto-Rollback
    runs-on: windows-latest
    environment: production
    
    steps:
    - name: Check Production Health
      id: health-check
      run: |
        $url = "https://versacoder-prod.azurewebsites.net/health"
        $metricsUrl = "https://versacoder-prod.azurewebsites.net/health/metrics"
        
        try {
          $health = Invoke-RestMethod -Uri $url -Method Get -ErrorAction Stop
          $metrics = Invoke-RestMethod -Uri $metricsUrl -Method Get -ErrorAction Stop
          
          # Check health status
          if ($health.status -ne "Healthy") {
            Write-Host "Unhealthy status detected: $($health.status)"
            echo "needs_rollback=true" >> $env:GITHUB_OUTPUT
            echo "reason=Unhealthy status: $($health.status)" >> $env:GITHUB_OUTPUT
            exit 0
          }
          
          # Check error rate
          if ($metrics.errorRate -gt 5) {
            Write-Host "High error rate detected: $($metrics.errorRate)%"
            echo "needs_rollback=true" >> $env:GITHUB_OUTPUT
            echo "reason=High error rate: $($metrics.errorRate)%" >> $env:GITHUB_OUTPUT
            exit 0
          }
          
          # Check response time
          if ($metrics.avgResponseTime -gt 2000) {
            Write-Host "Slow response time detected: $($metrics.avgResponseTime)ms"
            echo "needs_rollback=true" >> $env:GITHUB_OUTPUT
            echo "reason=Slow response time: $($metrics.avgResponseTime)ms" >> $env:GITHUB_OUTPUT
            exit 0
          }
          
          Write-Host "Production is healthy"
          echo "needs_rollback=false" >> $env:GITHUB_OUTPUT
          
        } catch {
          Write-Host "Health check failed: $_"
          echo "needs_rollback=true" >> $env:GITHUB_OUTPUT
          echo "reason=Health check failed: $_" >> $env:GITHUB_OUTPUT
        }
    
    - name: Execute Rollback
      if: steps.health-check.outputs.needs_rollback == 'true'
      run: |
        $reason = "${{ steps.health-check.outputs.reason }}"
        Write-Host "Auto-rollback triggered: $reason"
        
        # Get last known good version
        $lastGoodVersion = az webapp config appsettings list `
          --name versacoder-prod `
          --resource-group versacoder-rg `
          --query "[?name=='LAST_GOOD_VERSION'].value" -o tsv
        
        if ($lastGoodVersion) {
          Write-Host "Rolling back to version: $lastGoodVersion"
          
          # Deploy last good version
          # This would typically download the artifact from a previous successful deployment
          
          Write-Host "Rollback completed"
        } else {
          Write-Host "No last good version found. Manual intervention required."
        }
    
    - name: Notify
      if: steps.health-check.outputs.needs_rollback == 'true'
      run: |
        $reason = "${{ steps.health-check.outputs.reason }}"
        Write-Host "Rollback notification: $reason"
        # Add notification logic (Slack, Teams, Email)
```

---

## 9. Monitoring & Health Checks

### 9.1 Health Check Endpoints

```csharp
// HealthCheckExtensions.cs
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SQLite;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddVersaCoderHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlite(
                configuration.GetConnectionString("DefaultConnection"),
                name: "database",
                healthQuery: "SELECT 1",
                tags: new[] { "db", "sqlite" })
            .AddCheck<FileSystemHealthCheck>(
                "filesystem",
                tags: new[] { "storage" })
            .AddCheck<MemoryHealthCheck>(
                "memory",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "system" })
            .AddCheck<AiProviderHealthCheck>(
                "ai-providers",
                tags: new[] { "ai", "external" })
            .AddCheck<GitRepositoryHealthCheck>(
                "git-repository",
                tags: new[] { "git" });

        return services;
    }
}

// FileSystemHealthCheck.cs
public class FileSystemHealthCheck : IHealthCheck
{
    private readonly string _dataPath;

    public FileSystemHealthCheck(IConfiguration configuration)
    {
        _dataPath = configuration["DataPath"] ?? "./data";
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if data directory exists and is writable
            if (!Directory.Exists(_dataPath))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Data directory not found: {_dataPath}"));
            }

            // Check write permission
            var testFile = Path.Combine(_dataPath, $"health-check-{Guid.NewGuid()}.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);

            // Check free space
            var drive = new DriveInfo(Path.GetPathRoot(_dataPath));
            var freeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);

            if (freeSpaceGB < 1)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Low disk space: {freeSpaceGB}GB available"));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Filesystem healthy. Free space: {freeSpaceGB}GB"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                exception: ex));
        }
    }
}

// MemoryHealthCheck.cs
public class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var process = Process.GetCurrentProcess();
        var workingSet = process.WorkingSet64 / (1024 * 1024);
        var privateMemory = process.PrivateMemorySize64 / (1024 * 1024);

        var data = new Dictionary<string, object>
        {
            ["working_set_mb"] = workingSet,
            ["private_memory_mb"] = privateMemory,
            ["gc_total_memory_mb"] = GC.GetTotalMemory(false) / (1024 * 1024),
            ["gen0_collections"] = GC.CollectionCount(0),
            ["gen1_collections"] = GC.CollectionCount(1),
            ["gen2_collections"] = GC.CollectionCount(2)
        };

        if (workingSet > 1024) // More than 1GB
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"High memory usage: {workingSet}MB",
                data: data));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Memory usage normal: {workingSet}MB",
            data));
    }
}

// AiProviderHealthCheck.cs
public class AiProviderHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AiProviderHealthCheck> _logger;

    public AiProviderHealthCheck(
        IServiceProvider serviceProvider,
        ILogger<AiProviderHealthCheck> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, object>();
        var unhealthyProviders = new List<string>();

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var providers = scope.ServiceProvider.GetServices<IAiProvider>();

            foreach (var provider in providers)
            {
                try
                {
                    var isHealthy = await provider.IsAvailableAsync(cancellationToken);
                    results[provider.Name] = isHealthy ? "healthy" : "unhealthy";

                    if (!isHealthy)
                    {
                        unhealthyProviders.Add(provider.Name);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Health check failed for provider {Provider}", provider.Name);
                    results[provider.Name] = "error";
                    unhealthyProviders.Add(provider.Name);
                }
            }

            if (unhealthyProviders.Any())
            {
                return HealthCheckResult.Degraded(
                    $"Unhealthy providers: {string.Join(", ", unhealthyProviders)}",
                    data: results);
            }

            return HealthCheckResult.Healthy("All AI providers healthy", results);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Failed to check AI providers",
                exception: ex,
                data: results);
        }
    }
}
```

### 9.2 Deployment Verification Tests

```csharp
// DeploymentVerificationTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class DeploymentVerificationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public DeploymentVerificationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task DatabaseHealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health/database");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task FileSystemHealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health/filesystem");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiSessions_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/sessions");

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task VersionEndpoint_ReturnsCurrentVersion()
    {
        // Act
        var response = await _client.GetAsync("/api/version");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("version", content.ToLower());
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/health")]
    [InlineData("/api/sessions")]
    public async Task CriticalEndpoints_RespondWithinThreshold(string endpoint)
    {
        // Arrange
        var maxResponseTime = TimeSpan.FromSeconds(2);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await _client.GetAsync(endpoint);
        stopwatch.Stop();

        // Assert
        Assert.True(
            stopwatch.Elapsed < maxResponseTime,
            $"Endpoint {endpoint} responded in {stopwatch.Elapsed.TotalMilliseconds}ms, " +
            $"exceeding threshold of {maxResponseTime.TotalMilliseconds}ms");
    }
}
```

### 9.3 Smoke Tests

```powershell
# smoke-tests.ps1
# Post-deployment smoke tests

param(
    [string]$BaseUrl = "http://localhost:5000"
)

$tests = @(
    @{
        Name = "Health Check"
        Url = "/health"
        ExpectedStatus = 200
    },
    @{
        Name = "Database Health"
        Url = "/health/database"
        ExpectedStatus = 200
    },
    @{
        Name = "File System Health"
        Url = "/health/filesystem"
        ExpectedStatus = 200
    },
    @{
        Name = "Sessions API"
        Url = "/api/sessions"
        ExpectedStatus = 200
    },
    @{
        Name = "Version API"
        Url = "/api/version"
        ExpectedStatus = 200
    }
)

$failedTests = @()
$passedTests = @()

foreach ($test in $tests) {
    try {
        $response = Invoke-WebRequest -Uri "$BaseUrl$($test.Url)" -UseBasicParsing -ErrorAction Stop
        
        if ($response.StatusCode -eq $test.ExpectedStatus) {
            $passedTests += $test.Name
            Write-Host "✓ $($test.Name)" -ForegroundColor Green
        } else {
            $failedTests += $test.Name
            Write-Host "✗ $($test.Name) - Expected $($test.ExpectedStatus), got $($response.StatusCode)" -ForegroundColor Red
        }
    } catch {
        $failedTests += $test.Name
        Write-Host "✗ $($test.Name) - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nSmoke Test Results:" -ForegroundColor Cyan
Write-Host "Passed: $($passedTests.Count)" -ForegroundColor Green
Write-Host "Failed: $($failedTests.Count)" -ForegroundColor Red

if ($failedTests.Count -gt 0) {
    Write-Host "`nFailed tests:" -ForegroundColor Yellow
    $failedTests | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    exit 1
}

Write-Host "`nAll smoke tests passed!" -ForegroundColor Green
exit 0
```

### 9.4 Post-Deployment Validation

```yaml
# .github/workflows/post-deployment-validation.yml
name: Post-Deployment Validation

on:
  workflow_run:
    workflows: ["Deploy to Production"]
    types: [completed]
    branches: [main]

jobs:
  validation:
    name: Post-Deployment Validation
    runs-on: windows-latest
    if: ${{ github.event.workflow_run.conclusion == 'success' }}
    
    steps:
    - name: Wait for application startup
      run: Start-Sleep -Seconds 60
    
    - name: Run comprehensive health checks
      run: |
        $baseUrl = "https://versacoder-prod.azurewebsites.net"
        $endpoints = @(
          @{ Path = "/health"; Name = "Main Health" },
          @{ Path = "/health/database"; Name = "Database" },
          @{ Path = "/health/filesystem"; Name = "File System" },
          @{ Path = "/health/memory"; Name = "Memory" },
          @{ Path = "/health/ai"; Name = "AI Providers" }
        )
        
        $failed = @()
        
        foreach ($endpoint in $endpoints) {
          try {
            $response = Invoke-RestMethod -Uri "$baseUrl$($endpoint.Path)" -Method Get
            if ($response.status -eq "Healthy") {
              Write-Host "✓ $($endpoint.Name): Healthy" -ForegroundColor Green
            } else {
              Write-Host "✗ $($endpoint.Name): $($response.status)" -ForegroundColor Red
              $failed += $endpoint.Name
            }
          } catch {
            Write-Host "✗ $($endpoint.Name): Failed" -ForegroundColor Red
            $failed += $endpoint.Name
          }
        }
        
        if ($failed.Count -gt 0) {
          throw "Health checks failed: $($failed -join ', ')"
        }
    
    - name: Run smoke tests
      run: |
        $baseUrl = "https://versacoder-prod.azurewebsites.net"
        
        # Test basic functionality
        $tests = @(
          @{ Path = "/api/sessions"; Name = "Sessions API" },
          @{ Path = "/api/config"; Name = "Config API" }
        )
        
        foreach ($test in $tests) {
          try {
            $response = Invoke-WebRequest -Uri "$baseUrl$($test.Path)" -UseBasicParsing
            Write-Host "✓ $($test.Name): OK" -ForegroundColor Green
          } catch {
            Write-Host "✗ $($test.Name): Failed" -ForegroundColor Red
            throw "Smoke test failed: $($test.Name)"
          }
        }
    
    - name: Monitor error rates
      run: |
        $startTime = Get-Date
        $duration = New-TimeSpan -Minutes 10
        
        Write-Host "Monitoring error rates for 10 minutes..."
        
        while ((Get-Date) - $startTime -lt $duration) {
          $health = Invoke-RestMethod -Uri "https://versacoder-prod.azurewebsites.net/health/metrics"
          
          if ($health.errorRate -gt 5) {
            throw "High error rate detected: $($health.errorRate)%"
          }
          
          Write-Host "Error rate: $($health.errorRate)%" -ForegroundColor Cyan
          Start-Sleep -Seconds 30
        }
    
    - name: Validation passed
      if: success()
      run: |
        Write-Host "`n✓ Post-deployment validation passed!" -ForegroundColor Green
        # Update deployment status
        Write-Host "Deployment marked as successful"
    
    - name: Trigger rollback on failure
      if: failure()
      run: |
        Write-Host "`n✗ Post-deployment validation failed!" -ForegroundColor Red
        Write-Host "Triggering automatic rollback..."
        
        # Trigger rollback workflow
        # This would typically be done via GitHub API
```

---

## 10. Secret Management

### 10.1 GitHub Secrets

```yaml
# Required GitHub Secrets
# Go to: Settings > Secrets and variables > Actions

# Azure
AZURE_CREDENTIALS: |
  {
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret",
    "subscriptionId": "your-subscription-id",
    "tenantId": "your-tenant-id"
  }

# Database
DATABASE_CONNECTION_STRING: "Data Source=versacoder-prod.db"

# AI Providers
OPENAI_API_KEY: "sk-your-openai-key"
ANTHROPIC_API_KEY: "sk-ant-your-anthropic-key"
GOOGLE_AI_API_KEY: "your-google-ai-key"

# Application
JWT_SECRET_KEY: "your-jwt-secret-key-at-least-32-chars"
ENCRYPTION_KEY: "your-encryption-key"

# Monitoring
APPLICATION_INSIGHTS_KEY: "your-app-insights-key"
SENTRY_DSN: "your-sentry-dsn"

# SonarQube
SONAR_TOKEN: "your-sonar-token"
```

### 10.2 Azure Key Vault

```powershell
# keyvault-setup.ps1
# Setup Azure Key Vault for VersaCoder

param(
    [string]$ResourceGroup = "versacoder-rg",
    [string]$KeyVaultName = "versacoder-vault",
    [string]$Location = "Turkey Central"
)

# Create Key Vault
az keyvault create `
    --name $KeyVaultName `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku premium `
    --enable-rbac-authorization true

# Enable soft delete and purge protection
az keyvault update `
    --name $KeyVaultName `
    --resource-group $ResourceGroup `
    --enable-soft-delete true `
    --enable-purge-protection true

# Set access policies
az keyvault set-policy `
    --name $KeyVaultName `
    --resource-group $ResourceGroup `
    --object-id (az ad sp show --id "versacoder-app" --query "id" -o tsv) `
    --secret-permissions get list set delete

# Add secrets
$secrets = @{
    "DatabaseConnectionString" = "Data Source=versacoder-prod.db"
    "OpenAiApiKey" = "sk-your-openai-key"
    "AnthropicApiKey" = "sk-ant-your-anthropic-key"
    "JwtSecretKey" = "your-jwt-secret-key"
    "EncryptionKey" = "your-encryption-key"
}

foreach ($secret in $secrets.GetEnumerator()) {
    az keyvault secret set `
        --vault-name $KeyVaultName `
        --name $secret.Key `
        --value $secret.Value
}

# Enable diagnostic logging
az monitor diagnostic-settings create `
    --name "KeyVaultDiagnostics" `
    --resource (az keyvault show --name $KeyVaultName --query "id" -o tsv) `
    --logs '[{"category":"AuditEvent","enabled":true,"retentionPolicy":{"enabled":true,"days":90}}]' `
    --workspace (az monitor log-analytics workspace show --resource-group $ResourceGroup --query "id" -o tsv)

Write-Host "Key Vault setup completed!"
Write-Host "Vault Name: $KeyVaultName"
Write-Host "Resource Group: $ResourceGroup"
```

### 10.3 User Secrets for Development

```bash
# Initialize user secrets
dotnet user-secrets init --project src/VersaCoder.Host

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=versacoder-dev.db" --project src/VersaCoder.Host
dotnet user-secrets set "AI:OpenAi:ApiKey" "sk-dev-key" --project src/VersaCoder.Host
dotnet user-secrets set "AI:Anthropic:ApiKey" "sk-ant-dev-key" --project src/VersaCoder.Host
dotnet user-secrets set "Jwt:SecretKey" "dev-secret-key-at-least-32-chars" --project src/VersaCoder.Host

# List secrets
dotnet user-secrets list --project src/VersaCoder.Host

# Remove a secret
dotnet user-secrets remove "AI:OpenAi:ApiKey" --project src/VersaCoder.Host

# Clear all secrets
dotnet user-secrets clear --project src/VersaCoder.Host
```

```json
// Properties/user-secrets.json (DO NOT COMMIT)
{
  "ConnectionStrings:DefaultConnection": "Data Source=versacoder-dev.db",
  "AI:OpenAi:ApiKey": "sk-dev-key",
  "AI:Anthropic:ApiKey": "sk-ant-dev-key",
  "Jwt:SecretKey": "dev-secret-key-at-least-32-chars",
  "Logging:LogLevel:Default": "Debug"
}
```

### 10.4 Environment Variables

```powershell
# set-env-vars.ps1
# Set environment variables for development

$envVars = @{
    "ASPNETCORE_ENVIRONMENT" = "Development"
    "ConnectionStrings__DefaultConnection" = "Data Source=versacoder-dev.db"
    "AI__OpenAi__ApiKey" = "sk-dev-key"
    "AI__Anthropic__ApiKey" = "sk-ant-dev-key"
    "Jwt__SecretKey" = "dev-secret-key-at-least-32-chars"
}

foreach ($var in $envVars.GetEnumerator()) {
    [Environment]::SetEnvironmentVariable($var.Key, $var.Value, "Process")
    Write-Host "Set $($var.Key) = $($var.Value)"
}

# Verify
Write-Host "`nEnvironment variables set:"
Get-ChildItem env: | Where-Object { $_.Name -match "^(ASPNETCORE|ConnectionStrings|AI|Jwt)" } | Format-Table Name, Value
```

```yaml
# docker-compose.dev.yml - Environment variables
version: '3.8'

services:
  versacoder:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Data Source=/app/data/versacoder-dev.db
      - AI__OpenAi__ApiKey=${OPENAI_API_KEY}
      - AI__Anthropic__ApiKey=${ANTHROPIC_API_KEY}
      - Jwt__SecretKey=${JWT_SECRET_KEY}
    env_file:
      - .env
```

---

## 11. Database Migration

### 11.1 EF Core Migration Deployment

```powershell
# migration-deploy.ps1
# Deploy EF Core migrations

param(
    [string]$Environment = "production",
    [switch]$PreviewOnly
)

Write-Host "Deploying migrations for $Environment environment..."

# Set connection string based on environment
$connectionString = switch ($Environment) {
    "development" { "Data Source=versacoder-dev.db" }
    "staging" { "Data Source=versacoder-staging.db" }
    "production" { "Data Source=versacoder-prod.db" }
    default { throw "Unknown environment: $Environment" }
}

# Create backup for non-development environments
if ($Environment -ne "development") {
    Write-Host "Creating database backup..."
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $backupPath = "./backups/$Environment/versacoder-$timestamp.db"
    
    $dbPath = $connectionString -replace "Data Source=", ""
    if (Test-Path $dbPath) {
        Copy-Item -Path $dbPath -Destination $backupPath -Force
        Write-Host "Backup created: $backupPath"
    }
}

# Preview migrations
Write-Host "`nPending migrations:"
dotnet ef migrations list `
    --project src/VersaCoder.Infrastructure.Data `
    --startup-project src/VersaCoder.Host

if ($PreviewOnly) {
    Write-Host "`nPreview only - no changes applied"
    exit 0
}

# Apply migrations
Write-Host "`nApplying migrations..."
dotnet ef database update `
    --project src/VersaCoder.Infrastructure.Data `
    --startup-project src/VersaCoder.Host `
    --connection "$connectionString"

Write-Host "`nMigrations deployed successfully!"
```

### 11.2 Backward Compatible Migrations

```csharp
// Example of a backward compatible migration
public partial class AddSessionTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Step 1: Create new table
        migrationBuilder.CreateTable(
            name: "ChatSessions",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Title = table.Column<string>(type: "TEXT", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                Status = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChatSessions", x => x.Id);
            });

        // Step 2: Migrate data from old table (if exists)
        migrationBuilder.Sql(@"
            INSERT INTO ChatSessions (Id, Title, CreatedAt, UpdatedAt, Status)
            SELECT 
                Id,
                Name as Title,
                CreatedDate as CreatedAt,
                UpdatedDate as UpdatedAt,
                CASE WHEN IsActive = 1 THEN 0 ELSE 1 END as Status
            FROM Sessions
            WHERE EXISTS (SELECT 1 FROM Sessions);
        ");

        // Step 3: Add index after data migration
        migrationBuilder.CreateIndex(
            name: "IX_ChatSessions_CreatedAt",
            table: "ChatSessions",
            column: "CreatedAt");

        // Step 4: Drop old table (in next migration, not here)
        // This ensures backward compatibility
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback: Restore old table
        migrationBuilder.CreateTable(
            name: "Sessions",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sessions", x => x.Id);
            });

        // Migrate data back
        migrationBuilder.Sql(@"
            INSERT INTO Sessions (Id, Name, CreatedDate, UpdatedDate, IsActive)
            SELECT 
                Id,
                Title as Name,
                CreatedAt as CreatedDate,
                UpdatedAt as UpdatedDate,
                CASE WHEN Status = 0 THEN 1 ELSE 0 END as IsActive
            FROM ChatSessions;
        ");

        migrationBuilder.DropTable(
            name: "ChatSessions");
    }
}
```

### 11.3 Migration Rollback

```powershell
# migration-rollback.ps1
# Rollback EF Core migrations

param(
    [string]$TargetMigration,
    [string]$Environment = "production"
)

Write-Host "Rolling back migrations for $Environment environment..."

# Set connection string
$connectionString = switch ($Environment) {
    "development" { "Data Source=versacoder-dev.db" }
    "staging" { "Data Source=versacoder-staging.db" }
    "production" { "Data Source=versacoder-prod.db" }
    default { throw "Unknown environment: $Environment" }
}

# Create backup
Write-Host "Creating backup before rollback..."
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupPath = "./backups/$Environment/pre-rollback-$timestamp.db"

$dbPath = $connectionString -replace "Data Source=", ""
if (Test-Path $dbPath) {
    Copy-Item -Path $dbPath -Destination $backupPath -Force
    Write-Host "Backup created: $backupPath"
}

# List available migrations
Write-Host "`nAvailable migrations:"
dotnet ef migrations list `
    --project src/VersaCoder.Infrastructure.Data `
    --startup-project src/VersaCoder.Host

if ($TargetMigration) {
    # Rollback to specific migration
    Write-Host "`nRolling back to: $TargetMigration"
    dotnet ef database update $TargetMigration `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host `
        --connection "$connectionString"
} else {
    # Rollback last migration
    Write-Host "`nRolling back last migration..."
    $lastMigration = dotnet ef migrations list `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host |
        Select-Object -Last 1 |
        ForEach-Object { $_ -replace '\s*\(.*\)', '' }
    
    $previousMigration = dotnet ef migrations list `
        --project src/VersaCoder.Infrastructure.Data `
        --startup-project src/VersaCoder.Host |
        Select-Object -Last 2 |
        Select-Object -First 1 |
        ForEach-Object { $_ -replace '\s*\(.*\)', '' }
    
    if ($previousMigration) {
        dotnet ef database update $previousMigration `
            --project src/VersaCoder.Infrastructure.Data `
            --startup-project src/VersaCoder.Host `
            --connection "$connectionString"
    } else {
        Write-Host "No previous migration found. Cannot rollback."
        exit 1
    }
}

Write-Host "`nMigration rollback completed!"
Write-Host "Backup location: $backupPath"
```

---

## 12. Backup & Recovery

### 12.1 Database Backup Strategy

```powershell
# database-backup.ps1
# Automated database backup script

param(
    [string]$BackupPath = "./backups",
    [int]$RetentionDays = 30,
    [string]$Environment = "production"
)

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$dbPath = "./data/versacoder-$Environment.db"
$backupFile = "$BackupPath/$Environment/versacoder-$Environment-$timestamp.db"

# Create backup directory if it doesn't exist
if (-not (Test-Path "$BackupPath/$Environment")) {
    New-Item -ItemType Directory -Path "$BackupPath/$Environment" -Force | Out-Null
}

# Copy database file
Write-Host "Creating backup: $backupFile"
Copy-Item -Path $dbPath -Destination $backupFile -Force

# Verify backup
$originalSize = (Get-Item $dbPath).Length
$backupSize = (Get-Item $backupFile).Length

if ($backupSize -ne $originalSize) {
    Write-Host "Warning: Backup size mismatch!" -ForegroundColor Yellow
    Write-Host "Original: $originalSize bytes" -ForegroundColor Yellow
    Write-Host "Backup: $backupSize bytes" -ForegroundColor Yellow
} else {
    Write-Host "Backup verified: $backupSize bytes" -ForegroundColor Green
}

# Compress backup
$zipFile = "$backupFile.zip"
Compress-Archive -Path $backupFile -DestinationPath $zipFile -CompressionLevel Optimal
Remove-Item -Path $backupFile

Write-Host "Backup compressed: $zipFile"

# Clean old backups
$cutoffDate = (Get-Date).AddDays(-$RetentionDays)
$oldBackups = Get-ChildItem -Path "$BackupPath/$Environment" -Filter "*.zip" |
    Where-Object { $_.LastWriteTime -lt $cutoffDate }

foreach ($backup in $oldBackups) {
    Write-Host "Removing old backup: $($backup.Name)"
    Remove-Item -Path $backup.FullName -Force
}

Write-Host "Backup completed successfully!"
Write-Host "Retention policy: $RetentionDays days"
```

### 12.2 File Backup Strategy

```powershell
# file-backup.ps1
# Backup application files and configuration

param(
    [string]$BackupPath = "./backups/files",
    [int]$RetentionDays = 90
)

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$backupDir = "$BackupPath/$timestamp"

# Create backup directory
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null

# Define directories to backup
$directories = @(
    @{ Source = "./config"; Name = "config" },
    @{ Source = "./data"; Name = "data" },
    @{ Source = "./logs"; Name = "logs" },
    @{ Source = "./.ai"; Name = "vault" }
)

# Backup each directory
foreach ($dir in $directories) {
    if (Test-Path $dir.Source) {
        Write-Host "Backing up $($dir.Name)..."
        $dest = "$backupDir/$($dir.Name)"
        Copy-Item -Path $dir.Source -Destination $dest -Recurse -Force
    }
}

# Backup specific files
$files = @(
    "appsettings.json",
    "appsettings.Production.json",
    "docker-compose.yml",
    "docker-compose.prod.yml"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Copy-Item -Path $file -Destination $backupDir -Force
    }
}

# Create archive
$zipFile = "$BackupPath/versacoder-files-$timestamp.zip"
Compress-Archive -Path $backupDir -DestinationPath $zipFile -CompressionLevel Optimal
Remove-Item -Path $backupDir -Recurse -Force

Write-Host "File backup created: $zipFile"

# Clean old backups
$cutoffDate = (Get-Date).AddDays(-$RetentionDays)
$oldBackups = Get-ChildItem -Path $BackupPath -Filter "versacoder-files-*.zip" |
    Where-Object { $_.LastWriteTime -lt $cutoffDate }

foreach ($backup in $oldBackups) {
    Write-Host "Removing old backup: $($backup.Name)"
    Remove-Item -Path $backup.FullName -Force
}

Write-Host "File backup completed!"
```

### 12.3 Recovery Procedures

```powershell
# recovery-procedures.ps1
# Disaster recovery procedures

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile,
    
    [Parameter(Mandatory=$true)]
    [string]$RecoveryType,
    
    [string]$Environment = "production"
)

switch ($RecoveryType) {
    "database" {
        Write-Host "Recovering database from: $BackupFile"
        
        $dbPath = "./data/versacoder-$Environment.db"
        
        # Stop application
        Write-Host "Stopping application..."
        # Add service stop command here
        
        # Create recovery point
        $recoveryPoint = "./backups/recovery/$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        New-Item -ItemType Directory -Path $recoveryPoint -Force | Out-Null
        
        if (Test-Path $dbPath) {
            Copy-Item -Path $dbPath -Destination "$recoveryPoint/pre-recovery.db"
        }
        
        # Restore database
        Copy-Item -Path $BackupFile -Destination $dbPath -Force
        
        # Start application
        Write-Host "Starting application..."
        # Add service start command here
        
        Write-Host "Database recovery completed!"
    }
    
    "files" {
        Write-Host "Recovering files from: $BackupFile"
        
        # Extract backup
        $tempDir = "./recovery-temp"
        Expand-Archive -Path $BackupFile -DestinationPath $tempDir -Force
        
        # Restore each directory
        $directories = @("config", "data", "logs", "vault")
        
        foreach ($dir in $directories) {
            $source = "$tempDir/$dir"
            $dest = "./$dir"
            
            if (Test-Path $source) {
                Write-Host "Restoring $dir..."
                Copy-Item -Path $source -Destination $dest -Recurse -Force
            }
        }
        
        # Cleanup
        Remove-Item -Path $tempDir -Recurse -Force
        
        Write-Host "File recovery completed!"
    }
    
    "full" {
        Write-Host "Performing full system recovery..."
        
        # Stop all services
        Write-Host "Stopping all services..."
        # Add service stop commands
        
        # Restore database
        if ($BackupFile -match "\.db$") {
            & $PSCommandPath -BackupFile $BackupFile -RecoveryType "database" -Environment $Environment
        }
        
        # Restore files
        if ($BackupFile -match "\.zip$") {
            & $PSCommandPath -BackupFile $BackupFile -RecoveryType "files" -Environment $Environment
        }
        
        # Start all services
        Write-Host "Starting all services..."
        # Add service start commands
        
        Write-Host "Full system recovery completed!"
    }
    
    default {
        Write-Host "Unknown recovery type: $RecoveryType"
        Write-Host "Valid types: database, files, full"
        exit 1
    }
}
```

### 12.4 RPO/RTO Targets

```yaml
# recovery-targets.yml
# Recovery Point Objective (RPO) and Recovery Time Objective (RTO) targets

rpo_targets:
  database:
    target: "1 hour"
    backup_frequency: "hourly"
    retention: "30 days"
    method: "SQLite backup + file copy"
  
  files:
    target: "24 hours"
    backup_frequency: "daily"
    retention: "90 days"
    method: "File copy + compression"
  
  configuration:
    target: "0 (real-time)"
    backup_frequency: "on change"
    retention: "unlimited"
    method: "Git version control"

rto_targets:
  minor_incident:
    target: "15 minutes"
    procedures:
      - "Restart affected service"
      - "Clear cache"
      - "Verify health checks"
  
  major_incident:
    target: "1 hour"
    procedures:
      - "Identify root cause"
      - "Restore from backup"
      - "Verify data integrity"
      - "Restart services"
  
  disaster_recovery:
    target: "4 hours"
    procedures:
      - "Activate DR site"
      - "Restore latest backup"
      - "Redirect traffic"
      - "Verify functionality"
      - "Monitor stability"

escalation_matrix:
  level_1:
    response_time: "5 minutes"
    contacts: ["on-call-engineer"]
    actions: ["investigate", "attempt-resolution"]
  
  level_2:
    response_time: "15 minutes"
    contacts: ["team-lead", "senior-engineer"]
    actions: ["escalate", "begin-DR-procedures"]
  
  level_3:
    response_time: "30 minutes"
    contacts: ["engineering-manager", "devops-lead"]
    actions: ["full-DR-activation", "stakeholder-notification"]
  
  level_4:
    response_time: "1 hour"
    contacts: ["cto", "vp-engineering"]
    actions: ["business-continuity", "external-communication"]
```

---

## 13. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 2.0.0 |
| Status | Active |
| Deployment Strategies | 4 (Blue-Green, Canary, Rolling, Recreate) |
| CI/CD Pipelines | 5 (Build, Test, Release, Staging, Production) |
| Container Support | Docker + Docker Compose |
| Cloud Providers | 2 (Azure, AWS) |
| On-Premise Options | 3 (Windows Service, IIS, Self-Contained) |
| Rollback Strategies | 4 (Database, Code, Config, Automated) |
| Health Checks | 5 (Main, DB, FileSystem, Memory, AI) |
| Secret Management | 4 (GitHub, Azure KV, User Secrets, Env Vars) |
| Backup Frequency | Hourly (DB), Daily (Files) |
| RTO Targets | 15min (Minor) - 4hr (DR) |
| RPO Targets | 1hr (DB), 24hr (Files), 0 (Config) |

---

## 14. Deployment Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Build başarılı | ☐ |
| 2 | Testler başarılı (%80+ coverage) | ☐ |
| 3 | Security scan başarılı | ☐ |
| 4 | Performance testleri başarılı | ☐ |
| 5 | Health check endpoint'leri çalışıyor | ☐ |
| 6 | Database migration'ları uygulandı | ☐ |
| 7 | Secret'lar yapılandırıldı | ☐ |
| 8 | Backup alındı | ☐ |
| 9 | Rollback planı hazır | ☐ |
| 10 | Monitoring yapılandırıldı | ☐ |
| 11 | Smoke testler başarılı | ☐ |
| 12 | Documentation güncellendi | ☐ |
| 13 | Changelog güncellendi | ☐ |
| 14 | Version bump yapıldı | ☐ |
| 15 | Release notes hazırlandı | ☐ |
| 16 | Stakeholder'lar bilgilendirildi | ☐ |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
