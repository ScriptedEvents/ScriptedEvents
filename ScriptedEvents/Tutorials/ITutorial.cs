namespace ScriptedEvents.Tutorials
{
    public interface ITutorial
    {
        /// <summary>
        /// Gets the filename (without .txt).
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the name of the tutorial.
        /// </summary>
        public string TutorialName { get; }

        /// <summary>
        /// Gets the author of the tutorial.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the tutorial's category.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the contents of the script.
        /// </summary>
        public string Contents { get; }
    }
}
