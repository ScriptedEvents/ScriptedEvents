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
        public string Description => "Saves a new variable with a name of '{@}'. If a provided argument is ONLY a player variable, then it will be copied. Else it will saved as a literal variable and math operations will be performed.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(object), "The value to store. Can be a player variable. Math is supported (when applicable).", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length == 0 && Arguments[0] is PlayerCollection players)
            {
                script.AddPlayerVariable("{@}", string.Empty, players.GetInnerList());
                return new(true);
            }

            string input = RawArguments.JoinMessage(0);
            input = VariableSystemV2.ReplaceVariables(input, script).Replace("\\n", "\n");

            try
            {
                float value = (float)ConditionHelperV2.Math(input);
                script.AddVariable("{@}", string.Empty, value.ToString());
                return new(true);
            }
            catch
            {
                script.AddVariable("{@}", string.Empty, input);
                return new(true);
            }
        }
    }
}