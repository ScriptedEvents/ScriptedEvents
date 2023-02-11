using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.Actions
{
    public class GiveAction : IScriptAction, IHelpInfo
    {
        public string Name => "GIVE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Gives the targeted players an item.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to give the item to.", true),
            new Argument("item", typeof(ItemType), "The item to give.", true),
            new Argument("amount", typeof(int), "The amount to give. Variables & Math are supported. Default: 1", false)
        };

        public ActionResponse Execute(Script script)
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
