namespace ScriptedEvents.Variables
{
    using ScriptedEvents.Variables.Interfaces;

    public class CustomVariable : IStringVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomVariable"/> class.
        /// </summary>
        public CustomVariable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="description">The description of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public CustomVariable(string name, string description, string value)
        {
            Name = name;
            Description = description;
            Value = value;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public string Value { get; }
    }
}
