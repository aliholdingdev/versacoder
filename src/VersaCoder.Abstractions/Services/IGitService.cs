namespace VersaCoder.Abstractions.Services;

public interface IGitService
{
    Task<GitStatus> GetStatusAsync(string repositoryPath, CancellationToken cancellationToken = default);
    Task<List<GitCommit>> GetLogAsync(string repositoryPath, int count = 10, CancellationToken cancellationToken = default);
    Task<GitDiff> GetDiffAsync(string repositoryPath, string? fromCommit = null, string? toCommit = null, CancellationToken cancellationToken = default);
    Task<GitCommit> CommitAsync(string repositoryPath, string message, CancellationToken cancellationToken = default);
    Task PushAsync(string repositoryPath, string remote = "origin", string branch = "main", CancellationToken cancellationToken = default);
    Task PullAsync(string repositoryPath, string remote = "origin", string branch = "main", CancellationToken cancellationToken = default);
    Task<List<GitBranch>> GetBranchesAsync(string repositoryPath, CancellationToken cancellationToken = default);
}

public class GitStatus
{
    public bool IsClean { get; set; }
    public List<string> ModifiedFiles { get; set; } = new();
    public List<string> AddedFiles { get; set; } = new();
    public List<string> DeletedFiles { get; set; } = new();
    public List<string> UntrackedFiles { get; set; } = new();
    public string CurrentBranch { get; set; } = string.Empty;
}

public class GitCommit
{
    public string Sha { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class GitDiff
{
    public string FromCommit { get; set; } = string.Empty;
    public string ToCommit { get; set; } = string.Empty;
    public List<GitDiffFile> Files { get; set; } = new();
}

public class GitDiffFile
{
    public string Path { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Additions { get; set; }
    public int Deletions { get; set; }
}

public class GitBranch
{
    public string Name { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
    public string? TrackingBranch { get; set; }
}
