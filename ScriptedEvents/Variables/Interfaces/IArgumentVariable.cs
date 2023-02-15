namespace ScriptedEvents.Variables.Interfaces
{
    using ScriptedEvents.Structures;

    /// <summary>
    /// Represents a variable that supports arguments.
    /// </summary>
    public interface IArgumentVariable
    {
        /// <summary>
        /// Gets or sets the variable arguments.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// Gets the expected arguments.
        /// </summary>
        public Argument[] ExpectedArguments { get; }
    }
}
