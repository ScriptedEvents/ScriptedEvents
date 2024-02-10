namespace ScriptedEvents.Structures
{
    using System;

    using ScriptedEvents.API.Features;

    /// <summary>
    /// Represents an argument for an action.
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="type">The type of the argument.</param>
        /// <param name="description">The description of the argument.</param>
        /// <param name="required">Whether or not the argument is required.</param>
        public Argument(string argumentName, Type type, string description, bool required)
        {
            ArgumentName = argumentName;
            Type = type;
            Description = description;
            Required = required;
        }

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the description of the argument.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether or not the argument is required.
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> in a human-readable form.
        /// </summary>
        public string TypeString => Type.Display();

        public override string ToString() =>
            $"{ArgumentName} [T: {TypeString}] [R: {Required}] | {Description}";
    }
}
