namespace VersaCoder.Domain.ValueObjects;

public record FilePath
{
    public string Value { get; }

    public FilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("FilePath cannot be empty", nameof(value));
        Value = value;
    }

    public static implicit operator string(FilePath filePath) => filePath.Value;
    public static implicit operator FilePath(string value) => new(value);

    public string GetFileName() => Path.GetFileName(Value);
    public string GetExtension() => Path.GetExtension(Value);
    public string GetDirectoryName() => Path.GetDirectoryName(Value) ?? string.Empty;

    public override string ToString() => Value;
}
