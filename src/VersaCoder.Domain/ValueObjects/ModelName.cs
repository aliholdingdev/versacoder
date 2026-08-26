namespace VersaCoder.Domain.ValueObjects;

public record ModelName
{
    public string Provider { get; }
    public string Model { get; }

    public ModelName(string provider, string model)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException("Provider cannot be empty", nameof(provider));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty", nameof(model));

        Provider = provider;
        Model = model;
    }

    public static implicit operator string(ModelName modelName) => $"{modelName.Provider}/{modelName.Model}";

    public override string ToString() => $"{Provider}/{Model}";
}
