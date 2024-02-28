namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class StorageAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "STORE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Saves variables to long-term storage (.txt file) in the specified storage folder. This action doesn't work on player variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The variable to save.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (!VariableSystem.TryGetVariable((string)Arguments[0], out IConditionVariable var, out bool _, script))
                return new(false, "Invalid variable to store has been provided.");

            VariableStorage.Save(var);
            return new(true);
        }
    }
}
