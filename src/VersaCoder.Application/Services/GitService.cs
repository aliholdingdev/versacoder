using VersaCoder.Abstractions.Services;

namespace VersaCoder.Application.Services;

public class GitService : IGitService
{
    public Task<GitStatus> GetStatusAsync(string repositoryPath, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GitStatus
        {
            CurrentBranch = "main",
            IsClean = true
        });
    }

    public Task<List<GitCommit>> GetLogAsync(string repositoryPath, int count = 10, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<GitCommit>());
    }

    public Task<GitDiff> GetDiffAsync(string repositoryPath, string? fromCommit = null, string? toCommit = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GitDiff
        {
            FromCommit = fromCommit ?? string.Empty,
            ToCommit = toCommit ?? string.Empty
        });
    }

    public Task<GitCommit> CommitAsync(string repositoryPath, string message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GitCommit
        {
            Message = message,
            Author = "VersaCoder"
        });
    }

    public Task PushAsync(string repositoryPath, string remote = "origin", string branch = "main", CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task PullAsync(string repositoryPath, string remote = "origin", string branch = "main", CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<List<GitBranch>> GetBranchesAsync(string repositoryPath, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<GitBranch>
        {
            new GitBranch { Name = "main", IsCurrent = true }
        });
    }
}
