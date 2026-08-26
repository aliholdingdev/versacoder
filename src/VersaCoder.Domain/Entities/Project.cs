namespace VersaCoder.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RootPath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAnalyzedAt { get; set; }
    public Dictionary<string, object> AnalysisResult { get; set; } = new();

    public List<FileEntry> Files { get; set; } = new();

    protected Project() { }

    public Project(string name, string rootPath)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        CreatedAt = DateTime.UtcNow;
        LastAnalyzedAt = DateTime.MinValue;
    }

    public void UpdateAnalysis(Dictionary<string, object> result)
    {
        AnalysisResult = result;
        LastAnalyzedAt = DateTime.UtcNow;
    }
}
