namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class VExistsAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "VEXISTS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "Returns TRUE if the variable with the given name exists in the current context, else FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variableName", typeof(string), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = VariableSystemV2.TryGetVariable((string)Arguments[0], script, out VariableResult _).ToUpper();

            return new(true, variablesToRet: new[] { ret });
        }
    }
}