using MEC;
using System;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.Variables;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
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
                message = new(false, "Missing argument: duration");
                return null;
            }

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            float duration;

            try
            {
                duration = (float)ConditionHelper.Math(formula);
            }
            catch (Exception ex)
            {
                message = new(false, $"Invalid duration condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
                return null;
            }

            message = new(true);
            return Timing.WaitForSeconds(duration);
        }
    }
}
