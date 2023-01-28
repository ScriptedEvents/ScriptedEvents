using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class SetRoleAction : IAction
    {
        public string Name => "SETROLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2)
            {
                return new(false, "Missing arguments: players, role, max(optional)");
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], out List<Player> players))
                return new(false, "No players matching the criteria were found.");

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

            if (max is -1)
            {
                foreach (Player player in players)
                    player.Role.Set(roleType);
            }
            else
            {
                players.ShuffleList();
                for (int i = 0; i < max; i++)
                {
                    Player item = players.ElementAtOrDefault(i);
                    item?.Role?.Set(roleType);
                }
            }

            return new(true);
        }
    }
}
