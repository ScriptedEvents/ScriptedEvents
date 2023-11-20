namespace ScriptedEvents.DemoScripts
{
    /// <summary>
    /// Represents a demoscript that is automatically generated when the plugin is first installed.
    /// </summary>
    public interface IDemoScript
    {
        /// <summary>
        /// Gets the name of the file (without .txt).
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        public string Contents { get; }
    }
}
