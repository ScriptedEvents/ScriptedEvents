namespace ScriptedEvents.Structures
{
    /// <summary>
    /// Holds information about an SE error.
    /// </summary>
    public record ErrorInfo(string name, string description, string source);

    public static class ErrorInfoExtensions
    {
        public static ErrorTrace ToTrace(this ErrorInfo error)
        {
            return new(error);
        }
    }
}
