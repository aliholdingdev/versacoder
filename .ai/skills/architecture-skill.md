---
title: "Versa Coder — Architecture Skill"
type: skill
category: architecture
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Architecture Skill

---

## 1. Amaç

Mimari planlama ve tasarım görevleri için **özel skill**.

---

## 2. Kullanım Senaryoları

| Senaryo | Komut |
|---------|-------|
| Yeni modül tasarla | `/skill architecture modul-tasarla` |
| Mimari review | `/skill architecture review` |
| Layer violation kontrol | `/skill architecture layer-check` |
| Bağımlılık analizi | `/skill architecture dependency` |

---

## 3. Mimari Kontrol Listesi

| # | Kontrol |
|---|---------|
| 1 | Layer bağımlılık kuralları uygun mu? |
| 2 | SOLID prensiplerine uygun mu? |
| 3 | Domain'de iş mantığı var mı? |
| 4 | Interface Segregation uygulanmış mı? |
| 5 | Dependency Inversion doğru mu? |
| 6 | Template uyumluluğu var mı? |

---

## 4. Mimari Analiz

### 4.1 Layer Bağımlılık Kontrolü

```csharp
public class LayerDependencyAnalyzer
{
    private readonly Dictionary<string, string[]> _layerDependencies;
    
    public LayerDependencyAnalyzer()
    {
        _layerDependencies = new Dictionary<string, string[]>
        {
            ["L0"] = Array.Empty<string>(), // Domain - no dependencies
            ["L1"] = new[] { "L0" }, // Abstractions - depends on Domain
            ["L2"] = new[] { "L0", "L1" }, // Application - depends on Domain + Abstractions
            ["L3"] = new[] { "L2" }, // CrossCutting - depends on Application
            ["L4"] = new[] { "L3" }, // Infrastructure - depends on CrossCutting
            ["L5"] = new[] { "L4" }, // Protocol - depends on Infrastructure
            ["L6"] = new[] { "L5" }, // Host - depends on Protocol
            ["L7"] = new[] { "L6" } // UI - depends on Host
        };
    }
    
    public List<string> AnalyzeDependencies(string layer)
    {
        var violations = new List<string>();
        
        if (!_layerDependencies.ContainsKey(layer))
        {
            violations.Add($"Unknown layer: {layer}");
            return violations;
        }
        
        var allowedDependencies = _layerDependencies[layer];
        
        // Check for circular dependencies
        foreach (var dependency in allowedDependencies)
        {
            if (_layerDependencies.ContainsKey(dependency) &&
                _layerDependencies[dependency].Contains(layer))
            {
                violations.Add($"Circular dependency detected: {layer} -> {dependency}");
            }
        }
        
        return violations;
    }
    
    public bool ValidateLayerOrder(string sourceLayer, string targetLayer)
    {
        if (!_layerDependencies.ContainsKey(sourceLayer) ||
            !_layerDependencies.ContainsKey(targetLayer))
        {
            return false;
        }
        
        // Higher layers can depend on lower layers
        var sourceOrder = GetLayerOrder(sourceLayer);
        var targetOrder = GetLayerOrder(targetLayer);
        
        return sourceOrder > targetOrder;
    }
    
    private int GetLayerOrder(string layer)
    {
        return layer switch
        {
            "L0" => 0,
            "L1" => 1,
            "L2" => 2,
            "L3" => 3,
            "L4" => 4,
            "L5" => 5,
            "L6" => 6,
            "L7" => 7,
            _ => -1
        };
    }
}
```

### 4.2 SOLID Kontrol

```csharp
public class SOLIDAnalyzer
{
    public List<string> AnalyzeSOLID(Type type)
    {
        var violations = new List<string>();
        
        // Single Responsibility Principle
        if (HasMultipleResponsibilities(type))
        {
            violations.Add($"{type.Name} has multiple responsibilities");
        }
        
        // Open/Closed Principle
        if (IsNotOpenForExtension(type))
        {
            violations.Add($"{type.Name} is not open for extension");
        }
        
        // Liskov Substitution Principle
        if (ViolatesLiskovSubstitution(type))
        {
            violations.Add($"{type.Name} violates Liskov Substitution Principle");
        }
        
        // Interface Segregation Principle
        if (HasFatInterface(type))
        {
            violations.Add($"{type.Name} has a fat interface");
        }
        
        // Dependency Inversion Principle
        if (DependsOnConcretions(type))
        {
            violations.Add($"{type.Name} depends on concretions instead of abstractions");
        }
        
        return violations;
    }
    
    private bool HasMultipleResponsibilities(Type type)
    {
        // Check for multiple public methods that do different things
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.IsSpecialName)
            .ToList();
        
        return methods.Count > 10; // Heuristic
    }
    
    private bool IsNotOpenForExtension(Type type)
    {
        // Check if class is sealed or has no virtual/abstract members
        return type.IsSealed && !type.IsAbstract;
    }
    
    private bool ViolatesLiskovSubstitution(Type type)
    {
        // Check if derived classes properly override base class behavior
        var baseType = type.BaseType;
        if (baseType == null) return false;
        
        var baseMethods = baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => !m.IsSpecialName)
            .ToList();
        
        return baseMethods.Any(m => m.DeclaringType == baseType && !m.IsVirtual);
    }
    
    private bool HasFatInterface(Type type)
    {
        // Check if interface has too many methods
        var methods = type.GetMethods()
            .Where(m => !m.IsSpecialName)
            .ToList();
        
        return methods.Count > 7; // Interface Segregation Principle
    }
    
    private bool DependsOnConcretions(Type type)
    {
        // Check if type depends on concrete classes instead of interfaces
        var constructors = type.GetConstructors();
        
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsClass && !parameter.ParameterType.IsAbstract)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}
```

---

## 5. Mimari Rapor

### 5.1 Analiz Raporu

```csharp
public class ArchitectureReport
{
    public DateTime GeneratedAt { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<LayerReport> Layers { get; set; } = new();
    public List<string> Violations { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    
    public void GenerateReport(string projectPath)
    {
        GeneratedAt = DateTime.UtcNow;
        ProjectName = Path.GetFileName(projectPath);
        
        // Analyze layers
        var layers = new[] { "L0", "L1", "L2", "L3", "L4", "L5", "L6", "L7" };
        
        foreach (var layer in layers)
        {
            var layerReport = AnalyzeLayer(projectPath, layer);
            Layers.Add(layerReport);
        }
        
        // Generate recommendations
        GenerateRecommendations();
    }
    
    private LayerReport AnalyzeLayer(string projectPath, string layer)
    {
        var report = new LayerReport
        {
            Name = layer,
            ProjectCount = CountProjects(projectPath, layer),
            FileCount = CountFiles(projectPath, layer),
            LineCount = CountLines(projectPath, layer)
        };
        
        return report;
    }
    
    private int CountProjects(string projectPath, string layer)
    {
        var projects = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);
        return projects.Count(p => p.Contains(layer));
    }
    
    private int CountFiles(string projectPath, string layer)
    {
        var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        return files.Count(f => f.Contains(layer));
    }
    
    private int CountLines(string projectPath, string layer)
    {
        var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
        var layerFiles = files.Where(f => f.Contains(layer));
        
        return layerFiles.Sum(f => File.ReadLines(f).Count());
    }
    
    private void GenerateRecommendations()
    {
        Recommendations.Add("Consider adding more unit tests");
        Recommendations.Add("Review dependency injection configuration");
        Recommendations.Add("Check for potential memory leaks");
    }
}

public class LayerReport
{
    public string Name { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
    public int FileCount { get; set; }
    public int LineCount { get; set; }
}
```

---

## 6. Mimari Kararlar

### 6.1 Decision Record Template

```markdown
# ADR-001: Use Clean Architecture

## Status
Accepted

## Context
We need to decide on the architecture for the VersaCoder project.

## Decision
We will use Clean Architecture with the following layers:
- L0: Domain
- L1: Abstractions
- L2: Application
- L3: CrossCutting
- L4: Infrastructure
- L5: Protocol
- L6: Host
- L7: UI

## Consequences
### Positive
- Clear separation of concerns
- Testability
- Maintainability

### Negative
- More complex project structure
- Learning curve for new developers
```

---

## 7. Mimari Testler

### 7.1 Architecture Unit Tests

```csharp
public class ArchitectureTests
{
    private readonly Assembly _domainAssembly;
    private readonly Assembly _applicationAssembly;
    
    public ArchitectureTests()
    {
        _domainAssembly = typeof(VersaCoder.Domain.Session).Assembly;
        _applicationAssembly = typeof(VersaCoder.Application.CreateSessionCommand).Assembly;
    }
    
    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var domainTypes = _domainAssembly.GetTypes();
        var applicationTypes = _applicationAssembly.GetTypes();
        
        foreach (var domainType in domainTypes)
        {
            var references = domainType.GetReferencedAssemblies();
            
            foreach (var reference in references)
            {
                var isApplication = applicationTypes.Any(t => 
                    t.Assembly.GetName().Name == reference.Name);
                
                Assert.False(isApplication, 
                    $"Domain type {domainType.Name} depends on Application");
            }
        }
    }
    
    [Fact]
    public void Application_Should_Only_Depend_On_Domain_And_Abstractions()
    {
        var applicationTypes = _applicationAssembly.GetTypes();
        
        foreach (var applicationType in applicationTypes)
        {
            var references = applicationType.GetReferencedAssemblies();
            
            foreach (var reference in references)
            {
                var isAllowed = reference.Name == "VersaCoder.Domain" ||
                               reference.Name == "VersaCoder.Abstractions" ||
                               reference.Name.StartsWith("System") ||
                               reference.Name.StartsWith("Microsoft");
                
                Assert.True(isAllowed, 
                    $"Application type {applicationType.Name} depends on {reference.Name}");
            }
        }
    }
}
```

---

## 8. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Analysis Tools | 2 (LayerDependency, SOLID) |
| Report Types | 1 (Architecture) |
| Test Types | 2 (Architecture tests) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
