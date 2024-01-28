namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    // Represents a line in a file that does not have any actions.
    public class NullAction : IAction, IHiddenAction, ICustomReadDisplay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullAction"/> class, with an unknown NULL type.
        /// </summary>
        public NullAction()
        {
            Type = "UNKNOWN";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullAction"/> class, with the specified type.
        /// </summary>
        /// <param name="type">The type of NULL action.</param>
        public NullAction(string type)
        {
            Type = type;
        }

        /// <inheritdoc/>
        public string Name => "NULL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <summary>
        /// Gets the type of NULL action.
        /// </summary>
        public string Type { get; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = Array.Empty<Argument>();

        /// <inheritdoc/>
        public bool Read(out string display)
        {
            display = $"NULL <{Type}>";
            return MainPlugin.Singleton.Config.Debug;
        }
    }
}
