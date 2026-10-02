using Microsoft.Extensions.Logging;
using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Dto.Response;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Infrastructure.Logging;
using Rag_Console_App_Source.Proxy;
using Rag_Console_App_Source.Repository.Chunk;
using Rag_Console_App_Source.Repository.Document;

namespace Rag_Console_App_Source.Logic;

public sealed class DocumentLogic : IDocumentLogic
{
    private readonly IDocumentRepo          _documentRepo;
    private readonly IChunkRepo             _chunkRepo;
    private readonly IChunkingLogic         _chunkingLogic;
    private readonly IOllamaProxy           _ollamaProxy;
    private readonly ILogger<DocumentLogic> _logger;

    public DocumentLogic(
        IDocumentRepo          documentRepo,
        IChunkRepo             chunkRepo,
        IChunkingLogic         chunkingLogic,
        IOllamaProxy           ollamaProxy,
        ILogger<DocumentLogic> logger)
    {
        _documentRepo  = documentRepo;
        _chunkRepo     = chunkRepo;
        _chunkingLogic = chunkingLogic;
        _ollamaProxy   = ollamaProxy;
        _logger        = logger;
    }

    public async Task<IngestDocumentResponse> IngestAsync(
        IngestDocumentRequest request, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(IngestAsync));

        // 1. Copy file into documents folder and read its text
        DocumentModel doc = await _documentRepo.SaveAndReadAsync(request.SourceFilePath, ct);

        // 2. Split into fixed-size chunks
        List<ChunkModel> chunks = _chunkingLogic.Chunk(doc.Name, doc.Content);

        // 3. Embed all chunk texts via Ollama
        List<string>  texts      = chunks.Select(c => c.Text).ToList();
        List<float[]> embeddings = await _ollamaProxy.EmbedAsync(texts, ct);

        // 4. Pair chunk + embedding and store in memory
        var embedded = chunks
            .Zip(embeddings, (chunk, emb) => new EmbeddedChunkModel
            {
                Chunk     = chunk,
                Embedding = emb
            })
            .ToList();

        _chunkRepo.AddRange(embedded);

        _logger.LogOperationEnd(nameof(IngestAsync));
        return new IngestDocumentResponse
        {
            DocumentName = doc.Name,
            ChunkCount   = embedded.Count
        };
    }
}
