namespace Rag_Console_App_Source.Infrastructure.Exceptions;

public sealed class RagException : Exception
{
    public RagException(string message)
        : base(message) { }

    public RagException(string message, Exception innerException)
        : base(message, innerException) { }
}
