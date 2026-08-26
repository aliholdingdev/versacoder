namespace VersaCoder.Domain.Entities;

public class FileEntry
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string? ContentHash { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public Project Project { get; set; } = null!;

    protected FileEntry() { }

    public FileEntry(Guid projectId, string relativePath, string fileName, string extension, long size)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        Extension = extension ?? throw new ArgumentNullException(nameof(extension));
        Size = size;
        LastModified = DateTime.UtcNow;
    }

    public void UpdateHash(string hash)
    {
        ContentHash = hash;
        LastModified = DateTime.UtcNow;
    }
}
