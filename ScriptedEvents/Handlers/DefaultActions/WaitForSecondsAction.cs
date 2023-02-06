using MEC;
using System;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class WaitForSecondsAction : ITimingAction
    {
        public string Name => "WAITSEC";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            return new(true);
        }

        public float? GetDelay(out ActionResponse message)
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
