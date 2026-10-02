using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rag_Console_App_Source.Controllers;
using Rag_Console_App_Source.Domain.Models.Settings;
using Rag_Console_App_Source.Logic;
using Rag_Console_App_Source.Proxy;
using Rag_Console_App_Source.Repository.Chunk;
using Rag_Console_App_Source.Repository.Document;

var builder = Host.CreateApplicationBuilder(args);

// Bind appsettings.json sections to strongly-typed settings classes
builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("Ollama"));
builder.Services.Configure<RagSettings>(builder.Configuration.GetSection("Rag"));

// Register every service as singleton (in-memory list must live for the whole run)
builder.Services.AddSingleton<IOllamaProxy,    OllamaProxy>();
builder.Services.AddSingleton<IDocumentRepo,   DocumentRepo>();
builder.Services.AddSingleton<IChunkRepo,      ChunkRepo>();
builder.Services.AddSingleton<IChunkingLogic,  ChunkingLogic>();
builder.Services.AddSingleton<IDocumentLogic,  DocumentLogic>();
builder.Services.AddSingleton<IRetrievalLogic, RetrievalLogic>();
builder.Services.AddSingleton<IAnswerLogic,    AnswerLogic>();
builder.Services.AddSingleton<RagController>();

var app = builder.Build();

// Resolve the controller and hand control to it
var controller = app.Services.GetRequiredService<RagController>();
await controller.RunAsync();
