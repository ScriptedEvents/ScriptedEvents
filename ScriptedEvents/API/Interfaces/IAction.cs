namespace ScriptedEvents.API.Interfaces
{
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <summary>
    /// Represents any action.
    /// </summary>
    public interface IAction : IScriptComponent
    {
        /// <summary>
        /// Gets aliases of the action. Currently unused.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Gets this action's <see cref="ActionSubgroup"/>.
        /// </summary>
        public ActionSubgroup Subgroup { get; }

        /// <summary>
        /// Gets or sets the arguments that this action instance will run with.
        /// </summary>
        public object[] Arguments { get; set; }

        /// <summary>
        /// Gets or sets the raw arguments.
        /// </summary>
        public string[] RawArguments { get; set; }

        /// <summary>
        /// Gets an array of expected arguments for the action.
        /// </summary>
        public Argument[] ExpectedArguments { get; }
    }
}
