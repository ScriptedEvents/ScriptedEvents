namespace ScriptedEvents.Actions.Interfaces
{
    using ScriptedEvents.API.Enums;

    /// <summary>
    /// Represents any action.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets aliases of the action. Currently unused.
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// Gets this action's <see cref="ActionSubgroup"/>.
        /// </summary>
        public ActionSubgroup Subgroup { get; }

        /// <summary>
        /// Gets or sets the arguments that this action instance will run with.
        /// </summary>
        string[] Arguments { get; set; }
    }
}
