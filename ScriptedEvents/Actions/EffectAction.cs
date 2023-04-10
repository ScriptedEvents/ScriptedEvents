namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class EffectAction : IScriptAction, IHelpInfo
    {
        public string Name => "EFFECT";

        public string[] Aliases => Array.Empty<string>();

        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        public string[] Arguments { get; set; }

        public string Description => "Action for giving/removing player effects.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (GIVE, REMOVE, CLEAR)", true),
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Math and variables are NOT supported. Defaults to 1.", false),
            new Argument("duration", typeof(int), "The duration of the effect, or no duration for a permanent effect. Math and variables ARE supported.", false),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string mode = Arguments[0].ToUpper();

            if (!Enum.TryParse<EffectType>(Arguments[2], true, out EffectType effect))
                return new(false, "Invalid effect type provided.");

            int duration = 0;

            if (Arguments.Length > 4)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(4)), script);

                if (ConditionHelper.TryMath(formula, out MathResult result))
                {
                    duration = Mathf.RoundToInt(result.Result);
                }
                else
                {
                    return new(MessageType.NotANumberOrCondition, this, "duration", formula, result);
                }

                if (duration < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "duration", duration);
                }
            }

            Player[] plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[1], null, out plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            switch (mode)
            {
                case "GIVE":
                    string intensityString = "1";
                    if (Arguments.Length > 3)
                        intensityString = Arguments[3];

                    if (!byte.TryParse(intensityString, out byte intensity))
                        return new(MessageType.NotANumber, this, "intensity", Arguments[3]);

                    foreach (Player player in plys)
                    {
                        Log.Info(duration);
                        player.ChangeEffectIntensity(effect, intensity);
                        player.EnableEffect(effect, duration);
                    }

                    return new(true);
                case "REMOVE":
                    foreach (Player player in plys)
                    {
                        player.DisableEffect(effect);
                    }

                    return new(true);
                case "CLEAR":
                    return new(false, "CLEAR is Not implemented");
                default:
                    return new(MessageType.InvalidOption, this, "mode", mode, "GIVE/REMOVE/CLEAR");
            }
        }
    }
}
