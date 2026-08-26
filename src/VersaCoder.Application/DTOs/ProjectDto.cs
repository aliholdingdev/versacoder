namespace VersaCoder.Application.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RootPath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAnalyzedAt { get; set; }
    public int FileCount { get; set; }
    public Dictionary<string, object> AnalysisResult { get; set; } = new();
}
