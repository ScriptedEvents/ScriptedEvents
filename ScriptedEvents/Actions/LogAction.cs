namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;

    public class LogAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOG";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Creates a console log. Variables are supported.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Log.Info(string.Join(" ", Arguments.Select(r => ConditionVariables.ReplaceVariables(r))));
            return new(true);
        }
    }
}