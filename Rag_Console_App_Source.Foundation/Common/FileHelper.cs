namespace Rag_Console_App_Source.Foundation.Common;

public static class FileHelper
{
    public static string GetExtension(string filePath)
        => Path.GetExtension(filePath).ToLowerInvariant();

    public static bool IsSupported(string filePath, string[] supportedExtensions)
        => supportedExtensions.Contains(GetExtension(filePath));

    public static void EnsureDirectoryExists(string directoryPath)
        => Directory.CreateDirectory(directoryPath);
}
