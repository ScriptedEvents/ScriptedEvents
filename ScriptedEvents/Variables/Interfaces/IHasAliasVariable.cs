namespace ScriptedEvents.Variables.Interfaces
{
    /// <summary>
    /// Represents a variable with an alias.
    /// </summary>
    public interface IHasAliasVariable : IConditionVariable
    {
        /// <summary>
        /// Gets the variable alias.
        /// </summary>
        public string Alias { get; }
    }
}
