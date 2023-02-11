using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.Actions
{
    public class SavePlayerVariableAction : IScriptAction, IHelpInfo
    {
        public string Name => "SAVEPLAYERVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Saves a new player variable. Saved variables can be used in ANY script, and are reset when the round ends.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", true),
            new Argument("players", typeof(List<Player>), "The players to save as the new variable.", true),
            new Argument("max", typeof(int), "The maximum amount of players to save in this variable (default: unlimited).", false),
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 2)
            {
                return new(false, "Missing arguments: variableName, players, max(optional)");
            }

            int max = -1;

            if (Arguments.Length > 2)
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
                    return new(false, "A negative number cannot be used as the max argument of the SAVEPLAYERS action.");
                }
            }

            List<Player> plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[1], max, out plys))
                return new(false, "No players matching the criteria were found.");

            PlayerVariables.DefineVariable(Arguments[0], plys);

            return new(true);
        }
    }
}
