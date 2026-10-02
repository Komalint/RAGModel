using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rag_Console_App_Source.Domain.Models;
using Rag_Console_App_Source.Domain.Models.Settings;
using Rag_Console_App_Source.Foundation.Common;
using Rag_Console_App_Source.Infrastructure.Exceptions;
using Rag_Console_App_Source.Infrastructure.Logging;

namespace Rag_Console_App_Source.Repository.Document;

public sealed class DocumentRepo : IDocumentRepo
{
    private readonly RagSettings           _settings;
    private readonly ILogger<DocumentRepo> _logger;

    public DocumentRepo(IOptions<RagSettings> settings, ILogger<DocumentRepo> logger)
    {
        _settings = settings.Value;
        _logger   = logger;
    }

    public async Task<DocumentModel> SaveAndReadAsync(
        string sourceFilePath, CancellationToken ct = default)
    {
        _logger.LogOperationStart(nameof(SaveAndReadAsync));

        if (!File.Exists(sourceFilePath))
            throw new RagException($"File not found: {sourceFilePath}");

        if (!FileHelper.IsSupported(sourceFilePath, Domain.Constants.SupportedExtensions.All))
            throw new RagException(
                $"Unsupported file type '{Path.GetExtension(sourceFilePath)}'. " +
                $"Supported: {string.Join(", ", Domain.Constants.SupportedExtensions.All)}");

        string destDir = Path.GetFullPath(_settings.DocumentsFolder);
        FileHelper.EnsureDirectoryExists(destDir);

        string fileName = Path.GetFileName(sourceFilePath);
        string destPath = Path.Combine(destDir, fileName);

        try
        {
            File.Copy(sourceFilePath, destPath, overwrite: true);
            _logger.LogInformation("Copied document to {DestPath}", destPath);
        }
        catch (Exception ex)
        {
            throw new RagException($"Could not copy file to {destPath}.", ex);
        }

        string content;
        try
        {
            content = await File.ReadAllTextAsync(destPath, ct);
        }
        catch (Exception ex)
        {
            throw new RagException($"Could not read file {destPath}.", ex);
        }

        _logger.LogOperationEnd(nameof(SaveAndReadAsync));
        return new DocumentModel { Name = fileName, Content = content };
    }
}
