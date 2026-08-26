namespace VersaCoder.Abstractions.Providers;

public interface IEmbeddingProvider
{
    string Name { get; }
    bool IsAvailable { get; }
    
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<List<float[]>> GetEmbeddingsAsync(List<string> texts, CancellationToken cancellationToken = default);
    int MaxTokens { get; }
    int Dimension { get; }
}
