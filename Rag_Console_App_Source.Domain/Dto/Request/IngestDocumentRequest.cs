namespace Rag_Console_App_Source.Domain.Dto.Request;

public sealed class IngestDocumentRequest
{
    public required string SourceFilePath { get; init; }
}
