namespace ScriptedEvents.API.Interfaces
{
    /// <summary>
    /// Indicates a component that can be used in scripts (actions or variables).
    /// </summary>
    public interface IScriptComponent
    {
        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string Name { get; }
    }
}
