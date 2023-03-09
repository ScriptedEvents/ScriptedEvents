namespace ScriptedEvents.Actions.Samples
{
    /// <summary>
    /// Represents a sample of an action, shown in the help command.
    /// </summary>
    public readonly struct Sample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sample"/> class.
        /// </summary>
        /// <param name="title">The title of the code sample.</param>
        /// <param name="description">The description of the code sample.</param>
        /// <param name="code">The code sample.</param>
        public Sample(string title, string description, string code)
        {
            Title = title;
            Description = description;
            Code = code;
        }

        /// <summary>
        /// Gets the title of the code sample.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the description of the code sample.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the code sample.
        /// </summary>
        public string Code { get; }
    }
}
