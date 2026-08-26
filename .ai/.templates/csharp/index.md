---
title: "Versa Coder — C# Templates Index"
type: template-index
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — C# Templates Index

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture-detailed]]

---

## 1. Template Kategorileri

| Kategori | Template | Amaç |
|----------|----------|------|
| **Domain** | Entity | Varlık template'i |
| **Domain** | ValueObject | Değer objesi template'i |
| **Domain** | DomainEvent | Domain event template'i |
| **Abstractions** | Interface | Arayüz template'i |
| **Abstractions** | Repository | Repository arayüzü template'i |
| **Abstractions** | Service | Servis arayüzü template'i |
| **Application** | Command | CQRS komut template'i |
| **Application** | Query | CQRS sorgu template'i |
| **Application** | Handler | Handler template'i |
| **Application** | DTO | DTO template'i |
| **Application** | Validator | Validator template'i |
| **Infrastructure** | Repository | Repository implementasyonu template'i |
| **Infrastructure** | Service | Servis implementasyonu template'i |
| **Infrastructure** | Provider | Provider template'i |
| **Infrastructure** | Tool | Tool template'i |
| **UI** | ViewModel | ViewModel template'i |
| **UI** | View | View template'i |
| **Test** | UnitTest | Unit test template'i |
| **Test** | IntegrationTest | Entegrasyon testi template'i |

---

## 2. Template Kullanımı

### 2.1 Yeni Dosya Oluşturma

```csharp
// Template ile yeni dosya oluştur
var template = templateEngine.GetTemplate("entity");
var content = templateEngine.Render(template, new
{
    Namespace = "VersaCoder.Domain.Entities",
    ClassName = "Session",
    Properties = new[]
    {
        new { Type = "SessionId", Name = "Id" },
        new { Type = "string", Name = "Name" },
        new { Type = "SessionState", Name = "State" }
    }
});

await fileSystemService.CreateFileAsync(
    "src/VersaCoder.Domain/Entities/Session.cs",
    content);
```

### 2.2 Guardrail #16 Uygulaması

```csharp
// Her yeni dosya için template zorunlu
public class TemplateEnforcer
{
    public async Task<bool> ValidateFileAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        var templateType = GetTemplateType(extension);
        
        if (templateType == null)
            return true; // Template gerekmiyor
        
        var template = _templateEngine.GetTemplate(templateType);
        var fileContent = await File.ReadAllTextAsync(filePath);
        
        // Template uyumluluğunu kontrol et
        return _templateEngine.Validate(fileContent, template);
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
