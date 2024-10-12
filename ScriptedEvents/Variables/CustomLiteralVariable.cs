namespace ScriptedEvents.Variables
{
    using ScriptedEvents.Variables.Interfaces;

    public class CustomLiteralVariable : ILiteralVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLiteralVariable"/> class.
        /// </summary>
        public CustomLiteralVariable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLiteralVariable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="description">The description of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public CustomLiteralVariable(string name, string description, string value)
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
