namespace Rag_Console_App_Source.Proxy;

public interface IOllamaProxy
{
    Task<List<float[]>> EmbedAsync(IReadOnlyList<string> texts, CancellationToken ct = default);
    Task<string>        GenerateAsync(string systemPrompt, string userMessage, CancellationToken ct = default);
}
