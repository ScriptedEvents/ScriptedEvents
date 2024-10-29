using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class SaveAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "SAVE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Returns the value that was provided, but in a variable form.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("value", typeof(object), "The value to store.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string input = Arguments.JoinMessage();

            return new(true, variablesToRet: new[] { input });
        }
    }
}
