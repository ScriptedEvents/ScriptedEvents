namespace ScriptedEvents.Variables.Interfaces
{
    using ScriptedEvents.API.Interfaces;

    /// <summary>
    /// Represents a variable that can be used in scripts.
    /// </summary>
    public interface IVariable : IScriptComponent
    {
        /// <summary>
        /// Gets a description of the variable, used in help commands.
        /// </summary>
        public string Description { get; }
    }
}
