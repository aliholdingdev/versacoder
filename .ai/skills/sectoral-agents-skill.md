---
title: "Versa Coder — Sektörel Agent'lar Skill"
type: skill
category: sectoral-agents
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
---

# Versa Coder — Sektörel Agent'lar Skill

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[ROLE.md]] · [[keys.md]]

---

## 1. Amaç

Versa Coder ekosistemindeki **60+ sektörel agent'ın** nasıl kullanılacağını, koordine edileceğini ve sektör bazlı görevlerin nasıl dağıtılacağını tanımlar.

---

## 2. Skill Tanımı

| Özellik | Değer |
|---------|-------|
| Skill Adı | `sectoral-agents` |
| Versiyon | 1.0.0 |
| Sektör Sayısı | 60+ |
| Kullanım Alanları | Sektörel kod üretimi, analiz, dokümantasyon |

---

## 3. Sektörel Agent Seçim Akışı

```
Kullanıcı İsteği (Sektörel bağlam)
    ↓
[1. Sektör Tespiti] → Keyword analizi, bağlam analizi
    ↓
[2. Agent Eşleme] → Sektörel agent listesinden eşleme
    ↓
[3. Uzmanlık Kontrolü] → Agent'ın sektör uzmanlık seviyesi
    ↓
[4. Atama] → Doğru sektörel agent'ı ata
    ↓
[5. Koordinasyon] → Gerekirse çapraz sektörel handover
```

### 3.1 Sektör Tespit Algoritması

```csharp
public class SectorDetector
{
    private readonly Dictionary<string, List<string>> _sectorKeywords = new()
    {
        ["otomotiv"] = ["araç", "motor", "ECU", "CAN bus", "OBD", "emisyon", "fren", "ABS", "ESP"],
        ["saglik"] = ["hasta", "tedavi", "ilaç", "doktor", "hastane", "EHR", "HIPAA", "diyagnoz"],
        ["finans"] = ["banka", "kredi", "faiz", "borsa", "hisse", "portföy", "risk", "foreks"],
        ["siber"] = ["güvenlik", "saldırı", "penetrasyon", "zafiyet", "OWASP", "firewall", "IDS"],
        ["iot"] = ["sensör", "MQTT", "firmware", "ESP32", "Arduino", "GPIO", "gömülü"],
        ["oyun"] = ["oyuncu", "seviye", "fizik motoru", "render", "GPU", "shader", "FPS"],
        ["enerji"] = ["güneş", "rüzgar", "elektrik", "şebeke", " SCADA", "PLC", "power"],
        ["lojistik"] = ["depo", "sevkiyat", "rota", "envanter", "tedarik", "kargo", "FIFO"],
        ["tarim"] = ["tarla", "üretim", "sulama", " gübre", "hasat", "traktör", "GIS"],
        ["insaat"] = ["yapı", "beton", "proje", "çelik", "BIM", "immar", "ruhsat"],
        ["hukuk"] = ["dava", "mahkeme", "avukat", "sözleşme", "kanun", "yönetmelik", "ICMA"],
        ["egitim"] = ["öğrenci", "ders", "müfredat", "LMS", "sınav", "not", " akreditasyon"],
        ["medya"] = ["video", "ses", "yayın", "editör", "içerik", "akış", "medya"],
        ["turizm"] = ["otel", "konaklama", "tur", "rezervasyon", "bilet", "destinasyon"],
        ["perakende"] = ["mağaza", "ürün", "stok", "satış", "POS", "kampanya", "CRM"],
        ["gayrimenkul"] = ["konut", "arsa", "kira", "emlak", "tapu", "kat mülkiyeti"],
        ["sigorta"] = ["poliçe", "prim", "hasar", "risk", "sigortalı", "tazminat", "actuarial"],
        ["bankacilik"] = ["hesap", "havale", "EFT", "kredi", "mevduat", "IBAN", "SWIFT"],
        ["kripto"] = ["Bitcoin", "Ethereum", "blockchain", "token", "DEX", "DeFi", "staking"],
        ["metaverse"] = ["sanal dünya", "avatar", "3D", "VR", "AR", "sandbox", "weride"],
        ["robotik"] = ["robot", "kol", "hareket", "planning", "sensör", "motor", "servo"],
        ["uzay"] = ["roket", "uydu", "yörünge", "payload", "ISS", "telemetry", "Ground Station"],
        ["veri_bilimi"] = ["model", "eğitim", "veri seti", "accuracy", "loss", "ML", "AI", "pipeline"],
        ["devops"] = ["CI/CD", "Docker", "Kubernetes", "pipeline", "deploy", "monitor", "SRE"],
        ["embedded"] = ["mikrodenetleyici", "register", "interrupt", "DMA", "Timer", "UART", "SPI"],
    };

    public string DetectSector(string userPrompt)
    {
        var normalized = userPrompt.ToLowerInvariant();
        var scores = new Dictionary<string, int>();

        foreach (var (sector, keywords) in _sectorKeywords)
        {
            scores[sector] = keywords.Count(kw => normalized.Contains(kw.ToLowerInvariant()));
        }

        return scores.OrderByDescending(s => s.Value).First().Key;
    }
}
```

---

## 4. Sektörel Agent Kullanım Kalıpları

### 4.1 Tek Sektörel Agent

```
Kullanıcı: "Otomotiv ECU yazılımı için CAN bus haberleşme modülü yaz"
    ↓
Sektör Tespiti: otomotiv
Agent: otomotiv-agent
    ↓
Görev: CAN bus haberleşme modülü
Katman: L2 (Application) + L4 (Infrastructure)
```

### 4.2 Çoklu Sektörel Agent (Cross-Sector)

```
Kullanıcı: "IoT sensörlerinden toplanan sağlık verilerini analiz eden bir sistem tasarla"
    ↓
Sektörler: iot + saglik
Birincil Agent: iot-agent
İkincil Agent: saglik-agent
    ↓
Koordinasyon: IoT veri toplama → Sağlık analiz
```

### 4.3 Sektörel + Temel Agent Koordinasyonu

```
Kullanıcı: "Finansal risk analizi yapan bir API oluştur ve testlerini yaz"
    ↓
Sektör: finans
Birincil Agent: finans-agent (API tasarımı)
İkincil Agent: build-agent (kod yazma)
Üçüncül Agent: build-agent (test yazma)
    ↓
Sıralı Zincir: Plan → Kod → Test
```

---

## 5. Sektörel Agent Örnekleri

### 5.1 Sağlık Sektörü

| Özellik | Değer |
|---------|-------|
| Agent Adı | `healthcare` |
| Uzmanlık | EHR, HL7 FHIR, DICOM, HIPAA compliance |
| Araçlar | Read, Write, Edit, Glob, Grep |
| Kullanım | Tıbbi yazılım, hasta yönetimi, görüntü işleme |

```csharp
// Healthcare agent kullanım örneği
var healthcareAgent = new SectoralAgent
{
    Role = "healthcare",
    Expertise = new[] { "HL7 FHIR", "DICOM", "HIPAA", "EHR Integration" },
    SystemPrompt = """
        Sen sağlık sektöründe uzman bir yazılım geliştiricisisin.
        HL7 FHIR standardına uygun API'ler tasarlar ve geliştirirsin.
        HIPAA compliance kurallarına uyarsın.
        DICOM protokolünü bilirsin.
        Tıbbi verilerin güvenliği ve gizliliği konusunda hassassın.
        """,
    AvailableTools = new[] { "read", "write", "edit", "glob", "grep" }
};
```

### 5.2 Finans Sektörü

| Özellik | Değer |
|---------|-------|
| Agent Adı | `finance` |
| Uzmanlık | Risk analizi, portföy yönetimi, borsa, banking |
| Araçlar | Read, Write, Edit, Glob, Grep, Bash |
| Kullanım | Finansal yazılım, trading, risk yönetimi |

### 5.3 IoT Sektörü

| Özellik | Değer |
|---------|-------|
| Agent Adı | `iot` |
| Uzmanlık | MQTT, sensör, firmware, gömülü sistem |
| Araçlar | Read, Write, Edit, Bash |
| Kullanım | IoT cihaz yazılımı, veri toplama, edge computing |

### 5.4 Oyun Sektörü

| Özellik | Değer |
|---------|-------|
| Agent Adı | `gaming` |
| Uzmanlık | Unity, Unreal, fizik motoru, rendering |
| Araçlar | Read, Write, Edit, Glob, Grep |
| Kullanım | Oyun geliştirme, fizik simülasyonu, rendering |

---

## 6. Sektörel Enkripsyon & Compliance

### 6.1 Sektörel Compliance Kuralları

| Sektör | Compliance | Gerekli Standard |
|--------|------------|------------------|
| Sağlık | HIPAA, GDPR | EHR şifreleme, audit trail |
| Finans | PCI-DSS, SOX | Tokenization, access control |
| Kamu | KVKK, ISO 27001 | Veri koruma, denetim |
| E-ticaret | PCI-DSS | Kart verisi koruma |
| Eğitim | FERPA | Öğrenci verisi gizliliği |
| Sigorta | GDPR, KVKK | Kişisel veri koruma |

### 6.2 Sektörel Veri Sınıflandırması

```csharp
public static class DataClassification
{
    // Sağlık verileri
    public const string PHI = "Protected Health Information"; // HIPAA
    public const string EHR = "Electronic Health Record";

    // Finans verileri
    public const string PCI = "Payment Card Industry"; // PCI-DSS
    public const string PII = "Personally Identifiable Information";

    // Genel
    public const string CONFIDENTIAL = "Confidential";
    public const string INTERNAL = "Internal";
    public const string PUBLIC = "Public";

    public static Dictionary<string, string> GetClassificationRules(string sector) => sector switch
    {
        "healthcare" => new() { ["data"] = PHI, ["encryption"] = "AES-256", ["retention"] = "7 yıl" },
        "finance" => new() { ["data"] = PCI, ["encryption"] = "AES-256", ["retention"] = "10 yıl" },
        _ => new() { ["data"] = INTERNAL, ["encryption"] = "AES-128", ["retention"] = "3 yıl" }
    };
}
```

---

## 7. Sektörel Test Stratejileri

### 7.1 Sektörel Test Gereksinimleri

| Sektör | Test Türü | Hedef Kapsama |
|--------|-----------|---------------|
| Sağlık | HIMSS test, FHIR conformance | %95 |
| Finans | PCI-DSS audit, pen test | %95 |
| IoT | Hardware-in-the-loop, stress | %90 |
| Oyun | Performance, compatibility | %85 |
| E-ticaret | Load test, security scan | %90 |

### 7.2 Sektörel Test Senaryoları

```csharp
// Sağlık sektörü testi
[Fact]
public void FhirPatient_ShouldSerializeCorrectly()
{
    var patient = new FhirPatient
    {
        Id = "patient-123",
        Name = "Ahmet Yılmaz",
        BirthDate = "1990-01-15",
        Gender = Gender.Male
    };

    var json = JsonSerializer.Serialize(patient);
    Assert.Contains("\"resourceType\":\"Patient\"", json);
    Assert.Contains("\"id\":\"patient-123\"", json);
}

// Finans sektörü testi
[Fact]
public void RiskCalculator_ShouldCalculateVaR()
{
    var calculator = new RiskCalculator();
    var portfolio = CreateTestPortfolio();

    var var = calculator.CalculateValueAtRisk(portfolio, confidenceLevel: 0.95);

    Assert.True(var > 0);
    Assert.True(var < portfolio.TotalValue);
}
```

---

## 8. Sektörel Monitoring Metrikleri

### 8.1 Sektörel Dashboard Panelleri

| Panel | Metrik | Sektör |
|-------|--------|--------|
| Health Data Quality | Geçerli hasta kaydı oranı | Sağlık |
| Risk Exposure | Toplam risk maruziyeti | Finans |
| Device Connectivity | Bağlı IoT cihaz sayısı | IoT |
| Game Performance | Ortalama FPS | Oyun |
| Transaction Volume | İşlem hacmi | E-ticaret |

### 8.2 Sektörel Alerting

```yaml
# Sağlık sectoru alert
- alert: LowDataQuality
  expr: healthcare_data_quality_score < 0.9
  for: 30m
  labels:
    severity: warning
    sector: healthcare
  annotations:
    summary: "Düşük veri kalitesi"
    description: "Sağlık verisi kalite skoru %90'ın altında"

# Finans sectoru alert
- alert: HighRiskExposure
  expr: finance_risk_exposure > 1000000
  for: 15m
  labels:
    severity: critical
    sector: finance
  annotations:
    summary: "Yüksek risk maruziyeti"
    description: "Risk maruziyeti 1M limitini aşıyor"
```

---

## 9. Sektörel Entegrasyon Kalıpları

### 9.1 Sektörel API Entegrasyonu

| Sektör | Harici API | Entegrasyon |
|--------|-----------|-------------|
| Sağlık | HL7 FHIR Server | REST API |
| Finans | Borsa API (BIST) | WebSocket |
| IoT | MQTT Broker | MQTT Protocol |
| E-ticaret | Payment Gateway | REST API |
| Lojistik | Kargo API | REST API |

### 9.2 Sektörel Veri Formatları

| Sektör | Format | Kullanım |
|--------|--------|---------|
| Sağlık | HL7 FHIR JSON/XML | hasta kayıtları |
| Finans | FIX Protocol | işlemler |
| IoT | MQTT/JSON | sensör verileri |
| E-ticaret | EDI/XML | siparişler |
| Lojistik | GS1 XML | sevkiyat |

---

## 10. Sektörel Development Guide

### 10.1 Yeni Sektörel Agent Oluşturma

```csharp
// 1. Sektör tanımı
public class NewSectorDefinition
{
    public string Name = "yeni_sektor";
    public string[] Keywords = ["anahtar1", "anahtar2"];
    public string[] Expertise = ["uzmanlık1", "uzmanlık2"];
    public string SystemPrompt = "Sen ... alanında uzman bir yazılım geliştiricisisin.";
    public string[] Tools = ["read", "write", "edit"];
    public string[] ComplianceRules = ["standard1", "standard2"];
}

// 2. Agent profil dosyası
// .ai/.agents/new-sector-agent.md
```

### 10.2 Sektörel Agent Test Etme

```csharp
// 1. Unit test
[Fact]
public void NewSectorAgent_ShouldDetectSector()
{
    var detector = new SectorDetector();
    var result = detector.DetectSector("yeni_sektor ile ilgili bir soru");
    Assert.Equal("yeni_sektor", result);
}

// 2. Integration test
[Fact]
public async Task NewSectorAgent_ShouldExecuteTask()
{
    var agent = CreateSectoralAgent("yeni_sektor");
    var result = await agent.ExecuteAsync("Test görevi");
    Assert.True(result.IsSuccess);
}
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Sektör Sayısı | 60+ |
| Compliance Standards | 6 |
| Test Coverage Target | %90+ |
| Monitoring Metrics | 5 |
| Data Classifications | 6 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
