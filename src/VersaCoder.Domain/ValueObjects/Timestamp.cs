namespace VersaCoder.Domain.ValueObjects;

public record Timestamp
{
    public DateTime Value { get; }

    public Timestamp(DateTime value)
    {
        Value = value;
    }

    public static Timestamp Now => new(DateTime.UtcNow);

    public static implicit operator DateTime(Timestamp timestamp) => timestamp.Value;
    public static implicit operator Timestamp(DateTime value) => new(value);

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm:ss UTC");
}
