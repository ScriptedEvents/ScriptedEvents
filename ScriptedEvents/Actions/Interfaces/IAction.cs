namespace ScriptedEvents.Actions.Interfaces
{
    /// <summary>
    /// Represents any action.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the name of  the action.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets aliases of the action. Currently unused.
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// Gets or sets the arguments that this action instance will run with.
        /// </summary>
        string[] Arguments { get; set; }
    }
}
