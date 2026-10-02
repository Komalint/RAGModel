using Rag_Console_App_Source.Domain.Models;

namespace Rag_Console_App_Source.Logic;

public interface IChunkingLogic
{
    List<ChunkModel> Chunk(string documentName, string content);
}
