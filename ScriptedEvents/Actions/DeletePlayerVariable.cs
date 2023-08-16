namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class DeletePlayerVariable : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DELPLAYERVARIABLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Deletes a previously-defined player variable.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(IPlayerVariable), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
            if (VariableSystem.DefinedPlayerVariables.ContainsKey(Arguments[0]))
            {
                VariableSystem.DefinedPlayerVariables.Remove(Arguments[0]);
                return new(true);
            }

            return new(false, $"Invalid variable '{Arguments[0]}'");
        }
    }
}
