namespace ScriptedEvents.API.Interfaces
{
    using ScriptedEvents.Structures;

    /// <summary>
    /// Signifies that an action can be used in the "HELP" action to get information about it.
    /// </summary>
    public interface IHelpInfo
    {
        /// <summary>
        /// Gets the description of the action.
        /// </summary>
        public string Description { get; }
    }
}
