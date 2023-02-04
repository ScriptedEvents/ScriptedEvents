using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class SavePlayerVariableAction : IAction
    {
        public string Name => "SAVEPLAYERVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
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
