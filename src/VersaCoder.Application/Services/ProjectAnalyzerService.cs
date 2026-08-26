using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;

namespace VersaCoder.Application.Services;

public class ProjectAnalyzerService : IProjectAnalyzer
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFileRepository _fileRepository;

    public ProjectAnalyzerService(
        IProjectRepository projectRepository,
        IFileRepository fileRepository)
    {
        _projectRepository = projectRepository;
        _fileRepository = fileRepository;
    }

    public async Task<ProjectAnalysisResult> AnalyzeAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        var result = new ProjectAnalysisResult
        {
            RootPath = rootPath,
            ProjectName = Path.GetFileName(rootPath)
        };

        var directories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
            .Select(d => Path.GetRelativePath(rootPath, d))
            .ToList();
        result.Directories = directories;

        var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(rootPath, file);
            result.Files.Add(new FileAnalysis
            {
                RelativePath = relativePath,
                FileName = Path.GetFileName(file),
                Extension = Path.GetExtension(file),
                Size = new FileInfo(file).Length
            });
        }

        return result;
    }

    public Task<List<FileAnalysis>> AnalyzeFilesAsync(string rootPath, string pattern, CancellationToken cancellationToken = default)
    {
        var results = new List<FileAnalysis>();
        var files = Directory.GetFiles(rootPath, pattern, SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(rootPath, file);
            results.Add(new FileAnalysis
            {
                RelativePath = relativePath,
                FileName = Path.GetFileName(file),
                Extension = Path.GetExtension(file),
                Size = new FileInfo(file).Length
            });
        }

        return Task.FromResult(results);
    }
}
