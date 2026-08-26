namespace VersaCoder.Domain.ValueObjects;

public record SessionId
{
    public Guid Value { get; }

    public SessionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("SessionId cannot be empty", nameof(value));
        Value = value;
    }

    public static SessionId New() => new(Guid.NewGuid());

    public static implicit operator Guid(SessionId sessionId) => sessionId.Value;
    public static implicit operator SessionId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
