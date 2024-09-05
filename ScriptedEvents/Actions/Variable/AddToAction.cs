namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class AddToAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "APPEND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Adds a specified string to the end of the variable value, and saves that as the new value.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(IStringVariable), "The string variable.", true),
            new Argument("value", typeof(string), "The value to add. Math is supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            IStringVariable var = (IStringVariable)Arguments[0];
            string input = Arguments.JoinMessage(1);

            input = $"{var.Value} {input}";

            try
            {
                if (script.UniqueVariables.ContainsKey(var.Name))
                    script.AddVariable(var.Name, var.Description, ConditionHelperV2.Math(input).ToString());
                else
                    VariableSystemV2.DefineVariable(var.Name, var.Description, ConditionHelperV2.Math(input).ToString(), script);

                return new(true);
            }
            catch
            {
            }

            if (script.UniqueVariables.ContainsKey(var.Name))
                script.AddVariable(var.Name, var.Description, input);
            else
                VariableSystemV2.DefineVariable(var.Name, var.Description, input, script);

            return new(true);
        }
    }
}
