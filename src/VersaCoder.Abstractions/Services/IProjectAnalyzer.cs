namespace VersaCoder.Abstractions.Services;

public interface IProjectAnalyzer
{
    Task<ProjectAnalysisResult> AnalyzeAsync(string rootPath, CancellationToken cancellationToken = default);
    Task<List<FileAnalysis>> AnalyzeFilesAsync(string rootPath, string pattern, CancellationToken cancellationToken = default);
}

public class ProjectAnalysisResult
{
    public string RootPath { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public List<string> Directories { get; set; } = new();
    public List<FileAnalysis> Files { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class FileAnalysis
{
    public string RelativePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public List<string> Classes { get; set; } = new();
    public List<string> Methods { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
}
