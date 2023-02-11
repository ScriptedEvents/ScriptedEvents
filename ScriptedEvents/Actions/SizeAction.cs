using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
    public class SizeAction : IScriptAction, IHelpInfo
    {
        public string Name => "SIZE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Sets all players to the given size.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to rescale.", true),
            new Argument("size X", typeof(float), "The X size to put on the player.", true),
            new Argument("size Y", typeof(float), "The Y size to put on the player.", true),
            new Argument("size Z", typeof(float), "The Z size to put on the player.", true),
            new Argument("max", typeof(int), "The maximum amount of players to rescale (default: unlimited).", false),
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 4)
            {
                return new(false, "Missing arguments: players, size X, size Y, size Z, max(optional)");
            }
            if (!float.TryParse(Arguments.ElementAt(1), out float x))
                return new(false, $"X-scale '{Arguments.ElementAt(1)}' is not a valid number.");
            if (!float.TryParse(Arguments.ElementAt(2), out float y))
                return new(false, $"Y-scale '{Arguments.ElementAt(2)}' is not a valid number.");
            if (!float.TryParse(Arguments.ElementAt(3), out float z))
                return new(false, $"Z-scale '{Arguments.ElementAt(3)}' is not a valid number.");

            int max = -1;

            if (Arguments.Length > 5)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

                try
                {
                    float maxFloat = (float)ConditionHelper.Math(formula);
                    if (maxFloat != (int)maxFloat)
                    {
                        max = Mathf.RoundToInt(maxFloat);
                    }
                    else
                    {
                        max = (int)maxFloat;
                    }
                }
                catch (Exception ex)
                {
                    return new(false, $"Invalid maximum condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
                }

                if (max < 0)
                {
                    return new(false, "A negative number cannot be used as the max argument of the SIZE action.");
                }
            }


            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out List<Player> plys))
                return new(false, "No players matching the criteria were found.");

            foreach (Player player in plys)
                player.Scale = new(x, y, z);

            return new(true);
        }
    }
}
