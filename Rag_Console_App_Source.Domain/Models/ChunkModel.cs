namespace Rag_Console_App_Source.Domain.Models;

public sealed class ChunkModel
{
    public required Guid   Id           { get; init; }
    public required string DocumentName { get; init; }
    public required int    ChunkIndex   { get; init; }
    public required string Text         { get; init; }
}
