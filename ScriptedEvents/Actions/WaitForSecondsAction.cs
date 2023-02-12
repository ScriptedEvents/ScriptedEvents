namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class WaitForSecondsAction : ITimingAction, IHelpInfo
    {
        public string Name => "WAITSEC";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Yields execution of the script for the given number of seconds.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("seconds", typeof(float), "The amount of seconds. Variables & Math are supported.", true),
        };

        public float? Execute(Script scr, out ActionResponse message)
        {
            if (Arguments.Length < 1)
            {
                message = new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                return null;
            }

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                message = new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
                return null;
            }

            if (result.Result < 0)
            {
                message = new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
                message = new(false, "A negative number cannot be used as the duration argument of the WAITSEC action.");
                return null;
            }

            message = new(true);
            return Timing.WaitForSeconds(result.Result);
        }
    }
}
