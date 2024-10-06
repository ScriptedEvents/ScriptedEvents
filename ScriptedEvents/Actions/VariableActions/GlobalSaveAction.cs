namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class GlobalSaveAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GLOBAL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Makes a defined local variable accessible for all running scripts.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to make global.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            IVariable variable = (IVariable)Arguments[0];
            VariableSystemV2.TryDefineVariable(variable);

            return new(true);
        }
    }
}
