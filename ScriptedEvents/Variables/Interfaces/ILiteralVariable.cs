namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a conditional variable that has a string result.
    /// </summary>
    public interface ILiteralVariable : IVariable
    {
        /// <summary>
        /// Gets the string value of this variable.
        /// </summary>
        public string Value { get; }
    }
}
