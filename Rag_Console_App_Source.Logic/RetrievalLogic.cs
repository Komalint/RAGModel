using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Domain.Models.Settings;
using Rag_Console_App_Source.Foundation.Common;
using Rag_Console_App_Source.Infrastructure.Logging;
using Rag_Console_App_Source.Proxy;
using Rag_Console_App_Source.Repository.Chunk;

namespace Rag_Console_App_Source.Logic;

public sealed class RetrievalLogic : IRetrievalLogic
{
    private readonly IChunkRepo              _chunkRepo;
    private readonly IOllamaProxy            _ollamaProxy;
    private readonly RagSettings             _settings;
    private readonly ILogger<RetrievalLogic> _logger;

    public RetrievalLogic(
        IChunkRepo              chunkRepo,
        IOllamaProxy            ollamaProxy,
        IOptions<RagSettings>   settings,
        ILogger<RetrievalLogic> logger)
    {
        _chunkRepo   = chunkRepo;
        _ollamaProxy = ollamaProxy;
        _settings    = settings.Value;
        _logger      = logger;
    }

    public async Task<List<ScoredChunkModel>> RetrieveAsync(
        AskQuestionRequest request, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(RetrieveAsync));

        // 1. Embed the question using the same model
        List<float[]> qEmbeddings = await _ollamaProxy.EmbedAsync([request.Question], ct);
        float[]       qVector     = qEmbeddings[0];

        // 2. Score all stored chunks by cosine similarity, return top K
        var scored = _chunkRepo.GetAll()
            .Select(ec => new ScoredChunkModel
            {
                Chunk = ec.Chunk,
                Score = VectorMath.CosineSimilarity(qVector, ec.Embedding)
            })
            .OrderByDescending(s => s.Score)
            .Take(_settings.TopK)
            .ToList();

        _logger.LogInformation(
            "Top {K} chunks retrieved. Best score: {Score:F4}",
            scored.Count, scored.FirstOrDefault()?.Score ?? 0f);

        _logger.LogOperationEnd(nameof(RetrieveAsync));
        return scored;
    }
}
