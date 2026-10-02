using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Domain.Models.Settings;
using Rag_Console_App_Source.Infrastructure.Logging;

namespace Rag_Console_App_Source.Logic;

public sealed class ChunkingLogic : IChunkingLogic
{
    private readonly RagSettings            _settings;
    private readonly ILogger<ChunkingLogic> _logger;

    public ChunkingLogic(IOptions<RagSettings> settings, ILogger<ChunkingLogic> logger)
    {
        _settings = settings.Value;
        _logger   = logger;
    }

    public List<ChunkModel> Chunk(string documentName, string content)
    {
        _logger.LogOperationStart(nameof(Chunk));

        int chunkSize    = _settings.ChunkSize;
        int chunkOverlap = _settings.ChunkOverlap;
        int step         = chunkSize - chunkOverlap;

        if (step <= 0)
        {
            _logger.LogWarning(
                "ChunkOverlap ({Overlap}) >= ChunkSize ({Size}). Resetting step to ChunkSize.",
                chunkOverlap, chunkSize);
            step = chunkSize;
        }

        var chunks   = new List<ChunkModel>();
        int index    = 0;
        int chunkIdx = 0;

        while (index < content.Length)
        {
            int    length = Math.Min(chunkSize, content.Length - index);
            string text   = content.Substring(index, length).Trim();

            if (!string.IsNullOrWhiteSpace(text))
            {
                chunks.Add(new ChunkModel
                {
                    Id           = Guid.NewGuid(),
                    DocumentName = documentName,
                    ChunkIndex   = chunkIdx,
                    Text         = text
                });
                chunkIdx++;
            }

            index += step;
        }

        _logger.LogInformation(
            "Produced {Count} chunks from '{Document}'.", chunks.Count, documentName);
        _logger.LogOperationEnd(nameof(Chunk));
        return chunks;
    }
}
