using System.Text;
using Microsoft.Extensions.Logging;
using Rag_Console_App_Source.Domain;
using Rag_Console_App_Source.Domain.Dto.Request;
using Rag_Console_App_Source.Domain.Dto.Response;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Infrastructure.Logging;
using Rag_Console_App_Source.Proxy;

namespace Rag_Console_App_Source.Logic;

public sealed class AnswerLogic : IAnswerLogic
{
    private readonly IRetrievalLogic      _retrievalLogic;
    private readonly IOllamaProxy         _ollamaProxy;
    private readonly ILogger<AnswerLogic> _logger;

    public AnswerLogic(
        IRetrievalLogic      retrievalLogic,
        IOllamaProxy         ollamaProxy,
        ILogger<AnswerLogic> logger)
    {
        _retrievalLogic = retrievalLogic;
        _ollamaProxy    = ollamaProxy;
        _logger         = logger;
    }

    public async Task<AskQuestionResponse> AnswerAsync(
        AskQuestionRequest request, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(AnswerAsync));

        // 1. Retrieve the most relevant chunks
        List<ScoredChunkModel> sources = await _retrievalLogic.RetrieveAsync(request, ct);

        // 2. Build the prompt: numbered context chunks + question
        var userMessage = new StringBuilder();
        userMessage.AppendLine(Constants.Prompts.ContextHeader);
        for (int i = 0; i < sources.Count; i++)
            userMessage.AppendLine($"{i + 1}. {sources[i].Chunk.Text}");

        userMessage.AppendLine();
        userMessage.AppendLine(Constants.Prompts.QuestionHeader);
        userMessage.AppendLine(request.Question);

        // 3. Call the LLM
        string answer = await _ollamaProxy.GenerateAsync(
            Constants.Prompts.SystemInstruction, userMessage.ToString(), ct);

        _logger.LogOperationEnd(nameof(AnswerAsync));
        return new AskQuestionResponse
        {
            Answer  = answer,
            Sources = sources
        };
    }
}
