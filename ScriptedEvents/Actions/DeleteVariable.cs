namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class DeleteVariable : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DELVARIABLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Deletes a previously-defined variable.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
            if (ConditionVariables.DefinedVariables.ContainsKey(Arguments[0]))
            {
                ConditionVariables.DefinedVariables.Remove(Arguments[0]);
                return new(true);
            }

            return new(false, $"Invalid variable '{Arguments[0]}'");
        }
    }
}
