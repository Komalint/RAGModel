using Microsoft.Extensions.Logging;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Infrastructure.Logging;

namespace Rag_Console_App_Source.Repository.Chunk;

public sealed class ChunkRepo : IChunkRepo
{
    private readonly List<EmbeddedChunkModel> _store = [];
    private readonly ILogger<ChunkRepo>       _logger;

    public ChunkRepo(ILogger<ChunkRepo> logger) => _logger = logger;

    public int Count => _store.Count;

    public void Add(EmbeddedChunkModel chunk)
    {
        _store.Add(chunk);
        _logger.LogDebug("Added chunk {Id}.", chunk.Chunk.Id);
    }

    public void AddRange(IEnumerable<EmbeddedChunkModel> chunks)
    {
        var list = chunks.ToList();
        _store.AddRange(list);
        _logger.LogInformation(
            "Added {Count} chunks. Total in store: {Total}", list.Count, _store.Count);
    }

    public IReadOnlyList<EmbeddedChunkModel> GetAll() => _store.AsReadOnly();

    public void Clear()
    {
        _store.Clear();
        _logger.LogInformation("Chunk store cleared.");
    }
}
