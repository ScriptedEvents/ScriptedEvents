namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GotoIfAction : IScriptAction, ILogicAction, IHelpInfo, ILongDescription
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
            // Support for old GOTOIF
            if (script.HasFlag("OLD-GOTOIF"))
            {
                if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

                ConditionResponse deprecatedOutcome = ConditionHelperV2.Evaluate(Arguments.JoinMessage(2), script);
                if (!deprecatedOutcome.Success)
                    return new(false, $"GOTOIF execution error: {deprecatedOutcome.Message}", ActionFlags.FatalError);

                if (deprecatedOutcome.Passed)
                {
                    script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");

                    if (Arguments[0].ToUpper() == "STOP")
                        return new(true, flags: ActionFlags.StopEventExecution);

                    if (!script.Jump((string)Arguments[0]))
                    {
                        return new(false, ErrorGen.Get(139, "trueLine", Arguments[0]));
                    }
                }
                else
                {
                    script.DebugLog($"GOTOIF result: false. Jumping to line {Arguments[1]}.");

                    if (Arguments[1].ToUpper() == "STOP")
                        return new(true, flags: ActionFlags.StopEventExecution);

                    if (!script.Jump((string)Arguments[1]))
                    {
                        return new(false, ErrorGen.Get(139, "falseLine", Arguments[1]));
                    }
                }

                return new(true);
            }

            // Standard GOTOIF
            string label = (string)Arguments[0];

            ConditionResponse outcome = ConditionHelperV2.Evaluate(Arguments.JoinMessage(1), script);
            if (!outcome.Success)
                return new(false, $"GOTOIF execution error: {outcome.Message}. If you are using an older script, add \"!-- OLD-GOTOIF\" flag for old GOTOIF support or update the script accordingly.", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");
                if (!script.Jump(label))
                    return new(false, ErrorGen.Get(139, "trueLine", Arguments[0]));
            }
            else
            {
                script.DebugLog($"GOTOIF result: false. Proceeding to next line.");
            }

            return new(true);
        }
    }
}