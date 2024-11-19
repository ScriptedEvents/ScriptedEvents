using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.ScriptInfo
{
    using System;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class VExistsAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "VarExists";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.ScriptInfo;

        /// <inheritdoc/>
        public string Description => "Returns TRUE if the variable with the given name exists in the current context, else FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variableName", typeof(string), "The name of the variable. ", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(VariableSystem.TryGetVariable<IVariable>(RawArguments[0], script, out _, false, out _).ToUpper()));
        }
    }
}