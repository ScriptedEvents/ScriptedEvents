namespace ScriptedEvents.Structures
{
    /// <summary>
    /// Holds information about an SE error.
    /// </summary>
    public struct ErrorInfo
    {
        public string Name;
        public string Description;
        public string Source;
        
        public ErrorInfo(string name, string description, string source)
        {
            Name = name;
            Description = description;
            Source = source;
        }
    }

    public static class ErrorInfoExtensions
    {
        public static ErrorTrace ToTrace(this ErrorInfo? error)
        {
            if (error is null) throw new System.ArgumentNullException(nameof(error));
            return new ErrorTrace((ErrorInfo)error);
        }
    }
}
