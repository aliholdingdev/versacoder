using Microsoft.Extensions.Configuration;
using VersaCoder.Infrastructure.Config.Settings;

namespace VersaCoder.Infrastructure.Config;

/// <summary>
/// Uygulama konfigürasyonunu yöneten servis.
/// </summary>
public class ConfigurationManager : IConfigurationManager
{
    private readonly IConfiguration _configuration;

    public ConfigurationManager(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public AppSettings GetAppSettings()
    {
        var settings = new AppSettings();
        _configuration.GetSection("App").Bind(settings);
        return settings;
    }

    public AiSettings GetAiSettings()
    {
        var settings = new AiSettings();
        _configuration.GetSection("AI").Bind(settings);
        return settings;
    }

    public DatabaseSettings GetDatabaseSettings()
    {
        var settings = new DatabaseSettings();
        _configuration.GetSection("Database").Bind(settings);
        return settings;
    }

    public UiSettings GetUiSettings()
    {
        var settings = new UiSettings();
        _configuration.GetSection("UI").Bind(settings);
        return settings;
    }

    public SecuritySettings GetSecuritySettings()
    {
        var settings = new SecuritySettings();
        _configuration.GetSection("Security").Bind(settings);
        return settings;
    }

    public T GetSection<T>(string sectionName) where T : new()
    {
        var settings = new T();
        _configuration.GetSection(sectionName).Bind(settings);
        return settings;
    }

    public string GetString(string key, string defaultValue = "")
    {
        return _configuration[key] ?? defaultValue;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        var value = _configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        var value = _configuration[key];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
}

/// <summary>
/// Konfigürasyon yöneticisi arayüzü.
/// </summary>
public interface IConfigurationManager
{
    AppSettings GetAppSettings();
    AiSettings GetAiSettings();
    DatabaseSettings GetDatabaseSettings();
    UiSettings GetUiSettings();
    SecuritySettings GetSecuritySettings();
    T GetSection<T>(string sectionName) where T : new();
    string GetString(string key, string defaultValue = "");
    int GetInt(string key, int defaultValue = 0);
    bool GetBool(string key, bool defaultValue = false);
}
