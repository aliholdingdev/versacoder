---
title: "Versa Coder — Refactoring Skill"
type: skill
category: refactoring
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Refactoring Skill

---

## 1. Amaç

Kod iyileştirme ve refactoring görevleri için **özel skill**.

---

## 2. Refactoring Türleri

| Tür | Açıklama |
|-----|----------|
| Extract Method | Büyük metodu küçült |
| Extract Class | Yeni sınıf çıkarma |
| Rename | İsimlendirme iyileştirme |
| Move Method | Metodu taşıma |
| Inline Method | Küçük metodu inline et |

---

## 3. Refactoring Kuralları

| # | Kural |
|---|-------|
| 1 | In-Place modification (dosya adı değişmez) |
| 2 | Mevcut testler çalışır durumda kalmalı |
| 3 | Committed changes revert edilebilir olmalı |
| 4 | Küçük adımlar halinde yap |
| 5 | Her adımda test çalıştır |

---

## 4. Extract Method

### 4.1 Before/After

```csharp
// ❌ Before: Long method
public void ProcessOrder(Order order)
{
    // Validate order
    if (order == null)
        throw new ArgumentNullException(nameof(order));
    
    if (order.Items == null || !order.Items.Any())
        throw new ArgumentException("Order has no items");
    
    if (order.Total <= 0)
        throw new ArgumentException("Order total must be positive");
    
    // Calculate discount
    decimal discount = 0;
    if (order.Customer.IsVIP)
    {
        discount = order.Total * 0.1m;
    }
    else if (order.Items.Count > 10)
    {
        discount = order.Total * 0.05m;
    }
    
    // Process payment
    var paymentResult = _paymentService.ProcessPayment(
        order.Customer.Id,
        order.Total - discount);
    
    if (!paymentResult.Success)
    {
        throw new PaymentException("Payment failed");
    }
    
    // Send confirmation
    _emailService.SendOrderConfirmation(order);
    
    // Update inventory
    foreach (var item in order.Items)
    {
        _inventoryService.UpdateStock(item.ProductId, -item.Quantity);
    }
}

// ✅ After: Extracted methods
public void ProcessOrder(Order order)
{
    ValidateOrder(order);
    
    var discount = CalculateDiscount(order);
    var finalAmount = order.Total - discount;
    
    ProcessPayment(order, finalAmount);
    SendConfirmation(order);
    UpdateInventory(order);
}

private void ValidateOrder(Order order)
{
    if (order == null)
        throw new ArgumentNullException(nameof(order));
    
    if (order.Items == null || !order.Items.Any())
        throw new ArgumentException("Order has no items");
    
    if (order.Total <= 0)
        throw new ArgumentException("Order total must be positive");
}

private decimal CalculateDiscount(Order order)
{
    if (order.Customer.IsVIP)
        return order.Total * 0.1m;
    
    if (order.Items.Count > 10)
        return order.Total * 0.05m;
    
    return 0;
}

private void ProcessPayment(Order order, decimal amount)
{
    var paymentResult = _paymentService.ProcessPayment(
        order.Customer.Id,
        amount);
    
    if (!paymentResult.Success)
        throw new PaymentException("Payment failed");
}

private void SendConfirmation(Order order)
{
    _emailService.SendOrderConfirmation(order);
}

private void UpdateInventory(Order order)
{
    foreach (var item in order.Items)
    {
        _inventoryService.UpdateStock(item.ProductId, -item.Quantity);
    }
}
```

---

## 5. Extract Class

### 5.1 Before/After

```csharp
// ❌ Before: God class
public class UserManager
{
    public void CreateUser(string name, string email) { }
    public void UpdateUser(Guid id, string name) { }
    public void DeleteUser(Guid id) { }
    public void SendWelcomeEmail(User user) { }
    public void SendPasswordReset(User user) { }
    public void LogUserActivity(User user, string activity) { }
    public void TrackUserSession(User user, Guid sessionId) { }
    public void ValidateUserEmail(string email) { }
    public void ValidateUserName(string name) { }
}

// ✅ After: Extracted classes
public class UserCrudService
{
    public void CreateUser(string name, string email) { }
    public void UpdateUser(Guid id, string name) { }
    public void DeleteUser(Guid id) { }
}

public class UserNotificationService
{
    public void SendWelcomeEmail(User user) { }
    public void SendPasswordReset(User user) { }
}

public class UserActivityService
{
    public void LogUserActivity(User user, string activity) { }
    public void TrackUserSession(User user, Guid sessionId) { }
}

public class UserValidationService
{
    public void ValidateUserEmail(string email) { }
    public void ValidateUserName(string name) { }
}
```

---

## 6. Rename

### 6.1 Naming Improvements

```csharp
// ❌ Before: Poor naming
public class DataProcessor
{
    public void Proc(D d) { }
    public bool Chk(string s) { }
    public List<T> GetStuff() { }
}

// ✅ After: Clear naming
public class OrderProcessor
{
    public void ProcessOrder(Order order) { }
    public bool IsValidOrder(string orderNumber) { }
    public List<Order> GetPendingOrders() { }
}
```

---

## 7. Move Method

### 7.1 Before/After

```csharp
// ❌ Before: Method in wrong class
public class Order
{
    public void SendConfirmationEmail()
    {
        // Email sending logic
    }
}

// ✅ After: Method moved to correct class
public class Order
{
    // Order only contains order-related logic
}

public class EmailService
{
    public void SendOrderConfirmation(Order order)
    {
        // Email sending logic
    }
}
```

---

## 8. Inline Method

### 8.1 Before/After

```csharp
// ❌ Before: Unnecessary method
public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }
    
    public int CalculateTotal(int price, int quantity)
    {
        return Add(price * quantity, 0);
    }
}

// ✅ After: Inlined
public class Calculator
{
    public int CalculateTotal(int price, int quantity)
    {
        return price * quantity;
    }
}
```

---

## 9. Refactoring Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Mevcut testler çalışıyor | ☐ |
| 2 | Yeni testler eklendi | ☐ |
| 3 | Kod okunabilirliği arttı | ☐ |
| 4 | Tekrarlanan kod azaldı | ☐ |
| 5 | Sorumluluk netleşti | ☐ |
| 6 | Performans etkilenmedi | ☐ |
| 7 | Dokümantasyon güncellendi | ☐ |
| 8 | Code review yapıldı | ☐ |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Refactoring Types | 5 |
| Rules | 5 |
| Before/After Examples | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
