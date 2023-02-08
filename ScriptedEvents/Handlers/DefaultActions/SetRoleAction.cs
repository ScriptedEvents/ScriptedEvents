using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.Handlers.DefaultActions
{
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
            new Argument("max", typeof(int), "The maximum amount of players to save in this variable (default: unlimited).", false),
        };

        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 2)
            {
                return new(false, "Missing arguments: players, role, max(optional)");
            }

            if (!Enum.TryParse<RoleTypeId>(Arguments[1], true, out RoleTypeId roleType))
                return new(false, "Invalid role to spawn as provided.");

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
                    return new(false, "A negative number cannot be used as the max argument of the SETROLE action.");
                }
            }

            List<Player> plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out plys))
                return new(false, "No players matching the criteria were found.");

            foreach (Player player in plys)
                player.Role.Set(roleType);

            return new(true);
        }
    }
}
