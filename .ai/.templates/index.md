---
title: "Versa Coder — Template Kataloğu"
type: template
category: index
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Template Kataloğu

**Zorunlu Bağlantılar:** [[CLAUDE.md]] §16 · [[brain.md]]

---

## 1. Amaç

Versa Coder'daki tüm **şablonların listesi ve kullanım kılavuzu**. Guardrail #6'ya göre yeni dosya için template zorunludur.

---

## 2. Template Kategorileri

### 2.1 Domain Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Entity | `entity-template.cs` | Yeni entity oluştur |
| ValueObject | `valueobject-template.cs` | Yeni değer objesi |
| Enum | `enum-template.cs` | Yeni enum |
| Interface | `interface-template.cs` | Yeni arayüz |

### 2.2 Application Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Command | `command-template.cs` | Yeni CQRS command |
| Query | `query-template.cs` | Yeni CQRS query |
| Handler | `handler-template.cs` | Yeni MediatR handler |
| DTO | `dto-template.cs` | Yeni DTO |

### 2.3 Infrastructure Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Repository | `repository-template.cs` | Yeni repository |
| Configuration | `config-template.cs` | Yeni EF config |
| Provider | `provider-template.cs` | Yeni LLM provider |

### 2.4 Test Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| UnitTest | `unittest-template.cs` | Yeni unit test |
| IntegrationTest | `integrationtest-template.cs` | Yeni integration test |

### 2.5 UI Templates

| Template | Dosya | Kullanım |
|----------|-------|----------|
| Form | `form-template.cs` | Yeni DevExpress form |
| ViewModel | `viewmodel-template.cs` | Yeni MVVM ViewModel |

---

## 3. Kullanım Kuralları

| # | Kural |
|---|-------|
| 1 | Yeni dosya için template zorunlu (Guardrail #6) |
| 2 | Template'ler `.ai/.templates/` dizininde saklanır |
| 3 | Template'ler güncellenebilir |
| 4 | Yeni template eklenebilir |
| 5 | Template seçimi `index.md`'den yapılır |

---

## 4. Template Kullanım Akışı

```
İhtiyaç → index.md'den template seç → Kopyala → Personalizasyon → Kaydet
```

---

## 5. Template Detaylı Kullanım

### 5.1 Entity Template Kullanımı

```csharp
// 1. Template'i seç
// templates/csharp/entity.md

// 2. Değişkenleri doldur
var template = new EntityTemplate
{
    Namespace = "VersaCoder.Domain.Entities",
    ClassName = "Session",
    Description = "Oturum varlığı",
    Properties = new[]
    {
        new Property("Id", "Guid", "Oturum benzersiz tanımlayıcısı"),
        new Property("Name", "string", "Oturum adı"),
        new Property("State", "SessionState", "Oturum durumu"),
        new Property("CreatedAt", "DateTime", "Oluşturulma tarihi")
    },
    ConstructorParameters = "string name, Guid projectId",
    ConstructorBody = @"
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProjectId = projectId;
        State = SessionState.ACTIVE;
        CreatedAt = DateTime.UtcNow;"
};

// 3. Dosyayı oluştur
var content = templateEngine.Render(template);
await File.WriteAllTextAsync("Session.cs", content);
```

### 5.2 Repository Template Kullanımı

```csharp
// 1. Interface template'i seç
// templates/csharp/repository.md (L1 section)

// 2. Interface oluştur
var interfaceTemplate = new RepositoryInterfaceTemplate
{
    Namespace = "VersaCoder.Abstractions.Repositories",
    EntityName = "Session",
    Methods = new[]
    {
        new Method("GetByNameAsync", "string name", "Task<Session?>"),
        new Method("GetByProjectIdAsync", "Guid projectId", "Task<IReadOnlyList<Session>>")
    }
};

// 3. Implementation template'i seç
// templates/csharp/repository.md (L4 section)

// 4. Implementation oluştur
var implTemplate = new RepositoryImplTemplate
{
    Namespace = "VersaCoder.Infrastructure.Data.Repositories",
    EntityName = "Session",
    DbContext = "VersaCoderDbContext"
};
```

### 5.3 ViewModel Template Kullanımı

```csharp
// 1. Template'i seç
// templates/csharp/viewmodel.md

// 2. Değişkenleri doldur
var template = new ViewModelTemplate
{
    ViewModelNamespace = "VersaCoder.UI.ViewModels",
    ViewName = "ChatPanel",
    ServiceName = "IAgentRunner",
    Properties = new[]
    {
        new ObservableProperty("Messages", "ObservableCollection<MessageViewModel>", "new()"),
        new ObservableProperty("InputText", "string", "string.Empty"),
        new ObservableProperty("IsProcessing", "bool", "false")
    },
    Commands = new[]
    {
        new RelayCommand("SendMessageAsync", "CanSendMessage"),
        new RelayCommand("ClearChat")
    }
};

// 3. Dosyayı oluştur
var content = templateEngine.Render(template);
await File.WriteAllTextAsync("ChatPanelViewModel.cs", content);
```

---

## 6. Template Oluşturma

### 6.1 Yeni Template Oluşturma

```markdown
# Yeni Template Oluşturma Adımları

1. **Gereksinimleri Belirle**
   - Hangi katman için?
   - Hangi entity/sınıf için?
   - Hangi metodlar/özellikler gerekli?

2. **Template Dosyası Oluştur**
   - `templates/csharp/{name}.md`
   - Markdown formatında yaz
   - Code block içinde C# kodu ekle

3. **Değişkenleri Tanımla**
   - `{Namespace}`, `{ClassName}`, `{Description}`
   - `{Properties}`, `{Methods}`, `{ConstructorParameters}`

4. **Örnek Kullanım Ekle**
   - Template'in nasıl kullanılacağını göster
   - Gerçek örnekler ver

5. **Test Et**
   - Template'i farklı parametrelerle test et
   - Oluşturulan kodun çalıştığını doğrula

6. **Dokümante Et**
   - `index.md`'ye ekle
   - Kullanım talimatlarını yaz
```

### 6.2 Template Formatı

```markdown
---
title: "{Template Name}"
type: template
category: {category}
version: 1.0.0
---

# {Template Name}

## Kullanım

{Template'in ne zaman kullanılacağı}

## Template

```csharp
{Template kodu}
```

## Değişkenler

| Değişken | Tip | Açıklama |
|----------|-----|----------|
| `{Variable}` | `{Type}` | `{Description}` |

## Örnek Kullanım

```csharp
{Örnek kullanım kodu}
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
```

---

## 7. Template Validation

### 7.1 Validation Rules

```csharp
public class TemplateValidator
{
    public ValidationResult Validate(Template template)
    {
        var errors = new List<string>();

        // Required fields
        if (string.IsNullOrWhiteSpace(template.Title))
            errors.Add("Title is required");

        if (string.IsNullOrWhiteSpace(template.Content))
            errors.Add("Content is required");

        // Variable validation
        var variables = ExtractVariables(template.Content);
        foreach (var variable in variables)
        {
            if (!template.Variables.ContainsKey(variable))
                errors.Add($"Variable {variable} is not defined");
        }

        // Code block validation
        var codeBlocks = ExtractCodeBlocks(template.Content);
        foreach (var codeBlock in codeBlocks)
        {
            if (!IsValidCSharp(codeBlock))
                errors.Add("Invalid C# code in code block");
        }

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }

    private List<string> ExtractVariables(string content)
    {
        var regex = new Regex(@"\{(\w+)\}");
        return regex.Matches(content)
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();
    }

    private List<string> ExtractCodeBlocks(string content)
    {
        var regex = new Regex(@"```csharp\s*(.*?)\s*```", RegexOptions.Singleline);
        return regex.Matches(content)
            .Select(m => m.Groups[1].Value)
            .ToList();
    }

    private bool IsValidCSharp(string code)
    {
        try
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            return !tree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error);
        }
        catch
        {
            return false;
        }
    }
}
```

---

## 8. Template Listesi

### 8.1 Mevcut Template'ler

| # | Template | Kategori | Durum |
|---|----------|----------|-------|
| 1 | Entity | Domain | ✅ Aktif |
| 2 | Repository | Infrastructure | ✅ Aktif |
| 3 | ViewModel | UI | ✅ Aktif |
| 4 | Test | Testing | ✅ Aktif |
| 5 | Command | Application | 🔄 Planlanan |
| 6 | Query | Application | 🔄 Planlanan |
| 7 | Handler | Application | 🔄 Planlanan |
| 8 | DTO | Application | 🔄 Planlanan |
| 9 | Provider | Infrastructure | 🔄 Planlanan |
| 10 | Form | UI | 🔄 Planlanan |

---

## 9. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Template Categories | 5 |
| Total Templates | 10 |
| Active Templates | 4 |
| Planned Templates | 6 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
