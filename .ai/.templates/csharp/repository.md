---
title: "Repository Template"
type: template
category: csharp
version: 1.0.0
---

# Repository Template

## Kullanım

Yeni bir repository oluştururken bu template'i kullanın.

## Interface Template (L1 — Abstractions)

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using {DomainNamespace};

namespace {AbstractionsNamespace}
{
    /// <summary>
    /// {EntityName} repository arayüzü
    /// </summary>
    public interface I{EntityName}Repository
    {
        /// <summary>
        /// ID ile getir
        /// </summary>
        Task<{EntityName}?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tümünü getir
        /// </summary>
        Task<List<{EntityName}>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sayfalı getir
        /// </summary>
        Task<(List<{EntityName}> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekle
        /// </summary>
        Task<{EntityName}> AddAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Güncelle
        /// </summary>
        Task UpdateAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sil
        /// </summary>
        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kaydet
        /// </summary>
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
```

## Implementation Template (L4 — Infrastructure.Data)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using {DomainNamespace};
using {AbstractionsNamespace};

namespace {DataNamespace}
{
    /// <summary>
    /// {EntityName} repository implementasyonu
    /// </summary>
    public class {EntityName}Repository : I{EntityName}Repository
    {
        private readonly VersaCoderDbContext _context;

        public {EntityName}Repository(VersaCoderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<{EntityName}?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<{EntityName}>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<{EntityName}>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<{EntityName}>()
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<{EntityName}> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<{EntityName}>();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<{EntityName}> AddAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<{EntityName}>()
                .AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default)
        {
            _context.Set<{EntityName}>()
                .Update(entity);

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _context.Set<{EntityName}>()
                    .Remove(entity);
            }
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
