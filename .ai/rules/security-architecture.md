---
title: "Versa Coder — Güvenlik Mimarisi"
type: rules
category: security
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Güvenlik Mimarisi

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]]

---

## 1. Güvenlik İlkeleri

| # | İlke | Açıklama |
|---|------|----------|
| 1 |最小Yetki | Minimum privilege principle |
| 2 | Derinlemesine Savunma | Çoklu güvenlik katmanı |
| 3 | Güvenli Varsayılanlar | Varsayılan olarak güvenli yapılandırma |
| 4 | Açık Tasarım | Güvenlik gizli olmamalı |
| 5 | Hata Güvenliği | Hatalar bilgi sızdırmamalı |

---

## 2. API Key Yönetimi

| Kural | Açıklama |
|-------|----------|
| Hardcoded key yasak | IConfiguration + .env |
| Key rotasyonu | Periyodik key değişimi |
| Key erosion | Üretimde key erosion |
| Logging yasak | Key'ler loglanmaz |

---

## 3. Veri Güvenliği

| Veri Türü | Koruma |
|-----------|--------|
| API Key'leri | Şifrelenmiş saklama |
| Session verileri | SQLite şifreleme |
| Kullanıcı girdisi | Input validation |
| Dosya yolları | Path traversal koruması |

---

## 4. OWASP Kontrolleri

| Kontrol | Durum |
|---------|-------|
| Input Validation | ✅ FluentValidation |
| Output Encoding | ✅ Markdig sanitization |
| Authentication | 🔄 Planlanan |
| Authorization | 🔄 Planlanan |
| Error Handling | ✅ GlobalExceptionHandler |
| Logging | ✅ Serilog |

---

## 5. Şifreleme

### 5.1 AES Şifreleme

```csharp
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    public EncryptionService(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Encryption:Key"]!);
        _iv = Convert.FromBase64String(configuration["Encryption:IV"]!);
    }
    
    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }
        
        return Convert.ToBase64String(ms.ToArray());
    }
    
    public string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);
        
        return reader.ReadToEnd();
    }
}
```

### 5.2 Hashing

```csharp
public class HashService : IHashService
{
    public string HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(16);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, 100000, HashAlgorithmName.SHA256);
        
        var hash = pbkdf2.GetBytes(32);
        
        return Convert.ToBase64String(hash);
    }
    
    public bool VerifyPassword(string password, string storedHash, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, 100000, HashAlgorithmName.SHA256);
        
        var hash = pbkdf2.GetBytes(32);
        
        return Convert.ToBase64String(hash) == storedHash;
    }
    
    public string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        
        return Convert.ToBase64String(bytes);
    }
}
```

---

## 6. API Key Güvenliği

### 6.1 Secure Storage

```csharp
public class SecureApiKeyStorage : IApiKeyStorage
{
    private readonly IEncryptionService _encryption;
    private readonly ILogger<SecureApiKeyStorage> _logger;
    
    public SecureApiKeyStorage(
        IEncryptionService encryption,
        ILogger<SecureApiKeyStorage> logger)
    {
        _encryption = encryption;
        _logger = logger;
    }
    
    public async Task StoreApiKeyAsync(string provider, string apiKey)
    {
        var encrypted = _encryption.Encrypt(apiKey);
        
        // Store in secure location
        var keyPath = GetKeyPath(provider);
        var directory = Path.GetDirectoryName(keyPath);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllTextAsync(keyPath, encrypted);
        
        // Set file permissions (Windows only)
        if (OperatingSystem.IsWindows())
        {
            var fileInfo = new FileInfo(keyPath);
            var acl = fileInfo.GetAccessControl();
            acl.AddAccessRule(new FileSystemAccessRule(
                WindowsIdentity.GetCurrent().Name,
                FileSystemRights.Read,
                AccessControlType.Allow));
            fileInfo.SetAccessControl(acl);
        }
        
        _logger.LogInformation("API key stored for provider: {Provider}", provider);
    }
    
    public async Task<string> GetApiKeyAsync(string provider)
    {
        var keyPath = GetKeyPath(provider);
        
        if (!File.Exists(keyPath))
        {
            throw new KeyNotFoundException($"API key not found for provider: {provider}");
        }
        
        var encrypted = await File.ReadAllTextAsync(keyPath);
        return _encryption.Decrypt(encrypted);
    }
    
    public bool HasApiKey(string provider)
    {
        return File.Exists(GetKeyPath(provider));
    }
    
    private string GetKeyPath(string provider)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "VersaCoder", "keys", $"{provider}.key");
    }
}
```

---

## 7. Input Validation

### 7.1 FluentValidation Rules

```csharp
public class SessionValidator : AbstractValidator<CreateSessionCommand>
{
    public SessionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Session name is required")
            .MaximumLength(100).WithMessage("Session name cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9-_ ]+$").WithMessage("Session name contains invalid characters");
        
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");
    }
}

public class PromptValidator : AbstractValidator<SendPromptCommand>
{
    public PromptValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt is required")
            .MaximumLength(10000).WithMessage("Prompt cannot exceed 10000 characters")
            .Must(BeSafeContent).WithMessage("Prompt contains unsafe content");
        
        RuleFor(x => x.ModelName)
            .NotEmpty().WithMessage("Model name is required")
            .Must(BeValidModel).WithMessage("Invalid model name");
    }
    
    private bool BeSafeContent(string prompt)
    {
        // Check for injection attacks
        var blockedPatterns = new[]
        {
            @"<script[^>]*>.*?</script>",
            @"javascript:",
            @"on\w+\s*=",
            @"data:text/html"
        };
        
        return !blockedPatterns.Any(pattern => 
            Regex.IsMatch(prompt, pattern, RegexOptions.IgnoreCase));
    }
    
    private bool BeValidModel(string modelName)
    {
        var validModels = new[] { "gpt-4", "gpt-4-turbo", "gpt-4o", "claude-3", "ollama" };
        return validModels.Contains(modelName.ToLower());
    }
}
```

---

## 8. Rate Limiting

### 8.1 Rate Limiter

```csharp
public class RateLimiter : IRateLimiter
{
    private readonly Dictionary<string, RateLimitInfo> _limits;
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    
    public RateLimiter(int maxRequests, TimeSpan window)
    {
        _maxRequests = maxRequests;
        _window = window;
        _limits = new Dictionary<string, RateLimitInfo>();
    }
    
    public bool IsAllowed(string key)
    {
        var now = DateTime.UtcNow;
        
        if (!_limits.ContainsKey(key))
        {
            _limits[key] = new RateLimitInfo
            {
                Count = 1,
                WindowStart = now
            };
            return true;
        }
        
        var info = _limits[key];
        
        if (now - info.WindowStart > _window)
        {
            // Reset window
            info.Count = 1;
            info.WindowStart = now;
            return true;
        }
        
        if (info.Count >= _maxRequests)
        {
            return false;
        }
        
        info.Count++;
        return true;
    }
    
    public TimeSpan GetRetryAfter(string key)
    {
        if (!_limits.ContainsKey(key))
            return TimeSpan.Zero;
        
        var info = _limits[key];
        var elapsed = DateTime.UtcNow - info.WindowStart;
        
        return _window - elapsed;
    }
}

public class RateLimitInfo
{
    public int Count { get; set; }
    public DateTime WindowStart { get; set; }
}
```

---

## 9. Security Checklist

| # | Kontrol | Durum |
|---|---------|-------|
| 1 | Input validation | ☐ |
| 2 | SQL injection prevention | ☐ |
| 3 | XSS prevention | ☐ |
| 4 | CSRF protection | ☐ |
| 5 | API key encryption | ☐ |
| 6 | Password hashing | ☐ |
| 7 | Rate limiting | ☐ |
| 8 | Audit logging | ☐ |
| 9 | Error handling | ☐ |
| 10 | Security scanning | ☐ |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Security Principles | 5 |
| OWASP Controls | 6 |
| Encryption Algorithms | 2 (AES, SHA256) |
| Validation Rules | 10+ |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
