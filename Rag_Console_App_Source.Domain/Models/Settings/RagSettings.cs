namespace Rag_Console_App_Source.Domain.Models.Settings;

public sealed class RagSettings
{
    public int    ChunkSize       { get; set; } = 500;
    public int    ChunkOverlap    { get; set; } = 50;
    public int    TopK            { get; set; } = 5;
    public string DocumentsFolder { get; set; } = "Data/Documents";
}
