namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

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
        public string Description => "Saves variables to long-term storage (.txt file) in the specified storage folder.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use. Either LITERAL or PLAYER.", true),
            new Argument("variable", typeof(string), "The variable to save.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = ((string)Arguments[0]).ToUpper();

            if (mode == "LITERAL")
            {
                if (!VariableSystem.TryGetVariable(RawArguments[1], out IConditionVariable var, out bool _, script))
                    return new(false, "Invalid literal variable to store has been provided.");

                VariableStorage.Save(var);
                return new(true);
            }

            if (mode == "PLAYER")
            {
                string varName = RawArguments[1];

                if (!VariableSystem.TryGetPlayers(varName, out PlayerCollection var, script))
                    return new(false, "Invalid player variable to store has been provided.");

                string formattedVar = string.Join(".", var.ToList());
                VariableStorage.Save(varName, formattedVar);
                return new(true);
            }

            return new(false, $"Invalid mode '{mode}' selected.");
        }
    }
}
