using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Variable
{
    public class SaveAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "Save";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Returns the value that was provided, but in a variable (local literal) form.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(object), "The value to store.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script) => new(true, new(Arguments.JoinMessage()));
    }
}
