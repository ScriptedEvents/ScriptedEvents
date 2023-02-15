namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a variable that supports arguments.
    /// </summary>
    public interface IArgumentVariable
    {
        /// <summary>
        /// Gets or sets the variable arguments.
        /// </summary>
        public string[] Arguments { get; set; }
    }
}
