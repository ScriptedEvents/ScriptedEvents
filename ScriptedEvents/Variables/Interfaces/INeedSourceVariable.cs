namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Indicates a variable that requests its script source to be stored as a property of the variable.
    /// </summary>
    public interface INeedSourceVariable
    {
        /// <summary>
        /// Gets or sets the source that is executing this variable.
        /// </summary>
        public Script Source { get; set; }
    }
}
