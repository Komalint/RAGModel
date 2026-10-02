using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Dto.Response;
using Rag_Console_App_Source.Infrastructure.Exceptions;
using Rag_Console_App_Source.Logic;
using Rag_Console_App_Source.Repository.Chunk;

namespace Rag_Console_App_Source.Controllers;

public sealed class RagController
{
    private readonly IDocumentLogic _documentLogic;
    private readonly IAnswerLogic   _answerLogic;
    private readonly IChunkRepo     _chunkRepo;

    public RagController(
        IDocumentLogic documentLogic,
        IAnswerLogic   answerLogic,
        IChunkRepo     chunkRepo)
    {
        _documentLogic = documentLogic;
        _answerLogic   = answerLogic;
        _chunkRepo     = chunkRepo;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("   RAG Console App - Powered by Ollama  ");
        Console.WriteLine("========================================");

        bool running = true;
        while (running && !ct.IsCancellationRequested)
        {
            Console.WriteLine();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("  1) Upload a document");
            Console.WriteLine("  2) Ask a question");
            Console.WriteLine("  3) Show chunk count");
            Console.WriteLine("  4) Exit");
            Console.Write("> ");

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": await HandleUploadAsync(ct);   break;
                case "2": await HandleQuestionAsync(ct); break;
                case "3": Console.WriteLine($"Chunks in memory: {_chunkRepo.Count}"); break;
                case "4": running = false; Console.WriteLine("Goodbye!"); break;
                default:  Console.WriteLine("Invalid choice. Enter 1, 2, 3, or 4."); break;
            }
        }
    }

    private async Task HandleUploadAsync(CancellationToken ct)
    {
        Console.Write("Enter the full path to your document (.txt or .md): ");
        string? path = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("No path entered.");
            return;
        }

        try
        {
            Console.WriteLine("Ingesting document, please wait...");
            IngestDocumentResponse result = await _documentLogic.IngestAsync(
                new IngestDocumentRequest { SourceFilePath = path }, ct);

            Console.WriteLine();
            Console.WriteLine($"\u2713 '{result.DocumentName}' ingested successfully.");
            Console.WriteLine($"  {result.ChunkCount} chunks embedded and stored in memory.");
        }
        catch (RagException ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Unexpected error] {ex.Message}");
        }
    }

    private async Task HandleQuestionAsync(CancellationToken ct)
    {
        if (_chunkRepo.Count == 0)
        {
            Console.WriteLine(
                "No documents uploaded yet. Please upload a document first (option 1).");
            return;
        }

        Console.Write("Enter your question: ");
        string? question = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(question))
        {
            Console.WriteLine("No question entered.");
            return;
        }

        try
        {
            Console.WriteLine("Thinking, please wait...");
            AskQuestionResponse result = await _answerLogic.AnswerAsync(
                new AskQuestionRequest { Question = question }, ct);

            Console.WriteLine();
            Console.WriteLine("--- Answer ---");
            Console.WriteLine(result.Answer);

            Console.WriteLine();
            Console.WriteLine("--- Top Sources ---");
            for (int i = 0; i < result.Sources.Count; i++)
            {
                var src = result.Sources[i];
                Console.WriteLine(
                    $"  {i + 1}. [{src.Chunk.DocumentName}] " +
                    $"Chunk #{src.Chunk.ChunkIndex} | Score: {src.Score:F4}");
            }
        }
        catch (RagException ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Unexpected error] {ex.Message}");
        }
    }
}
