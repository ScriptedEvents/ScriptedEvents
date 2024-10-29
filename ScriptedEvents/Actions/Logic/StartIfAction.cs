using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    public class StartIfAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "IF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "If the condition is FALSE, all actions will be ignored until ENDIF action is encountered. If the condition is TRUE, script will continue executing normally.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Math is supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ConditionResponse outcome = ConditionHelper.Evaluate(RawArguments.JoinMessage(), script);
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}");

            script.IfActionBlocksExecution = !outcome.Passed;

            return new(true);
        }
    }
}