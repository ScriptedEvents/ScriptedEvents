using PlayerRoles;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Variables;
using System;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.Actions
{
    public class SpawnRuleAction : IScriptAction, IHelpInfo
    {
        public string Name => "SPAWNRULE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Creates a new spawn rule, modifying how players spawn at the start of the game. MUST BE USED BEFORE THE ROUND STARTS.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role to create the rule for.", true),
            new Argument("max", typeof(int), "The maximum amount of players to spawn as this role. If not provided, EVERY player who does not become a role with a different spawn rule will become this role. Variables & Math are supported.", false),
        };

        public ActionResponse Execute(Script scr)
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
