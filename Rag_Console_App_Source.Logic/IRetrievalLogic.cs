using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Models;

namespace Rag_Console_App_Source.Logic;

public interface IRetrievalLogic
{
    Task<List<ScoredChunkModel>> RetrieveAsync(
        AskQuestionRequest request, CancellationToken ct = default);
}
