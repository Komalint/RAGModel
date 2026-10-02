namespace Rag_Console_App_Source.Domain.Models;

public sealed class EmbeddedChunkModel
{
    public required ChunkModel Chunk     { get; init; }
    public required float[]    Embedding { get; init; }
}
