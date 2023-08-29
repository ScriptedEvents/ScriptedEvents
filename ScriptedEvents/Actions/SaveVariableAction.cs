namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SaveVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SAVE";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "SAVEVARIABLE" };

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Saves a new variable. Saved variables can be used in ANY script, and are reset when the round ends.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", true),
            new Argument("value", typeof(object), "The value to store. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string varName = Arguments[0];
            string input = string.Join(" ", Arguments.Skip(1));

            input = VariableSystem.ReplaceVariables(input, script);

            try
            {
                float value = (float)ConditionHelper.Math(input);
                VariableSystem.DefineVariable(varName, "User-defined variable.", value.ToString());
                return new(true);
            }
            catch
            {
            }

            VariableSystem.DefineVariable(varName, "User-defined variable.", input);

            return new(true);
        }
    }
}
