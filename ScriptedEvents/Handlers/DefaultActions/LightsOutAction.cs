using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;

namespace ScriptedEvents.Actions
{
    public class TurnOffLightsAction : IAction
    {
        public string Name => "LIGHTSOFF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: duration");

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            float duration;

            try
            {
                duration = (float)ConditionHelper.Math(formula);
            }
            catch (Exception ex)
            {
                return new(false, $"Invalid duration condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            foreach (var room in Room.List)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}