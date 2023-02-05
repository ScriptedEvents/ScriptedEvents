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
    public class SpawnRuleAction : IAction
    {
        public string Name => "SPAWNRULE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing arguments: role, max(optional)");

            if (!Enum.TryParse<RoleTypeId>(Arguments[0], true, out RoleTypeId roleType))
                return new(false, "Invalid role to spawn as provided.");

            int max = -1;

            if (Arguments.Length > 1)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(1)));

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
                    return new(false, "A negative number cannot be used as the max argument of the SETROLE action.");
                }
            }

            MainPlugin.Handlers.SpawnRules.Remove(roleType);
            MainPlugin.Handlers.SpawnRules.Add(roleType, max);

            return new(true);
        }
    }
}
