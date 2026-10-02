using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Dto.Response;

namespace Rag_Console_App_Source.Logic;

public interface IDocumentLogic
{
    Task<IngestDocumentResponse> IngestAsync(
        IngestDocumentRequest request, CancellationToken ct = default);
}
