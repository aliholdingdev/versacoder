namespace VersaCoder.Infrastructure.Config.Settings;

/// <summary>
/// Ana uygulama ayarları.
/// </summary>
public class AppSettings
{
    /// <summary>Uygulama adı.</summary>
    public string AppName { get; set; } = "Versa Coder";

    /// <summary>Uygulama versiyonu.</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>Çalışma modu (Development, Staging, Production).</summary>
    public string Environment { get; set; } = "Development";

    /// <summary>Veritabanı yolu.</summary>
    public string DatabasePath { get; set; } = "versacoder.db";

    /// <summary>Log dizini.</summary>
    public string LogDirectory { get; set; } = "logs";

    /// <summary>Proje dizini.</summary>
    public string ProjectsDirectory { get; set; } = "projects";

    /// <summary>Maksimum session sayısı.</summary>
    public int MaxSessions { get; set; } = 10;

    /// <summary>Maksimum token kullanımı (oturum başına).</summary>
    public int MaxTokensPerSession { get; set; } = 100_000;

    /// <summary>Oturum zaman aşımı (dakika).</summary>
    public int SessionTimeoutMinutes { get; set; } = 240;

    /// <summary>Otomatik kaydetme aralığı (saniye).</summary>
    public int AutoSaveIntervalSeconds { get; set; } = 30;
}
