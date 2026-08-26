namespace VersaCoder.Abstractions.Plugins;

public interface IPluginManager
{
    Task<List<IPlugin>> GetPluginsAsync(CancellationToken cancellationToken = default);
    Task<IPlugin?> GetPluginAsync(string name, CancellationToken cancellationToken = default);
    Task LoadPluginsAsync(string pluginDirectory, CancellationToken cancellationToken = default);
    Task UnloadPluginAsync(string name, CancellationToken cancellationToken = default);
}
