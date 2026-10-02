namespace Rag_Console_App_Source.Domain.Dto.Response;

public sealed class IngestDocumentResponse
{
    public required string DocumentName { get; init; }
    public required int    ChunkCount   { get; init; }
}
