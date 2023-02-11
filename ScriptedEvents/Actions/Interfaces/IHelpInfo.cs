namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.Structures;

    /// <summary>
    /// Represents an action that can be used in the "HELP" action to get information about it.
    /// </summary>
    public interface IHelpInfo
    {
        /// <summary>
        /// The description of the action.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// An array of expected arguments for the action.
        /// </summary>
        public Argument[] ExpectedArguments { get; }
    }
}
