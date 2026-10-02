using Rag_Console_App_Source.Domain.Models;

namespace Rag_Console_App_Source.Repository.Document;

public interface IDocumentRepo
{
    Task<DocumentModel> SaveAndReadAsync(string sourceFilePath, CancellationToken ct = default);
}
