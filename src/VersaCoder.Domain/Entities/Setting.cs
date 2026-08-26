namespace VersaCoder.Domain.Entities;

public class Setting
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }

    protected Setting() { }

    public Setting(string key, string value, string category)
    {
        Id = Guid.NewGuid();
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue ?? throw new ArgumentNullException(nameof(newValue));
        UpdatedAt = DateTime.UtcNow;
    }
}
