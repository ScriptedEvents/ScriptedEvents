namespace ScriptedEvents.Structures
{
    using System;

    using ScriptedEvents.Interfaces;

    /// <summary>
    /// This struct is designed as a linting addition.
    /// The help command will use the fact that the Type set for this option will declare what value will be returned by an action using it.
    /// Its just for linting, works the same as providing that information in the description of the "Option" struct.
    /// </summary>
    public readonly struct OptionValueDepending : IOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionValueDepending"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The type the action will return based on this option.</param>
        public OptionValueDepending(string name, string returns, Type type)
        {
            Name = name;
            Description = returns;
            Type = type;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        public Type Type { get; }
    }
}