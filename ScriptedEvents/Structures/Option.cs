namespace ScriptedEvents.Structures
{
    using ScriptedEvents.Interfaces;

    public readonly struct Option : IOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        public Option(string name, string? description = null)
        {
            Name = name;
            Description = description ?? string.Empty;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }
    }
}
