namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class TempVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TEMP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Saves a new variable with the name of '{@}' in the local scope. It works the same as doing 'LOCAL {@} ...'. The variable will be able to be used until its overwritten by another TEMP action.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(object), "The value to store. Math is supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string input = Arguments.JoinMessage(0);
            input = VariableSystemV2.ReplaceVariables(input, script).Replace("\\n", "\n");

            try
            {
                float value = (float)ConditionHelperV2.Math(input);
                script.AddVariable("{@}", "User-defined variable.", value.ToString());
                return new(true);
            }
            catch
            {
                script.AddVariable("{@}", "User-defined variable.", input);
                return new(true);
            }
        }
    }
}