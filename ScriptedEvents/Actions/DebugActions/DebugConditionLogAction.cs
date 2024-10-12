namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DebugConditionLogAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "DEBUGCOND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "Condition to debug", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Logger.Info(ConditionHelper.Evaluate(Arguments.JoinMessage(0), script).ToString(), script);
            return new(true);
        }
    }
}