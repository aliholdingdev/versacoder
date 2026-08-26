---
title: "Entity Template"
type: template
category: csharp
version: 1.0.0
---

# Entity Template

## Kullanım

Yeni bir domain entity oluştururken bu template'i kullanın.

## Template

```csharp
using System;
using System.Collections.Generic;

namespace {Namespace}
{
    /// <summary>
    /// {Description}
    /// </summary>
    public class {ClassName}
    {
        #region Properties

{Properties}

        #endregion

        #region Constructor

        /// <summary>
        /// EF Core için varsayılan constructor
        /// </summary>
        protected {ClassName}() { }

        /// <summary>
        /// Yeni {ClassName} oluşturur
        /// </summary>
        public {ClassName}({ConstructorParameters})
        {
{ConstructorBody}
        }

        #endregion

        #region Methods

{Methods}

        #endregion

        #region Equality

        public override bool Equals(object? obj)
        {
            if (obj is not {ClassName} other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==({ClassName}? left, {ClassName}? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=({ClassName}? left, {ClassName}? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
```

## Örnek Kullanım

```csharp
// Template ile entity oluşturma
var template = templateEngine.GetTemplate("entity");
var content = templateEngine.Render(template, new
{
    Namespace = "VersaCoder.Domain.Entities",
    ClassName = "Session",
    Description = "Oturum varlığı",
    Properties = @"        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SessionState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }",
    ConstructorParameters = "string name",
    ConstructorBody = @"        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        State = SessionState.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;",
    Methods = @"        public void UpdateName(string newName)
        {
            Name = newName ?? throw new ArgumentNullException(nameof(newName));
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            State = SessionState.COMPLETED;
            UpdatedAt = DateTime.UtcNow;
        }"
});
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
