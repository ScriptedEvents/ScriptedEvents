namespace ScriptedEvents.Actions
{
    using System;

    using MEC;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class WaitAction : ITimingAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WAIT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Yielding;

        /// <inheritdoc/>
        public string Description => "Yields execution of the script for the given number of time.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SEC", "Delay will be using seconds."),
                new("MIL", "Delay will be using miliseconds.")),
            new Argument("delay", typeof(float), "The amount of delay. Math is supported.", true),
        };

        /// <inheritdoc/>
        public float? Execute(Script script, out ActionResponse message)
        {
            string formula = VariableSystemV2.ReplaceVariables(RawArguments.JoinMessage(1), script);

            if (!ConditionHelperV2.TryMath(formula, out MathResult result))
            {
                message = new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
                return null;
            }

            if (result.Result < 0)
            {
                message = new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
                return null;
            }

            message = new(true);
            return Timing.WaitForSeconds(Arguments.ToUpper() == "MIL" ? result.Result / 1000 : result.Result);
        }
    }
}
