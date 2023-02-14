namespace ScriptedEvents.DemoScripts
{
    /// <summary>
    /// Represents a demoscript that is automatically generated when the plugin is first installed.
    /// </summary>
    public interface IDemoScript
    {
        /// <summary>
        /// The name of the file (without .txt).
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The contents of the script.
        /// </summary>
        public string Contents { get; }
    }
}
