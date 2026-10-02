using Rag_Console_App_Source.Domain.Models;

namespace Rag_Console_App_Source.Repository.Chunk;

public interface IChunkRepo
{
    void   Add(EmbeddedChunkModel chunk);
    void   AddRange(IEnumerable<EmbeddedChunkModel> chunks);
    IReadOnlyList<EmbeddedChunkModel> GetAll();
    int    Count { get; }
    void   Clear();
}
