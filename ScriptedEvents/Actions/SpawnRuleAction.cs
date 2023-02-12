namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

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
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, ExpectedArguments);

            if (!Enum.TryParse<RoleTypeId>(Arguments[0], true, out RoleTypeId roleType))
                return new(MessageType.InvalidRole, this, "spawnrole", Arguments[0]);

            int max = -1;

            if (Arguments.Length > 1)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(1)));

                if (!ConditionHelper.TryMath(formula, out MathResult result))
                {
                    return new(MessageType.NotANumberOrCondition, this, "max", formula, result);
                }

                if (result.Result < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "max", result.Result);
                }

                max = Mathf.RoundToInt(result.Result);
            }

            MainPlugin.Handlers.SpawnRules.Remove(roleType);
            MainPlugin.Handlers.SpawnRules.Add(roleType, max);

            return new(true);
        }
    }
}
