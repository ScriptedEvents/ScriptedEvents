namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class SetRoleAction : IScriptAction, IHelpInfo
    {
        public string Name => "SETROLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Sets all players to the given role.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to save as the new variable.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set all the players as.", true),
            new Argument("max", typeof(int), "The maximum amount of players to save in this variable. Variables & Math are supported. (default: unlimited).", false),
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!Enum.TryParse<RoleTypeId>(Arguments[1], true, out RoleTypeId roleType))
                return new(MessageType.InvalidRole, this, "role", Arguments[1]);

            int max = -1;

            if (Arguments.Length > 2)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

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

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out List<Player> plys))
                return new(false, "No players matching the criteria were found.");

            foreach (Player player in plys)
                player.Role.Set(roleType);

            return new(true);
        }
    }
}
