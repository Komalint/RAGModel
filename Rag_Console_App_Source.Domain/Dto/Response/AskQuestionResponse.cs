using Rag_Console_App_Source.Domain.Models;

namespace Rag_Console_App_Source.Domain.Dto.Response;

public sealed class AskQuestionResponse
{
    public required string                 Answer  { get; init; }
    public required List<ScoredChunkModel> Sources { get; init; }
}
