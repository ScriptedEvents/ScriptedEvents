namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class GotoIfAction : IScriptAction, ILogicAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "GOTOIF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Reads the condition and jumps to the first provided line if the condition is TRUE, or the second provided line if the condition is FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("label", typeof(string), "The label to jump to if the condition is TRUE. Variables & Math are NOT supported.", true),
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.GotoInput;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            // Support for old GOTOIF
            if (script.Flags.Contains("OLD-GOTOIF"))
            {
                if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

                ConditionResponse deprecatedOutcome = ConditionHelperV2.Evaluate(string.Join(" ", Arguments.Skip(2)), script);
                if (!deprecatedOutcome.Success)
                    return new(false, $"IF execution error: {deprecatedOutcome.Message}", ActionFlags.FatalError);

                if (deprecatedOutcome.Passed)
                {
                    script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");
                    if (!script.Jump(Arguments[0]))
                    {
                        if (Arguments[0].ToUpper() == "STOP")
                            return new(true, flags: ActionFlags.StopEventExecution);

                        return new(false, ErrorGen.Get(139, "trueLine", Arguments[0]));
                    }
                }
                else
                {
                    script.DebugLog($"GOTOIF result: false. Jumping to line {Arguments[1]}.");
                    if (!script.Jump(Arguments[1]))
                    {
                        if (Arguments[1].ToUpper() == "STOP")
                            return new(true, flags: ActionFlags.StopEventExecution);

                        return new(false, ErrorGen.Get(139, "falseLine", Arguments[1]));
                    }
                }

                return new(true);
            }

            // Standard GOTOIF
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            ConditionResponse outcome = ConditionHelperV2.Evaluate(string.Join(" ", Arguments.Skip(1)), script);
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
            {
                script.DebugLog($"GOTOIF result: true. Jumping to line {Arguments[0]}.");
                if (!script.Jump(Arguments[0]))
                {
                    if (Arguments[0].ToUpper() == "STOP")
                        return new(true, flags: ActionFlags.StopEventExecution);

                    return new(false, ErrorGen.Get(139, "trueLine", Arguments[0]));
                }
            }
            else
            {
                script.DebugLog($"GOTOIF result: false. Proceeding to next line.");
            }

            return new(true);
        }
    }
}