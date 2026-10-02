namespace Rag_Console_App_Source.Domain.Models.Settings;

public sealed class OllamaSettings
{
    public string BaseUrl        { get; set; } = "http://localhost:11434";
    public string EmbeddingModel { get; set; } = "qwen3-embedding:4b";
    public string ChatModel      { get; set; } = "qwen3:4b";
    public int    EmbedBatchSize { get; set; } = 16;
}
