using VersaCoder.Abstractions.Services;

namespace VersaCoder.Application.Services;

public class TemplateService : ITemplateService
{
    private readonly string _templatesPath;

    public TemplateService(string templatesPath)
    {
        _templatesPath = templatesPath;
    }

    public Task<List<TemplateInfo>> GetAvailableTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = new List<TemplateInfo>();

        if (Directory.Exists(_templatesPath))
        {
            var files = Directory.GetFiles(_templatesPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                templates.Add(new TemplateInfo
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Category = new DirectoryInfo(Path.GetDirectoryName(file)!).Name,
                    Language = "C#"
                });
            }
        }

        return Task.FromResult(templates);
    }

    public Task<string> GetTemplateContentAsync(string templateName, CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(_templatesPath, templateName + ".cs");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template {templateName} not found");

        return Task.FromResult(File.ReadAllText(templatePath));
    }

    public Task<string> RenderTemplateAsync(string templateName, object model, CancellationToken cancellationToken = default)
    {
        return GetTemplateContentAsync(templateName, cancellationToken);
    }
}
