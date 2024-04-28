namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GotoIfAction : IScriptAction, ILogicAction, IHelpInfo, ILongDescription, IIgnoresSkipAction
    {
        /// <inheritdoc/>
        public string Name => "GOTOIF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Moves to the provided label if the condition evaluates to TRUE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to jump to.", true),
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.GotoInput;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            script.SkipExecution = false;
            string label = (string)Arguments[0];

            ConditionResponse outcome = ConditionHelperV2.Evaluate(Arguments.JoinMessage(1), script);
            if (!outcome.Success)
                return new(false, $"GOTOIF execution error: {outcome.Message}.", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");
                if (!script.Jump(label))
                    return new(false, ErrorGen.Get(ErrorCode.ScriptJumpFailed, "trueLine", Arguments[0]));
            }
            else
            {
                script.DebugLog($"GOTOIF result: false. Proceeding to next line.");
            }

            return new(true);
        }
    }
}