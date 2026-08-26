---
title: "Versa Coder — CI/CD Skill"
type: skill
category: cicd
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
---

# Versa Coder — CI/CD Skill

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

CI/CD skill'inin Versa Coder ekosisteminde nasıl kullanılacağını tanımlar. Bu skill, Continuous Integration ve Continuous Deployment süreçlerini otomatikleştirmek için GitHub Actions, Docker ve deployment pipeline'larını kapsar.

### 1.1 Kapsam

| Kapsam | Kapsam Dışı |
|--------|-------------|
| GitHub Actions workflow tanımları | Jenkins/GitLab CI |
| .NET build pipeline optimizasyonu | Ruby/Python pipeline |
| Docker multi-stage build | Kubernetes orkestrasyon |
| NuGet package publishing | npm/yarn publishing |
| Quality gate entegrasyonu | Manuel test süreçleri |
| Deployment pipeline | Hosting sağlayıcı seçimi |

### 1.2 Trigger Keywords

| Keyword Grubu | Aksiyon |
|---------------|---------|
| CI, CD, pipeline, workflow, GitHub Actions | CI/CD skill tetiklenir |
| build, compile, dotnet build | Build pipeline oluşturulur |
| test, coverage, quality | Test pipeline çalıştırılır |
| deploy, release, publish | Deployment pipeline tetiklenir |
| docker, container, image | Docker pipeline oluşturulur |
| nuget, package | Package publishing tetiklenir |

---

## 2. Skill Tanımı

### 2.1 Skill Bilgileri

| Özellik | Değer |
|---------|-------|
| Skill Adı | `cicd` |
| Kod Adı | `cicd-pipeline` |
| Açıklama | CI/CD pipeline oluşturma ve yönetme |
| Version | 1.0.0 |
| Durum | Active |
| Kategori | DevOps / Automation |

### 2.2 Kullanılabilir Araçlar

| Araç | Amaç | Kullanım Alanı |
|------|------|----------------|
| `read_file` | Dosya okuma | Mevcut workflow analizi |
| `write_file` | Dosya yazma | Workflow oluşturma |
| `edit_file` | Dosya düzenleme | Workflow güncelleme |
| `bash` | Komut çalıştırma | Build/test/deploy komutları |
| `glob` | Dosya arama | Workflow dosyalarını bulma |
| `grep` | İçerik arama | Pipeline yapılandırma araştırması |

### 2.3 Uygulanabilir Senaryolar

| # | Senaryo | Öncelik |
|---|---------|---------|
| 1 | Yeni GitHub Actions workflow oluşturma | Yüksek |
| 2 | .NET build pipeline optimizasyonu | Yüksek |
| 3 | Docker image oluşturma ve publish | Yüksek |
| 4 | NuGet package publishing | Orta |
| 5 | Quality gate entegrasyonu | Orta |
| 6 | Deployment pipeline oluşturma | Yüksek |
| 7 | Multi-environment deployment | Orta |
| 8 | Rollback mekanizması | Yüksek |

---

## 3. GitHub Actions Workflow Oluşturma

### 3.1 Temel Workflow Yapısı

```yaml
# .github/workflows/ci.yml
name: CI Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
  workflow_dispatch:

env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_FILE: 'VersaCoder.sln'

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore ${{ env.SOLUTION_FILE }}

      - name: Build
        run: dotnet build ${{ env.SOLUTION_FILE }} --no-restore --configuration Release

      - name: Test
        run: dotnet test ${{ env.SOLUTION_FILE }} --no-build --configuration Release --verbosity normal
```

### 3.2 Trigger Tanımları

#### 3.2.1 Push Trigger

```yaml
on:
  push:
    branches:
      - main
      - develop
      - 'feature/**'
      - 'bugfix/**'
    paths:
      - 'src/**'
      - 'tests/**'
      - '*.sln'
      - '*.csproj'
    paths-ignore:
      - '**.md'
      - '.ai/**'
```

#### 3.2.2 Pull Request Trigger

```yaml
on:
  pull_request:
    branches:
      - main
      - develop
    types:
      - opened
      - synchronize
      - reopened
      - ready_for_review
```

#### 3.2.3 Schedule Trigger

```yaml
on:
  schedule:
    - cron: '0 2 * * 1-5'  # Her hafta içi 02:00
```

#### 3.2.4 Workflow Dispatch Trigger

```yaml
on:
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deploy environment'
        required: true
        type: choice
        options:
          - staging
          - production
      skip_tests:
        description: 'Skip tests'
        required: false
        type: boolean
        default: false
```

### 3.3 Job ve Step Tanımları

#### 3.3.1 Çok Adımlı Job

```yaml
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
        with:
          fetch-depth: 0  # Full history for versioning

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Install dependencies
        run: dotnet restore

      - name: Build solution
        run: dotnet build --no-restore --configuration Release

      - name: Run unit tests
        run: dotnet test --no-build --configuration Release --filter "Category=Unit"

      - name: Run integration tests
        run: dotnet test --no-build --configuration Release --filter "Category=Integration"

      - name: Generate code coverage
        run: |
          dotnet test --no-build --configuration Release \
            /p:CollectCoverage=true \
            /p:CoverletOutput=../coverage/ \
            /p:CoverletOutputFormat=cobertura

      - name: Upload coverage artifact
        uses: actions/upload-artifact@v4
        with:
          name: coverage-report
          path: coverage/
```

#### 3.3.2 Job Bağımlılıkları

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.version }}
    steps:
      - name: Calculate version
        id: version
        run: echo "version=1.0.${{ github.run_number }}" >> $GITHUB_OUTPUT

  test:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Run tests
        run: dotnet test

  deploy-staging:
    needs: test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/develop'
    environment: staging
    steps:
      - name: Deploy to staging
        run: echo "Deploying version ${{ needs.build.outputs.version }}"

  deploy-production:
    needs: [build, test]
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    environment: production
    steps:
      - name: Deploy to production
        run: echo "Deploying version ${{ needs.build.outputs.version }}"
```

### 3.4 Variable ve Secret Yönetimi

#### 3.4.1 Environment Variables

```yaml
env:
  DOTNET_VERSION: '8.0.x'
  CONFIGURATION: 'Release'
  SOLUTION_FILE: 'VersaCoder.sln'
  TEST_PROJECTS: '**/*Tests.csproj'

jobs:
  build:
    env:
      BUILD_CONFIGURATION: 'Release'  # Job-level env
    steps:
      - name: Build
        env:
          COMPILE_MODE: 'optimized'  # Step-level env
        run: dotnet build --configuration ${{ env.BUILD_CONFIGURATION }}
```

#### 3.4.2 Secrets Kullanımı

```yaml
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Azure
        env:
          AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        run: |
          echo "Deploying with provided credentials"
          dotnet nuget push --api-key $NUGET_API_KEY
```

#### 3.4.3 Environments Tanımlama

```yaml
# .github/workflows/deploy.yml
jobs:
  deploy-staging:
    environment:
      name: staging
      url: https://staging.versacoder.com
    steps:
      - name: Deploy
        run: echo "Deploying to staging"

  deploy-production:
    environment:
      name: production
      url: https://versacoder.com
    steps:
      - name: Deploy
        run: echo "Deploying to production"
```

### 3.5 Matrix Stratejisi

```yaml
jobs:
  build:
    runs-on: ${{ matrix.os }}
    strategy:
      fail-fast: false
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        dotnet-version: ['8.0.x', '9.0.x']
        exclude:
          - os: macos-latest
            dotnet-version: '9.0.x'
        include:
          - os: ubuntu-latest
            dotnet-version: '8.0.x'
            coverage: true
    steps:
      - name: Setup .NET ${{ matrix.dotnet-version }}
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ matrix.dotnet-version }}

      - name: Build
        run: dotnet build

      - name: Test with coverage
        if: matrix.coverage
        run: |
          dotnet test /p:CollectCoverage=true
```

### 3.6 Caching Stratejisi

```yaml
jobs:
  build:
    steps:
      # NuGet package caching
      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      # Docker layer caching
      - name: Cache Docker layers
        uses: actions/cache@v4
        with:
          path: /tmp/.buildx-cache
          key: ${{ runner.os }}-docker-${{ hashFiles('**/Dockerfile') }}
          restore-keys: |
            ${{ runner.os }}-docker-

      # Build output caching
      - name: Cache build output
        uses: actions/cache@v4
        with:
          path: |
            **/bin
            **/obj
          key: ${{ runner.os }}-build-${{ github.sha }}
          restore-keys: |
            ${{ runner.os }}-build-
```

### 3.7 Reusable Workflows

#### 3.7.1 Reusable Workflow Tanımı

```yaml
# .github/workflows/reusable-build.yml
name: Reusable Build Pipeline

on:
  workflow_call:
    inputs:
      dotnet-version:
        required: false
        type: string
        default: '8.0.x'
      configuration:
        required: false
        type: string
        default: 'Release'
      run-tests:
        required: false
        type: boolean
        default: true
    secrets:
      NUGET_API_KEY:
        required: false
    outputs:
      version:
        description: "Build version"
        value: ${{ jobs.build.outputs.version }}

jobs:
  build:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.version }}
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ inputs.dotnet-version }}

      - name: Build
        run: dotnet build --configuration ${{ inputs.configuration }}

      - name: Version
        id: version
        run: echo "version=1.0.${{ github.run_number }}" >> $GITHUB_OUTPUT

  test:
    needs: build
    if: ${{ inputs.run-tests }}
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Test
        run: dotnet test
```

#### 3.7.2 Reusable Workflow Kullanımı

```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main]

jobs:
  build-and-test:
    uses: ./.github/workflows/reusable-build.yml
    with:
      dotnet-version: '8.0.x'
      configuration: 'Release'
      run-tests: true
    secrets:
      NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
```

### 3.8 Composite Actions

#### 3.8.1 Composite Action Tanımı

```yaml
# .github/actions/setup-net/action.yml
name: 'Setup .NET Environment'
description: 'Setup .NET SDK with caching'

inputs:
  dotnet-version:
    description: '.NET SDK version'
    required: false
    default: '8.0.x'

runs:
  using: 'composite'
  steps:
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ inputs.dotnet-version }}

    - name: Cache NuGet packages
      uses: actions/cache@v4
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-

    - name: Restore packages
      shell: bash
      run: dotnet restore
```

#### 3.8.2 Composite Action Kullanımı

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET environment
        uses: ./.github/actions/setup-net
        with:
          dotnet-version: '8.0.x'

      - name: Build
        run: dotnet build --no-restore
```

---

## 4. .NET Build Pipeline

### 4.1 dotnet restore Optimizasyonu

```yaml
steps:
  # Önce restore cache kontrolü
  - name: Cache NuGet packages
    uses: actions/cache@v4
    with:
      path: ~/.nuget/packages
      key: ${{ runner.os }}-nuget-${{ hashFiles('**/packages.lock.json') }}
      restore-keys: |
        ${{ runner.os }}-nuget-

  # Lock file 사용 varsa
  - name: Restore with lock file
    run: dotnet restore --locked-mode

  # Parallel restore
  - name: Restore packages
    run: dotnet restore --verbosity minimal

  # JetBrains packages için
  - name: Add JetBrains feed
    run: dotnet nuget add source "https://packages.jetbrains.com/api/nuget/v3/nuget-index.json" --name JetBrains
```

### 4.2 dotnet Build Konfigürasyonları

```yaml
jobs:
  build-debug:
    runs-on: ubuntu-latest
    steps:
      - name: Build Debug
        run: |
          dotnet build --configuration Debug \
            /p:TreatWarningsAsErrors=true \
            /p:WarningLevel=5 \
            /p:AnalysisLevel=latest

  build-release:
    runs-on: ubuntu-latest
    steps:
      - name: Build Release
        run: |
          dotnet build --configuration Release \
            /p:ContinuousIntegrationBuild=true \
            /p:DeterministicSourcePaths=true \
            /p:EmbedUntrackedSources=true \
            /p:PublishRepositoryUrl=true \
            /p:IncludeSymbols=true \
            /p:SymbolPackageFormat=snupkg
```

### 4.3 dotnet Test Stratejileri

```yaml
jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - name: Run unit tests
        run: |
          dotnet test \
            --filter "Category=Unit" \
            --logger "trx;LogFileName=unit-tests.trx" \
            --results-directory ./TestResults \
            /p:CollectCoverage=true \
            /p:CoverletOutput=../coverage/unit/ \
            /p:CoverletOutputFormat=cobertura

  integration-tests:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_USER: test
          POSTGRES_PASSWORD: test
          POSTGRES_DB: versacoder_test
        ports:
          - 5432:5432
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
    steps:
      - name: Run integration tests
        env:
          ConnectionStrings__DefaultConnection: "Host=localhost;Database=versacoder_test;Username=test;Password=test"
        run: |
          dotnet test \
            --filter "Category=Integration" \
            --logger "trx;LogFileName=integration-tests.trx"
```

### 4.4 Code Coverage Raporlama

```yaml
jobs:
  coverage:
    runs-on: ubuntu-latest
    steps:
      - name: Generate coverage report
        run: |
          dotnet test \
            /p:CollectCoverage=true \
            /p:CoverletOutput=../coverage/ \
            /p:CoverletOutputFormat=cobertura \
            /p:ExcludeByFile="**/obj/**,**/bin/**"

      - name: Generate HTML report
        run: |
          dotnet tool install --global dotnet-reportgenerator-globaltool
          reportgenerator \
            -reports:coverage/**/coverage.cobertura.xml \
            -targetdir:coverage/report \
            -reporttypes:Html

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v4
        with:
          files: coverage/**/coverage.cobertura.xml
          flags: unittests
          name: VersaCoder-Coverage

      - name: Coverage summary
        run: |
          echo "## Code Coverage Summary" >> $GITHUB_STEP_SUMMARY
          echo "Coverage report uploaded to Codecov" >> $GITHUB_STEP_SUMMARY
```

### 4.5 Static Analysis

```yaml
jobs:
  code-analysis:
    runs-on: ubuntu-latest
    steps:
      - name: Run Roslyn analyzers
        run: |
          dotnet build \
            /p:TreatWarningsAsErrors=true \
            /p:RunAnalyzers=true \
            /p:RunAnalyzersDuringBuild=true

      - name: Run StyleCop
        run: |
          dotnet format analyzers \
            --verify-no-changes \
            --severity info

      - name: Run security analyzers
        run: |
          dotnet list package --vulnerable --include-transitive
```

### 4.6 Package Publishing (NuGet)

```yaml
jobs:
  publish:
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && startsWith(github.ref, 'refs/tags/v')
    steps:
      - name: Build packages
        run: |
          dotnet pack \
            --configuration Release \
            --output ./nupkgs \
            /p:Version=${{ github.ref_name }}

      - name: Push to NuGet
        run: |
          dotnet nuget push ./nupkgs/*.nupkg \
            --api-key ${{ secrets.NUGET_API_KEY }} \
            --source https://api.nuget.org/v3/index.json \
            --skip-duplicate

      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: ./nupkgs/*
          generate_release_notes: true
```

---

## 5. Docker Pipeline

### 5.1 Multi-Stage Build Optimizasyonu

```dockerfile
# Dockerfile
# Stage 1: Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src
COPY *.sln .
COPY src/VersaCoder.Domain/*.csproj src/VersaCoder.Domain/
COPY src/VersaCoder.Application/*.csproj src/VersaCoder.Application/
COPY src/VersaCoder.Infrastructure.Data/*.csproj src/VersaCoder.Infrastructure.Data/
COPY src/VersaCoder.Host/*.csproj src/VersaCoder.Host/
RUN dotnet restore

# Stage 2: Build
FROM restore AS build
COPY src/ .
RUN dotnet build --no-restore --configuration Release

# Stage 3: Test
FROM build AS test
COPY tests/ .
RUN dotnet test --no-build --configuration Release --logger trx;LogFileName=test-results.trx

# Stage 4: Publish
FROM build AS publish
RUN dotnet publish src/VersaCoder.Host/*.csproj \
    --no-build \
    --configuration Release \
    --output /app/publish

# Stage 5: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "VersaCoder.Host.dll"]
```

### 5.2 Layer Caching

```yaml
jobs:
  docker-build:
    runs-on: ubuntu-latest
    steps:
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Cache Docker layers
        uses: actions/cache@v4
        with:
          path: /tmp/.buildx-cache
          key: ${{ runner.os }}-buildx-${{ github.sha }}
          restore-keys: |
            ${{ runner.os }}-buildx-

      - name: Build Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          push: false
          load: true
          tags: versacoder:latest
          cache-from: type=local,src=/tmp/.buildx-cache
          cache-to: type=local,dest=/tmp/.buildx-cache-new,mode=max
```

### 5.3 Image Scanning

```yaml
jobs:
  security-scan:
    runs-on: ubuntu-latest
    needs: docker-build
    steps:
      - name: Run Trivy vulnerability scanner
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: 'versacoder:latest'
          format: 'sarif'
          output: 'trivy-results.sarif'
          severity: 'CRITICAL,HIGH'
          exit-code: '1'

      - name: Upload Trivy scan results
        uses: github/codeql-action/upload-sarif@v3
        if: always()
        with:
          sarif_file: 'trivy-results.sarif'

      - name: Run Snyk security scan
        uses: snyk/actions/docker@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          image: versacoder:latest
          args: --severity-threshold=high
```

### 5.4 Container Registry Push

```yaml
jobs:
  push-image:
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Login to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: |
            ${{ secrets.DOCKERHUB_USERNAME }}/versacoder:${{ github.sha }}
            ${{ secrets.DOCKERHUB_USERNAME }}/versacoder:latest
            ghcr.io/${{ github.repository }}:${{ github.sha }}
            ghcr.io/${{ github.repository }}:latest
```

### 5.5 Docker Compose Test

```yaml
jobs:
  docker-compose-test:
    runs-on: ubuntu-latest
    steps:
      - name: Start services
        run: |
          docker-compose -f docker-compose.test.yml up -d --build
          sleep 30

      - name: Run health checks
        run: |
          curl -f http://localhost:8080/health
          docker-compose -f docker-compose.test.yml ps

      - name: Run tests
        run: |
          docker-compose -f docker-compose.test.yml run \
            --rm tests dotnet test

      - name: Stop services
        if: always()
        run: docker-compose -f docker-compose.test.yml down
```

---

## 6. Quality Gate Entegrasyonu

### 6.1 SonarQube/SonarCloud

```yaml
jobs:
  sonar-analysis:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: SonarCloud Scan
        uses: SonarSource/sonarcloud-github-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
        with:
          args: >
            -Dsonar.projectKey=versacoder
            -Dsonar.organization=versacoder
            -Dsonar.sources=src
            -Dsonar.tests=tests
            -Dsonar.cs.opencover.reportsPaths=coverage/**/coverage.opencover.xml
            -Dsonar.exclusions=**/obj/**,**/bin/**
```

### 6.2 Codecov/Coveralls

```yaml
jobs:
  coverage-report:
    runs-on: ubuntu-latest
    steps:
      - name: Generate coverage
        run: |
          dotnet test \
            /p:CollectCoverage=true \
            /p:CoverletOutput=../coverage/ \
            /p:CoverletOutputFormat=cobertura

      - name: Upload to Codecov
        uses: codecov/codecov-action@v4
        with:
          token: ${{ secrets.CODECOV_TOKEN }}
          files: coverage/**/coverage.cobertura.xml
          flags: unittests
          name: VersaCoder-Coverage
          fail_ci_if_error: true
          verbose: true

      - name: Coverage check
        run: |
          COVERAGE=$(cat coverage/**/coverage.cobertura.xml | grep -o 'line-rate="[0-9.]*"' | head -1 | grep -o '[0-9.]*')
          echo "Coverage: $COVERAGE"
          if (( $(echo "$COVERAGE < 0.80" | bc -l) )); then
            echo "Coverage is below 80% threshold"
            exit 1
          fi
```

### 6.3 Snyk Security Scanning

```yaml
jobs:
  security-scan:
    runs-on: ubuntu-latest
    steps:
      - name: Run Snyk to check for vulnerabilities
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=high

      - name: Run Snyk Open Source
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          command: test
          args: --all-projects --severity-threshold=high
```

### 6.4 License Compliance

```yaml
jobs:
  license-check:
    runs-on: ubuntu-latest
    steps:
      - name: Check licenses
        run: |
          dotnet list package --include-transitive --output json > packages.json
          # License check script
          python .github/scripts/check-licenses.py packages.json

      - name: Run license-eye
        uses: apache/skywalking-eyes@v0.6.0
        with:
          config: .licenserc.yaml
          mode: check
```

---

## 7. Deployment Pipeline

### 7.1 Environment Provisioning

```yaml
jobs:
  provision-infrastructure:
    runs-on: ubuntu-latest
    environment: ${{ inputs.environment }}
    steps:
      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Deploy Bicep template
        uses: azure/arm-deploy@v2
        with:
          subscriptionId: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
          resourceGroupName: versacoder-${{ inputs.environment }}
          template: ./infra/main.bicep
          parameters: ./infra/params.${{ inputs.environment }}.json
          failOnStdErr: true
```

### 7.2 Infrastructure as Code (Bicep)

```bicep
// infra/main.bicep
param location string = resourceGroup().location
param environment string

var storageAccountName = 'versacoder${environment}${uniqueString(resourceGroup().id)}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'asp-versacoder-${environment}'
  location: location
  sku: {
    name: 'B1'
  }
}

resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: 'app-versacoder-${environment}'
  location: location
  properties: {
    serverFarmId: appServicePlan.id
  }
}
```

### 7.3 Database Migration Deployment

```yaml
jobs:
  database-migration:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Install EF Core tools
        run: dotnet tool install --global dotnet-ef

      - name: Run migrations
        env:
          ConnectionStrings__DefaultConnection: ${{ secrets.DB_CONNECTION_STRING }}
        run: |
          dotnet ef database update \
            --project src/VersaCoder.Infrastructure.Data \
            --startup-project src/VersaCoder.Host

      - name: Verify migration
        env:
          ConnectionStrings__DefaultConnection: ${{ secrets.DB_CONNECTION_STRING }}
        run: |
          dotnet ef migrations list \
            --project src/VersaCoder.Infrastructure.Data \
            --startup-project src/VersaCoder.Host
```

### 7.4 Application Deployment

```yaml
jobs:
  deploy-azure:
    runs-on: ubuntu-latest
    needs: [build, test, database-migration]
    environment: ${{ inputs.environment }}
    steps:
      - name: Download build artifact
        uses: actions/download-artifact@v4
        with:
          name: publish-output

      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v3
        with:
          app-name: 'app-versacoder-${{ inputs.environment }}'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish

      - name: Warm up application
        run: |
          sleep 30
          curl -f https://app-versacoder-${{ inputs.environment }}.azurewebsites.net/health
```

### 7.5 Post-Deployment Verification

```yaml
jobs:
  smoke-test:
    runs-on: ubuntu-latest
    needs: deploy
    steps:
      - name: Health check
        run: |
          for i in {1..5}; do
            if curl -sf https://app-versacoder-${{ inputs.environment }}.azurewebsites.net/health; then
              echo "Health check passed"
              exit 0
            fi
            echo "Attempt $i failed, retrying in 10s..."
            sleep 10
          done
          echo "Health check failed after 5 attempts"
          exit 1

      - name: Functional smoke tests
        run: |
          # API endpoint tests
          curl -sf https://app-versacoder-${{ inputs.environment }}.azurewebsites.net/api/health || exit 1
          curl -sf https://app-versacoder-${{ inputs.environment }}.azurewebsites.net/api/sessions || exit 1

      - name: Performance baseline
        run: |
          RESPONSE_TIME=$(curl -o /dev/null -s -w '%{time_total}' https://app-versacoder-${{ inputs.environment }}.azurewebsites.net/health)
          echo "Response time: ${RESPONSE_TIME}s"
          if (( $(echo "$RESPONSE_TIME > 2.0" | bc -l) )); then
            echo "WARNING: Response time exceeds 2s baseline"
          fi
```

---

## 8. Monitoring & Alerting

### 8.1 Deployment Notifications

```yaml
jobs:
  notify-success:
    runs-on: ubuntu-latest
    if: success()
    needs: [deploy, smoke-test]
    steps:
      - name: Slack notification - Success
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "✅ Deployment Successful",
              "blocks": [
                {
                  "type": "section",
                  "text": {
                    "type": "mrkdwn",
                    "text": "*Deployment Successful* ✅\n*App:* VersaCoder\n*Env:* ${{ inputs.environment }}\n*Version:* ${{ needs.build.outputs.version }}\n*Branch:* ${{ github.ref_name }}"
                  }
                }
              ]
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}

  notify-failure:
    runs-on: ubuntu-latest
    if: failure()
    steps:
      - name: Slack notification - Failure
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "❌ Deployment Failed",
              "blocks": [
                {
                  "type": "section",
                  "text": {
                    "type": "mrkdwn",
                    "text": "*Deployment Failed* ❌\n*App:* VersaCoder\n*Env:* ${{ inputs.environment }}\n*Workflow:* ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
                  }
                }
              ]
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

### 8.2 Health Check Monitoring

```yaml
jobs:
  health-monitor:
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Continuous health monitoring
        run: |
          for i in {1..30}; do
            STATUS=$(curl -s -o /dev/null -w '%{http_code}' https://app-versacoder-prod.azurewebsites.net/health)
            if [ "$STATUS" -ne "200" ]; then
              echo "Health check failed at attempt $i (status: $STATUS)"
              # Trigger rollback
              echo "TRIGGER_ROLLBACK=true" >> $GITHUB_ENV
              exit 1
            fi
            echo "Health check passed (attempt $i)"
            sleep 60
          done
          echo "Monitoring completed - all checks passed"

      - name: Rollback on failure
        if: env.TRIGGER_ROLLBACK == 'true'
        run: |
          echo "Initiating rollback..."
          # Rollback logic here
```

### 8.3 Performance Baseline Comparison

```yaml
jobs:
  performance-check:
    runs-on: ubuntu-latest
    steps:
      - name: Measure baseline
        run: |
          # Response time
          RESPONSE_TIME=$(curl -o /dev/null -s -w '%{time_total}' https://app-versacoder-prod.azurewebsites.net/health)

          # Memory usage (if accessible)
          MEMORY=$(curl -s https://app-versacoder-prod.azurewebsites.net/health | jq '.memory')

          echo "response_time=$RESPONSE_TIME" >> $GITHUB_OUTPUT
          echo "memory=$MEMORY" >> $GITHUB_OUTPUT

      - name: Compare with baseline
        run: |
          BASELINE_RESPONSE=1.5
          CURRENT_RESPONSE=${{ steps.measure.outputs.response_time }}

          if (( $(echo "$CURRENT_RESPONSE > $BASELINE_RESPONSE * 1.2" | bc -l) )); then
            echo "WARNING: Response time degraded by more than 20%"
            echo "BASELINE: ${BASELINE_RESPONSE}s"
            echo "CURRENT: ${CURRENT_RESPONSE}s"
          fi
```

### 8.4 Rollback Triggers

```yaml
jobs:
  auto-rollback:
    runs-on: ubuntu-latest
    needs: [deploy, smoke-test]
    if: failure()
    steps:
      - name: Get previous deployment
        id: previous
        run: |
          PREVIOUS_SHA=$(git rev-parse HEAD~1)
          echo "sha=$PREVIOUS_SHA" >> $GITHUB_OUTPUT

      - name: Rollback deployment
        run: |
          echo "Rolling back to ${{ steps.previous.outputs.sha }}"
          # Rollback to previous version
          git checkout ${{ steps.previous.outputs.sha }}
          # Trigger redeployment
```

---

## 9. Best Practices

### 9.1 Pipeline as Code Principles

| Prensip | Açıklama | Uygulama |
|---------|----------|----------|
| Version Control | Pipeline kodları versiyon kontrolünde | `.github/workflows/` dizini |
| Review Process | Pipeline değişiklikleri review'dan geçmeli | PR zorunlu |
| Testing | Pipeline değişiklikleri test edilmeli | Dry-run modu |
| Documentation | Her workflow açıklamalı olmalı | YAML yorumları |
| Modularity | Reusable workflows ve composite actions | Modüler yapı |

### 9.2 Least Privilege for CI/CD

```yaml
# Minimum permissions
permissions:
  contents: read
  packages: write
  security-events: write

# Job-level permissions
jobs:
  build:
    permissions:
      contents: read
    steps:
      - uses: actions/checkout@v4
        with:
          persist-credentials: false

  deploy:
    permissions:
      contents: read
      deployments: write
    environment: production
```

### 9.3 Reproducible Builds

```yaml
steps:
  - name: Deterministic build
    run: |
      dotnet build \
        /p:ContinuousIntegrationBuild=true \
        /p:DeterministicSourcePaths=true \
        /p:EmbedUntrackedSources=true \
        /p:PublishRepositoryUrl=true

  - name: Lock dependencies
    run: |
      dotnet restore --locked-mode
      dotnet restore --force-evaluate
```

### 9.4 Immutable Artifacts

```yaml
jobs:
  build:
    steps:
      - name: Create artifact
        run: |
          dotnet publish --configuration Release --output ./publish

      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: versacoder-${{ github.sha }}
          path: ./publish
          retention-days: 30

      - name: Publish to registry
        run: |
          # Artifacts are immutable once created
          dotnet nuget push ./nupkgs/*.nupkg \
            --api-key ${{ secrets.NUGET_API_KEY }} \
            --source https://api.nuget.org/v3/index.json \
            --skip-duplicate
```

### 9.5 Progressive Delivery

```yaml
jobs:
  canary-deploy:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Deploy canary (10%)
        run: |
          echo "Deploying canary version..."

      - name: Monitor canary
        run: |
          sleep 300  # 5 minutes
          # Check error rates

      - name: Promote to 50%
        if: success()
        run: |
          echo "Promoting to 50% traffic..."

      - name: Full deployment
        if: success()
        run: |
          echo "Deploying to 100%..."
```

---

## 10. Troubleshooting

### 10.1 Common Build Failures

| Hata | Sebep | Çözüm |
|------|-------|-------|
| `NU1101` | Package bulunamadı | NuGet feed'i ekle |
| `CS0246` | Type tanımlanmamış | Namespace import et |
| `NETSDK1100` | SDK version uyumsuz | `global.json` güncelle |
| `MSB4025` | Solution dosyası bulunamadı | Dosya yolunu kontrol et |
| `error : The process ... failed` | Komut hatası | Komutu test et |

### 10.2 Test Failures

| Hata | Sebep | Çözüm |
|------|-------|-------|
| `Timeout` | Test süresi aşıldı | Timeout artır veya test optimize et |
| `Collection was modified` | Eşzamanlı erişim | Lock mekanizması ekle |
| `NullReferenceException` | Mock eksik | Mock'ları tamamla |
| `Database locked` | SQLite WAL modu | WAL modunu aktifleştir |
| `Connection refused` | Service çalışmıyor | Service health check ekle |

### 10.3 Deployment Failures

| Hata | Sebep | Çözüm |
|------|-------|-------|
| `401 Unauthorized` | Credential hatası | Secret'ları kontrol et |
| `403 Forbidden` | Yetki yetersiz | RBAC ayarlarını kontrol et |
| `503 Service Unavailable` | Service DOWN | Health check ekle |
| `502 Bad Gateway` | Upstream timeout | Timeout artır |
| `Deployment failed` | Resource yetersiz | Resource quota kontrol et |

### 10.4 Performance Issues

| Sorun | Belirti | Çözüm |
|-------|---------|-------|
| Yavaş build | Build > 10dk | Cache stratejisi ekle |
| Yavaş test | Test > 5dk | Parallel test çalıştır |
| Yavaş deploy | Deploy > 10dk | Incremental deployment |
| Memory leak | OOM hatası | Memory profiling |
| Disk space | Disk dolu | Cleanup step ekle |

---

## 11. Örnekler

### 11.1 Complete CI/CD Pipeline

```yaml
# .github/workflows/ci-cd.yml
name: VersaCoder CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]
  workflow_dispatch:
    inputs:
      environment:
        description: 'Deploy environment'
        required: true
        type: choice
        options:
          - staging
          - production

env:
  DOTNET_VERSION: '8.0.x'
  SOLUTION_FILE: 'VersaCoder.sln'

jobs:
  # ============================================================
  # BUILD & TEST
  # ============================================================
  build:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.version }}
      artifact-name: ${{ steps.artifact.outputs.name }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cache NuGet
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          restore-keys: ${{ runner.os }}-nuget-

      - name: Restore
        run: dotnet restore ${{ env.SOLUTION_FILE }}

      - name: Build
        run: dotnet build ${{ env.SOLUTION_FILE }} --no-restore --configuration Release

      - name: Version
        id: version
        run: echo "version=1.0.${{ github.run_number }}" >> $GITHUB_OUTPUT

      - name: Publish
        run: |
          dotnet publish src/VersaCoder.Host/*.csproj \
            --no-build --configuration Release --output ./publish

      - name: Upload artifact
        id: artifact
        uses: actions/upload-artifact@v4
        with:
          name: versacoder-${{ steps.version.outputs.version }}
          path: ./publish
          retention-days: 7

  test:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cache NuGet
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          restore-keys: ${{ runner.os }}-nuget-

      - name: Restore
        run: dotnet restore ${{ env.SOLUTION_FILE }}

      - name: Build
        run: dotnet build ${{ env.SOLUTION_FILE }} --no-restore --configuration Release

      - name: Unit Tests
        run: |
          dotnet test ${{ env.SOLUTION_FILE }} --no-build --configuration Release \
            --filter "Category=Unit" \
            /p:CollectCoverage=true \
            /p:CoverletOutput=../coverage/unit/ \
            /p:CoverletOutputFormat=cobertura

      - name: Upload Coverage
        uses: codecov/codecov-action@v4
        with:
          files: coverage/unit/**/coverage.cobertura.xml
          flags: unit

  security:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Security scan
        uses: snyk/actions/dotnet@master
        env:
          SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
        with:
          args: --severity-threshold=high

  # ============================================================
  # DEPLOY STAGING
  # ============================================================
  deploy-staging:
    needs: [build, test, security]
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    environment:
      name: staging
      url: https://staging.versacoder.com
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: versacoder-${{ needs.build.outputs.version }}

      - name: Deploy to staging
        run: echo "Deploying version ${{ needs.build.outputs.version }} to staging"

      - name: Health check
        run: |
          for i in {1..5}; do
            if curl -sf https://staging.versacoder.com/health; then
              exit 0
            fi
            sleep 10
          done
          exit 1

      - name: Notify success
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "✅ Staging deployment successful",
              "text": "Version: ${{ needs.build.outputs.version }}"
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}

  # ============================================================
  # DEPLOY PRODUCTION
  # ============================================================
  deploy-production:
    needs: [build, test, security]
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment:
      name: production
      url: https://versacoder.com
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: versacoder-${{ needs.build.outputs.version }}

      - name: Deploy to production
        run: echo "Deploying version ${{ needs.build.outputs.version }} to production"

      - name: Post-deploy verification
        run: |
          for i in {1..10}; do
            STATUS=$(curl -s -o /dev/null -w '%{http_code}' https://versacoder.com/health)
            if [ "$STATUS" -eq "200" ]; then
              echo "Health check passed"
              exit 0
            fi
            sleep 15
          done
          echo "Health check failed"
          exit 1

      - name: Notify success
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "🚀 Production deployment successful",
              "text": "Version: ${{ needs.build.outputs.version }}"
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}

  # ============================================================
  # ROLLBACK
  # ============================================================
  rollback:
    needs: deploy-production
    if: failure()
    runs-on: ubuntu-latest
    steps:
      - name: Rollback
        run: |
          echo "Rolling back production deployment..."
          # Rollback to previous version
```

### 11.2 Feature Branch Workflow

```yaml
# .github/workflows/feature.yml
name: Feature Branch CI

on:
  push:
    branches:
      - 'feature/**'
  pull_request:
    branches:
      - develop
      - main

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Build
        run: dotnet build --configuration Debug

      - name: Test
        run: dotnet test --configuration Debug

      - name: Code coverage check
        run: |
          dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
          # Check if coverage is above threshold
```

### 11.3 Release Workflow

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Version from tag
        id: version
        run: echo "version=${GITHUB_REF#refs/tags/v}" >> $GITHUB_OUTPUT

      - name: Build
        run: dotnet build --configuration Release

      - name: Test
        run: dotnet test --configuration Release

      - name: Pack
        run: dotnet pack --configuration Release --output ./nupkgs /p:Version=${{ steps.version.outputs.version }}

      - name: Push to NuGet
        run: dotnet nuget push ./nupkgs/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --skip-duplicate

      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: ./nupkgs/*
          generate_release_notes: true
          draft: false
          prerelease: false

      - name: Deploy to staging
        run: echo "Deploying ${{ steps.version.outputs.version }} to staging"
```

### 11.4 Hotfix Workflow

```yaml
# .github/workflows/hotfix.yml
name: Hotfix

on:
  push:
    branches:
      - 'hotfix/**'

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Validate hotfix
        run: |
          # Ensure it's a valid hotfix branch
          BRANCH_NAME=${GITHUB_REF#refs/heads/}
          if [[ ! "$BRANCH_NAME" =~ ^hotfix/TASK-[0-9]+- ]]; then
            echo "Invalid hotfix branch name format"
            exit 1
          fi

      - name: Build
        run: dotnet build --configuration Release

      - name: Test
        run: dotnet test --configuration Release --filter "Category=Unit"

  deploy-hotfix:
    needs: validate
    runs-on: ubuntu-latest
    steps:
      - name: Deploy hotfix
        run: echo "Deploying hotfix to production"

      - name: Merge to main
        run: |
          git checkout main
          git merge ${{ github.ref_name }}
          git push origin main

      - name: Notify
        uses: slackapi/slack-github-action@v1
        with:
          payload: |
            {
              "text": "🔴 Hotfix deployed to production"
            }
        env:
          SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

---

## 12. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Sections | 12 |
| Workflow Examples | 4 |
| Code Examples | 50+ |
| Best Practices | 5 |
| Troubleshooting Items | 20+ |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
