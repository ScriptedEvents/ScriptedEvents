namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
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
            new OptionsArgument("mode", true,
                new("LITERAL", "Saves the value as-is."),
                new("PLAYER", "Saves player(s).")),
            new Argument("variable", typeof(IVariable), "The variable to save.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = ((string)Arguments[0]).ToUpper();

            if (mode == "LITERAL")
            {
                if (!VariableSystemV2.TryGetVariable(RawArguments[1], script, out VariableResult var))
                    return new(false, $"Invalid literal variable '{RawArguments[1]}' to store has been provided.");

                VariableStorage.Save(var.Variable);
            }

            if (mode == "PLAYER")
            {
                string varName = RawArguments[1];

                if (!VariableSystemV2.TryGetPlayers(varName, script, out PlayerCollection var))
                    return new(false, "Invalid player variable to store has been provided.");

                string formattedVar = string.Join(".", var.ToList());
                VariableStorage.Save(varName, formattedVar);
            }

            return new(true);
        }
    }
}
