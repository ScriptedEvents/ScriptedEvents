namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

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
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments));
            float duration;

            if (!ConditionHelper.TryMath(formula, out MathResult result))
            {
                return new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
            }

            if (result.Result < 0)
            {
                return new(MessageType.LessThanZeroNumber, this, "duration", result.Result);
            }

            duration = result.Result;

            foreach (Room room in Room.List)
                room.TurnOffLights(duration);

            return new(true, string.Empty);
        }
    }
}