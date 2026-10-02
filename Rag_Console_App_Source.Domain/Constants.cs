namespace Rag_Console_App_Source.Domain;

public static class Constants
{
    public static class SupportedExtensions
    {
        public const string Txt = ".txt";
        public const string Md  = ".md";

        public static readonly string[] All = [Txt, Md];
    }

    public static class Prompts
    {
        public const string SystemInstruction =
            "You are a helpful assistant. Answer ONLY from the context provided. " +
            "If the answer is not in the context, say you don't know.";

        public const string ContextHeader  = "Context:";
        public const string QuestionHeader = "Question:";
    }

    public static class Think
    {
        public const string OpenTag  = "<think>";
        public const string CloseTag = "</think>";
    }
}
