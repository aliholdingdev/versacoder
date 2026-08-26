namespace VersaCoder.Abstractions.Services;

public interface ITemplateService
{
    Task<string> RenderTemplateAsync(string templateName, object model, CancellationToken cancellationToken = default);
    Task<List<TemplateInfo>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default);
    Task<string> GetTemplateContentAsync(string templateName, CancellationToken cancellationToken = default);
}

public class TemplateInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
