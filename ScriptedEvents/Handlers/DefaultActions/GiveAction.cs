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
    public class GiveAction : IAction
    {
        public string Name => "GIVE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2)
            {
                return new(false, "Missing arguments: players, item, amount(optional)");
            }

            if (!Enum.TryParse<ItemType>(Arguments[1], true, out ItemType itemType))
                return new(false, "Invalid item provided.");

            int amt = 1;

            if (Arguments.Length > 2)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

                try
                {
                    float maxFloat = (float)ConditionHelper.Math(formula);
                    if (maxFloat != (int)maxFloat)
                    {
                        amt = Mathf.RoundToInt(maxFloat);
                    }
                    else
                    {
                        amt = (int)maxFloat;
                    }
                }
                catch (Exception ex)
                {
                    return new(false, $"Invalid maximum condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
                }

                if (amt < 0)
                {
                    return new(false, "A negative number cannot be used as the amount argument of the GIVE action.");
                }
            }

            List<Player> plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out plys))
                return new(false, "No players matching the criteria were found.");

            foreach (Player player in plys)
            {
                for (int i = 0; i < amt; i++)
                {
                    player.AddItem(itemType);
                }
            }    

            return new(true);
        }
    }
}
