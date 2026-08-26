namespace VersaCoder.Infrastructure.Config.Settings;

/// <summary>
/// Güvenlik ayarları.
/// </summary>
public class SecuritySettings
{
    /// <summary>Şifreleme aktif mi?</summary>
    public bool EnableEncryption { get; set; } = true;

    /// <summary>Şifreleme anahtarı (vault'tan okunur).</summary>
    public string EncryptionKey { get; set; } = string.Empty;

    /// <summary>API key storage yolu.</summary>
    public string ApiKeyStoragePath { get; set; } = "credentials";

    /// <summary>Otomatik token yenileme aktif mi?</summary>
    public bool AutoRefreshTokens { get; set; } = true;

    /// <summary>Token yaşam süresi (dakika).</summary>
    public int TokenLifetimeMinutes { get; set; } = 60;

    /// <summary>Maksimum deneme sayısı.</summary>
    public int MaxLoginAttempts { get; set; } = 5;

    /// <summary>Rate limiting aktif mi?</summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>Rate limit (istek/dakika).</summary>
    public int RateLimitPerMinute { get; set; } = 60;
}
