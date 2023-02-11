using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Variables;
using System;

namespace ScriptedEvents.Actions
{
    public class TurnOffLightsAction : IScriptAction, IHelpInfo
    {
        public string Name => "LIGHTSOFF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Turns all the lights off for a given period of time.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("duration", typeof(float), "The duration of the lights out", true),
        };

        public ActionResponse Execute(Script script)
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

            foreach (Room room in Room.List)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}