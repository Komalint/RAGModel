namespace Rag_Console_App_Source.Domain.Models;

public sealed class ScoredChunkModel
{
    public required ChunkModel Chunk { get; init; }
    public required float      Score { get; init; }
}
