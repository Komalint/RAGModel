using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rag_Console_App_Source.Domain;
using Rag_Console_App_Source.Domain.Models.Settings;
using Rag_Console_App_Source.Infrastructure.Exceptions;
using Rag_Console_App_Source.Infrastructure.Logging;

namespace Rag_Console_App_Source.Proxy;

public sealed class OllamaProxy : IOllamaProxy
{
    private readonly HttpClient           _http;
    private readonly OllamaSettings       _settings;
    private readonly ILogger<OllamaProxy> _logger;

    // Strip <think>...</think> blocks emitted by reasoning models
    private static readonly Regex ThinkRegex =
        new(Constants.Think.OpenTag + "[\\s\\S]*?" + Constants.Think.CloseTag,
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public OllamaProxy(IOptions<OllamaSettings> settings, ILogger<OllamaProxy> logger)
    {
        _settings = settings.Value;
        _logger   = logger;
        _http     = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    }

    public async Task<List<float[]>> EmbedAsync(
        IReadOnlyList<string> texts, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(EmbedAsync));

        var allEmbeddings = new List<float[]>();
        int batchSize     = _settings.EmbedBatchSize;

        for (int i = 0; i < texts.Count; i += batchSize)
        {
            var batch = texts.Skip(i).Take(batchSize).ToList();

            var payload = new
            {
                model = _settings.EmbeddingModel,
                input = batch
            };

            string json    = JsonConvert.SerializeObject(payload);
            var    content = new StringContent(json, Encoding.UTF8, "application/json");
            string url     = $"{_settings.BaseUrl}/api/embed";

            HttpResponseMessage response;
            try
            {
                response = await _http.PostAsync(url, content, ct);
            }
            catch (Exception ex)
            {
                throw new RagException(
                    $"Cannot reach Ollama at {url}. Is Ollama running?", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync(ct);
                throw new RagException(
                    $"Ollama embed returned {(int)response.StatusCode}: {body}");
            }

            string responseJson = await response.Content.ReadAsStringAsync(ct);
            var    parsed       = JObject.Parse(responseJson);
            var    embeddings   = parsed["embeddings"]
                ?? throw new RagException("Ollama embed response missing 'embeddings' field.");

            foreach (var token in embeddings)
            {
                float[] vec = token.ToObject<float[]>()
                    ?? throw new RagException("Could not parse embedding vector from Ollama response.");
                allEmbeddings.Add(vec);
            }

            _logger.LogInformation("Embedded batch {Start}-{End} of {Total}",
                i + 1, Math.Min(i + batchSize, texts.Count), texts.Count);
        }

        _logger.LogOperationEnd(nameof(EmbedAsync));
        return allEmbeddings;
    }

    public async Task<string> GenerateAsync(
        string systemPrompt, string userMessage, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(GenerateAsync));

        var payload = new
        {
            model    = _settings.ChatModel,
            stream   = false,
            think    = false,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userMessage  }
            }
        };

        string json    = JsonConvert.SerializeObject(payload);
        var    content = new StringContent(json, Encoding.UTF8, "application/json");
        string url     = $"{_settings.BaseUrl}/api/chat";

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsync(url, content, ct);
        }
        catch (Exception ex)
        {
            throw new RagException(
                $"Cannot reach Ollama at {url}. Is Ollama running?", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(ct);
            throw new RagException(
                $"Ollama chat returned {(int)response.StatusCode}: {body}");
        }

        string responseJson = await response.Content.ReadAsStringAsync(ct);
        var    parsed       = JObject.Parse(responseJson);
        string? answer      = parsed["message"]?["content"]?.Value<string>();

        if (answer is null)
            throw new RagException("Ollama chat response missing 'message.content' field.");

        // Remove any <think>...</think> reasoning blocks
        answer = ThinkRegex.Replace(answer, string.Empty).Trim();

        _logger.LogOperationEnd(nameof(GenerateAsync));
        return answer;
    }
}
