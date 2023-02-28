namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <summary>
    /// Signifies that an action can be used in the "HELP" action to get information about it.
    /// </summary>
    public interface IHelpInfo
    {
        /// <summary>
        /// Gets this action's <see cref="ActionSubgroup"/>.
        /// </summary>
        public ActionSubgroup Subgroup { get; }

        /// <summary>
        /// Gets the description of the action.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets an array of expected arguments for the action.
        /// </summary>
        public Argument[] ExpectedArguments { get; }
    }
}
