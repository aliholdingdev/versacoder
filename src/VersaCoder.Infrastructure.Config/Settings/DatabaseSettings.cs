namespace VersaCoder.Infrastructure.Config.Settings;

/// <summary>
/// Veritabanı ayarları.
/// </summary>
public class DatabaseSettings
{
    /// <summary>SQLite dosya yolu.</summary>
    public string ConnectionString { get; set; } = "Data Source=versacoder.db";

    /// <summary>WAL modu aktif mi?</summary>
    public bool EnableWAL { get; set; } = true;

    /// <summary>Cache boyutu (KB).</summary>
    public int CacheSizeKB { get; set; } = 64000;

    /// <summary>Synchronous mod (FULL, NORMAL, OFF).</summary>
    public string Synchronous { get; set; } = "NORMAL";

    /// <summary>Foreign keys aktif mi?</summary>
    public bool EnableForeignKeys { get; set; } = true;

    /// <summary>Migration otomatik uygulansın mı?</summary>
    public bool AutoMigrate { get; set; } = true;

    /// <summary>Otomatik backup aktif mi?</summary>
    public bool EnableAutoBackup { get; set; } = true;

    /// <summary>Backup dizini.</summary>
    public string BackupDirectory { get; set; } = "backups";

    /// <summary>Maksimum backup sayısı.</summary>
    public int MaxBackupCount { get; set; } = 30;
}
